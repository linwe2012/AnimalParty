using Lean.Transition;
using System.Xml.Schema;
using TMPro;
using UnityEngine;

static partial class VectorExtentions
{
    static public Vector3 ElementWiseMul(this Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(
            lhs.x * rhs.x,
            lhs.y * rhs.y,
            lhs.z * rhs.z
            );
    }

    static public Vector3 Clone(this Vector3 lhs)
    {
        return new Vector3(lhs.x, lhs.y, lhs.z);
    }

    static public string PrettyString(this Vector3 v)
    {
        return string.Format("Vector3({0}, {1}, {2})", v.x, v.y, v.z);
    }
}

static partial class ColorExtenions
{
    static public string HexString(this Color color)
    {
        byte r = (byte)Mathf.RoundToInt(color.r * 255);
        byte g = (byte)Mathf.RoundToInt(color.g * 255);
        byte b = (byte)Mathf.RoundToInt(color.b * 255);
        byte a = (byte)Mathf.RoundToInt(color.a * 255);

        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
    }
}

namespace Leon
{
    public class TranslateTransition
    {

    }
}


public class ExtendedGameObject
{
    public Vector3 lastPosition;
    public Vector3 lastScale;
    public Vector3 lastRotation;

    public GameObject target;
    public Transform transform {  get { return target.transform; } }

    Transform myTransform;

    public ExtendedGameObject(GameObject _target, bool is2D = false)
    {
        lastRotation = _target.transform.localRotation.eulerAngles;
        lastPosition = _target.transform.localPosition;
        lastScale = _target.transform.localScale;
        target = _target;
        if(is2D)
        {
            myTransform = _target.GetComponent<RectTransform>();
        }
        else
        {
            myTransform = _target.transform;
        }
    }

    public ExtendedGameObject SetActive(bool active)
    {
        target.SetActive(active);
        return this;
    }

    public ExtendedGameObject LocalTranslateFromStart(Vector3 direction)
    {
        myTransform.localPosition = lastPosition + direction;
        return this;
    }

    public ExtendedGameObject LocalTranslateFromStartTransition(Vector3 direction, float duration, LeanEase ease = LeanEase.Smooth)
    {
        myTransform.localPositionTransition(lastPosition + direction, duration, ease);
        return this;
    }

    public ExtendedGameObject LocalScaleAdditionFromStartTransition(Vector3 add_scale, float duration, LeanEase ease = LeanEase.Smooth)
    {
        myTransform.localScaleTransition(lastScale + add_scale, duration, ease);
        return this;
    }

    public ExtendedGameObject LocalScaleAdditionFromStart(Vector3 add_scale)
    {
        myTransform.localScale = (lastScale + add_scale);
        return this;
    }

    public ExtendedGameObject LocalScaleMultiplyFromStartTransition(Vector3 mul_scale, float duration, LeanEase ease = LeanEase.Smooth)
    {
        myTransform.localScaleTransition(mul_scale.ElementWiseMul(lastScale), duration, ease);
        return this;
    }

    public ExtendedGameObject LocalRotateFromStart(Vector3 rotation)
    {
        myTransform.localEulerAngles = lastRotation + rotation;

        return this;
    }


    public ExtendedGameObject LocalRotateFromStartTransition(Vector3 rotation, float duration, LeanEase ease = LeanEase.Smooth)
    {
        myTransform.localEulerAnglesTransform(lastRotation + rotation, duration, ease);
        return this;
    }

    public ExtendedGameObject JoinTransition()
    {
        myTransform.JoinTransition();
        return this;
    }

    public ExtendedGameObject SetTimeOut(System.Action action, float delay)
    {
        myTransform.EventTransition(action, delay);
        return this;
    }
}

public class ExtendedGameObject2D
{
    public Vector3 lastPosition;
    public Vector3 lastScale;
    public Vector3 lastRotation;

    public Vector3 startPosition
    {
        get { return lastPosition; }
    }

    public RectTransform transform { get { return graphic; } }
    public RectTransform graphic;
    public GameObject target;

    public ExtendedGameObject2D(GameObject _target)
    {
        target = _target;
        graphic = target.GetComponent<RectTransform>();
        

        lastPosition =graphic.localPosition;
        lastScale = graphic.localScale;
        lastRotation = graphic.localRotation.eulerAngles;
    }

    

    public ExtendedGameObject2D SetActive(bool active)
    {
        target.SetActive(active);
        return this;
    }

    public ExtendedGameObject2D LocalTranslateFromStart(Vector3 direction)
    {
        graphic.localPosition = lastPosition + direction;
        return this;
    }

