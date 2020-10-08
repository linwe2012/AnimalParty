using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoFog : MonoBehaviour
{
    public bool doWeHaveFogInScene = false;

    private void Start()
    {
        doWeHaveFogInScene = RenderSettings.fog;
    }

    private void OnPreRender()
    {
        RenderSettings.fog = false;
    }
    private void OnPostRender()
    {
        RenderSettings.fog = doWeHaveFogInScene;
    }
}
