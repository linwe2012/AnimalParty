using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UIInternal
{
    [System.Serializable]
    public class MissingJoyconManager
    {
        public GameObject PanelMissingJoycon;
        public GameObject WinToggleOn;
        public GameObject WinToggleOff;
        public GameObject WinToggleIndicator;
        public GameObject leftActive;
        public GameObject rightActive;
        public GameObject JoyconModel;
        public GameObject JoyconLeftMask;
        public GameObject JoyconRightMask;
        public AnimationCurve CurveRotate;

        Image joyconLMaskImage;
        Image joyconRMaskImage;


        ExtendedGameObject2D extWinToggleIndicator;
        ExtendedGameObject extJoyconModel;
        public float ToggleStayOff = 2.0f;
        public float ToggleTransition = 0.2f;
        public float ToggleStayOn = 2.0f;

        public float currentTime;
        Image WinToggleOnImage;
        float pullJoyconInterval = 0;

        System.Action done;

        public GameObject PanelExistJoycon;
        public GameObject JoyconLeftMask2;
        public GameObject JoyconRightMask2;
        Image joyconLMaskImage2;
        Image joyconRMaskImage2;

        enum ToggleStage
        {
            Off,
            TurningOn,
            On,
            TurningOff
        }

        ToggleStage toggleStage;
        Color toggleOnImageColor;
        float joyconRotateTime;

        bool hasInit = false;

        public void Init()
        {
            if (hasInit) return;
            hasInit = true;

            JoyconModel.SetActive(false);
            extJoyconModel = new ExtendedGameObject(JoyconModel);

            extWinToggleIndicator = new ExtendedGameObject2D(WinToggleIndicator);
            WinToggleOnImage = WinToggleOn.GetComponent<Image>();

            toggleOnImageColor = WinToggleOnImage.color;
            toggleOnImageColor.a = 0;
            WinToggleOnImage.color = toggleOnImageColor;

            toggleStage = ToggleStage.Off;

            leftActive.SetActive(false);
            rightActive.SetActive(false);
            PanelMissingJoycon.SetActive(false);

            joyconRotateTime = 0;

            joyconLMaskImage = JoyconLeftMask.GetComponent<Image>();
            joyconRMaskImage = JoyconRightMask.GetComponent<Image>();

            joyconLMaskImage2 = JoyconLeftMask2.GetComponent<Image>();
            joyconRMaskImage2 = JoyconRightMask2.GetComponent<Image>();

            PanelExistJoycon.SetActive(false);
        }

        public void Show(System.Action done_)
        {
            if (!hasInit)
            {
                Init();
            }
            done = () =>
            {
                PanelExistJoycon.SetActive(true);
                JoyconModel.SetActive(false);
                PanelMissingJoycon.SetActive(false);

                done_();
            };

            if (CheckJoyconFullyConnected())
            {
                return;
            }

            JoyconModel.SetActive(true);
            PanelMissingJoycon.SetActive(true);
            currentTime = 0;
        }

        bool CheckJoyconFullyConnected()
        {
            if (JoyconInfo.joyconL.hasBodyColor)
            {
                UnityEngine.Debug.Log("Joycon left is :" + JoyconInfo.joyconL.bodyColor.HexString());
                joyconLMaskImage.color = JoyconInfo.joyconL.bodyColor;
                joyconLMaskImage2.color = JoyconInfo.joyconL.bodyColor;
            }
            if (JoyconInfo.joyconR.hasBodyColor)
            {
                UnityEngine.Debug.Log("Joycon right is :" + JoyconInfo.joyconR.bodyColor.HexString());
                joyconRMaskImage.color = JoyconInfo.joyconR.bodyColor;
                joyconRMaskImage2.color = JoyconInfo.joyconR.bodyColor;
            }

            if (JoyconInfo.leftConnected && JoyconInfo.rightConnected)
            {
                JoyconLeftMask2.SetActive(true);
                JoyconRightMask2.SetActive(true);
                done();
                return true;
            }

            else if (JoyconInfo.leftConnected)
            {
                leftActive.SetActive(true);
                rightActive.SetActive(false);

                JoyconLeftMask.SetActive(true);
                JoyconRightMask.SetActive(false);
                JoyconLeftMask2.SetActive(true);
                JoyconRightMask2.SetActive(false);
            }
            else if(JoyconInfo.rightConnected)
            {
                leftActive.SetActive(false);
                rightActive.SetActive(true);
                JoyconLeftMask.SetActive(false);
                JoyconRightMask.SetActive(true);
                JoyconLeftMask2.SetActive(false);
                JoyconRightMask2.SetActive(true);
            }
            else
            {
                leftActive.SetActive(false);
                rightActive.SetActive(false);
                JoyconLeftMask.SetActive(false);
                JoyconRightMask.SetActive(false);
                JoyconLeftMask2.SetActive(false);
                JoyconRightMask2.SetActive(false);
            }
            return false;
        }

        public System.Action onUpdateExistDone = ()=> { };

        float showExistTime = 0;

        public void UpdateExist()
        {
            showExistTime += Time.deltaTime;
            if(showExistTime > 10f)
            {
                showExistTime = 0;
                PanelExistJoycon.SetActive(false);
                onUpdateExistDone();
            }

        }
        
        public void Update()
        {
            pullJoyconInterval += Time.deltaTime;
            showExistTime = 0;
            if (pullJoyconInterval > 5)
            {
                pullJoyconInterval = 0;
                JoyconInfo.ResetConnection();
                if (CheckJoyconFullyConnected())
                {
                    return;
                }
            }


            currentTime += Time.unscaledDeltaTime;
            joyconRotateTime += Time.unscaledDeltaTime;

            if (currentTime < ToggleStayOff)
            {

            }
            else if(currentTime < ToggleStayOff + ToggleTransition)
            {
                if(toggleStage == ToggleStage.Off)
                {
                    extWinToggleIndicator.LocalTranslateFromStartTransition(new Vector3(63, 0, 0), ToggleTransition);
                    toggleOnImageColor.a = 1;
                    WinToggleOnImage.colorTransition(toggleOnImageColor, ToggleTransition);
                    toggleStage = ToggleStage.TurningOn;
                }
            }
            else if(currentTime > ToggleStayOff + ToggleTransition + ToggleStayOn)
            {
                if(toggleStage == ToggleStage.TurningOn)
                {
                    extWinToggleIndicator.LocalTranslateFromStartTransition(new Vector3(), ToggleTransition);
                    toggleOnImageColor.a = 0;
                    WinToggleOnImage.colorTransition(toggleOnImageColor, ToggleTransition);
                    toggleStage = ToggleStage.TurningOff;
                }

                if(currentTime > ToggleStayOff + ToggleTransition * 2 + ToggleStayOn)
                {
                    currentTime = 0;
                    toggleStage = ToggleStage.Off;
                }
            
            }

            if(joyconRotateTime > 4)
            {
                joyconRotateTime -= 4;
            }
            var rportion = CurveRotate.Evaluate(joyconRotateTime / 4);

            extJoyconModel.LocalRotateFromStart(new Vector3(
                rportion * 5, rportion * 25 + 15, 0
                ));
        }

        public void OnClickSkip()
        {
            PanelMissingJoycon.SetActive(false);
            done();
        }

        public void OnClickOpenSettings()
        {
            Process.Start("ms-settings:bluetooth");
        }

    }

}
