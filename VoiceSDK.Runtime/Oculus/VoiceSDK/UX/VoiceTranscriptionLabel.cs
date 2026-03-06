using System;
using Meta.WitAi;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Oculus.VoiceSDK.UX
{
	[RequireComponent(typeof(Text))]
	[ExecuteInEditMode]
	public class VoiceTranscriptionLabel : MonoBehaviour
	{
		public Text Label
		{
			get
			{
				if (this._label == null)
				{
					this._label = base.gameObject.GetComponent<Text>();
				}
				return this._label;
			}
		}

		private void Awake()
		{
			if (this._voiceServices == null || this._voiceServices.Length == 0)
			{
				this._voiceServices = Object.FindObjectsByType<VoiceService>(FindObjectsSortMode.None);
			}
		}

		private void OnEnable()
		{
			if (this._voiceServices != null)
			{
				foreach (VoiceService voiceService in this._voiceServices)
				{
					voiceService.VoiceEvents.OnStartListening.AddListener(new UnityAction(this.OnStartListening));
					voiceService.VoiceEvents.OnPartialTranscription.AddListener(new UnityAction<string>(this.OnTranscriptionChange));
					voiceService.VoiceEvents.OnFullTranscription.AddListener(new UnityAction<string>(this.OnTranscriptionChange));
					voiceService.VoiceEvents.OnError.AddListener(new UnityAction<string, string>(this.OnError));
					voiceService.VoiceEvents.OnComplete.AddListener(new UnityAction<VoiceServiceRequest>(this.OnComplete));
				}
			}
		}

		private void OnDisable()
		{
			if (this._voiceServices != null)
			{
				foreach (VoiceService voiceService in this._voiceServices)
				{
					voiceService.VoiceEvents.OnStartListening.RemoveListener(new UnityAction(this.OnStartListening));
					voiceService.VoiceEvents.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.OnTranscriptionChange));
					voiceService.VoiceEvents.OnFullTranscription.RemoveListener(new UnityAction<string>(this.OnTranscriptionChange));
					voiceService.VoiceEvents.OnError.RemoveListener(new UnityAction<string, string>(this.OnError));
					voiceService.VoiceEvents.OnComplete.RemoveListener(new UnityAction<VoiceServiceRequest>(this.OnComplete));
				}
			}
		}

		private void OnStartListening()
		{
			this.SetText(this._promptListening, this._promptColor);
		}

		private void OnTranscriptionChange(string text)
		{
			this.SetText(text, this._transcriptionColor);
		}

		private void OnError(string status, string error)
		{
			this.SetText("[" + status + "] " + error, this._errorColor);
		}

		private void OnComplete(VoiceServiceRequest request)
		{
			if (this.Label != null)
			{
				Text label = this.Label;
				if (string.Equals((label != null) ? label.text : null, this._promptListening))
				{
					this.SetText(this._promptDefault, this._promptColor);
				}
			}
		}

		private void SetText(string newText, Color newColor)
		{
			if (this.Label == null || (string.Equals(newText, this.Label.text) && newColor == this.Label.color))
			{
				return;
			}
			this._label.text = newText;
			this._label.color = newColor;
		}

		private Text _label;

		[Header("Listen Settings")]
		[Tooltip("Various voice services to be observed")]
		[SerializeField]
		private VoiceService[] _voiceServices;

		[Tooltip("Text color while receiving text")]
		[SerializeField]
		private Color _transcriptionColor = Color.black;

		[Header("Prompt Settings")]
		[Tooltip("Color to be used for prompt text")]
		[SerializeField]
		private Color _promptColor = new Color(0.2f, 0.2f, 0.2f);

		[Tooltip("Prompt text that displays while listening but prior to completion")]
		[SerializeField]
		private string _promptDefault = "Press activate to begin listening";

		[Tooltip("Prompt text that displays while listening but prior to completion")]
		[SerializeField]
		private string _promptListening = "Listening...";

		[Header("Error Settings")]
		[Tooltip("Color to be used for error text")]
		[SerializeField]
		private Color _errorColor = new Color(0.8f, 0.2f, 0.2f);
	}
}
