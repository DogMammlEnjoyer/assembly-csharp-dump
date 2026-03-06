using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class InstalledApplicationList : DeserializableList<InstalledApplication>
	{
		public InstalledApplicationList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_InstalledApplicationArray_GetSize(a));
			this._Data = new List<InstalledApplication>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new InstalledApplication(CAPI.ovr_InstalledApplicationArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
		}
	}
}
