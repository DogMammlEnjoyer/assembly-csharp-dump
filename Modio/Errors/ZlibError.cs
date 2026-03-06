using System;

namespace Modio.Errors
{
	public class ZlibError : Error
	{
		public new ZlibErrorCode Code
		{
			get
			{
				return (ZlibErrorCode)this.Code;
			}
		}

		public ZlibError(ZlibErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly ZlibError None = new ZlibError(ZlibErrorCode.NONE);
	}
}
