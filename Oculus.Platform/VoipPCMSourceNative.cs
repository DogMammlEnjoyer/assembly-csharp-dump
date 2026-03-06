using System;

namespace Oculus.Platform
{
	public class VoipPCMSourceNative : IVoipPCMSource
	{
		public int GetPCM(float[] dest, int length)
		{
			return (int)((uint)CAPI.ovr_Voip_GetPCMFloat(this.senderID, dest, (UIntPtr)((ulong)((long)length))));
		}

		public void SetSenderID(ulong senderID)
		{
			this.senderID = senderID;
		}

		public int PeekSizeElements()
		{
			return (int)((uint)CAPI.ovr_Voip_GetPCMSize(this.senderID));
		}

		public void Update()
		{
		}

		private ulong senderID;
	}
}
