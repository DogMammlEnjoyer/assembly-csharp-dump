using System;
using Modio.Mods;
using Modio.Unity.UI.Components;

namespace Modio.Unity.UI.Panels.Report
{
	public class ModioReportPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._modioUIMod = base.GetComponent<ModioUIMod>();
		}

		public void OpenReportFlow(Mod mod)
		{
			this._modioUIMod.SetMod(mod);
			ModioReportTypePanel panelOfType = ModioPanelManager.GetPanelOfType<ModioReportTypePanel>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.OpenPanel();
		}

		private void LateUpdate()
		{
			if (base.HasFocus)
			{
				base.ClosePanel();
			}
		}

		private ModioUIMod _modioUIMod;
	}
}
