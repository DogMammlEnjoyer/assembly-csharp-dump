using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class BehaviourStatisticsSnapshot
	{
		public int FixedUpdateNetworkExecutionCount { get; private set; }

		public int RenderExecutionCount { get; private set; }

		public double FixedUpdateNetworkExecutionTime { get; private set; }

		public double RenderExecutionTime { get; private set; }

		[Conditional("DEBUG")]
		internal void ClearSnapshot()
		{
			this.FixedUpdateNetworkExecutionCount = 0;
			this.FixedUpdateNetworkExecutionTime = 0.0;
			this.RenderExecutionCount = 0;
			this.RenderExecutionTime = 0.0;
		}

		[Conditional("DEBUG")]
		internal void CopyFromSnapshot(BehaviourStatisticsSnapshot snapshot)
		{
			this.FixedUpdateNetworkExecutionCount = snapshot.FixedUpdateNetworkExecutionCount;
			this.RenderExecutionCount = snapshot.RenderExecutionCount;
			this.FixedUpdateNetworkExecutionTime = snapshot.FixedUpdateNetworkExecutionTime;
			this.RenderExecutionTime = snapshot.RenderExecutionTime;
		}

		[Conditional("DEBUG")]
		internal void AccumulateFixedUpdateNetworkExecutionCount(int count)
		{
			this.FixedUpdateNetworkExecutionCount += count;
		}

		[Conditional("DEBUG")]
		internal void AccumulateRenderExecutionCount(int count)
		{
			this.RenderExecutionCount += count;
		}

		[Conditional("DEBUG")]
		internal void AccumulateFixedUpdateNetworkExecutionTime(double time)
		{
			this.FixedUpdateNetworkExecutionTime += time;
		}

		[Conditional("DEBUG")]
		internal void AccumulateRenderExecutionTime(double time)
		{
			this.RenderExecutionTime += time;
		}
	}
}
