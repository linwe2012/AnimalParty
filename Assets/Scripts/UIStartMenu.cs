using Lean.Transition;
using TMPro;
using UnityEngine;



public class UIStartMenu : MonoBehaviour
{

    public GameObject[] Buttons = new GameObject[3];
    public GameObject UIObjSettings;
    public GameObject UIObjStartMenu;
    public GameObject UIObjOpeningScene;

    ExtendedGameObject2D MyUIObjStartMenu;

    public int currentSelection = -1;

    float lastSelectTime = -1;
    bool lastUpdateSelected = false;

    class ButtonInfo
    {
        public TextMeshProUGUI text;
        public Vector3 lastPosition;
        public Vector3 lastScale;

        GameObject button;

        public Transform transform
        {
            get { return button.transform; }
        }

        public ButtonInfo(GameObject _button)
        {
            lastPosition = _button.transform.localPosition;
            lastScale = _button.transform.localScale;
            text = _button.GetComponent<TextMeshProUGUI>();
            button = _button;
        }

        public ButtonInfo LocalTranslateFromStartTransition(Vector3 direction, float duration)
        {
            text.rectTransform.localPositionTransition(lastPosition + direction, duration);
            // button.transform.localPositionTransition(lastPosition + direction, duration);

            return this;
        }

        public ButtonInfo LocalScaleFromStartTransition(Vector3 new_scale, float duration)
        {
            text.rectTransform.localScaleTransition(lastScale.ElementWiseMul(new_scale), duration);
            // button.transform.localScaleTransition(lastScale.ElementWiseMul(new_scale), duration);

            return this;
        }

        public ButtonInfo JoinTransistion()
        {
            button.transform.JoinTransition();
            return this;
        }
    }



    ButtonInfo[] infos = new ButtonInfo[3];

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (var btn in Buttons)
        {
            infos[i] = new ButtonInfo(btn);
            ++i;
        }
        lastSelectTime = Time.time;
        MyUIObjStartMenu = new ExtendedGameObject2D(UIObjStartMenu.transform.Find("Panel").gameObject);
        MyUIObjStartMenu.SetActive(false);
    }

    public void Show()
    {
        MyUIObjStartMenu.LocalTranslateFromStart(new Vector3());
        MyUIObjStartMenu.SetActive(true);
    }

    void RenderSelect(int last_id, int id, float duration)
    {

        if (last_id >= 0)
        {
            infos[last_id].LocalScaleFromStartTransition(new Vector3(1f, 1f, 1f), duration)
            .text.colorTransition(new Color(1.0f, 1.0f, 1.0f, 0.5f), duration);
        }

        for (int i = 0; i < id; ++i)
        {
            infos[i].LocalTranslateFromStartTransition(new Vector3(0, 5, 0), duration);
            // infos[i].text.colorTransition
        }

        if (id >= 0)
        {
            infos[id]
            .LocalTranslateFromStartTransition(new Vector3(0, 0, 0), duration)
            .LocalScaleFromStartTransition(new Vector3(1.5f, 1.5f, 1.5f), duration)
            .text.colorTransition(new Color(1.0f, 1.0f, 1.0f, 1.0f), duration);
        }


        for (int i = id + 1; i < Buttons.Length; ++i)
        {
            infos[i].LocalTranslateFromStartTransition(new Vector3(0, -5, 0), duration);
        }
    }

    public void HideUI()
    {
        /*
        MyUIObjStartMenu.LocalTranslateFromStartTransition(
            new Vector3(600, 0, 0),
            0.3f,
            LeanEase.Accelerate
            );*/
        MyUIObjStartMenu.LocalTranslateFromStart(
            new Vector3(600, 0, 0));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // UIObjOpeningScene.transform.Rotate(new Vector3(0, 4 * Time.deltaTime, 0));

        System.Action<int> next = (int inc) =>
        {
            int x = currentSelection + inc;
            if (x < 0 || x >= Buttons.Length)
            {
                return;
            }
            RenderSelect(currentSelection, x, 0.2f);
            currentSelection = x;
        };

        if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
        {
            if(currentSelection >= 0)
            {
                HideUI();
            }
            //Debug.Log("Clicked A button");
            
            return;
        }

        if (lastUpdateSelected)
        {
            if (Time.time - lastSelectTime < UIManager.Settings.startMenuSelectInterval)
            {
                return;
            }
            lastUpdateSelected = false;
            lastSelectTime = Time.time;
        }

        var info = JoyconInfo.rightStick;

        if (info.y > 0)
        {
            lastUpdateSelected = true;
            Debug.Log("joycon up");
            next(-1);
        }
        else if (info.y < 0)
        {
            lastUpdateSelected = true;
            Debug.Log("joycon down");
            next(1);
        }
        else
        {
            lastUpdateSelected = false;
        }
        lastSelectTime = Time.time;


    }
}
