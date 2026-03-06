using System;
using UnityEngine;

namespace Liv.Lck.Smoothing
{
	[DefaultExecutionOrder(1000)]
	public class LckSimpleStabilizer : MonoBehaviour
	{
		private void OnEnable()
		{
			this._lastPosition = this._targetToFollow.position;
			this._lastRotation = this._targetToFollow.rotation;
		}

		private void LateUpdate()
		{
			this._stabilizationTarget.position = Vector3.SmoothDamp(this._lastPosition, this._targetToFollow.position, ref this._velocity, this._followTime);
			this._stabilizationTarget.rotation = SmoothingUtils.SmoothDampQuaternion(this._lastRotation, this._targetToFollow.rotation, ref this._rotationVelocity, this._rotateTime);
			this._lastPosition = this._stabilizationTarget.position;
			this._lastRotation = this._stabilizationTarget.rotation;
		}

		public void ReachTargetInstantly()
		{
			this._stabilizationTarget.position = this._targetToFollow.position;
			this._stabilizationTarget.rotation = this._targetToFollow.rotation;
		}

		public Transform _stabilizationTarget;

		public Transform _targetToFollow;

		public float _followTime = 0.1f;

		public float _rotateTime = 0.1f;

		private Vector3 _velocity = Vector3.zero;

		private Vector3 _rotationVelocity = Vector3.zero;

		private Vector3 _lastPosition;

		private Quaternion _lastRotation;
	}
}
