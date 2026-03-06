using System;

namespace Oculus.Platform.Models
{
	public class NetSyncVoipAttenuationValue
	{
		public NetSyncVoipAttenuationValue(IntPtr o)
		{
			this.Decibels = CAPI.ovr_NetSyncVoipAttenuationValue_GetDecibels(o);
			this.Distance = CAPI.ovr_NetSyncVoipAttenuationValue_GetDistance(o);
		}

		public readonly float Decibels;

		public readonly float Distance;
	}
}
