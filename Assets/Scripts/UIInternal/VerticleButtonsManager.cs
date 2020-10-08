using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace UIInternal
{
    public class VerticleButtonsManager
    {
        SelectBox[] selectBox = new SelectBox[4];
        List<SingleButton> buttons;

        class SingleButton
        {
            GameObject deactiveBG;
            GameObject activeBG;

            Image imActiveBG;
            Image imDeactiveGB;

            Color colorActiveBG;
            Color colorDactiveBG;

            Color colorHidingActiveBG;
            Color colorHidingDactiveBG;

            public RectTransform rectTransform;

            public SingleButton(GameObject parent, int deactiveBGIndex, int activeBGIndex)
            {
                deactiveBG = parent.transform.GetChild(deactiveBGIndex).gameObject;
                activeBG = parent.transform.GetChild(activeBGIndex).gameObject;

                imActiveBG = activeBG.GetComponent<Image>();
                imDeactiveGB = deactiveBG.GetComponent<Image>();

                colorActiveBG = imActiveBG.color;
                colorDactiveBG = imDeactiveGB.color;

                colorHidingActiveBG = colorActiveBG;
                colorHidingDactiveBG = colorDactiveBG;

                colorHidingActiveBG.a = 0;
                colorHidingDactiveBG.a = 0;

                imActiveBG.color = colorHidingActiveBG;

                rectTransform = parent.GetComponent<RectTransform>();
            }

            public void Activate()
            {
                imActiveBG.colorTransition(colorActiveBG, 0.1f, LeanEase.Linear);
                imDeactiveGB.colorTransition(colorHidingDactiveBG, 0.1f, LeanEase.Linear);
            }

            public void Deactivate()
            {
                imActiveBG.colorTransition(colorHidingActiveBG, 0.1f, LeanEase.Linear);
                imDeactiveGB.colorTransition(colorDactiveBG, 0.1f, LeanEase.Linear);
            }
        }

        int indicatorAtIndex;
        float BounceTime;

        ExtendedGameObject2D extActiveUpperPanel;
        ExtendedGameObject2D extActiveLowerPanel;

        Vector3 startPosOfActiveButton;

        float curMoveTime;
        float totalMoveTime;

        Vector3 curPos;
        Vector3 targetPos;
        Vector3 startPos;
        AnimationCurve CurveMove;

        public void Init(
            GameObject ActiveUpperPanel,
            GameObject ActiveLowerPanel,
            GameObject buttonsParent,
            int indicatorAtIndex_,
            float bounceTime_,
            AnimationCurve curveMove,
            float totalMoveTime_,
            int deactiveBGIndex = 0, int activeBGIndex = 1)
        {
            BounceTime = bounceTime_;
            CurveMove = curveMove;

            totalMoveTime = totalMoveTime_;

            System.Action<int, Vector3> setUp =
            (id, dir) =>
            {
                selectBox[id] = new SelectBox(
                    ActiveUpperPanel.transform.GetChild(id).gameObject,
                    ActiveLowerPanel.transform.GetChild(id).gameObject,
                    dir
                    );
            };

            var rect = buttonsParent.GetComponent<RectTransform>().rect;
            float w = rect.width;
            float h = rect.height;

            setUp(0, new Vector3(-w, -h, 0));
            setUp(1, new Vector3(w, -h, 0));
            setUp(2, new Vector3(w, h, 0));
            setUp(3, new Vector3(-w, h, 0));


            extActiveUpperPanel = new ExtendedGameObject2D(ActiveUpperPanel);
            extActiveLowerPanel = new ExtendedGameObject2D(ActiveLowerPanel);

            buttons = new List<SingleButton>();
            int nButtons = buttonsParent.transform.childCount;
            for (int i = 0; i < nButtons; ++i)
            {
                var btn = buttonsParent.transform.GetChild(i).gameObject;
                buttons.Add(
                    new SingleButton(btn, deactiveBGIndex, activeBGIndex)
                    );

                if (indicatorAtIndex_ == i)
                {
                    startPosOfActiveButton = buttons[i].rectTransform.localPosition;
                }
            }

            curPos = new Vector3();
            targetPos = new Vector3();
            startPos = new Vector3();
            curMoveTime = 0;
        }

        float currenTime;

        public int ButtonCount 
        {
            get { return buttons.Count; }
        }

        public int ClampIndex(int index)
        {
            if (index < 0) return 0;
            if (index >= ButtonCount) return ButtonCount - 1;
            return index;
        }

        int lastActiveIndex = -1;

        public void Show(int index)
        {
            currenTime = 0;
            curMoveTime = 0;

            startPos = curPos.Clone();
            if(lastActiveIndex > 0 && lastActiveIndex != index)
            {
                buttons[lastActiveIndex].Deactivate();
            }
            if (index >= 0)
            {
                buttons[index].Activate();

                if(index != lastActiveIndex)
                {
                    curMoveTime = totalMoveTime;
                    startPos = buttons[index].rectTransform.localPosition - startPosOfActiveButton;
                    curPos = startPos.Clone();
                    targetPos = curPos.Clone();

                    extActiveUpperPanel.LocalTranslateFromStart(curPos);
                    extActiveLowerPanel.LocalTranslateFromStart(curPos);
                }
            }
            lastActiveIndex = index;
        }

        public void Update()
        {
            currenTime += Time.unscaledDeltaTime;
            if (currenTime > BounceTime)
            {
                currenTime -= BounceTime;
            }
            foreach (var select in selectBox)
            {
                select.Update(currenTime / BounceTime);
            }

            
            if(curMoveTime < totalMoveTime)
            {
                curMoveTime += Time.unscaledDeltaTime;

                if(curMoveTime > totalMoveTime)
                {
                    curMoveTime = totalMoveTime;
                }

                float portion = CurveMove.Evaluate(curMoveTime / totalMoveTime);
                curPos = Vector3.Lerp(startPos, targetPos, portion);
                extActiveUpperPanel.LocalTranslateFromStart(curPos);
                extActiveLowerPanel.LocalTranslateFromStart(curPos);


            }
            
            
        }


        public void MoveToIndex(int index)
        {
            curMoveTime = 0;
            startPos = curPos.Clone();

            targetPos = buttons[index].rectTransform.localPosition - startPosOfActiveButton;

            if(lastActiveIndex >= 0)
                buttons[lastActiveIndex].Deactivate();
            buttons[index].Activate();

            lastActiveIndex = index;
        }

    }
}
