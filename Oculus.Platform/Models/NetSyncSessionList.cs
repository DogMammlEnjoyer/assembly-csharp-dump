using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class NetSyncSessionList : DeserializableList<NetSyncSession>
	{
		public NetSyncSessionList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_NetSyncSessionArray_GetSize(a));
			this._Data = new List<NetSyncSession>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new NetSyncSession(CAPI.ovr_NetSyncSessionArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
		}
	}
}
