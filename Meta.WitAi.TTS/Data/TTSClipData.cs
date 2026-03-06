using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meta.Voice.Audio;
using UnityEngine;

namespace Meta.WitAi.TTS.Data
{
	[Serializable]
	public class TTSClipData
	{
		public string queryRequestId { get; } = WitConstants.GetUniqueId();

		public IAudioClipStream clipStream
		{
			get
			{
				return this._clipStream;
			}
			set
			{
				if (this._clipStream != null && this._clipStream != value)
				{
					this.clipStream.OnStreamReady = null;
					this.clipStream.OnStreamUpdated = null;
					this.clipStream.OnStreamComplete = null;
					this._clipStream.Unload();
				}
				this._clipStream = value;
			}
		}

		public AudioClip clip
		{
			get
			{
				IAudioClipProvider audioClipProvider = this.clipStream as IAudioClipProvider;
				if (audioClipProvider != null)
				{
					return audioClipProvider.Clip;
				}
				return null;
			}
		}

		public TTSEventContainer Events { get; } = new TTSEventContainer();

		public int LoadStatusCode { get; set; }

		public string LoadError { get; set; }

		public TaskCompletionSource<bool> LoadReady { get; } = new TaskCompletionSource<bool>();

		public TaskCompletionSource<bool> LoadCompletion { get; } = new TaskCompletionSource<bool>();

		public override bool Equals(object obj)
		{
			TTSClipData ttsclipData = obj as TTSClipData;
			return ttsclipData != null && this.Equals(ttsclipData);
		}

		public bool Equals(TTSClipData other)
		{
			return this.HasClipId((other != null) ? other.clipID : null);
		}

		public bool HasClipId(string clipId)
		{
			return string.Equals(this.clipID, clipId, StringComparison.CurrentCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			return 17 * 31 + this.clipID.GetHashCode();
		}

		public override string ToString()
		{
			string format = "Text: {0}\nVoice: {1}\nClip Id: {2}\nType: {3}\nStream: {4}\nEvents: {5}\nAudio Length: {6:0.00} seconds";
			object[] array = new object[7];
			array[0] = this.textToSpeak;
			int num = 1;
			TTSVoiceSettings ttsvoiceSettings = this.voiceSettings;
			array[num] = (((ttsvoiceSettings != null) ? ttsvoiceSettings.SettingsId : null) ?? "Null");
			array[2] = this.clipID;
			array[3] = this.extension;
			array[4] = this.queryStream;
			int num2 = 5;
			TTSEventContainer events = this.Events;
			int? num3;
			if (events == null)
			{
				num3 = null;
			}
			else
			{
				IEnumerable<ITTSEvent> events2 = events.Events;
				num3 = ((events2 != null) ? new int?(events2.Count<ITTSEvent>()) : null);
			}
			int? num4 = num3;
			array[num2] = num4.GetValueOrDefault();
			int num5 = 6;
			IAudioClipStream clipStream = this.clipStream;
			array[num5] = ((clipStream != null) ? clipStream.Length : 0f);
			return string.Format(format, array);
		}

		public string textToSpeak;

		public string clipID;

		[Obsolete("Use extension directly.")]
		public AudioType audioType;

		public TTSVoiceSettings voiceSettings;

		public TTSDiskCacheSettings diskCacheSettings;

		public string queryOperationId;

		public bool queryStream;

		public Dictionary<string, string> queryParameters;

		private IAudioClipStream _clipStream;

		[NonSerialized]
		public TTSClipLoadState loadState;

		[NonSerialized]
		public float loadProgress;

		[NonSerialized]
		public float readyDuration;

		[NonSerialized]
		public float completeDuration;

		public Action<TTSClipData, TTSClipLoadState> onStateChange;

		public bool useEvents;

		public string extension;

		public Action<TTSClipData> onPlaybackReady;

		public Action<string> onDownloadComplete;

		public Action<TTSClipData> onRequestBegin;

		public Action<TTSClipData> onRequestComplete;

		public Action<TTSClipData> onPlaybackQueued;

		public Action<TTSClipData> onPlaybackBegin;

		public Action<TTSClipData> onPlaybackComplete;
	}
}
