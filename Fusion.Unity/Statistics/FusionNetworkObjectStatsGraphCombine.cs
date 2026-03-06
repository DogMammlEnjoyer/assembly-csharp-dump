using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionNetworkObjectStatsGraphCombine : MonoBehaviour
	{
		public NetworkId NetworkObjectID
		{
			get
			{
				return this._networkObject.Id;
			}
		}

		public void SetupNetworkObject(NetworkObject networkObject, FusionStatistics fusionStatistics, FusionNetworkObjectStatistics objectStatisticsInstance)
		{
			this._networkObject = networkObject;
			this._fusionStatistics = fusionStatistics;
			this._objectStatisticsInstance = objectStatisticsInstance;
		}

		private void Start()
		{
			this._statsGraphs = new Dictionary<NetworkObjectStat, FusionNetworkObjectStatsGraph>();
			this._parentContentSizeFitter = base.GetComponentInParent<ContentSizeFitter>();
			List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
			list.Add(new Dropdown.OptionData("Toggle Stats"));
			foreach (string text in Enum.GetNames(typeof(NetworkObjectStat)))
			{
				list.Add(new Dropdown.OptionData(text));
			}
			this._statDropdown.options = list;
			this._statDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropDownChanged));
			this.UpdateHeight(-1f);
			this._titleText.text = this._networkObject.Name;
		}

		private void OnDropDownChanged(int arg0)
		{
			if (arg0 <= 0)
			{
				return;
			}
			arg0--;
			NetworkObjectStat networkObjectStat = (NetworkObjectStat)(1 << arg0);
			if ((this._statsToRender & networkObjectStat) == networkObjectStat)
			{
				this._statsToRender &= ~networkObjectStat;
				this.DestroyStatGraph(networkObjectStat);
			}
			else
			{
				this._statsToRender |= networkObjectStat;
				this.InstantiateStatGraph(networkObjectStat);
			}
			this.UpdateHeight(-1f);
			this._statDropdown.SetValueWithoutNotify(0);
		}

		private void InstantiateStatGraph(NetworkObjectStat stat)
		{
			FusionNetworkObjectStatsGraph fusionNetworkObjectStatsGraph = Object.Instantiate<FusionNetworkObjectStatsGraph>(this._statsGraphPrefab, this._combinedGraphRender);
			fusionNetworkObjectStatsGraph.SetupNetworkObjectStat(this.NetworkObjectID, stat);
			this._statsGraphs.Add(stat, fusionNetworkObjectStatsGraph);
		}

		private void DestroyStatGraph(NetworkObjectStat stat)
		{
			this._statsGraphs[stat].gameObject.SetActive(false);
			Object.Destroy(this._statsGraphs[stat].gameObject);
			this._statsGraphs.Remove(stat);
		}

		private void UpdateHeight(float overrideValue = -1f)
		{
			Vector2 sizeDelta = this._rect.sizeDelta;
			float y = (overrideValue >= 0f) ? overrideValue : (this._headerHeight + (float)this._statsGraphs.Count * this._graphHeight);
			this._rect.sizeDelta = new Vector2(sizeDelta.x, y);
			this._parentContentSizeFitter.enabled = false;
			this._parentContentSizeFitter.enabled = true;
		}

		private void OnDisable()
		{
			if (this._statsGraphs == null)
			{
				return;
			}
			foreach (FusionNetworkObjectStatsGraph fusionNetworkObjectStatsGraph in this._statsGraphs.Values)
			{
				fusionNetworkObjectStatsGraph.gameObject.SetActive(false);
			}
		}

		private void OnEnable()
		{
			if (this._statsGraphs == null)
			{
				return;
			}
			foreach (FusionNetworkObjectStatsGraph fusionNetworkObjectStatsGraph in this._statsGraphs.Values)
			{
				fusionNetworkObjectStatsGraph.gameObject.SetActive(true);
			}
		}

		public void ToggleRenderDisplay()
		{
			bool activeSelf = this._combinedGraphRender.gameObject.activeSelf;
			this._combinedGraphRender.gameObject.SetActive(!activeSelf);
			if (activeSelf)
			{
				this.OnDisable();
				this.UpdateHeight(this._headerHeight);
				this._toggleButton.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
				return;
			}
			this.OnEnable();
			this.UpdateHeight(-1f);
			this._toggleButton.transform.rotation = Quaternion.identity;
		}

		public void DestroyCombinedGraph()
		{
			this._fusionStatistics.MonitorNetworkObject(this._networkObject, this._objectStatisticsInstance, false);
		}

		[SerializeField]
		private Text _titleText;

		[SerializeField]
		private Dropdown _statDropdown;

		[SerializeField]
		private NetworkObjectStat _statsToRender;

		[SerializeField]
		private RectTransform _rect;

		[SerializeField]
		private RectTransform _combinedGraphRender;

		[SerializeField]
		private Button _toggleButton;

		private float _headerHeight = 50f;

		private float _graphHeight = 150f;

		private Dictionary<NetworkObjectStat, FusionNetworkObjectStatsGraph> _statsGraphs;

		[SerializeField]
		private FusionNetworkObjectStatsGraph _statsGraphPrefab;

		private ContentSizeFitter _parentContentSizeFitter;

		private NetworkObject _networkObject;

		private FusionStatistics _fusionStatistics;

		private FusionNetworkObjectStatistics _objectStatisticsInstance;
	}
}
