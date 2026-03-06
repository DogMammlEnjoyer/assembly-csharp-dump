using System;
using System.Threading.Tasks;
using Modio.Mods;
using Modio.Reports;
using Modio.Unity.UI.Components;
using Modio.Unity.UI.Components.Selectables;
using Modio.Unity.UI.Navigation;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels.Report
{
	public class ModioReportDetailsPanel : ModioPanelBase
	{
		protected override void Start()
		{
			base.Start();
			this._description.onValueChanged.AddListener(new UnityAction<string>(this.OnDescriptionTextChanged));
			this.OnDescriptionTextChanged(this._description.text);
			this._modioUIMod = base.GetComponentInParent<ModioUIMod>();
			if (this._modioUIMod != null)
			{
				this._modioUIMod.onModUpdate.AddListener(new UnityAction(this.OnModUpdated));
				this.OnModUpdated();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this._modioUIMod != null)
			{
				this._modioUIMod.onModUpdate.RemoveListener(new UnityAction(this.OnModUpdated));
			}
		}

		private void OnModUpdated()
		{
			Mod lastMod = this._lastMod;
			ModioUIMod modioUIMod = this._modioUIMod;
			if (lastMod == ((modioUIMod != null) ? modioUIMod.Mod : null))
			{
				return;
			}
			ModioUIMod modioUIMod2 = this._modioUIMod;
			this._lastMod = ((modioUIMod2 != null) ? modioUIMod2.Mod : null);
			this._description.text = "";
		}

		private void OnDescriptionTextChanged(string description)
		{
			this._disableWhenInvalidToSubmit.interactable = !string.IsNullOrEmpty(description);
			ModioGridNavigation componentInParent = this._disableWhenInvalidToSubmit.GetComponentInParent<ModioGridNavigation>();
			if (componentInParent != null)
			{
				componentInParent.NeedsNavigationCorrection();
			}
		}

		public void OpenPanel(ReportType type)
		{
			this._reportType = type;
			base.OpenPanel();
		}

		public void OnUserPressedBackButton()
		{
			base.ClosePanel();
			ModioPanelManager.GetPanelOfType<ModioReportTypePanel>().OpenPanel();
		}

		public void OnUserSubmittedReportDetails()
		{
			base.ClosePanel();
			if (User.Current == null)
			{
				return;
			}
			Task<Error> task = this._modioUIMod.Mod.Report(this._reportType, ModNotWorkingReason.None, this._email.text, this._description.text);
			ModioPanelManager.GetPanelOfType<ModioReportWaitingPanel>().OpenAndWaitFor<Error>(task, new Action<Error>(this.ReportCompleted));
		}

		private void ReportCompleted(Error error)
		{
			if (error)
			{
				ModioReportErrorPanel panelOfType = ModioPanelManager.GetPanelOfType<ModioReportErrorPanel>();
				if (panelOfType == null)
				{
					return;
				}
				panelOfType.OpenPanel(error);
				return;
			}
			else
			{
				ModioReportConfirmationPanel panelOfType2 = ModioPanelManager.GetPanelOfType<ModioReportConfirmationPanel>();
				if (panelOfType2 == null)
				{
					return;
				}
				panelOfType2.OpenPanel();
				return;
			}
		}

		private ReportType _reportType;

		[SerializeField]
		private TMP_InputField _email;

		[SerializeField]
		private TMP_InputField _description;

		[SerializeField]
		private ModioUIButton _disableWhenInvalidToSubmit;

		private ModioUIMod _modioUIMod;

		private Mod _lastMod;
	}
}
