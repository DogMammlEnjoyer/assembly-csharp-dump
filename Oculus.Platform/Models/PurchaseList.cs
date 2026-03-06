using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class PurchaseList : DeserializableList<Purchase>
	{
		public PurchaseList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_PurchaseArray_GetSize(a));
			this._Data = new List<Purchase>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new Purchase(CAPI.ovr_PurchaseArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_PurchaseArray_GetNextUrl(a);
		}
	}
}
