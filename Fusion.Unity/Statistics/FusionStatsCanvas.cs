using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fusion.Statistics
{
	public class FusionStatsCanvas : MonoBehaviour, IDragHandler, IEventSystemHandler, IEndDragHandler, IBeginDragHandler
	{
		private bool _isColapsed
		{
			get
			{
				return !this._contentPanel.gameObject.activeSelf;
			}
		}

		internal void SetupStatsCanvas(FusionStatistics fusionStatistics, CanvasAnchor canvasAnchor, UnityAction closeButtonAction)
		{
			this._anchor = canvasAnchor;
			this._canvasPanel.anchoredPosition = this.GetDefinedAnchorPosition();
			int num = Mathf.Min(FusionStatsCanvas._statsCanvasActiveCount, 3);
			this._canvasPanel.anchoredPosition += Vector2.down * (50f * (float)num);
			this._closeButton.onClick.RemoveAllListeners();
			this._closeButton.onClick.AddListener(closeButtonAction);
			this._hideButton.onClick.RemoveAllListeners();
			this._hideButton.onClick.AddListener(new UnityAction(this.ToggleHide));
			this._config.SetupStatisticReference(fusionStatistics);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (this._config.IsWorldAnchored)
			{
				return;
			}
			if (this._dragMode != FusionStatsCanvas.DragMode.None)
			{
				return;
			}
			Vector2 vector = eventData.pressPosition;
			RectTransform bottomPanel = this._bottomPanel;
			vector = bottomPanel.InverseTransformPoint(vector);
			this._dragMode = ((bottomPanel.rect.Contains(vector) && eventData.button == PointerEventData.InputButton.Right) ? FusionStatsCanvas.DragMode.ResizeContent : FusionStatsCanvas.DragMode.DragCanvas);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (this._config.IsWorldAnchored)
			{
				return;
			}
			FusionStatsCanvas.DragMode dragMode = this._dragMode;
			if (dragMode == FusionStatsCanvas.DragMode.DragCanvas)
			{
				this._canvasPanel.anchoredPosition += eventData.delta / this._canvas.scaleFactor;
				return;
			}
			if (dragMode != FusionStatsCanvas.DragMode.ResizeContent)
			{
				return;
			}
			this.UpdateContentContainerHeight(eventData.delta.y / this._canvas.scaleFactor);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (this._config.IsWorldAnchored)
			{
				return;
			}
			if (!this.CheckDraggableRectVisibility(this._canvasPanel))
			{
				this.SnapPanelBackToOriginPos();
			}
			if (this._dragMode == FusionStatsCanvas.DragMode.ResizeContent)
			{
				float y = this._contentPanel.sizeDelta.y;
				float num = 0f;
				float yDelta = 0f;
				int i = 0;
				while (i < this._contentContainer.childCount)
				{
					float num2 = num;
					num += ((RectTransform)this._contentContainer.GetChild(i)).sizeDelta.y + 10f;
					if (num >= y)
					{
						if (y - num2 < num - y)
						{
							yDelta = y - num2;
							break;
						}
						yDelta = -(num - y);
						break;
					}
					else
					{
						i++;
					}
				}
				this.UpdateContentContainerHeight(yDelta);
			}
			this._dragMode = FusionStatsCanvas.DragMode.None;
		}

		public void SnapPanelBackToOriginPos()
		{
			this._canvasPanel.anchoredPosition = this.GetDefinedAnchorPosition();
		}

		private void UpdateContentContainerHeight(float yDelta)
		{
			float contentPanelHeight = this._contentPanel.sizeDelta.y - yDelta;
			this.SetContentPanelHeight(contentPanelHeight);
		}

		internal void ToggleHide()
		{
			bool activeSelf = this._contentPanel.gameObject.activeSelf;
			this._hideButton.transform.rotation = (activeSelf ? Quaternion.Euler(0f, 0f, 90f) : Quaternion.identity);
			this._contentPanel.gameObject.SetActive(!activeSelf);
			this._bottomPanel.gameObject.SetActive(!activeSelf);
		}

		private bool CheckDraggableRectVisibility(RectTransform rectTransform)
		{
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			Vector2 size = rectTransform.rect.size;
			return Mathf.Abs(anchoredPosition.x) < this._canvasScaler.referenceResolution.x * 0.5f + size.x * 0.5f && anchoredPosition.y < this._canvasScaler.referenceResolution.y * 0.5f + size.y && anchoredPosition.y > -this._canvasScaler.referenceResolution.y * 0.5f;
		}

		private void SetContentPanelHeight(float value)
		{
			if (value < 150f)
			{
				value = 150f;
			}
			else
			{
				float num = (float)Screen.height / this._canvas.scaleFactor - 100f;
				if (value > num)
				{
					value = num;
				}
			}
			this._contentPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
			this._contentPanel.gameObject.SetActive(false);
			this._contentPanel.gameObject.SetActive(true);
		}

		private void AdaptContentHeightToGraphs()
		{
			float num = 0f;
			for (int i = 0; i < this._contentContainer.childCount; i++)
			{
				num += ((RectTransform)this._contentContainer.GetChild(i)).sizeDelta.y + 10f;
			}
			float num2 = (float)Screen.height / this._canvas.scaleFactor - 100f;
			if (num > num2)
			{
				num = num2;
			}
			if (num < 150f)
			{
				num = 150f;
			}
			this.SetContentPanelHeight(num);
		}

		private void OnEnable()
		{
			FusionStatsCanvas._statsCanvasActiveCount++;
			this._header.OnRenderStatsUpdate += this.AdaptContentHeightToGraphs;
		}

		private void OnDisable()
		{
			FusionStatsCanvas._statsCanvasActiveCount--;
			this._header.OnRenderStatsUpdate -= this.AdaptContentHeightToGraphs;
		}

		public void SetCanvasAnchor(CanvasAnchor anchor)
		{
			this._anchor = anchor;
			this.SnapPanelBackToOriginPos();
		}

		private Vector2 GetDefinedAnchorPosition()
		{
			Vector2 referenceResolution = this._canvasScaler.referenceResolution;
			CanvasAnchor anchor = this._anchor;
			if (anchor == CanvasAnchor.TopLeft)
			{
				referenceResolution.x *= -1f;
				return referenceResolution * 0.5f + Vector2.right * (this._canvasPanel.sizeDelta.x * 0.5f);
			}
			if (anchor == CanvasAnchor.TopRight)
			{
				return referenceResolution * 0.5f - Vector2.right * (this._canvasPanel.sizeDelta.x * 0.5f);
			}
			return Vector2.zero;
		}

		[Header("General References")]
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private CanvasScaler _canvasScaler;

		[SerializeField]
		private RectTransform _canvasPanel;

		[Space]
		[Header("Panel References")]
		[SerializeField]
		private RectTransform _contentPanel;

		[SerializeField]
		private RectTransform _contentContainer;

		[SerializeField]
		private RectTransform _bottomPanel;

		[SerializeField]
		private FusionStatsPanelHeader _header;

		[Space]
		[Header("Misc")]
		[SerializeField]
		private Button _hideButton;

		[SerializeField]
		private Button _closeButton;

		[Space]
		[Header("World Anchor Panel Settings")]
		[SerializeField]
		private FusionStatsConfig _config;

		private CanvasAnchor _anchor;

		private FusionStatsCanvas.DragMode _dragMode;

		private static int _statsCanvasActiveCount;

		private enum DragMode
		{
			None,
			DragCanvas,
			ResizeContent
		}
	}
}
