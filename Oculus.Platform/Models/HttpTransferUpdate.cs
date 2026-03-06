using System;
using System.Runtime.InteropServices;

namespace Oculus.Platform.Models
{
	public class HttpTransferUpdate
	{
		public HttpTransferUpdate(IntPtr o)
		{
			this.ID = CAPI.ovr_HttpTransferUpdate_GetID(o);
			this.IsCompleted = CAPI.ovr_HttpTransferUpdate_IsCompleted(o);
			long num = (long)((ulong)CAPI.ovr_HttpTransferUpdate_GetSize(o));
			this.Payload = new byte[num];
			Marshal.Copy(CAPI.ovr_Packet_GetBytes(o), this.Payload, 0, (int)num);
		}

		public readonly ulong ID;

		public readonly byte[] Payload;

		public readonly bool IsCompleted;
	}
}
