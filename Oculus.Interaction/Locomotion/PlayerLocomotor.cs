using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class PlayerLocomotor : MonoBehaviour, ILocomotionEventHandler
	{
		public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled
		{
			add
			{
				this._whenLocomotionEventHandled = (Action<LocomotionEvent, Pose>)Delegate.Combine(this._whenLocomotionEventHandled, value);
			}
			remove
			{
				this._whenLocomotionEventHandled = (Action<LocomotionEvent, Pose>)Delegate.Remove(this._whenLocomotionEventHandled, value);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		private void OnEnable()
		{
			if (this._started)
			{
				this.RegisterEndOfFrameCallback(new Action(this.MovePlayer));
			}
		}

		private void OnDisable()
		{
			if (this._started)
			{
				this._deferredEvent.Clear();
				this.UnregisterEndOfFrameCallback();
			}
		}

		public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			this._deferredEvent.Enqueue(locomotionEvent);
		}

		private void MovePlayer()
		{
			while (this._deferredEvent.Count > 0)
			{
				LocomotionEvent arg = this._deferredEvent.Dequeue();
				Pose pose = this._playerOrigin.GetPose(Space.World);
				this.MovePlayer(arg.Pose.position, arg.Translation);
				this.RotatePlayer(arg.Pose.rotation, arg.Rotation);
				Pose pose2 = this._playerOrigin.GetPose(Space.World);
				Pose arg2 = PoseUtils.Delta(pose, pose2);
				this._whenLocomotionEventHandled(arg, arg2);
			}
		}

		private void MovePlayer(Vector3 targetPosition, LocomotionEvent.TranslationType translationMode)
		{
			if (translationMode == LocomotionEvent.TranslationType.None)
			{
				return;
			}
			if (translationMode == LocomotionEvent.TranslationType.Absolute)
			{
				Vector3 b = this._playerOrigin.position - this._playerHead.position;
				b.y = 0f;
				this._playerOrigin.position = targetPosition + b;
				return;
			}
			if (translationMode == LocomotionEvent.TranslationType.AbsoluteEyeLevel)
			{
				Vector3 b2 = this._playerOrigin.position - this._playerHead.position;
				this._playerOrigin.position = targetPosition + b2;
				return;
			}
			if (translationMode == LocomotionEvent.TranslationType.Relative)
			{
				this._playerOrigin.position = this._playerOrigin.position + targetPosition;
				return;
			}
			if (translationMode == LocomotionEvent.TranslationType.Velocity)
			{
				this._playerOrigin.position = this._playerOrigin.position + targetPosition * Time.deltaTime;
			}
		}

		private void RotatePlayer(Quaternion targetRotation, LocomotionEvent.RotationType rotationMode)
		{
			if (rotationMode == LocomotionEvent.RotationType.None)
			{
				return;
			}
			Vector3 position = this._playerHead.position;
			if (rotationMode == LocomotionEvent.RotationType.Absolute)
			{
				Quaternion rhs = Quaternion.LookRotation(Vector3.ProjectOnPlane(this._playerHead.forward, this._playerOrigin.up).normalized, this._playerOrigin.up);
				Quaternion rotation = Quaternion.Inverse(this._playerOrigin.rotation) * rhs;
				this._playerOrigin.rotation = Quaternion.Inverse(rotation) * targetRotation;
			}
			else if (rotationMode == LocomotionEvent.RotationType.Relative)
			{
				this._playerOrigin.rotation = targetRotation * this._playerOrigin.rotation;
			}
			else if (rotationMode == LocomotionEvent.RotationType.Velocity)
			{
				float num;
				Vector3 axis;
				targetRotation.ToAngleAxis(out num, out axis);
				num *= Time.deltaTime;
				this._playerOrigin.rotation = Quaternion.AngleAxis(num, axis) * this._playerOrigin.rotation;
			}
			this._playerOrigin.position = this._playerOrigin.position + position - this._playerHead.position;
		}

		public void InjectAllPlayerLocomotor(Transform playerOrigin, Transform playerHead)
		{
			this.InjectPlayerOrigin(playerOrigin);
			this.InjectPlayerHead(playerHead);
		}

		public void InjectPlayerOrigin(Transform playerOrigin)
		{
			this._playerOrigin = playerOrigin;
		}

		public void InjectPlayerHead(Transform playerHead)
		{
			this._playerHead = playerHead;
		}

		[SerializeField]
		private Transform _playerOrigin;

		[SerializeField]
		private Transform _playerHead;

		private Action<LocomotionEvent, Pose> _whenLocomotionEventHandled = delegate(LocomotionEvent <p0>, Pose <p1>)
		{
		};

		protected bool _started;

		private Queue<LocomotionEvent> _deferredEvent = new Queue<LocomotionEvent>();
	}
}
