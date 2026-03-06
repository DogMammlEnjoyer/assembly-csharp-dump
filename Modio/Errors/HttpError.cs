using System;

namespace Modio.Errors
{
	public class HttpError : Error
	{
		public new HttpErrorCode Code
		{
			get
			{
				return (HttpErrorCode)this.Code;
			}
		}

		public HttpError(HttpErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly HttpError None = new HttpError(HttpErrorCode.NONE);
	}
}
