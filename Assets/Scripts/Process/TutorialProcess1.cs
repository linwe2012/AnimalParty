using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialProcess1 : MonoBehaviour
{
    enum Process : int
    {
        IDLE = 0,
        PLAYER_MOVE = 1,
        CAMERA_MOVE = 2,
        PICK_UP = 3,
        FEEDING = 4,
        PETTING = 5,
        SHIFT = 6,
        CLEAR = 7,

        NumberOfActions
    }
    // 判断是否已经完成了这个process
    public BitArray processState = new BitArray((int)Process.NumberOfActions);

    //public UIManager ui;
    public GameObject player;
    public GameObject camera;
    public GameObject bug_dog;


    private bool conversationProcessing = false;
    private bool showingConversation = true;
    private int conversationIndex = 0;
    private string[] conversation =
    {
        "   欢迎您游玩 Animal Party! 在这里您可以遇到一堆奇奇怪怪的动物~~~~",
        "   首先, 您需要熟悉一下Joycon的基本操作",
        "",
        "   恭喜您已经掌握了Joycon的基本使用方法! 现在您可以随便走走~~~~~~",
        "",
        "   哦呀, 这里有根火腿肠, 不妨尝试捡起来? ",
        "",
        "   哦呀呀, 您家的狗狗来了. 看来它一直在帮您看家呢. 把你手里的火腿肠喂给他吧! ",
        "",
        "   棒极了! 现在您尝试看看Joycon的动作吧, 请模仿画面上的人物做做动作看.",
        "",
        "   接下来你可以试试按下左摇杆的减号键, 可以切换第一人称与第三人称视角",
        "",
        "   恭喜您已经完成了所有的新手教程! 点击右手柄X键, 可以正式进入游戏! "
    };

    private Collider[] cols;
    //private bool pettingFlag = false;
    private Process nowProcess;
    private Vector3 originPos;

    private PlayerController.CameraState cameraState;
    private Vector3 originCameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        processState.SetAll(false);
        processState.Set((int)Process.IDLE, true);
        nowProcess = Process.PLAYER_MOVE;
        originPos = player.transform.position;
        conversationProcessing = true;
    }

    void LateUpdate()
    {
        if (conversationProcessing)
        {
            player.GetComponent<PlayerController>().state = PlayerController.State.UI;
            player.GetComponent<PlayerAction>().state = PlayerAction.State.UI;
            if (conversationIndex >= conversation.Length)
            {
                conversationProcessing = false;
            }
            if (showingConversation)// 接收到了展示的指令
            {
                if(conversationIndex < conversation.Length)
                {
                    string converse = conversation[conversationIndex];
                    if (converse == "")
                    {
                        conversationProcessing = false;
                    }
                    else
                    {
                        UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, conversation[conversationIndex], "");
                    }
                }
                showingConversation = false;
            }
            else
            {
                if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                {
                    UIManager.HUD.ShowDialogHide();
                    showingConversation = true;
                    conversationIndex++;
                }
            }
        }
        else
        {
            player.GetComponent<PlayerController>().state = PlayerController.State.PLAY;
            player.GetComponent<PlayerAction>().state = PlayerAction.State.PLAY;
            switch (nowProcess)
            {
                case Process.IDLE:
                    UIManager.HUD.ShowHelpHide();
                    if (processState.Get((int)Process.PICK_UP) == false)
                    {
                        cols = Physics.OverlapSphere(player.transform.position, 6);
                        for (int i = 0; i < cols.Length; i++)
                            if (cols[i].gameObject.GetComponent<ItemLock>() != null)
                            {
                                if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                                {
                                    if(Vector3.Distance(cols[i].gameObject.transform.position,player.transform.position) < 3)
                                        nowProcess = Process.PICK_UP;
                                    if (conversationIndex <= 5)
                                    {
                                        conversationProcessing = true;
                                        showingConversation = true;
                                        conversationIndex++;
                                    }
                                }
                            }
                    }
                    else if (processState.Get((int)Process.FEEDING) == true && !processState.Get((int)Process.PETTING))
                    {
                        nowProcess = Process.PETTING;
                        conversationProcessing = true;
                        showingConversation = true;
                        conversationIndex++;
                    }
                    else if(processState.Get((int)Process.SHIFT))
                    {

                        nowProcess = Process.CLEAR;
                        conversationProcessing = true;
                        showingConversation = true;
                        conversationIndex++;
                    }
                    break;
                case Process.PLAYER_MOVE:

                    if (!processState.Get((int)Process.PLAYER_MOVE))
                    {
                        UIManager.HUD.ShowJoyconStick("左 摇杆移动人物");
                    }
                    if (Vector3.Distance(player.transform.position, originPos) > 6.0f
                        || JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                    {
                        //Debug.Log("PLAYER_MOVE");
                        processState.Set((int)Process.PLAYER_MOVE, true);
                        nowProcess = Process.CAMERA_MOVE;
                        originCameraRotation = camera.transform.localEulerAngles;
                    }
                    break;
                case Process.CAMERA_MOVE:
                    if (!processState.Get((int)Process.CAMERA_MOVE))
                        UIManager.HUD.ShowJoyconStick("右 摇杆移动视角");
                    if (Vector3.Distance(camera.transform.localEulerAngles, originCameraRotation) > 240.0f
                        || JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                    {
                        //Debug.Log("CAMERA_MOVE");
                        processState.Set((int)Process.CAMERA_MOVE, true);
                        nowProcess = Process.IDLE;
                        UIManager.HUD.ShowJoyconStickHide();

                        conversationProcessing = true;
                        showingConversation = true;
                        conversationIndex++;
                    }
                    break;
                case Process.PICK_UP:
                    if (!processState.Get((int)Process.PICK_UP))
                    {
                        if (conversationIndex > 5)
                            UIManager.HUD.ShowHelp(UIManager.Button.A, "拾取香肠");
                    }

                    cols = Physics.OverlapSphere(player.transform.position, 3);
                    nowProcess = Process.IDLE;
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if (cols[i].gameObject.GetComponent<ItemLock>() != null)
                        {
                            if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                            {
                                nowProcess = Process.PICK_UP;
                            }
                        }
                    }
                    if (player.GetComponent<PlayerAction>().isHolding())
                    {
                        Debug.Log("PICK_UP");
                        processState.Set((int)Process.PICK_UP, true);
                        nowProcess = Process.FEEDING;
                        UIManager.HUD.ShowHelpHide();

                        conversationProcessing = true;
                        showingConversation = true;
                        conversationIndex++;
                    }
                    break;
                case Process.FEEDING:
                    if (!processState.Get((int)Process.FEEDING))
                    {
                        if (conversationIndex > 7)
                            UIManager.HUD.ShowHelp(UIManager.Button.A, "给狗喂食");
                    }
                    if (!player.GetComponent<PlayerAction>().isHolding())
                    {
                        Debug.Log("FEEDING");
                        processState.Set((int)Process.FEEDING, true);
                        nowProcess = Process.IDLE;
                        UIManager.HUD.ShowHelpHide();
                    }
                    break;
                case Process.PETTING:
                    if (!processState.Get((int)Process.PETTING))
                    {
                        processState.Set((int)Process.PETTING, true);
                        UIManager.Joycon.ShowPettingAnimal("按住 B 键, 爱抚小狗");
                    }
                    if (PlayerAction.nowAction == PlayerAction.Action.PETTING)
                    {
                        Debug.Log("PETTING");
                        nowProcess = Process.SHIFT;
                        UIManager.Joycon.ShowPettingAnimalHide();

                        bug_dog.SendMessage("touched");
                        cameraState = player.GetComponent<PlayerController>().cameraState;
                        conversationProcessing = true;
                        showingConversation = true;
                        conversationIndex++;
                    }
                    break;
                case Process.SHIFT:
                    if (!processState.Get((int)Process.SHIFT))
                    {
                        if (conversationIndex > 9)
                        {
                            processState.Set((int)Process.SHIFT, true);
                            UIManager.Joycon.ShowMinus("使用 '-' 键切换视角", 20);
                        }
                    }
                    if (cameraState != player.GetComponent<PlayerController>().cameraState
                        || JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
                    {
                        UIManager.Joycon.ShowMinusHide();
                        Debug.Log("SHIFTIED");
                        nowProcess = Process.IDLE;
                    }
                    break;
                case Process.CLEAR:
                    if(JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_UP))
                    {
                        LoadLevel(2);

                    }
                    Debug.Log("Clear");
                    break;
            }
        }
    }

    public void LoadLevel(int sceneIndex)
    {
        //UIManager.HUD.ShowLoading("Loading.....");
        //StartCoroutine(LoadAsynchronously(sceneIndex));
        UIManager.LoadMission(sceneIndex);
    }


    IEnumerator LoadAsynchronously(int sceneIndex)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            UIManager.HUD.ShowLoadingValue(progress);
            if (operation.progress >= 0.9)
                UIManager.HUD.ShowLoadingHide();
            //slider.value = progress;
            //Debug.Log(operation.progress);
            yield return null;
        }
    }
}
