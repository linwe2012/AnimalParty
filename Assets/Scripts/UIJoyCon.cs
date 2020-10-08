using Lean.Transition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

using System.Linq;

public class UIJoyCon : MonoBehaviour
{
    public GameObject Right;
    public GameObject Left;

    public GameObject ImBoat;
    public GameObject ImSLSR;

    public GameObject ImLeftSLSR;
    public GameObject RightPanel;
    public GameObject LeftPanel;

    public GameObject PlayerPanel;
    public GameObject PlayerModel;
    public GameObject LeftJoycon;
    public GameObject RightJoycon;
    public GameObject PlayerTitle;

    


    ExtendedGameObject extLeftJoycon;
    ExtendedGameObject extRightJoycon;
    ExtendedGameObject extPlayerModel;
    ExtendedGameObject extPlayerPanel;

    TextMeshProUGUI playerTitleText;

    Animator playerAnimator;
    System.Action terminateLastAnimate = () => { };
    System.Action doUpdate = () => { };


    public Vector3 BoatStep1 = new Vector3(110, -110, 120);
    public Vector3 BoatStep2Trans = new Vector3(-37, 10, -20);
    public Vector3 BoatLoop1 = new Vector3(-10, 0, -20);
    public Vector3 BoatLoop2 = new Vector3(-14, -18, -28);
    public Vector3 BoatLoop3 = new Vector3(-17, 0, -20);
    public Vector3 FetchJoyConFrom = new Vector3(8, 8, 8);

    // 0~3: 右手柄的 SL, SR, R, ZR
    // 4-7: 左手柄
#if UNITY_EDITOR
    [UIInternal.Inspector.NamedArray(new string[] { 
        "Right SL", "Right SR", "Right R", "Right ZR", 
        "Left SL", "Left SR", "Left L", "Left ZL" } )]
#endif
    SideButtonManager[] side_btn_managers = new SideButtonManager[8];

    [System.Serializable]
    public class MiddlePanelManager
    {
        public GameObject MiddlePanel;
        public GameObject CircleMinus;
        public GameObject ArrMinus;
        public GameObject Title;

        public Vector3 HideTrans = new Vector3(0, -600, 0);
        public Vector3 HideScale = new Vector3(0.2f, 0.2f, 0.2f);
        ExtendedGameObject2D extMiddlePanel;
        ExtendedGameObject extArrMinus;
        ExtendedGameObject2D extCircleMinus;
        TextMeshProUGUI textTitle;
        public void Init()
        {
            extMiddlePanel = new ExtendedGameObject2D(MiddlePanel);
            extArrMinus = new ExtendedGameObject(ArrMinus);
            extCircleMinus = new ExtendedGameObject2D(CircleMinus);
            textTitle = Title.GetComponent<TextMeshProUGUI>();

            extMiddlePanel.LocalTranslateFromStart(HideTrans);
            extMiddlePanel.LocalScaleMultiplyFromStart(HideScale);
            extMiddlePanel.SetActive(false);

            
        }

        public void ShowArrMinus(string title, int num_loops)
        {
            extMiddlePanel.SetActive(true);
            extMiddlePanel
                .LocalTranslateFromStartTransition(new Vector3(), 0.2f)
                .LocalScaleAdditionFromStartTransition(new Vector3(), 0.2f)
                .transform.EventTransition(
                () =>
                {
                    for (int i = 0; i < num_loops; ++i)
                    {
                        if(i != 0)
                        {
                            //extArrMinus.JoinTransition();
                            extCircleMinus.JoinTransition();
                        }
                        //extArrMinus
                        //    .LocalTranslateFromStartTransition(-5 * new Vector3(-1, -1, 0), 0.5f, LeanEase.Decelerate)
                        //    .JoinTransition()
                        //    .LocalTranslateFromStartTransition(new Vector3(), 0.5f, LeanEase.Decelerate);

                        extCircleMinus
                            .LocalScaleMultiplyFromStartTransition(new Vector3(1.5f, 1.5f, 1.5f), 0.5f, LeanEase.Accelerate)
                            .JoinTransition()
                            .LocalScaleAdditionFromStartTransition(new Vector3(), 0.5f, LeanEase.Decelerate);
                    }

                }, 0.2f);

            textTitle.text = title;
        }

