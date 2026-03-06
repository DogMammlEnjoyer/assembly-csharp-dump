using System;
using System.Collections.Generic;
using System.Text;

namespace g3
{
	public class LocalProfiler : IDisposable
	{
		public BlockTimer Start(string label)
		{
			if (this.Timers.ContainsKey(label))
			{
				this.Timers[label].Reset();
			}
			else
			{
				this.Timers[label] = new BlockTimer(label, true);
				this.Order.Add(label);
			}
			return this.Timers[label];
		}

		public BlockTimer StopAllAndStartNew(string label)
		{
			this.StopAll();
			return this.Start(label);
		}

		public BlockTimer Get(string label)
		{
			return this.Timers[label];
		}

		public void Stop(string label)
		{
			this.Timers[label].Stop();
		}

		public void StopAll()
		{
			foreach (BlockTimer blockTimer in this.Timers.Values)
			{
				if (blockTimer.Running)
				{
					blockTimer.Stop();
				}
			}
		}

		public void StopAndAccumulate(string label, bool bReset = false)
		{
			this.Timers[label].Accumulate(bReset);
		}

		public void Reset(string label)
		{
			this.Timers[label].Reset();
		}

		public void ResetAccumulated(string label)
		{
			this.Timers[label].Accumulated = TimeSpan.Zero;
		}

		public void ResetAllAccumulated(string label)
		{
			foreach (BlockTimer blockTimer in this.Timers.Values)
			{
				blockTimer.Accumulated = TimeSpan.Zero;
			}
		}

		public void DivideAllAccumulated(int div)
		{
			foreach (BlockTimer blockTimer in this.Timers.Values)
			{
				blockTimer.Accumulated = new TimeSpan(blockTimer.Accumulated.Ticks / (long)div);
			}
		}

		public string Elapsed(string label)
		{
			return this.Timers[label].ToString();
		}

		public string Accumulated(string label)
		{
			TimeSpan accumulated = this.Timers[label].Accumulated;
			return string.Format(BlockTimer.TimeFormatString(accumulated), accumulated);
		}

		public string AllTicks(string prefix = "Times:")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix + " ");
			foreach (string text in this.Order)
			{
				stringBuilder.Append(text + ": " + this.Timers[text].ToString() + " ");
			}
			return stringBuilder.ToString();
		}

		public string AllAccumulatedTicks(string prefix = "Times:")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix + " ");
			foreach (string text in this.Order)
			{
				stringBuilder.Append(text + ": " + this.Accumulated(text) + " ");
			}
			return stringBuilder.ToString();
		}

		public string AllTimes(string prefix = "Times:", string separator = " ")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix + " ");
			foreach (string text in this.Order)
			{
				TimeSpan elapsed = this.Timers[text].Watch.Elapsed;
				stringBuilder.Append(text + ": " + string.Format(BlockTimer.TimeFormatString(elapsed), elapsed) + separator);
			}
			return stringBuilder.ToString();
		}

		public string AllAccumulatedTimes(string prefix = "Times:", string separator = " ")
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix + " ");
			foreach (string text in this.Order)
			{
				TimeSpan accumulated = this.Timers[text].Accumulated;
				stringBuilder.Append(text + ": " + string.Format(BlockTimer.TimeFormatString(accumulated), accumulated) + separator);
			}
			return stringBuilder.ToString();
		}

		public void Dispose()
		{
			foreach (BlockTimer blockTimer in this.Timers.Values)
			{
				blockTimer.Stop();
			}
			this.Timers.Clear();
		}

		private Dictionary<string, BlockTimer> Timers = new Dictionary<string, BlockTimer>();

		private List<string> Order = new List<string>();
	}
}
