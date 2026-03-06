using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	[Serializable]
	public abstract class BaseAffordanceTheme<T> : IEquatable<BaseAffordanceTheme<T>> where T : struct
	{
		public AnimationCurve animationCurve
		{
			get
			{
				return this.m_StateAnimationCurve.Value;
			}
		}

		protected BaseAffordanceTheme()
		{
			this.m_List = new List<AffordanceThemeData<T>>
			{
				new AffordanceThemeData<T>
				{
					stateName = "disabled"
				},
				new AffordanceThemeData<T>
				{
					stateName = "idle"
				},
				new AffordanceThemeData<T>
				{
					stateName = "hovered"
				},
				new AffordanceThemeData<T>
				{
					stateName = "hoveredPriority"
				},
				new AffordanceThemeData<T>
				{
					stateName = "selected"
				},
				new AffordanceThemeData<T>
				{
					stateName = "activated"
				},
				new AffordanceThemeData<T>
				{
					stateName = "focused"
				}
			};
		}

		internal void ValidateTheme()
		{
			if (this.m_List == null)
			{
				return;
			}
			int count = this.m_List.Count;
			int num = (int)AffordanceStateShortcuts.stateCount - count;
			if (num > 0)
			{
				AffordanceThemeData<T> affordanceThemeData;
				if (count < 2)
				{
					affordanceThemeData = new AffordanceThemeData<T>
					{
						stateName = "idle"
					};
				}
				else
				{
					affordanceThemeData = this.m_List[1];
				}
				while (num-- > 0)
				{
					AffordanceThemeData<T> item = new AffordanceThemeData<T>
					{
						stateName = affordanceThemeData.stateName,
						animationStateStartValue = affordanceThemeData.animationStateStartValue,
						animationStateEndValue = affordanceThemeData.animationStateEndValue
					};
					this.m_List.Add(item);
					byte b = (byte)(this.m_List.Count - 1);
					string nameForIndex = AffordanceStateShortcuts.GetNameForIndex(b);
					this.m_List[(int)b].stateName = nameForIndex;
					Debug.LogWarning(string.Format("Found missing state {0} \"{1}\" in your affordance theme. Adding missing state with idle state data.", b, nameForIndex));
				}
			}
		}

		public AffordanceThemeData<T> GetAffordanceThemeDataForIndex(byte stateIndex)
		{
			if ((int)stateIndex >= this.m_List.Count)
			{
				return null;
			}
			return this.m_List[(int)stateIndex];
		}

		public void SetAffordanceThemeDataList(List<AffordanceThemeData<T>> newList)
		{
			this.m_List.Clear();
			this.m_List.AddRange(newList);
		}

		public virtual void CopyFrom(BaseAffordanceTheme<T> other)
		{
			this.m_List = new List<AffordanceThemeData<T>>(other.m_List);
			this.m_StateAnimationCurve = other.m_StateAnimationCurve;
		}

		public void SetAnimationCurve(AnimationCurve newAnimationCurve)
		{
			this.m_StateAnimationCurve.Value = newAnimationCurve;
		}

		public bool Equals(BaseAffordanceTheme<T> other)
		{
			return other != null && (this == other || (object.Equals(this.m_StateAnimationCurve, other.m_StateAnimationCurve) && object.Equals(this.m_List, other.m_List)));
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((BaseAffordanceTheme<T>)obj)));
		}

		public override int GetHashCode()
		{
			return (17 * 31 + this.m_StateAnimationCurve.GetHashCode()) * 31 + this.m_List.GetHashCode();
		}

		[SerializeField]
		[Tooltip("Curve used to evaluate the target value of the animation state according to the affordance state's transition amount value.")]
		private AnimationCurveDatumProperty m_StateAnimationCurve = new AnimationCurveDatumProperty(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

		[SerializeField]
		[Tooltip("List of affordance states supported by this theme. The entry index is how states are mapped to their theme data.\nDo not re-order entries.")]
		private List<AffordanceThemeData<T>> m_List;
	}
}
