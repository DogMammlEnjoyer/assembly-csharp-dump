using System;

namespace Oculus.Platform.Models
{
	public class OrgScopedID
	{
		public OrgScopedID(IntPtr o)
		{
			this.ID = CAPI.ovr_OrgScopedID_GetID(o);
		}

		public readonly ulong ID;
	}
}
