/*
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the specified Graphic.color to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanGraphicColor")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Graphic/Font.color" + LeanTransition.MethodsMenuSuffix + "(LeanGraphicColor)")]
	public class LeanFontWeight : LeanMethodWithStateAndTarget
	{
		public override System.Type GetTargetType()
		{
			return typeof(TextMeshProUGUI);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.fontWeight, Data.Duration, Data.Ease);
		}

		public static LeanState Register(TextMeshProUGUI target, FontWeight fontWeight, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var data = LeanTransition.RegisterWithTarget(State.Pool, duration, target);

			data.fontWeight = fontWeight;
			data.Ease = ease;


			return data;
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<TextMeshProUGUI>
		{
			[Tooltip("The color we will transition to.")]
			public FontWeight fontWeight = FontWeight.Regular;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease = LeanEase.Smooth;

			[System.NonSerialized] private FontWeight oldFontWeight = FontWeight.Regular;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.fontWeight != fontWeight ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				fontWeight = Target.fontWeight;
				// Color = Target.color;
			}

			public override void BeginWithTarget()
			{
				oldFontWeight = Target.fontWeight;
			}

			public override void UpdateWithTarget(float progress)
			{
				Target.fontWeight = fon // Color.LerpUnclamped((int)oldFontWeight, (int)fontWeight, Smooth(Ease, progress));
			}

			public static Stack<State> Pool = new Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static TextMeshProUGUI fontWeightTransition(this TextMeshProUGUI target, FontWeight color, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanFontWeight.Register(target, color, duration, ease); return target;
		}
	}
}*/