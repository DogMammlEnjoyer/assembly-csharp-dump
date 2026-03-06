using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerUIntField : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.UIntField>();
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
			long num = (long)((ulong)this.m_Field.GetValue());
			if (num == 0L && multiplier < 0)
			{
				return;
			}
			num += (long)((ulong)(this.m_Field.incStep * (fast ? this.m_Field.intStepMult : 1U)) * (ulong)((long)multiplier));
			this.m_Field.SetValue((uint)num);
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

		private DebugUI.UIntField m_Field;
	}
}
