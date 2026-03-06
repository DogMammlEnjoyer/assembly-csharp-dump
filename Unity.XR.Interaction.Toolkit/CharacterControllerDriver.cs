using System;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Character Controller Driver", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.CharacterControllerDriver.html")]
	[Obsolete("CharacterControllerDriver is deprecated in XRI 3.0.0 and will be removed in a future release. Instead set useCharacterControllerIfExists to true on the instance of XRBodyTransformer in the scene, and then, if at runtime, re-enable the Body Transformer to make the locomotion system drive the CharacterController.", false)]
	public class CharacterControllerDriver : MonoBehaviour
	{
		public LocomotionProvider locomotionProvider
		{
			get
			{
				return this.m_LocomotionProvider;
			}
			set
			{
				this.Unsubscribe(this.m_LocomotionProvider);
				this.m_LocomotionProvider = value;
				this.Subscribe(this.m_LocomotionProvider);
				this.SetupCharacterController();
				this.UpdateCharacterController();
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_MinHeight;
			}
			set
			{
				this.m_MinHeight = value;
			}
		}

		public float maxHeight
		{
			get
			{
				return this.m_MaxHeight;
			}
			set
			{
				this.m_MaxHeight = value;
			}
		}

		protected XROrigin xrOrigin
		{
			get
			{
				return this.m_XROrigin;
			}
		}

		[Obsolete("xrRig has been deprecated. Use xrOrigin instead.", true)]
		protected XRRig xrRig
		{
			get
			{
				return null;
			}
		}

		protected CharacterController characterController
		{
			get
			{
				return this.m_CharacterController;
			}
		}

		protected void Awake()
		{
			if (this.m_LocomotionProvider == null)
			{
				this.m_LocomotionProvider = base.GetComponent<ContinuousMoveProviderBase>();
				if (this.m_LocomotionProvider == null)
				{
					this.m_LocomotionProvider = ComponentLocatorUtility<ContinuousMoveProviderBase>.FindComponent();
					if (this.m_LocomotionProvider == null)
					{
						Debug.LogWarning("Unable to drive properties of the Character Controller without the locomotion events of a Locomotion Provider. Set Locomotion Provider or ensure a Continuous Move Provider component is in your scene.", this);
					}
				}
			}
		}

		protected void OnEnable()
		{
			this.Subscribe(this.m_LocomotionProvider);
		}

		protected void OnDisable()
		{
			this.Unsubscribe(this.m_LocomotionProvider);
		}

		protected void Start()
		{
			this.SetupCharacterController();
			this.UpdateCharacterController();
		}

		protected virtual void UpdateCharacterController()
		{
			if (this.m_XROrigin == null || this.m_CharacterController == null)
			{
				return;
			}
			float num = Mathf.Clamp(this.m_XROrigin.CameraInOriginSpaceHeight, this.m_MinHeight, this.m_MaxHeight);
			Vector3 cameraInOriginSpacePos = this.m_XROrigin.CameraInOriginSpacePos;
			cameraInOriginSpacePos.y = num / 2f + this.m_CharacterController.skinWidth;
			this.m_CharacterController.height = num;
			this.m_CharacterController.center = cameraInOriginSpacePos;
		}

		private void Subscribe(LocomotionProvider provider)
		{
			if (provider != null)
			{
				provider.beginLocomotion += this.OnBeginLocomotion;
				provider.endLocomotion += this.OnEndLocomotion;
			}
		}

		private void Unsubscribe(LocomotionProvider provider)
		{
			if (provider != null)
			{
				provider.beginLocomotion -= this.OnBeginLocomotion;
				provider.endLocomotion -= this.OnEndLocomotion;
			}
		}

		private void SetupCharacterController()
		{
			if (this.m_LocomotionProvider == null || this.m_LocomotionProvider.system == null)
			{
				return;
			}
			this.m_XROrigin = this.m_LocomotionProvider.system.xrOrigin;
			this.m_CharacterController = ((this.m_XROrigin != null) ? this.m_XROrigin.Origin.GetComponent<CharacterController>() : null);
			if (this.m_CharacterController == null && this.m_XROrigin != null)
			{
				Debug.LogError(string.Format("Could not get CharacterController on {0}, unable to drive properties.", this.m_XROrigin.Origin) + string.Format(" Ensure there is a CharacterController on the \"Rig\" GameObject of {0}.", this.m_XROrigin), this);
			}
		}

		private void OnBeginLocomotion(LocomotionSystem system)
		{
			this.UpdateCharacterController();
		}

		private void OnEndLocomotion(LocomotionSystem system)
		{
			this.UpdateCharacterController();
		}

		[SerializeField]
		[Tooltip("The Locomotion Provider object to listen to.")]
		private LocomotionProvider m_LocomotionProvider;

		[SerializeField]
		[Tooltip("The minimum height of the character's capsule that will be set by this behavior.")]
		private float m_MinHeight;

		[SerializeField]
		[Tooltip("The maximum height of the character's capsule that will be set by this behavior.")]
		private float m_MaxHeight = float.PositiveInfinity;

		private XROrigin m_XROrigin;

		private CharacterController m_CharacterController;
	}
}
