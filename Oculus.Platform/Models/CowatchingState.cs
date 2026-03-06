using System;

namespace Oculus.Platform.Models
{
	public class CowatchingState
	{
		public CowatchingState(IntPtr o)
		{
			this.InSession = CAPI.ovr_CowatchingState_GetInSession(o);
		}

		public readonly bool InSession;
	}
}
