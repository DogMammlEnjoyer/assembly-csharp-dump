using System;
using Modio.Mods;
using Modio.Unity.UI.Components;
using Modio.Unity.UI.Input;
using Modio.Unity.UI.Panels.Report;
using Modio.Unity.UI.Search;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels
{
	public class ModDisplayPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._modioUIMod = base.GetComponent<ModioUIMod>();
		}

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			base.OnGainedFocus(selectionBehaviour);
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Report, new Action(this.ReportPressed));
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.MoreFromThisCreator, new Action(this.MoreFromCreatorPressed));
			if (this._onMoreOptionsPressed.GetPersistentEventCount() > 0)
			{
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.MoreOptions, new Action(this.MoreOptionsPressed));
			}
		}

		public override void OnLostFocus()
		{
			base.OnLostFocus();
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Report, new Action(this.ReportPressed));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.MoreOptions, new Action(this.MoreOptionsPressed));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.MoreFromThisCreator, new Action(this.MoreFromCreatorPressed));
		}

		public void OpenPanel(Mod mod)
		{
			base.OpenPanel();
			this._modioUIMod.SetMod(mod);
		}

		private void ReportPressed()
		{
			ModioPanelManager.GetPanelOfType<ModioReportPanel>().OpenReportFlow(this._modioUIMod.Mod);
		}

		private void MoreOptionsPressed()
		{
			this._onMoreOptionsPressed.Invoke();
		}

		private void MoreFromCreatorPressed()
		{
			ModioUISearch.Default.SetSearchForUser(this._modioUIMod.Mod.Creator);
			base.ClosePanel();
		}

		private ModioUIMod _modioUIMod;

		[SerializeField]
		private UnityEvent _onMoreOptionsPressed;
	}
}
