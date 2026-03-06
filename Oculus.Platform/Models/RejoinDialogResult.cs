using System;

namespace Oculus.Platform.Models
{
	public class RejoinDialogResult
	{
		public RejoinDialogResult(IntPtr o)
		{
			this.RejoinSelected = CAPI.ovr_RejoinDialogResult_GetRejoinSelected(o);
		}

		public readonly bool RejoinSelected;
	}
}
