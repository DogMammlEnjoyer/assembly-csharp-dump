using System;
using System.Diagnostics;

namespace g3
{
	public class BlockTimer
	{
		public BlockTimer(string label, bool bStart)
		{
			this.Label = label;
			this.Watch = new Stopwatch();
			if (bStart)
			{
				this.Watch.Start();
			}
			this.Accumulated = TimeSpan.Zero;
		}

		public void Start()
		{
			this.Watch.Start();
		}

		public void Stop()
		{
			this.Watch.Stop();
		}

		public bool Running
		{
			get
			{
				return this.Watch.IsRunning;
			}
		}

		public void Accumulate(bool bReset = false)
		{
			this.Watch.Stop();
			this.Accumulated += this.Watch.Elapsed;
			if (bReset)
			{
				this.Watch.Reset();
			}
		}

		public void Reset()
		{
			this.Watch.Stop();
			this.Watch.Reset();
			this.Watch.Start();
		}

		public string AccumulatedString
		{
			get
			{
				return string.Format(BlockTimer.TimeFormatString(this.Accumulated), this.Accumulated);
			}
		}

		public override string ToString()
		{
			TimeSpan elapsed = this.Watch.Elapsed;
			return string.Format(BlockTimer.TimeFormatString(this.Accumulated), this.Watch.Elapsed);
		}

		public static string TimeFormatString(TimeSpan span)
		{
			if (span.Minutes > 0)
			{
				return "{0:mm}:{0:ss}.{0:fffffff}";
			}
			return "{0:ss}.{0:fffffff}";
		}

		public Stopwatch Watch;

		public string Label;

		public TimeSpan Accumulated;

		private const string minute_format = "{0:mm}:{0:ss}.{0:fffffff}";

		private const string second_format = "{0:ss}.{0:fffffff}";
	}
}
