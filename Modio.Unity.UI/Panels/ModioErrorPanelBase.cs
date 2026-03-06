using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Errors;
using Modio.Unity.UI.Components.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Modio.Unity.UI.Panels
{
	public abstract class ModioErrorPanelBase : ModioPanelBase
	{
		public void OpenPanel(Error error)
		{
			base.OpenPanel();
			ModioErrorPanelBase.ErrorMessageResponse[] errorMessageResponses = this._errorMessageResponses;
			int i = 0;
			while (i < errorMessageResponses.Length)
			{
				ModioErrorPanelBase.ErrorMessageResponse errorMessageResponse = errorMessageResponses[i];
				if (errorMessageResponse.errorCode.Contains((long)error.Code) || (errorMessageResponse.apiCode.Count != 0 && errorMessageResponse.apiCode.Contains((long)error.Code)))
				{
					RateLimitError rateLimitError = error as RateLimitError;
					if (rateLimitError != null)
					{
						this.OpenPanel(errorMessageResponse, new object[]
						{
							rateLimitError.RetryAfterSeconds
						});
						return;
					}
					this.OpenPanel(errorMessageResponse, Array.Empty<object>());
					return;
				}
				else
				{
					i++;
				}
			}
			if (this._errorCode != null)
			{
				this._errorCode.text = string.Format("[Error code: {0}]", error.Code);
			}
			if (this._errorMessageLocalised != null)
			{
				string keyIfItExists = "modio_error_description_api_" + error.Code.ToString();
				if (!this._errorMessageLocalised.SetKeyIfItExists(keyIfItExists))
				{
					this._errorMessageLocalised.ResetKey();
				}
			}
			if (this._errorCodeLocalised != null)
			{
				this._errorCodeLocalised.gameObject.SetActive(true);
				string text = error.Code.ToString();
				this._errorCodeLocalised.SetFormatArgs(new object[]
				{
					text
				});
			}
			if (this._showWhenActionProvided != null)
			{
				this._showWhenActionProvided.SetActive(false);
			}
			if (this._titleLocalised != null)
			{
				this._titleLocalised.ResetKey();
			}
			this._action = null;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose == null)
			{
				return;
			}
			verbose.Log(string.Format("Showing error for response {0}", error));
		}

		public void OpenPanel(string message)
		{
			base.OpenPanel();
			this._action = null;
			if (this._showWhenActionProvided != null)
			{
				this._showWhenActionProvided.SetActive(false);
			}
			if (this._titleLocalised != null)
			{
				this._titleLocalised.ResetKey();
			}
			if (this._errorCode != null)
			{
				this._errorCode.text = message;
			}
		}

		public void OpenPanel(ModioErrorPanelBase.ErrorMessageResponse response, params object[] args)
		{
			if (this._titleLocalised != null)
			{
				this._titleLocalised.SetKey(response.windowTitleLocalised);
			}
			if (this._errorMessageLocalised != null)
			{
				this._errorMessageLocalised.SetKey(response.windowMessageLocalised, args);
			}
			if (this._actionMessageLocalised != null)
			{
				this._actionMessageLocalised.SetKey(response.actionPromptLocalised);
			}
			this._useLocalizedActionPrompt = !string.IsNullOrEmpty(response.actionPromptLocalised);
			if (this._showWhenActionProvided != null)
			{
				this._showWhenActionProvided.SetActive(this._useLocalizedActionPrompt);
			}
			if (this._errorCodeLocalised != null)
			{
				this._errorCodeLocalised.gameObject.SetActive(false);
			}
			this._action = new Action(response.onActionPressed.Invoke);
		}

		protected override void CancelPressed()
		{
			if (!this._useLocalizedActionPrompt)
			{
				Action action = this._action;
				if (action != null)
				{
					action();
				}
			}
			base.CancelPressed();
		}

		public Task MonitorTaskThenOpenPanelIfError(Task<Error> task)
		{
			ModioErrorPanelBase.<MonitorTaskThenOpenPanelIfError>d__14 <MonitorTaskThenOpenPanelIfError>d__;
			<MonitorTaskThenOpenPanelIfError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<MonitorTaskThenOpenPanelIfError>d__.<>4__this = this;
			<MonitorTaskThenOpenPanelIfError>d__.task = task;
			<MonitorTaskThenOpenPanelIfError>d__.<>1__state = -1;
			<MonitorTaskThenOpenPanelIfError>d__.<>t__builder.Start<ModioErrorPanelBase.<MonitorTaskThenOpenPanelIfError>d__14>(ref <MonitorTaskThenOpenPanelIfError>d__);
			return <MonitorTaskThenOpenPanelIfError>d__.<>t__builder.Task;
		}

		public void InvokeAction()
		{
			if (this._action != null)
			{
				this._action();
			}
			base.ClosePanel();
		}

		[SerializeField]
		private ModioUILocalizedText _titleLocalised;

		[SerializeField]
		private TMP_Text _errorCode;

		[SerializeField]
		private ModioUILocalizedText _errorCodeLocalised;

		[SerializeField]
		private ModioUILocalizedText _errorMessageLocalised;

		[SerializeField]
		private GameObject _showWhenActionProvided;

		[SerializeField]
		private ModioUILocalizedText _actionMessageLocalised;

		[SerializeField]
		private ModioErrorPanelBase.ErrorMessageResponse[] _errorMessageResponses;

		private Action _action;

		private bool _useLocalizedActionPrompt;

		[Serializable]
		public class ErrorMessageResponse
		{
			public List<long> errorCode;

			public List<long> apiCode;

			public string windowTitleLocalised;

			public string windowMessageLocalised;

			public string actionPromptLocalised;

			public UnityEvent onActionPressed;
		}
	}
}
