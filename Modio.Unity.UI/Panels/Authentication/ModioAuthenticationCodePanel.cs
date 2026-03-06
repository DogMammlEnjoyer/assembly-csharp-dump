using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels.Authentication
{
	public class ModioAuthenticationCodePanel : ModioPanelBase
	{
		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			if (selectionBehaviour == ModioPanelBase.GainedFocusCause.OpeningFromClosed)
			{
				this._codeField.text = "";
			}
			base.OnGainedFocus(selectionBehaviour);
		}

		public void OpenPanel(string emailFieldText, Action<string> codeCallback)
		{
			if (this._emailDisplay != null)
			{
				this._emailDisplay.text = emailFieldText;
			}
			this._codeCallback = codeCallback;
			base.OpenPanel();
		}

		public void OnPressSubmitCode()
		{
			base.ClosePanel();
			ModioPanelManager.GetPanelOfType<ModioAuthenticationWaitingPanel>().OpenPanel();
			this._codeCallback(this._codeField.text);
		}

		protected override void CancelPressed()
		{
			this.OnPressUseAnotherEmail();
			base.CancelPressed();
		}

		public void OnPressUseAnotherEmail()
		{
			base.ClosePanel();
			Action<string> codeCallback = this._codeCallback;
			if (codeCallback != null)
			{
				codeCallback(string.Empty);
			}
			ModioAuthenticationIEmailPanel panelOfType = ModioPanelManager.GetPanelOfType<ModioAuthenticationIEmailPanel>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.OpenPanel();
		}

		public void OnPressCancel()
		{
			base.ClosePanel();
			this._codeCallback(string.Empty);
		}

		[SerializeField]
		private TMP_InputField _codeField;

		[SerializeField]
		private TMP_Text _emailDisplay;

		[SerializeField]
		private UnityEvent<Error> _onError;

		private Action<string> _codeCallback;
	}
}
