using System;
using System.Collections;
using Unity.XR.CoreUtils.Bindings.Variables;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class TweenableVariableBase<T> : BindableVariable<T> where T : IEquatable<T>
	{
		public AnimationCurve animationCurve
		{
			get
			{
				return this.m_AnimationCurve;
			}
			set
			{
				this.m_AnimationCurve = value;
				this.OnAnimationCurveChanged(value);
			}
		}

		public T target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				if (this.m_Target.Equals(value))
				{
					return;
				}
				this.m_Target = value;
				this.OnTargetChanged(this.m_Target);
			}
		}

		public T initialValue { get; set; }

		public void HandleTween(float tweenTarget)
		{
			if (this.ValueEquals(this.target))
			{
				return;
			}
			this.PreprocessTween();
			this.ExecuteTween(base.Value, this.target, tweenTarget, false);
		}

		protected abstract void ExecuteTween(T startValue, T targetValue, float tweenAmount, bool useCurve = false);

		public IEnumerator StartAutoTween(float deltaTimeMultiplier)
		{
			for (;;)
			{
				this.HandleTween(Time.deltaTime * deltaTimeMultiplier);
				yield return null;
			}
			yield break;
		}

		public IEnumerator PlaySequence(T start, T finish, float duration, Action onComplete = null)
		{
			for (float timeElapsed = 0f; timeElapsed < duration; timeElapsed += Time.deltaTime)
			{
				this.PreprocessTween();
				float tweenAmount = Mathf.Clamp01(timeElapsed / duration);
				this.ExecuteTween(start, finish, tweenAmount, true);
				yield return null;
			}
			this.PreprocessTween();
			this.ExecuteTween(start, finish, 1f, false);
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		protected virtual void OnAnimationCurveChanged(AnimationCurve value)
		{
		}

		protected virtual void OnTargetChanged(T newTarget)
		{
		}

		protected virtual void PreprocessTween()
		{
		}

		protected TweenableVariableBase() : base(default(T), true, null, false)
		{
		}

		protected const float k_NearlyOne = 0.99999f;

		private AnimationCurve m_AnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		private T m_Target;
	}
}
