using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ViewerManager
{
    public Joycon joycon;

    public bool blind;
    bool left;

    public ViewerManager(Joycon joycon_, bool left_)
    {
        left = left_;
        blind = false;
        joycon = joycon_;
    }

    static Dictionary<Joycon.Button, KeyCode> rightMappings = new Dictionary<Joycon.Button, KeyCode>
    {
        { Joycon.Button.PLUS, KeyCode.Plus },
        { Joycon.Button.DPAD_RIGHT, KeyCode.Return },
        { Joycon.Button.DPAD_DOWN, KeyCode.B },
        { Joycon.Button.DPAD_UP, KeyCode.X },
        { Joycon.Button.DPAD_LEFT, KeyCode.Y },
        { Joycon.Button.SHOULDER_1, KeyCode.KeypadMultiply }, // R
        { Joycon.Button.SHOULDER_2, KeyCode.RightBracket },   // ZR
    };

    static Dictionary<Joycon.Button, KeyCode> rightMappings2 = new Dictionary<Joycon.Button, KeyCode>
    {
        { Joycon.Button.PLUS, KeyCode.KeypadPlus },
    };

    static Dictionary<Joycon.Button, KeyCode> leftMappings = new Dictionary<Joycon.Button, KeyCode>
    {
        { Joycon.Button.MINUS, KeyCode.Minus },
        { Joycon.Button.DPAD_RIGHT, KeyCode.E },
        { Joycon.Button.DPAD_LEFT, KeyCode.Q },
        { Joycon.Button.DPAD_DOWN, KeyCode.F },
        { Joycon.Button.DPAD_UP, KeyCode.R },
        { Joycon.Button.STICK, KeyCode.LeftShift },
        { Joycon.Button.SHOULDER_1, KeyCode.KeypadDivide }, // L
        { Joycon.Button.SHOULDER_2, KeyCode.LeftBracket },   // ZL
    };

    static Dictionary<Joycon.Button, KeyCode> leftMappings2 = new Dictionary<Joycon.Button, KeyCode>
    {
        { Joycon.Button.MINUS, KeyCode.KeypadMinus },
    };

    
    bool keyMapRight(Joycon.Button b, Func<KeyCode, bool> fn)
    {
        var key1 = KeyCode.None;
        var key2 = KeyCode.None;


        if (rightMappings.ContainsKey(b))
        {
            key1 = rightMappings[b];
        }
        if (rightMappings2.ContainsKey(b))
        {
            key2 = rightMappings2[b];
        }
        return fn(key1) || fn(key2);
    }

    bool keyMapLeft(Joycon.Button b, Func<KeyCode, bool> fn)
    {
        var key1 = KeyCode.None;
        var key2 = KeyCode.None;


        if (leftMappings.ContainsKey(b))
        {
            key1 = leftMappings[b];
        }
        if (leftMappings2.ContainsKey(b))
        {
            key2 = leftMappings2[b];
        }
        return fn(key1) || fn(key2);
    }

    bool keyMap(Joycon.Button b, Func<KeyCode, bool> fn)
    {
        return left ?
            keyMapLeft(b, fn) :
            keyMapRight(b, fn);
    }

    public bool GetButtonUp(Joycon.Button b)
    {
        if (blind) 
            return false;

        if (joycon != null) 
            return joycon.GetButtonUp(b);
        else
        {
            return keyMap(b, Input.GetKeyUp);
        }
    }

    

    public bool GetButtonDown(Joycon.Button b)
    {
        if (blind) 
            return false;

        if (joycon != null) 
            return joycon.GetButtonDown(b);
        else
        {

            return keyMap(b, Input.GetKeyDown);
        }

        return false;
    }

    public bool GetButton(Joycon.Button b)
    {
        if (blind)
        {
            return false;
        }

        if (joycon != null)
        {
            return joycon.GetButton(b);
        }
        else
        {
            return keyMap(b, Input.GetKey);
        }
        return false;
    }

    public Vector3 GetGyro()
    {
        if (blind)
        {
            return Vector3.zero;
        }

        if (joycon != null)
        {
            return joycon.GetGyro();
        }

        return Vector3.zero;
    }

    public Vector3 GetAccel()
    {
        if (blind)
        {
            return Vector3.zero;
        }

        if (joycon != null)
        {
            return joycon.GetAccel();
        }

        return Vector3.zero;
    }

    public Quaternion GetVector()
    {
        if (blind)
        {
            return Quaternion.identity;
        }

        if (joycon != null)
        {
            return joycon.GetVector();
        }

        return Quaternion.identity;
    }

    float[] GetStickOfKeyboardLeft()
    {
        var stick = new float[] { 0, 0 };
        if (Input.GetKey(KeyCode.W))
        {
            stick[1] += 0.8f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            stick[1] -= 0.8f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            stick[0] -= 0.8f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            stick[0] += 0.8f;
        }
        return stick;
    }

    float[] GetStickOfKeyboardRight()
    {
        var stick = new float[] { 0, 0 };
        if (Input.GetKey(KeyCode.UpArrow))
        {
            stick[1] += 0.8f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            stick[1] -= 0.8f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            stick[0] -= 0.8f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            stick[0] += 0.8f;
        }
        return stick;
    }

    float[] GetStickOfKeyboard()
    {
        return left ? GetStickOfKeyboardLeft() : GetStickOfKeyboardRight();
    }

    public float[] GetStick()
    {
        if (blind)
        {
            return new float[] { 0, 0 };
        }

        if (joycon != null)
        {
            var stick = joycon.GetStick();
            if(stick[0] == 0 && stick[1] == 0)
            {
                return GetStickOfKeyboard();
            }

            return stick;

        }

        return GetStickOfKeyboard();
    }

    public void SetRumble(float low_freq, float high_freq, float amp, int time)
    {
        if (blind || joycon == null)
        {

        }
        else
        {
            joycon.SetRumble(low_freq, high_freq, amp, time);
        }
    }
}


