using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoyconInfo : MonoBehaviour
{
    private static List<Joycon> joycons;

    // Values made available via Unity

    public static Vector2 leftStick
    {
        get { return joyconL.GetStickAsVector2(viewer); }
    }

    public static Vector2 rightStick
    {
        get { return joyconR.GetStickAsVector2(viewer); }
    }

    public static Vector3 leftGyro
    {
        get { return joyconL.GetGyro(viewer); }
    }

    public static Vector3 rightGyro
    {
        get { return joyconR.GetGyro(viewer); }
    }

    public static Vector3 leftAccel
    {
        get { return joyconL.GetAccel(viewer); }
    }

    public static Vector3 rightAccel
    {
        get { return joyconR.GetAccel(viewer); }
    }

    public static Quaternion leftOrientation
    {
        get { return joyconL.GetVector(viewer); }
    }

    public static Quaternion rightOrientation
    {
        get { return joyconR.GetVector(viewer); }
    }

    //public static Vector2 leftStick;
    //public static Vector2 rightStick;
    //public static Vector3 leftGyro;
    //public static Vector3 rightGyro;
    //public static Vector3 leftAccel;
    //public static Vector3 rightAccel;
    //public static Quaternion leftOrientation;
    //public static Quaternion rightOrientation;

    public static ManagedInput.Viewer viewer;

    
    //public static Joycon joyconL;
    //public static Joycon joyconR;

    public static ManagedInput joyconL;
    public static ManagedInput joyconR;

    public static bool leftConnected = false;
    public static bool rightConnected = false;

    public static void ResetConnection()
    {
        JoyconManager.ResetConnection();

        joycons = JoyconManager.Instance.j;

        joyconL = new ManagedInput();
        joyconR = new ManagedInput();

        var jL = joycons.Find(c => c.isLeft);
        var jR = joycons.Find(c => !c.isLeft);

        joyconL.Init(jL, true);
        joyconR.Init(jR, false);


        if (jL != null) leftConnected = true;
        if (jR != null) rightConnected = true;
    }

    bool hasInited = false;

    public static void Init()
    {
        var gameObj = GameObject.FindObjectOfType<JoyconInfo>();
        gameObj.DoInit();
    }

    void DoInit()
    {
        if (hasInited)
        {
            return;
        }
        hasInited = true;

        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
        //if (joycons.Count < 2)
        //{
        //    Destroy(gameObject);
        //}

        joyconL = new ManagedInput();
        joyconR = new ManagedInput();

        var jL = joycons.Find(c => c.isLeft);
        var jR = joycons.Find(c => !c.isLeft);

        joyconL.Init(jL, true);
        joyconR.Init(jR, false);


        if (jL != null) leftConnected = true;

        if (jR != null) rightConnected = true;

        viewer = ManagedInput.Viewer.Default;
        Blind(viewer, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        DoInit();
    }

    // Update is called once per frame
    //void Update()
    //{
        // UpdateJoyconInfo();
    //}

    
    /*
    void UpdateJoyconInfo()
    {
        //// left
        // stick
        float[] sticks = joyconL.GetStick();
        leftStick = new Vector2(sticks[0], sticks[1]);
        // Gyro values: x, y, z axis values (in radians per second)
        leftGyro = joyconL.GetGyro();
        // Accel values:  x, y, z axis values (in Gs)
        leftAccel = joyconL.GetAccel();
        // orient
        leftOrientation = joyconL.GetVector();

        //// right
        // stick
        sticks = joyconR.GetStick();
        rightStick = new Vector2(sticks[0], sticks[1]);
        // Gyro values: x, y, z axis values (in radians per second)
        rightGyro = joyconR.GetGyro();
        // Accel values:  x, y, z axis values (in Gs)
        rightAccel = joyconR.GetAccel();
        // orient
        rightOrientation = joyconR.GetVector();

    }
    */


    void PrintInfo(Vector2 info)
    {
    }

    void PrintInfo(Vector3 info)
    {

    }

    static public void Blind(ManagedInput.Viewer viewer_, bool blind)
    {
        joyconL.Blind(viewer_, blind);
        joyconR.Blind(viewer_, blind);
    }

}

