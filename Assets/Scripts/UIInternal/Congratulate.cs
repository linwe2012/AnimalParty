using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
//using UnityEditor.VersionControl;
using Boo.Lang;

namespace UIInternal
{
    [System.Serializable]
    public class Congratulate
    {
        public GameObject PanelCongratulate;

        public GameObject[] Stars = new GameObject[3];
        public GameObject Message;
        public GameObject Congrat;
        public Vector3 MiniScale = new Vector3(0.2f, 0.2f, 0.2f);
        public Vector3 MiniTranslate = new Vector3(0, -20, 0);

        TextMeshProUGUI textMessage;
        TextMeshProUGUI textCongrat;
        System.Action callabck;


        ExtendedGameObject2D[] extStars = new ExtendedGameObject2D[3];
        List<TransitionHelper> stars;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        System.Action doUpdate = () => { };

        bool showing;

        public void Init()
        {
            for(int i = 0; i < Stars.Length; ++i)
            {
                extStars[i] = new ExtendedGameObject2D(Stars[i]);
                //extStars[i]
                //    .LocalTranslateFromStart(MiniTranslate)
                //    .LocalScaleMultiplyFromStart(MiniScale)
                //    ;
                extStars[i].SetActive(false);

            }

            textMessage = Message.GetComponent<TextMeshProUGUI>();
            textCongrat = Congrat.GetComponent<TextMeshProUGUI>();
            PanelCongratulate.SetActive(false);
            showing = false;
        }

        public void Show(int n_stars, string message, string congrat, System.Action callabck_)
        {
            
            callabck = callabck_;
            PanelCongratulate.SetActive(true);
            if (showing) return;
            showing = true;

            for (int i = 0; i < Stars.Length; ++i)
            {
                extStars[i]
                    .LocalTranslateFromStart(MiniTranslate)
                    .LocalScaleMultiplyFromStart(MiniScale)
                    ;
                extStars[i].SetActive(false);
            }

            

            textMessage.text = message;
            textCongrat.text = congrat;

            stars = new List<TransitionHelper>();
            System.Action<int> add = (int id) =>
            {
                stars.Add(new TransitionHelper(extStars[id], curve, 0.2f));
            };

            if(n_stars == 1)
            {
                add(1);
                
            }
            else if(n_stars == 2)
            {
                add(0);
                add(2);
            }
            else if(n_stars == 3)
            {
                add(1);
                add(0);         
                add(2);

            }

            int j = 0;
            foreach (var star in stars)
            {
                star.target.SetActive(true);
                if (j == 0)
                {
                    star.SetRestore();
                    star.startScale = MiniScale;
                    
                }
                else
                {
                    star.SetRestore();
                    star.startScale = MiniScale;
                }

                ++j;

            }
            doUpdate = UpdateStars;
        }

        public void UpdateStars()
        {
            foreach(var star in stars)
            {
                star.Update();
                if (star.animationDone)
                {
                    doUpdate = () => { };
                }
            }
        }

        public void Update()
        {
            doUpdate();
            using (var priviledge = UIManager.EnterUIState())
            {
                if (JoyconInfo.joyconR.GetButtonDown(Joycon.Button.DPAD_RIGHT, ManagedInput.Viewer.UI))
                {
                    Hide();
                    callabck();
                }
            }
        }

        public void Hide()
        {
            PanelCongratulate.SetActive(false);
            showing = false;
        }
        
    }
}
