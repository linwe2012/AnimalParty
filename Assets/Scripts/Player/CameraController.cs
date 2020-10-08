using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotateSpeed = 60;
    public GameObject player;
    private Vector3 cameraOffset;
    [Range(0.01f, 1.00f)]
    public float SmoothFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if(player is null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        cameraOffset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        Move();
        Look(JoyconInfo.rightStick);
    }

    //private void Look(Vector2 rotate)
    //{
    //    if (rotate.sqrMagnitude < 0.01)
    //        return;
    //    var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
    //    m_Rotation.y += rotate.x * scaledRotateSpeed;
    //    m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -89, 89);
    //    transform.localEulerAngles = m_Rotation;
    //}
    private void Move()
    {
        Vector3 newPos = player.transform.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    }
    private void Look(Vector2 rotate)
    {
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;

        transform.RotateAround(player.transform.position, new Vector3(0, 1, 0), scaledRotateSpeed * rotate.x);
        //transform.RotateAround(player.transform.position, new Vector3(1,0,0), scaledRotateSpeed * rotate.y);
        transform.LookAt(player.transform);
        //transform.RotateAround(transform.parent.position,transform.right, scaledRotateSpeed * rotate.x);

    }

}
