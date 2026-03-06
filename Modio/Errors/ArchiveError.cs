using System;

namespace Modio.Errors
{
	public class ArchiveError : Error
	{
		public new ArchiveErrorCode Code
		{
			get
			{
				return (ArchiveErrorCode)this.Code;
			}
		}

		public ArchiveError(ArchiveErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly ArchiveError None = new ArchiveError(ArchiveErrorCode.NONE);
	}
}
