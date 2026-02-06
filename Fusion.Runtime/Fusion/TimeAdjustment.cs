using System;

namespace Fusion
{
	internal struct TimeAdjustment
	{
		public TimeAdjustment(Tick tick, double total)
		{
			this.Tick = tick;
			this.Total = total;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", this.Tick, this.Total);
		}

		public Tick Tick;

		public double Total;
	}
}
