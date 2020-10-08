using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferObject : MonoBehaviour
{

    public GameObject referredObject;
    public bool isReferring = true;



    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            isReferring = false;
        }
        if (isReferring)
        {
            transform.position = referredObject.transform.position;
        }
        //if(JoyconInfo.joyconL.GetButtonDown(Joycon.Button.SHOULDER_2))
        //{
        //    Debug.Log("Back");
        //    isReferring = true;
        //    Destroy(gameObject.GetComponent<Rigidbody>());
        //    Destroy(gameObject.GetComponent<SphereCollider>());
        //}
    }
}
