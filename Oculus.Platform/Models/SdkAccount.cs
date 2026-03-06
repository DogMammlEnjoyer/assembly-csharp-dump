using System;

namespace Oculus.Platform.Models
{
	public class SdkAccount
	{
		public SdkAccount(IntPtr o)
		{
			this.AccountType = CAPI.ovr_SdkAccount_GetAccountType(o);
			this.UserId = CAPI.ovr_SdkAccount_GetUserId(o);
		}

		public readonly SdkAccountType AccountType;

		public readonly ulong UserId;
	}
}