        public void ShowArrMinusHide()
        {
            extMiddlePanel
                .LocalTranslateFromStartTransition(HideTrans, 0.2f)
                .LocalScaleMultiplyFromStartTransition(HideScale, 0.2f)
                .transform.EventTransition(
                () =>
                {
                    extMiddlePanel.SetActive(false);
                }, 0.2f);
        }

    }
    public MiddlePanelManager MiddlePanel;

    class ControllerManager
    {
        Vector3 lastRotate;
        Vector3 lastPosition;
        GameObject target;
        GameObject panel;

        public Vector3 lastPanelRotate;
        public Vector3 lastPanelPosition;
        public Vector3 lastPanelScale;

        

        public ControllerManager(GameObject obj, GameObject _panel)
        {
            target = obj;
            panel = _panel;

            lastRotate = obj.transform.localEulerAngles;
            lastPosition = obj.transform.localPosition;

            lastPanelRotate = _panel.transform.localEulerAngles;
            lastPanelPosition = _panel.transform.localPosition;
            lastPanelScale = _panel.transform.localScale;
            
            _panel.SetActive(false);
        }

        public void Restore()
        {
            target.transform.localEulerAngles = lastRotate;
            target.transform.localPosition = lastPosition;
        }
    }

    ControllerManager RightManager;
    ControllerManager LeftManager;




    class SideButtonManager
    {
        Vector3 translate_dir;
        GameObject arrow;
        UIJoyCon parent;
        Vector3 start_position;
        GameObject image_target;
        GameObject panel;

        public void Animate(int num_loops)
        {
            panel.SetActive(true);
            arrow.SetActive(true);

            if(image_target != null)
            {
                image_target.SetActive(true);
            }

            for (int i = 0; i < num_loops; ++i)
            {
                if(i != 0)
                {
                    arrow.transform.JoinTransition();
                }
                arrow.transform
                .localPositionTransition(start_position - 8 * translate_dir, 0.5f, LeanEase.Decelerate).JoinTransition()
                .localPositionTransition(start_position, 0.5f, LeanEase.Accelerate);
            }
            
            arrow.transform.EventTransition(() =>
            {
                arrow.SetActive(false); 
                if (image_target != null)
                {
                    image_target.SetActive(false);
                }
                panel.SetActive(false);
            }, num_loops);
        }

        public SideButtonManager(UIJoyCon _parent, GameObject obj_arrow, Vector3 trans, GameObject image, GameObject _panel)
        {
            parent = _parent;
            arrow = obj_arrow;
            translate_dir = trans.normalized;
            start_position = obj_arrow.transform.localPosition;
            image_target = image;
            panel = _panel;
        }
    }

    private void RestoreManagers()
    {
        RightManager.Restore();
        LeftManager.Restore();
    }

    public UIJoyCon ShowBoat(int num_loops = 6)
    {
        RestoreManagers();
        RightPanel.SetActive(true);
        ImBoat.SetActive(true);
        Right.transform.localRotationTransition(Quaternion.Euler(BoatStep1), 0.3f);
        
        for (int i = 0; i < num_loops; ++i)
        {
            Right.transform.JoinTransition()
            .localPositionTransition(BoatStep2Trans, 0.5f, LeanEase.Decelerate).JoinTransition()
            .localPositionTransition(BoatLoop1, 0.5f, LeanEase.Accelerate).JoinTransition()
            .localPositionTransition(BoatLoop2, 0.5f, LeanEase.Decelerate).JoinTransition()
            .localPositionTransition(BoatLoop3, 0.5f, LeanEase.Accelerate);
        }

        Right.transform.EventTransition(() =>
        {
            ImBoat.SetActive(false);
            RightPanel.SetActive(false);
        }, 2 * num_loops + 0.5f);

        return this;
    }

