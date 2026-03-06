using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerButton : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.Button>();
			this.nameLabel.text = this.m_Field.displayName;
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
		}

		public override void OnAction()
		{
			if (this.m_Field.action != null)
			{
				this.m_Field.action();
			}
		}

		public Text nameLabel;

		private DebugUI.Button m_Field;
	}
}
