using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerRenderingLayerField : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.RenderingLayerField>();
			this.m_Container = base.GetComponent<DebugUIHandlerContainer>();
			this.nameLabel.text = this.m_Field.displayName;
			int i = 0;
			int num = this.m_Field.renderingLayersNames.Length - 1;
			foreach (string text in this.m_Field.renderingLayersNames)
			{
				if (i < this.toggles.Count)
				{
					DebugUIHandlerIndirectToggle debugUIHandlerIndirectToggle = this.toggles[i];
					debugUIHandlerIndirectToggle.getter = new Func<int, bool>(this.GetValue);
					debugUIHandlerIndirectToggle.setter = new Action<int, bool>(this.SetValue);
					debugUIHandlerIndirectToggle.nextUIHandler = ((i < num) ? this.toggles[i + 1] : null);
					debugUIHandlerIndirectToggle.previousUIHandler = ((i > 0) ? this.toggles[i - 1] : null);
					debugUIHandlerIndirectToggle.parentUIHandler = this;
					debugUIHandlerIndirectToggle.index = i;
					debugUIHandlerIndirectToggle.nameLabel.text = text;
					debugUIHandlerIndirectToggle.Init();
					i++;
				}
			}
			while (i < this.toggles.Count)
			{
				CoreUtils.Destroy(this.toggles[i].gameObject);
				this.toggles[i] = null;
				i++;
			}
		}

		private bool GetValue(int index)
		{
			return (this.m_Field.GetValue() & 1U << index) > 0U;
		}

		private void SetValue(int index, bool value)
		{
			RenderingLayerMask renderingLayerMask = this.m_Field.GetValue();
			if (value)
			{
				renderingLayerMask |= 1 << index;
			}
			else
			{
				renderingLayerMask &= ~(1 << index);
			}
			this.m_Field.SetValue(renderingLayerMask);
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
			this.valueToggle.isOn = true;
		}

		public override void OnDecrement(bool fast)
		{
			this.valueToggle.isOn = false;
		}

		public override void OnAction()
		{
			this.valueToggle.isOn = !this.valueToggle.isOn;
		}

		public override DebugUIHandlerWidget Next()
		{
			if (!this.valueToggle.isOn || this.m_Container == null)
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

		public List<DebugUIHandlerIndirectToggle> toggles;

		private DebugUI.RenderingLayerField m_Field;

		private DebugUIHandlerContainer m_Container;
	}
}
