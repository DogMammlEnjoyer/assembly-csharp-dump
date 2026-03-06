using System;

namespace Modio.Errors
{
	public class UserAuthError : Error
	{
		public new UserAuthErrorCode Code
		{
			get
			{
				return (UserAuthErrorCode)this.Code;
			}
		}

		public UserAuthError(UserAuthErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly UserAuthError None = new UserAuthError(UserAuthErrorCode.NONE);
	}
}
