using System;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
	internal sealed class RealtimePlayerLoopTimer : PlayerLoopTimer
	{
		public RealtimePlayerLoopTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state) : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
		{
			this.ResetCore(new TimeSpan?(interval));
		}

		protected override bool MoveNextCore()
		{
			return this.stopwatch.ElapsedTicks < this.intervalTicks;
		}

		protected override void ResetCore(TimeSpan? interval)
		{
			this.stopwatch = ValueStopwatch.StartNew();
			if (interval != null)
			{
				this.intervalTicks = interval.Value.Ticks;
			}
		}

		private ValueStopwatch stopwatch;

		private long intervalTicks;
	}
}
