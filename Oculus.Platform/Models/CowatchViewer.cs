using System;

namespace Oculus.Platform.Models
{
	public class CowatchViewer
	{
		public CowatchViewer(IntPtr o)
		{
			this.Data = CAPI.ovr_CowatchViewer_GetData(o);
			this.Id = CAPI.ovr_CowatchViewer_GetId(o);
		}

		public readonly string Data;

		public readonly ulong Id;
	}
}
