using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyJoyconDemo : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);
    }

    private void Update()
    {
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;
        
        foreach (var button in m_buttons)
        {
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }
        Debug.Log(string.Format("Left ButtonL:{0:D3}", m_pressedButtonL));
        Debug.Log(string.Format("Left ButtonR:{0:D3}", m_pressedButtonR));
        if (m_joyconL.GetButtonDown(Joycon.Button.DPAD_UP))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);
        }
        if (m_joyconR.GetButtonDown(Joycon.Button.DPAD_UP))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);
        }
    }
    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(960));

        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;

            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label("押されているボタン：" + button);
            GUILayout.EndVertical();
        }

        GUILayout.EndHorizontal();
    }
}

