using System;

namespace Modio.Errors
{
	public class TempModsError : Error
	{
		public new TempModsErrorCode Code
		{
			get
			{
				return (TempModsErrorCode)this.Code;
			}
		}

		public TempModsError(TempModsErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly TempModsError None = new TempModsError(TempModsErrorCode.NONE);
	}
}