    /// <summary>
    /// 显示 SL, SR, Top, ZTop 四类按键
    /// </summary>
    /// <param name="buttons">
    ///     按键 ID, 如果没有说明默认展示 右手柄的用法, 可以通过 LeftJoycon & RightJoycon 限定使用那个演示    
    /// </param>
    /// <param name="num_loops">演示的循环次数</param>
    public UIJoyCon ShowClickSideButtons(UIManager.Button[] buttons, int num_loops = 6)
    {
        RestoreManagers();

        foreach(var button in buttons)
        {
            var btn = (int)(button & UIManager.Button.ClearLeftRight) - (int) UIManager.Button.SL;
            int specified_button = 0;

            if ((int)(button & UIManager.Button.RightJoycon) != 0)
            {
                side_btn_managers[btn].Animate(num_loops);
                specified_button = 1;
            }
            if((int)(button & UIManager.Button.LeftJoycon) != 0)
            {
                side_btn_managers[btn + 4].Animate(num_loops);
                specified_button = 1;
            }

            if(specified_button == 0)
            {
                side_btn_managers[btn].Animate(num_loops);
            }
        }

        return this;
    }

    public UIJoyCon MakeBigger(float delay, float transition_time = 0.2f)
    {
        if(RightPanel.activeSelf)
        {
            RightPanel.transform
                .localScaleTransition(RightManager.lastPanelScale * 2.0f, transition_time)
                .localPositionTransition(RightManager.lastPanelPosition + new Vector3(-40, -20, 0), transition_time);

            RightPanel.transform.EventTransition(
            () =>
            {
                RightPanel.transform
                .localScaleTransition(RightManager.lastPanelScale, transition_time)
                .localPositionTransition(RightManager.lastPanelPosition, transition_time);
            }, delay);
        }

        if (LeftPanel.activeSelf)
        {
            LeftPanel.transform
                .localScaleTransition(LeftManager.lastPanelScale * 2.0f, transition_time)
                .localPositionTransition(LeftManager.lastPanelPosition + new Vector3(40, -20, 0), transition_time);

            LeftPanel.transform.EventTransition(
            () =>
            {
                LeftPanel.transform
                .localScaleTransition(LeftManager.lastPanelScale, transition_time)
                .localPositionTransition(LeftManager.lastPanelPosition, transition_time);
            }, delay);
        }

        return this;
    }

    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title">显示在右上角的标题</param>
    public void ShowThrowFromBelow(string title)
    {
        ShowAnimation(title, "ThrowingFromBelow");
    }

    public void ShowThrowFromBelowHide()
    {
        ShowAnimationHide();
    }

    public void ShowZombieAttackHide()
    {
        ShowAnimationHide();
    }

    public void ShowZombieAttack(string title)
    {   
        ShowAnimation(title, "ZombieAttack");
    }

