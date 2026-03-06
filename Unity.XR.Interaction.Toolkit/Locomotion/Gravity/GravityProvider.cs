using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity
{
	[AddComponentMenu("XR/Locomotion/Gravity Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity.GravityProvider.html")]
	[DefaultExecutionOrder(-207)]
	public class GravityProvider : LocomotionProvider
	{
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

		public bool useLocalSpaceGravity
		{
			get
			{
				return this.m_UseLocalSpaceGravity;
			}
			set
			{
				this.m_UseLocalSpaceGravity = value;
			}
		}

		public float terminalVelocity
		{
			get
			{
				return this.m_TerminalVelocity;
			}
			set
			{
				this.m_TerminalVelocity = value;
			}
		}

		public float gravityAccelerationModifier
		{
			get
			{
				return this.m_GravityAccelerationModifier;
			}
			set
			{
				this.m_GravityAccelerationModifier = value;
			}
		}

		public bool updateCharacterControllerCenterEachFrame
		{
			get
			{
				return this.m_UpdateCharacterControllerCenterEachFrame;
			}
			set
			{
				this.m_UpdateCharacterControllerCenterEachFrame = value;
			}
		}

		public float sphereCastRadius
		{
			get
			{
				return this.m_SphereCastRadius;
			}
			set
			{
				this.m_SphereCastRadius = value;
			}
		}

		public float sphereCastDistanceBuffer
		{
			get
			{
				return this.m_SphereCastDistanceBuffer;
			}
			set
			{
				this.m_SphereCastDistanceBuffer = value;
			}
		}

		public LayerMask sphereCastLayerMask
		{
			get
			{
				return this.m_SphereCastLayerMask;
			}
			set
			{
				this.m_SphereCastLayerMask = value;
			}
		}

		public QueryTriggerInteraction sphereCastTriggerInteraction
		{
			get
			{
				return this.m_SphereCastTriggerInteraction;
			}
			set
			{
				this.m_SphereCastTriggerInteraction = value;
			}
		}

		public UnityEvent<GravityOverride> onGravityLockChanged
		{
			get
			{
				return this.m_OnGravityLockChanged;
			}
			set
			{
				this.m_OnGravityLockChanged = value;
			}
		}

		public UnityEvent<bool> onGroundedChanged
		{
			get
			{
				return this.m_OnGroundedChanged;
			}
		}

		public bool isGrounded
		{
			get
			{
				return this.m_IsGrounded;
			}
		}

		public XROriginMovement transformation { get; set; } = new XROriginMovement();

		public List<IGravityController> gravityControllers
		{
			get
			{
				return this.m_GravityControllers;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			if (base.mediator != null)
			{
				base.mediator.GetComponentsInChildren<IGravityController>(true, this.m_GravityControllers);
			}
		}

		protected virtual void Start()
		{
			this.FindHeadTransform();
		}

		protected virtual void Update()
		{
			this.CheckGrounded();
			if (this.m_IsGrounded && base.locomotionState == LocomotionState.Moving)
			{
				base.TryEndLocomotion();
				this.ResetFallForce();
			}
			if (this.TryProcessGravity(Time.deltaTime))
			{
				base.TryStartLocomotionImmediately();
				if (base.locomotionState == LocomotionState.Moving)
				{
					this.transformation.motion = this.m_CurrentFallVelocity * Time.deltaTime;
					base.TryQueueTransformation(this.transformation);
				}
			}
			if (this.m_HeadTransform != null && this.m_CharacterController != null && this.m_UpdateCharacterControllerCenterEachFrame)
			{
				this.m_CharacterController.center = new Vector3(this.m_HeadTransform.localPosition.x, this.m_CharacterController.center.y, this.m_HeadTransform.localPosition.z);
			}
		}

		protected virtual bool TryProcessGravity(float time)
		{
			if (this.IsGravityBlocked())
			{
				this.ResetFallForce();
				return false;
			}
			if (!Mathf.Approximately(this.m_CurrentFallVelocity.sqrMagnitude, this.m_TerminalVelocity * this.m_TerminalVelocity))
			{
				this.m_CurrentFallVelocity = Vector3.ClampMagnitude(this.m_CurrentFallVelocity + this.m_GravityAccelerationModifier * time * this.GetCurrentGravity(), this.m_TerminalVelocity);
			}
			return true;
		}

		public Vector3 GetCurrentUp()
		{
			if (!this.m_UseLocalSpaceGravity)
			{
				return -Physics.gravity.normalized;
			}
			return base.mediator.xrOrigin.Origin.transform.up;
		}

		private Vector3 GetCurrentGravity()
		{
			if (!this.m_UseLocalSpaceGravity)
			{
				return Physics.gravity;
			}
			return -base.mediator.xrOrigin.Origin.transform.up * Physics.gravity.magnitude;
		}

		public bool IsGravityBlocked()
		{
			return !this.m_UseGravity || this.m_IsGrounded || !this.CanProcessGravity();
		}

		public void ResetFallForce()
		{
			this.m_CurrentFallVelocity = Vector3.zero;
		}

		private bool CanProcessGravity()
		{
			if (this.m_GravityForcedOffProviders.Count > 0)
			{
				return false;
			}
			if (this.m_GravityForcedOnProviders.Count > 0)
			{
				return true;
			}
			foreach (IGravityController gravityController in this.m_GravityControllers)
			{
				if (gravityController.canProcess && gravityController.gravityPaused)
				{
					return false;
				}
			}
			return true;
		}

		public bool TryLockGravity(IGravityController provider, GravityOverride gravityOverride)
		{
			if (this.m_GravityForcedOffProviders.Contains(provider) || this.m_GravityForcedOnProviders.Contains(provider))
			{
				string format = "Gravity Provider is already being locked by {0}. Unlock first before trying to lock again.";
				Object @object = provider as Object;
				Debug.LogWarning(string.Format(format, (@object != null) ? @object.name : provider), (provider as Object) ?? this);
				return false;
			}
			if (gravityOverride == GravityOverride.ForcedOff)
			{
				this.m_GravityForcedOffProviders.Add(provider);
			}
			else if (gravityOverride == GravityOverride.ForcedOn)
			{
				this.m_GravityForcedOnProviders.Add(provider);
			}
			foreach (IGravityController gravityController in this.m_GravityControllers)
			{
				gravityController.OnGravityLockChanged(gravityOverride);
			}
			UnityEvent<GravityOverride> onGravityLockChanged = this.m_OnGravityLockChanged;
			if (onGravityLockChanged != null)
			{
				onGravityLockChanged.Invoke(gravityOverride);
			}
			return true;
		}

		public void UnlockGravity(IGravityController provider)
		{
			this.m_GravityForcedOnProviders.Remove(provider);
			this.m_GravityForcedOffProviders.Remove(provider);
		}

		private void CheckGrounded()
		{
			bool isGrounded = this.m_IsGrounded;
			this.m_IsGrounded = (this.m_LocalPhysicsScene.SphereCast(this.GetBodyHeadPosition(), this.m_SphereCastRadius, -this.GetCurrentUp(), this.m_GroundedAllocHits, this.GetLocalHeadHeight(), this.m_SphereCastLayerMask, this.m_SphereCastTriggerInteraction) > 0);
			if (isGrounded != this.m_IsGrounded)
			{
				foreach (IGravityController gravityController in this.m_GravityControllers)
				{
					gravityController.OnGroundedChanged(this.m_IsGrounded);
				}
				UnityEvent<bool> onGroundedChanged = this.m_OnGroundedChanged;
				if (onGroundedChanged == null)
				{
					return;
				}
				onGroundedChanged.Invoke(this.m_IsGrounded);
			}
		}

		private float GetLocalHeadHeight()
		{
			return base.mediator.xrOrigin.CameraInOriginSpaceHeight + this.m_SphereCastDistanceBuffer;
		}

		private Vector3 GetBodyHeadPosition()
		{
			if (this.m_CharacterController == null)
			{
				this.FindCharacterController();
			}
			if (this.m_HeadTransform == null)
			{
				this.FindHeadTransform();
				if (this.m_HeadTransform == null)
				{
					if (!(this.m_CharacterController != null))
					{
						return base.transform.position;
					}
					return this.m_CharacterController.bounds.center;
				}
			}
			if (this.m_CharacterController == null && this.m_UpdateCharacterControllerCenterEachFrame)
			{
				return this.m_HeadTransform.position;
			}
			Vector3 center = this.m_CharacterController.bounds.center;
			return new Vector3(center.x, this.m_HeadTransform.position.y, center.z);
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

		private void FindHeadTransform()
		{
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (!(xrOrigin != null))
			{
				Debug.LogError("XR Origin is not available through the Locomotion Mediator, cannot obtain Transform reference to use as the head position. Disabling Gravity Provider.", this);
				base.enabled = false;
				return;
			}
			Camera camera = xrOrigin.Camera;
			if (camera != null)
			{
				this.m_HeadTransform = camera.transform;
				return;
			}
			Debug.LogError("Camera is not set in XR Origin, cannot obtain Transform reference to use as the head position. Disabling Gravity Provider.", this);
			base.enabled = false;
		}

		protected void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying || this.m_HeadTransform == null)
			{
				return;
			}
			Color color = this.m_IsGrounded ? Color.green : Color.red;
			Gizmos.color = color;
			Vector3 bodyHeadPosition = this.GetBodyHeadPosition();
			Vector3 vector = bodyHeadPosition + -this.GetCurrentUp() * this.m_GroundedAllocHits[0].distance;
			Gizmos.DrawWireSphere(vector, this.m_SphereCastRadius);
			Gizmos.DrawSphere(this.m_GroundedAllocHits[0].point, 0.025f);
			Debug.DrawLine(bodyHeadPosition, vector, color);
		}

		[SerializeField]
		[Tooltip("Apply gravity to the XR Origin.")]
		private bool m_UseGravity = true;

		[SerializeField]
		[Tooltip("Apply gravity based on the current Up vector of the XR Origin.")]
		private bool m_UseLocalSpaceGravity = true;

		[SerializeField]
		[Tooltip("Determines the maximum fall speed based on units per second.")]
		private float m_TerminalVelocity = 90f;

		[SerializeField]
		[Tooltip("Determines the speed at which a player reaches max gravity velocity.")]
		private float m_GravityAccelerationModifier = 1f;

		[SerializeField]
		[Tooltip("Sets the center of the character controller to match the local x and z positions of the player camera.")]
		private bool m_UpdateCharacterControllerCenterEachFrame = true;

		[SerializeField]
		[Tooltip("Buffer for the radius of the sphere cast used to check if the player is grounded.")]
		private float m_SphereCastRadius = 0.09f;

		[SerializeField]
		[Tooltip("Buffer for the distance of the sphere cast used to check if the player is grounded.")]
		private float m_SphereCastDistanceBuffer = -0.05f;

		[SerializeField]
		[Tooltip("The layer mask used for the sphere cast to check if the player is grounded.")]
		private LayerMask m_SphereCastLayerMask = -5;

		[SerializeField]
		[Tooltip("Whether trigger colliders are considered when using a sphere cast to determine if grounded. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.")]
		private QueryTriggerInteraction m_SphereCastTriggerInteraction = QueryTriggerInteraction.Ignore;

		[Tooltip("Event that is called when gravity lock is changed.")]
		[SerializeField]
		private UnityEvent<GravityOverride> m_OnGravityLockChanged = new UnityEvent<GravityOverride>();

		[Tooltip("Callback for anytime the grounded state changes.")]
		[SerializeField]
		private UnityEvent<bool> m_OnGroundedChanged = new UnityEvent<bool>();

		private bool m_IsGrounded;

		private readonly List<IGravityController> m_GravityControllers = new List<IGravityController>();

		private Transform m_HeadTransform;

		private readonly RaycastHit[] m_GroundedAllocHits = new RaycastHit[1];

		private Vector3 m_CurrentFallVelocity;

		private PhysicsScene m_LocalPhysicsScene;

		private CharacterController m_CharacterController;

		private bool m_AttemptedGetCharacterController;

		private readonly List<IGravityController> m_GravityForcedOnProviders = new List<IGravityController>();

		private readonly List<IGravityController> m_GravityForcedOffProviders = new List<IGravityController>();
	}
}
