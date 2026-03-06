using System;

namespace Modio.Errors
{
	public class SystemError : Error
	{
		public new SystemErrorCode Code
		{
			get
			{
				return (SystemErrorCode)this.Code;
			}
		}

		public SystemError(SystemErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly SystemError None = new SystemError(SystemErrorCode.NONE);
	}
}
