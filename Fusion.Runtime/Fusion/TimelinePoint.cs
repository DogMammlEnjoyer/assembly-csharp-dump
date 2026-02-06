using System;

namespace Fusion
{
	internal struct TimelinePoint
	{
		public TimelinePoint(Tick snapshot, Tick tick, double tickDeltaDouble)
		{
			this.Snapshot = snapshot;
			this.Tick = tick;
			this.Time = (double)tick * tickDeltaDouble;
		}

		public Tick Snapshot;

		public Tick Tick;

		public double Time;
	}
}
