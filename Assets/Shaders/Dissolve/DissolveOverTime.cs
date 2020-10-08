using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class DissolveOverTime : MonoBehaviour
{
    private SkinnedMeshRenderer meshRenderer;

    public float speed = .5f;

    private void Start(){
        //GetComponent<SkinnedMeshRenderer>
        meshRenderer = this.GetComponent<SkinnedMeshRenderer>();
    }

    private float t = 0.0f;
    private bool flag = false;
    private void Update(){

        if (Input.GetKeyDown(KeyCode.Space))
        {
            flag = true;
        }
        if (flag)
        {
            dissolve();
        }
        if(t*speed > 1)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void dissolve()
    {
        Material[] mats = meshRenderer.materials;
        mats[0].SetFloat("_Cutoff", (t * speed));
        t += Time.deltaTime;
        Debug.Log(t * speed);
        meshRenderer.materials = mats;
    }
}
