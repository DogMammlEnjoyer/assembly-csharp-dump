using System;

namespace Modio.Errors
{
	public class FilesystemError : Error
	{
		public new FilesystemErrorCode Code
		{
			get
			{
				return (FilesystemErrorCode)this.Code;
			}
		}

		public FilesystemError(FilesystemErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly FilesystemError None = new FilesystemError(FilesystemErrorCode.NONE);
	}
}
