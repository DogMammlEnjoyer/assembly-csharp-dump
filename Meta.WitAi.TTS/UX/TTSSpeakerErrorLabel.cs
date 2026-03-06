using System;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.WitAi.TTS.UX
{
	public class TTSSpeakerErrorLabel : TTSSpeakerObserver
	{
		protected override void Awake()
		{
			base.Awake();
			if (this._errorLabel == null)
			{
				this._errorLabel = base.gameObject.GetComponentInChildren<Text>();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshError();
		}

		protected override void OnLoadBegin(TTSSpeaker speaker, TTSClipData clipData)
		{
			base.OnLoadBegin(speaker, clipData);
			this._lastLoadError = null;
			this.RefreshError();
		}

		protected override void OnLoadFailed(TTSSpeaker speaker, TTSClipData clipData, string error)
		{
			base.OnLoadFailed(speaker, clipData, error);
			this._lastLoadError = error;
			this.RefreshError();
		}

		public void RefreshError()
		{
			string currentError = this.GetCurrentError();
			if (string.Equals(this._lastError, currentError, StringComparison.CurrentCulture))
			{
				return;
			}
			this._lastError = currentError;
			if (this._errorLabel)
			{
				if (string.IsNullOrEmpty(this._lastError))
				{
					this._errorLabel.text = string.Empty;
					return;
				}
				this._errorLabel.text = "Error: " + this._lastError;
			}
		}

		public string GetCurrentError()
		{
			if (base.Speaker == null)
			{
				return "No TTS Speaker found";
			}
			if (base.Speaker.TTSService == null)
			{
				return "No TTS Service found on speaker";
			}
			string invalidError = base.Speaker.TTSService.GetInvalidError();
			if (!string.IsNullOrEmpty(invalidError))
			{
				return invalidError;
			}
			if (!string.IsNullOrEmpty(this._lastLoadError))
			{
				return this._lastLoadError;
			}
			return string.Empty;
		}

		[SerializeField]
		private Text _errorLabel;

		private string _lastError;

		private string _lastLoadError;
	}
}
