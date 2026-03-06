using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionStatsGraphDefault : FusionStatsGraphBase
	{
		internal RenderSimStats Stat
		{
			get
			{
				return this._selectedStats;
			}
		}

		protected override void Initialize(int accumulateTimeMs)
		{
			base.Initialize(accumulateTimeMs);
			this._descriptionText.text = this._selectedStats.ToString();
			string str;
			if (this._statsAdditionalInfo.TryGetValue(this.Stat, out str))
			{
				Text descriptionText = this._descriptionText;
				descriptionText.text = descriptionText.text + " " + str;
			}
		}

		public override void UpdateGraph(NetworkRunner runner, FusionStatisticsManager statisticsManager, ref DateTime now)
		{
			float statDataFromSnapshot = FusionStatisticsHelper.GetStatDataFromSnapshot(this._selectedStats, statisticsManager.CompleteSnapshot);
			this.AddValueToBuffer(statDataFromSnapshot, ref now);
		}

		public virtual void ApplyCustomStatsConfig(FusionStatistics.FusionStatisticsStatCustomConfig config)
		{
			base.SetThresholds(config.Threshold1, config.Threshold2, config.Threshold3);
			base.SetIgnoreZeroValues(config.IgnoreZeroOnAverageCalculation, config.IgnoreZeroOnBuffer);
			base.SetAccumulateTime(config.AccumulateTimeMs);
		}

		internal void SetupDefaultGraph(RenderSimStats stat)
		{
			this._selectedStats = stat;
			string valueTextFormat;
			float valueTextMultiplier;
			bool ignoreZeroOnAverage;
			bool ignoreZeroOnBuffer;
			int accumulateTimeMs;
			FusionStatisticsHelper.GetStatGraphDefaultSettings(this._selectedStats, out valueTextFormat, out valueTextMultiplier, out ignoreZeroOnAverage, out ignoreZeroOnBuffer, out accumulateTimeMs);
			base.SetValueTextFormat(valueTextFormat);
			base.SetValueTextMultiplier(valueTextMultiplier);
			base.SetIgnoreZeroValues(ignoreZeroOnAverage, ignoreZeroOnBuffer);
			this.Initialize(accumulateTimeMs);
		}

		private RenderSimStats _selectedStats;

		[SerializeField]
		private Text _descriptionText;

		private Dictionary<RenderSimStats, string> _statsAdditionalInfo = new Dictionary<RenderSimStats, string>
		{
			{
				RenderSimStats.InPackets,
				"(Per second)"
			},
			{
				RenderSimStats.OutPackets,
				"(Per second)"
			},
			{
				RenderSimStats.InObjectUpdates,
				"(Per second)"
			},
			{
				RenderSimStats.OutObjectUpdates,
				"(Per second)"
			},
			{
				RenderSimStats.InBandwidth,
				"(Per second)"
			},
			{
				RenderSimStats.OutBandwidth,
				"(Per second)"
			},
			{
				RenderSimStats.InputInBandwidth,
				"(Per second)"
			},
			{
				RenderSimStats.InputOutBandwidth,
				"(Per second)"
			},
			{
				RenderSimStats.StateReceiveDelta,
				"(Per second)"
			},
			{
				RenderSimStats.WordsWrittenSize,
				"(Per second)"
			},
			{
				RenderSimStats.WordsWrittenCount,
				"(Per second)"
			},
			{
				RenderSimStats.WordsReadCount,
				"(Per second)"
			},
			{
				RenderSimStats.WordsReadSize,
				"(Per second)"
			}
		};
	}
}
