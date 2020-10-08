using Lean.Gui;
using Lean.Transition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UIInternal;

// TODO: 增加 Padding Animals UI
public class UIHUD : MonoBehaviour
{
    // public GameObject[] Buttons = new GameObject[8];
    // public GameObject Panel;
    public GameObject PanelAim;
    public AnimationCurve EaseAimEnter;
    public AnimationCurve EaseAimExit;
    public AnimationCurve EaseAimBlur;
    public GameObject PanelSuccess;

    public GameObject PanelDialog;
    DialogManager dialogManager;
    
    public GameObject[] ItemsInHandIcons = new GameObject[1];

    public LoadingManager loading;

    public PowerbarManager powerBar;
    public Congratulate congratulate;
    public MissingJoyconManager missingJoycon;
    public ABXYHelpManager abxyHelp;
    public NewItemHintManager newItemHintManager;

    System.Action parallelDoUpdate;
    System.Action parallelDoUpdate2;
    System.Action parallelDoUpdate3;

    public class DialogManager
    {
        struct Item
        {
            public GameObject Image;
            public GameObject Text;
            public GameObject Title;
            public TextMeshProUGUI TM_Text;
            public TextMeshProUGUI TM_Title;
            public GameObject theBtext;

            public void Init()
            {
                TM_Text = Text.GetComponent<TextMeshProUGUI>();
                TM_Title = Title.GetComponent<TextMeshProUGUI>();
                Hide();
            }

            public void Show(string text, string title, string BText)
            {
                Image.SetActive(true);
                Text.SetActive(true);
                Title.SetActive(true);

                TM_Text.text = text;
                TM_Title.text = title;

                if(BText != null)
                {
                    theBtext.SetActive(true);
                }
                else
                {
                    theBtext.SetActive(false);
                }
            }
            public void Hide()
            {
                Image.SetActive(false);
                Text.SetActive(false);
                Title.SetActive(false);
                theBtext.SetActive(false);
            }

        }
        UIManager.DialogAt lastDialogAt;

        Item[] items;
        GameObject PanelDialog;

        public DialogManager(GameObject _PanelDialog)
        {
            items = new Item[1];
            PanelDialog = _PanelDialog;
            for (int i = 0; i < items.Length; ++i)
            {
                items[i].Image = PanelDialog.transform.GetChild(i * 5).gameObject;
                items[i].Text = PanelDialog.transform.GetChild(i * 5 + 1).gameObject;
                items[i].Title = PanelDialog.transform.GetChild(i * 5 + 2).gameObject;
                items[i].theBtext = PanelDialog.transform.GetChild(i * 5 + 4).gameObject;
                items[i].Init();
            }
            PanelDialog.SetActive(false);
        }

        public void Show(UIManager.DialogAt dialogAt, string text, string title, string BText)
        {
            PanelDialog.SetActive(true);
            int at = (int)dialogAt;
            items[at].Show(text, title, BText);
            lastDialogAt = dialogAt;
        }

        public void Hide()
        {
            int at = (int)lastDialogAt;
            items[at].Hide();
            PanelDialog.SetActive(false);
        }
    }

    

    [System.Serializable]
    public class JoyconStick
    {
        public GameObject PanelJoycon;
        public GameObject Stick;
        public GameObject StickActiveRight;
        public GameObject StickActiveLeft;
        public GameObject StickGreyRight;
        public GameObject StickGreyLeft;
        public GameObject StickHelpText;

        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [HideInInspector]
        public ExtendedGameObject2D extPanelJoycon;

        [HideInInspector]
        public ExtendedGameObject2D extStick;

        [HideInInspector]
        public LeanJoystick leanJoystick;

        [HideInInspector]
        public Image PanelImage;

        [HideInInspector]
        public Color startPanelImageColor;

        [HideInInspector]
        public Color hidePanelImageColor;

        [HideInInspector]
        public Vector3 HideTranslate;

        [HideInInspector]
        public Vector3 HideScale;

        ExtendedGameObject2D extJoyStickHandle;
        ExtendedGameObject2D extJoyStickRing;
        int random_move = 0;
        int max_random_move = 0;
        bool last_user_move = false;
        Random rand = new Random();
        bool is_falling_back = false;

        

