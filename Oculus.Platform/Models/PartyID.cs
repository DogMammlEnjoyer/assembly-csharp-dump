using System;

namespace Oculus.Platform.Models
{
	public class PartyID
	{
		public PartyID(IntPtr o)
		{
			this.ID = CAPI.ovr_PartyID_GetID(o);
		}

		public readonly ulong ID;
	}
}
