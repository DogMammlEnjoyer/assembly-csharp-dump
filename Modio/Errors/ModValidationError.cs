using System;

namespace Modio.Errors
{
	public class ModValidationError : Error
	{
		public new ModValidationErrorCode Code
		{
			get
			{
				return (ModValidationErrorCode)this.Code;
			}
		}

		public ModValidationError(ModValidationErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly ModValidationError None = new ModValidationError(ModValidationErrorCode.NONE);
	}
}
