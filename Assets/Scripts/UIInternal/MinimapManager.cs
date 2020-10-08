using Lean.Gui;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UIInternal;
using UnityEditor;
using System;
using Boo.Lang;

namespace UIInternal
{
#if UNITY_EDITOR
    namespace Inspector
    {
        public class NamedArrayAttribute : PropertyAttribute
        {
            public readonly string[] names;
            public NamedArrayAttribute(string[] names) { this.names = names; }
        }

        [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
        public class NamedArrayDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                try
                {
                    int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                    EditorGUI.ObjectField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
                }
                catch
                {
                    EditorGUI.ObjectField(rect, property, label);
                }
            }
        }

        public class EnumArrayAttribute : PropertyAttribute
        {
            public readonly string[] names;
            public EnumArrayAttribute(Type type) { this.names = Enum.GetNames(type); }
        }

        [CustomPropertyDrawer(typeof(EnumArrayAttribute))]
        public class EnumArrayDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                try
                {
                    int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                    EditorGUI.ObjectField(rect, property, new GUIContent(((EnumArrayAttribute)attribute).names[pos]));
                }
                catch
                {
                    EditorGUI.ObjectField(rect, property, label);
                }
            }
        }
    }
#endif




    class MinimapBlinker
    {
        public GameObject vanilla;
        public GameObject glow;

        Image imVanilla;
        Image imGlow;

        static AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        static float totalTime = 2.0f;

        bool active = false;

        static Color colorHidden = new Color(1, 1, 1, 0);
        static Color colorShow = new Color(1, 1, 1, 0);
        public float currentTime;
        public float delltaT;

        public MinimapBlinker(GameObject vanilla_)
        {
            vanilla = vanilla_;
            glow = vanilla.transform.GetChild(0).gameObject;

            imVanilla = vanilla.GetComponent<Image>();
            imGlow = glow.GetComponent<Image>();

            vanilla.SetActive(false);
            currentTime = 0;
            delltaT = 1;
        }

        public void SetActive(bool active_)
        {
            if(active != active_)
            {
                currentTime = 0;
                delltaT = 1;
            }
            active = active_;
            vanilla.SetActive(active);

        }

        public void Update()
        {
            // if (!active) return;
            if (currentTime > totalTime)
            {
                delltaT = -1;
            }
            if(currentTime < 0)
            {
                delltaT = 1;
            }

            currentTime += delltaT * Time.deltaTime;
            

            float portion = curve.Evaluate(Mathf.Clamp01(currentTime / totalTime));
            // var under = new Color(1, 1, 1, portion);
            var top = new Color(1, 1, 1, portion);

            // imVanilla.color = under;
            imGlow.color = top;
        }
    }

    public class GameObjectTracer
    {
        public GameObject icon;
        public GameObject tracing;

        Camera minimapCamera;
        MinimapBlinker iconBlink;

        ExtendedGameObject2D extIcon;
        Vector3 imageScale;
        public static float Scale = 1;

        public GameObjectTracer(GameObject icon_, GameObject tracing_, Camera cam, Vector3 imageScale_)
        {
            icon = icon_;
            tracing = tracing_;
            minimapCamera = cam;
            imageScale = imageScale_;
            iconBlink = new MinimapBlinker(icon);
            extIcon = new ExtendedGameObject2D(icon);
            iconBlink.SetActive(true);
        }

        public void Update()
        {
            iconBlink.Update();
            var scr_point = minimapCamera.WorldToScreenPoint(tracing.transform.position);
            scr_point.z = 0;
            scr_point -= new Vector3(256, 256, 0) * 0.5f;
            scr_point /= 0.5f;
            
            if(scr_point.magnitude > Scale)
            {
                scr_point = scr_point.normalized * Scale;
            }

            extIcon.LocalTranslateFromStart(scr_point);
            // Debug.Log(scr_point.sqrMagnitude);
        }

    }

    [System.Serializable]
    public class MinimapManager
    {
        public enum Icon
        {
            BlueTarget,
            Elephant
        }

        public GameObject PanelMinimap;
        public Camera camera;
        // public Image TargetImage;
        public Image ArrowImage;
        public float theScale;
#if UNITY_EDITOR
        [Inspector.EnumArray(typeof(Icon))]
#endif
        public GameObject[] TracerIcons;

        MinimapBlinker arrowBlink;

        Camera following;

        List<GameObjectTracer> tracers;
        TransitionHelper transPanelMinimap;
        System.Action updateEmapsize;

        // 在小地图上绘制一个图标，这个图标会跟踪物体
        public MinimapManager AddIconTracerIcon(Icon icon, GameObject target)
        {
            var iconObj = TracerIcons[(int)icon];
            var tracer = new GameObjectTracer(iconObj, target, camera, PanelMinimap.transform.localScale);
            tracers.Add(tracer);

            return this;
        }

        public void Init()
        {
            PanelMinimap.SetActive(false);
            arrowBlink = new MinimapBlinker(ArrowImage.gameObject);
            arrowBlink.SetActive(true);
            tracers = new List<GameObjectTracer>();
            transPanelMinimap = new TransitionHelper(PanelMinimap, AnimationCurve.EaseInOut(0, 0, 1, 1), 0.2f);
            updateEmapsize = () => { };
        }

        public void FollowCamera(Camera cam)
        {
            PanelMinimap.SetActive(true);
            following = cam;
        }

        public MinimapManager SetViewSize(float f)
        {
            camera.orthographicSize = f;
            return this;
        }

        void TransPanelMinimapUpdator()
        {
            transPanelMinimap.Update();
            if (transPanelMinimap.animationDone)
            {
                updateEmapsize = () => { };
            }
        }

        public void EmphasizeMiniMap()
        {
            transPanelMinimap.SetTargetPosition(new Vector3(-550, 400, 0));
            transPanelMinimap.SetTargetScaleMultiply(new Vector3(2.5f, 2.5f, 2.5f));
            updateEmapsize = TransPanelMinimapUpdator;
        }

        public void EmphasizeMiniMapStop()
        {
            transPanelMinimap.SetRestore();

            updateEmapsize = TransPanelMinimapUpdator;
        }

        public void Update()
        {
            if (following != null)
            {
                updateEmapsize();
                arrowBlink.Update();
                GameObjectTracer.Scale = theScale;
                foreach (var tracer in tracers)
                {
                    tracer.Update();
                }

                camera.transform.position = new Vector3(
                    following.transform.position.x,
                    camera.transform.position.y,
                    following.transform.position.z
                    );

                //float z = following.transform.rotation.eulerAngles.y;
                var forward = following.transform.forward;

                float z = Mathf.Atan2(forward.z, forward.x) * 180 / Mathf.PI;

                ArrowImage.rectTransform.rotation = Quaternion.Euler(
                    new Vector3(
                        0,
                        0,
                        z - 90)
                    );
            }


        }

        public void Unfollow()
        {
            PanelMinimap.SetActive(false);
            following = null;
        }
    }
}
