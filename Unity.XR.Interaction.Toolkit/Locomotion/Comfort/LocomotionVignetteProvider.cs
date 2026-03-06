using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[Serializable]
	public class LocomotionVignetteProvider : ITunnelingVignetteProvider
	{
		public LocomotionProvider locomotionProvider
		{
			get
			{
				return this.m_LocomotionProvider;
			}
			set
			{
				this.m_LocomotionProvider = value;
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				this.m_Enabled = value;
			}
		}

		public bool overrideDefaultParameters
		{
			get
			{
				return this.m_OverrideDefaultParameters;
			}
			set
			{
				this.m_OverrideDefaultParameters = value;
			}
		}

		public VignetteParameters overrideParameters
		{
			get
			{
				return this.m_OverrideParameters;
			}
			set
			{
				this.m_OverrideParameters = value;
			}
		}

		public VignetteParameters vignetteParameters
		{
			get
			{
				if (!this.m_OverrideDefaultParameters)
				{
					return null;
				}
				return this.m_OverrideParameters;
			}
		}

		[SerializeField]
		private LocomotionProvider m_LocomotionProvider;

		[SerializeField]
		private bool m_Enabled;

		[SerializeField]
		private bool m_OverrideDefaultParameters;

		[SerializeField]
		private VignetteParameters m_OverrideParameters = new VignetteParameters();
	}
}
