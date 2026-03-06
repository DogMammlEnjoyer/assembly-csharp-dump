using System;
using System.Collections.Generic;
using Backtrace.Unity.Common;
using Backtrace.Unity.Model;
using UnityEngine;

namespace Backtrace.Unity.Services
{
	public class ReportLimitWatcher
	{
		internal ReportLimitWatcher(uint reportPerMin)
		{
			if (reportPerMin < 0U)
			{
				throw new ArgumentException("reportPerMin have to be greater than or equal to zero");
			}
			int num = checked((int)reportPerMin);
			this._reportQueue = new Queue<long>(num);
			this._reportPerMin = num;
			this._watcherEnable = (reportPerMin > 0U);
		}

		internal void SetClientReportLimit(uint reportPerMin)
		{
			int reportPerMin2 = checked((int)reportPerMin);
			this._reportPerMin = reportPerMin2;
			this._watcherEnable = (reportPerMin > 0U);
		}

		public bool WatchReport(long timestamp, bool displayMessageOnLimitHit = true)
		{
			if (!this._watcherEnable)
			{
				return true;
			}
			object @object = this._object;
			lock (@object)
			{
				this.Clear();
				if (this._reportQueue.Count + 1 > this._reportPerMin)
				{
					this._limitHit = true;
					if (displayMessageOnLimitHit)
					{
						this.DisplayReportLimitHitMessage();
					}
					return false;
				}
				this._limitHit = false;
				this._displayMessage = true;
				this._reportQueue.Enqueue(timestamp);
			}
			return true;
		}

		public bool WatchReport(BacktraceReport report, bool displayMessageOnLimitHit = true)
		{
			return this.WatchReport(report.Timestamp, displayMessageOnLimitHit);
		}

		internal bool ShouldDisplayMessage()
		{
			return this._limitHit && this._displayMessage;
		}

		private void DisplayReportLimitHitMessage()
		{
			if (this.ShouldDisplayMessage())
			{
				this._displayMessage = false;
				Debug.LogWarning(string.Format("Backtrace report limit hit({0}/min) – Ignoring errors for 1 minute", this._reportPerMin));
			}
		}

		private void Clear()
		{
			long num = (long)DateTimeHelper.Timestamp();
			bool flag = false;
			while (!flag && this._reportQueue.Count != 0)
			{
				long num2 = this._reportQueue.Peek();
				flag = (num - num2 < this._queueReportTime);
				if (!flag)
				{
					this._reportQueue.Dequeue();
				}
			}
		}

		internal void Reset()
		{
			this._reportQueue.Clear();
		}

		internal readonly Queue<long> _reportQueue;

		internal readonly object _object = new object();

		private readonly long _queueReportTime = 60L;

		private bool _watcherEnable;

		private int _reportPerMin;

		private bool _displayMessage;

		private bool _limitHit;
	}
}
