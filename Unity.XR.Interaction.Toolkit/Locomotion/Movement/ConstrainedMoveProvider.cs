using System;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public abstract class ConstrainedMoveProvider : LocomotionProvider
	{
		public bool enableFreeXMovement
		{
			get
			{
				return this.m_EnableFreeXMovement;
			}
			set
			{
				this.m_EnableFreeXMovement = value;
			}
		}

		public bool enableFreeYMovement
		{
			get
			{
				return this.m_EnableFreeYMovement;
			}
			set
			{
				this.m_EnableFreeYMovement = value;
			}
		}

		public bool enableFreeZMovement
		{
			get
			{
				return this.m_EnableFreeZMovement;
			}
			set
			{
				this.m_EnableFreeZMovement = value;
			}
		}

		public XROriginMovement transformation { get; set; } = new XROriginMovement();

		protected override void Awake()
		{
			base.Awake();
			if (ComponentLocatorUtility<GravityProvider>.TryFindComponent(out this.m_GravityProvider) && !this.m_UseGravity)
			{
				this.MigrateUseGravityToGravityProvider();
			}
		}

		protected void Update()
		{
			this.m_IsMovingXROrigin = false;
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (((xrOrigin != null) ? xrOrigin.Origin : null) == null)
			{
				return;
			}
			bool flag;
			Vector3 translationInWorldSpace = this.ComputeDesiredMove(out flag);
			ConstrainedMoveProvider.GravityApplicationMode gravityApplicationMode = this.m_GravityApplicationMode;
			if (gravityApplicationMode != ConstrainedMoveProvider.GravityApplicationMode.AttemptingMove)
			{
				if (gravityApplicationMode == ConstrainedMoveProvider.GravityApplicationMode.Immediately)
				{
					this.MoveRig(translationInWorldSpace);
				}
			}
			else if (flag || this.m_GravityDrivenVelocity != Vector3.zero)
			{
				this.MoveRig(translationInWorldSpace);
			}
			if (!this.m_IsMovingXROrigin)
			{
				base.TryEndLocomotion();
			}
		}

		protected abstract Vector3 ComputeDesiredMove(out bool attemptingMove);

		protected virtual void MoveRig(Vector3 translationInWorldSpace)
		{
			this.FindCharacterController();
			Vector3 vector = translationInWorldSpace;
			if (!this.m_EnableFreeXMovement)
			{
				vector.x = 0f;
			}
			if (!this.m_EnableFreeYMovement)
			{
				vector.y = 0f;
			}
			if (!this.m_EnableFreeZMovement)
			{
				vector.z = 0f;
			}
			if (this.m_GravityProvider == null && this.m_CharacterController != null && this.m_CharacterController.enabled)
			{
				if (!this.m_UseGravity || this.m_CharacterController.isGrounded)
				{
					this.m_GravityDrivenVelocity = Vector3.zero;
				}
				else
				{
					this.m_GravityDrivenVelocity += Physics.gravity * Time.deltaTime;
					if (this.m_EnableFreeXMovement)
					{
						this.m_GravityDrivenVelocity.x = 0f;
					}
					if (this.m_EnableFreeYMovement)
					{
						this.m_GravityDrivenVelocity.y = 0f;
					}
					if (this.m_EnableFreeZMovement)
					{
						this.m_GravityDrivenVelocity.z = 0f;
					}
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

		[Obsolete("gravityMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
		public ConstrainedMoveProvider.GravityApplicationMode gravityMode
		{
			get
			{
				return this.m_GravityApplicationMode;
			}
			set
			{
				this.m_GravityApplicationMode = value;
			}
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
		[Tooltip("Controls whether to enable unconstrained movement along the x-axis.")]
		private bool m_EnableFreeXMovement = true;

		[SerializeField]
		[Tooltip("Controls whether to enable unconstrained movement along the y-axis.")]
		private bool m_EnableFreeYMovement;

		[SerializeField]
		[Tooltip("Controls whether to enable unconstrained movement along the z-axis.")]
		private bool m_EnableFreeZMovement = true;

		private CharacterController m_CharacterController;

		private bool m_AttemptedGetCharacterController;

		private bool m_IsMovingXROrigin;

		private GravityProvider m_GravityProvider;

		[SerializeField]
		[Tooltip("Controls when gravity begins to take effect.")]
		[Obsolete("m_GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
		private ConstrainedMoveProvider.GravityApplicationMode m_GravityApplicationMode;

		[SerializeField]
		[Tooltip("Controls whether gravity applies to constrained axes when a Character Controller is used. Ignored when a Gravity Provider component is found in the scene.")]
		[Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
		private bool m_UseGravity = true;

		[Obsolete("Controlling gravity directly in the move provider has been deprecated in XRI 3.1.0, use Gravity Provider instead.")]
		private Vector3 m_GravityDrivenVelocity;

		[Obsolete("GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version.")]
		public enum GravityApplicationMode
		{
			AttemptingMove,
			Immediately
		}
	}
}
