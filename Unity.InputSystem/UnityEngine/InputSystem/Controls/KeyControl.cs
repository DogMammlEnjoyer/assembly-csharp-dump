using System;
using System.Globalization;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Controls
{
	public class KeyControl : ButtonControl
	{
		public Key keyCode { get; set; }

		public int scanCode
		{
			get
			{
				base.RefreshConfigurationIfNeeded();
				return this.m_ScanCode;
			}
		}

		protected override void RefreshConfiguration()
		{
			base.displayName = null;
			this.m_ScanCode = 0;
			QueryKeyNameCommand queryKeyNameCommand = QueryKeyNameCommand.Create(this.keyCode);
			if (base.device.ExecuteCommand<QueryKeyNameCommand>(ref queryKeyNameCommand) > 0L)
			{
				this.m_ScanCode = queryKeyNameCommand.scanOrKeyCode;
				string text = queryKeyNameCommand.ReadKeyName();
				if (string.IsNullOrEmpty(text))
				{
					base.displayName = text;
					return;
				}
				string text2 = text.ToLowerInvariant();
				if (string.IsNullOrEmpty(text2))
				{
					base.displayName = text;
					return;
				}
				TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
				base.displayName = textInfo.ToTitleCase(text2);
			}
		}

		private int m_ScanCode;
	}
}
