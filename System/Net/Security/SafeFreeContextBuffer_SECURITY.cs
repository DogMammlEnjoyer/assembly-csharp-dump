using System;

namespace System.Net.Security
{
	internal sealed class SafeFreeContextBuffer_SECURITY : SafeFreeContextBuffer
	{
		internal SafeFreeContextBuffer_SECURITY()
		{
		}

		protected override bool ReleaseHandle()
		{
			return Interop.SspiCli.FreeContextBuffer(this.handle) == 0;
		}
	}
}
