using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Mods;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Panels.Report;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyOptionButtonsForPanels : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._mod = mod;
			ModPropertyOptionButtonsForPanels.<OnModUpdate>g__SetupButton|6_0(this._viewModButton, new UnityAction(this.ViewModButtonClicked));
			ModPropertyOptionButtonsForPanels.<OnModUpdate>g__SetupButton|6_0(this._moreFromCreatorButton, new UnityAction(this.MoreFromCreatorButtonClicked));
			ModPropertyOptionButtonsForPanels.<OnModUpdate>g__SetupButton|6_0(this._reportModButton, new UnityAction(this.ReportModButtonClicked));
			ModPropertyOptionButtonsForPanels.<OnModUpdate>g__SetupButton|6_0(this._retryDownloadButton, new UnityAction(this.RetryDownloadButtonClicked));
			ModPropertyOptionButtonsForPanels.<OnModUpdate>g__SetupButton|6_0(this._uninstallButton, new UnityAction(this.UninstallModButtonClicked));
		}

		private void ViewModButtonClicked()
		{
			ModioPanelManager.GetPanelOfType<ModDisplayPanel>().OpenPanel(this._mod);
		}

		private void MoreFromCreatorButtonClicked()
		{
			ModioUISearch.Default.SetSearchForUser(this._mod.Creator);
		}

		private void ReportModButtonClicked()
		{
			ModioPanelManager.GetPanelOfType<ModioReportPanel>().OpenReportFlow(this._mod);
		}

		private void RetryDownloadButtonClicked()
		{
			Task<Error> task = ModInstallationManagement.RetryInstallingMod(this._mod);
			ModioErrorPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.MonitorTaskThenOpenPanelIfError(task);
		}

		private void UninstallModButtonClicked()
		{
			this._mod.UninstallOtherUserMod(false);
		}

		[CompilerGenerated]
		internal static void <OnModUpdate>g__SetupButton|6_0(Button button, UnityAction listener)
		{
			if (button == null)
			{
				return;
			}
			button.onClick.RemoveListener(listener);
			button.onClick.AddListener(listener);
		}

		[SerializeField]
		private Button _viewModButton;

		[SerializeField]
		private Button _moreFromCreatorButton;

		[SerializeField]
		private Button _reportModButton;

		[SerializeField]
		private Button _retryDownloadButton;

		[SerializeField]
		private Button _uninstallButton;

		private Mod _mod;
	}
}
