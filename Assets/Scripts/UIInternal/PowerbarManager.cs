using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace UIInternal
{
    [System.Serializable]
    public class PowerbarManager
    {
        public GameObject PanelPowerbar;
        public GameObject Bar;
        public GameObject BarMini;
        public int DeltaX = 4;
        public int NumberForEachSide = 10;
        public Color WeakColor = new Color(197, 90, 17);
        public Color ExactColor = new Color(102, 158, 64);
        public Color ToomuchColor = new Color(197, 90, 17);
        public Sprite SpriteMiniBar;
        public float Scale = 0.03850349f;
        public AnimationCurve curve;
        struct ImageInfo
        {
            public Color start;
            public Image im;
        }
        ImageInfo[] images;

        ExtendedGameObject2D[] barMinis;
        ExtendedGameObject2D CreateObject(int id)
        {

            var obj = new GameObject("BMini" + id.ToString());
            var rect = obj.AddComponent<RectTransform>();
            obj.AddComponent<CanvasRenderer>();

            rect.SetParent(Bar.GetComponent<RectTransform>());
            // rect.parent = ;
            rect.localScale = new Vector3(Scale, Scale, Scale);

            var n = id - NumberForEachSide;
            rect.localPosition = new Vector3(
                n * 4,
                0, 0
                );
            var image = obj.AddComponent<Image>();
            image.sprite = SpriteMiniBar;
            image.preserveAspect = true;

            float t = Mathf.Abs((float)n) / NumberForEachSide;
            Color color = new Color();
            

            if (n < 0 && t > 0.3f)
            {
                color = Color.Lerp(WeakColor, Color.white, 1 - t * 0.75f);
            }
            else if (t <= 0.3f)
            {
                if (n < 0)
                {
                    color = Color.Lerp(ExactColor, Color.white, t / 0.3f * 0.7f);
                }
                else
                {
                    color = Color.Lerp(ExactColor, Color.white, t / 0.3f * 0.7f);
                }
            }
            else
            {
                color = Color.Lerp(ToomuchColor, Color.white, 1-t * 0.75f);
            }
            color.a = 0.8f;
            image.color = color;
            ImageInfo im = new ImageInfo();
            im.start = color;
            im.im = image;
            images[id] = im;

            var ext = new ExtendedGameObject2D(obj);
            return ext;
        }

        public void Init()
        {
            int count = NumberForEachSide * 2 + 1;
            barMinis = new ExtendedGameObject2D[count];
            images = new ImageInfo[count];

            for (int i = 0; i < count; ++i)
            {
                barMinis[i] = CreateObject(i);
            }

            PanelPowerbar.SetActive(false);
        }

        public void Show()
        {
            PanelPowerbar.SetActive(true);
        }


        // 0 ~ 1
        public void SetValue(float v)
        {
            int count = NumberForEachSide * 2 + 1;

            for (int i = 0; i < count; ++i)
            {
                float k = ((float)i) / count - v;
                k =  Mathf.Abs(k);
                float n = curve.Evaluate(1 - k) + 1;
                barMinis[i]
                    .LocalScaleMultiplyFromStartTransition(
                    new Vector3(1.0f + (n-1) * 0.15f, n, 1),
                    0.1f);
                var color = images[i].im.color;
                color.a = n * 0.5f;
                images[i].im.colorTransition(color, 0.1f);
            }
        }

        public void Hide()
        {
            PanelPowerbar.SetActive(false);
        }
    }
}
