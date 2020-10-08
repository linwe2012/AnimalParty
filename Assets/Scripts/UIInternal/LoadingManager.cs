using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

namespace UIInternal
{
    [System.Serializable]
    public class LoadingManager
    {
        public GameObject PanelLoading;
        public GameObject TextObject;
        public GameObject AnimalCoverGrey;
        public GameObject Mask;
        public GameObject AnimalCover;
        public RawImage Background;

        TextMeshProUGUI text;
        RectTransform rectAnimalCover;
        RectTransform rectPanelLoading;
        ExtendedGameObject2D extMask;
        Sprite spriteBackground;
        UIHUD hud;

        public float currentValue = 0;
        public float targetValue = 1;

        public void Init(UIHUD _hud)
        {
            text = TextObject.GetComponent<TextMeshProUGUI>();
            extMask = new ExtendedGameObject2D(Mask);
            PanelLoading.SetActive(false);
            //CoverSprite = AnimalCover.GetComponent<SpriteRenderer>();
            rectAnimalCover = AnimalCover.GetComponent<RectTransform>();
            rectPanelLoading = PanelLoading.GetComponent<RectTransform>();
            hud = _hud;
        }

        public void SetValue(float value)
        {
            targetValue = value;
            //rectAnimalCover.parent = rectPanelLoading;
            //extMask.LocalTranslateFromStart(new Vector3(-317, 0, 0) * (1 - value));
            //rectAnimalCover.parent = extMask.graphic;
            //CoverSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        public void Update()
        {
            if(currentValue > targetValue)
            {
                return;
            }
            currentValue += 0.1f * Time.deltaTime;
            rectAnimalCover.parent = rectPanelLoading;
            extMask.LocalTranslateFromStart(new Vector3(-317, 0, 0) * (1 - currentValue));
            rectAnimalCover.parent = extMask.graphic;
        }

        IEnumerator CaptureScreen()
        {
            yield return new WaitForEndOfFrame();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();

            
            // var center = new Rect(0, 0, texture.width, texture.height);
            // spriteBackground = Sprite.Create(texture, center, new Vector2(0.5f, 0.5f), 100.0f);

            // PanelLoading.GetComponent<Image>().sprite = spriteBackground;
            Background.texture = texture;
            PanelLoading.SetActive(true);
        }

        public void Show(string _text)
        {
            //int width = Camera.main.pixelWidth;
            //int height = Camera.main.pixelHeight;

            //int width = Screen.width;
            //int height = Screen.height;

            //int width = RenderTexture.active.width;
            //int height = RenderTexture.active.height;
            text.text = _text;
            currentValue = 0;
            targetValue = 0.8f;
            hud.StartCoroutine(CaptureScreen());
        }

       

        public void Hide()
        {
            PanelLoading.SetActive(false);
        }

    }
}
