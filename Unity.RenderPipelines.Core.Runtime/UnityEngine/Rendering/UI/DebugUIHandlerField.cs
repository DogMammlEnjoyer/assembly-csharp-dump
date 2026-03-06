using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public abstract class DebugUIHandlerField<T> : DebugUIHandlerWidget where T : DebugUI.Widget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<T>();
			this.nameLabel.text = this.m_Field.displayName;
			this.UpdateValueLabel();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			if (this.nextButtonText != null)
			{
				this.nextButtonText.color = this.colorSelected;
			}
			if (this.previousButtonText != null)
			{
				this.previousButtonText.color = this.colorSelected;
			}
			this.nameLabel.color = this.colorSelected;
			this.valueLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			if (this.nextButtonText != null)
			{
				this.nextButtonText.color = this.colorDefault;
			}
			if (this.previousButtonText != null)
			{
				this.previousButtonText.color = this.colorDefault;
			}
			this.nameLabel.color = this.colorDefault;
			this.valueLabel.color = this.colorDefault;
		}

		public override void OnAction()
		{
			this.OnIncrement(false);
		}

		public abstract void UpdateValueLabel();

		protected void SetLabelText(string text)
		{
			if (text.Length > 26)
			{
				text = text.Substring(0, 23) + "...";
			}
			this.valueLabel.text = text;
		}

		public Text nextButtonText;

		public Text previousButtonText;

		public Text nameLabel;

		public Text valueLabel;

		protected internal T m_Field;
	}
}
