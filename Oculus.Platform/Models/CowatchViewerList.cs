using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class CowatchViewerList : DeserializableList<CowatchViewer>
	{
		public CowatchViewerList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_CowatchViewerArray_GetSize(a));
			this._Data = new List<CowatchViewer>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new CowatchViewer(CAPI.ovr_CowatchViewerArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_CowatchViewerArray_GetNextUrl(a);
		}
	}
}
