using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class RangeExAttribute : DrawerPropertyAttribute
	{
		public double Max { get; }

		public double Min { get; }

		public RangeExAttribute(double min, double max)
		{
			this.Max = max;
			this.Min = min;
		}

		public bool ClampMin = true;

		public bool ClampMax = true;

		public bool UseSlider = true;
	}
}
