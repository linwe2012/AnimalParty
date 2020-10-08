using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [System.Flags]
    public enum Button
    {
        B,
        A,
        Y,
        X,
        // 默认是任意左手柄或者右手柄都可以
        SL,
        SR,
        Top, // R, L
        ZTop, // ZR, ZL

        LeftJoycon = 0x10000,
        RightJoycon = 0x20000,
        ClearLeftRight = LeftJoycon - 1,
    }

    // public static List<System.Action> FirstUpdate = new List<Action>();

    public enum StickMoveTo
    {
        None,
        Left,
        Right,
        Down,
        Up
    }

    public enum DialogAt
    {
        Bottom
    }

    public class ScopedState : IDisposable
    {
        public ManagedInput.Viewer lastView;

        public void Dispose()
        {
            JoyconInfo.viewer = lastView;
        }
    }

    public static void HijackInput()
    {
        JoyconInfo.Blind(ManagedInput.Viewer.Default, true);
    }

    public static void ReturnInputControl()
    {
        JoyconInfo.Blind(ManagedInput.Viewer.Default, false);
    }

    public static ScopedState EnterUIState(bool uiInputView = true)
    {
        ScopedState state = new ScopedState();
        state.lastView = JoyconInfo.viewer;

        if (uiInputView)
        {
            JoyconInfo.viewer = ManagedInput.Viewer.UI;
        }

        return state;
    }

    static public UIManager global = null;

    // 二维的 UI 内容
    public static UIHUD HUD { get { return global.hud; } }

    // 和 3D 有关的 UI
    public static UIJoyCon Joycon { get { return global.jc; } }

    public static UIInventory Inventory { get { return global.invent; } }


    [Obsolete("请使用 UIManager.HUD 的方式访问")]
    public UIHUD HelpHUD { get { return hud; } }

    [Obsolete("请使用 UIManager.Joycon 的方式访问")]
    public UIJoyCon HelpJoycon { get { return jc; } }

    [Obsolete("请使用 UIManager.Inventory 的方式访问")]
    public UIInventory HelpInventory { get { return invent; } }

    UIHUD hud;
    UIJoyCon jc;
    UIInventory invent;

    [System.Serializable]
    public struct Settings
    {
        public static float startMenuSelectInterval = 0.2f;
        public static float rotateUIHumanoidSpeed = 120f;

        public static float inventoryStickSensitivity = 0.3f;
        public static float hintStayingTime = 5.8f;
    };
    //static public Settings settings;

    public UISettings settings;
    // Start is called before the first frame update

    bool hasInited = false;

    public static void Init()
    {
        var gameObj = GameObject.FindObjectOfType<UIManager>();
        gameObj.DoInit();
    }

    // 在 Start 的时候调用 UIManager 请用 Init() 函数
    // 保证其已经初始化完毕
    public void DoInit()
    {
        if (hasInited)
        {
            return;
        }

        hasInited = true;

        global = this;


        hud = gameObject.GetComponent<UIHUD>();
        jc = gameObject.GetComponent<UIJoyCon>();
        invent = gameObject.GetComponent<UIInventory>();

        hud.Init();
        jc.Init();
        invent.Init();
    }

    void Start()
    {
        Init();
    }




    public static StickMoveTo EvaluateStickMove(Vector2 stick)
    {
        StickMoveTo key = StickMoveTo.None;

        var ldotY = Vector2.Dot(stick, new Vector2(0, 1));
        var ldotX = Vector2.Dot(stick, new Vector2(1, 0));

        var absldotY = Mathf.Abs(ldotY);
        var absldotX = Mathf.Abs(ldotX);

        if (absldotX < UIManager.Settings.inventoryStickSensitivity
            && absldotY < UIManager.Settings.inventoryStickSensitivity)
        {
            return key;
        }

        if (Math.Abs(absldotX - absldotY) < UIManager.Settings.inventoryStickSensitivity * 0.3f)
        {
            return key;
        }

        if (absldotY > absldotX)
        {
            if (ldotY > 0)
            {
                key = StickMoveTo.Up;
            }
            else
            {
                key = StickMoveTo.Down;
            }
        }
        else
        {
            if (ldotX > 0)
            {
                key = StickMoveTo.Right;
            }
            else
            {
                key = StickMoveTo.Left;
            }
        }
        return key;
    }

    public static StickMoveTo LeftStickMove = StickMoveTo.None;

    public static StickMoveTo RightStickMove = StickMoveTo.None;


    static float stickTime = 0;
    static bool allowStick = true;

    public static void PrepareSticks()
    {
        stickTime = 0;
        allowStick = true;
    }

    // Update is called once per frame
    public static void CalculateSticks()
    {
        LeftStickMove = StickMoveTo.None;
        RightStickMove = StickMoveTo.None;

        if (allowStick)
        {
            LeftStickMove = EvaluateStickMove(JoyconInfo.joyconL.GetStickAsVector2(ManagedInput.Viewer.UI));
            RightStickMove = EvaluateStickMove(JoyconInfo.joyconR.GetStickAsVector2(ManagedInput.Viewer.UI));

            allowStick = false;
            stickTime = 0;
        }
        else
        {
            stickTime += Time.unscaledDeltaTime;
            if (stickTime > Settings.startMenuSelectInterval)
            {
                stickTime = 0;
                allowStick = true;
            }
        }
    }

    // int loopCnt = 0;
    //private void Update()
    //{
    //    if(loopCnt == 0)
    //    {
    //        loopCnt = 1;
    //        return;
    //    }
    //    if(FirstUpdate != null)
    //    {
    //        foreach(var fn in FirstUpdate)
    //        {
    //            fn();
    //        }
    //        FirstUpdate = null;
    //    }
    //}


    static string[] MessageForEachMission =
    {
        "Loading...",
        "新手教程: 回到自己的家, 看看自己的狗",
        "带领受伤大象吃掉尽可能多的野果, 博取好感, 然后带着大象走到终点",
        "猩猩暴走啦, 喂猩猩它想要的食物, 使它安定下来",
        "使用 刚刚好 的力度钓上鱼",
        "养在动物园的企鹅已经忘记怎么捕食了, 请你喂给企鹅尽可能多的鱼",
        "疯狂转动手柄, 给小鸡喂食"
    };


    public static void LoadMission(int index)
    {
        if (MissionIndex == index)
        {
            return;
        }

        UIManager.HUD.ShowLoading(MessageForEachMission[index]);
        UIManager.global.StartCoroutine(global.LoadMissionImpl(index));
    }

    IEnumerator LoadMissionImpl(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        // operation.allowSceneActivation = false;
        //UIManager.HUD.ShowLoadingValue(0.8f);
        //for(int i = 0; i < 20; ++i)
        //{
        //    UIManager.HUD.loading.Update();
        //    Thread.Sleep(10);
        //}
        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            UIManager.HUD.ShowLoadingValue(progress);
            UIManager.HUD.loading.Update();

            if (operation.progress >= 0.85f)
            {
                UIManager.HUD.ShowLoadingHide();
                // operation.allowSceneActivation = true;
            }
                
            yield return null;
        }
    }

    public static int MissionIndex
    {
        get { return SceneManager.GetActiveScene().buildIndex; }
    }

}