public class ManagedInput
{
    Joycon joycon;

    public bool restricted = false;



    public enum Viewer
    {
        UseCurrentSettings = -1,
        Default,
        UI
    }

    List<ViewerManager> viewers;

    public Color bodyColor
    {
        get
        {
            if(joycon == null)
            {
                return new Color(0, 0, 0, 0);
            }
            else
            {
                return joycon.bodyColor;
            }

        }
    }

    public bool hasBodyColor
    {
        get
        {
            return joycon != null && joycon.device_color != null;
        }
    }

    public void Init(Joycon joycon_, bool left)
    {
        joycon = joycon_;
        if (viewers != null)
        {
            foreach(var viewer in viewers)
            {
                viewer.joycon = joycon;
                viewer.blind = false;
            }
        }
        else
        {
            viewers = new List<ViewerManager>();
            viewers.Add(
                new ViewerManager(joycon_, left));
            viewers.Add(
                new ViewerManager(joycon_, left));
        }
    }

    public void Blind(Viewer viewer, bool blind)
    {
        viewers[(int)viewer].blind = blind;
    }

    public bool GetButtonDown(Joycon.Button b, Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetButtonDown(b);
    }

    public bool GetButton(Joycon.Button b, Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetButton(b);
    }

    public bool GetButtonUp(Joycon.Button b, Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetButtonUp(b);
    }

    public Vector3 GetGyro(Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetGyro();
    }

    public Vector3 GetAccel(Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetAccel();
    }

    public Quaternion GetVector(Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetVector();
    }

    public float[] GetStick(Viewer viewer = Viewer.Default)
    {
        return viewers[(int)viewer].GetStick();
    }

    public Vector2 GetStickAsVector2(Viewer viewer = Viewer.Default)
    {
        var stick = GetStick(viewer);
        return new Vector2(stick[0], stick[1]);
    }

    public void SetRumble(float low_freq, float high_freq, float amp, int time = 0, Viewer viewer = Viewer.Default)
    {
        viewers[(int)viewer].SetRumble(low_freq, high_freq, amp, time);
    }

    public void Update()
    {
        if(joycon != null)
        {
            joycon.Update();
        }
    }
}

