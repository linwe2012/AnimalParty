using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIInternal
{
    [System.Serializable]
    public class SystemManager
    {
        public GameObject GroupOfButtons;
        public GameObject ActiveUpperPanel;
        public GameObject ActiveLowerPanel;

        VerticleButtonsManager buttonsManager;
        UIInventory inventoryManager;

        System.Action[] clickActions;
        System.Action doUpdate;
        System.Action onDoneCallback;

        // 允许输入交给 UIIventory 进一步处理
        [HideInInspector]
        public bool InputBubbleUp = true;

        int currentSelectedAction = 0;
        bool isShowing = false;

        System.Action lazyInit;
        int updateCount = 1;

        int ClampActions(int index)
        {
            if (index < 0) return 0;
            if (index >= clickActions.Length) return clickActions.Length - 1;
            return index;
        }

        public void Init(UIInventory inventoryManager_)
        {
            inventoryManager = inventoryManager_;

            lazyInit = () =>
            {
                if(updateCount == 1)
                {
                    updateCount = 0;
                    return;
                }
                lazyInit = null;

                buttonsManager = new VerticleButtonsManager();
                buttonsManager.Init(
                    ActiveUpperPanel,
                    ActiveLowerPanel,
                    GroupOfButtons,
                    0,
                    1.2f,
                    AnimationCurve.EaseInOut(0, 0, 1, 1),
                    0.2f
                    );

                clickActions = new System.Action[]
                {
                ClickChooseMission,
                ClickQuit
                };
                doUpdate = UpdateSystem;

                UIManager.PrepareSticks();
                buttonsManager.Show(0);
            };
        }

        public void Show(System.Action onDoneCallback_)
        {
            if(isShowing)
            {
                return;
            }

            InputBubbleUp = true;
            onDoneCallback = () =>
            {
                UIManager.ReturnInputControl();
                isShowing = false;
                onDoneCallback_();
            };

            if (lazyInit != null)
            {
                doUpdate = lazyInit;
                return;
            }
            else
            {
                doUpdate = UpdateSystem;
            }

            UIManager.PrepareSticks();
            buttonsManager.Show(0);
        }

        public void Update()
        {
            doUpdate();
        }

        void UpdateSystem()
        {
            if(JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
            {
                clickActions[currentSelectedAction]();
            }
            else if(JoyconInfo.joyconR.GetButton(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI))
            {
                onDoneCallback();
            }
            UIManager.CalculateSticks();

            var move = UIManager.RightStickMove;
            var select = -1;
            if(move == UIManager.StickMoveTo.Down)
            {
                select = ClampActions(currentSelectedAction + 1);
            }
            else if(move == UIManager.StickMoveTo.Up)
            {
                select = ClampActions(currentSelectedAction - 1);
            }
            if(select >= 0 && select != currentSelectedAction)
            {
                buttonsManager.MoveToIndex(select);
                currentSelectedAction = select;
            }
            buttonsManager.Update();
        }

        void UpdateMission()
        {
            inventoryManager.selectMission.Update();
        }
        


        void ClickQuit()
        {
            Application.Quit();
        }

        void ClickChooseMission()
        {
            InputBubbleUp = false;
            // UIManager.ReturnInputControl();
            inventoryManager.PanelInventory.SetActive(false);
            inventoryManager.selectMission.Show((select)=>
            {
                if(select < 0)
                {
                    inventoryManager.PanelInventory.SetActive(true);
                    doUpdate = UpdateSystem;
                    InputBubbleUp = true;
                }
                else
                {
                    doUpdate = () =>
                    {
                        doUpdate = () =>
                        {
                            onDoneCallback();
                            UIManager.LoadMission(select + 2);
                        };
                    };
                    
                }
            });

            doUpdate = () =>
            {
                doUpdate = () =>
                {
                    doUpdate = UpdateMission;
                };
            };
        }
    }
}
