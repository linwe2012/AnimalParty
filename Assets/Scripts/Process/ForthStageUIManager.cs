using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ForthStageUIManager : MonoBehaviour
{
    public Slider slider;
    public GameObject loadingScreen;
    //public UIManager ui;
    public float gameTimer;
    public int feedCount = 0;
    public GameObject player;
    private bool showMinMapFlag = false;

    private System.Action doUpdate;
    UIManagerProcess.StepbyStep steps;

    // Start is called before the first frame update
    void Start()
    {
        feedCount = 0;
        loadingScreen.SetActive(false);
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        gameTimer = 0;
        steps = new UIManagerProcess.StepbyStep();
        doUpdate = () =>
        {
            UIManager.HUD.ShowMinimap(Camera.main);
            UIManager.HUD.SettingsMinimap().SetViewSize(6);
            doUpdate = steps.Update;
        };
        StepByStepTutorial();
    }

    void StepByStepTutorial()
    {
        Func<bool> confirm = () =>
        {
            if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
            {
                return true;
            }
            else if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_DOWN, ManagedInput.Viewer.UI))
            {
                steps.SkipTo("Done");
                return false;
            }
            return false;
        };

        steps.Do(() =>
        {
            UIManager.HijackInput();
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "拿起鱼和老干妈, 按住 ZR 瞄准, 向企鹅用力投去", "欢迎来到第四关");
            Time.timeScale = 0;
        }).Wait(confirm)
        .Do(() =>
        {
            doUpdate = NormalUpdate;
            UIManager.HUD.ShowDialogHide();
            UIManager.ReturnInputControl();
            Time.timeScale = 1;
        }, "Done")
        .Catch((e) =>
        {
            steps.ClearQueue();
            UIManager.ReturnInputControl();
            UIManager.HUD.ShowDialogHide();
            Time.timeScale = 1;
            doUpdate = NormalUpdate;
        });
    }
    void NormalUpdate()
    {
        gameTimer += Time.deltaTime;

        bool showingFlag = false;
        int SearchRadius = 3;
        Collider[] cols = Physics.OverlapSphere(player.transform.position, SearchRadius);
        for (int i = 0; i < cols.Length; i++)
            if (cols[i].gameObject.GetComponent<ItemLock>() != null)
            {
                if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                {
                    showingFlag = true;
                    UIManager.HUD.ShowHelp(UIManager.Button.A, "Pick Up the Fish");
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

        if (gameTimer > 120.0f)
        {
            GameWin();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        

        doUpdate();
    }

    void AddFeedCount()
    {
        feedCount += 1;
    }

    private void GameWin()
    {
        int heartCount = 0;
        if (feedCount > 10)
            heartCount = 3;
        else if (feedCount > 7)
            heartCount = 2;
        else
            heartCount = 1;
        string msg;
        switch (heartCount)
        {
            case 1:
                msg = "企鹅们有些不高兴地回家了";
                break;

            case 2:
                msg = "企鹅非常快乐地回家了";
                break;
            case 3:
                msg = "企鹅歌颂着你的名字回家了";
                break;
            default:
                msg = "<error>";
                break;
        }
        UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
        {
            LoadLevel(6);
        });
    }
    public void LoadLevel(int sceneIndex)
    {
        // StartCoroutine(LoadAsynchronously(sceneIndex));
        UIManager.LoadMission(sceneIndex);
    }


    IEnumerator LoadAsynchronously(int sceneIndex)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;

            Debug.Log(operation.progress);
            yield return null;
        }
    }
}
