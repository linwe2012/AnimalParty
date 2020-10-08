using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    private AsyncOperation asyncOperation;
    public string nextScene;
    public float process;

    // Start is called before the first frame update
    void Start()
    {
        process = 0;
        asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;
;
    }

    // Update is called once per frame
    void Update()
    {
        if(process > 0.99)
        {

        }
    }

}
