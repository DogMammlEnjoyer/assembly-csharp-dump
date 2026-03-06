using System;

namespace Oculus.Platform.Models
{
	public class UserDataStoreUpdateResponse
	{
		public UserDataStoreUpdateResponse(IntPtr o)
		{
			this.Success = CAPI.ovr_UserDataStoreUpdateResponse_GetSuccess(o);
		}

		public readonly bool Success;
	}
}
