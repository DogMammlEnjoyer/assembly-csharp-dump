using System;

namespace Oculus.Platform.Models
{
	public class Leaderboard
	{
		public Leaderboard(IntPtr o)
		{
			this.ApiName = CAPI.ovr_Leaderboard_GetApiName(o);
			IntPtr intPtr = CAPI.ovr_Leaderboard_GetDestination(o);
			this.Destination = new Destination(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.DestinationOptional = null;
			}
			else
			{
				this.DestinationOptional = this.Destination;
			}
			this.ID = CAPI.ovr_Leaderboard_GetID(o);
		}

		public readonly string ApiName;

		public readonly Destination DestinationOptional;

		[Obsolete("Deprecated in favor of DestinationOptional")]
		public readonly Destination Destination;

		public readonly ulong ID;
	}
}
