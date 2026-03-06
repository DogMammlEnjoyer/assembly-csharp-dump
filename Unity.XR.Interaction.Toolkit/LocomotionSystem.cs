using System;
using Unity.XR.CoreUtils;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/Locomotion/Legacy/Locomotion System", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.LocomotionSystem.html")]
	[Obsolete("LocomotionSystem is deprecated and will be removed in a future release. Use LocomotionMediator instead.", false)]
	public class LocomotionSystem : MonoBehaviour
	{
		public float timeout
		{
			get
			{
				return this.m_Timeout;
			}
			set
			{
				this.m_Timeout = value;
			}
		}

		public XROrigin xrOrigin
		{
			get
			{
				return this.m_XROrigin;
			}
			set
			{
				this.m_XROrigin = value;
			}
		}

		public bool busy
		{
			get
			{
				return this.m_CurrentExclusiveProvider != null;
			}
		}

		[Obsolete("xrRig is marked for deprecation and will be removed in a future version. Please use xrOrigin instead.", true)]
		public XRRig xrRig
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("Busy has been deprecated. Use busy instead. (UnityUpgradable) -> busy", true)]
		public bool Busy
		{
			get
			{
				return false;
			}
		}

		protected void Awake()
		{
			if (this.m_XROrigin == null)
			{
				this.m_XROrigin = base.GetComponentInParent<XROrigin>();
				if (this.m_XROrigin == null)
				{
					ComponentLocatorUtility<XROrigin>.TryFindComponent(out this.m_XROrigin);
				}
			}
			LocomotionMediator locomotionMediator;
			if (ComponentLocatorUtility<LocomotionMediator>.TryFindComponent(out locomotionMediator))
			{
				Debug.LogWarning("This scene contains both a Locomotion System and a Locomotion Mediator, which may result in unexpected locomotion behavior. It is recommended to use the Locomotion Mediator.", this);
			}
		}

		protected void Update()
		{
			if (this.m_CurrentExclusiveProvider != null && Time.time > this.m_TimeMadeExclusive + this.m_Timeout)
			{
				this.ResetExclusivity();
			}
		}

		public RequestResult RequestExclusiveOperation(LocomotionProvider provider)
		{
			if (provider == null)
			{
				return RequestResult.Error;
			}
			if (this.m_CurrentExclusiveProvider == null)
			{
				this.m_CurrentExclusiveProvider = provider;
				this.m_TimeMadeExclusive = Time.time;
				return RequestResult.Success;
			}
			if (!(this.m_CurrentExclusiveProvider != provider))
			{
				return RequestResult.Error;
			}
			return RequestResult.Busy;
		}

		private void ResetExclusivity()
		{
			this.m_CurrentExclusiveProvider = null;
			this.m_TimeMadeExclusive = 0f;
		}

		public RequestResult FinishExclusiveOperation(LocomotionProvider provider)
		{
			if (provider == null || this.m_CurrentExclusiveProvider == null)
			{
				return RequestResult.Error;
			}
			if (this.m_CurrentExclusiveProvider == provider)
			{
				this.ResetExclusivity();
				return RequestResult.Success;
			}
			return RequestResult.Error;
		}

		private LocomotionProvider m_CurrentExclusiveProvider;

		private float m_TimeMadeExclusive;

		[SerializeField]
		[Tooltip("The timeout (in seconds) for exclusive access to the XR Origin.")]
		private float m_Timeout = 10f;

		[SerializeField]
		[FormerlySerializedAs("m_XRRig")]
		[Tooltip("The XR Origin object to provide access control to.")]
		private XROrigin m_XROrigin;
	}
}
