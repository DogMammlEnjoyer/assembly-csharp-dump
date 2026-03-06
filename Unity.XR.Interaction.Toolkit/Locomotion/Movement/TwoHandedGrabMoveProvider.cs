using System;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement
{
	[DefaultExecutionOrder(-209)]
	[AddComponentMenu("XR/Locomotion/Two-Handed Grab Move Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement.TwoHandedGrabMoveProvider.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class TwoHandedGrabMoveProvider : ConstrainedMoveProvider
	{
		public GrabMoveProvider leftGrabMoveProvider
		{
			get
			{
				return this.m_LeftGrabMoveProvider;
			}
			set
			{
				this.m_LeftGrabMoveProvider = value;
			}
		}

		public GrabMoveProvider rightGrabMoveProvider
		{
			get
			{
				return this.m_RightGrabMoveProvider;
			}
			set
			{
				this.m_RightGrabMoveProvider = value;
			}
		}

		public bool overrideSharedSettingsOnInit
		{
			get
			{
				return this.m_OverrideSharedSettingsOnInit;
			}
			set
			{
				this.m_OverrideSharedSettingsOnInit = value;
			}
		}

		public float moveFactor
		{
			get
			{
				return this.m_MoveFactor;
			}
			set
			{
				this.m_MoveFactor = value;
			}
		}

		public bool requireTwoHandsForTranslation
		{
			get
			{
				return this.m_RequireTwoHandsForTranslation;
			}
			set
			{
				this.m_RequireTwoHandsForTranslation = value;
			}
		}

		public bool enableRotation
		{
			get
			{
				return this.m_EnableRotation;
			}
			set
			{
				this.m_EnableRotation = value;
			}
		}

		public bool enableScaling
		{
			get
			{
				return this.m_EnableScaling;
			}
			set
			{
				this.m_EnableScaling = value;
			}
		}

		public float minimumScale
		{
			get
			{
				return this.m_MinimumScale;
			}
			set
			{
				this.m_MinimumScale = value;
			}
		}

		public float maximumScale
		{
			get
			{
				return this.m_MaximumScale;
			}
			set
			{
				this.m_MaximumScale = value;
			}
		}

		public XRBodyYawRotation rotateTransformation { get; set; } = new XRBodyYawRotation();

		public XRBodyScale scaleTransformation { get; set; } = new XRBodyScale();

		protected void OnEnable()
		{
			if (this.m_LeftGrabMoveProvider == null || this.m_RightGrabMoveProvider == null)
			{
				Debug.LogError("Left or Right Grab Move Provider is not set or has been destroyed.", this);
				base.enabled = false;
				return;
			}
			if (this.m_RequireTwoHandsForTranslation)
			{
				this.m_LeftGrabMoveProvider.canMove = false;
				this.m_RightGrabMoveProvider.canMove = false;
			}
			if (this.m_OverrideSharedSettingsOnInit)
			{
				this.m_LeftGrabMoveProvider.mediator = base.mediator;
				this.m_LeftGrabMoveProvider.enableFreeXMovement = base.enableFreeXMovement;
				this.m_LeftGrabMoveProvider.enableFreeYMovement = base.enableFreeYMovement;
				this.m_LeftGrabMoveProvider.enableFreeZMovement = base.enableFreeZMovement;
				this.m_LeftGrabMoveProvider.moveFactor = this.m_MoveFactor;
				this.m_RightGrabMoveProvider.mediator = base.mediator;
				this.m_RightGrabMoveProvider.enableFreeXMovement = base.enableFreeXMovement;
				this.m_RightGrabMoveProvider.enableFreeYMovement = base.enableFreeYMovement;
				this.m_RightGrabMoveProvider.enableFreeZMovement = base.enableFreeZMovement;
				this.m_RightGrabMoveProvider.moveFactor = this.m_MoveFactor;
				this.m_LeftGrabMoveProvider.useGravity = base.useGravity;
				this.m_RightGrabMoveProvider.useGravity = base.useGravity;
			}
		}

		protected void OnDisable()
		{
			if (this.m_LeftGrabMoveProvider != null)
			{
				this.m_LeftGrabMoveProvider.canMove = true;
			}
			if (this.m_RightGrabMoveProvider != null)
			{
				this.m_RightGrabMoveProvider.canMove = true;
			}
		}

		protected override Vector3 ComputeDesiredMove(out bool attemptingMove)
		{
			attemptingMove = false;
			bool isMoving = this.m_IsMoving;
			XROrigin xrOrigin = base.mediator.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			this.m_IsMoving = (this.m_LeftGrabMoveProvider.IsGrabbing() && this.m_RightGrabMoveProvider.IsGrabbing() && gameObject != null);
			if (!this.m_IsMoving)
			{
				if (!this.m_RequireTwoHandsForTranslation)
				{
					this.m_LeftGrabMoveProvider.canMove = true;
					this.m_RightGrabMoveProvider.canMove = true;
				}
				return Vector3.zero;
			}
			this.m_LeftGrabMoveProvider.canMove = false;
			this.m_RightGrabMoveProvider.canMove = false;
			Transform transform = gameObject.transform;
			Vector3 localPosition = this.m_LeftGrabMoveProvider.controllerTransform.localPosition;
			Vector3 localPosition2 = this.m_RightGrabMoveProvider.controllerTransform.localPosition;
			Vector3 vector = (localPosition + localPosition2) * 0.5f;
			if (!isMoving && this.m_IsMoving)
			{
				this.m_InitialOriginYaw = transform.eulerAngles.y;
				this.m_InitialLeftToRightDirection = localPosition2 - localPosition;
				this.m_InitialLeftToRightDirection.y = 0f;
				this.m_InitialLeftToRightOrthogonal = Quaternion.AngleAxis(90f, Vector3.down) * this.m_InitialLeftToRightDirection;
				this.m_InitialOriginScale = transform.localScale.x;
				this.m_InitialDistanceBetweenHands = Vector3.Distance(localPosition, localPosition2);
				this.m_PreviousMidpointBetweenControllers = vector;
				return Vector3.zero;
			}
			attemptingMove = true;
			Vector3 result = transform.TransformVector(this.m_PreviousMidpointBetweenControllers - vector) * this.m_MoveFactor;
			this.m_PreviousMidpointBetweenControllers = vector;
			return result;
		}

		protected override void MoveRig(Vector3 translationInWorldSpace)
		{
			base.MoveRig(translationInWorldSpace);
			XROrigin xrOrigin = base.mediator.xrOrigin;
			GameObject gameObject = (xrOrigin != null) ? xrOrigin.Origin : null;
			if (gameObject == null)
			{
				return;
			}
			Transform transform = gameObject.transform;
			Vector3 localPosition = this.m_LeftGrabMoveProvider.controllerTransform.localPosition;
			Vector3 localPosition2 = this.m_RightGrabMoveProvider.controllerTransform.localPosition;
			if (this.m_EnableRotation)
			{
				Vector3 vector = localPosition2 - localPosition;
				vector.y = 0f;
				float num = Mathf.Sign(Vector3.Dot(this.m_InitialLeftToRightOrthogonal, vector));
				float num2 = this.m_InitialOriginYaw + Vector3.Angle(this.m_InitialLeftToRightDirection, vector) * num;
				this.rotateTransformation.angleDelta = num2 - transform.eulerAngles.y;
				base.TryQueueTransformation(this.rotateTransformation);
			}
			if (this.m_EnableScaling)
			{
				float num3 = Vector3.Distance(localPosition, localPosition2);
				float num4 = (num3 != 0f) ? (this.m_InitialOriginScale * (this.m_InitialDistanceBetweenHands / num3)) : transform.localScale.x;
				num4 = Mathf.Clamp(num4, this.m_MinimumScale, this.m_MaximumScale);
				this.scaleTransformation.uniformScale = num4;
				base.TryQueueTransformation(this.scaleTransformation);
			}
		}

		[SerializeField]
		[Tooltip("The left hand grab move instance which will be used as one half of two-handed locomotion.")]
		private GrabMoveProvider m_LeftGrabMoveProvider;

		[SerializeField]
		[Tooltip("The right hand grab move instance which will be used as one half of two-handed locomotion.")]
		private GrabMoveProvider m_RightGrabMoveProvider;

		[SerializeField]
		[Tooltip("Controls whether to override the settings for individual handed providers with this provider's settings on initialization.")]
		private bool m_OverrideSharedSettingsOnInit = true;

		[SerializeField]
		[Tooltip("The ratio of actual movement distance to controller movement distance.")]
		private float m_MoveFactor = 1f;

		[SerializeField]
		[Tooltip("Controls whether translation requires both grab move inputs to be active.")]
		private bool m_RequireTwoHandsForTranslation;

		[SerializeField]
		[Tooltip("Controls whether to enable yaw rotation of the user.")]
		private bool m_EnableRotation = true;

		[SerializeField]
		[Tooltip("Controls whether to enable uniform scaling of the user.")]
		private bool m_EnableScaling;

		[SerializeField]
		[Tooltip("The minimum user scale allowed.")]
		private float m_MinimumScale = 0.2f;

		[SerializeField]
		[Tooltip("The maximum user scale allowed.")]
		private float m_MaximumScale = 100f;

		private bool m_IsMoving;

		private Vector3 m_PreviousMidpointBetweenControllers;

		private float m_InitialOriginYaw;

		private Vector3 m_InitialLeftToRightDirection;

		private Vector3 m_InitialLeftToRightOrthogonal;

		private float m_InitialOriginScale;

		private float m_InitialDistanceBetweenHands;
	}
}
