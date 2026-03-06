using System;

namespace Oculus.Platform.Models
{
	public class SupplementaryMetric
	{
		public SupplementaryMetric(IntPtr o)
		{
			this.ID = CAPI.ovr_SupplementaryMetric_GetID(o);
			this.Metric = CAPI.ovr_SupplementaryMetric_GetMetric(o);
		}

		public readonly ulong ID;

		public readonly long Metric;
	}
}
