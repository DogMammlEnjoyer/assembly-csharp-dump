using System;

namespace Modio.Errors
{
	public class ModManagementError : Error
	{
		public new ModManagementErrorCode Code
		{
			get
			{
				return (ModManagementErrorCode)this.Code;
			}
		}

		public ModManagementError(ModManagementErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly ModManagementError None = new ModManagementError(ModManagementErrorCode.NONE);
	}
}
