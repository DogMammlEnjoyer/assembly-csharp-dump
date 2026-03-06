using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Audio;
using Meta.Voice.Audio.Decoding;
using Meta.Voice.Net.WebSockets;
using Meta.Voice.Net.WebSockets.Requests;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.TTS.Integrations
{
	public class TTSWit : TTSService, ITTSVoiceProvider, ITTSWebHandler, IWitConfigurationProvider, IWitConfigurationSetter, ILogSource
	{
		public override ITTSVoiceProvider VoiceProvider
		{
			get
			{
				return this;
			}
		}

		public override ITTSWebHandler WebHandler
		{
			get
			{
				return this;
			}
		}

		public WitConfiguration Configuration
		{
			get
			{
				return this.RequestSettings._configuration;
			}
			set
			{
				this.RequestSettings._configuration = value;
				Action<WitConfiguration> onConfigurationUpdated = this.OnConfigurationUpdated;
				if (onConfigurationUpdated != null)
				{
					onConfigurationUpdated(this.RequestSettings._configuration);
				}
				this.RefreshWebSocketSettings();
			}
		}

		public event Action<WitConfiguration> OnConfigurationUpdated;

		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshWebSocketSettings();
			this.RefreshAudioSystemSettings();
			if (base.AudioSystem != null)
			{
				base.AudioSystem.PreloadClipStreams(this.RequestSettings.audioStreamPreloadCount);
			}
		}

		protected virtual void RefreshWebSocketSettings()
		{
			this._webSocketAdapter = base.GetOrCreateInterface<WitWebSocketAdapter, WitWebSocketAdapter>(this._webSocketAdapter);
			WitConfiguration configuration = this.Configuration;
			this._webSocketAdapter.SetClientProvider((configuration != null && configuration.RequestType == WitRequestType.WebSocket) ? configuration : null);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this._webSocketAdapter)
			{
				this._webSocketAdapter.SetClientProvider(null);
			}
		}

		public override string GetInvalidError()
		{
			string invalidError = base.GetInvalidError();
			if (!string.IsNullOrEmpty(invalidError))
			{
				return invalidError;
			}
			if (this.Configuration == null)
			{
				return "No WitConfiguration Set";
			}
			if (string.IsNullOrEmpty(this.Configuration.GetClientAccessToken()))
			{
				return "No WitConfiguration Client Token";
			}
			return string.Empty;
		}

		public string GetWebErrors(TTSClipData clipData)
		{
			string invalidError = this.GetInvalidError();
			if (!string.IsNullOrEmpty(invalidError))
			{
				return invalidError;
			}
			string ttsErrors = WitRequestSettings.GetTtsErrors((clipData != null) ? clipData.textToSpeak : null, this.Configuration);
			if (!string.IsNullOrEmpty(ttsErrors))
			{
				return ttsErrors;
			}
			return string.Empty;
		}

		public TTSClipData CreateClipData(string clipId, string textToSpeak, TTSVoiceSettings voiceSettings, TTSDiskCacheSettings diskCacheSettings)
		{
			return new TTSClipData
			{
				clipID = clipId,
				textToSpeak = textToSpeak,
				voiceSettings = voiceSettings,
				diskCacheSettings = diskCacheSettings,
				loadState = TTSClipLoadState.Unloaded,
				loadProgress = 0f,
				queryParameters = ((voiceSettings != null) ? voiceSettings.EncodedValues : null),
				clipStream = (string.IsNullOrEmpty(textToSpeak) ? null : this.CreateClipStream()),
				extension = WitRequestSettings.GetAudioExtension(this.RequestSettings.audioType, this.RequestSettings.useEvents),
				queryStream = this.RequestSettings.audioStream,
				useEvents = this.RequestSettings.useEvents
			};
		}

		private IAudioClipStream CreateClipStream()
		{
			if (base.AudioSystem == null)
			{
				return new RawAudioClipStream(1, 24000, this.RequestSettings.audioReadyDuration, 15f);
			}
			this.RefreshAudioSystemSettings();
			return base.AudioSystem.GetAudioClipStream();
		}

		private void RefreshAudioSystemSettings()
		{
			if (base.AudioSystem == null)
			{
				return;
			}
			base.AudioSystem.ClipSettings = new AudioClipSettings
			{
				Channels = 1,
				SampleRate = 24000,
				ReadyDuration = this.RequestSettings.audioReadyDuration,
				MaxDuration = this.RequestSettings.audioMaxDuration
			};
		}

		private WitTTSVRequest CreateHttpRequest(TTSClipData clipData)
		{
			WitTTSVRequest witTTSVRequest = new WitTTSVRequest(this.Configuration, clipData.queryRequestId, clipData.queryOperationId);
			witTTSVRequest.TimeoutMs = this.Configuration.RequestTimeoutMs;
			witTTSVRequest.TextToSpeak = clipData.textToSpeak;
			witTTSVRequest.TtsParameters = clipData.queryParameters;
			witTTSVRequest.FileType = this.RequestSettings.audioType;
			witTTSVRequest.Stream = clipData.queryStream;
			witTTSVRequest.UseEvents = clipData.useEvents;
			this._httpRequests[clipData.clipID] = witTTSVRequest;
			return witTTSVRequest;
		}

		private WitWebSocketTtsRequest CreateWebSocketRequest(TTSClipData clipData, string downloadPath)
		{
			WitWebSocketTtsRequest witWebSocketTtsRequest = new WitWebSocketTtsRequest(clipData.queryRequestId, clipData.textToSpeak, clipData.queryParameters, this.RequestSettings.audioType, clipData.useEvents, downloadPath, clipData.queryOperationId);
			witWebSocketTtsRequest.TimeoutMs = this.Configuration.RequestTimeoutMs;
			witWebSocketTtsRequest.OnSamplesReceived = new AudioSampleDecodeDelegate(clipData.clipStream.AddSamples);
			witWebSocketTtsRequest.OnEventsReceived = new AudioJsonDecodeDelegate(clipData.Events.AddEvents);
			if (base.SimulatedErrorType != (VoiceErrorSimulationType)(-1))
			{
				witWebSocketTtsRequest.SimulatedErrorType = base.SimulatedErrorType;
				base.SimulatedErrorType = (VoiceErrorSimulationType)(-1);
			}
			this._webSocketRequests[clipData.clipID] = witWebSocketTtsRequest;
			return witWebSocketTtsRequest;
		}

		public bool DecodeTtsFromJson(WitResponseNode responseNode, out string textToSpeak, out TTSVoiceSettings voiceSettings)
		{
			WitResponseClass witResponseClass = (responseNode != null) ? responseNode.AsObject : null;
			if (witResponseClass != null && TTSWitVoiceSettings.CanDecode(witResponseClass))
			{
				TTSWitVoiceSettings ttswitVoiceSettings = new TTSWitVoiceSettings();
				if (ttswitVoiceSettings.DeserializeObject(witResponseClass))
				{
					voiceSettings = ttswitVoiceSettings;
					textToSpeak = witResponseClass["q"].Value;
					return true;
				}
			}
			textToSpeak = null;
			voiceSettings = null;
			return false;
		}

		public Task<string> RequestStreamFromWeb(TTSClipData clipData, Action<TTSClipData> onReady)
		{
			TTSWit.<RequestStreamFromWeb>d__25 <RequestStreamFromWeb>d__;
			<RequestStreamFromWeb>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<RequestStreamFromWeb>d__.<>4__this = this;
			<RequestStreamFromWeb>d__.clipData = clipData;
			<RequestStreamFromWeb>d__.onReady = onReady;
			<RequestStreamFromWeb>d__.<>1__state = -1;
			<RequestStreamFromWeb>d__.<>t__builder.Start<TTSWit.<RequestStreamFromWeb>d__25>(ref <RequestStreamFromWeb>d__);
			return <RequestStreamFromWeb>d__.<>t__builder.Task;
		}

		private Task<string> RequestStreamFromWebSocket(TTSClipData clipData)
		{
			TTSWit.<RequestStreamFromWebSocket>d__26 <RequestStreamFromWebSocket>d__;
			<RequestStreamFromWebSocket>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<RequestStreamFromWebSocket>d__.<>4__this = this;
			<RequestStreamFromWebSocket>d__.clipData = clipData;
			<RequestStreamFromWebSocket>d__.<>1__state = -1;
			<RequestStreamFromWebSocket>d__.<>t__builder.Start<TTSWit.<RequestStreamFromWebSocket>d__26>(ref <RequestStreamFromWebSocket>d__);
			return <RequestStreamFromWebSocket>d__.<>t__builder.Task;
		}

		private Task<string> RequestStreamViaHttp(TTSClipData clipData)
		{
			TTSWit.<>c__DisplayClass27_0 CS$<>8__locals1 = new TTSWit.<>c__DisplayClass27_0();
			CS$<>8__locals1.clipData = clipData;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.clipId = CS$<>8__locals1.clipData.clipID;
			CS$<>8__locals1.request = this.CreateHttpRequest(CS$<>8__locals1.clipData);
			this._httpRequests[CS$<>8__locals1.clipId] = CS$<>8__locals1.request;
			return ThreadUtility.BackgroundAsync<string>(base.Logger, delegate()
			{
				TTSWit.<>c__DisplayClass27_0.<<RequestStreamViaHttp>b__0>d <<RequestStreamViaHttp>b__0>d;
				<<RequestStreamViaHttp>b__0>d.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
				<<RequestStreamViaHttp>b__0>d.<>4__this = CS$<>8__locals1;
				<<RequestStreamViaHttp>b__0>d.<>1__state = -1;
				<<RequestStreamViaHttp>b__0>d.<>t__builder.Start<TTSWit.<>c__DisplayClass27_0.<<RequestStreamViaHttp>b__0>d>(ref <<RequestStreamViaHttp>b__0>d);
				return <<RequestStreamViaHttp>b__0>d.<>t__builder.Task;
			});
		}

		public Task<string> RequestDownloadFromWeb(TTSClipData clipData, string diskPath)
		{
			this.CancelRequests(clipData);
			if (this.Configuration != null && this.Configuration.RequestType == WitRequestType.WebSocket && this._webSocketAdapter)
			{
				return this.RequestDownloadFromWebSocket(clipData, diskPath);
			}
			return this.RequestDownloadViaHttp(clipData, diskPath);
		}

		private Task<string> RequestDownloadFromWebSocket(TTSClipData clipData, string diskPath)
		{
			TTSWit.<RequestDownloadFromWebSocket>d__29 <RequestDownloadFromWebSocket>d__;
			<RequestDownloadFromWebSocket>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<RequestDownloadFromWebSocket>d__.<>4__this = this;
			<RequestDownloadFromWebSocket>d__.clipData = clipData;
			<RequestDownloadFromWebSocket>d__.diskPath = diskPath;
			<RequestDownloadFromWebSocket>d__.<>1__state = -1;
			<RequestDownloadFromWebSocket>d__.<>t__builder.Start<TTSWit.<RequestDownloadFromWebSocket>d__29>(ref <RequestDownloadFromWebSocket>d__);
			return <RequestDownloadFromWebSocket>d__.<>t__builder.Task;
		}

		private Task<string> RequestDownloadViaHttp(TTSClipData clipData, string diskPath)
		{
			TTSWit.<>c__DisplayClass30_0 CS$<>8__locals1 = new TTSWit.<>c__DisplayClass30_0();
			CS$<>8__locals1.diskPath = diskPath;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.clipId = clipData.clipID;
			CS$<>8__locals1.request = this.CreateHttpRequest(clipData);
			this._httpRequests[CS$<>8__locals1.clipId] = CS$<>8__locals1.request;
			return ThreadUtility.BackgroundAsync<string>(base.Logger, delegate()
			{
				TTSWit.<>c__DisplayClass30_0.<<RequestDownloadViaHttp>b__0>d <<RequestDownloadViaHttp>b__0>d;
				<<RequestDownloadViaHttp>b__0>d.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
				<<RequestDownloadViaHttp>b__0>d.<>4__this = CS$<>8__locals1;
				<<RequestDownloadViaHttp>b__0>d.<>1__state = -1;
				<<RequestDownloadViaHttp>b__0>d.<>t__builder.Start<TTSWit.<>c__DisplayClass30_0.<<RequestDownloadViaHttp>b__0>d>(ref <<RequestDownloadViaHttp>b__0>d);
				return <<RequestDownloadViaHttp>b__0>d.<>t__builder.Task;
			});
		}

		public Task<string> IsDownloadedToDisk(string diskPath)
		{
			TTSWit.<IsDownloadedToDisk>d__31 <IsDownloadedToDisk>d__;
			<IsDownloadedToDisk>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<IsDownloadedToDisk>d__.<>4__this = this;
			<IsDownloadedToDisk>d__.diskPath = diskPath;
			<IsDownloadedToDisk>d__.<>1__state = -1;
			<IsDownloadedToDisk>d__.<>t__builder.Start<TTSWit.<IsDownloadedToDisk>d__31>(ref <IsDownloadedToDisk>d__);
			return <IsDownloadedToDisk>d__.<>t__builder.Task;
		}

		public Task<string> RequestStreamFromDisk(TTSClipData clipData, string diskPath, Action<TTSClipData> onReady)
		{
			TTSWit.<RequestStreamFromDisk>d__32 <RequestStreamFromDisk>d__;
			<RequestStreamFromDisk>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<RequestStreamFromDisk>d__.<>4__this = this;
			<RequestStreamFromDisk>d__.clipData = clipData;
			<RequestStreamFromDisk>d__.diskPath = diskPath;
			<RequestStreamFromDisk>d__.onReady = onReady;
			<RequestStreamFromDisk>d__.<>1__state = -1;
			<RequestStreamFromDisk>d__.<>t__builder.Start<TTSWit.<RequestStreamFromDisk>d__32>(ref <RequestStreamFromDisk>d__);
			return <RequestStreamFromDisk>d__.<>t__builder.Task;
		}

		private Task<string> RequestStreamFromDiskViaVRequest(TTSClipData clipData, string diskPath)
		{
			TTSWit.<>c__DisplayClass33_0 CS$<>8__locals1 = new TTSWit.<>c__DisplayClass33_0();
			CS$<>8__locals1.diskPath = diskPath;
			CS$<>8__locals1.clipData = clipData;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.clipId = CS$<>8__locals1.clipData.clipID;
			CS$<>8__locals1.request = this.CreateHttpRequest(CS$<>8__locals1.clipData);
			this._httpRequests[CS$<>8__locals1.clipId] = CS$<>8__locals1.request;
			return ThreadUtility.BackgroundAsync<string>(base.Logger, delegate()
			{
				TTSWit.<>c__DisplayClass33_0.<<RequestStreamFromDiskViaVRequest>b__0>d <<RequestStreamFromDiskViaVRequest>b__0>d;
				<<RequestStreamFromDiskViaVRequest>b__0>d.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
				<<RequestStreamFromDiskViaVRequest>b__0>d.<>4__this = CS$<>8__locals1;
				<<RequestStreamFromDiskViaVRequest>b__0>d.<>1__state = -1;
				<<RequestStreamFromDiskViaVRequest>b__0>d.<>t__builder.Start<TTSWit.<>c__DisplayClass33_0.<<RequestStreamFromDiskViaVRequest>b__0>d>(ref <<RequestStreamFromDiskViaVRequest>b__0>d);
				return <<RequestStreamFromDiskViaVRequest>b__0>d.<>t__builder.Task;
			});
		}

		public bool CancelRequests(TTSClipData clipData)
		{
			WitTTSVRequest witTTSVRequest;
			if (this._httpRequests.TryGetValue(clipData.clipID, out witTTSVRequest))
			{
				if (witTTSVRequest != null)
				{
					witTTSVRequest.Cancel();
				}
				return true;
			}
			WitWebSocketTtsRequest witWebSocketTtsRequest;
			if (this._webSocketRequests.TryGetValue(clipData.clipID, out witWebSocketTtsRequest))
			{
				if (witWebSocketTtsRequest != null)
				{
					witWebSocketTtsRequest.Cancel();
				}
				return true;
			}
			return false;
		}

		public TTSWitVoiceSettings[] PresetWitVoiceSettings
		{
			get
			{
				return this._presetVoiceSettings;
			}
		}

		public TTSVoiceSettings[] PresetVoiceSettings
		{
			get
			{
				if (this._presetVoiceSettings == null)
				{
					this._presetVoiceSettings = new TTSWitVoiceSettings[0];
				}
				return this._presetVoiceSettings;
			}
		}

		public TTSVoiceSettings VoiceDefaultSettings
		{
			get
			{
				if (this.PresetVoiceSettings != null && this.PresetVoiceSettings.Length != 0)
				{
					return this.PresetVoiceSettings[0];
				}
				return null;
			}
		}

		private string IsRequestValid(TTSClipData clipData, WitConfiguration configuration)
		{
			string invalidError = this.GetInvalidError();
			if (!string.IsNullOrEmpty(invalidError))
			{
				return invalidError;
			}
			if (clipData == null)
			{
				return "No clip data provided";
			}
			return string.Empty;
		}

		private WitWebSocketAdapter _webSocketAdapter;

		[Header("Web Request Settings")]
		[FormerlySerializedAs("_settings")]
		public TTSWitRequestSettings RequestSettings = new TTSWitRequestSettings
		{
			audioType = TTSWitAudioType.MPEG,
			audioReadyDuration = 1.5f,
			audioMaxDuration = 15f,
			audioStreamPreloadCount = 5,
			audioStream = true,
			useEvents = true
		};

		private ConcurrentDictionary<string, WitTTSVRequest> _httpRequests = new ConcurrentDictionary<string, WitTTSVRequest>();

		private ConcurrentDictionary<string, WitWebSocketTtsRequest> _webSocketRequests = new ConcurrentDictionary<string, WitWebSocketTtsRequest>();

		[Header("Voice Settings")]
		[SerializeField]
		private TTSWitVoiceSettings[] _presetVoiceSettings;
	}
}
