using System;

namespace Modio.Errors
{
	public class ApiError : Error
	{
		public new ApiErrorCode Code
		{
			get
			{
				return (ApiErrorCode)this.Code;
			}
		}

		public ApiError(ApiErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly ApiError None = new ApiError(ApiErrorCode.NONE);
	}
}
