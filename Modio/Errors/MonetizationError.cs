using System;

namespace Modio.Errors
{
	public class MonetizationError : Error
	{
		public new MonetizationErrorCode Code
		{
			get
			{
				return (MonetizationErrorCode)this.Code;
			}
		}

		public MonetizationError(MonetizationErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly MonetizationError None = new MonetizationError(MonetizationErrorCode.NONE);
	}
}
