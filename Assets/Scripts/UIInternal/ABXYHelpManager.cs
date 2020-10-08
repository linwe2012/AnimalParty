using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Transition;
using TMPro;

namespace UIInternal
{
    class ABXYHelpItem
    {
        UIManager.Button button;
        GameObject buttonOn;
        GameObject buttonOff;
        ABXYHelpManager manager;

        Image imTapped;
        Image imNotTapped;

        TextMeshProUGUI tmpHelpText;

        static Color PureWhite = new Color(1, 1, 1, 1);
        static Color TransWhite = new Color(1, 1, 1, 0);

        ExtendedGameObject2D extButtonOn;

        public bool Activated
        {
            get { return activated; }
            set
            {
                if (value) Activate();
                else Deactivate();
                activated = value;
            }
        }

        public string Message
        {
            set { SetMessage(value); }
        }

        bool activated;

        public bool isDown;
        public ABXYHelpItem(UIManager.Button button_, GameObject buttonOff_, GameObject buttonOn_, ABXYHelpManager manager_)
        {
            button = button_;
            buttonOn = buttonOn_;
            buttonOff = buttonOff_;

            imTapped = buttonOn.transform.GetChild(1).gameObject.GetComponent<Image>();
            imNotTapped = buttonOn.transform.GetChild(0).gameObject.GetComponent<Image>();
            tmpHelpText = buttonOn.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

            Deactivate();
            activated = false;
            manager = manager_;
            extButtonOn = new ExtendedGameObject2D(buttonOn);
            isDown = false;
        }

        void SetMessage(string msg)
        {
            isDown = false;
            tmpHelpText.text = msg;
        }

        

        void Deactivate()
        {
            buttonOff.SetActive(true);
            buttonOn.SetActive(false);
        }

        void Activate()
        {
            isDown = false;

            buttonOff.SetActive(false);
            buttonOn.SetActive(true);

            imTapped.color = TransWhite;
            imNotTapped.color = PureWhite;
        }

        public bool Update()
        {
            float t = manager.heartBeatPortion;

            extButtonOn.LocalScaleMultiplyFromStart(
                new Vector3(1, 1, 1) + new Vector3(t, t, t));
            extButtonOn.LocalTranslateFromStart(
                 new Vector3(t * 40, 0, 0));

            if (JoyconInfo.joyconR.GetButtonDown((Joycon.Button)button))
            {
                isDown = true;
                imTapped.colorTransition(PureWhite, 0.1f);
                imNotTapped.colorTransition(TransWhite, 0.1f);
            }
            else if(JoyconInfo.joyconR.GetButtonUp((Joycon.Button)button))
            {
                imTapped.colorTransition(TransWhite, 0.1f);
                imNotTapped.colorTransition(PureWhite, 0.1f);
                isDown = false;
                return true;
            }
            return false;
        }
    }

    
    [System.Serializable]
    public class ABXYHelpManager
    {
        float currentTimeForHelpHeartbeat;
        public AnimationCurve EaseBouncingHelp;
        public float totalTimeForHeartbeat = 1.0f;
        public GameObject PanelHelpABXY;

        ExtendedGameObject2D extPanelHelpABXY;
        ABXYHelpItem[] buttons = new ABXYHelpItem[4];
        System.Action onTapped;
        ABXYHelpItem currentButton;

        bool isShowing = false;

        public float heartBeatPortion
        {
            get { return theHeartbeatPartion; }
        }
        float theHeartbeatPartion;


        // Start is called before the first frame update
        public void Init()
        {
            UIManager.Button[] btnId = new UIManager.Button[]
            {
                UIManager.Button.A,
                UIManager.Button.B,

                UIManager.Button.X,
                UIManager.Button.Y,
               
            };

            var paren = PanelHelpABXY.transform;
            for (int i = 0; i < 4; ++i)
            {
                int k = (int)btnId[i];
                buttons[k] = new ABXYHelpItem((UIManager.Button) k, paren.GetChild(i).gameObject, paren.GetChild(i + 4).gameObject, this);
            }

            onTapped = () =>
            {
                

            };
            extPanelHelpABXY = new ExtendedGameObject2D(PanelHelpABXY);
            PanelHelpABXY.SetActive(false);
            isShowing = false;
        }

        void DeactivateButtons()
        {
            foreach(var btn in buttons)
            {
                btn.Activated = false;
            }
        }

        bool shouldClose;

        public void MarkShouldClose()
        {
            shouldClose = true;
        }

        public void Show(UIManager.Button button, string message, System.Action onTapped_)
        {
            if (isShowing)
            {
                return;
            }
            shouldClose = false;
            isShowing = true;
            onTapped = () =>
            {
                isShowing = false;
                onTapped_();
            };

            PanelHelpABXY.SetActive(true);
            currentButton = buttons[(int)button];

            currentButton.Activated = true;
            currentButton.Message = message;
            currentTimeForHelpHeartbeat = 0;
        }

        // Update is called once per frame
        public void Update()
        {
            currentTimeForHelpHeartbeat += Time.deltaTime;
            if (currentTimeForHelpHeartbeat > totalTimeForHeartbeat)
            {
                currentTimeForHelpHeartbeat -= totalTimeForHeartbeat;
            }
            float t = EaseBouncingHelp.Evaluate(currentTimeForHelpHeartbeat / totalTimeForHeartbeat);

            if (currentButton.Update())
            {
                DeactivateButtons();
                PanelHelpABXY.SetActive(false);
                onTapped();
                return;
            }
            else if (!currentButton.isDown && shouldClose)
            {
                DeactivateButtons();
                PanelHelpABXY.SetActive(false);
                onTapped();
            }

            
            theHeartbeatPartion = t;
            extPanelHelpABXY.LocalScaleMultiplyFromStart(
                new Vector3(1, 1, 1) + new Vector3(t, t, t));
        }
    }
}