        public void Init()
        {

            extPanelJoycon = new ExtendedGameObject2D(PanelJoycon);
            extStick = new ExtendedGameObject2D(Stick);

            leanJoystick = Stick.GetComponent<LeanJoystick>();

            HideTranslate = new Vector3(0, 200, 0);
            HideScale = new Vector3(0.2f, 0.2f, 0.2f);

            extPanelJoycon
                .LocalScaleMultiplyFromStart(HideScale)
                .LocalTranslateFromStart(HideTranslate);

            extJoyStickHandle = new ExtendedGameObject2D(
                Stick.transform.GetChild(1).gameObject
                );
            extJoyStickRing = new ExtendedGameObject2D(
                Stick.transform.GetChild(0).gameObject
                );

            PanelImage = PanelJoycon.GetComponent<Image>();
            startPanelImageColor = PanelImage.color;
            hidePanelImageColor = PanelImage.color;
            hidePanelImageColor.a *= 0.2f;
            PanelImage.color = hidePanelImageColor;
            extPanelJoycon.SetActive(false);

            StickActiveRight.SetActive(false);
            StickActiveLeft.SetActive(false);

        }

        public void AnimationHide(float duration)
        {

            extPanelJoycon
                .LocalScaleMultiplyFromStartTransition(HideScale, duration, LeanEase.Accelerate)
                .LocalTranslateFromStartTransition(HideTranslate, duration, LeanEase.Accelerate)
                .transform.EventTransition(() =>
                {
                    extPanelJoycon.SetActive(false);
                }, duration);

            PanelImage.colorTransition(hidePanelImageColor, duration, LeanEase.Accelerate);
        }

        public void AnimationShow(string name, float duration)
        {
            StickHelpText.GetComponent<TextMeshProUGUI>().text = name;
            extPanelJoycon.SetActive(true);
            extPanelJoycon
                .LocalScaleAdditionFromStartTransition(new Vector3(), duration, LeanEase.Smooth)
                .LocalTranslateFromStartTransition(new Vector3(), duration, LeanEase.Smooth);

            PanelImage.colorTransition(startPanelImageColor, duration, LeanEase.Smooth);
        }

        void FallbackToCenter()
        {
            is_falling_back = true;
            extJoyStickHandle
                .LocalRotateFromStartTransition(new Vector3(), 0.1f)
                .graphic.EventTransition(() =>
                {
                    is_falling_back = false;
                }, 0.1f);
        }

        public bool CheckUserStickMoved(Vector2 v)
        {
            return v.sqrMagnitude != 0;
        }

        public bool UserInteract(out Vector2 vec)
        {

            if(CheckUserStickMoved(JoyconInfo.rightStick))
            {

                StickActiveRight.SetActive(true);
                StickGreyLeft.SetActive(true);

                StickGreyRight.SetActive(false);
                StickActiveLeft.SetActive(false);


                vec = JoyconInfo.rightStick;
                return true;
            }

            if (CheckUserStickMoved(JoyconInfo.leftStick))
            {
                StickActiveLeft.SetActive(true);
                StickGreyRight.SetActive(true);

                StickGreyLeft.SetActive(false);
                StickActiveRight.SetActive(false);

                vec = JoyconInfo.leftStick;
                return true;
            }

            vec = new Vector2();
            return false;
        }
        
        public void AnimationHandleInput()
        {
            if(UserInteract(out Vector2 dir))
            {
                if(!last_user_move)
                {
                    extJoyStickHandle.LocalScaleMultiplyFromStartTransition(new Vector3(2, 2, 2), 0.1f);
                }
                extJoyStickHandle.LocalTranslateFromStart(dir * 100);
                random_move = 0;
                last_user_move = true;
            }
            else if(last_user_move)
            {
                extJoyStickHandle.LocalScaleMultiplyFromStartTransition(new Vector3(1, 1, 1), 0.1f);
                StickActiveRight.SetActive(false);
                StickActiveLeft.SetActive(false);

                StickGreyLeft.SetActive(true);
                StickGreyRight.SetActive(true);
                
                FallbackToCenter();
                last_user_move = false;
            }
            else if(is_falling_back)
            {
                return;
            }
            else
            {
                if(random_move == 0)
                {
                    max_random_move = Random.Range(2, 4);
                }
            }
        }

    }
    public JoyconStick joyconStick = new JoyconStick();
    public MinimapManager miniMapManager = new MinimapManager();


