using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class FusionStatisticsManager
	{
		public FusionStatisticsSnapshot CompleteSnapshot
		{
			get
			{
				return this._previousTickSnapshot;
			}
		}

		internal FusionStatisticsSnapshot PendingSnapshot
		{
			get
			{
				return this._currentTickSnapshot;
			}
		}

		public NetworkObjectStatisticsManager ObjectStatisticsManager
		{
			get
			{
				return this._objectStatisticsManager;
			}
		}

		internal FusionStatisticsManager()
		{
			this._currentTickSnapshot = new FusionStatisticsSnapshot();
			this._previousTickSnapshot = new FusionStatisticsSnapshot();
			this._objectStatisticsManager = new NetworkObjectStatisticsManager();
		}

		[Conditional("DEBUG")]
		internal void FinishPendingSnapshot()
		{
			this._previousTickSnapshot.CopyFrom(this._currentTickSnapshot);
			this._currentTickSnapshot.ClearSnapshot();
			this._objectStatisticsManager.CollectStatistics();
		}

		private FusionStatisticsSnapshot _currentTickSnapshot;

		private FusionStatisticsSnapshot _previousTickSnapshot;

		private NetworkObjectStatisticsManager _objectStatisticsManager;
	}
}
