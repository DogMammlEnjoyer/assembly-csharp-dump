using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public struct TeleportHit
	{
		public Vector3 Point
		{
			get
			{
				if (this.relativeTo == null)
				{
					return this._localPose.position;
				}
				Pose pose = this.relativeTo.GetPose(Space.World);
				return PoseUtils.Multiply(pose, this._localPose).position;
			}
		}

		public Vector3 Normal
		{
			get
			{
				if (this.relativeTo == null)
				{
					return this._localPose.rotation * Vector3.forward;
				}
				Pose pose = this.relativeTo.GetPose(Space.World);
				return PoseUtils.Multiply(pose, this._localPose).rotation * Vector3.forward;
			}
		}

		public TeleportHit(Transform relativeTo, Vector3 position, Vector3 normal)
		{
			this.relativeTo = relativeTo;
			Pose localPose = new Pose(position, Quaternion.LookRotation(normal));
			if (relativeTo == null)
			{
				this._localPose = localPose;
				return;
			}
			Pose pose = relativeTo.GetPose(Space.World);
			this._localPose = PoseUtils.Delta(pose, localPose);
		}

		public Transform relativeTo;

		private Pose _localPose;

		public static readonly TeleportHit DEFAULT = new TeleportHit
		{
			relativeTo = null,
			_localPose = Pose.identity
		};
	}
}
