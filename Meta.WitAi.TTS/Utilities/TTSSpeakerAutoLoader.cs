using System;
using System.Collections.Generic;
using Meta.WitAi.TTS.Data;
using UnityEngine;

namespace Meta.WitAi.TTS.Utilities
{
	[RequireComponent(typeof(TTSSpeaker))]
	public class TTSSpeakerAutoLoader : MonoBehaviour, ITTSPhraseProvider
	{
		public string[] Phrases
		{
			get
			{
				return this._phrases;
			}
		}

		public TTSClipData[] Clips
		{
			get
			{
				return this._clips;
			}
		}

		public bool IsLoaded
		{
			get
			{
				return this._clipsLoading == 0;
			}
		}

		protected virtual void Start()
		{
			if (!this.LoadManually)
			{
				this.LoadClips();
			}
		}

		public virtual void LoadClips()
		{
			if (this._clips != null)
			{
				VLog.W("Cannot autoload clips twice.", null);
				return;
			}
			this._phrases = this.GetAllPhrases().ToArray();
			List<TTSClipData> list = new List<TTSClipData>();
			foreach (string textToSpeak in this._phrases)
			{
				this._clipsLoading++;
				TTSClipData item = TTSService.Instance.Load(textToSpeak, this.Speaker.VoiceID, null, null, new Action<TTSClipData, string>(this.OnClipReady));
				list.Add(item);
			}
			this._clips = list.ToArray();
		}

		public virtual List<string> GetAllPhrases()
		{
			this.SetupSpeaker();
			List<string> list = new List<string>();
			List<string> list2 = list;
			TextAsset phraseFile = this.PhraseFile;
			this.AddUniquePhrases(list2, (phraseFile != null) ? phraseFile.text.Split('\n', StringSplitOptions.None) : null);
			this.AddUniquePhrases(list, this.Phrases);
			List<string> list3 = new List<string>();
			for (int i = 0; i < list.Count; i++)
			{
				List<string> finalText = this.Speaker.GetFinalText(list[i]);
				if (finalText != null && finalText.Count > 0)
				{
					list3.AddRange(finalText);
				}
			}
			return list3;
		}

		private void AddUniquePhrases(List<string> list, string[] newPhrases)
		{
			if (newPhrases != null)
			{
				foreach (string text in newPhrases)
				{
					if (!string.IsNullOrEmpty(text) && !list.Contains(text))
					{
						list.Add(text);
					}
				}
			}
		}

		protected virtual void SetupSpeaker()
		{
			if (!this.Speaker)
			{
				this.Speaker = base.gameObject.GetComponent<TTSSpeaker>();
				if (!this.Speaker)
				{
					this.Speaker = base.gameObject.AddComponent<TTSSpeaker>();
				}
			}
		}

		protected virtual void OnClipReady(TTSClipData clipData, string error)
		{
			this._clipsLoading--;
		}

		protected virtual void OnDestroy()
		{
			this.UnloadClips();
		}

		protected virtual void UnloadClips()
		{
			if (this._clips == null)
			{
				return;
			}
			foreach (TTSClipData clipData in this._clips)
			{
				TTSService instance = TTSService.Instance;
				if (instance != null)
				{
					instance.Unload(clipData);
				}
			}
			this._clips = null;
			this._phrases = null;
		}

		public virtual List<string> GetVoiceIds()
		{
			this.SetupSpeaker();
			TTSSpeaker speaker = this.Speaker;
			string text = (speaker != null) ? speaker.VoiceSettings.SettingsId : null;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new List<string>
			{
				text
			};
		}

		public virtual List<string> GetVoicePhrases(string voiceId)
		{
			return this.GetAllPhrases();
		}

		public TTSSpeaker Speaker;

		public TextAsset PhraseFile;

		[SerializeField]
		private string[] _phrases;

		public bool LoadManually;

		private TTSClipData[] _clips;

		private int _clipsLoading;
	}
}
