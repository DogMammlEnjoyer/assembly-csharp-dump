using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionStatsPanelHeader : MonoBehaviour
	{
		public event Action OnRenderStatsUpdate;

		public void SetupHeader(string title, FusionStatistics fusionStatistics)
		{
			this._statsHeaderTitle.text = title;
			this._fusionStatistics = fusionStatistics;
			this.SetupDropdown();
		}

		private void SetupDropdown()
		{
			this._defaultStatsGraph = new Dictionary<RenderSimStats, FusionStatsGraphDefault>();
			List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
			list.Add(new Dropdown.OptionData("Toggle Stats"));
			foreach (string text in Enum.GetNames(typeof(RenderSimStats)))
			{
				list.Add(new Dropdown.OptionData(text));
			}
			this._statsDropdown.options = list;
			this._statsDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropDownChanged));
		}

		internal void SetStatsToRender(RenderSimStats stats)
		{
			if (stats == this._statsToRender)
			{
				return;
			}
			foreach (object obj in Enum.GetValues(typeof(RenderSimStats)))
			{
				RenderSimStats renderSimStats = (RenderSimStats)obj;
				if ((stats & renderSimStats) == renderSimStats)
				{
					if ((this._statsToRender & renderSimStats) != renderSimStats)
					{
						this.AddStat(renderSimStats);
					}
				}
				else if ((this._statsToRender & renderSimStats) == renderSimStats)
				{
					this.RemoveStat(renderSimStats);
				}
			}
			this._statsToRender = stats;
		}

		private void AddStat(RenderSimStats stat)
		{
			this._statsToRender |= stat;
			this.InstantiateStatGraph(stat);
			this.InvokeRenderStatsUpdate();
		}

		private void RemoveStat(RenderSimStats stat)
		{
			this._statsToRender &= ~stat;
			this.DestroyStatGraph(stat);
			this.InvokeRenderStatsUpdate();
		}

		private void InvokeRenderStatsUpdate()
		{
			Action onRenderStatsUpdate = this.OnRenderStatsUpdate;
			if (onRenderStatsUpdate == null)
			{
				return;
			}
			onRenderStatsUpdate();
		}

		private void OnDropDownChanged(int arg0)
		{
			if (arg0 <= 0)
			{
				return;
			}
			arg0--;
			RenderSimStats renderSimStats = (RenderSimStats)(1 << arg0);
			if ((this._statsToRender & renderSimStats) == renderSimStats)
			{
				this.RemoveStat(renderSimStats);
			}
			else
			{
				this.AddStat(renderSimStats);
			}
			this._statsDropdown.SetValueWithoutNotify(0);
			this._fusionStatistics.UpdateStatsEnabled(this._statsToRender);
		}

		private void InstantiateStatGraph(RenderSimStats stat)
		{
			FusionStatsGraphDefault fusionStatsGraphDefault = Object.Instantiate<FusionStatsGraphDefault>(this._defaultGraphPrefab, this.ContentRect);
			fusionStatsGraphDefault.SetupDefaultGraph(stat);
			this.TryApplyCustomStatConfig(fusionStatsGraphDefault);
			this._defaultStatsGraph.Add(stat, fusionStatsGraphDefault);
		}

		private void DestroyStatGraph(RenderSimStats stat)
		{
			FusionStatsGraphDefault fusionStatsGraphDefault;
			if (this._defaultStatsGraph.Remove(stat, out fusionStatsGraphDefault))
			{
				Object.Destroy(fusionStatsGraphDefault.gameObject);
			}
		}

		private void TryApplyCustomStatConfig(FusionStatsGraphDefault graph)
		{
			foreach (FusionStatistics.FusionStatisticsStatCustomConfig fusionStatisticsStatCustomConfig in this._fusionStatistics.StatsCustomConfig)
			{
				if (fusionStatisticsStatCustomConfig.Stat == graph.Stat)
				{
					this.ApplyCustomStatsConfig(graph, fusionStatisticsStatCustomConfig);
				}
			}
		}

		private void ApplyCustomStatsConfig(FusionStatsGraphDefault graph, FusionStatistics.FusionStatisticsStatCustomConfig config)
		{
			graph.ApplyCustomStatsConfig(config);
		}

		internal void ApplyStatsConfig(List<FusionStatistics.FusionStatisticsStatCustomConfig> statsConfig)
		{
			foreach (FusionStatistics.FusionStatisticsStatCustomConfig fusionStatisticsStatCustomConfig in statsConfig)
			{
				FusionStatsGraphDefault graph;
				if (this._defaultStatsGraph.TryGetValue(fusionStatisticsStatCustomConfig.Stat, out graph))
				{
					this.ApplyCustomStatsConfig(graph, fusionStatisticsStatCustomConfig);
				}
			}
		}

		[SerializeField]
		private Text _statsHeaderTitle;

		[SerializeField]
		private Dropdown _statsDropdown;

		[SerializeField]
		private FusionStatsGraphDefault _defaultGraphPrefab;

		public RectTransform ContentRect;

		private Dictionary<RenderSimStats, FusionStatsGraphDefault> _defaultStatsGraph;

		private FusionStatistics _fusionStatistics;

		private RenderSimStats _statsToRender;
	}
}
