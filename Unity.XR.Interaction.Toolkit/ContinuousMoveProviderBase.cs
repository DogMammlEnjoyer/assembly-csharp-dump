using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("The ContinuousMoveProviderBase has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use ContinuousMoveProvider instead.", false)]
	public abstract class ContinuousMoveProviderBase : LocomotionProvider
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

		public bool useGravity
		{
			get
			{
				return this.m_UseGravity;
			}
			set
			{
				this.m_UseGravity = value;
			}
		}

		public ContinuousMoveProviderBase.GravityApplicationMode gravityApplicationMode
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

		protected void Update()
		{
			this.m_IsMovingXROrigin = false;
			XROrigin xrOrigin = base.system.xrOrigin;
			if (((xrOrigin != null) ? xrOrigin.Origin : null) == null)
			{
				return;
			}
			Vector2 vector = this.ReadInput();
			Vector3 translationInWorldSpace = this.ComputeDesiredMove(vector);
			ContinuousMoveProviderBase.GravityApplicationMode gravityApplicationMode = this.m_GravityApplicationMode;
			if (gravityApplicationMode != ContinuousMoveProviderBase.GravityApplicationMode.AttemptingMove)
			{
				if (gravityApplicationMode == ContinuousMoveProviderBase.GravityApplicationMode.Immediately)
				{
					this.MoveRig(translationInWorldSpace);
				}
			}
			else if (vector != Vector2.zero || this.m_VerticalVelocity != Vector3.zero)
			{
				this.MoveRig(translationInWorldSpace);
			}
			switch (base.locomotionPhase)
			{
			case LocomotionPhase.Idle:
			case LocomotionPhase.Started:
				if (this.m_IsMovingXROrigin)
				{
					base.locomotionPhase = LocomotionPhase.Moving;
					return;
				}
				break;
			case LocomotionPhase.Moving:
				if (!this.m_IsMovingXROrigin)
				{
					base.locomotionPhase = LocomotionPhase.Done;
					return;
				}
				break;
			case LocomotionPhase.Done:
				base.locomotionPhase = (this.m_IsMovingXROrigin ? LocomotionPhase.Moving : LocomotionPhase.Idle);
				break;
			default:
				return;
			}
		}

		protected abstract Vector2 ReadInput();

		protected virtual Vector3 ComputeDesiredMove(Vector2 input)
		{
			if (input == Vector2.zero)
			{
				return Vector3.zero;
			}
			XROrigin xrOrigin = base.system.xrOrigin;
			if (xrOrigin == null)
			{
				return Vector3.zero;
			}
			Vector3 vector = Vector3.ClampMagnitude(new Vector3(this.m_EnableStrafe ? input.x : 0f, 0f, input.y), 1f);
			Transform transform = (this.m_ForwardSource == null) ? xrOrigin.Camera.transform : this.m_ForwardSource;
			Vector3 vector2 = transform.forward;
			Transform transform2 = xrOrigin.Origin.transform;
			float d = this.m_MoveSpeed * Time.deltaTime * transform2.localScale.x;
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
			Vector3 direction = Quaternion.FromToRotation(transform2.forward, toDirection) * vector * d;
			return transform2.TransformDirection(direction);
		}

		protected virtual void MoveRig(Vector3 translationInWorldSpace)
		{
			XROrigin xrOrigin = base.system.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			if (gameObject == null)
			{
				return;
			}
			this.FindCharacterController();
			if (this.m_CharacterController != null && this.m_CharacterController.enabled)
			{
				if (this.m_CharacterController.isGrounded || !this.m_UseGravity || this.m_EnableFly)
				{
					this.m_VerticalVelocity = Vector3.zero;
				}
				else
				{
					this.m_VerticalVelocity += Physics.gravity * Time.deltaTime;
				}
				Vector3 motion = translationInWorldSpace + this.m_VerticalVelocity * Time.deltaTime;
				if (base.CanBeginLocomotion() && base.BeginLocomotion())
				{
					this.m_IsMovingXROrigin = true;
					this.m_CharacterController.Move(motion);
					base.EndLocomotion();
					return;
				}
			}
			else if (base.CanBeginLocomotion() && base.BeginLocomotion())
			{
				this.m_IsMovingXROrigin = true;
				gameObject.transform.position += translationInWorldSpace;
				base.EndLocomotion();
			}
		}

		private void FindCharacterController()
		{
			XROrigin xrOrigin = base.system.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			if (gameObject == null)
			{
				return;
			}
			if (this.m_CharacterController == null && !this.m_AttemptedGetCharacterController)
			{
				if (!gameObject.TryGetComponent<CharacterController>(out this.m_CharacterController) && gameObject != base.system.xrOrigin.gameObject)
				{
					base.system.xrOrigin.TryGetComponent<CharacterController>(out this.m_CharacterController);
				}
				this.m_AttemptedGetCharacterController = true;
			}
		}

		[SerializeField]
		[Tooltip("The speed, in units per second, to move forward.")]
		private float m_MoveSpeed = 1f;

		[SerializeField]
		[Tooltip("Controls whether to enable strafing (sideways movement).")]
		private bool m_EnableStrafe = true;

		[SerializeField]
		[Tooltip("Controls whether to enable flying (unconstrained movement). This overrides the use of gravity.")]
		private bool m_EnableFly;

		[SerializeField]
		[Tooltip("Controls whether gravity affects this provider when a Character Controller is used and flying is disabled.")]
		private bool m_UseGravity = true;

		[SerializeField]
		[Tooltip("Controls when gravity begins to take effect.")]
		private ContinuousMoveProviderBase.GravityApplicationMode m_GravityApplicationMode;

		[SerializeField]
		[Tooltip("The source Transform to define the forward direction.")]
		private Transform m_ForwardSource;

		private CharacterController m_CharacterController;

		private bool m_AttemptedGetCharacterController;

		private bool m_IsMovingXROrigin;

		private Vector3 m_VerticalVelocity;

		[Obsolete("GravityApplicationMode has been deprecated in XRI 3.0.0 and will be removed in a future version of XRI. Please use LocomotionMediator with a GravityProvider.", false)]
		public enum GravityApplicationMode
		{
			AttemptingMove,
			Immediately
		}
	}
}