    public ExtendedGameObject2D LocalTranslateFromStartTransition(Vector3 direction, float duration, LeanEase ease = LeanEase.Smooth)
    {
        graphic.localPositionTransition(lastPosition + direction, duration, ease);
        return this;
    }

    public ExtendedGameObject2D LocalScaleAdditionFromStart(Vector3 add_scale)
    {
        graphic.localScale = (lastScale + add_scale);
        return this;
    }

    public ExtendedGameObject2D LocalScaleMultiplyFromStart(Vector3 mul_scale)
    {
        graphic.localScale = lastScale.ElementWiseMul(mul_scale);
        return this;
    }

    public ExtendedGameObject2D LocalScaleAdditionFromStartTransition(Vector3 add_scale, float duration, LeanEase ease = LeanEase.Smooth)
    {
        graphic.localScaleTransition(lastScale + add_scale, duration, ease);
        return this;
    }

    public ExtendedGameObject2D LocalScaleMultiplyFromStartTransition(Vector3 mul_scale, float duration, LeanEase ease = LeanEase.Smooth)
    {
        graphic.localScaleTransition(mul_scale.ElementWiseMul(lastScale), duration, ease);
        return this;
    }

    public ExtendedGameObject2D LocalRotateFromStartTransition(Vector3 rotation, float duration, LeanEase ease = LeanEase.Smooth)
    {
        graphic.localEulerAnglesTransform(lastRotation + rotation, duration, ease);
        return this;
    }

    public ExtendedGameObject2D JoinTransition()
    {
        graphic.JoinTransition();
        return this;
    }

    public ExtendedGameObject2D SetTimeOut(System.Action action, float delay)
    {
        graphic.EventTransition(action, delay);
        return this;
    }
}

namespace UIInternal
{
    public class TransitionHelper
    {
        public float currentTime;
        public float totalTime;

        public Vector3 startPosition;
        public Vector3 targetPosition;
        public Vector3 currentPosition;

        public Vector3 startScale;
        public Vector3 targetScale;
        public Vector3 currentScale;

        public ExtendedGameObject2D target;

        AnimationCurve curve;

        public bool animationDone
        {
            get { return currentTime >= totalTime; }
        }

        void ConrtructMe(ExtendedGameObject2D target_, AnimationCurve curve_, float totalTime_)
        {
            target = target_;

            curve = curve_;

            currentTime = totalTime_;
            totalTime = totalTime_;

            startPosition = new Vector3();
            targetPosition = new Vector3();
            currentPosition = new Vector3();

            startScale = new Vector3(1, 1, 1);
            targetScale = new Vector3(1, 1, 1);
            currentScale = new Vector3(1, 1, 1);
        }

        public TransitionHelper(ExtendedGameObject2D target_, AnimationCurve curve_, float totalTime_)
        {
            ConrtructMe(target_, curve_, totalTime_);
        }

        public TransitionHelper(GameObject target_, AnimationCurve curve_, float totalTime_)
        {
            var ext = new ExtendedGameObject2D(target_);

            ConrtructMe(ext, curve_, totalTime_);
        }

        public float targetX {
            set
            {
                SetTargetPosition(new Vector3(value, targetPosition.y, targetPosition.z));
            }
        }

        public void SetTargetPosition(Vector3 target)
        {
            currentTime = 0;
            targetPosition = target;
            startPosition = currentPosition.Clone();
        }

        public void SetTargetScaleMultiply(Vector3 target)
        {
            currentTime = 0;
            targetScale = target;
            startScale = currentScale.Clone();
        }

        public void SetRestore()
        {
            SetTargetPosition(new Vector3());
            SetTargetScaleMultiply(new Vector3(1, 1, 1));
        }

        public void Update()
        {
            if (currentTime >= totalTime)
            {
                return;
            }

            currentTime += Time.unscaledDeltaTime;
            if (currentTime >= totalTime)
            {
                currentTime = totalTime;
            }

            var portion = curve.Evaluate(currentTime / totalTime);
            currentPosition = Vector3.Lerp(startPosition, targetPosition, portion);
            currentScale = Vector3.Lerp(startScale, targetScale, portion);
            target.LocalTranslateFromStart(currentPosition);
            target.LocalScaleMultiplyFromStart(currentScale);
        }
    }
}


namespace Leon.Animation
{
    public abstract class Animatable
    {
        abstract public void Update();

    }

    public class Transformer: Animatable
    {
        AnimationCurve curve;
        float current;
        float total;

        Vector3 from;
        Vector3 to;
        

        public override void Update()
        {
            current += Time.deltaTime;
            curve.Evaluate(current / total);
            Vector3.Lerp(from, to, current);
        }


        
    }
}
