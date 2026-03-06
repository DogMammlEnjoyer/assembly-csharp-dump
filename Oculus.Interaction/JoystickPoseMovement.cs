using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class JoystickPoseMovement : IMovement
	{
		public Pose Pose
		{
			get
			{
				return this._currentPose;
			}
		}

		public bool Stopped
		{
			get
			{
				return false;
			}
		}

		public JoystickPoseMovement(IController controller, float moveSpeed, float rotationSpeed, float minDistance, float maxDistance)
		{
			this._controller = controller;
			this._moveSpeed = moveSpeed;
			this._rotationSpeed = rotationSpeed;
			this._minDistance = minDistance;
			this._maxDistance = maxDistance;
		}

		public void MoveTo(Pose target)
		{
			this._targetPose = target;
			this._localDirection = Quaternion.Inverse(this._targetPose.rotation) * (this._currentPose.position - this._targetPose.position).normalized;
		}

		public void UpdateTarget(Pose target)
		{
			this._targetPose = target;
		}

		public void StopAndSetPose(Pose pose)
		{
			this._currentPose = pose;
		}

		public void Tick()
		{
			this.AdjustPoseWithJoystickInput();
		}

		public void AdjustPoseWithJoystickInput()
		{
			if (this._controller == null)
			{
				return;
			}
			Vector2 primary2DAxis = this._controller.ControllerInput.Primary2DAxis;
			float num = primary2DAxis.y * this._moveSpeed;
			float angle = -primary2DAxis.x * this._rotationSpeed;
			Vector3 vector = this._targetPose.rotation * this._localDirection;
			Vector3 position = this._targetPose.position;
			float d = Mathf.Clamp(Vector3.Project(this._currentPose.position - position, vector).magnitude + num, this._minDistance, this._maxDistance);
			Vector3 position2 = position + vector * d;
			Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up) * this._currentPose.rotation;
			this._currentPose = new Pose(position2, rotation);
			this.UpdateTarget(this._currentPose);
		}

		public void InjectController(IController controller)
		{
			this._controller = controller;
		}

		private Pose _currentPose;

		private Pose _targetPose;

		private Vector3 _localDirection;

		private IController _controller;

		private float _moveSpeed;

		private float _rotationSpeed;

		private float _minDistance;

		private float _maxDistance;
	}
}
