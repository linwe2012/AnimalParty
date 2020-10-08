using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Transition;

namespace UIInternal
{
    [System.Serializable]
    public class NewItemHintManager
    {
        public GameObject PanelNewItemHint;
        public GameObject ItemHint;

        AnimationCurve CurveEaseInOut;
        System.Action onDoneCallback;

        Item currentItem = null;

        class TextImageManager
        {
            public ExtendedGameObject2D extIm;
            public ExtendedGameObject2D extText;
            public Image im;
            public TextMeshProUGUI tmp;

            public TransitionHelper transText;
            public TransitionHelper transIm;
            public int textCount;

            static Vector3 singleTextDx = new Vector3(18, 0, 0);
            static Vector3 ImageDx = new Vector3(43, 0, 0);
            Vector3 posImYZ;
            Vector3 posTextYZ;

            System.Action doUpdate;
            Item theItem;

            public TextImageManager(ExtendedGameObject2D extIm_, ExtendedGameObject2D extText_, AnimationCurve curve)
            {
                extIm = extIm_;
                extText = extText_;

                im = extIm.target.GetComponent<Image>();
                tmp = extText.target.GetComponent<TextMeshProUGUI>();

                transIm = new TransitionHelper(extIm, curve, 0.2f);
                transText = new TransitionHelper(extText, curve, 0.2f);

                //var s1 = extIm.startPosition;
                //posImYZ = new Vector3(0, s1.y, s1.z);

                //s1 = extText.startPosition;
                //posTextYZ = new Vector3(0, s1.y, s1.z);

                posImYZ = new Vector3();
                posTextYZ = new Vector3();

                textCount = 0;
                doUpdate = () => { };
            }

            public void Update()
            {
                doUpdate();
            }

            void UpdateNormal()
            {
                transText.Update();
                transIm.Update();
            }

            void UpdateDying()
            {
                transText.Update();
                transIm.Update();
                if (transText.animationDone)
                {
                    doUpdate = UpdateDoNothing;
                }
            }

            void UpdateDoNothing()
            {

            }

            public void Hide()
            {
                extIm.SetActive(false);
                extText.SetActive(false);
            }

            public void SetEnteringAnimated(Item item)
            {
                theItem = item;
                doUpdate = UpdateNormal;

                extIm.SetActive(true);
                extText.SetActive(true);

                tmp.alpha = 0;
                tmp.text = item.itemName + ",";
                tmp.colorTransition(new Color(1, 1, 1, 1), 0.2f);

                var len = item.itemName.Length + 1;
                textCount = len;

                transText.currentPosition = singleTextDx * len * -0.5f + posTextYZ;
                transText.targetX = ImageDx.x;

                im.color = new Color(1, 1, 1, 0);
                im.colorTransition(new Color(1, 1, 1, 1), 0.2f);
                im.sprite = item.itemImage;

                transIm.currentPosition = ImageDx * -1 + posImYZ;
                transIm.targetX = 0;
            }

            public void SetEntering(Item item)
            {
                theItem = item;
                doUpdate = UpdateDoNothing;

                extIm.SetActive(true);
                extText.SetActive(true);

                tmp.text = item.itemName;
                tmp.alpha = 1;

                transText.currentPosition = new Vector3();
                transText.targetPosition = new Vector3();
                extText.LocalTranslateFromStart(new Vector3());

                transIm.currentPosition = new Vector3();
                transIm.currentPosition = new Vector3();
                extIm.LocalTranslateFromStart(new Vector3());
                im.sprite = item.itemImage;

                textCount = item.itemName.Length;

                im.color = new Color(1, 1, 1, 1);
            }

            public void SetShiftRight(TextImageManager prev, bool textAddComma)
            {
                doUpdate = UpdateNormal;
                
                if (textAddComma)
                {
                    var txt = theItem.itemName + ",";
                    textCount = txt.Length;
                    tmp.text = txt;
                }

                var pos = prev.textCount * singleTextDx.x + ImageDx.x;
                
                transText.targetX = pos;
                transIm.targetX = ImageDx.x;
            }

