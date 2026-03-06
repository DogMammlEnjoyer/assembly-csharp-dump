using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class FirstPersonLocomotor : MonoBehaviour, ILocomotionEventHandler, IDeltaTimeConsumer, ITimeConsumer
	{
		public float MaxWallPenetrationDistance
		{
			get
			{
				return this._maxWallPenetrationDistance;
			}
			set
			{
				this._maxWallPenetrationDistance = value;
			}
		}

		public float ExitHotspotDistance
		{
			get
			{
				return this._exitHotspotDistance;
			}
			set
			{
				this._exitHotspotDistance = value;
			}
		}

		public bool AutoUpdateHeight
		{
			get
			{
				return this._autoUpdateHeight;
			}
			set
			{
				this._autoUpdateHeight = value;
			}
		}

		public float DefaultHeight
		{
			get
			{
				return this._defaultHeight;
			}
			set
			{
				this._defaultHeight = value;
			}
		}

		public float HeightOffset
		{
			get
			{
				return this._heightOffset;
			}
			set
			{
				this._heightOffset = value;
			}
		}

		public float CrouchHeightOffset
		{
			get
			{
				return this._crouchHeightOffset;
			}
			set
			{
				this._crouchHeightOffset = value;
			}
		}

		public float SpeedFactor
		{
			get
			{
				return this._speedFactor;
			}
			set
			{
				this._speedFactor = value;
			}
		}

		public float CrouchSpeedFactor
		{
			get
			{
				return this._crouchSpeedFactor;
			}
			set
			{
				this._crouchSpeedFactor = value;
			}
		}

		public float RunningSpeedFactor
		{
			get
			{
				return this._runningSpeedFactor;
			}
			set
			{
				this._runningSpeedFactor = value;
			}
		}

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

		public float GroundDamping
		{
			get
			{
				return this._groundDamping;
			}
			set
			{
				this._groundDamping = value;
			}
		}

		public float JumpDamping
		{
			get
			{
				return this._jumpDamping;
			}
			set
			{
				this._jumpDamping = value;
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

		public float JumpForce
		{
			get
			{
				return this._jumpForce;
			}
			set
			{
				this._jumpForce = value;
			}
		}

		public float GravityFactor
		{
			get
			{
				return this._gravityFactor;
			}
			set
			{
				this._gravityFactor = value;
			}
		}

		public float CoyoteTime
		{
			get
			{
				return this._coyoteTime;
			}
			set
			{
				this._coyoteTime = value;
			}
		}

		public bool FlattenInputVelocity
		{
			get
			{
				return this._flattenInputVelocity;
			}
			set
			{
				this._flattenInputVelocity = value;
			}
		}

		public AnimationCurve InputVelocityStabilization
		{
			get
			{
				return this._inputVelocityStabilization;
			}
			set
			{
				this._inputVelocityStabilization = value;
			}
		}

		public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
		{
			this._deltaTimeProvider = deltaTimeProvider;
		}

		public void SetTimeProvider(Func<float> timeProvider)
		{
			this._timeProvider = timeProvider;
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

		public bool IsRunning
		{
			get
			{
				return this._isRunning;
			}
		}

		public bool IsCrouching
		{
			get
			{
				return this._isCrouching;
			}
		}

		public bool IgnoringVelocity
		{
			get
			{
				return this._velocityDisabled || this._isHeadInHotspot;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return this._velocity;
			}
			set
			{
				this._velocity = value;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (!this._velocityDisabled && this._maxStartGroundDistance >= 0f && !this._characterController.TryGround(this._maxStartGroundDistance))
			{
				this.DisableMovement();
			}
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
			this.TryExitHotspot(false);
			this.UpdateCharacterHeight();
			this.CatchUpCharacterToPlayer();
			if (!this.IgnoringVelocity)
			{
				this.UpdateVelocity();
				Pose pose = this._characterController.Pose;
				Vector3 delta = this._velocity * this._deltaTimeProvider();
				this._characterController.Move(delta);
				Pose pose2 = this._characterController.Pose;
				this.AccumulateDelta(ref this._accumulatedDeltaFrame, pose, pose2);
				if (this._endedFrameGrounded && !this.IsGrounded)
				{
					this._leftGroundTime = this._timeProvider();
				}
			}
		}

		protected virtual void LateUpdate()
		{
			this.ConsumeDeferredLocomotionEvents();
		}

		protected virtual void LastUpdate()
		{
			this.CatchUpPlayerToCharacter(this._accumulatedDeltaFrame, this.GetCharacterFeet().y);
			this._accumulatedDeltaFrame = Pose.identity;
			if (!this._jumpThisFrame)
			{
				this._endedFrameGrounded = this.IsGrounded;
			}
			this._jumpThisFrame = false;
		}

		public void Jump()
		{
			bool flag = this._coyoteTime > 0f && this._timeProvider() - this._leftGroundTime <= this._coyoteTime;
			if (!this.IsGrounded && !flag)
			{
				return;
			}
			if (this._isCrouching)
			{
				this.Crouch(false);
				return;
			}
			this.TryExitHotspot(true);
			if (flag && this._velocity.y < 0f)
			{
				this._velocity.y = 0f;
			}
			this._velocity += Vector3.up * this._jumpForce;
			this._leftGroundTime = 0f;
			this._endedFrameGrounded = false;
			this._jumpThisFrame = true;
		}

		public void ToggleCrouch()
		{
			this.Crouch(!this._isCrouching);
		}

		public void Crouch(bool crouch)
		{
			if (this._isCrouching != crouch)
			{
				this._isCrouching = crouch;
			}
		}

		public void ToggleRun()
		{
			this.Run(!this._isRunning);
		}

		public void Run(bool run)
		{
			if (this._isRunning != run)
			{
				this._isRunning = run;
				this.TryExitHotspot(true);
			}
		}

		public void DisableMovement()
		{
			this._velocityDisabled = true;
		}

		public void EnableMovement()
		{
			if (!this.IgnoringVelocity)
			{
				return;
			}
			this._velocityDisabled = false;
			this._isHeadInHotspot = false;
			this._headHotspotCenter = null;
			this._velocity = Vector3.zero;
			this._characterController.TryGround(0f);
		}

		public void ResetPlayerToCharacter()
		{
			Pose pose = this._characterController.Pose;
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = this._playerOrigin.position - this.GetPlayerHead();
			b.y = 0f;
			this._playerOrigin.position = characterFeet + b;
			this._accumulatedDeltaFrame = Pose.identity;
			this._characterController.SetPosition(pose.position);
			this._characterController.SetRotation(pose.rotation);
		}

		public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Velocity)
			{
				Vector3 vector = locomotionEvent.Pose.position;
				if (this._flattenInputVelocity)
				{
					Quaternion rotation = Quaternion.LookRotation(vector.normalized, locomotionEvent.Pose.up);
					vector = Vector3.ProjectOnPlane(this.FlattenForwardOffset(rotation) * vector, Vector3.up).normalized * vector.magnitude;
				}
				this.AddVelocity(vector);
				if (this.IsHeadFarFromPoint(this.GetCharacterHead(), this._maxWallPenetrationDistance))
				{
					this.ResetPlayerToCharacter();
				}
				this._whenLocomotionEventHandled(locomotionEvent, locomotionEvent.Pose);
				return;
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.None && locomotionEvent.Rotation == LocomotionEvent.RotationType.None)
			{
				LocomotionActionsBroadcaster.LocomotionAction action;
				if (LocomotionActionsBroadcaster.TryGetLocomotionActions(locomotionEvent, out action, this._context))
				{
					this.TryPerformLocomotionActions(action);
					return;
				}
			}
			else
			{
				if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
				{
					this._velocity = Vector3.zero;
				}
				this._deferredLocomotionEvent.Enqueue(locomotionEvent);
			}
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

		private bool TryPerformLocomotionActions(LocomotionActionsBroadcaster.LocomotionAction action)
		{
			switch (action)
			{
			case LocomotionActionsBroadcaster.LocomotionAction.Crouch:
				this.Crouch(true);
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.StandUp:
				this.Crouch(false);
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.ToggleCrouch:
				this.ToggleCrouch();
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.Run:
				this.Run(true);
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.Walk:
				this.Run(false);
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.ToggleRun:
				this.ToggleRun();
				return true;
			case LocomotionActionsBroadcaster.LocomotionAction.Jump:
				this.Jump();
				return true;
			default:
				return false;
			}
		}

		private void AccumulateDelta(ref Pose accumulator, in Pose from, in Pose to)
		{
			accumulator.position = accumulator.position + to.position - from.position;
			accumulator.rotation = Quaternion.Inverse(from.rotation) * to.rotation * accumulator.rotation;
		}

		private void AddVelocity(Vector3 velocity)
		{
			this.TryExitHotspot(true);
			this._velocity += velocity * this.GetModifiedSpeedFactor();
		}

		private void MoveAbsoluteFeet(Vector3 target)
		{
			this.TryExitHotspot(true);
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = target - characterFeet;
			Vector3 position = this._characterController.Pose.position + b;
			this._characterController.SetPosition(position);
			this._characterController.TryGround(this._characterController.MaxStep);
		}

		private void MoveAbsoluteHead(Vector3 target)
		{
			Vector3 characterHead = this.GetCharacterHead();
			Vector3 b = target - characterHead;
			Vector3 position = this._characterController.Pose.position + b;
			this._characterController.SetPosition(position);
			this._isHeadInHotspot = true;
			this._headHotspotCenter = new Vector3?(this.GetCharacterHead());
		}

		private void MoveRelative(Vector3 offset)
		{
			if (this._characterController.IsGrounded)
			{
				this.TryExitHotspot(true);
				this._velocity = Vector3.zero;
				this._characterController.Move(offset);
			}
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

		private bool TryExitHotspot(bool force = false)
		{
			if (this._isHeadInHotspot && this._headHotspotCenter != null && (force || this.IsHeadFarFromPoint(this._headHotspotCenter.Value, this._exitHotspotDistance)))
			{
				this._isHeadInHotspot = false;
				this._headHotspotCenter = null;
				this._velocity = Vector3.zero;
				this._characterController.TryGround(0f);
				return true;
			}
			return false;
		}

		private void UpdateCharacterHeight()
		{
			float desiredHeight = Mathf.Max(this._heightOffset + (this._isCrouching ? this._crouchHeightOffset : 0f) + (this._autoUpdateHeight ? (this.GetPlayerHeadTop().y - this._playerOrigin.position.y) : this._defaultHeight), this._characterController.Radius * 2f);
			this._characterController.TrySetHeight(desiredHeight);
		}

		private void CatchUpCharacterToPlayer()
		{
			Vector3 delta = Vector3.ProjectOnPlane(this.GetPlayerHead() - this._characterController.Pose.position, Vector3.up);
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
			Vector3 b = Vector3.ProjectOnPlane(delta.position, Vector3.up);
			Vector3 position = this._playerOrigin.position + b;
			position.y = feetHeight + (this._isCrouching ? this._crouchHeightOffset : 0f) + this._heightOffset;
			this._playerOrigin.position = position;
			this._characterController.SetPosition(pose.position);
			this._characterController.SetRotation(pose.rotation);
		}

		private void UpdateVelocity()
		{
			float num = this._deltaTimeProvider();
			if (this.IsGrounded && this._velocity.y <= 0f)
			{
				this._velocity *= 1f / (1f + this._groundDamping * num);
				this._velocity.y = 0f;
				return;
			}
			float num2 = 1f / (1f + this._airDamping * num);
			this._velocity.x = this._velocity.x * num2;
			this._velocity.z = this._velocity.z * num2;
			if (this._velocity.y > 0f)
			{
				this._velocity.y = this._velocity.y * (1f / (1f + this._jumpDamping * num));
			}
			this._velocity += Physics.gravity * this._gravityFactor * num;
		}

		private float GetModifiedSpeedFactor()
		{
			if (!this.IsGrounded || this._velocity.y > 0f)
			{
				return 0f;
			}
			return this._acceleration * (this._isCrouching ? this._crouchSpeedFactor : (this._isRunning ? this._runningSpeedFactor : this._speedFactor)) * this._deltaTimeProvider();
		}

		private Quaternion FlattenForwardOffset(Quaternion rotation)
		{
			Vector3 vector = rotation * Vector3.forward;
			Vector3 a = rotation * Vector3.up;
			Vector3 up = Vector3.up;
			float time = Vector3.Dot(vector, up);
			float f = this._inputVelocityStabilization.Evaluate(time);
			vector = Vector3.Slerp(vector, a * -Mathf.Sign(f), Mathf.Abs(f));
			vector = Vector3.ProjectOnPlane(vector, up).normalized;
			return Quaternion.FromToRotation(rotation * Vector3.forward, vector);
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

		private Vector3 GetPlayerHeadTop()
		{
			return this.GetPlayerHead() + Vector3.up * 0.1085f;
		}

		private bool IsHeadFarFromPoint(Vector3 point, float maxDistance)
		{
			return Vector3.ProjectOnPlane(this.GetPlayerHead() - point, Vector3.up).sqrMagnitude >= maxDistance * maxDistance;
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

		public void InjectAllFirstPersonLocomotor(CharacterController characterController, Transform playerEyes, Transform playerOrigin)
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

		public void InjectOptionalMaxStartGroundDistance(float maxStartGroundDistance)
		{
			this._maxStartGroundDistance = maxStartGroundDistance;
		}

		public void InjectOptionalContext(Context context)
		{
			this._context = context;
		}

		[Header("Character")]
		[SerializeField]
		[Tooltip("The CharacterController reprensenting the character that is used to move the player around the scene.")]
		private CharacterController _characterController;

		[Header("VR Player")]
		[SerializeField]
		[Tooltip("Root of the actual VR player so it can be sync with with the CharacterController. If you provided a _playerEyes you must also provide a _playerOrigin.")]
		private Transform _playerOrigin;

		[SerializeField]
		[Tooltip("Eyes of the actual VR player so it can be sync with the capsule. If you provided a _playerOrigin you must also provide a _playerEyes.")]
		private Transform _playerEyes;

		[SerializeField]
		[Tooltip("After the player penetrates the head inside a collider (for example a wall), the maximum distance before the player gets reset to the capsule position when trying to move synthetically.")]
		private float _maxWallPenetrationDistance = 0.3f;

		[SerializeField]
		[Tooltip("After using LocomotionEvent.TranslationType.AbsoluteEyeLevel that disables the ground checks. What is the maximum deviation of the player before the physics are re-enabled.")]
		private float _exitHotspotDistance = 0.3f;

		[SerializeField]
		[Tooltip("When _playerOrigin and _playerEyes are present. This will force the capsule height to update using the actual player height, instead of using _defaultHeight")]
		private bool _autoUpdateHeight = true;

		[Header("Parameters")]
		[SerializeField]
		[Tooltip("Height of the character capsule when standing normally. This might be overriden by _autoUpdateHeight")]
		private float _defaultHeight = 1.4f;

		[SerializeField]
		[Tooltip("General height offset applied to the capsule.")]
		private float _heightOffset;

		[SerializeField]
		[Tooltip("Height offset added while crouching.")]
		private float _crouchHeightOffset = -0.5f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while moving normally.")]
		private float _speedFactor = 30f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while crouching.")]
		private float _crouchSpeedFactor = 10f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while running.")]
		private float _runningSpeedFactor = 50f;

		[SerializeField]
		[Tooltip("The rate of acceleration during movement.")]
		private float _acceleration = 5f;

		[SerializeField]
		[Tooltip("The rate of damping on movement while grounded.")]
		private float _groundDamping = 40f;

		[SerializeField]
		[Tooltip("The rate of damping on the vertical movement while jumping.")]
		private float _jumpDamping;

		[SerializeField]
		[Tooltip("The rate of damping on the horizontal movement while in the air.")]
		private float _airDamping = 1f;

		[SerializeField]
		[Tooltip("The force applied to the character when jumping.")]
		private float _jumpForce = 2.5f;

		[SerializeField]
		[Tooltip("Modifies the strength of gravity.")]
		private float _gravityFactor = 1f;

		[SerializeField]
		[Tooltip("Extra time after starting to fall to allow jumping.")]
		private float _coyoteTime;

		[SerializeField]
		[Tooltip("Correct the input velocity so it always points in the XZ plane.Use with the _inputVelocityStabilization curve to adjust the range")]
		private bool _flattenInputVelocity = true;

		[SerializeField]
		[Tooltip("When the input velocity points too far up or down the forward direction will be slerped between the .forward and the .up using this curve for the final forward to be stable. x: from -1 to 1, represents the dot product of forward.worldUp. y: 0 represents the real forward, 1 the up direction and -1 the down direction.")]
		[ConditionalHide("_flattenInputVelocity", true, ConditionalHideAttribute.DisplayMode.ShowIfTrue)]
		private AnimationCurve _inputVelocityStabilization = AnimationCurve.EaseInOut(-1f, 0f, 1f, 0f);

		[SerializeField]
		[Tooltip("When Velocity is ignored the character will not try to catch up to the player and the character won't slide or fall.It is preferred to re-enable the movement by calling EnableMovement instead of setting this variable to false directly.")]
		private bool _velocityDisabled;

		[SerializeField]
		[Optional]
		[Min(-1f)]
		[Tooltip("If no ground is detected below this distance in meters on Start, it will disable the velocity to prevent falling. Negative numbers disable this behavior.")]
		private float _maxStartGroundDistance = 10f;

		[SerializeField]
		[Optional]
		private Context _context;

		private Func<float> _deltaTimeProvider = () => Time.deltaTime;

		private Func<float> _timeProvider = () => Time.time;

		protected Action<LocomotionEvent, Pose> _whenLocomotionEventHandled = delegate(LocomotionEvent <p0>, Pose <p1>)
		{
		};

		private Pose _accumulatedDeltaFrame;

		private Vector3 _velocity;

		private bool _isHeadInHotspot;

		private Vector3? _headHotspotCenter;

		private float _leftGroundTime;

		private bool _isRunning;

		private bool _isCrouching;

		private const float _sellionToTopOfHead = 0.1085f;

		private const float _sellionToBackOfHeadHalf = 0.0965f;

		private Queue<LocomotionEvent> _deferredLocomotionEvent = new Queue<LocomotionEvent>();

		private YieldInstruction _endOfFrame = new WaitForEndOfFrame();

		private Coroutine _endOfFrameRoutine;

		private bool _jumpThisFrame;

		private bool _endedFrameGrounded = true;

		protected bool _started;
	}
}
