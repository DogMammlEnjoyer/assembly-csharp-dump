using System;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security
{
	internal sealed class SafeSspiAuthDataHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeSspiAuthDataHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return Interop.SspiCli.SspiFreeAuthIdentity(this.handle) == Interop.SECURITY_STATUS.OK;
		}
	}
}
