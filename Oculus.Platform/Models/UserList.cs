using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class UserList : DeserializableList<User>
	{
		public UserList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_UserArray_GetSize(a));
			this._Data = new List<User>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new User(CAPI.ovr_UserArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_UserArray_GetNextUrl(a);
		}
	}
}
