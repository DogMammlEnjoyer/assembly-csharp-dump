using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Fusion.Statistics
{
	[RequireComponent(typeof(NetworkRunner))]
	[AddComponentMenu("Fusion/Statistics/Fusion Statistics")]
	public class FusionStatistics : SimulationBehaviour, ISpawned, IPublicFacingInterface
	{
		internal List<FusionStatsGraphBase> ActiveGraphs
		{
			get
			{
				return this._statsGraph;
			}
		}

		internal List<FusionStatistics.FusionStatisticsStatCustomConfig> StatsCustomConfig
		{
			get
			{
				return this._statsCustomConfig;
			}
		}

		public bool IsPanelActive
		{
			get
			{
				return this._statsPanelObject;
			}
		}

		private void Awake()
		{
			this._statsGraph = new List<FusionStatsGraphBase>();
			this._statsCanvasPrefab = Resources.Load<GameObject>("FusionStatsResources/FusionStatsRenderPanel");
			this._objectGraphCombinePrefab = Resources.Load<FusionNetworkObjectStatsGraphCombine>("FusionStatsResources/NetworkObjectStatistics");
			if (this._statsCanvasPrefab == null || this._objectGraphCombinePrefab == null)
			{
				Log.Error("Error loading the required assets for Fusion Statistics, destroying stats instance. Make sure that the following paths are valid for the Fusion Statistics resource assets: \n 1. FusionStatsResources/FusionStatsRenderPanel \n 2. FusionStatsResources/NetworkObjectStatistics");
				UnityEngine.Object.Destroy(this);
			}
		}

		void ISpawned.Spawned()
		{
			this.SetupStatisticsPanel();
		}

		public void SetStatsCustomConfig(List<FusionStatistics.FusionStatisticsStatCustomConfig> customConfig)
		{
			if (customConfig == null)
			{
				Log.Warn("Trying to set a null Fusion Statistics custom stats config");
				return;
			}
			this._statsCustomConfig = customConfig;
			this.ApplyCustomConfig();
		}

		public void SetCanvasAnchor(CanvasAnchor anchor)
		{
			this._canvasAnchor = anchor;
			if (!this._statsCanvas)
			{
				return;
			}
			this._statsCanvas.SetCanvasAnchor(anchor);
		}

		private void ApplyCustomConfig()
		{
			if (!this._header)
			{
				return;
			}
			this._header.ApplyStatsConfig(this._statsCustomConfig);
		}

		public void OnEditorChange()
		{
			this.RenderEnabledStats();
			this.ApplyCustomConfig();
			this.SetCanvasAnchor(this._canvasAnchor);
		}

		private void RenderEnabledStats()
		{
			if (!this.IsPanelActive)
			{
				return;
			}
			this._header.SetStatsToRender(this._statsEnabled);
		}

		internal void UpdateStatsEnabled(RenderSimStats stats)
		{
			this._statsEnabled = stats;
		}

		public void SetupStatisticsPanel()
		{
			if (this.IsPanelActive)
			{
				return;
			}
			if (!(base.Runner == null))
			{
				this._objectStatsGraphCombines = new Dictionary<FusionNetworkObjectStatistics, FusionNetworkObjectStatsGraphCombine>();
				this._statsPanelObject = UnityEngine.Object.Instantiate<GameObject>(this._statsCanvasPrefab, base.transform);
				this._statsCanvas = this._statsPanelObject.GetComponentInChildren<FusionStatsCanvas>();
				this._statsCanvas.SetupStatsCanvas(this, this._canvasAnchor, new UnityAction(this.DestroyStatisticsPanel));
				this._header = this._statsPanelObject.GetComponentInChildren<FusionStatsPanelHeader>();
				this._header.SetupHeader(base.Runner.LocalPlayer.ToString(), this);
				this._config = this._statsPanelObject.GetComponentInChildren<FusionStatsConfig>(true);
				this._statsPanelObject.AddComponent<FusionBasicBillboard>();
				this.ApplyCustomConfig();
				base.Runner.AddVisibilityNodes(this._statsPanelObject);
				if (this._statsEnabled != (RenderSimStats)0)
				{
					this.RenderEnabledStats();
				}
				if (!EventSystem.current)
				{
					new GameObject("EventSystem-FusionStatistics", new Type[]
					{
						typeof(EventSystem),
						typeof(StandaloneInputModule)
					});
				}
				return;
			}
			NetworkRunner component = base.GetComponent<NetworkRunner>();
			if (!component.IsRunning)
			{
				Log.Warn(string.Format("Network Runner on ({0}) is not yet running.", component.gameObject));
				return;
			}
			component.AddGlobal(this);
		}

		public void SetWorldAnchor(FusionStatsWorldAnchor anchor, float scale)
		{
			this._config.SetWorldCanvasScale(scale);
			if (anchor == null)
			{
				this._config.ResetToCanvasAnchor();
				return;
			}
			this._config.SetWorldAnchor(anchor.transform);
		}

		public void DestroyStatisticsPanel()
		{
			Dictionary<FusionNetworkObjectStatistics, FusionNetworkObjectStatsGraphCombine> objectStatsGraphCombines = this._objectStatsGraphCombines;
			FusionNetworkObjectStatistics[] array = (objectStatsGraphCombines != null) ? objectStatsGraphCombines.Keys.ToArray<FusionNetworkObjectStatistics>() : null;
			if (array != null)
			{
				foreach (FusionNetworkObjectStatistics fusionNetworkObjectStatistics in array)
				{
					this.MonitorNetworkObject(fusionNetworkObjectStatistics.NetworkObject, fusionNetworkObjectStatistics, false);
				}
			}
			Dictionary<FusionNetworkObjectStatistics, FusionNetworkObjectStatsGraphCombine> objectStatsGraphCombines2 = this._objectStatsGraphCombines;
			if (objectStatsGraphCombines2 != null)
			{
				objectStatsGraphCombines2.Clear();
			}
			this._statsGraph.Clear();
			UnityEngine.Object.Destroy(this._statsPanelObject);
			this._statsPanelObject = null;
			FusionStatisticsManager fusionStatisticsManager;
			if (base.Runner && base.Runner.TryGetFusionStatistics(out fusionStatisticsManager))
			{
				fusionStatisticsManager.ObjectStatisticsManager.ClearMonitoredNetworkObjects();
			}
		}

		public bool MonitorNetworkObject(NetworkObject networkObject, FusionNetworkObjectStatistics objectStatisticsInstance, bool monitor)
		{
			FusionStatisticsManager fusionStatisticsManager;
			if (base.Runner.TryGetFusionStatistics(out fusionStatisticsManager))
			{
				fusionStatisticsManager.ObjectStatisticsManager.MonitorNetworkObjectStatistics(networkObject.Id, monitor);
			}
			FusionNetworkObjectStatsGraphCombine fusionNetworkObjectStatsGraphCombine2;
			if (monitor)
			{
				if (this._objectStatsGraphCombines.ContainsKey(objectStatisticsInstance))
				{
					return false;
				}
				FusionNetworkObjectStatsGraphCombine fusionNetworkObjectStatsGraphCombine = UnityEngine.Object.Instantiate<FusionNetworkObjectStatsGraphCombine>(this._objectGraphCombinePrefab, this._header.ContentRect);
				fusionNetworkObjectStatsGraphCombine.SetupNetworkObject(networkObject, this, objectStatisticsInstance);
				this._objectStatsGraphCombines.Add(objectStatisticsInstance, fusionNetworkObjectStatsGraphCombine);
			}
			else if (this._objectStatsGraphCombines.Remove(objectStatisticsInstance, out fusionNetworkObjectStatsGraphCombine2))
			{
				UnityEngine.Object.Destroy(fusionNetworkObjectStatsGraphCombine2.gameObject);
				UnityEngine.Object.Destroy(objectStatisticsInstance);
			}
			return true;
		}

		private void UpdateAllGraphs(FusionStatisticsManager statisticsManager)
		{
			DateTime now = DateTime.Now;
			foreach (FusionStatsGraphBase fusionStatsGraphBase in this._statsGraph)
			{
				fusionStatsGraphBase.UpdateGraph(base.Runner, statisticsManager, ref now);
			}
		}

		public void RegisterGraph(FusionStatsGraphBase graph)
		{
			this._statsGraph.Add(graph);
		}

		public void UnregisterGraph(FusionStatsGraphBase graph)
		{
			this._statsGraph.Remove(graph);
		}

		private void Update()
		{
			if (!base.Runner)
			{
				return;
			}
			FusionStatisticsManager statisticsManager;
			if (base.Runner.TryGetFusionStatistics(out statisticsManager))
			{
				this.UpdateAllGraphs(statisticsManager);
			}
		}

		private GameObject _statsCanvasPrefab;

		private FusionNetworkObjectStatsGraphCombine _objectGraphCombinePrefab;

		private const string STATS_CANVAS_PREFAB_PATH = "FusionStatsResources/FusionStatsRenderPanel";

		private const string STATS_OBJECT_COMBINE_PREFAB_PATH = "FusionStatsResources/NetworkObjectStatistics";

		private List<FusionStatsGraphBase> _statsGraph;

		private FusionStatsPanelHeader _header;

		private FusionStatsConfig _config;

		private FusionStatsCanvas _statsCanvas;

		private GameObject _statsPanelObject;

		private Dictionary<FusionNetworkObjectStatistics, FusionNetworkObjectStatsGraphCombine> _objectStatsGraphCombines;

		[InlineHelp]
		[ExpandableEnum]
		[SerializeField]
		private RenderSimStats _statsEnabled;

		[InlineHelp]
		[SerializeField]
		private CanvasAnchor _canvasAnchor = CanvasAnchor.TopRight;

		[FormerlySerializedAs("_statsConfig")]
		[SerializeField]
		[Header("Custom configuration to override default values.\nSelect only one stat flag per configuration.")]
		private List<FusionStatistics.FusionStatisticsStatCustomConfig> _statsCustomConfig = new List<FusionStatistics.FusionStatisticsStatCustomConfig>();

		[Serializable]
		public struct FusionStatisticsStatCustomConfig
		{
			public RenderSimStats Stat;

			public float Threshold1;

			public float Threshold2;

			public float Threshold3;

			public bool IgnoreZeroOnBuffer;

			public bool IgnoreZeroOnAverageCalculation;

			public int AccumulateTimeMs;
		}
	}
}
