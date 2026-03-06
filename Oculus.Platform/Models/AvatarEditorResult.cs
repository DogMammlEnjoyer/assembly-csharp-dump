using System;

namespace Oculus.Platform.Models
{
	public class AvatarEditorResult
	{
		public AvatarEditorResult(IntPtr o)
		{
			this.RequestSent = CAPI.ovr_AvatarEditorResult_GetRequestSent(o);
		}

		public readonly bool RequestSent;
	}
}
