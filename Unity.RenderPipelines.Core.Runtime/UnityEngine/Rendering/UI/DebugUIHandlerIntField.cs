using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerIntField : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.IntField>();
			this.nameLabel.text = this.m_Field.displayName;
			this.UpdateValueLabel();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			this.valueLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
			this.valueLabel.color = this.colorDefault;
		}

		public override void OnIncrement(bool fast)
		{
			this.ChangeValue(fast, 1);
		}

		public override void OnDecrement(bool fast)
		{
			this.ChangeValue(fast, -1);
		}

		private void ChangeValue(bool fast, int multiplier)
		{
			int num = this.m_Field.GetValue();
			num += this.m_Field.incStep * (fast ? this.m_Field.intStepMult : 1) * multiplier;
			this.m_Field.SetValue(num);
			this.UpdateValueLabel();
		}

		private void UpdateValueLabel()
		{
			if (this.valueLabel != null)
			{
				this.valueLabel.text = this.m_Field.GetValue().ToString("N0");
			}
		}

		public Text nameLabel;

		public Text valueLabel;

		private DebugUI.IntField m_Field;
	}
}
