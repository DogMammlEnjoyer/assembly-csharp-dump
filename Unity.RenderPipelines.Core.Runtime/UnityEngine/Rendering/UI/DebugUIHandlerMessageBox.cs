using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerMessageBox : DebugUIHandlerWidget
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Field = base.CastWidget<DebugUI.MessageBox>();
			this.nameLabel.text = this.m_Field.displayName;
			Image component = base.GetComponent<Image>();
			DebugUI.MessageBox.Style style = this.m_Field.style;
			if (style == DebugUI.MessageBox.Style.Warning)
			{
				component.color = DebugUIHandlerMessageBox.k_WarningBackgroundColor;
				return;
			}
			if (style != DebugUI.MessageBox.Style.Error)
			{
				return;
			}
			component.color = DebugUIHandlerMessageBox.k_ErrorBackgroundColor;
		}

		private void Update()
		{
			this.nameLabel.text = this.m_Field.message;
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			return false;
		}

		public Text nameLabel;

		private DebugUI.MessageBox m_Field;

		private static Color32 k_WarningBackgroundColor = new Color32(231, 180, 3, 30);

		private static Color32 k_WarningTextColor = new Color32(231, 180, 3, byte.MaxValue);

		private static Color32 k_ErrorBackgroundColor = new Color32(231, 75, 3, 30);

		private static Color32 k_ErrorTextColor = new Color32(231, 75, 3, byte.MaxValue);
	}
}
