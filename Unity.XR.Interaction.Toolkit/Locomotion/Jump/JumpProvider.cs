using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump
{
	[AddComponentMenu("XR/Locomotion/Jump Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump.JumpProvider.html")]
	public class JumpProvider : LocomotionProvider, IGravityController
	{
		public bool disableGravityDuringJump
		{
			get
			{
				return this.m_DisableGravityDuringJump;
			}
			set
			{
				this.m_DisableGravityDuringJump = value;
			}
		}

		public bool unlimitedInAirJumps
		{
			get
			{
				return this.m_UnlimitedInAirJumps;
			}
			set
			{
				this.m_UnlimitedInAirJumps = value;
			}
		}

		public int inAirJumpCount
		{
			get
			{
				return this.m_InAirJumpCount;
			}
			set
			{
				this.m_InAirJumpCount = Mathf.Max(0, value);
				this.m_CurrentInAirJumpCount = this.m_InAirJumpCount;
			}
		}

		public float jumpForgivenessWindow
		{
			get
			{
				return this.m_JumpForgivenessWindow;
			}
			set
			{
				this.m_JumpForgivenessWindow = value;
				this.m_CurrentJumpForgivenessWindowTime = this.m_JumpForgivenessWindow;
			}
		}

		public float jumpHeight
		{
			get
			{
				return this.m_JumpHeight;
			}
			set
			{
				this.m_JumpHeight = value;
			}
		}

		public bool variableHeightJump
		{
			get
			{
				return this.m_VariableHeightJump;
			}
			set
			{
				this.m_VariableHeightJump = value;
			}
		}

		public float minJumpHoldTime
		{
			get
			{
				return this.m_MinJumpHoldTime;
			}
			set
			{
				this.m_MinJumpHoldTime = value;
			}
		}

		public float maxJumpHoldTime
		{
			get
			{
				return this.m_MaxJumpHoldTime;
			}
			set
			{
				this.m_MaxJumpHoldTime = value;
			}
		}

		public float earlyOutDecelerationSpeed
		{
			get
			{
				return this.m_EarlyOutDecelerationSpeed;
			}
			set
			{
				this.m_EarlyOutDecelerationSpeed = value;
			}
		}

		public XRInputButtonReader jumpInput
		{
			get
			{
				return this.m_JumpInput;
			}
			set
			{
				XRInputReaderUtility.SetInputProperty(ref this.m_JumpInput, value, this);
			}
		}

		public XROriginMovement transformation { get; set; } = new XROriginMovement();

		public bool isJumping
		{
			get
			{
				return this.m_IsJumping;
			}
		}

		public bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled;
			}
		}

		public bool gravityPaused { get; protected set; }

		protected virtual void OnValidate()
		{
			this.m_InAirJumpCount = Mathf.Max(0, this.m_InAirJumpCount);
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_HasGravityProvider = ComponentLocatorUtility<GravityProvider>.TryFindComponent(out this.m_GravityProvider);
			if (!this.m_HasGravityProvider)
			{
				Debug.LogError("Could not find Gravity Provider component which is required by the Jump Provider component. Disabling component.", this);
				base.enabled = false;
				return;
			}
		}

		protected virtual void OnEnable()
		{
			this.m_JumpInput.EnableDirectActionIfModeUsed();
			this.m_CurrentInAirJumpCount = this.m_InAirJumpCount;
		}

		protected virtual void OnDisable()
		{
			this.m_JumpInput.DisableDirectActionIfModeUsed();
		}

		protected virtual void Update()
		{
			this.CheckJump();
		}

		private void CheckJump()
		{
			if (!this.m_HasGravityProvider)
			{
				return;
			}
			if (this.m_CurrentJumpForgivenessWindowTime > 0f)
			{
				this.m_CurrentJumpForgivenessWindowTime -= Time.deltaTime;
			}
			if (this.m_HasJumped && this.m_JumpInput.ReadWasCompletedThisFrame())
			{
				this.m_HasJumped = false;
			}
			if (!this.m_HasJumped && this.m_JumpInput.ReadIsPerformed())
			{
				this.Jump();
			}
			if (this.m_IsJumping)
			{
				this.UpdateJump();
			}
		}

		public void Jump()
		{
			if (!this.CanJump())
			{
				return;
			}
			if (!this.m_GravityProvider.isGrounded)
			{
				this.m_CurrentInAirJumpCount--;
			}
			this.m_HasJumped = true;
			this.m_IsJumping = true;
			this.m_CurrentJumpTimer = 0f;
			this.m_StoppingJumpTime = this.m_MaxJumpHoldTime;
			this.m_CurrentJumpForgivenessWindowTime = 0f;
			this.m_CurrentJumpForceThisFrame = this.m_JumpHeight;
			if (this.m_DisableGravityDuringJump)
			{
				this.TryLockGravity(GravityOverride.ForcedOff);
			}
			this.m_GravityProvider.ResetFallForce();
		}

		public bool CanJump()
		{
			return this.m_UnlimitedInAirJumps || this.m_CurrentInAirJumpCount > 0 || this.m_GravityProvider.isGrounded || this.m_CurrentJumpForgivenessWindowTime > 0f;
		}

		private void UpdateJump()
		{
			float deltaTime = Time.deltaTime;
			this.ProcessJumpForce(deltaTime);
			if (this.m_GravityProvider.useLocalSpaceGravity)
			{
				this.m_JumpVector = this.m_CurrentJumpForceThisFrame * deltaTime * this.m_GravityProvider.GetCurrentUp();
			}
			else
			{
				this.m_JumpVector.y = this.m_CurrentJumpForceThisFrame * deltaTime;
			}
			base.TryStartLocomotionImmediately();
			if (base.locomotionState != LocomotionState.Moving)
			{
				return;
			}
			this.transformation.motion = this.m_JumpVector;
			base.TryQueueTransformation(this.transformation);
		}

		private void ProcessJumpForce(float dt)
		{
			this.m_CurrentJumpTimer += dt;
			if (this.m_StoppingJumpTime == this.m_MaxJumpHoldTime && (this.m_MaxJumpHoldTime <= 0f || (this.m_VariableHeightJump && this.m_CurrentJumpTimer > this.m_MinJumpHoldTime && !this.m_JumpInput.ReadIsPerformed())))
			{
				this.m_StoppingJumpTime = Mathf.Min(this.m_CurrentJumpTimer + this.m_EarlyOutDecelerationSpeed, this.m_MaxJumpHoldTime);
			}
			this.m_CurrentJumpForceThisFrame = this.CalculateJumpForceForFrame(Mathf.Clamp01(this.m_CurrentJumpTimer / this.m_StoppingJumpTime));
			if (this.m_CurrentJumpTimer >= this.m_StoppingJumpTime)
			{
				this.StopJump();
			}
		}

		private float CalculateJumpForceForFrame(float normalizedJumpTime)
		{
			float a = 7f;
			float b = 5f;
			float num = 4f;
			float num2 = Mathf.Lerp(a, b, Mathf.Clamp01(this.m_JumpHeight / num));
			if (this.m_DisableGravityDuringJump)
			{
				num2 /= 1.5f;
			}
			return (1f - normalizedJumpTime) * this.m_JumpHeight * num2;
		}

		private void StopJump()
		{
			this.m_IsJumping = false;
			if (this.m_DisableGravityDuringJump)
			{
				this.RemoveGravityLock();
			}
		}

		private void StartCoyoteTimer()
		{
			this.m_CurrentJumpForgivenessWindowTime = this.m_JumpForgivenessWindow;
		}

		public bool IsPausingGravity()
		{
			return this.m_IsJumping && this.m_DisableGravityDuringJump;
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
			this.gravityPaused = false;
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (!isGrounded)
			{
				if (!this.m_IsJumping)
				{
					this.StartCoyoteTimer();
					return;
				}
			}
			else
			{
				this.m_CurrentJumpForgivenessWindowTime = 0f;
				this.m_JumpVector = Vector3.zero;
				this.m_CurrentInAirJumpCount = this.m_InAirJumpCount;
				if (this.m_IsJumping)
				{
					this.StopJump();
				}
			}
		}

		protected virtual void OnGravityLockChanged(GravityOverride gravityOverride)
		{
			if (gravityOverride == GravityOverride.ForcedOn)
			{
				this.gravityPaused = false;
			}
		}

		[SerializeField]
		[Tooltip("Disable gravity during the jump. This will result in a more floaty jump.")]
		private bool m_DisableGravityDuringJump;

		[SerializeField]
		[Tooltip("Allow player to jump without being grounded.")]
		private bool m_UnlimitedInAirJumps;

		[SerializeField]
		[Tooltip("The number of times a player can jump before landing.")]
		private int m_InAirJumpCount = 1;

		[SerializeField]
		[Tooltip("The time window after leaving the ground that a jump can still be performed. Sometimes known as coyote time.")]
		private float m_JumpForgivenessWindow = 0.25f;

		[SerializeField]
		[Tooltip("The height (approximately in meters) the player will be when reaching the apex of the jump.")]
		private float m_JumpHeight = 1.25f;

		[SerializeField]
		[Tooltip("Allow the player to stop their jump early when input is released before reaching the maximum jump height.")]
		private bool m_VariableHeightJump = true;

		[SerializeField]
		[Tooltip("The minimum amount of time the jump will execute for.")]
		private float m_MinJumpHoldTime = 0.1f;

		[SerializeField]
		[Tooltip("The maximum time a player can hold down the jump button to increase altitude.")]
		private float m_MaxJumpHoldTime = 0.5f;

		[SerializeField]
		[Tooltip("The speed at which the jump will decelerate when the player releases the jump button early.")]
		private float m_EarlyOutDecelerationSpeed = 0.1f;

		[SerializeField]
		[Tooltip("Input data that will be used to perform a jump.")]
		private XRInputButtonReader m_JumpInput = new XRInputButtonReader("Jump", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		private bool m_IsJumping;

		private bool m_HasJumped;

		private float m_CurrentJumpForgivenessWindowTime;

		private float m_StoppingJumpTime;

		private float m_CurrentJumpForceThisFrame;

		private Vector3 m_JumpVector;

		private GravityProvider m_GravityProvider;

		private bool m_HasGravityProvider;

		private float m_CurrentJumpTimer;

		private int m_CurrentInAirJumpCount;
	}
}
