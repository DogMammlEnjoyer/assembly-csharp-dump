using System;

namespace Oculus.Platform.Models
{
	public class CowatchViewerUpdate
	{
		public CowatchViewerUpdate(IntPtr o)
		{
			this.DataList = new CowatchViewerList(CAPI.ovr_CowatchViewerUpdate_GetDataList(o));
			this.Id = CAPI.ovr_CowatchViewerUpdate_GetId(o);
		}

		public readonly CowatchViewerList DataList;

		public readonly ulong Id;
	}
}
