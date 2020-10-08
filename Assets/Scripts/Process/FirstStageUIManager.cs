using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lean.Transition;
using System;

public class FirstStageUIManager : MonoBehaviour
{
    public GameObject player;
    public GameObject destination;
    public GameObject elephant;

    public CPC_CameraPath camPath;

    private bool motionFlag = false;

    System.Action SingleUpdate;

    // Start is called before the first frame update

    UIManagerProcess.StepbyStep steps;
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        steps = new UIManagerProcess.StepbyStep();

        SingleUpdate = () =>
        {
            UIManager.HUD.ShowMinimap(Camera.main);
            UIManager.HUD.SettingsMinimap()
                .SetViewSize(27)
                .AddIconTracerIcon(UIInternal.MinimapManager.Icon.BlueTarget, destination)
                .AddIconTracerIcon(UIInternal.MinimapManager.Icon.Elephant, elephant);
            
            SingleUpdate = steps.Update;
        };

        StepByStepTutorial();
    }

    
    void StepByStepTutorial()
    {
        UIManager.HijackInput();

        bool allowSkip = true;

        Func<bool> confirm = () =>
        {
            if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
            {
                return true;
            }
            else if (allowSkip && JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI))
            {
                steps.SkipTo("Done");
                return false;
            }
            return false;
        };

        steps.Do(() =>
        {
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "你需要把眼前的大象带到终点", "欢迎来到第一关", BText: "跳过");
            Time.timeScale = 0;
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.SettingsMinimap().EmphasizeMiniMap();
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "小地图上蓝色标记就是终点位置, 大象标记会告诉你大象在哪", "目标", BText: "跳过");
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.SettingsMinimap().EmphasizeMiniMapStop();
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "你需要去捡起起水果, 然后走到大象身边给它喂食, 这样大象才会跟着你走", "吸引大象", BText: "跳过");
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "蛇会攻击大象, 降低大象对你的好感度, 你当然可以杀死蛇, 小心不要误伤大象", "驱赶蛇", BText: "跳过");
        })
        .Wait(confirm)
        .Do(() =>
        {
            Time.timeScale = 1;
            allowSkip = false;
            UIManager.Joycon.ShowAnimation("按住 ZR 挥动 Joycon 攻击并且消灭蛇", "ZombieAttack");
            UIManager.HUD.ShowDialogHide();
        })
        .Wait(confirm)
        .Do(() =>
        {
            Time.timeScale = 0;
            allowSkip = true;
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "保护好大象, 不要离大象太远, 第一个水果就在右前方.", "祝你好运");
            UIManager.Joycon.ShowAnimationHide();
        })
        .Wait(confirm)
        .Do(() =>
        {
            SingleUpdate = NormalUpdate;
            UIManager.HUD.ShowDialogHide();
            UIManager.ReturnInputControl();
            UIManager.HUD.SettingsMinimap().EmphasizeMiniMapStop();
            Time.timeScale = 1;
        }, "Done")
        .Catch((e) =>
        {
            steps.ClearQueue();
            UIManager.ReturnInputControl();
            UIManager.HUD.ShowDialogHide();
            UIManager.HUD.SettingsMinimap().EmphasizeMiniMapStop();
            Time.timeScale = 1;
            SingleUpdate = NormalUpdate;
        });
    }

    void NormalUpdate()
    {
        bool showingFlag = false;
        int SearchRadius = 3;
        Collider[] cols = Physics.OverlapSphere(player.transform.position, SearchRadius);
        for (int i = 0; i < cols.Length; i++)
            if (cols[i].gameObject.GetComponent<ItemLock>() != null)
            {
                if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                {
                    showingFlag = true;
                    UIManager.HUD.ShowHelp(UIManager.Button.A, "Pick Up the Fruit");
                }
            }
        if (player.GetComponent<PlayerAction>().isHolding())
        {
            GameObject[] creatrues = GameObject.FindGameObjectsWithTag("Creatures");
            foreach (var creature in creatrues)
            {
                if (Vector3.Distance(creature.transform.position, player.transform.position) < SearchRadius)
                {
                    showingFlag = true;
                    UIManager.HUD.ShowHelp(UIManager.Button.A, "Feed the elephant");
                }
            }
        }
        if (!showingFlag)
            UIManager.HUD.ShowHelpHide();
    }
    

    // Update is called once per frame
    void LateUpdate()
    {
        SingleUpdate();
        //if (!motionFlag)
        //{
        //    UIManager.Joycon.ShowAnimation("按住 ZR 挥动 Joycon 攻击并且消灭蛇", "ZombieAttack");
        //    motionFlag = true;
        //
        //    player.transform.EventTransition(() =>
        //    {
        //        UIManager.Joycon.ShowThrowFromBelowHide();
        //    }, 6.0f);
        //}

        
        
    }


    private void GameWin(object[] message)
    {
        if (message.Length > 0)
        {
            int heartCount = int.Parse(message[0].ToString());
            string msg;
            switch(heartCount)
            {
                case 1:
                    msg = "大象回到了自己的家";
                    break;

                case 2:
                    msg = "大象欢快地回到了自己的家";
                    break;
                case 3:
                    msg = "大象依依不舍地回到了自己的家";
                    break;
                default:
                    msg = "<error>";
                    break;
            }
            UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
            {
                LoadLevel(3);
            });
            
        }
    }
    public void LoadLevel(int sceneIndex)
    {
        // UIManager.HUD.ShowLoading("Loading.....");
        // StartCoroutine(LoadAsynchronously(sceneIndex));
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