            public void SetShiftingHiding(int len)
            {
                doUpdate = UpdateDying;

                tmp.text = "...";
                transText.targetX = singleTextDx.x * len + ImageDx.x;
                transIm.targetX = ImageDx.x * 2;

                tmp.colorTransition(new Color(1, 1, 1, 1), 0.2f);
                im.colorTransition(new Color(1, 1, 1, 0), 0.2f);
            }
        }

        ExtendedGameObject2D extItemHint;
        TransitionHelper transItemHint;
        List<TextImageManager> tmManager;


        Vector3 posHiddenItemHint;

        Item item1;
        Item item2;

        Vector3 textDx;

        System.Action doUpdate;


        bool isShowing = false;
        int numberOfItems = 0;

        float showingTime = 0;

        int firstIndex = 0;


        public void Init()
        {
            CurveEaseInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

            tmManager = new List<TextImageManager>();

            for(int i = 0; i < 3; ++i)
            {
                var obj1 = ItemHint.transform.GetChild(i).gameObject;
                var obj2 = ItemHint.transform.GetChild(i+3).gameObject;

                var tm = new TextImageManager(new ExtendedGameObject2D(obj1), new ExtendedGameObject2D(obj2), CurveEaseInOut);
                tmManager.Add(tm);
            }

            currentItem = null;


            posHiddenItemHint = new Vector3(300, 0, 0);
            extItemHint = new ExtendedGameObject2D(ItemHint);
            transItemHint = new TransitionHelper(extItemHint, CurveEaseInOut, 0.2f);

            ItemHint.SetActive(false);
            PanelNewItemHint.SetActive(false);
            doUpdate = () => { };
            // PanelNewItemHint.SetActive(false);
        }

        public void Show(Item item, System.Action onDone)
        {
            ItemHint.SetActive(true);
            PanelNewItemHint.SetActive(true);

            onDoneCallback = onDone;

            showingTime = 0;
            if(!isShowing)
            {
                tmManager[2].Hide();
                tmManager[1].Hide();
                tmManager[0].SetEntering(item);

                extItemHint.LocalTranslateFromStart(new Vector3(300, 0, 0));
                transItemHint.currentPosition = new Vector3(300, 0, 0);
                transItemHint.targetX = 0;

                numberOfItems = 1;
                firstIndex = 0;
                isShowing = true;
            }
            else
            {
                if(item == item1 || item == item2)
                {
                    return;
                }

                var lastLastFirstIndex = (firstIndex + 3 - 1) % 3;
                var lastFirstIndex = firstIndex;
                firstIndex = (firstIndex + 1) % 3;

                if (numberOfItems == 1)
                {
                    tmManager[firstIndex].SetEnteringAnimated(item);
                    tmManager[lastFirstIndex].SetShiftRight(tmManager[firstIndex], false);
                }
                else
                {
                    tmManager[firstIndex].SetEnteringAnimated(item);
                    tmManager[lastFirstIndex].SetShiftRight(tmManager[firstIndex], true);
                    var l1 = tmManager[firstIndex].textCount;
                    var l2 = tmManager[lastFirstIndex].textCount;

                    tmManager[lastLastFirstIndex].SetShiftingHiding(l1 + l2);
                }
                ++numberOfItems;
                // firstIndex = lastFirstIndex;
            }
            
            doUpdate = UpdateNormal;
        }

        public void Update()
        {
            doUpdate();
        }

        void UpdateNormal()
        {
            transItemHint.Update();
            foreach(var tm in tmManager)
            {
                tm.Update();
            }

            showingTime += Time.unscaledDeltaTime;
            if (showingTime > UIManager.Settings.hintStayingTime)
            {
                // 我们已经进入隐藏状态了
                isShowing = false;
                doUpdate = UpdateClosing;
                transItemHint.targetX = posHiddenItemHint.x;
            }
        }

        void UpdateClosing()
        {
            transItemHint.Update();
            if (transItemHint.animationDone)
            {
                numberOfItems = 0;
                item1 = null;
                item2 = null;

                ItemHint.SetActive(false);
                PanelNewItemHint.SetActive(false);
                onDoneCallback();
                return;
            }
        }

    }
}