    public struct JoyconExistance<T>
    {

        public T ActiveLeft;
        public T ActiveRight;
        public T GreyLeft;
        public T GreyRight;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1: return ActiveLeft;
                    case 2: return ActiveRight;
                    case 3: return GreyLeft;
                    case 4: return GreyRight;
                    default: return default(T);
                }
            }
        }

    }
    public JoyconExistance<GameObject> joyconExistance = new JoyconExistance<GameObject>();


    ExtendedGameObject2D[] aimers = new ExtendedGameObject2D[4];
    Vector3[] aimerDirections = new Vector3[4];
    public float aimerDirectionDistance = 140;
    float aimerCurrentDirectionDistance = 1.0f;

    System.Action updateAction = () => { };

    public System.Action InjectUpdateAction { set { updateAction = value; } }

    struct TextInfo
    {
        public float y;
        public float height;
    }
    TextInfo[] text_y = new TextInfo[4];

    bool hasBeenInited = false;
    public void Init()
    {
        if (hasBeenInited)
        {
            return;
        }

        hasBeenInited = true;
        parallelDoUpdate = () => { };

        for (int i = 0; i < 4; ++i)
        {
            aimers[i] = new ExtendedGameObject2D(PanelAim.transform.GetChild(i).gameObject);
            var rot = (90 - aimers[i].graphic.localRotation.eulerAngles.z) * Mathf.PI / 180.0f;
            var sign = 1;
            if (rot == 0)
            {
                sign = -1;
            }
            aimerDirections[i] = -sign * Mathf.Sign(rot) * new Vector3(Mathf.Sin(rot), Mathf.Cos(rot), 0).normalized;
        }

        PanelAim.SetActive(false);
        joyconStick.Init();
        dialogManager = new DialogManager(PanelDialog);
        miniMapManager.Init();
        powerBar.Init();
        loading.Init(this);
        congratulate.Init();
        missingJoycon.Init();
        abxyHelp.Init();
        newItemHintManager.Init();
        parallelDoUpdate2 = () => { };
        parallelDoUpdate3 = () => { };
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    /// <summary>
    /// 显示准心
    /// </summary>
    public void ShowAim()
    {
        var scale = aimerDirectionDistance;

        PanelAim.SetActive(true);
        for(int i = 0; i < 3; ++i)
        {
            // aimers[i].LocalTranslateFromStart(xdir_vec);
            aimers[i].LocalTranslateFromStart(aimerDirections[i] * scale) ;
        }
    }

    /// <summary>
    /// 将准心聚焦到中心
    /// </summary>
    public void ShowAimFocus()
    {
        var scale = aimerDirectionDistance;

        int translate = 0;
        float time = 0;
        const float contract_time = 2.2f;
        const float bouncy_time = 1.6f;

        updateAction = () =>
        {
            time += Time.deltaTime;
            float new_scale = scale;

            if (translate == 0)
            {
                new_scale *= (1 - time / contract_time);

                if (time > contract_time)
                {
                    translate = 1;
                    time -= contract_time;
                }
            }
            else
            {
                if (time > bouncy_time)
                {
                    time -= bouncy_time;
                }

                if (time < bouncy_time / 2)
                {
                    var eaval = EaseAimEnter.Evaluate(time / bouncy_time * 2);
                    new_scale *= 0.2f * eaval;
                }
                else
                {
                    var eaval = EaseAimExit.Evaluate((1 - time / bouncy_time) * 2);
                    new_scale *= 0.2f * eaval;
                }
            }


            for (int i = 0; i < 3; ++i)
            {
                aimers[i].LocalTranslateFromStart(aimerDirections[i] * new_scale);
            }

            aimerCurrentDirectionDistance = new_scale;
        };
    }

    /// <summary>
    /// 准心失焦
    /// </summary>
    public void ShowAimBlur()
    {
        var scale = aimerDirectionDistance;

        float blur_time = aimerCurrentDirectionDistance / aimerDirectionDistance;
        
        updateAction = () =>
        {
            blur_time += Time.deltaTime;
            if (blur_time > 1)
            {
                updateAction = () => { };
                return;
            }

            float dist = EaseAimBlur.Evaluate(blur_time);
            dist *= aimerDirectionDistance;

            for (int i = 0; i < 3; ++i)
            {
                aimers[i].LocalTranslateFromStart(aimerDirections[i] * dist);
            }

            aimerCurrentDirectionDistance = dist;
        };
    }

    public void ShowJoyconStick(string name)
    {
        joyconStick.AnimationShow(name, 0.2f);
        updateAction = () =>
        {
            joyconStick.AnimationHandleInput();
        };

    }

    public void ShowJoyconStickHide()
    {
        joyconStick.AnimationHide(0.2f);
        updateAction = () => { };
    }

    public void ShowAimHide()
    {
        PanelAim.SetActive(false);
        updateAction = () => { };
    }

    public void ShowMinimap(Camera camera)
    {
        miniMapManager.FollowCamera(camera);
    }

    public void ShowJoyconConnectTutorial(System.Action callback)
    {
        updateAction = () =>
        {
            missingJoycon.Update();
        };

        missingJoycon.Show(() =>
        {
            updateAction = () => { };
            parallelDoUpdate3 = missingJoycon.UpdateExist;
            missingJoycon.onUpdateExistDone = () =>
            {
                parallelDoUpdate3 = () => { };
            };
            callback();
        });
    }

    public void ShowMinimapHide()
    {
        miniMapManager.Unfollow();
    }

    public MinimapManager SettingsMinimap()
    {
        return miniMapManager;
    }

    /// <summary>
    /// 显示一个对话框
    /// </summary>
    /// <param name="dialogAt">Dilog 的具体位置</param>
    /// <param name="text">diolog 内容</param>
    /// <param name="speaker">dialog的说话人, 展示在Dialog左上角</param>
    public void ShowDialog(UIManager.DialogAt dialogAt, string text, string speaker, string BText = null)
    {
        dialogManager.Show(dialogAt, text, speaker, BText);
    }

    public void ShowDialogHide()
    {
        dialogManager.Hide();
    }

    
    public void ShowHelp(UIManager.Button button, string message)
    {
        parallelDoUpdate2 = () =>
        {
            abxyHelp.Update();
        };

        abxyHelp.Show(button, message, () =>
        {
            parallelDoUpdate2 = () => { };
        });
    }

    public void ShowNewItemInInventoryHint(Item item)
    {
        newItemHintManager.Show(item, () =>
        {
            parallelDoUpdate = () => { };
        });

        parallelDoUpdate = () =>
        {
            newItemHintManager.Update();
        };
    }

    public void ShowHelpHide()
    {
        abxyHelp.MarkShouldClose();
        //Panel.SetActive(false);
        //updateAction = () => { };
    }

    public void ShowPowerBar()
    {
        powerBar.Show();
    }

    public void ShowPowerBarHide()
    {
        powerBar.Hide();
    }

    public void ShowLoading(string message)
    {
        loading.Show(message);
        updateAction = () => { loading.Update(); };
    }

    public void ShowLoadingHide()
    {
        loading.Hide();
        updateAction = () => { };
    }

    public void ShowCongraluate(int n_stars, string message, string congrat = "恭喜过关", System.Action next = null)
    {
        if(next == null)
        {
            next = () => { };
        }

        JoyconInfo.Blind(ManagedInput.Viewer.Default, true);
        congratulate.Show(n_stars, message, congrat, ()=>
        {
            updateAction = () => { };
            JoyconInfo.Blind(ManagedInput.Viewer.Default, false);
            next();
        });

        updateAction = () =>
        {
            congratulate.Update();
        };

    }

    public void ShowCongraluateHide()
    {
        congratulate.Hide();
    }

    public void ShowLoadingValue(float value)
    {
        loading.SetValue(value);
    }


    // value 是 0~1, 其中  0.5 是最佳, 0 是力气过小, 1 是用力过猛
    public void ShowPowerBarValue(float value)
    {
        powerBar.SetValue(value);
    }

    public void LateUpdate()
    {
        updateAction();
        miniMapManager.Update();
        parallelDoUpdate();
        parallelDoUpdate2();
        parallelDoUpdate3();
    }

    public void OnClick_MissingJoycon_Skip()
    {
        missingJoycon.OnClickSkip();
    }

    public void OnClick_MissingJoycon_OpenSettings()
    {
        missingJoycon.OnClickOpenSettings();
    }

}
