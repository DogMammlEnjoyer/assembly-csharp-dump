using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	internal class LagCompensationStatisticsManager
	{
		public LagCompensationStatisticsSnapshot CompletedSnapshot
		{
			get
			{
				return this._previousUpdateSnapshot;
			}
		}

		internal LagCompensationStatisticsSnapshot PendingSnapshot
		{
			get
			{
				return this._currentUpdateSnapshot;
			}
		}

		internal LagCompensationStatisticsManager()
		{
			this._previousUpdateSnapshot = new LagCompensationStatisticsSnapshot();
			this._currentUpdateSnapshot = new LagCompensationStatisticsSnapshot();
		}

		[Conditional("DEBUG")]
		internal void FinishPendingSnapshot()
		{
			this._previousUpdateSnapshot.CopyFromSnapshot(this._currentUpdateSnapshot);
			this._currentUpdateSnapshot.ClearSnapshot();
		}

		private LagCompensationStatisticsSnapshot _previousUpdateSnapshot;

		private LagCompensationStatisticsSnapshot _currentUpdateSnapshot;
	}
}
