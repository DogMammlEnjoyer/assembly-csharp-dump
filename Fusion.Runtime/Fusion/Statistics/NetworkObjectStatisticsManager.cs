using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class NetworkObjectStatisticsManager
	{
		internal NetworkObjectStatisticsManager()
		{
			this._monitoredNetworkObjects = new HashSet<NetworkId>();
			this._completedSnapshots = new Dictionary<NetworkId, NetworkObjectStatisticsSnapshot>();
			this._pendingSnapshots = new Dictionary<NetworkId, NetworkObjectStatisticsSnapshot>();
			this._free = new Stack<NetworkObjectStatisticsSnapshot>();
		}

		private NetworkObjectStatisticsSnapshot GetNewStatisticsObject()
		{
			return (this._free.Count > 0) ? this._free.Pop() : new NetworkObjectStatisticsSnapshot();
		}

		public void MonitorNetworkObjectStatistics(NetworkId id, bool monitor)
		{
			if (monitor)
			{
				this._monitoredNetworkObjects.Add(id);
			}
			else
			{
				this._monitoredNetworkObjects.Remove(id);
			}
		}

		public void ClearMonitoredNetworkObjects()
		{
			this._monitoredNetworkObjects.Clear();
		}

		private bool IsObjectMonitored(NetworkId id, Dictionary<NetworkId, NetworkObjectStatisticsSnapshot> source, out NetworkObjectStatisticsSnapshot snapshot)
		{
			snapshot = null;
			bool flag = !this._monitoredNetworkObjects.Contains(id);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !source.TryGetValue(id, out snapshot);
				if (flag2)
				{
					snapshot = this.GetNewStatisticsObject();
					snapshot.Reset();
					source.Add(id, snapshot);
				}
				result = true;
			}
			return result;
		}

		public bool GetNetworkObjectStatistics(NetworkId id, out NetworkObjectStatisticsSnapshot objectStatisticsSnapshot)
		{
			bool flag = this.IsObjectMonitored(id, this._completedSnapshots, out objectStatisticsSnapshot);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				objectStatisticsSnapshot = null;
				result = false;
			}
			return result;
		}

		[Conditional("DEBUG")]
		internal void AddToNetworkObjectInBandwidth(NetworkId id, float value, bool overrideValue = false)
		{
			NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot;
			bool flag = this.IsObjectMonitored(id, this._pendingSnapshots, out networkObjectStatisticsSnapshot);
			if (flag)
			{
				networkObjectStatisticsSnapshot.AddToInBandwidthStat(value, overrideValue);
			}
		}

		[Conditional("DEBUG")]
		internal void AddToNetworkObjectOutBandwidth(NetworkId id, float value, bool overrideValue = false)
		{
			NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot;
			bool flag = this.IsObjectMonitored(id, this._pendingSnapshots, out networkObjectStatisticsSnapshot);
			if (flag)
			{
				networkObjectStatisticsSnapshot.AddToOutBandwidthStat(value, overrideValue);
			}
		}

		[Conditional("DEBUG")]
		internal void AddToNetworkObjectInPackets(NetworkId id, int value, bool overrideValue = false)
		{
			NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot;
			bool flag = this.IsObjectMonitored(id, this._pendingSnapshots, out networkObjectStatisticsSnapshot);
			if (flag)
			{
				networkObjectStatisticsSnapshot.AddToInPacketsStat(value, overrideValue);
			}
		}

		[Conditional("DEBUG")]
		internal void AddToNetworkObjectOutPackets(NetworkId id, int value, bool overrideValue = false)
		{
			NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot;
			bool flag = this.IsObjectMonitored(id, this._pendingSnapshots, out networkObjectStatisticsSnapshot);
			if (flag)
			{
				networkObjectStatisticsSnapshot.AddToOutPacketsStat(value, overrideValue);
			}
		}

		[Conditional("DEBUG")]
		internal void CollectStatistics()
		{
			foreach (NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot in this._completedSnapshots.Values)
			{
				networkObjectStatisticsSnapshot.Reset();
				this._free.Push(networkObjectStatisticsSnapshot);
			}
			this._completedSnapshots.Clear();
			foreach (KeyValuePair<NetworkId, NetworkObjectStatisticsSnapshot> keyValuePair in this._pendingSnapshots)
			{
				this._completedSnapshots.Add(keyValuePair.Key, keyValuePair.Value);
			}
			this._pendingSnapshots.Clear();
		}

		private HashSet<NetworkId> _monitoredNetworkObjects;

		private Dictionary<NetworkId, NetworkObjectStatisticsSnapshot> _pendingSnapshots;

		private Dictionary<NetworkId, NetworkObjectStatisticsSnapshot> _completedSnapshots;

		private Stack<NetworkObjectStatisticsSnapshot> _free;
	}
}
