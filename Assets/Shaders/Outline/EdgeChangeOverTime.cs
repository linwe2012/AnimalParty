using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeChangeOverTime : MonoBehaviour
{
    private Material mat;
    private float value;
    float speed = 2.5f;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        value = Mathf.PingPong(Time.time * speed, 1);
        mat.SetFloat("_OutlineWidth", value);
        //Debug.Log(value);
    }
}
