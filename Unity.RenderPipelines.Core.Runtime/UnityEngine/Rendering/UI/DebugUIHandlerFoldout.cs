using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerFoldout : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.Foldout>();
			this.m_Container = base.GetComponent<DebugUIHandlerContainer>();
			this.nameLabel.text = this.m_Field.displayName;
			string[] columnLabels = this.m_Field.columnLabels;
			int num = (columnLabels != null) ? columnLabels.Length : 0;
			float num2 = (num > 0) ? (230f / (float)num) : 0f;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.nameLabel.gameObject, base.GetComponent<DebugUIHandlerContainer>().contentHolder);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				RectTransform rectTransform = gameObject.transform as RectTransform;
				RectTransform rectTransform2 = this.nameLabel.transform as RectTransform;
				Vector2 vector = new Vector2(0f, 1f);
				rectTransform.anchorMin = vector;
				rectTransform.anchorMax = vector;
				rectTransform.sizeDelta = new Vector2(100f, 26f);
				Vector3 v = rectTransform2.anchoredPosition;
				v.x += (float)(i + 1) * num2 + 215f;
				rectTransform.anchoredPosition = v;
				rectTransform.pivot = new Vector2(0f, 0.5f);
				rectTransform.eulerAngles = new Vector3(0f, 0f, 13f);
				Text component = gameObject.GetComponent<Text>();
				component.fontSize = 15;
				component.text = this.m_Field.columnLabels[i];
			}
			this.UpdateValue();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			if (fromNext || !this.valueToggle.isOn)
			{
				this.nameLabel.color = this.colorSelected;
			}
			else if (this.valueToggle.isOn)
			{
				if (this.m_Container.IsDirectChild(previous))
				{
					this.nameLabel.color = this.colorSelected;
				}
				else
				{
					DebugUIHandlerWidget lastItem = this.m_Container.GetLastItem();
					DebugManager.instance.ChangeSelection(lastItem, false);
				}
			}
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
		}

		public override void OnIncrement(bool fast)
		{
			this.m_Field.SetValue(true);
			this.UpdateValue();
		}

		public override void OnDecrement(bool fast)
		{
			this.m_Field.SetValue(false);
			this.UpdateValue();
		}

		public override void OnAction()
		{
			bool value = !this.m_Field.GetValue();
			this.m_Field.SetValue(value);
			this.UpdateValue();
		}

		private void UpdateValue()
		{
			this.valueToggle.isOn = this.m_Field.GetValue();
		}

		public override DebugUIHandlerWidget Next()
		{
			if (!this.m_Field.GetValue() || this.m_Container == null)
			{
				return base.Next();
			}
			DebugUIHandlerWidget firstItem = this.m_Container.GetFirstItem();
			if (firstItem == null)
			{
				return base.Next();
			}
			return firstItem;
		}

		public Text nameLabel;

		public UIFoldout valueToggle;

		private DebugUI.Foldout m_Field;

		private DebugUIHandlerContainer m_Container;

		private const float k_FoldoutXOffset = 215f;

		private const float k_XOffset = 230f;
	}
}
