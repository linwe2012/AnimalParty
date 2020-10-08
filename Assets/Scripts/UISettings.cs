using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum KeyMappingType
{
    Mouse,
    Keyboard,
    Mixed
}
[CreateAssetMenu(fileName = "Settings", menuName = "UIManager/Settings")]
public class UISettings : ScriptableObject
{
    public float startMenuSelectInterval = 0.3f;
    public float rotateUIHumanoidSpeed = 120f;
    public float inventoryStickSensitivity = 0.3f;

    public float volume;

    

    public struct KeyMappingItem
    {
        public KeyMappingType type;
        public KeyCode key;
        public MouseButton mouse;

        public KeyMappingItem(KeyCode key_)
        {
            type = KeyMappingType.Keyboard;
            key = key_;
            mouse = MouseButton.LeftMouse;
        }

        public KeyMappingItem(MouseButton mouse_)
        {
            type = KeyMappingType.Keyboard;
            key = KeyCode.None;
            mouse = mouse_;
        }
    }

    public struct JoyconKeyMappings
    {
        // public KeyMappingItem leftStick = 
    };

    public JoyconKeyMappings keyMappings;

}
