using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneMovement
	{
		private float PositionSpeed
		{
			get
			{
				return this._moveSpeed * Time.deltaTime;
			}
		}

		private float RotationSpeed
		{
			get
			{
				return this._rotationSpeed * Time.deltaTime;
			}
		}

		private float SmoothPositionSpeed
		{
			get
			{
				return Time.deltaTime / this._moveSmoothness;
			}
		}

		private float SmoothRotationSpeed
		{
			get
			{
				return Time.deltaTime / this._rotationSmoothness;
			}
		}

		public DroneMovement(Transform droneTransform, Transform gimbalTransform)
		{
			this._droneTransform = droneTransform;
			this._gimbalTransform = gimbalTransform;
			this._localTargetPosition = this._droneTransform.localPosition;
			this._localTargetRotation = this._droneTransform.localRotation;
			this._tiltTargetRotation = this._gimbalTransform.localRotation;
		}

		public void Run()
		{
			if (this._smoothMovement)
			{
				this._droneTransform.localPosition = Vector3.Lerp(this._droneTransform.localPosition, this._localTargetPosition, this.SmoothPositionSpeed);
				this._droneTransform.localRotation = Quaternion.Slerp(this._droneTransform.localRotation, this._localTargetRotation * (this._useTiltAsDirection ? this._tiltTargetRotation : Quaternion.identity), this.SmoothRotationSpeed);
			}
			else
			{
				this._droneTransform.localPosition = this._localTargetPosition;
				this._droneTransform.localRotation = this._localTargetRotation;
			}
			if (this._smoothRotation)
			{
				this._gimbalTransform.localRotation = Quaternion.Slerp(this._gimbalTransform.localRotation, ((!this._useTiltAsDirection) ? this._tiltTargetRotation : Quaternion.identity) * this._rollTargetRotation, this.SmoothRotationSpeed);
				return;
			}
			this._gimbalTransform.localRotation = this._tiltTargetRotation * this._rollTargetRotation;
		}

		public void MoveForward()
		{
			this._localTargetPosition += this._droneTransform.forward * this.PositionSpeed;
		}

		public void MoveBackward()
		{
			this._localTargetPosition -= this._droneTransform.forward * this.PositionSpeed;
		}

		public void MoveLeft()
		{
			this._localTargetPosition -= this._droneTransform.right * this.PositionSpeed;
		}

		public void MoveRight()
		{
			this._localTargetPosition += this._droneTransform.right * this.PositionSpeed;
		}

		public void MoveUp()
		{
			this._localTargetPosition += this._droneTransform.up * this.PositionSpeed;
		}

		public void MoveDown()
		{
			this._localTargetPosition -= this._droneTransform.up * this.PositionSpeed;
		}

		public void RotateLeft()
		{
			this._localTargetRotation *= Quaternion.Euler(0f, -this.RotationSpeed, 0f);
		}

		public void RotateRight()
		{
			this._localTargetRotation *= Quaternion.Euler(0f, this.RotationSpeed, 0f);
		}

		public void TiltUp()
		{
			this._tiltAngle -= this.RotationSpeed;
			this.ProcessTilt();
		}

		public void TiltDown()
		{
			this._tiltAngle += this.RotationSpeed;
			this.ProcessTilt();
		}

		private void ProcessTilt()
		{
			this._tiltAngle = Mathf.Clamp(this._tiltAngle, this._tiltUpAngle, this._tiltDownAngle);
			this._tiltTargetRotation = Quaternion.AngleAxis(this._tiltAngle, Vector3.right);
		}

		private void ProcessRoll()
		{
			this._rollTargetRotation = Quaternion.AngleAxis(this._rollAngle, Vector3.forward);
		}

		private void ProcessMovement(Vector2 stick, float trigger)
		{
			Vector3 a = this._droneTransform.forward * stick.y + this._droneTransform.right * stick.x + this._droneTransform.up * trigger;
			this._localTargetPosition += a * this.PositionSpeed;
		}

		public void ResetTillAndRoll()
		{
			this._tiltAngle = 0f;
			this._rollAngle = 0f;
			this.ProcessTilt();
			this.ProcessRoll();
		}

		public void MoveForwardBackwardLeftRight(Vector2 v)
		{
			if (this._snapAxis)
			{
				if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
				{
					v.y = 0f;
				}
				else
				{
					v.x = 0f;
				}
			}
			this.ProcessMovement(v, 0f);
		}

		public void MoveUpAndDown(float f)
		{
			this.ProcessMovement(Vector2.zero, f);
		}

		public void TiltAndRotateGamePad(Vector2 v)
		{
			v.y *= -1f;
			this.TiltAndRotate(v);
		}

		public void TiltAndRotateMouse(Vector2 v)
		{
			v /= 2f;
			v.y *= -1f;
			if (this._isMouseInverted)
			{
				v *= -1f;
			}
			this.TiltAndRotate(v);
		}

		private void TiltAndRotate(Vector2 v)
		{
			if (this._snapAxis)
			{
				if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
				{
					v.y = 0f;
				}
				else
				{
					v.x = 0f;
				}
			}
			this._localTargetRotation *= Quaternion.Euler(0f, this.RotationSpeed * v.x, 0f);
			this._tiltAngle += this.RotationSpeed * v.y;
			this.ProcessTilt();
		}

		public void Roll(Vector2 v)
		{
			v.x = (this._isMouseInverted ? v.x : (-v.x));
			this._rollAngle += this.RotationSpeed * v.x;
			this.ProcessRoll();
		}

		public void SetMoveSpeedChanged(float speed)
		{
			this._moveSpeed = speed;
		}

		public void SetMoveSmoothness(float smoothness)
		{
			this._moveSmoothness = smoothness;
		}

		public void SetRotationSpeed(float speed)
		{
			this._rotationSpeed = speed;
		}

		public void SetRotationSmoothness(float smoothness)
		{
			this._rotationSmoothness = smoothness;
		}

		public void SetSnapAxis(bool snap)
		{
			this._snapAxis = snap;
		}

		public void SetIsSmoothMovement(bool smooth)
		{
			this._smoothMovement = smooth;
		}

		public void SetIsSmoothRotation(bool smooth)
		{
			this._smoothRotation = smooth;
		}

		public void SetUseTiltAsDirection(bool use)
		{
			this._useTiltAsDirection = use;
		}

		public void SetIsMouseInverted(bool inverted)
		{
			this._isMouseInverted = inverted;
		}

		public void MoveAndRotateDroneInstantly(Vector3 position, Quaternion rotation)
		{
			this._droneTransform.localPosition = position;
			this._droneTransform.localRotation = rotation;
			this._localTargetPosition = position;
			this._localTargetRotation = rotation;
		}

		private float _moveSpeed = 1f;

		private float _rotationSpeed = 90f;

		private float _tiltUpAngle = -45f;

		private float _tiltDownAngle = 90f;

		private bool _smoothMovement = true;

		private bool _smoothRotation = true;

		private float _moveSmoothness = 1f;

		private float _rotationSmoothness = 1f;

		private bool _snapAxis;

		private bool _useTiltAsDirection;

		private bool _isMouseInverted;

		private Transform _gimbalTransform;

		private Transform _droneTransform;

		private Vector3 _localTargetPosition;

		private Quaternion _localTargetRotation;

		private Quaternion _tiltTargetRotation = Quaternion.identity;

		private Quaternion _rollTargetRotation = Quaternion.identity;

		private float _tiltAngle;

		private float _rollAngle;
	}
}
