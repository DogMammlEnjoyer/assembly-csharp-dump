using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerValue : DebugUIHandlerWidget
	{
		protected override void OnEnable()
		{
			this.m_Timer = 0f;
		}

		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.Value>();
			this.nameLabel.text = this.m_Field.displayName;
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

		private void Update()
		{
			if (this.m_Timer >= this.m_Field.refreshRate)
			{
				object value = this.m_Field.GetValue();
				this.valueLabel.text = this.m_Field.FormatString(value);
				if (value is float)
				{
					this.valueLabel.color = (((float)value == 0f) ? DebugUIHandlerValue.k_ZeroColor : this.colorDefault);
				}
				this.m_Timer -= this.m_Field.refreshRate;
			}
			this.m_Timer += Time.deltaTime;
		}

		public Text nameLabel;

		public Text valueLabel;

		private DebugUI.Value m_Field;

		protected internal float m_Timer;

		private static readonly Color k_ZeroColor = Color.gray;
	}
}