    float animationTime;
    public void ShowAnimation(string title, string animationFlag, bool hijackInput = false, float maximumTime = -1)
    {

        terminateLastAnimate();
        playerTitleText.text = title;
        PlayerPanel.SetActive(true);
        PlayerModel.SetActive(true);

        extPlayerPanel
            .LocalTranslateFromStart(new Vector3())
            .LocalScaleAdditionFromStart(new Vector3());


        extPlayerModel.transform.localEulerAngles = extPlayerModel.lastRotation + new Vector3(0, 40, 0);

        extRightJoycon
            .SetActive(true)
            .transform.localPosition = extRightJoycon.lastPosition + FetchJoyConFrom;

        extRightJoycon
            .transform.localRotation = Quaternion.Euler(
                extRightJoycon.lastRotation + new Vector3(-20, 40, 10)
                );

        extRightJoycon
            .transform.localScale = extRightJoycon.lastScale * 3;

        const float animation_time = 1.5f;

        playerAnimator.SetBool("PickingUpObject", true);
        //extPlayerModel
        //.LocalRotateFromStartTransition(new Vector3(), animation_time);

        extRightJoycon
            .LocalTranslateFromStartTransition(new Vector3(0, 0, 0), animation_time / 2, LeanEase.Smooth)
            .LocalRotateFromStartTransition(new Vector3(0, 0, 0), animation_time / 2, LeanEase.Smooth)
            .LocalScaleAdditionFromStartTransition(new Vector3(0, 0, 0), animation_time / 2, LeanEase.Smooth)

            .SetTimeOut(
            () =>
            {
                playerAnimator.SetBool("PickingUpObject", false);
                playerAnimator.SetBool(animationFlag, true);
            }, animation_time);

        animationTime = 0;

        if (hijackInput)
        {
            UIManager.HijackInput();
        }

        terminateLastAnimate = () =>
        {
            playerAnimator.SetBool(animationFlag, false);
            extRightJoycon.SetActive(false);
            if (hijackInput)
            {
                UIManager.ReturnInputControl();
            }
        };

        doUpdate = () =>
        {
            using (var state = UIManager.EnterUIState())
            {
                if (hijackInput && JoyconInfo.joyconL.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
                {
                    ShowAnimationHide();
                    return;
                }
                animationTime += Time.deltaTime;
                if(maximumTime > 0 && animationTime > maximumTime)
                {
                    ShowAnimationHide();
                    return;
                }

                var rotate = Vector2.Dot(
                JoyconInfo.rightStick,
                new Vector2(1, 0));

                rotate *= UIManager.Settings.rotateUIHumanoidSpeed;
                rotate *= Time.deltaTime;

                var euler = extPlayerModel.transform.localRotation.eulerAngles;
                euler.y += rotate;
                extPlayerModel.transform.localRotation = Quaternion.Euler(euler);
            }
        };
    }

    public void ShowAnimationHide()
    {
        float animate_time = 0.3f;

        terminateLastAnimate();
        terminateLastAnimate = () => { };
        doUpdate = () => { };

        extPlayerPanel
            .LocalScaleMultiplyFromStartTransition(new Vector3(0.2f, 0.2f, 0.2f), animate_time)
            .LocalTranslateFromStartTransition(new Vector3(0, -400, 0), animate_time)
            .transform.EventTransition(() =>
            {
                extPlayerPanel.SetActive(false);
            }, animate_time);

        PlayerModel.SetActive(false);
    }

    public void ShowPettingAnimalHide()
    {
        ShowThrowFromBelowHide();
    }

    public void ShowPettingAnimal(string title)
    {
        ShowAnimation(title, "PettingAnimal");
    }


    bool hasBeenInit = false;
    public void Init()
    {
        if (hasBeenInit)
        {
            return;
        }
        hasBeenInit = true;

        System.Action<GameObject, string, int, Vector3, GameObject> create_arrow =
            (GameObject target, string name, int idx, Vector3 direction, GameObject image_target) =>
            {
                var obj = target.transform.Find(name).gameObject;
                side_btn_managers[idx] = new SideButtonManager(this, obj, direction, image_target, target);
            };


        create_arrow(RightPanel, "ArrSL", 0, new Vector3(1, 0, 0), ImSLSR);
        create_arrow(RightPanel, "ArrSR", 1, new Vector3(1, 0, 0), ImSLSR);
        create_arrow(RightPanel, "ArrR", 2, new Vector3(-1, -1, 0), null);
        create_arrow(RightPanel, "ArrZR", 3, new Vector3(0, -1, 0), null);

        create_arrow(LeftPanel, "ArrSL", 4, -new Vector3(1, 0, 0), ImLeftSLSR);
        create_arrow(LeftPanel, "ArrSR", 5, -new Vector3(1, 0, 0), ImLeftSLSR);
        create_arrow(LeftPanel, "ArrL", 6, new Vector3(1, -1, 0), null);
        create_arrow(LeftPanel, "ArrZL", 7, new Vector3(0, -1, 0), null);


        RightManager = new ControllerManager(Right, RightPanel);
        LeftManager = new ControllerManager(Left, LeftPanel);

        playerAnimator = PlayerModel.GetComponent<Animator>();
        extLeftJoycon = new ExtendedGameObject(LeftJoycon);
        extRightJoycon = new ExtendedGameObject(RightJoycon);
        extPlayerModel = new ExtendedGameObject(PlayerModel);
        extPlayerPanel = new ExtendedGameObject(PlayerPanel);

        playerTitleText = PlayerTitle.GetComponent<TextMeshProUGUI>();

        extLeftJoycon.SetActive(false);
        extRightJoycon.SetActive(false);

        PlayerPanel.SetActive(false);
        PlayerModel.SetActive(false);
        MiddlePanel.Init();
    }


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public void ShowMinus(string title, int num_loops = 6)
    {
        MiddlePanel.ShowArrMinus(title, num_loops);
    }

    public void ShowMinusHide()
    {
        MiddlePanel.ShowArrMinusHide();
    }

    // Update is called once per frame
    void Update()
    {
        doUpdate();
    }
}
