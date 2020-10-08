using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SecondStageUIManager : MonoBehaviour
{
    //public UIManager ui;
    public GameObject player;
    private bool showMinMapFlag = false;
    public Item[] ItemsToBeAdded;

    UIManagerProcess.StepbyStep steps;
    System.Action doUpdate;

    bool win;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        doUpdate = OnFirstUpdate;
        steps = new UIManagerProcess.StepbyStep();
        MakeTutorial();
        win = false;
    }

    void OnFirstUpdate()
    {
        showMinMapFlag = true;
        UIManager.HUD.ShowMinimap(Camera.main);
        UIManager.HUD.SettingsMinimap().SetViewSize(10);

        doUpdate = steps.Update;
    }

    void MakeTutorial()
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
            Time.timeScale = 0f;
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "猩猩暴走啦, 你要给他喂它想要的食物", "第二关", BText: "跳过");
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "猩猩头上气泡会告诉你要吃什么, 如果你捡到的不是猩猩想吃的, 可以先放进背包", "使用背包", BText: "跳过");
            int id = 0;
            steps.Interval(() =>
            {

                UIManager.HUD.ShowNewItemInInventoryHint(ItemsToBeAdded[id]);

                id = id + 1;
                if (id >= ItemsToBeAdded.Length)
                {
                    steps.ClearInterval();
                }
                
            }, 2, true);
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.ShowDialog(UIManager.DialogAt.Bottom, "猩猩每跑一圈好感度都会下降, 它暴走速度也会加快", "别让猩猩跑一圈", BText: "跳过");
        })
        .Wait(confirm)
        .Do(() =>
        {
            UIManager.HUD.ShowDialogHide();
            UIManager.ReturnInputControl();
            Time.timeScale = 1;
            doUpdate = NormalUpdate;
        }, "Done")
        .Catch((e) =>
        {
            steps.ClearQueue();
            UIManager.HUD.ShowDialogHide();
            UIManager.ReturnInputControl();
            Time.timeScale = 1;
            doUpdate = NormalUpdate;
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
                    UIManager.HUD.ShowHelp(UIManager.Button.A, "Feed the Gorilla");
                }
            }
        }
        if (!showingFlag)
            UIManager.HUD.ShowHelpHide();
    }


    // Update is called once per frame
    void LateUpdate()
    {
        doUpdate();
    }
    
    private void GameWin(object[] message)
    {
        if (win) return;
        win = true;
        if (message.Length > 0)
        {
            int heartCount = int.Parse(message[0].ToString());
            string msg;
            switch (heartCount)
            {
                case 1:
                    msg = "猩猩觉得有点不满意";
                    break;

                case 2:
                    msg = "猩猩快乐地躺在地上";
                    break;
                case 3:
                    msg = "猩猩无比感激";
                    break;
                default:
                    msg = "猩猩很生气";
                    break;
            }
            UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
            {
                LoadLevel(4);
            });

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


    //private void GameWin()
    //{
    //    LoadLevel(4);
    //}
    //public void LoadLevel(int sceneIndex)
    //{
    //    UIManager.HUD.ShowLoading("Loading.....");
    //    StartCoroutine(LoadAsynchronously(sceneIndex));
    //}


    //IEnumerator LoadAsynchronously(int sceneIndex)
    //{

    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    //    loadingScreen.SetActive(true);

    //    while (!operation.isDone)
    //    {

    //        float progress = Mathf.Clamp01(operation.progress / 0.9f);

    //        UIManager.HUD.ShowLoadingValue(progress);
    //        if (operation.progress >= 0.9)
    //            UIManager.HUD.ShowLoadingHide();
    //        //slider.value = progress;
    //        //Debug.Log(operation.progress);
    //        yield return null;
    //    }
    //}

}
