using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class BehaviourStatisticsManager
	{
		public BehaviourStatisticsSnapshot CompletedSnapshot
		{
			get
			{
				return this._previousUpdateSnapshot;
			}
		}

		internal BehaviourStatisticsSnapshot PendingSnapshot
		{
			get
			{
				return this._currentUpdateSnapshot;
			}
		}

		internal BehaviourStatisticsManager()
		{
			this._previousUpdateSnapshot = new BehaviourStatisticsSnapshot();
			this._currentUpdateSnapshot = new BehaviourStatisticsSnapshot();
		}

		[Conditional("DEBUG")]
		internal void FinishPendingSnapshot()
		{
			this._previousUpdateSnapshot.CopyFromSnapshot(this._currentUpdateSnapshot);
			this._currentUpdateSnapshot.ClearSnapshot();
		}

		private BehaviourStatisticsSnapshot _previousUpdateSnapshot;

		private BehaviourStatisticsSnapshot _currentUpdateSnapshot;
	}
}
