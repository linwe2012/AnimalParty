using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lean.Transition;

public class ThirdStageUIManager : MonoBehaviour
{


    //public UIManager ui;
    public GameObject player;
    private bool showMinMapFlag = false;
    private int fishIndex = 0;
    private int gotFishCount = 0;

    private bool motionFlag = false;

    private float destroyTimer;
    public GameObject[] fishes;
    private PlayerAction playerAction;
    private Vector3 originFishPos;
    public float timer = 19f;
    private bool firstEnterTrigger = false;
    public GameObject youWinText;
    public GameObject youLoseText;
    public GameObject canvas;

    bool fishingFlag = false;

    bool showingFlag = false;
    bool showingFlag2 = false;


    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        playerAction = player.GetComponent<PlayerAction>();
        timer = 19f;
        gotFishCount = 0;
        UIManager.HUD.SettingsMinimap().SetViewSize(10);
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (fishIndex >= fishes.Length)
        {
            //player.transform.EventTransition(() =>
            //{
            //    UIManager.Joycon.ShowThrowFromBelowHide();

            //}, 1.0f);
            //player.transform.EventTransition(() =>
            //{
            //    UIManager.HUD.ShowCongraluate(2, "恭喜你小有收获！");

            //}, 15.0f);
            GameWin();
        }
        if (!fishingFlag)
        {
            //if (playerAction.fishingTrigger && !firstEnterTrigger)
            //{
            //    firstEnterTrigger = true;
            //    timer = 16.0f;
            //}
            //if (!playerAction.fishingTrigger)
            //{
            //    firstEnterTrigger = false;
            //}
            if (playerAction.heldObject != null && playerAction.heldObject.GetComponent<ItemLock>().type == ItemLock.ItemType.FISHINGROD && !motionFlag)
            {
                //if (timer > 15.0f)
                //{
                //    if (playerAction.fishingTrigger && firstEnterTrigger)
                //    {
                //        UIManager.Joycon.ShowAnimation("在此地挥动鱼竿可钓鱼", "FCast");
                //        firstEnterTrigger = false;
                //        motionFlag = true;
                //        timer = 0f;


                //        player.transform.EventTransition(() =>
                //        {
                //            UIManager.Joycon.ShowThrowFromBelowHide();
                //            UIManager.HUD.ShowHelp(UIManager.Button.A, "Start Fishing");
                //            showingFlag2 = true;


                //        }, 4.0f);
                //        player.transform.EventTransition(() =>
                //        {
                //            UIManager.Joycon.ShowAnimation("转动手柄收缩鱼线", "FReel");

                //        }, 5.0f);


                //        player.transform.EventTransition(() =>
                //        {
                //            UIManager.Joycon.ShowThrowFromBelowHide();
                //            motionFlag = false;
                //            showingFlag2 = false;
                //        //UIManager.HUD.ShowHelpHide();

                //    }, 9.0f);
                //    }
                //else if (!playerAction.fishingTrigger)
                if (!playerAction.fishingTrigger)
                {
                    UIManager.Joycon.ShowAnimation("在指定地点挥动鱼竿可钓鱼", "FCast");
                    motionFlag = true;
                    timer = 0f;


                    player.transform.EventTransition(() =>
                    {
                        UIManager.Joycon.ShowThrowFromBelowHide();


                    }, 4.0f);
                    player.transform.EventTransition(() =>
                    {
                        UIManager.Joycon.ShowAnimation("转动手柄收缩鱼线", "FReel");
                    //UIManager.Joycon.ShowAnimation("转动手柄收缩鱼线", "FReel");

                    }, 5.0f);
                    player.transform.EventTransition(() =>
                    {
                        UIManager.Joycon.ShowThrowFromBelowHide();
                        motionFlag = false;
                    //UIManager.Joycon.ShowAnimation("转动手柄收缩鱼线", "FReel");

                    }, 9.0f);
                    //}
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }

            if (playerAction.fishInstance == null && fishIndex < fishes.Length)
            {
                playerAction.fishInstance = fishes[fishIndex];
                playerAction.fishCurve = fishes[fishIndex].GetComponent<FishCurve>();
                playerAction.fishRigidBody = fishes[fishIndex].GetComponent<Rigidbody>();
                //ParticleSystem particleSystem = fishes[fishIndex].GetComponentsInChildren<ParticleSystem>()[0];
                //GameObject splash = FindChild(fishes[fishIndex].transform, "splash").gameObject;
                //GameObject splash = fishes[fishIndex].transform.Find("splash").gameObject;
                //playerAction.fishParticleSystem = splash;
                originFishPos = fishes[fishIndex].transform.position;
                destroyTimer = 5.0f;
                
                //playerAction.fishParticleSystem = fishes[fishIndex].GetComponent<ParticleSystem>();
            }


            

            if (Vector3.Distance(fishes[fishIndex].transform.position, originFishPos) > 1.0f && destroyTimer >= 0)
            {
                if (PlayerAction.fishOnHook)
                {
                    youWinText.SetActive(true);
                }
                else
                {
                    youLoseText.SetActive(true);
                }
                canvas.SetActive(true);
                destroyTimer -= Time.deltaTime;

            }
            if (destroyTimer < 0.0f)
            {
                Destroy(fishes[fishIndex]);
                playerAction.fishInstance = null;
                fishIndex++;
                youWinText.SetActive(false);
                youLoseText.SetActive(false);
                canvas.SetActive(false);
                PlayerAction.fishOnHook = false;
                //destroyTimer = 5.0f;
            }



            if (!showMinMapFlag)
            {
                showMinMapFlag = true;
                UIManager.HUD.ShowMinimap(Camera.main);
            }


            if (PlayerAction.nowAction == PlayerAction.Action.FISHINGIDLE)
            {
                UIManager.HUD.ShowHelp(UIManager.Button.A, "Start Fishing");
            }


            showingFlag = false;
            int SearchRadius = 3;
            Collider[] cols = Physics.OverlapSphere(player.transform.position, SearchRadius);
            for (int i = 0; i < cols.Length; i++)
                if (cols[i].gameObject.GetComponent<ItemLock>() != null)
                {
                    if (cols[i].gameObject.GetComponent<ItemLock>().natual)
                    {
                        showingFlag = true;
                        UIManager.HUD.ShowHelp(UIManager.Button.A, "Pick Up the Fishing Rod");
                    }
                }

            //if(playerAction.fishingTrigger && !showingFlag)
            //{
            //    showingFlag = true;
            //    UIManager.Joycon.ShowAnimation("在此地挥动鱼竿可钓鱼", "FCast");
            //    //timer = 0f;

            //    player.transform.EventTransition(() =>
            //    {
            //        UIManager.Joycon.ShowThrowFromBelowHide();
            //        showingFlag = false;
            //    }, 4.0f);

            //    //UIManager.HUD.ShowHelp(UIManager.Button.A, "Start the fishing");
            //}
            if (!showingFlag && !showingFlag2)
                UIManager.HUD.ShowHelpHide();

        }
        else
        {
            UIManager.HUD.ShowHelpHide();
            UIManager.Joycon.ShowThrowFromBelowHide();

        }



    }

    void FishStart()
    {
        fishingFlag = true;
    }
    void FishEnd()
    {
        fishingFlag = false;
    }

    private void getOne()
    {
        gotFishCount += 1;
    }

    private void GameWin()
    {
        
        int heartCount = gotFishCount;
        string msg;
        if(heartCount == 0)
        {
            heartCount = 1;
        }
        switch (heartCount)
        {
            case 1:
                msg = "emm, 再接再厉啊";
                break;

            case 2:
                msg = "你抱着两个条鱼快乐地回家了";
                break;
            case 3:
                msg = "恭喜你练成麒麟臂";
                break;
            default:
                msg = "<error>";
                break;
        }
        UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
        {
            LoadLevel(5);
        });

        
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
