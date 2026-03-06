using System;

namespace Oculus.Platform.Models
{
	public class UserProof
	{
		public UserProof(IntPtr o)
		{
			this.Value = CAPI.ovr_UserProof_GetNonce(o);
		}

		public readonly string Value;
	}
}
