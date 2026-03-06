using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerBitField : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.BitField>();
			this.m_Container = base.GetComponent<DebugUIHandlerContainer>();
			this.nameLabel.text = this.m_Field.displayName;
			int i = 0;
			foreach (GUIContent guicontent in this.m_Field.enumNames)
			{
				if (i < this.toggles.Count)
				{
					DebugUIHandlerIndirectToggle debugUIHandlerIndirectToggle = this.toggles[i];
					debugUIHandlerIndirectToggle.getter = new Func<int, bool>(this.GetValue);
					debugUIHandlerIndirectToggle.setter = new Action<int, bool>(this.SetValue);
					debugUIHandlerIndirectToggle.nextUIHandler = ((i < this.m_Field.enumNames.Length - 1) ? this.toggles[i + 1] : null);
					debugUIHandlerIndirectToggle.previousUIHandler = ((i > 0) ? this.toggles[i - 1] : null);
					debugUIHandlerIndirectToggle.parentUIHandler = this;
					debugUIHandlerIndirectToggle.index = i;
					debugUIHandlerIndirectToggle.nameLabel.text = guicontent.text;
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
			if (index == 0)
			{
				return false;
			}
			index--;
			return (Convert.ToInt32(this.m_Field.GetValue()) & 1 << index) != 0;
		}

		private void SetValue(int index, bool value)
		{
			if (index == 0)
			{
				this.m_Field.SetValue(Enum.ToObject(this.m_Field.enumType, 0));
				using (List<DebugUIHandlerIndirectToggle>.Enumerator enumerator = this.toggles.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DebugUIHandlerIndirectToggle debugUIHandlerIndirectToggle = enumerator.Current;
						if (debugUIHandlerIndirectToggle != null && debugUIHandlerIndirectToggle.getter != null)
						{
							debugUIHandlerIndirectToggle.UpdateValueLabel();
						}
					}
					return;
				}
			}
			int num = Convert.ToInt32(this.m_Field.GetValue());
			if (value)
			{
				num |= this.m_Field.enumValues[index];
			}
			else
			{
				num &= ~this.m_Field.enumValues[index];
			}
			this.m_Field.SetValue(Enum.ToObject(this.m_Field.enumType, num));
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

		private DebugUI.BitField m_Field;

		private DebugUIHandlerContainer m_Container;
	}
}
