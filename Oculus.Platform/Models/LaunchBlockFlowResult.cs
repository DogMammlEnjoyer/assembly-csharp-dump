using System;

namespace Oculus.Platform.Models
{
	public class LaunchBlockFlowResult
	{
		public LaunchBlockFlowResult(IntPtr o)
		{
			this.DidBlock = CAPI.ovr_LaunchBlockFlowResult_GetDidBlock(o);
			this.DidCancel = CAPI.ovr_LaunchBlockFlowResult_GetDidCancel(o);
		}

		public readonly bool DidBlock;

		public readonly bool DidCancel;
	}
}
