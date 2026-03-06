using System;

namespace Oculus.Platform.Models
{
	public class LinkedAccount
	{
		public LinkedAccount(IntPtr o)
		{
			this.AccessToken = CAPI.ovr_LinkedAccount_GetAccessToken(o);
			this.ServiceProvider = CAPI.ovr_LinkedAccount_GetServiceProvider(o);
			this.UserId = CAPI.ovr_LinkedAccount_GetUserId(o);
		}

		public readonly string AccessToken;

		public readonly ServiceProvider ServiceProvider;

		public readonly string UserId;
	}
}
