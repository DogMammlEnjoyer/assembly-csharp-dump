using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Tween : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this._pose;
			}
		}

		public Pose StartPose
		{
			get
			{
				return this._startPose;
			}
		}

		public bool Stopped
		{
			get
			{
				return this._tweenCurves.TrueForAll((Tween.TweenCurve t) => t.PrevProgress >= 1f);
			}
		}

		public Tween(Pose start, float tweenTime = 0.5f, float maxOverlapTime = 0.25f, AnimationCurve curve = null)
		{
			this._startPose = start;
			this._pose = start;
			this._tweenTime = tweenTime;
			this._maxOverlapTime = maxOverlapTime;
			this._tweenCurves = new List<Tween.TweenCurve>();
			this._animationCurve = (curve ?? AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
			this.TweenToInTime(this._pose, 0f);
		}

		private void TweenToInTime(Pose target, float time)
		{
			Pose current = this._pose;
			if (this._tweenCurves.Count > 0)
			{
				Tween.TweenCurve tweenCurve = this._tweenCurves[this._tweenCurves.Count - 1];
				float num = tweenCurve.Curve.ProgressIn(Mathf.Min(this._maxOverlapTime, time));
				if (num != 1f)
				{
					float num2 = num - tweenCurve.PrevProgress;
					float num3 = 1f - tweenCurve.PrevProgress;
					float t = num2 / num3;
					current = tweenCurve.Current;
					ref current.Lerp(tweenCurve.Target, t);
				}
			}
			Tween.TweenCurve tweenCurve2 = new Tween.TweenCurve
			{
				Curve = new ProgressCurve(this._animationCurve, time),
				PrevProgress = 0f,
				Current = current,
				Target = target
			};
			this._tweenCurves.Add(tweenCurve2);
			tweenCurve2.Curve.Start();
		}

		public void MoveTo(Pose target)
		{
			if (this._pose.Equals(target))
			{
				this.StopAndSetPose(target);
				return;
			}
			this.TweenToInTime(target, this._tweenTime);
		}

		public void UpdateTarget(Pose target)
		{
			this._tweenCurves[this._tweenCurves.Count - 1].Target = target;
		}

		public void StopAndSetPose(Pose source)
		{
			this._tweenCurves.Clear();
			this._pose = source;
			this.TweenToInTime(source, 0f);
		}

		public void Tick()
		{
			for (int i = this._tweenCurves.Count - 1; i >= 0; i--)
			{
				Tween.TweenCurve tweenCurve = this._tweenCurves[i];
				float num = tweenCurve.Curve.Progress();
				if (num == 1f)
				{
					tweenCurve.Current = tweenCurve.Target;
					tweenCurve.PrevProgress = 1f;
				}
				else
				{
					float num2 = num - tweenCurve.PrevProgress;
					float num3 = 1f - tweenCurve.PrevProgress;
					float t = num2 / num3;
					ref tweenCurve.Current.Lerp(tweenCurve.Target, t);
					tweenCurve.PrevProgress = num;
				}
			}
			float num4 = 1f;
			Pose current = this._tweenCurves[this._tweenCurves.Count - 1].Current;
			for (int j = this._tweenCurves.Count - 2; j >= 0; j--)
			{
				Tween.TweenCurve tweenCurve2 = this._tweenCurves[j + 1];
				float b = tweenCurve2.Curve.ProgressTime();
				float num5;
				if (tweenCurve2.Curve.AnimationLength == 0f)
				{
					num5 = 1f;
				}
				else
				{
					num5 = Mathf.Min(this._maxOverlapTime, b) / Mathf.Min(this._maxOverlapTime, tweenCurve2.Curve.AnimationLength);
				}
				if (num5 == 1f)
				{
					this._tweenCurves.RemoveRange(0, j);
					break;
				}
				num4 = (1f - num5) * num4;
				Pose current2 = this._tweenCurves[j].Current;
				ref current.Lerp(current2, num4);
			}
			this._pose = current;
		}

		private List<Tween.TweenCurve> _tweenCurves;

		private Pose _pose;

		private Pose _startPose;

		private float _maxOverlapTime;

		private float _tweenTime;

		private AnimationCurve _animationCurve;

		private class TweenCurve
		{
			public ProgressCurve Curve;

			public float PrevProgress;

			public Pose Current;

			public Pose Target;
		}
	}
}
