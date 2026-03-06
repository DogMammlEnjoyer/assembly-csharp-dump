using System;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.VoiceSDK.UX
{
	[RequireComponent(typeof(Button))]
	public class VoiceActivationButton : MonoBehaviour
	{
		private void Awake()
		{
			this._buttonLabel = base.GetComponentInChildren<Text>();
			this._button = base.GetComponent<Button>();
			if (this._voiceService == null)
			{
				this._voiceService = Object.FindAnyObjectByType<VoiceService>();
			}
		}

		private void OnEnable()
		{
			this.RefreshActive();
			if (this._voiceService != null)
			{
				this._voiceService.VoiceEvents.OnStartListening.AddListener(new UnityAction(this.OnStartListening));
				this._voiceService.VoiceEvents.OnStoppedListening.AddListener(new UnityAction(this.OnStopListening));
			}
			if (this._button != null)
			{
				this._button.onClick.AddListener(new UnityAction(this.OnClick));
			}
		}

		private void OnDisable()
		{
			this._isActive = false;
			if (this._voiceService != null)
			{
				this._voiceService.VoiceEvents.OnStartListening.RemoveListener(new UnityAction(this.OnStartListening));
				this._voiceService.VoiceEvents.OnStoppedListening.RemoveListener(new UnityAction(this.OnStopListening));
			}
			if (this._button != null)
			{
				this._button.onClick.RemoveListener(new UnityAction(this.OnClick));
			}
		}

		private void OnClick()
		{
			if (!this._isActive)
			{
				this.Activate();
				return;
			}
			this.Deactivate();
		}

		private void Activate()
		{
			if (!this._activateImmediately)
			{
				this._request = this._voiceService.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), new VoiceServiceRequestEvents());
				return;
			}
			this._request = this._voiceService.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), new VoiceServiceRequestEvents());
		}

		private void Deactivate()
		{
			if (this._deactivateAndAbort)
			{
				if (this._request != null)
				{
					this._request.Cancel("Request was cancelled.");
				}
				return;
			}
			if (this._request != null)
			{
				this._request.DeactivateAudio();
				return;
			}
			this._voiceService.Deactivate();
		}

		private void OnStartListening()
		{
			this._isActive = true;
			this.RefreshActive();
		}

		private void OnStopListening()
		{
			this._isActive = false;
			this._request = null;
			this.RefreshActive();
		}

		private void RefreshActive()
		{
			if (this._buttonLabel != null)
			{
				this._buttonLabel.text = (this._isActive ? this._deactivateText : this._activateText);
			}
		}

		[Tooltip("Reference to the current voice service")]
		[SerializeField]
		private VoiceService _voiceService;

		[Tooltip("Text to be shown while the voice service is not actively recording")]
		[SerializeField]
		private string _activateText = "Activate";

		[Tooltip("Whether to immediately send data to service or to wait for the audio threshold")]
		[SerializeField]
		private bool _activateImmediately;

		[Tooltip("Text to be shown while the voice service is actively recording")]
		[SerializeField]
		private string _deactivateText = "Deactivate";

		[Tooltip("Whether to immediately abort request activation on deactivate")]
		[SerializeField]
		private bool _deactivateAndAbort;

		private Button _button;

		private Text _buttonLabel;

		private VoiceServiceRequest _request;

		private bool _isActive;
	}
}
