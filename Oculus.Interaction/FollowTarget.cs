using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class FollowTarget : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this.ToWorld(this._localPose);
			}
		}

		public bool Stopped
		{
			get
			{
				return false;
			}
		}

		public FollowTarget(float speed, Transform space)
		{
			this._speed = speed;
			this._space = space;
		}

		private Pose ToLocal(in Pose pose)
		{
			Vector3 position = this._space.InverseTransformPoint(pose.position);
			Quaternion rotation = Quaternion.Inverse(this._space.rotation) * pose.rotation;
			return new Pose(position, rotation);
		}

		private Pose ToWorld(in Pose pose)
		{
			Vector3 position = this._space.TransformPoint(pose.position);
			Quaternion rotation = this._space.rotation * pose.rotation;
			return new Pose(position, rotation);
		}

		public void MoveTo(Pose target)
		{
			this._startTime = Time.time;
			this._localTarget = this.ToLocal(target);
		}

		public void UpdateTarget(Pose target)
		{
			this._localTarget = this.ToLocal(target);
			this.Tick();
		}

		public void StopAndSetPose(Pose source)
		{
			this._localPose = this.ToLocal(source);
		}

		public void Tick()
		{
			float time = Time.time;
			float num = (time - this._startTime) * this._speed;
			this._startTime = time;
			this._localPose.position = Vector3.MoveTowards(this._localPose.position, this._localTarget.position, num);
			this._localPose.rotation = Quaternion.RotateTowards(this._localPose.rotation, this._localTarget.rotation, num * 50f);
		}

		private float _speed;

		private Transform _space;

		private Pose _localTarget;

		private Pose _localPose;

		private float _startTime;

		private const float ROTATION_SPEED_FACTOR = 50f;
	}
}
