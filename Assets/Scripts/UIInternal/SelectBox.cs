using TMPro;
using Lean.Gui;
using Lean.Transition;
using UnityEngine;
using UnityEngine.UI;

namespace UIInternal
{
    public class SelectBox
    {
        public GameObject Triangle;
        public GameObject Glow;
        ExtendedGameObject2D extTriangle;
        ExtendedGameObject2D extGlow;

        public Vector3 direction;

        public static AnimationCurve curve;
        public static float scale;

        public SelectBox(GameObject _Triangle, GameObject _Glow, Vector3 _direction)
        {
            Triangle = _Triangle;
            Glow = _Glow;

            extTriangle = new ExtendedGameObject2D(Triangle);
            extGlow = new ExtendedGameObject2D(Glow);
            direction = _direction.normalized;
        }


        public void Update(float time)
        {
            Vector3 toward;
            if(time < 0.5f)
            {
                time *= 2f;
                toward = direction * scale * time;
                
            }
            else
            {
                time = 1 - time;
                time *= 2f;
                toward = direction * scale * time;
            }
            extTriangle.LocalTranslateFromStart(toward);
            extGlow.LocalTranslateFromStart(toward);
        }

        
    }
}
