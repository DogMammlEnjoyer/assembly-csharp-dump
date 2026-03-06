using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Audio;
using Meta.Voice.Logging;
using Meta.WitAi.Json;
using Meta.WitAi.Speech;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Integrations;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.Utilities
{
	[LogCategory(LogCategory.TextToSpeech)]
	public class TTSSpeaker : MonoBehaviour, ISpeechEventProvider, ISpeaker, ITTSEventPlayer, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.TextToSpeech, null);

		public TTSSpeakerEvents Events
		{
			get
			{
				return this._events;
			}
		}

		public VoiceSpeechEvents SpeechEvents
		{
			get
			{
				return this._events;
			}
		}

		public TTSService TTSService
		{
			get
			{
				if (!this._ttsService)
				{
					this._ttsService = base.GetComponent<TTSService>();
					if (!this._ttsService)
					{
						this._ttsService = TTSService.Instance;
					}
				}
				return this._ttsService;
			}
		}

		public string VoiceID
		{
			get
			{
				return this.presetVoiceID;
			}
			set
			{
				this.presetVoiceID = value;
			}
		}

		public TTSVoiceSettings VoiceSettings
		{
			get
			{
				if (this._isPlaying && this._overrideVoiceSettings != null)
				{
					return this._overrideVoiceSettings;
				}
				TTSVoiceSettings ttsvoiceSettings = string.IsNullOrEmpty(this.presetVoiceID) ? null : this.TTSService.GetPresetVoiceSettings(this.presetVoiceID);
				if (ttsvoiceSettings != null)
				{
					return ttsvoiceSettings;
				}
				return this.customWitVoiceSettings;
			}
		}

		public bool IsSpeaking
		{
			get
			{
				return this.SpeakingClip != null;
			}
		}

		public TTSClipData SpeakingClip
		{
			get
			{
				TTSSpeaker.TTSSpeakerRequestData speakingRequest = this._speakingRequest;
				if (speakingRequest == null)
				{
					return null;
				}
				return speakingRequest.ClipData;
			}
		}

		public bool IsLoading
		{
			get
			{
				return this._queuedRequests.Count > 0;
			}
		}

		public bool IsPreparing
		{
			get
			{
				foreach (TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData in this._queuedRequests)
				{
					if (ttsspeakerRequestData.ClipData != null && ttsspeakerRequestData.ClipData.loadState == TTSClipLoadState.Preparing)
					{
						return true;
					}
				}
				return false;
			}
		}

		public List<TTSClipData> QueuedClips
		{
			get
			{
				List<TTSClipData> list = new List<TTSClipData>();
				foreach (TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData in this._queuedRequests)
				{
					list.Add(ttsspeakerRequestData.ClipData);
				}
				return list;
			}
		}

		public bool IsActive
		{
			get
			{
				return this.IsSpeaking || this.IsLoading;
			}
		}

		public IAudioPlayer AudioPlayer
		{
			get
			{
				if (this._audioPlayer == null)
				{
					this._audioPlayer = base.gameObject.GetComponent<IAudioPlayer>();
					if (this._audioPlayer == null)
					{
						TTSService ttsservice = this.TTSService;
						IAudioPlayer audioPlayer;
						if (ttsservice == null)
						{
							audioPlayer = null;
						}
						else
						{
							IAudioSystem audioSystem = ttsservice.AudioSystem;
							audioPlayer = ((audioSystem != null) ? audioSystem.GetAudioPlayer(base.gameObject) : null);
						}
						this._audioPlayer = audioPlayer;
						if (this._audioPlayer == null)
						{
							this._audioPlayer = base.gameObject.AddComponent<UnityAudioPlayer>();
						}
					}
				}
				return this._audioPlayer;
			}
		}

		public AudioSource AudioSource
		{
			get
			{
				IAudioSourceProvider audioSourceProvider = this.AudioPlayer as IAudioSourceProvider;
				if (audioSourceProvider != null)
				{
					return audioSourceProvider.AudioSource;
				}
				return null;
			}
		}

		protected virtual void Start()
		{
			this.AudioPlayer.Init();
		}

		protected virtual void OnDestroy()
		{
			this.Stop();
			this._speakingRequest = null;
			List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
			lock (queuedRequests)
			{
				this._queuedRequests.Clear();
			}
		}

		protected virtual void OnEnable()
		{
			this._isPlaying = Application.isPlaying;
			if (this._textPreprocessors == null)
			{
				this._textPreprocessors = base.GetComponents<ISpeakerTextPreprocessor>();
			}
			if (this._textPostprocessors == null)
			{
				this._textPostprocessors = base.GetComponents<ISpeakerTextPostprocessor>();
			}
			if (!string.IsNullOrEmpty(this.PrependedText) && this.PrependedText.Length > 0 && !this.PrependedText.EndsWith(" "))
			{
				this.PrependedText += " ";
			}
			if (!string.IsNullOrEmpty(this.AppendedText) && this.AppendedText.Length > 0 && !this.AppendedText.StartsWith(" "))
			{
				this.AppendedText = " " + this.AppendedText;
			}
			if (this.TTSService)
			{
				this.TTSService.Events.OnClipUnloaded.AddListener(new UnityAction<TTSClipData>(this.StopAndUnloadClip));
			}
		}

		protected virtual void OnDisable()
		{
			this.Stop();
			if (this.TTSService)
			{
				this.TTSService.Events.OnClipUnloaded.RemoveListener(new UnityAction<TTSClipData>(this.StopAndUnloadClip));
			}
		}

		protected virtual void StopAndUnloadClip(TTSClipData clipData)
		{
			this.Stop(clipData, true);
		}

		private TTSSpeaker.TTSSpeakerRequestData GetFirstQueuedRequest(TTSClipData clipData)
		{
			if (this._queuedRequests != null)
			{
				foreach (TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData in this._queuedRequests)
				{
					string a = (clipData != null) ? clipData.clipID : null;
					string b;
					if (ttsspeakerRequestData == null)
					{
						b = null;
					}
					else
					{
						TTSClipData clipData2 = ttsspeakerRequestData.ClipData;
						b = ((clipData2 != null) ? clipData2.clipID : null);
					}
					if (string.Equals(a, b))
					{
						return ttsspeakerRequestData;
					}
				}
			}
			return null;
		}

		private TTSSpeaker.TTSSpeakerRequestData GetFirstQueuedRequest(string textToSpeak)
		{
			if (this._queuedRequests != null)
			{
				foreach (TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData in this._queuedRequests)
				{
					string b;
					if (ttsspeakerRequestData == null)
					{
						b = null;
					}
					else
					{
						TTSClipData clipData = ttsspeakerRequestData.ClipData;
						b = ((clipData != null) ? clipData.textToSpeak : null);
					}
					if (string.Equals(textToSpeak, b))
					{
						return ttsspeakerRequestData;
					}
				}
			}
			return null;
		}

		private static bool RequestEquals(TTSSpeaker.TTSSpeakerRequestData requestData1, TTSSpeaker.TTSSpeakerRequestData requestData2)
		{
			return requestData1 != null && requestData2 != null && requestData1.Equals(requestData2);
		}

		private static bool RequestHasClipData(TTSSpeaker.TTSSpeakerRequestData requestData, TTSClipData clipData)
		{
			string a;
			if (requestData == null)
			{
				a = null;
			}
			else
			{
				TTSClipData clipData2 = requestData.ClipData;
				a = ((clipData2 != null) ? clipData2.clipID : null);
			}
			string b = (clipData != null) ? clipData.clipID : null;
			return string.Equals(a, b);
		}

		private static bool RequestHasClipText(TTSSpeaker.TTSSpeakerRequestData requestData, string textToSpeak)
		{
			string a;
			if (requestData == null)
			{
				a = null;
			}
			else
			{
				TTSClipData clipData = requestData.ClipData;
				a = ((clipData != null) ? clipData.textToSpeak : null);
			}
			return string.Equals(a, textToSpeak);
		}

		private void RefreshQueueEvents()
		{
			bool flag = this.IsActive || this._queueNotYetComplete;
			if (this._hasQueue != flag)
			{
				this._hasQueue = flag;
				if (this._hasQueue)
				{
					this.RaiseEvents(new Action(this.RaiseOnPlaybackQueueBegin));
					return;
				}
				this.RaiseEvents(new Action(this.RaiseOnPlaybackQueueComplete));
			}
		}

		private bool IsClipRequestActive(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			return this.IsClipRequestLoading(requestData) || this.IsClipRequestSpeaking(requestData);
		}

		private bool IsClipRequestLoading(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			return this._queuedRequests.Contains(requestData);
		}

		private bool IsClipRequestSpeaking(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			return this._speakingRequest != null && this._speakingRequest.Equals(requestData);
		}

		public List<string> GetFinalText(string textToSpeak)
		{
			List<string> list = new List<string>();
			list.Add(textToSpeak);
			if (this._textPreprocessors != null)
			{
				ISpeakerTextPreprocessor[] textPreprocessors = this._textPreprocessors;
				for (int i = 0; i < textPreprocessors.Length; i++)
				{
					textPreprocessors[i].OnPreprocessTTS(this, list);
				}
			}
			if (!string.IsNullOrEmpty(this.PrependedText) || !string.IsNullOrEmpty(this.AppendedText))
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (!string.IsNullOrEmpty(list[j].Trim()))
					{
						string text = list[j];
						text = (this.PrependedText + text + this.AppendedText).Trim();
						list[j] = text;
					}
				}
			}
			if (this._textPostprocessors != null)
			{
				ISpeakerTextPostprocessor[] textPostprocessors = this._textPostprocessors;
				for (int i = 0; i < textPostprocessors.Length; i++)
				{
					textPostprocessors[i].OnPostprocessTTS(this, list);
				}
			}
			return list;
		}

		public List<string> GetFinalTextFormatted(string format, params string[] textsToSpeak)
		{
			return this.GetFinalText(this.GetFormattedText(format, textsToSpeak));
		}

		public string GetFormattedText(string format, params string[] textsToSpeak)
		{
			if (textsToSpeak != null && !string.IsNullOrEmpty(format))
			{
				object[] array = new object[textsToSpeak.Length];
				textsToSpeak.CopyTo(array, 0);
				return string.Format(format, array);
			}
			return null;
		}

		public void Speak(string textToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			this.Load(textToSpeak, null, diskCacheSettings, playbackEvents, null, true, null).WrapErrors();
		}

		public void Speak(string textToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			this.Speak(textToSpeak, null, playbackEvents);
		}

		public void Speak(string textToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			this.Speak(textToSpeak, diskCacheSettings, null);
		}

		public void Speak(string textToSpeak)
		{
			this.Speak(textToSpeak, null, null);
		}

		public void SpeakFormat(string format, params string[] textsToSpeak)
		{
			this.Speak(this.GetFormattedText(format, textsToSpeak), null, null);
		}

		public IEnumerator SpeakAsync(string textToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			yield return ThreadUtility.CoroutineAwait(delegate()
			{
				this.Load(textToSpeak, null, diskCacheSettings, playbackEvents, null, true, null).WrapErrors();
				return Task.CompletedTask;
			});
			yield break;
		}

		public IEnumerator SpeakAsync(string textToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			yield return this.SpeakAsync(textToSpeak, null, playbackEvents);
			yield break;
		}

		public IEnumerator SpeakAsync(string textToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			yield return this.SpeakAsync(textToSpeak, diskCacheSettings, null);
			yield break;
		}

		public IEnumerator SpeakAsync(string textToSpeak)
		{
			yield return this.SpeakAsync(textToSpeak, null, null);
			yield break;
		}

		public Task SpeakTask(string textToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Load(textToSpeak, null, diskCacheSettings, playbackEvents, null, true, null);
		}

		public Task SpeakTask(string textToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			return this.SpeakTask(textToSpeak, null, playbackEvents);
		}

		public Task SpeakTask(string textToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			return this.SpeakTask(textToSpeak, diskCacheSettings, null);
		}

		public Task SpeakTask(string textToSpeak)
		{
			return this.SpeakTask(textToSpeak, null, null);
		}

		public Task SpeakTask(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Load(responseNode, null, playbackEvents, true);
		}

		public void SpeakQueued(string textToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			this.Load(textToSpeak, null, diskCacheSettings, playbackEvents, null, false, null).WrapErrors();
		}

		public void SpeakQueued(string textToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			this.SpeakQueued(textToSpeak, null, playbackEvents);
		}

		public void SpeakQueued(string textToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			this.SpeakQueued(textToSpeak, diskCacheSettings, null);
		}

		public void SpeakQueued(string textToSpeak)
		{
			this.SpeakQueued(textToSpeak, null, null);
		}

		public void SpeakFormatQueued(string format, params string[] textsToSpeak)
		{
			this.SpeakQueued(this.GetFormattedText(format, textsToSpeak), null, null);
		}

		public IEnumerator SpeakQueuedAsync(string[] textsToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			yield return ThreadUtility.CoroutineAwait(() => this.Load(textsToSpeak, null, diskCacheSettings, playbackEvents, null, false, null));
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(string[] textsToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			yield return this.SpeakQueuedAsync(textsToSpeak, null, playbackEvents);
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(string[] textsToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			yield return this.SpeakQueuedAsync(textsToSpeak, diskCacheSettings, null);
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(string[] textsToSpeak)
		{
			yield return this.SpeakQueuedAsync(textsToSpeak, null, null);
			yield break;
		}

		public Task SpeakQueuedTask(string[] textsToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Load(textsToSpeak, null, diskCacheSettings, playbackEvents, null, false, null);
		}

		public Task SpeakQueuedTask(string[] textsToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			return this.SpeakQueuedTask(textsToSpeak, null, playbackEvents);
		}

		public Task SpeakQueuedTask(string[] textsToSpeak, TTSDiskCacheSettings diskCacheSettings)
		{
			return this.SpeakQueuedTask(textsToSpeak, diskCacheSettings, null);
		}

		public Task SpeakQueuedTask(string[] textsToSpeak)
		{
			return this.SpeakQueuedTask(textsToSpeak, null, null);
		}

		public void SetVoiceOverride(TTSVoiceSettings overrideVoiceSettings)
		{
			this._overrideVoiceSettings = overrideVoiceSettings;
		}

		public void ClearVoiceOverride()
		{
			this.SetVoiceOverride(null);
		}

		public bool Speak(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			string textToSpeak;
			TTSVoiceSettings voiceSettings;
			if (!this.TTSService.DecodeTts(responseNode, out textToSpeak, out voiceSettings))
			{
				return false;
			}
			this.Load(textToSpeak, voiceSettings, diskCacheSettings, playbackEvents, responseNode, true, null).WrapErrors();
			return true;
		}

		public bool Speak(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings)
		{
			return this.Speak(responseNode, diskCacheSettings, null);
		}

		public bool Speak(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Speak(responseNode, null, playbackEvents);
		}

		public bool Speak(WitResponseNode responseNode)
		{
			return this.Speak(responseNode, null, null);
		}

		public IEnumerator SpeakAsync(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			string textToSpeak;
			TTSVoiceSettings voiceSettings;
			if (!this.TTSService.DecodeTts(responseNode, out textToSpeak, out voiceSettings))
			{
				yield break;
			}
			yield return ThreadUtility.CoroutineAwait(() => this.Load(textToSpeak, voiceSettings, diskCacheSettings, playbackEvents, responseNode, true, null));
			yield break;
		}

		public IEnumerator SpeakAsync(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			yield return this.SpeakAsync(responseNode, null, playbackEvents);
			yield break;
		}

		public IEnumerator SpeakAsync(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings)
		{
			yield return this.SpeakAsync(responseNode, diskCacheSettings, null);
			yield break;
		}

		public bool SpeakQueued(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			string textToSpeak;
			TTSVoiceSettings voiceSettings;
			if (!this.TTSService.DecodeTts(responseNode, out textToSpeak, out voiceSettings))
			{
				return false;
			}
			this.Load(textToSpeak, voiceSettings, diskCacheSettings, playbackEvents, responseNode, false, null).WrapErrors();
			return true;
		}

		public bool SpeakQueued(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			return this.SpeakQueued(responseNode, null, playbackEvents);
		}

		public bool SpeakQueued(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings)
		{
			return this.SpeakQueued(responseNode, diskCacheSettings, null);
		}

		public bool SpeakQueued(WitResponseNode responseNode)
		{
			return this.SpeakQueued(responseNode, null, null);
		}

		public IEnumerator SpeakQueuedAsync(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			string textToSpeak;
			TTSVoiceSettings voiceSettings;
			if (!this.TTSService.DecodeTts(responseNode, out textToSpeak, out voiceSettings))
			{
				yield break;
			}
			yield return ThreadUtility.CoroutineAwait(() => this.Load(textToSpeak, voiceSettings, diskCacheSettings, playbackEvents, responseNode, false, null));
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			yield return this.SpeakQueuedAsync(responseNode, null, playbackEvents);
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings)
		{
			yield return this.SpeakQueuedAsync(responseNode, diskCacheSettings, null);
			yield break;
		}

		public IEnumerator SpeakQueuedAsync(WitResponseNode responseNode)
		{
			yield return this.SpeakQueuedAsync(responseNode, null, null);
			yield break;
		}

		public Task SpeakQueuedTask(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Load(responseNode, diskCacheSettings, playbackEvents, false);
		}

		public Task SpeakQueuedTask(string textToSpeak, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents)
		{
			return this.Load(textToSpeak, null, diskCacheSettings, playbackEvents, null, false, null);
		}

		public Task SpeakQueuedTask(WitResponseNode responseNode, TTSSpeakerClipEvents playbackEvents)
		{
			return this.SpeakQueuedTask(responseNode, null, playbackEvents);
		}

		public Task SpeakQueuedTask(string textToSpeak, TTSSpeakerClipEvents playbackEvents)
		{
			return this.SpeakQueuedTask(textToSpeak, null, playbackEvents);
		}

		public Task SpeakQueuedTask(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings)
		{
			return this.SpeakQueuedTask(responseNode, diskCacheSettings, null);
		}

		public Task SpeakQueuedTask(WitResponseNode responseNode)
		{
			return this.SpeakQueuedTask(responseNode, null, null);
		}

		public virtual void Stop(string textToSpeak, bool allInstances = false)
		{
			bool flag = this.SpeakingClip != null && this.SpeakingClip.textToSpeak.Equals(textToSpeak);
			if (flag)
			{
				this.StopSpeaking();
			}
			if (allInstances)
			{
				this.UnloadQueuedText(textToSpeak);
				return;
			}
			if (!flag)
			{
				this.UnloadQueuedClipRequest(this.GetFirstQueuedRequest(textToSpeak));
			}
		}

		public virtual void Stop(TTSClipData clipData, bool allInstances = false)
		{
			bool flag = this.SpeakingClip != null && clipData.Equals(this.SpeakingClip);
			if (allInstances)
			{
				this.UnloadQueuedClip(clipData);
			}
			else if (!flag)
			{
				this.UnloadQueuedClipRequest(this.GetFirstQueuedRequest(clipData));
			}
			if (flag)
			{
				this.StopSpeaking();
			}
		}

		private void StopLoadingButKeepQueue()
		{
			this._queueNotYetComplete = true;
			this.StopLoading();
			this._queueNotYetComplete = false;
		}

		public virtual void StopLoading()
		{
			if (!this.IsLoading)
			{
				return;
			}
			for (int i = 0; i < this._queuedRequests.Count; i++)
			{
				TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData = this._queuedRequests[i];
				if (ttsspeakerRequestData != null)
				{
					this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData>(new Action<TTSSpeaker.TTSSpeakerRequestData>(this.RaiseOnLoadAborted), ttsspeakerRequestData);
				}
			}
			List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
			lock (queuedRequests)
			{
				this._queuedRequests.Clear();
			}
			this.RefreshQueueEvents();
		}

		public virtual void StopSpeaking()
		{
			if (!this.IsSpeaking)
			{
				return;
			}
			this.HandlePlaybackComplete(true);
		}

		public virtual void Stop()
		{
			this.StopLoading();
			this.StopSpeaking();
		}

		private bool DecodeTts(WitResponseNode responseNode, out string textToSpeak, out TTSVoiceSettings voiceSettings)
		{
			return this.TTSService.DecodeTts(responseNode, out textToSpeak, out voiceSettings);
		}

		private TTSSpeaker.TTSSpeakerRequestData CreateRequest(TTSSpeakerClipEvents playbackEvents, WitResponseNode speechNode, bool clearQueue, bool add = true)
		{
			TTSSpeaker.TTSSpeakerRequestData requestData = new TTSSpeaker.TTSSpeakerRequestData();
			requestData.OnReady = delegate(TTSClipData clip)
			{
				this.TryPlayLoadedClip(requestData);
			};
			requestData.IsReady = false;
			requestData.StartTime = DateTime.UtcNow;
			requestData.PlaybackCompletion = new TaskCompletionSource<bool>();
			requestData.PlaybackEvents = (playbackEvents ?? new TTSSpeakerClipEvents());
			requestData.StopPlaybackOnLoad = clearQueue;
			requestData.SpeechNode = speechNode;
			if (add)
			{
				List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
				lock (queuedRequests)
				{
					this._queuedRequests.Add(requestData);
				}
			}
			return requestData;
		}

		private Task Load(WitResponseNode responseNode, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents, bool clearQueue)
		{
			TTSSpeaker.<Load>d__122 <Load>d__;
			<Load>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Load>d__.<>4__this = this;
			<Load>d__.responseNode = responseNode;
			<Load>d__.diskCacheSettings = diskCacheSettings;
			<Load>d__.playbackEvents = playbackEvents;
			<Load>d__.clearQueue = clearQueue;
			<Load>d__.<>1__state = -1;
			<Load>d__.<>t__builder.Start<TTSSpeaker.<Load>d__122>(ref <Load>d__);
			return <Load>d__.<>t__builder.Task;
		}

		private Task Load(string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents, WitResponseNode speechNode, bool clearQueue, TTSSpeaker.TTSSpeakerRequestData requestPlaceholder = null)
		{
			return this.Load(new string[]
			{
				textToSpeak
			}, voiceSettings, diskCacheSettings, playbackEvents, speechNode, clearQueue, requestPlaceholder);
		}

		private Task Load(string[] textsToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings, TTSSpeakerClipEvents playbackEvents, WitResponseNode speechNode, bool clearQueue, TTSSpeaker.TTSSpeakerRequestData requestPlaceholder = null)
		{
			TTSSpeaker.<Load>d__124 <Load>d__;
			<Load>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Load>d__.<>4__this = this;
			<Load>d__.textsToSpeak = textsToSpeak;
			<Load>d__.voiceSettings = voiceSettings;
			<Load>d__.diskCacheSettings = diskCacheSettings;
			<Load>d__.playbackEvents = playbackEvents;
			<Load>d__.speechNode = speechNode;
			<Load>d__.clearQueue = clearQueue;
			<Load>d__.requestPlaceholder = requestPlaceholder;
			<Load>d__.<>1__state = -1;
			<Load>d__.<>t__builder.Start<TTSSpeaker.<Load>d__124>(ref <Load>d__);
			return <Load>d__.<>t__builder.Task;
		}

		private Task LoadClip(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			TTSSpeaker.<LoadClip>d__125 <LoadClip>d__;
			<LoadClip>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadClip>d__.<>4__this = this;
			<LoadClip>d__.requestData = requestData;
			<LoadClip>d__.<>1__state = -1;
			<LoadClip>d__.<>t__builder.Start<TTSSpeaker.<LoadClip>d__125>(ref <LoadClip>d__);
			return <LoadClip>d__.<>t__builder.Task;
		}

		private void TryPlayLoadedClip(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			if (requestData.IsReady)
			{
				return;
			}
			requestData.IsReady = true;
			this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData>(new Action<TTSSpeaker.TTSSpeakerRequestData>(this.RaiseOnPlaybackReady), requestData);
			if (requestData.StopPlaybackOnLoad && this.IsSpeaking)
			{
				this.StopSpeaking();
				return;
			}
			this.RefreshPlayback();
		}

		private void FinalizeLoadedClip(TTSSpeaker.TTSSpeakerRequestData requestData, string error)
		{
			if (string.IsNullOrEmpty(error) && (requestData.ClipData == null || !string.IsNullOrEmpty(requestData.ClipData.textToSpeak)))
			{
				if (requestData.ClipData == null)
				{
					error = "No TTSClipData found";
				}
				else if (requestData.ClipData.clipStream == null)
				{
					error = "No AudioClip found";
				}
				else if (requestData.ClipData.loadState == TTSClipLoadState.Error)
				{
					error = "Error without message";
				}
				else if (requestData.ClipData.loadState == TTSClipLoadState.Unloaded)
				{
					error = "Cancelled";
				}
			}
			if (!string.IsNullOrEmpty(error))
			{
				requestData.Error = error;
				this.UnloadQueuedClipRequest(requestData);
				return;
			}
			if (!requestData.IsReady)
			{
				this.TryPlayLoadedClip(requestData);
			}
		}

		private void RefreshPlayback()
		{
			if (this.SpeakingClip != null || this._queuedRequests == null || this._queuedRequests.Count == 0 || this._audioPlayer == null)
			{
				return;
			}
			string playbackErrors = this.AudioPlayer.GetPlaybackErrors();
			if (!string.IsNullOrEmpty(playbackErrors))
			{
				this.Logger.Error("Refresh Playback Failed\nError: " + playbackErrors, Array.Empty<object>());
				return;
			}
			List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
			TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData;
			lock (queuedRequests)
			{
				ttsspeakerRequestData = this._queuedRequests[0];
				if (ttsspeakerRequestData == null || ttsspeakerRequestData.ClipData == null || ttsspeakerRequestData.ClipData.loadState != TTSClipLoadState.Loaded)
				{
					return;
				}
				this._queuedRequests.RemoveAt(0);
			}
			this._speakingRequest = ttsspeakerRequestData;
			this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData>(new Action<TTSSpeaker.TTSSpeakerRequestData>(this.RaiseOnPlaybackBegin), this._speakingRequest);
			if (this._speakingRequest.StopPlaybackOnLoad && this.IsPaused)
			{
				this.Resume();
			}
			if (string.IsNullOrEmpty(this._speakingRequest.ClipData.textToSpeak))
			{
				this.HandlePlaybackComplete(false);
				return;
			}
			if (this._speakingRequest.ClipData.clipStream == null)
			{
				this.HandlePlaybackComplete(true);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.AudioPlayer.Play(this._speakingRequest.ClipData.clipStream, 0, this._speakingRequest.SpeechNode);
			});
			if (this._waitForCompletion != null)
			{
				base.StopCoroutine(this._waitForCompletion);
				this._waitForCompletion = null;
			}
			this._waitForCompletion = base.StartCoroutine(this.WaitForPlaybackComplete());
		}

		private IEnumerator WaitForPlaybackComplete()
		{
			int sample = -1;
			this._elapsedPlayTime = 0f;
			while (!this.IsPlaybackComplete())
			{
				yield return null;
				if (!this.IsPaused)
				{
					this._elapsedPlayTime += Time.deltaTime;
				}
				bool flag = !this.AudioPlayer.IsPlaying;
				if (this.IsPaused != flag)
				{
					if (this.IsPaused)
					{
						this.AudioPlayer.Pause();
					}
					else
					{
						this.AudioPlayer.Resume();
					}
				}
				int elapsedSamples = this.ElapsedSamples;
				if (sample != elapsedSamples)
				{
					sample = elapsedSamples;
					this.RaisePlaybackSampleUpdated(sample);
				}
			}
			this.HandlePlaybackComplete(false);
			yield break;
		}

		protected virtual bool IsPlaybackComplete()
		{
			if (!this.AudioPlayer.IsPlaying && !this.IsPaused)
			{
				return true;
			}
			IAudioPlayer audioPlayer = this.AudioPlayer;
			return ((audioPlayer != null) ? audioPlayer.ClipStream : null) == null || (this.AudioPlayer.ClipStream.IsComplete && this.ElapsedSamples >= this.TotalSamples);
		}

		protected virtual void HandlePlaybackComplete(bool stopped)
		{
			if (this._waitForCompletion != null)
			{
				base.StopCoroutine(this._waitForCompletion);
				this._waitForCompletion = null;
			}
			TTSSpeaker.TTSSpeakerRequestData speakingRequest = this._speakingRequest;
			this._speakingRequest = null;
			this.RaisePlaybackSampleUpdated(0);
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.AudioPlayer.Stop();
			}).WrapErrors();
			if (stopped)
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData, string>(new Action<TTSSpeaker.TTSSpeakerRequestData, string>(this.RaiseOnPlaybackCancelled), speakingRequest, "Playback stopped manually");
			}
			else if (speakingRequest.ClipData.loadState == TTSClipLoadState.Unloaded)
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData, string>(new Action<TTSSpeaker.TTSSpeakerRequestData, string>(this.RaiseOnPlaybackCancelled), speakingRequest, "TTSClipData was unloaded");
			}
			else if (speakingRequest.ClipData.clipStream == null)
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData, string>(new Action<TTSSpeaker.TTSSpeakerRequestData, string>(this.RaiseOnPlaybackCancelled), speakingRequest, "AudioClip no longer exists");
			}
			else
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData>(new Action<TTSSpeaker.TTSSpeakerRequestData>(this.RaiseOnPlaybackComplete), speakingRequest);
			}
			this.RefreshQueueEvents();
			this.RefreshPlayback();
		}

		public bool IsPaused { get; private set; }

		public void Pause()
		{
			this.SetPause(true);
		}

		public void Resume()
		{
			this.SetPause(false);
		}

		public void PrepareToSpeak()
		{
		}

		public void StartTextBlock()
		{
		}

		public void EndTextBlock()
		{
		}

		protected virtual void SetPause(bool toPaused)
		{
			if (this.IsPaused == toPaused)
			{
				return;
			}
			this.IsPaused = toPaused;
			this.Log("Speak Audio " + (this.IsPaused ? "Paused" : "Resumed"), Array.Empty<object>());
			if (this.IsSpeaking)
			{
				if (this.IsPaused)
				{
					this.AudioPlayer.Pause();
					return;
				}
				if (!this.IsPaused)
				{
					this.AudioPlayer.Resume();
				}
			}
		}

		private bool UnloadQueuedClipRequest(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			return this.FindAndUnloadRequests<TTSSpeaker.TTSSpeakerRequestData>(new Func<TTSSpeaker.TTSSpeakerRequestData, TTSSpeaker.TTSSpeakerRequestData, bool>(TTSSpeaker.RequestEquals), requestData);
		}

		private bool UnloadQueuedClip(TTSClipData clipData)
		{
			return this.FindAndUnloadRequests<TTSClipData>(new Func<TTSSpeaker.TTSSpeakerRequestData, TTSClipData, bool>(TTSSpeaker.RequestHasClipData), clipData);
		}

		private bool UnloadQueuedText(string textToSpeak)
		{
			return this.FindAndUnloadRequests<string>(new Func<TTSSpeaker.TTSSpeakerRequestData, string, bool>(TTSSpeaker.RequestHasClipText), textToSpeak);
		}

		private void RemoveQueuedRequest(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			if (this._queuedRequests == null || !this._queuedRequests.Contains(requestData))
			{
				return;
			}
			List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
			lock (queuedRequests)
			{
				this._queuedRequests.Remove(requestData);
			}
		}

		private bool FindAndUnloadRequests<T>(Func<TTSSpeaker.TTSSpeakerRequestData, T, bool> findMethod, T findParameter)
		{
			if (this._queuedRequests == null)
			{
				return false;
			}
			bool flag = false;
			int i = 0;
			while (i < this._queuedRequests.Count)
			{
				TTSSpeaker.TTSSpeakerRequestData ttsspeakerRequestData = this._queuedRequests[i];
				if (ttsspeakerRequestData == null || findMethod(ttsspeakerRequestData, findParameter))
				{
					flag = true;
					List<TTSSpeaker.TTSSpeakerRequestData> queuedRequests = this._queuedRequests;
					lock (queuedRequests)
					{
						this._queuedRequests.RemoveAt(i);
					}
					this.RaiseUnloadEvents(ttsspeakerRequestData);
				}
				else
				{
					i++;
				}
			}
			if (!flag)
			{
				return false;
			}
			this.RefreshQueueEvents();
			return true;
		}

		private void RaiseUnloadEvents(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			string text = (requestData != null) ? requestData.Error : null;
			if (string.IsNullOrEmpty(text))
			{
				string text2;
				if (requestData == null)
				{
					text2 = null;
				}
				else
				{
					TTSClipData clipData = requestData.ClipData;
					text2 = ((clipData != null) ? clipData.LoadError : null);
				}
				text = text2;
			}
			if (requestData != null && requestData.Equals(this._speakingRequest))
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData, string>(new Action<TTSSpeaker.TTSSpeakerRequestData, string>(this.RaiseOnPlaybackCancelled), requestData, text);
				return;
			}
			if (string.IsNullOrEmpty(text) || string.Equals(text, "Cancelled"))
			{
				this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData>(new Action<TTSSpeaker.TTSSpeakerRequestData>(this.RaiseOnLoadAborted), requestData);
				return;
			}
			this.RaiseEvents<TTSSpeaker.TTSSpeakerRequestData, string>(new Action<TTSSpeaker.TTSSpeakerRequestData, string>(this.RaiseOnLoadFailed), requestData, text);
		}

		private void Log(string format, params object[] parameters)
		{
			if (this.verboseLogging)
			{
				this.Logger.Verbose(format, parameters);
			}
		}

		private void Error(string format, params object[] parameters)
		{
			this.Logger.Warning(format, parameters);
		}

		private void LogRequest(string comment, TTSSpeaker.TTSSpeakerRequestData requestData, string error = null)
		{
			if (!this.verboseLogging && string.IsNullOrEmpty(error))
			{
				return;
			}
			if (!string.IsNullOrEmpty(error))
			{
				string format = "{0}\n{1}\nElapsed: {2:0.00} seconds\nAudio Player Type: {3}\nError: {4}";
				object[] array = new object[5];
				array[0] = comment;
				array[1] = requestData.ClipData;
				array[2] = (DateTime.UtcNow - requestData.StartTime).TotalSeconds;
				int num = 3;
				IAudioPlayer audioPlayer = this._audioPlayer;
				array[num] = (((audioPlayer != null) ? audioPlayer.GetType().Name : null) ?? "Null");
				array[4] = error;
				this.Error(format, array);
				return;
			}
			string format2 = "{0}\n{1}\nElapsed: {2:0.00} seconds\nAudio Player Type: {3}";
			object[] array2 = new object[4];
			array2[0] = comment;
			array2[1] = requestData.ClipData;
			array2[2] = (DateTime.UtcNow - requestData.StartTime).TotalSeconds;
			int num2 = 3;
			IAudioPlayer audioPlayer2 = this._audioPlayer;
			array2[num2] = (((audioPlayer2 != null) ? audioPlayer2.GetType().Name : null) ?? "Null");
			this.Log(format2, array2);
		}

		private void RaiseEvents(Action events)
		{
			ThreadUtility.CallOnMainThread(this.Logger, new Action(events.Invoke)).WrapErrors();
		}

		private void RaiseEvents<T>(Action<T> events, T parameter)
		{
			this.RaiseEvents(delegate()
			{
				Action<T> events2 = events;
				if (events2 == null)
				{
					return;
				}
				events2(parameter);
			});
		}

		private void RaiseEvents<T1, T2>(Action<T1, T2> events, T1 parameter1, T2 parameter2)
		{
			this.RaiseEvents(delegate()
			{
				Action<T1, T2> events2 = events;
				if (events2 == null)
				{
					return;
				}
				events2(parameter1, parameter2);
			});
		}

		protected virtual void RaiseOnPlaybackQueueBegin()
		{
			this.Log("Playback Queue Begin", Array.Empty<object>());
			TTSSpeakerEvents events = this.Events;
			if (events == null)
			{
				return;
			}
			UnityEvent onPlaybackQueueBegin = events.OnPlaybackQueueBegin;
			if (onPlaybackQueueBegin == null)
			{
				return;
			}
			onPlaybackQueueBegin.Invoke();
		}

		protected virtual void RaiseOnPlaybackQueueComplete()
		{
			this.Log("Playback Queue Complete", Array.Empty<object>());
			TTSSpeakerEvents events = this.Events;
			if (events == null)
			{
				return;
			}
			UnityEvent onPlaybackQueueComplete = events.OnPlaybackQueueComplete;
			if (onPlaybackQueueComplete == null)
			{
				return;
			}
			onPlaybackQueueComplete.Invoke();
		}

		private void RaiseOnBegin(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Speak Begin", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipEvent onInit = events.OnInit;
				if (onInit != null)
				{
					onInit.Invoke(this, requestData.ClipData);
				}
			}
			TTSClipData clipData = requestData.ClipData;
			if (clipData != null)
			{
				Action<TTSClipData> onRequestBegin = clipData.onRequestBegin;
				if (onRequestBegin != null)
				{
					onRequestBegin(requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents == null)
			{
				return;
			}
			TTSSpeakerClipEvent onInit2 = playbackEvents.OnInit;
			if (onInit2 == null)
			{
				return;
			}
			onInit2.Invoke(this, requestData.ClipData);
		}

		private void RaiseOnLoadBegin(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Load Begin", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipDataEvent onClipDataQueued = events.OnClipDataQueued;
				if (onClipDataQueued != null)
				{
					onClipDataQueued.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				TTSSpeakerClipDataEvent onClipDataLoadBegin = events2.OnClipDataLoadBegin;
				if (onClipDataLoadBegin != null)
				{
					onClipDataLoadBegin.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerEvent onClipLoadBegin = events3.OnClipLoadBegin;
				if (onClipLoadBegin != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onClipLoadBegin.Invoke(this, (clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events4 = this.Events;
			if (events4 != null)
			{
				TTSSpeakerClipEvent onLoadBegin = events4.OnLoadBegin;
				if (onLoadBegin != null)
				{
					onLoadBegin.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents == null)
			{
				return;
			}
			TTSSpeakerClipEvent onLoadBegin2 = playbackEvents.OnLoadBegin;
			if (onLoadBegin2 == null)
			{
				return;
			}
			onLoadBegin2.Invoke(this, requestData.ClipData);
		}

		private void RaiseOnLoadAborted(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Load Aborted", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipDataEvent onClipDataLoadAbort = events.OnClipDataLoadAbort;
				if (onClipDataLoadAbort != null)
				{
					onClipDataLoadAbort.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				TTSSpeakerEvent onClipLoadAbort = events2.OnClipLoadAbort;
				if (onClipLoadAbort != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onClipLoadAbort.Invoke(this, (clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipEvent onLoadAbort = events3.OnLoadAbort;
				if (onLoadAbort != null)
				{
					onLoadAbort.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				TTSSpeakerClipEvent onLoadAbort2 = playbackEvents.OnLoadAbort;
				if (onLoadAbort2 != null)
				{
					onLoadAbort2.Invoke(this, requestData.ClipData);
				}
			}
			this.RaiseOnComplete(requestData);
		}

		private void RaiseOnLoadFailed(TTSSpeaker.TTSSpeakerRequestData requestData, string error)
		{
			if (string.Equals(error, "Cancelled"))
			{
				this.RaiseOnLoadAborted(requestData);
				return;
			}
			this.LogRequest("Load Failed", requestData, error);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipDataEvent onClipDataLoadFailed = events.OnClipDataLoadFailed;
				if (onClipDataLoadFailed != null)
				{
					onClipDataLoadFailed.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				TTSSpeakerEvent onClipLoadFailed = events2.OnClipLoadFailed;
				if (onClipLoadFailed != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onClipLoadFailed.Invoke(this, (clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipMessageEvent onLoadFailed = events3.OnLoadFailed;
				if (onLoadFailed != null)
				{
					onLoadFailed.Invoke(this, requestData.ClipData, error);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				TTSSpeakerClipMessageEvent onLoadFailed2 = playbackEvents.OnLoadFailed;
				if (onLoadFailed2 != null)
				{
					onLoadFailed2.Invoke(this, requestData.ClipData, error);
				}
			}
			this.RaiseOnComplete(requestData);
		}

		private void RaiseOnPlaybackReady(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Playback Ready", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipDataEvent onClipDataLoadSuccess = events.OnClipDataLoadSuccess;
				if (onClipDataLoadSuccess != null)
				{
					onClipDataLoadSuccess.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				TTSSpeakerEvent onClipLoadSuccess = events2.OnClipLoadSuccess;
				if (onClipLoadSuccess != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onClipLoadSuccess.Invoke(this, (clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipDataEvent onClipDataPlaybackReady = events3.OnClipDataPlaybackReady;
				if (onClipDataPlaybackReady != null)
				{
					onClipDataPlaybackReady.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events4 = this.Events;
			if (events4 != null)
			{
				TTSSpeakerClipEvent onLoadSuccess = events4.OnLoadSuccess;
				if (onLoadSuccess != null)
				{
					onLoadSuccess.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				TTSSpeakerClipEvent onLoadSuccess2 = playbackEvents.OnLoadSuccess;
				if (onLoadSuccess2 != null)
				{
					onLoadSuccess2.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerEvents events5 = this.Events;
			if (events5 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackReady = events5.OnAudioClipPlaybackReady;
				if (onAudioClipPlaybackReady != null)
				{
					TTSClipData clipData2 = requestData.ClipData;
					onAudioClipPlaybackReady.Invoke((clipData2 != null) ? clipData2.clip : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents2 = requestData.PlaybackEvents;
			if (playbackEvents2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackReady2 = playbackEvents2.OnAudioClipPlaybackReady;
				if (onAudioClipPlaybackReady2 != null)
				{
					TTSClipData clipData3 = requestData.ClipData;
					onAudioClipPlaybackReady2.Invoke((clipData3 != null) ? clipData3.clip : null);
				}
			}
			TTSClipData clipData4 = requestData.ClipData;
			if (clipData4 != null)
			{
				Action<TTSClipData> onPlaybackQueued = clipData4.onPlaybackQueued;
				if (onPlaybackQueued != null)
				{
					onPlaybackQueued(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events6 = this.Events;
			if (events6 != null)
			{
				TTSSpeakerClipEvent onPlaybackReady = events6.OnPlaybackReady;
				if (onPlaybackReady != null)
				{
					onPlaybackReady.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents3 = requestData.PlaybackEvents;
			if (playbackEvents3 == null)
			{
				return;
			}
			TTSSpeakerClipEvent onPlaybackReady2 = playbackEvents3.OnPlaybackReady;
			if (onPlaybackReady2 == null)
			{
				return;
			}
			onPlaybackReady2.Invoke(this, requestData.ClipData);
		}

		private void RaiseOnPlaybackBegin(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Playback Begin", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				VoiceTextEvent onTextPlaybackStart = events.OnTextPlaybackStart;
				if (onTextPlaybackStart != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onTextPlaybackStart.Invoke((clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				VoiceTextEvent onTextPlaybackStart2 = playbackEvents.OnTextPlaybackStart;
				if (onTextPlaybackStart2 != null)
				{
					TTSClipData clipData2 = requestData.ClipData;
					onTextPlaybackStart2.Invoke((clipData2 != null) ? clipData2.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackStart = events2.OnAudioClipPlaybackStart;
				if (onAudioClipPlaybackStart != null)
				{
					TTSClipData clipData3 = requestData.ClipData;
					onAudioClipPlaybackStart.Invoke((clipData3 != null) ? clipData3.clip : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents2 = requestData.PlaybackEvents;
			if (playbackEvents2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackStart2 = playbackEvents2.OnAudioClipPlaybackStart;
				if (onAudioClipPlaybackStart2 != null)
				{
					TTSClipData clipData4 = requestData.ClipData;
					onAudioClipPlaybackStart2.Invoke((clipData4 != null) ? clipData4.clip : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipDataEvent onClipDataPlaybackStart = events3.OnClipDataPlaybackStart;
				if (onClipDataPlaybackStart != null)
				{
					onClipDataPlaybackStart.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events4 = this.Events;
			if (events4 != null)
			{
				TTSSpeakerEvent onStartSpeaking = events4.OnStartSpeaking;
				if (onStartSpeaking != null)
				{
					TTSClipData clipData5 = requestData.ClipData;
					onStartSpeaking.Invoke(this, (clipData5 != null) ? clipData5.textToSpeak : null);
				}
			}
			TTSClipData clipData6 = requestData.ClipData;
			if (clipData6 != null)
			{
				Action<TTSClipData> onPlaybackBegin = clipData6.onPlaybackBegin;
				if (onPlaybackBegin != null)
				{
					onPlaybackBegin(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events5 = this.Events;
			if (events5 != null)
			{
				TTSSpeakerClipEvent onPlaybackStart = events5.OnPlaybackStart;
				if (onPlaybackStart != null)
				{
					onPlaybackStart.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents3 = requestData.PlaybackEvents;
			if (playbackEvents3 == null)
			{
				return;
			}
			TTSSpeakerClipEvent onPlaybackStart2 = playbackEvents3.OnPlaybackStart;
			if (onPlaybackStart2 == null)
			{
				return;
			}
			onPlaybackStart2.Invoke(this, requestData.ClipData);
		}

		private void RaiseOnPlaybackCancelled(TTSSpeaker.TTSSpeakerRequestData requestData, string reason)
		{
			this.LogRequest("Playback Cancelled\nReason: " + reason, requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				VoiceTextEvent onTextPlaybackCancelled = events.OnTextPlaybackCancelled;
				if (onTextPlaybackCancelled != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onTextPlaybackCancelled.Invoke((clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				VoiceTextEvent onTextPlaybackCancelled2 = playbackEvents.OnTextPlaybackCancelled;
				if (onTextPlaybackCancelled2 != null)
				{
					TTSClipData clipData2 = requestData.ClipData;
					onTextPlaybackCancelled2.Invoke((clipData2 != null) ? clipData2.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackCancelled = events2.OnAudioClipPlaybackCancelled;
				if (onAudioClipPlaybackCancelled != null)
				{
					TTSClipData clipData3 = requestData.ClipData;
					onAudioClipPlaybackCancelled.Invoke((clipData3 != null) ? clipData3.clip : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents2 = requestData.PlaybackEvents;
			if (playbackEvents2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackCancelled2 = playbackEvents2.OnAudioClipPlaybackCancelled;
				if (onAudioClipPlaybackCancelled2 != null)
				{
					TTSClipData clipData4 = requestData.ClipData;
					onAudioClipPlaybackCancelled2.Invoke((clipData4 != null) ? clipData4.clip : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipDataEvent onClipDataPlaybackCancelled = events3.OnClipDataPlaybackCancelled;
				if (onClipDataPlaybackCancelled != null)
				{
					onClipDataPlaybackCancelled.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events4 = this.Events;
			if (events4 != null)
			{
				TTSSpeakerEvent onCancelledSpeaking = events4.OnCancelledSpeaking;
				if (onCancelledSpeaking != null)
				{
					TTSClipData clipData5 = requestData.ClipData;
					onCancelledSpeaking.Invoke(this, (clipData5 != null) ? clipData5.textToSpeak : null);
				}
			}
			TTSClipData clipData6 = requestData.ClipData;
			if (clipData6 != null)
			{
				Action<TTSClipData> onPlaybackComplete = clipData6.onPlaybackComplete;
				if (onPlaybackComplete != null)
				{
					onPlaybackComplete(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events5 = this.Events;
			if (events5 != null)
			{
				TTSSpeakerClipMessageEvent onPlaybackCancelled = events5.OnPlaybackCancelled;
				if (onPlaybackCancelled != null)
				{
					onPlaybackCancelled.Invoke(this, requestData.ClipData, reason);
				}
			}
			TTSSpeakerClipEvents playbackEvents3 = requestData.PlaybackEvents;
			if (playbackEvents3 != null)
			{
				TTSSpeakerClipMessageEvent onPlaybackCancelled2 = playbackEvents3.OnPlaybackCancelled;
				if (onPlaybackCancelled2 != null)
				{
					onPlaybackCancelled2.Invoke(this, requestData.ClipData, reason);
				}
			}
			this.RaiseOnComplete(requestData);
		}

		private void RaiseOnPlaybackComplete(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Playback Complete", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				VoiceTextEvent onTextPlaybackFinished = events.OnTextPlaybackFinished;
				if (onTextPlaybackFinished != null)
				{
					TTSClipData clipData = requestData.ClipData;
					onTextPlaybackFinished.Invoke((clipData != null) ? clipData.textToSpeak : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				VoiceTextEvent onTextPlaybackFinished2 = playbackEvents.OnTextPlaybackFinished;
				if (onTextPlaybackFinished2 != null)
				{
					TTSClipData clipData2 = requestData.ClipData;
					onTextPlaybackFinished2.Invoke((clipData2 != null) ? clipData2.textToSpeak : null);
				}
			}
			TTSSpeakerEvents events2 = this.Events;
			if (events2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackFinished = events2.OnAudioClipPlaybackFinished;
				if (onAudioClipPlaybackFinished != null)
				{
					TTSClipData clipData3 = requestData.ClipData;
					onAudioClipPlaybackFinished.Invoke((clipData3 != null) ? clipData3.clip : null);
				}
			}
			TTSSpeakerClipEvents playbackEvents2 = requestData.PlaybackEvents;
			if (playbackEvents2 != null)
			{
				VoiceAudioEvent onAudioClipPlaybackFinished2 = playbackEvents2.OnAudioClipPlaybackFinished;
				if (onAudioClipPlaybackFinished2 != null)
				{
					TTSClipData clipData4 = requestData.ClipData;
					onAudioClipPlaybackFinished2.Invoke((clipData4 != null) ? clipData4.clip : null);
				}
			}
			TTSSpeakerEvents events3 = this.Events;
			if (events3 != null)
			{
				TTSSpeakerClipDataEvent onClipDataPlaybackFinished = events3.OnClipDataPlaybackFinished;
				if (onClipDataPlaybackFinished != null)
				{
					onClipDataPlaybackFinished.Invoke(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events4 = this.Events;
			if (events4 != null)
			{
				TTSSpeakerEvent onFinishedSpeaking = events4.OnFinishedSpeaking;
				if (onFinishedSpeaking != null)
				{
					TTSClipData clipData5 = requestData.ClipData;
					onFinishedSpeaking.Invoke(this, (clipData5 != null) ? clipData5.textToSpeak : null);
				}
			}
			TTSClipData clipData6 = requestData.ClipData;
			if (clipData6 != null)
			{
				Action<TTSClipData> onPlaybackComplete = clipData6.onPlaybackComplete;
				if (onPlaybackComplete != null)
				{
					onPlaybackComplete(requestData.ClipData);
				}
			}
			TTSSpeakerEvents events5 = this.Events;
			if (events5 != null)
			{
				TTSSpeakerClipEvent onPlaybackComplete2 = events5.OnPlaybackComplete;
				if (onPlaybackComplete2 != null)
				{
					onPlaybackComplete2.Invoke(this, requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents3 = requestData.PlaybackEvents;
			if (playbackEvents3 != null)
			{
				TTSSpeakerClipEvent onPlaybackComplete3 = playbackEvents3.OnPlaybackComplete;
				if (onPlaybackComplete3 != null)
				{
					onPlaybackComplete3.Invoke(this, requestData.ClipData);
				}
			}
			this.RaiseOnComplete(requestData);
		}

		private void RaiseOnComplete(TTSSpeaker.TTSSpeakerRequestData requestData)
		{
			this.LogRequest("Speak Complete", requestData, null);
			TTSSpeakerEvents events = this.Events;
			if (events != null)
			{
				TTSSpeakerClipEvent onComplete = events.OnComplete;
				if (onComplete != null)
				{
					onComplete.Invoke(this, requestData.ClipData);
				}
			}
			TTSClipData clipData = requestData.ClipData;
			if (clipData != null)
			{
				Action<TTSClipData> onRequestComplete = clipData.onRequestComplete;
				if (onRequestComplete != null)
				{
					onRequestComplete(requestData.ClipData);
				}
			}
			TTSSpeakerClipEvents playbackEvents = requestData.PlaybackEvents;
			if (playbackEvents != null)
			{
				TTSSpeakerClipEvent onComplete2 = playbackEvents.OnComplete;
				if (onComplete2 != null)
				{
					onComplete2.Invoke(this, requestData.ClipData);
				}
			}
			TaskCompletionSource<bool> playbackCompletion = requestData.PlaybackCompletion;
			if (playbackCompletion == null)
			{
				return;
			}
			playbackCompletion.TrySetResult(true);
		}

		public int ElapsedSamples
		{
			get
			{
				if (this.IsSpeaking)
				{
					IAudioPlayer audioPlayer = this._audioPlayer;
					if (((audioPlayer != null) ? audioPlayer.ClipStream : null) != null)
					{
						if (this._audioPlayer.CanSetElapsedSamples)
						{
							return this._audioPlayer.ElapsedSamples;
						}
						return Mathf.FloorToInt(this._elapsedPlayTime * (float)this._audioPlayer.ClipStream.Channels * (float)this._audioPlayer.ClipStream.SampleRate);
					}
				}
				return 0;
			}
		}

		public int TotalSamples
		{
			get
			{
				if (this.IsSpeaking)
				{
					TTSClipData speakingClip = this.SpeakingClip;
					if (((speakingClip != null) ? speakingClip.clipStream : null) != null)
					{
						return this.SpeakingClip.clipStream.TotalSamples;
					}
				}
				return 0;
			}
		}

		public TTSEventSampleDelegate OnSampleUpdated { get; set; }

		protected virtual void RaisePlaybackSampleUpdated(int sample)
		{
			TTSEventSampleDelegate onSampleUpdated = this.OnSampleUpdated;
			if (onSampleUpdated == null)
			{
				return;
			}
			onSampleUpdated(sample);
		}

		public TTSEventContainer CurrentEvents
		{
			get
			{
				TTSClipData speakingClip = this.SpeakingClip;
				if (speakingClip == null)
				{
					return null;
				}
				return speakingClip.Events;
			}
		}

		[Header("Event Settings")]
		[Tooltip("All speaker load and playback events")]
		[SerializeField]
		private TTSSpeakerEvents _events = new TTSSpeakerEvents();

		[Header("Text Settings")]
		[Tooltip("Text that is added to the front of any Speech() request")]
		[TextArea]
		[FormerlySerializedAs("prependedText")]
		public string PrependedText;

		[Tooltip("Text that is added to the end of any Speech() text")]
		[TextArea]
		[FormerlySerializedAs("appendedText")]
		public string AppendedText;

		[Header("Load Settings")]
		[Tooltip("Optional TTSService reference to be used for text-to-speech loading.  If missing, it will check the component.  If that is also missing then it will use the current singleton")]
		[SerializeField]
		private TTSService _ttsService;

		[Tooltip("Preset voice setting id of TTSService voice settings")]
		[HideInInspector]
		[SerializeField]
		private string presetVoiceID;

		[Tooltip("Custom wit specific voice settings used if the preset is null or empty")]
		[HideInInspector]
		[SerializeField]
		public TTSWitVoiceSettings customWitVoiceSettings;

		[SerializeField]
		private bool verboseLogging;

		private TTSVoiceSettings _overrideVoiceSettings;

		private float _elapsedPlayTime;

		private TTSSpeaker.TTSSpeakerRequestData _speakingRequest;

		private List<TTSSpeaker.TTSSpeakerRequestData> _queuedRequests = new List<TTSSpeaker.TTSSpeakerRequestData>();

		private bool _hasQueue;

		private bool _queueNotYetComplete;

		private ISpeakerTextPreprocessor[] _textPreprocessors;

		private ISpeakerTextPostprocessor[] _textPostprocessors;

		private IAudioPlayer _audioPlayer;

		private Coroutine _waitForCompletion;

		private bool _isPlaying;

		private class TTSSpeakerRequestData
		{
			public TTSClipData ClipData;

			public Action<TTSClipData> OnReady;

			public bool IsReady;

			public string Error;

			public DateTime StartTime;

			public bool StopPlaybackOnLoad;

			public TTSSpeakerClipEvents PlaybackEvents;

			public TaskCompletionSource<bool> PlaybackCompletion;

			public WitResponseNode SpeechNode;
		}
	}
}
