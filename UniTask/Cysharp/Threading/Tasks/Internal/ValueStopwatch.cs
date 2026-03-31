using System;
using System.Diagnostics;

namespace Cysharp.Threading.Tasks.Internal
{
	internal readonly struct ValueStopwatch
	{
		public static ValueStopwatch StartNew()
		{
			return new ValueStopwatch(Stopwatch.GetTimestamp());
		}

		private ValueStopwatch(long startTimestamp)
		{
			this.startTimestamp = startTimestamp;
		}

		public TimeSpan Elapsed
		{
			get
			{
				return TimeSpan.FromTicks(this.ElapsedTicks);
			}
		}

		public bool IsInvalid
		{
			get
			{
				return this.startTimestamp == 0L;
			}
		}

		public long ElapsedTicks
		{
			get
			{
				if (this.startTimestamp == 0L)
				{
					throw new InvalidOperationException("Detected invalid initialization(use 'default'), only to create from StartNew().");
				}
				return (long)((double)(Stopwatch.GetTimestamp() - this.startTimestamp) * ValueStopwatch.TimestampToTicks);
			}
		}

		private static readonly double TimestampToTicks = 10000000.0 / (double)Stopwatch.Frequency;

		private readonly long startTimestamp;
	}
}
