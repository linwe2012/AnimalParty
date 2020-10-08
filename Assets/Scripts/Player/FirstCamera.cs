using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCamera : MonoBehaviour
{
    public float forwardDistance = 0.5f;
    public float updDistance = 1.5f;
    public GameObject player;
    public float rotateSpeed = 60.0f; 
    public Vector3 m_Rotation;
    private Vector3 cameraOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        cameraOffset = player.transform.forward * forwardDistance + player.transform.up * updDistance;
        transform.position = player.transform.position + cameraOffset;
        transform.forward = player.transform.forward;
        m_Rotation = new Vector3(0, 0, 0);
    }

    private void OnEnable()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        cameraOffset = player.transform.forward * forwardDistance + player.transform.up * updDistance;
        transform.position = player.transform.position + cameraOffset;
        transform.forward = player.transform.forward;
        m_Rotation = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        cameraOffset = player.transform.forward * forwardDistance + player.transform.up * updDistance;
        transform.position = player.transform.position + cameraOffset;
        Look(JoyconInfo.rightStick);

    }
    private void Move()
    {
        Vector3 newPos = player.transform.position + cameraOffset;
        transform.position = newPos;
    }

    //private void Move(Vector2 direction)
    //{
    //    if (direction.sqrMagnitude < 0.01)
    //        return;
    //    var scaledMoveSpeed = moveSpeed * Time.deltaTime;
    //    // For simplicity's sake, we just keep movement in a single plane here. Rotate
    //    // direction according to world Y rotation of player.
    //    var move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
    //    transform.position += move * scaledMoveSpeed;
    //}

    private void Look(Vector2 rotate)
    {
        //if (rotate.sqrMagnitude < 0.01)
            //return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        m_Rotation.y = 0;
        //m_Rotation.y += rotate.x * scaledRotateSpeed;
        m_Rotation.x = Mathf.Clamp(m_Rotation.x - rotate.y * scaledRotateSpeed, -60, 60);
        transform.localEulerAngles = new Vector3(m_Rotation.x, transform.localEulerAngles.y,transform.localEulerAngles.z);
    }
}
