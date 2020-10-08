using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartStageManager : MonoBehaviour
{
    public GameObject ui;
    //public Slider slider;
    //public GameObject loadingScreen;
    private UIStartMenu startMenu;
    bool stickConnected;
    bool shouldUpdate = false;
    bool stickTried;
    public bool NoJoyconConnectTutorial = false;
    // Start is called before the first frame update
    void Start()
    {
        stickConnected = false;
        stickTried = false;
        shouldUpdate = true;
        startMenu = ui.GetComponent<UIStartMenu>();
        if (NoJoyconConnectTutorial)
        {
            stickTried = true;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!stickTried)
        {
            UIManager.HUD.ShowJoyconConnectTutorial(() =>
            {
                startMenu.Show();
                stickConnected = true;
            });

            
            stickTried = true;
            return;
        }

        if (!stickConnected)
        {
            return;
        }
        if (!shouldUpdate)
        {
            return;
        }

        if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT))
        {
            if (startMenu.currentSelection == 0)
                LoadLevel(1);
            else if(startMenu.currentSelection == 1)
            {
                shouldUpdate = false;
                UIManager.Inventory.ShowSelectMission((index) =>
                {
                    if(index >= 0)
                    {
                        UIManager.LoadMission(index + 2);
                    }
                        
                    else
                    {
                        shouldUpdate = true;
                        startMenu.Show();
                    }
                });
            }
            else if(startMenu.currentSelection == 2)
            {
                Application.Quit();
            }
        }
        
       
    }
    public void LoadLevel(int sceneIndex)
    {
        //UIManager.HUD.ShowLoading("Loading.....");
        startMenu.HideUI();
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
            Debug.Log(operation.progress);
            yield return null;
        }
    }
}
