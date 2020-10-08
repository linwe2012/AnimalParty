using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using Boo.Lang;

namespace UIInternal
{
    [System.Serializable]
    public class SelectMission
    {
        VerticleButtonsManager verticleButtons;

        public GameObject PanelSelectMission;
        public GameObject ActiveUpperPanel;
        public GameObject ActiveLowerPanel;
        public GameObject ButtonsParent;
        public GameObject PreviewsObject;

        [Tooltip("当前选中的'按钮指示'所在的按钮的 index, 用于定位按钮指示")]
        public int indicatorAt = 1;
        public float bounceTime = 0.8f;
        public AnimationCurve CurveMove;
        public int selectIndex = 0;
        public float moveTime = 0.2f;

        System.Action<int> onSelect;

        //List<GameObject> previews;
        List<ExtendedGameObject> extPreviews;

        float rotation = 0;
        bool setUpBefore = false;
        public void Init()
        {
            
            PanelSelectMission.SetActive(false);
            PreviewsObject.SetActive(false);
        }

        void Setup()
        {
            verticleButtons = new VerticleButtonsManager();

            verticleButtons.Init(ActiveUpperPanel, ActiveLowerPanel, ButtonsParent, indicatorAt, bounceTime, CurveMove, moveTime);


            //previews = new List<GameObject>();
            extPreviews = new List<ExtendedGameObject>();

            int n_previews = PreviewsObject.transform.childCount;
            for (int i = 0; i < n_previews; ++i)
            {
                var obj = PreviewsObject.transform.GetChild(i).gameObject;
                //previews.Add(obj);
                obj.SetActive(false);
                extPreviews.Add(new ExtendedGameObject(obj));
            }
        }

        int skipCount = 1;
        public void Update()
        {
            if (skipCount >= 0)
            {
                skipCount--;
            }

            if (!setUpBefore)
            {

                Setup();
                DoShow();
                setUpBefore = true;
                skipCount = 0;
                return;
            }

            if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
            {
                onSelect(selectIndex);
                return;
            }
            else if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI))
            {
                onSelect(-1);
                return;
            }


            UIManager.CalculateSticks();

            var move = UIManager.RightStickMove;
            int select = -1;
            if (move == UIManager.StickMoveTo.Down)
            {
                select = verticleButtons.ClampIndex(selectIndex + 1);
            }
            else if(move == UIManager.StickMoveTo.Up)
            {
                select = verticleButtons.ClampIndex(selectIndex - 1);
            }

            if(select >= 0 && select != selectIndex)
            {
                verticleButtons.MoveToIndex(select);
                
                extPreviews[selectIndex].SetActive(false);
                extPreviews[select].SetActive(true);
                selectIndex = select;
            }

            rotation += Time.unscaledDeltaTime * 10;
            rotation %= 360;

            extPreviews[selectIndex].LocalRotateFromStart(new Vector3(0, -rotation, 0));

            verticleButtons.Update();
        }

        void DoShow()
        {
            verticleButtons.Show(0);
            UIManager.PrepareSticks();
            extPreviews[0].SetActive(true);
            PreviewsObject.SetActive(true);
            skipCount = 2;
        }

        public void Show(System.Action<int> onSelect_)
        {
            onSelect = (index) =>
            {
                Hide();
                onSelect_(index);
            };

            PanelSelectMission.SetActive(true);

            if(setUpBefore)
            {
                DoShow();
            }
        }

        public void Hide()
        {
            PanelSelectMission.SetActive(false);
        }
    }
}
