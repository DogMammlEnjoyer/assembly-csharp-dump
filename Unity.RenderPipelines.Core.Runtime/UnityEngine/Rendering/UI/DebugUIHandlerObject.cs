using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerObject : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			DebugUI.ObjectField objectField = base.CastWidget<DebugUI.ObjectField>();
			this.nameLabel.text = objectField.displayName;
			Object value = objectField.GetValue();
			this.valueLabel.text = ((value != null) ? value.name : "None");
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

		public Text nameLabel;

		public Text valueLabel;
	}
}
