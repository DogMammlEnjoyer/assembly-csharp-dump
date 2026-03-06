using System;
using System.Collections;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Meta.WitAi.TTS.UX
{
	public class TTSSpeakerInput : MonoBehaviour
	{
		private void OnEnable()
		{
			this.RefreshStopButton();
			this.RefreshPauseButton();
			this._stopButton.onClick.AddListener(new UnityAction(this.StopClick));
			this._pauseButton.onClick.AddListener(new UnityAction(this.PauseClick));
			this._speakButton.onClick.AddListener(new UnityAction(this.SpeakClick));
		}

		private void StopClick()
		{
			this._speaker.Stop();
		}

		private void PauseClick()
		{
			if (this._speaker.IsPaused)
			{
				this._speaker.Resume();
				return;
			}
			this._speaker.Pause();
		}

		private void SpeakClick()
		{
			string text = this.FormatText(this._input.text);
			bool flag = this._queueButton != null && this._queueButton.isOn;
			if (this._asyncToggle != null && this._asyncToggle.isOn)
			{
				base.StartCoroutine(this.SpeakAsync(text, flag));
			}
			else if (flag)
			{
				this._speaker.SpeakQueued(text);
			}
			else
			{
				this._speaker.Speak(text);
			}
			if (this._queuedText != null && this._queuedText.Length != 0 && flag)
			{
				foreach (string text2 in this._queuedText)
				{
					this._speaker.SpeakQueued(this.FormatText(text2));
				}
			}
		}

		private IEnumerator SpeakAsync(string phrase, bool queued)
		{
			if (queued)
			{
				yield return this._speaker.SpeakQueuedAsync(new string[]
				{
					phrase
				});
			}
			else
			{
				yield return this._speaker.SpeakAsync(phrase);
			}
			if (this._asyncClip != null)
			{
				this._speaker.AudioSource.PlayOneShot(this._asyncClip);
			}
			yield break;
		}

		private string FormatText(string text)
		{
			string text2 = text;
			if (text2.Contains(this._dateId))
			{
				DateTime utcNow = DateTime.UtcNow;
				string newValue = utcNow.ToLongDateString() + " at " + utcNow.ToLongTimeString();
				text2 = text.Replace(this._dateId, newValue);
			}
			return text2;
		}

		private void OnDisable()
		{
			this._stopButton.onClick.RemoveListener(new UnityAction(this.StopClick));
			this._speakButton.onClick.RemoveListener(new UnityAction(this.SpeakClick));
		}

		private void Update()
		{
			if (!string.Equals(this._voice, this._speaker.VoiceID))
			{
				this._voice = this._speaker.VoiceID;
				this._input.placeholder.GetComponent<Text>().text = "Write something to say in " + this._voice + "'s voice";
			}
			if (this._loading != this._speaker.IsLoading)
			{
				this.RefreshStopButton();
			}
			if (this._speaking != this._speaker.IsSpeaking)
			{
				this.RefreshStopButton();
			}
			if (this._paused != this._speaker.IsPaused)
			{
				this.RefreshPauseButton();
			}
		}

		private void RefreshStopButton()
		{
			this._loading = this._speaker.IsLoading;
			this._speaking = this._speaker.IsSpeaking;
			this._stopButton.interactable = (this._loading || this._speaking);
		}

		private void RefreshPauseButton()
		{
			this._paused = this._speaker.IsPaused;
			this._pauseButton.GetComponentInChildren<Text>().text = (this._paused ? "Resume" : "Pause");
		}

		[SerializeField]
		private TTSSpeaker _speaker;

		[SerializeField]
		private InputField _input;

		[SerializeField]
		private Button _stopButton;

		[SerializeField]
		private Button _pauseButton;

		[SerializeField]
		private Button _speakButton;

		[SerializeField]
		private Toggle _queueButton;

		[SerializeField]
		private Toggle _asyncToggle;

		[SerializeField]
		private AudioClip _asyncClip;

		[SerializeField]
		private string _dateId = "[DATE]";

		[SerializeField]
		private string[] _queuedText;

		private string _voice;

		private bool _loading;

		private bool _speaking;

		private bool _paused;
	}
}
