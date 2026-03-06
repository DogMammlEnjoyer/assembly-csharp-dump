using System;
using UnityEngine;

namespace Unity.XR.CoreUtils.Capabilities
{
	public abstract class CapabilityProfile : ScriptableObject
	{
		public static event Action<CapabilityProfile> CapabilityChanged;

		public void ReportCapabilityChanged()
		{
			Action<CapabilityProfile> capabilityChanged = CapabilityProfile.CapabilityChanged;
			if (capabilityChanged == null)
			{
				return;
			}
			capabilityChanged(this);
		}
	}
}
