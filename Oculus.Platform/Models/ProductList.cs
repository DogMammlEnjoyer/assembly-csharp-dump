using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class ProductList : DeserializableList<Product>
	{
		public ProductList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_ProductArray_GetSize(a));
			this._Data = new List<Product>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new Product(CAPI.ovr_ProductArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_ProductArray_GetNextUrl(a);
		}
	}
}
