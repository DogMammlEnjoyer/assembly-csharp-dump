using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionStatsConfig : MonoBehaviour
	{
		public bool IsWorldAnchored
		{
			get
			{
				return this._worldTransformAnchor != null;
			}
		}

		private static event Action _onWorldAnchorCandidatesUpdate;

		internal static void SetWorldAnchorCandidate(Transform candidate, bool register)
		{
			if (register)
			{
				if (!FusionStatsConfig._worldAnchorCandidates.Contains(candidate))
				{
					FusionStatsConfig._worldAnchorCandidates.Add(candidate);
				}
			}
			else
			{
				FusionStatsConfig._worldAnchorCandidates.Remove(candidate);
			}
			Action onWorldAnchorCandidatesUpdate = FusionStatsConfig._onWorldAnchorCandidatesUpdate;
			if (onWorldAnchorCandidatesUpdate == null)
			{
				return;
			}
			onWorldAnchorCandidatesUpdate();
		}

		internal void SetupStatisticReference(FusionStatistics fusionStatistics)
		{
			this._fusionStatistics = fusionStatistics;
		}

		public void ToggleConfigPanel()
		{
			this._configPanel.SetActive(!this._configPanel.activeSelf);
		}

		public void ToggleUseWorldAnchor(bool value)
		{
			if (!value)
			{
				this.ResetToCanvasAnchor();
			}
		}

		internal void SetWorldAnchor(Transform worldTransformAnchor)
		{
			this._canvas.renderMode = RenderMode.WorldSpace;
			this._renderPanelRectTransform.localScale = Vector3.one * this._worldCanvasScale;
			this._renderPanelRectTransform.localPosition = Vector3.zero;
			if (worldTransformAnchor == this._worldTransformAnchor)
			{
				return;
			}
			this._renderPanelRectTransform.SetParent(worldTransformAnchor);
			this._worldTransformAnchor = worldTransformAnchor;
			this._renderPanelRectTransform.localPosition = Vector3.zero;
		}

		public void SetWorldCanvasScale(float value)
		{
			this._worldCanvasScale = value;
		}

		internal void ResetToCanvasAnchor()
		{
			if (!this._fusionStatistics)
			{
				return;
			}
			RectTransform rectTransform = (RectTransform)this._renderPanelRectTransform.GetChild(0);
			this._renderPanelRectTransform.SetParent(this._fusionStatistics.transform);
			this._canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			this._renderPanelRectTransform.localScale = Vector3.one;
			this._renderPanelRectTransform.localPosition = Vector3.zero;
			rectTransform.localPosition = Vector3.zero;
			rectTransform.anchoredPosition = Vector3.zero;
			this._worldTransformAnchor = null;
		}

		private void UpdateWorldAnchorButtons()
		{
			for (int i = this._worldAnchorListContainer.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(this._worldAnchorListContainer.GetChild(i).gameObject);
			}
			using (List<Transform>.Enumerator enumerator = FusionStatsConfig._worldAnchorCandidates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform candidate = enumerator.Current;
					Button button = Object.Instantiate<Button>(this._worldAnchorButtonPrefab, this._worldAnchorListContainer);
					button.onClick.AddListener(delegate()
					{
						this.SetWorldAnchor(candidate);
					});
					button.GetComponentInChildren<Text>().text = candidate.name;
				}
			}
		}

		private void OnEnable()
		{
			FusionStatsConfig._onWorldAnchorCandidatesUpdate -= this.UpdateWorldAnchorButtons;
			FusionStatsConfig._onWorldAnchorCandidatesUpdate += this.UpdateWorldAnchorButtons;
			this.UpdateWorldAnchorButtons();
		}

		private void OnDestroy()
		{
			FusionStatsConfig._onWorldAnchorCandidatesUpdate -= this.UpdateWorldAnchorButtons;
		}

		[SerializeField]
		private Button _worldAnchorButtonPrefab;

		[SerializeField]
		private Transform _worldAnchorListContainer;

		[SerializeField]
		private GameObject _configPanel;

		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private RectTransform _renderPanelRectTransform;

		private Transform _worldTransformAnchor;

		private float _worldCanvasScale = 0.005f;

		private FusionStatistics _fusionStatistics;

		private static List<Transform> _worldAnchorCandidates = new List<Transform>();
	}
}
