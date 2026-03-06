using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class MoveRelativeToTarget : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this._current;
			}
		}

		public bool Stopped
		{
			get
			{
				return true;
			}
		}

		public void MoveTo(Pose target)
		{
			this._originalTarget = target;
		}

		public void UpdateTarget(Pose target)
		{
			Pose pose = new Pose(this._originalSource.position, this._originalTarget.rotation);
			Pose pose2 = PoseUtils.Delta(this._originalTarget, target);
			PoseUtils.Multiply(pose, pose2, ref this._current);
		}

		public void StopAndSetPose(Pose source)
		{
			this._originalSource = source;
			this._current = source;
		}

		public void Tick()
		{
		}

		private Pose _current = Pose.identity;

		private Pose _originalTarget;

		private Pose _originalSource;
	}
}
