using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerPanel : MonoBehaviour
	{
		private void OnEnable()
		{
			this.m_ScrollTransform = this.scrollRect.GetComponent<RectTransform>();
			this.m_ContentTransform = base.GetComponent<DebugUIHandlerContainer>().contentHolder;
			this.m_MaskTransform = base.GetComponentInChildren<Mask>(true).rectTransform;
		}

		internal void SetPanel(DebugUI.Panel panel)
		{
			this.m_Panel = panel;
			this.nameLabel.text = panel.displayName;
		}

		internal DebugUI.Panel GetPanel()
		{
			return this.m_Panel;
		}

		public void SelectNextItem()
		{
			this.Canvas.SelectNextPanel();
		}

		public void SelectPreviousItem()
		{
			this.Canvas.SelectPreviousPanel();
		}

		public void OnScrollbarClicked()
		{
			DebugManager.instance.SetScrollTarget(null);
		}

		internal void SetScrollTarget(DebugUIHandlerWidget target)
		{
			this.m_ScrollTarget = target;
		}

		internal void UpdateScroll()
		{
			if (this.m_ScrollTarget == null)
			{
				return;
			}
			RectTransform component = this.m_ScrollTarget.GetComponent<RectTransform>();
			float yposInScroll = this.GetYPosInScroll(component);
			float num = (this.GetYPosInScroll(this.m_MaskTransform) - yposInScroll) / (this.m_ContentTransform.rect.size.y - this.m_ScrollTransform.rect.size.y);
			float num2 = this.scrollRect.verticalNormalizedPosition - num;
			num2 = Mathf.Clamp01(num2);
			this.scrollRect.verticalNormalizedPosition = Mathf.Lerp(this.scrollRect.verticalNormalizedPosition, num2, Time.deltaTime * 10f);
		}

		private float GetYPosInScroll(RectTransform target)
		{
			Vector3 b = new Vector3((0.5f - target.pivot.x) * target.rect.size.x, (0.5f - target.pivot.y) * target.rect.size.y, 0f);
			Vector3 position = target.localPosition + b;
			Vector3 position2 = target.parent.TransformPoint(position);
			return this.m_ScrollTransform.TransformPoint(position2).y;
		}

		internal DebugUIHandlerWidget GetFirstItem()
		{
			return base.GetComponent<DebugUIHandlerContainer>().GetFirstItem();
		}

		public void ResetDebugManager()
		{
			DebugManager.instance.Reset();
		}

		public Text nameLabel;

		public ScrollRect scrollRect;

		public RectTransform viewport;

		public DebugUIHandlerCanvas Canvas;

		private RectTransform m_ScrollTransform;

		private RectTransform m_ContentTransform;

		private RectTransform m_MaskTransform;

		private DebugUIHandlerWidget m_ScrollTarget;

		protected internal DebugUI.Panel m_Panel;
	}
}
