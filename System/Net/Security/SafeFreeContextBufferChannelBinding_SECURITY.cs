using System;

namespace System.Net.Security
{
	internal sealed class SafeFreeContextBufferChannelBinding_SECURITY : SafeFreeContextBufferChannelBinding
	{
		protected override bool ReleaseHandle()
		{
			return Interop.SspiCli.FreeContextBuffer(this.handle) == 0;
		}
	}
}
