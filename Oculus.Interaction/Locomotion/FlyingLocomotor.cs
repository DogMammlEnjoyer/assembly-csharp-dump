using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class FlyingLocomotor : MonoBehaviour, ILocomotionEventHandler, IDeltaTimeConsumer
	{
		public float Acceleration
		{
			get
			{
				return this._acceleration;
			}
			set
			{
				this._acceleration = value;
			}
		}

		public float AirDamping
		{
			get
			{
				return this._airDamping;
			}
			set
			{
				this._airDamping = value;
			}
		}

		public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
		{
			this._deltaTimeProvider = deltaTimeProvider;
		}

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

		public bool IsGrounded
		{
			get
			{
				return this._characterController.IsGrounded;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._endOfFrameRoutine = base.StartCoroutine(this.EndOfFrameCoroutine());
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._accumulatedDeltaFrame = Pose.identity;
				base.StopCoroutine(this._endOfFrameRoutine);
				this._endOfFrameRoutine = null;
			}
		}

		protected virtual void Update()
		{
			this.CatchUpCharacterToPlayer();
			this.UpdateVelocity();
			Pose pose = this._characterController.Pose;
			Vector3 delta = this._velocity * this._deltaTimeProvider();
			this._characterController.Move(delta);
			Pose pose2 = this._characterController.Pose;
			this.AccumulateDelta(ref this._accumulatedDeltaFrame, pose, pose2);
		}

		protected virtual void LateUpdate()
		{
			this.ConsumeDeferredLocomotionEvents();
		}

		protected virtual void LastUpdate()
		{
			this.CatchUpPlayerToCharacter(this._accumulatedDeltaFrame, this.GetCharacterFeet().y);
			this._accumulatedDeltaFrame = Pose.identity;
		}

		public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Velocity)
			{
				this.AddVelocity(locomotionEvent.Pose.position);
				this._whenLocomotionEventHandled(locomotionEvent, locomotionEvent.Pose);
				return;
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
			{
				this._velocity = Vector3.zero;
			}
			this._deferredLocomotionEvent.Enqueue(locomotionEvent);
		}

		private void ConsumeDeferredLocomotionEvents()
		{
			if (this._deferredLocomotionEvent.Count == 0)
			{
				return;
			}
			Pose pose = this._characterController.Pose;
			while (this._deferredLocomotionEvent.Count > 0)
			{
				LocomotionEvent locomotionEvent = this._deferredLocomotionEvent.Dequeue();
				this.HandleDeferredLocomotionEvent(locomotionEvent);
			}
			Pose pose2 = this._characterController.Pose;
			this.AccumulateDelta(ref this._accumulatedDeltaFrame, pose, pose2);
		}

		private void HandleDeferredLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			Pose pose = this._characterController.Pose;
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute)
			{
				this.MoveAbsoluteFeet(locomotionEvent.Pose.position);
			}
			else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel)
			{
				this.MoveAbsoluteHead(locomotionEvent.Pose.position);
			}
			else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
			{
				this.MoveRelative(locomotionEvent.Pose.position);
			}
			if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Absolute)
			{
				this.RotateAbsolute(locomotionEvent.Pose.rotation);
			}
			else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Relative)
			{
				this.RotateRelative(locomotionEvent.Pose.rotation);
			}
			else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Velocity)
			{
				this.RotateVelocity(locomotionEvent.Pose.rotation);
			}
			Pose pose2 = this._characterController.Pose;
			Pose identity = Pose.identity;
			this.AccumulateDelta(ref identity, pose, pose2);
			this._whenLocomotionEventHandled(locomotionEvent, identity);
		}

		private void AccumulateDelta(ref Pose accumulator, in Pose from, in Pose to)
		{
			accumulator.position = accumulator.position + to.position - from.position;
			accumulator.rotation = Quaternion.Inverse(from.rotation) * to.rotation * accumulator.rotation;
		}

		private void AddVelocity(Vector3 velocity)
		{
			this._velocity += velocity * this.GetModifiedSpeedFactor();
		}

		private void MoveAbsoluteFeet(Vector3 target)
		{
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = target - characterFeet;
			Vector3 position = this._characterController.Pose.position + b;
			this._characterController.SetPosition(position);
		}

		private void MoveAbsoluteHead(Vector3 target)
		{
			Vector3 characterHead = this.GetCharacterHead();
			Vector3 b = target - characterHead;
			Vector3 position = this._characterController.Pose.position + b;
			this._characterController.SetPosition(position);
		}

		private void MoveRelative(Vector3 offset)
		{
			this._velocity = Vector3.zero;
			this._characterController.Move(offset);
		}

		private void RotateAbsolute(Quaternion target)
		{
			this._characterController.SetRotation(target);
		}

		private void RotateRelative(Quaternion target)
		{
			target *= this._characterController.Pose.rotation;
			this._characterController.SetRotation(target);
		}

		private void RotateVelocity(Quaternion target)
		{
			float num;
			Vector3 axis;
			target.ToAngleAxis(out num, out axis);
			num *= this._deltaTimeProvider();
			target = Quaternion.AngleAxis(num, axis) * this._characterController.Pose.rotation;
			this._characterController.SetRotation(target);
		}

		private void CatchUpCharacterToPlayer()
		{
			Vector3 delta = this.GetPlayerHead() - this._characterController.Pose.position;
			Vector3 forward = Vector3.ProjectOnPlane(this._playerEyes.forward, Vector3.up);
			this._characterController.Move(delta);
			this._characterController.SetRotation(Quaternion.LookRotation(forward, Vector3.up));
		}

		private void CatchUpPlayerToCharacter(Pose delta, float feetHeight)
		{
			Pose pose = this._characterController.Pose;
			Vector3 playerHead = this.GetPlayerHead();
			this._playerOrigin.rotation = delta.rotation * this._playerOrigin.rotation;
			this._playerOrigin.position = this._playerOrigin.position + playerHead - this.GetPlayerHead();
			Vector3 position = this._playerOrigin.position + delta.position;
			this._playerOrigin.position = position;
			this._characterController.SetPosition(pose.position);
			this._characterController.SetRotation(pose.rotation);
		}

		public void ResetPlayerToCharacter()
		{
			Pose pose = this._characterController.Pose;
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = this._playerOrigin.position - this.GetPlayerHead();
			this._playerOrigin.position = characterFeet + b;
			this._accumulatedDeltaFrame = Pose.identity;
			this._characterController.SetPosition(pose.position);
			this._characterController.SetRotation(pose.rotation);
		}

		private void UpdateVelocity()
		{
			float num = this._deltaTimeProvider();
			float num2 = 1f / (1f + this._airDamping * num);
			this._velocity.x = this._velocity.x * num2;
			this._velocity.y = this._velocity.y * num2;
			this._velocity.z = this._velocity.z * num2;
		}

		private float GetModifiedSpeedFactor()
		{
			return this._acceleration * this._deltaTimeProvider();
		}

		private Vector3 GetCharacterFeet()
		{
			return this._characterController.Pose.position + Vector3.down * (this._characterController.Height * 0.5f + this._characterController.SkinWidth);
		}

		private Vector3 GetCharacterHead()
		{
			return this._characterController.Pose.position + Vector3.up * (this._characterController.Height * 0.5f - 0.1085f + this._characterController.SkinWidth);
		}

		private Vector3 GetPlayerHead()
		{
			return this._playerEyes.position - this._playerEyes.forward * 0.0965f;
		}

		private IEnumerator EndOfFrameCoroutine()
		{
			for (;;)
			{
				yield return this._endOfFrame;
				this.LastUpdate();
			}
			yield break;
		}

		public void InjectAllFlyingLocomotor(CharacterController characterController, Transform playerEyes, Transform playerOrigin)
		{
			this.InjectCharacterController(characterController);
			this.InjectPlayerEyes(playerEyes);
			this.InjectPlayerOrigin(playerOrigin);
		}

		public void InjectCharacterController(CharacterController characterController)
		{
			this._characterController = characterController;
		}

		public void InjectPlayerEyes(Transform playerEyes)
		{
			this._playerEyes = playerEyes;
		}

		public void InjectPlayerOrigin(Transform playerOrigin)
		{
			this._playerOrigin = playerOrigin;
		}

		[Header("Character")]
		[SerializeField]
		private CharacterController _characterController;

		[Header("VR Player")]
		[SerializeField]
		[Tooltip("Root of the actual VR player so it can be sync with with capsule. If you provided a _playerEyes you must also provide a _playerOrigin.")]
		private Transform _playerOrigin;

		[SerializeField]
		[Tooltip("Eyes of the actual VR player so it can be sync with the capsule. If you provided a _playerOrigin you must also provide a _playerEyes.")]
		private Transform _playerEyes;

		[Header("Parameters")]
		[SerializeField]
		[Tooltip("The rate of acceleration during movement.")]
		private float _acceleration = 150f;

		[SerializeField]
		[Tooltip("The rate of damping on the horizontal movement while in the air.")]
		private float _airDamping = 30f;

		private Func<float> _deltaTimeProvider = () => Time.deltaTime;

		protected Action<LocomotionEvent, Pose> _whenLocomotionEventHandled = delegate(LocomotionEvent <p0>, Pose <p1>)
		{
		};

		private Pose _accumulatedDeltaFrame;

		private Vector3 _velocity;

		private const float _sellionToTopOfHead = 0.1085f;

		private const float _sellionToBackOfHeadHalf = 0.0965f;

		private Queue<LocomotionEvent> _deferredLocomotionEvent = new Queue<LocomotionEvent>();

		private YieldInstruction _endOfFrame = new WaitForEndOfFrame();

		private Coroutine _endOfFrameRoutine;

		protected bool _started;
	}
}
