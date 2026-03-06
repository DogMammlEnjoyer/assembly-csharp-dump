using System;

namespace Oculus.Platform.Models
{
	public class PlatformInitialize
	{
		public PlatformInitialize(IntPtr o)
		{
			this.Result = CAPI.ovr_PlatformInitialize_GetResult(o);
		}

		public readonly PlatformInitializeResult Result;
	}
}
