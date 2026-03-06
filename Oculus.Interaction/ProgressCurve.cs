using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Serializable]
	public class ProgressCurve : ITimeConsumer
	{
		public AnimationCurve AnimationCurve
		{
			get
			{
				return this._animationCurve;
			}
			set
			{
				this._animationCurve = value;
			}
		}

		public float AnimationLength
		{
			get
			{
				return this._animationLength;
			}
			set
			{
				this._animationLength = value;
			}
		}

		[Obsolete("Use SetTimeProvider()")]
		public Func<float> TimeProvider
		{
			get
			{
				return this._timeProvider;
			}
			set
			{
				this._timeProvider = value;
			}
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
		}

		public ProgressCurve()
		{
			this._animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
			this._animationLength = 1f;
		}

		public ProgressCurve(AnimationCurve animationCurve, float animationLength)
		{
			this._animationCurve = animationCurve;
			this._animationLength = animationLength;
		}

		public ProgressCurve(ProgressCurve other)
		{
			this.Copy(other);
		}

		public void Copy(ProgressCurve other)
		{
			this._animationCurve = other._animationCurve;
			this._animationLength = other._animationLength;
			this._animationStartTime = other._animationStartTime;
			this._timeProvider = other._timeProvider;
		}

		public void Start()
		{
			this._animationStartTime = this._timeProvider();
		}

		public float Progress()
		{
			if (this._animationLength <= 0f)
			{
				return this._animationCurve.Evaluate(1f);
			}
			float time = Mathf.Clamp01(this.ProgressTime() / this._animationLength);
			return this._animationCurve.Evaluate(time);
		}

		public float ProgressIn(float time)
		{
			if (this._animationLength <= 0f)
			{
				return this._animationCurve.Evaluate(1f);
			}
			float time2 = Mathf.Clamp01((this.ProgressTime() + time) / this._animationLength);
			return this._animationCurve.Evaluate(time2);
		}

		public float ProgressTime()
		{
			return Mathf.Clamp(this._timeProvider() - this._animationStartTime, 0f, this._animationLength);
		}

		public void End()
		{
			this._animationStartTime = this._timeProvider() - this._animationLength;
		}

		[SerializeField]
		private AnimationCurve _animationCurve;

		[SerializeField]
		private float _animationLength;

		private Func<float> _timeProvider = () => Time.time;

		private float _animationStartTime;
	}
}
