using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class FifthStatgeUIManager : MonoBehaviour
{
    private bool showMinMapFlag = false;

    public GameObject cornKernelList;
    public GameObject heartSlides;

    public GameObject timeText;
    int heartIndex = 3;
    int heartCount = 0;

    public int TimeCount = 60;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(timeCountDown());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!showMinMapFlag)
        {
            showMinMapFlag = true;
            UIManager.HUD.ShowMinimap(Camera.main);
        }
        bool showingFlag = false;

        if (!showingFlag)
            UIManager.HUD.ShowHelpHide();

        Debug.Log(cornKernelList.transform.childCount / 10);
        if (cornKernelList.transform.childCount / 10 > (heartIndex - 3)&&heartIndex<6)
        {
            
            heartSlides.transform.GetChild(heartIndex - 3).gameObject.SetActive(true);
            heartSlides.transform.GetChild(heartIndex).gameObject.SetActive((false));
            heartIndex++;
            heartCount++;
        }
        if(heartCount>=3||TimeCount<=0)
        {
            GameFinished();
        }
    }
    public void GameFinished()
    {
        string msg;
        switch (heartCount)
        {
            case 0:
                msg = "未能通过";
                break;
            case 1:
                msg = "一星过关";
                break;

            case 2:
                msg = "二星过关";
                break;
            case 3:
                msg = "三星过关";
                break;
            default:
                msg = "<error>";
                break;
        }
        UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
        {
            LoadLevel(0);
        });
    }


    private void GameWin(object[] message)
    {
        if (message.Length > 0)
        {
            int heartCount = int.Parse(message[0].ToString());
            string msg;
            switch (heartCount)
            {
                case 1:
                    msg = "一星过关";
                    break;

                case 2:
                    msg = "二星过关";
                    break;
                case 3:
                    msg = "三星过关";
                    break;
                default:
                    msg = "<error>";
                    break;
            }
            UIManager.HUD.ShowCongraluate(heartCount, msg, next: () =>
            {
                LoadLevel(0);
            });

        }
    }
    public void LoadLevel(int sceneIndex)
    {
        UIManager.HUD.ShowLoading("Loading.....");
        StartCoroutine(LoadAsynchronously(sceneIndex));
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


    IEnumerator timeCountDown()
    {
        while (TimeCount >= 0)
        {
            timeText.GetComponent<TMP_Text>().text = TimeCount.ToString();
            yield return new WaitForSeconds(1);
            TimeCount--;

        }
    }

}
