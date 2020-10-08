 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{

    public GameObject player;
    public AnimationCurve VerticalCurve;
    public AnimationCurve testCurve;

    public float rotateSpeed = 60.0f;
    private float verticalAngle = Mathf.PI / 2.0f;
    private Vector3 playerHeadOffset = new Vector3(0, 2.2f, 0);
    // Start is called before the first frame update
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Vector3 playerHead = player.transform.position + playerHeadOffset;
        float distanceY = VerticalCurve.Evaluate(verticalAngle / Mathf.PI);
        //float distanceY = testCurve.Evaluate(verticalAngle / Mathf.PI);
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 offset;
        offset = (
            -Mathf.Sin(verticalAngle) * forward * distanceY
            - Mathf.Cos(verticalAngle) * distanceY * player.transform.up.normalized);
        transform.position = playerHead + offset;
        transform.LookAt(playerHead);
    }

    private void OnEnable()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        Vector3 playerHead = player.transform.position + playerHeadOffset;
        float distanceY = VerticalCurve.Evaluate(verticalAngle / Mathf.PI);
        //float distanceY = testCurve.Evaluate(verticalAngle / Mathf.PI);
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 offset;
        offset = (
            -Mathf.Sin(verticalAngle) * forward * distanceY
            - Mathf.Cos(verticalAngle) * distanceY * player.transform.up.normalized);
        transform.position = playerHead + offset;
        transform.LookAt(playerHead);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Look(JoyconInfo.rightStick);
    }

    void Look(Vector2 rotate)
    {
        //if (rotate.sqrMagnitude < 0.01)
            //return;
        var scaledRotateSpeed = rotateSpeed * Time.deltaTime;
        Vector3 axis = new Vector3(0, 1, 0);
        Vector3 playerHead = player.transform.position + playerHeadOffset;

        verticalAngle += (-rotate.y / 0.7f) * Mathf.PI / 180.0f;
        //verticalAngle = Mathf.Lerp(verticalAngle,Mathf.Clamp(verticalAngle, 0.2f, Mathf.PI - 0.2f),Time.deltaTime);
        verticalAngle = Mathf.Clamp(verticalAngle, 0.2f, Mathf.PI - 0.2f);
        float distanceY = VerticalCurve.Evaluate(verticalAngle / Mathf.PI);
        //float distanceY = testCurve.Evaluate(verticalAngle / Mathf.PI);
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 offset;
        offset = (
            -Mathf.Sin(verticalAngle) * forward * distanceY
            -Mathf.Cos(verticalAngle) * distanceY * player.transform.up.normalized);
        transform.position = playerHead + offset;
        transform.RotateAround(player.transform.position, axis, rotate.x * scaledRotateSpeed);
        transform.LookAt(playerHead);
        
    }
}
