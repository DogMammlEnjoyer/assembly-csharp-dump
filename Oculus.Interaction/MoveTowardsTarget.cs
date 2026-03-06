using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class MoveTowardsTarget : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this._tween.Pose;
			}
		}

		public bool Stopped
		{
			get
			{
				return this._tween != null && this._tween.Stopped;
			}
		}

		public MoveTowardsTarget(PoseTravelData travellingData)
		{
			this._travellingData = travellingData;
		}

		public void MoveTo(Pose target)
		{
			this._target = target;
			this._tween = this._travellingData.CreateTween(this._source, target);
		}

		public void UpdateTarget(Pose target)
		{
			if (this._target != target)
			{
				this._target = target;
				this._tween.UpdateTarget(this._target);
			}
		}

		public void StopAndSetPose(Pose pose)
		{
			this._source = pose;
			Tween tween = this._tween;
			if (tween == null)
			{
				return;
			}
			tween.StopAndSetPose(this._source);
		}

		public void Tick()
		{
			this._tween.Tick();
		}

		private PoseTravelData _travellingData;

		private Tween _tween;

		private Pose _source;

		private Pose _target;
	}
}
