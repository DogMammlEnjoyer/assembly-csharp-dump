using System;
using System.Threading.Tasks;

namespace Modio.Customizations
{
	public struct ExternalAuthenticationToken
	{
		public void Cancel()
		{
			Action cancel = this.cancel;
			if (cancel != null)
			{
				cancel();
			}
			this.cancel = null;
		}

		internal Action cancel { readonly get; set; }

		public string url;

		public string autoUrl;

		public string code;

		public Task<ValueTuple<Error, WssLoginSuccess>> task;

		public DateTime expiryTime;
	}
}
