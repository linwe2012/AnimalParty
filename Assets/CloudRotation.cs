using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudRotation : MonoBehaviour
{
    public GameObject center;
    public float rotateSpeed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.RotateAround(center.transform.position, new Vector3(0.0f, 1.0f, 0.0f), rotateSpeed * Time.deltaTime);
    }
}
