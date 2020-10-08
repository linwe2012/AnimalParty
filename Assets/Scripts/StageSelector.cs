using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelector : MonoBehaviour
{
    KeyCode currentKey;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      
        if(Input.GetKeyDown(KeyCode.Alpha0))
            LoadLevel(0);
        if(Input.GetKeyDown(KeyCode.Alpha1))
            LoadLevel(1);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            LoadLevel(2);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            LoadLevel(3);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            LoadLevel(4);
        if(Input.GetKeyDown(KeyCode.Alpha5))
            LoadLevel(5);
                        
    }
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }


    IEnumerator LoadAsynchronously(int sceneIndex)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        //loadingScreen.SetActive(true);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            //slider.value = progress;

            Debug.Log(operation.progress);
            yield return null;
        }
    }
}