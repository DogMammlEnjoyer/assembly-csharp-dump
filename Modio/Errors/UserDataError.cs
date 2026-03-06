using System;

namespace Modio.Errors
{
	public class UserDataError : Error
	{
		public new UserDataErrorCode Code
		{
			get
			{
				return (UserDataErrorCode)this.Code;
			}
		}

		public UserDataError(UserDataErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly UserDataError None = new UserDataError(UserDataErrorCode.NONE);
	}
}
