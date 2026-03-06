using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ObjectPull : IMovement
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
				return this._reachedGrabber;
			}
		}

		public ObjectPull(float speed, float deadZone)
		{
			this._speed = speed;
			this._deadZone = deadZone;
		}

		public void MoveTo(Pose target)
		{
			this._grabberStartPose = target;
			this._target = target;
			this._current = this._grabbableStartPose;
			this._lastTime = Time.time;
			this._reachedGrabber = false;
			Vector3 vector = this._grabbableStartPose.position - this._grabberStartPose.position;
			this._originalDistance = vector.magnitude;
			this._pullingPlane = new Plane(vector.normalized, this._grabberStartPose.position);
		}

		public void UpdateTarget(Pose target)
		{
			this._target = target;
		}

		public void StopAndSetPose(Pose source)
		{
			this._grabbableStartPose = source;
		}

		public void Tick()
		{
			if (this._reachedGrabber)
			{
				this._current = this._target;
				return;
			}
			float d = Time.time - this._lastTime;
			this._lastTime = Time.time;
			float distanceToPoint = this._pullingPlane.GetDistanceToPoint(this._target.position);
			if (Mathf.Abs(distanceToPoint) < this._deadZone)
			{
				return;
			}
			Vector3 normalized = (this._current.position - this._target.position).normalized;
			this._translationDelta = normalized * distanceToPoint * this._speed * d;
			if (Vector3.Distance(this._current.position, this._target.position) < this._translationDelta.magnitude)
			{
				this._reachedGrabber = true;
				this._current = this._target;
				return;
			}
			this._current.position = this._current.position + this._translationDelta;
			float num = Vector3.Distance(this._current.position, this._target.position);
			float t = 1f - Mathf.Clamp01(num / this._originalDistance);
			this._current.rotation = Quaternion.Slerp(this._grabbableStartPose.rotation, this._target.rotation, t);
		}

		private float _speed = 1f;

		private float _deadZone;

		private Pose _current = Pose.identity;

		private Pose _grabberStartPose;

		private Pose _grabbableStartPose;

		private Pose _target;

		private Plane _pullingPlane;

		private Vector3 _translationDelta = Vector3.zero;

		private float _lastTime;

		private float _originalDistance;

		private bool _reachedGrabber;
	}
}
