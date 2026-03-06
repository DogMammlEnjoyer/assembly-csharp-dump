using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
	[AddComponentMenu("XR/Locomotion/Continuous Move Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.ContinuousMoveProvider.html")]
	public class ContinuousMoveProvider : LocomotionProvider, IGravityController
	{
		public float moveSpeed
		{
			get
			{
				return this.m_MoveSpeed;
			}
			set
			{
				this.m_MoveSpeed = value;
			}
		}

		public float inAirControlModifier
		{
			get
			{
				return this.m_InAirControlModifier;
			}
			set
			{
				this.m_InAirControlModifier = value;
			}
		}

		public bool enableStrafe
		{
			get
			{
				return this.m_EnableStrafe;
			}
			set
			{
				this.m_EnableStrafe = value;
			}
		}

		public bool enableFly
		{
			get
			{
				return this.m_EnableFly;
			}
			set
			{
				this.m_EnableFly = value;
			}
		}

		public Transform forwardSource
		{
			get
			{
				return this.m_ForwardSource;
			}
			set
			{
				this.m_ForwardSource = value;
			}
		}

		public XROriginMovement transformation { get; set; } = new XROriginMovement();

		public XRInputValueReader<Vector2> leftHandMoveInput
		{
			get
			{
				return this.m_LeftHandMoveInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_LeftHandMoveInput, value, this);
			}
		}

		public XRInputValueReader<Vector2> rightHandMoveInput
		{
			get
			{
				return this.m_RightHandMoveInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty<Vector2>(ref this.m_RightHandMoveInput, value, this);
			}
		}

		public bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled;
			}
		}

		public bool gravityPaused
		{
			get
			{
				return this.m_EnableFly;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (ComponentLocatorUtility<GravityProvider>.TryFindComponent(out this.m_GravityProvider) && !this.m_UseGravity)
			{
				this.MigrateUseGravityToGravityProvider();
			}
		}

		protected void OnEnable()
		{
			this.m_LeftHandMoveInput.EnableDirectActionIfModeUsed();
			this.m_RightHandMoveInput.EnableDirectActionIfModeUsed();
			this.m_GravityDrivenVelocity = Vector3.zero;
			this.m_InAirVelocity = Vector3.zero;
		}

		protected void OnDisable()
		{
			this.m_LeftHandMoveInput.DisableDirectActionIfModeUsed();
			this.m_RightHandMoveInput.DisableDirectActionIfModeUsed();
		}

		protected override void OnLocomotionStateChanging(LocomotionState state)
		{
			base.OnLocomotionStateChanging(state);
			if (state == LocomotionState.Moving)
			{
				this.TryLockGravity(this.m_EnableFly ? GravityOverride.ForcedOff : GravityOverride.ForcedOn);
				return;
			}
			if (state == LocomotionState.Ended)
			{
				this.RemoveGravityLock();
			}
		}

		protected override void OnLocomotionStarting()
		{
			base.OnLocomotionStarting();
		}

		protected override void OnLocomotionEnding()
		{
			base.OnLocomotionEnding();
		}

		protected void Update()
		{
			this.m_IsMovingXROrigin = false;
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (((xrOrigin != null) ? xrOrigin.Origin : null) == null)
			{
				return;
			}
			Vector2 vector = this.ReadInput();
			Vector3 translationInWorldSpace = this.ComputeDesiredMove(vector);
			if (vector != Vector2.zero || this.m_GravityDrivenVelocity != Vector3.zero || this.m_InAirVelocity != Vector3.zero)
			{
				this.MoveRig(translationInWorldSpace);
			}
			if (!this.m_IsMovingXROrigin)
			{
				base.TryEndLocomotion();
			}
		}

		private Vector2 ReadInput()
		{
			Vector2 a = this.m_LeftHandMoveInput.ReadValue();
			Vector2 b = this.m_RightHandMoveInput.ReadValue();
			return a + b;
		}

		protected virtual Vector3 ComputeDesiredMove(Vector2 input)
		{
			if (input == Vector2.zero && this.m_InAirVelocity == Vector3.zero)
			{
				return Vector3.zero;
			}
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (xrOrigin == null)
			{
				return Vector3.zero;
			}
			Vector3 vector = Vector3.ClampMagnitude(new Vector3(this.m_EnableStrafe ? input.x : 0f, 0f, input.y), 1f);
			float deltaTime = Time.deltaTime;
			if (this.m_GravityProvider == null || !this.m_GravityProvider.enabled || !this.m_GravityProvider.useGravity || this.m_GravityProvider.isGrounded)
			{
				this.m_InAirVelocity = vector;
			}
			else
			{
				this.m_InAirVelocity += deltaTime * this.m_InAirControlModifier * 10f * (vector - this.m_InAirVelocity);
			}
			Transform transform = (this.m_ForwardSource == null) ? xrOrigin.Camera.transform : this.m_ForwardSource;
			Vector3 vector2 = transform.forward;
			Transform transform2 = xrOrigin.Origin.transform;
			float d = this.m_MoveSpeed * deltaTime * transform2.localScale.x;
			if (this.m_EnableFly)
			{
				Vector3 right = transform.right;
				return (vector.x * right + vector.z * vector2) * d;
			}
			Vector3 up = transform2.up;
			if (Mathf.Approximately(Mathf.Abs(Vector3.Dot(vector2, up)), 1f))
			{
				vector2 = -transform.up;
			}
			Vector3 toDirection = Vector3.ProjectOnPlane(vector2, up);
			Vector3 direction = Quaternion.FromToRotation(transform2.forward, toDirection) * this.m_InAirVelocity * d;
			return transform2.TransformDirection(direction);
		}

		protected virtual void MoveRig(Vector3 translationInWorldSpace)
		{
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (((xrOrigin != null) ? xrOrigin.Origin : null) == null)
			{
				return;
			}
			this.FindCharacterController();
			Vector3 vector = translationInWorldSpace;
			if (this.m_GravityProvider == null && this.m_CharacterController != null && this.m_CharacterController.enabled)
			{
				if (this.m_CharacterController.isGrounded || !this.m_UseGravity || this.m_EnableFly)
				{
					this.m_GravityDrivenVelocity = Vector3.zero;
				}
				else
				{
					this.m_GravityDrivenVelocity += Physics.gravity * Time.deltaTime;
				}
				vector += this.m_GravityDrivenVelocity * Time.deltaTime;
			}
			base.TryStartLocomotionImmediately();
			if (base.locomotionState != LocomotionState.Moving)
			{
				return;
			}
			this.m_IsMovingXROrigin = true;
			this.transformation.motion = vector;
			base.TryQueueTransformation(this.transformation);
		}

		private void FindCharacterController()
		{
			XROrigin xrOrigin = base.mediator.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			if (gameObject == null)
			{
				return;
			}
			if (this.m_CharacterController == null && !this.m_AttemptedGetCharacterController)
			{
				if (!gameObject.TryGetComponent<CharacterController>(out this.m_CharacterController) && gameObject != base.mediator.xrOrigin.gameObject)
				{
					base.mediator.xrOrigin.TryGetComponent<CharacterController>(out this.m_CharacterController);
				}
				this.m_AttemptedGetCharacterController = true;
			}
		}

		public bool TryLockGravity(GravityOverride gravityOverride)
		{
			return this.m_GravityProvider != null && this.m_GravityProvider.TryLockGravity(this, gravityOverride);
		}

		public void RemoveGravityLock()
		{
			if (this.m_GravityProvider != null)
			{
				this.m_GravityProvider.UnlockGravity(this);
			}
		}

		void IGravityController.OnGroundedChanged(bool isGrounded)
		{
			this.OnGroundedChanged(isGrounded);
		}

		void IGravityController.OnGravityLockChanged(GravityOverride gravityOverride)
		{
			this.OnGravityLockChanged(gravityOverride);
		}

		protected virtual void OnGroundedChanged(bool isGrounded)
		{
		}

		protected virtual void OnGravityLockChanged(GravityOverride gravityOverride)
		{
		}

		[Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
		public bool useGravity
		{
			get
			{
				return this.m_UseGravity;
			}
			set
			{
				this.m_UseGravity = value;
				if (Application.isPlaying && this.m_GravityProvider != null)
				{
					this.MigrateUseGravityToGravityProvider();
				}
			}
		}

		[Obsolete("Private migration helper.")]
		private void MigrateUseGravityToGravityProvider()
		{
			if (this.m_GravityProvider.useGravity != this.m_UseGravity)
			{
				Debug.LogWarning("Use Gravity is deprecated on this locomotion component while Gravity Provider component is in scene." + string.Format(" Automatically setting Use Gravity to {0} on Gravity Provider.", this.m_UseGravity) + " Gravity should be controlled on the Gravity Provider instead.", this);
				this.m_GravityProvider.useGravity = this.m_UseGravity;
			}
		}

		[SerializeField]
		[Tooltip("The speed, in units per second, to move forward.")]
		private float m_MoveSpeed = 1f;

		[SerializeField]
		[Tooltip("Determines how much control the player has while in the air (0 = no control, 1 = full control).")]
		private float m_InAirControlModifier = 0.5f;

		[SerializeField]
		[Tooltip("Controls whether to enable strafing (sideways movement).")]
		private bool m_EnableStrafe = true;

		[SerializeField]
		[Tooltip("Controls whether to enable flying (unconstrained movement). This overrides the use of gravity.")]
		private bool m_EnableFly;

		[SerializeField]
		[Tooltip("The source Transform to define the forward direction.")]
		private Transform m_ForwardSource;

		[SerializeField]
		[Tooltip("Reads input data from the left hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_LeftHandMoveInput = new XRInputValueReader<Vector2>("Left Hand Move", XRInputValueReader.InputSourceMode.InputActionReference);

		[SerializeField]
		[Tooltip("Reads input data from the right hand controller. Input Action must be a Value action type (Vector 2).")]
		private XRInputValueReader<Vector2> m_RightHandMoveInput = new XRInputValueReader<Vector2>("Right Hand Move", XRInputValueReader.InputSourceMode.InputActionReference);

		private GravityProvider m_GravityProvider;

		private CharacterController m_CharacterController;

		private bool m_AttemptedGetCharacterController;

		private bool m_IsMovingXROrigin;

		private Vector3 m_GravityDrivenVelocity;

		private Vector3 m_InAirVelocity;

		[SerializeField]
		[Tooltip("Controls whether gravity affects this provider when a Character Controller is used and flying is disabled. Ignored when a Gravity Provider component is found in the scene. Deprecated in XRI 3.1.0, use Gravity Provider instead.")]
		[Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
		private bool m_UseGravity = true;
	}
}
