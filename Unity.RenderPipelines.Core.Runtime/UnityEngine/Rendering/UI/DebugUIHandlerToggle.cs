using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerToggle : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.BoolField>();
			this.nameLabel.text = this.m_Field.displayName;
			this.UpdateValueLabel();
			this.valueToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		}

		private void OnToggleValueChanged(bool value)
		{
			this.m_Field.SetValue(value);
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			this.checkmarkImage.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
			this.checkmarkImage.color = this.colorDefault;
		}

		public override void OnAction()
		{
			bool value = !this.m_Field.GetValue();
			this.m_Field.SetValue(value);
			this.UpdateValueLabel();
		}

		protected internal virtual void UpdateValueLabel()
		{
			if (this.valueToggle != null)
			{
				this.valueToggle.isOn = this.m_Field.GetValue();
			}
		}

		public Text nameLabel;

		public Toggle valueToggle;

		public Image checkmarkImage;

		protected internal DebugUI.BoolField m_Field;
	}
}
