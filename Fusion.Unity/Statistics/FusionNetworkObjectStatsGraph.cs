using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionNetworkObjectStatsGraph : FusionStatsGraphBase
	{
		public override void UpdateGraph(NetworkRunner runner, FusionStatisticsManager statisticsManager, ref DateTime now)
		{
			this.AddValueToBuffer(this.GetNetworkObjectStatValue(statisticsManager), ref now);
		}

		private float GetNetworkObjectStatValue(FusionStatisticsManager statisticsManager)
		{
			NetworkObjectStatisticsSnapshot networkObjectStatisticsSnapshot;
			if (statisticsManager.ObjectStatisticsManager.GetNetworkObjectStatistics(this._id, out networkObjectStatisticsSnapshot))
			{
				NetworkObjectStat stat = this._stat;
				if (stat <= NetworkObjectStat.OutPackets)
				{
					switch (stat)
					{
					case NetworkObjectStat.InBandwidth:
						return networkObjectStatisticsSnapshot.InBandwidth;
					case NetworkObjectStat.OutBandwidth:
						return networkObjectStatisticsSnapshot.OutBandwidth;
					case NetworkObjectStat.InBandwidth | NetworkObjectStat.OutBandwidth:
						break;
					case NetworkObjectStat.InPackets:
						return (float)networkObjectStatisticsSnapshot.InPackets;
					default:
						if (stat == NetworkObjectStat.OutPackets)
						{
							return (float)networkObjectStatisticsSnapshot.OutPackets;
						}
						break;
					}
				}
				else
				{
					if (stat == NetworkObjectStat.AverageInPacketSize)
					{
						return networkObjectStatisticsSnapshot.InBandwidth / (float)Mathf.Max(1, networkObjectStatisticsSnapshot.InPackets);
					}
					if (stat == NetworkObjectStat.AverageOutPacketSize)
					{
						return networkObjectStatisticsSnapshot.OutBandwidth / (float)Mathf.Max(1, networkObjectStatisticsSnapshot.OutPackets);
					}
				}
			}
			return -1f;
		}

		internal void SetupNetworkObjectStat(NetworkId id, NetworkObjectStat stat)
		{
			this._id = id;
			this._stat = stat;
			this._description.text = this._stat.ToString();
			float threshold = 0f;
			float threshold2 = 0f;
			float threshold3 = 0f;
			float valueTextMultiplier = 1f;
			bool ignoreZeroOnAverage = false;
			bool ignoreZeroOnBuffer = false;
			int accumulateTimeMs = 0;
			string valueTextFormat;
			if (stat <= NetworkObjectStat.InPackets)
			{
				if (stat - NetworkObjectStat.InBandwidth <= 1)
				{
					valueTextFormat = "{0:0} B";
					accumulateTimeMs = 1000;
					Text description = this._description;
					description.text += " (Per second)";
					goto IL_D4;
				}
				if (stat != NetworkObjectStat.InPackets)
				{
					goto IL_CE;
				}
			}
			else if (stat != NetworkObjectStat.OutPackets)
			{
				if (stat != NetworkObjectStat.AverageInPacketSize && stat != NetworkObjectStat.AverageOutPacketSize)
				{
					goto IL_CE;
				}
				valueTextFormat = "{0:0} B";
				ignoreZeroOnAverage = true;
				ignoreZeroOnBuffer = true;
				goto IL_D4;
			}
			valueTextFormat = "{0:0}";
			accumulateTimeMs = 1000;
			Text description2 = this._description;
			description2.text += " (Per second)";
			goto IL_D4;
			IL_CE:
			valueTextFormat = "{0:0}";
			IL_D4:
			base.SetValueTextFormat(valueTextFormat);
			base.SetValueTextMultiplier(valueTextMultiplier);
			base.SetThresholds(threshold, threshold2, threshold3);
			base.SetIgnoreZeroValues(ignoreZeroOnAverage, ignoreZeroOnBuffer);
			this.Initialize(accumulateTimeMs);
		}

		[SerializeField]
		private Text _description;

		private NetworkId _id;

		private NetworkObjectStat _stat;

		private FusionNetworkObjectStatsGraphCombine _combineParentGraph;
	}
}
