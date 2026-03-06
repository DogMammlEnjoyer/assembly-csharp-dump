using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	[Serializable]
	public struct ScheduledDuration
	{
		public ScheduledDuration(long startTimeTicks, long endTimeTicks)
		{
			this._startTimeTicks = startTimeTicks;
			this._endTimeTicks = endTimeTicks;
		}

		public bool IsActive()
		{
			long ticks = DateTime.Now.Ticks;
			return ticks >= this._startTimeTicks && ticks <= this._endTimeTicks;
		}

		[SerializeField]
		private long _startTimeTicks;

		[SerializeField]
		private long _endTimeTicks;
	}
}
