using System;

namespace Modio.Errors
{
	public class GenericError : Error
	{
		public new GenericErrorCode Code
		{
			get
			{
				return (GenericErrorCode)this.Code;
			}
		}

		public GenericError(GenericErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly GenericError None = new GenericError(GenericErrorCode.NONE);
	}
}
