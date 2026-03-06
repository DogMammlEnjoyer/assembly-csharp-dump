using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Meta.Voice;
using Meta.Voice.Logging;
using Meta.Voice.Net.Encoding.Wit;
using Meta.Voice.Net.PubSub;
using Meta.Voice.Net.WebSockets;
using Meta.Voice.Net.WebSockets.Requests;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Meta.WitAi
{
	[LogCategory(LogCategory.Requests)]
	public class WitService : MonoBehaviour, IVoiceEventProvider, IVoiceActivationHandler, ITelemetryEventsProvider, IWitRuntimeConfigProvider, IWitConfigurationProvider
	{
		public IVLogger _log { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Requests, null);

		public IPubSubAdapter PubSub
		{
			get
			{
				this.SetupWebSockets();
				return this._webSocketAdapter;
			}
		}

		public WitConfiguration Configuration
		{
			get
			{
				WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
				if (runtimeConfiguration == null)
				{
					return null;
				}
				return runtimeConfiguration.witConfiguration;
			}
		}

		public bool Active
		{
			get
			{
				return this._isActive || this.IsRequestActive;
			}
		}

		public bool IsRequestActive
		{
			get
			{
				return this._recordingRequest != null && this._recordingRequest.IsActive;
			}
		}

		public IVoiceEventProvider VoiceEventProvider
		{
			get
			{
				return this._voiceEventProvider;
			}
			set
			{
				this._voiceEventProvider = value;
			}
		}

		public ITelemetryEventsProvider TelemetryEventsProvider
		{
			get
			{
				return this._telemetryEventsProvider;
			}
			set
			{
				this._telemetryEventsProvider = value;
			}
		}

		public IWitRuntimeConfigProvider ConfigurationProvider
		{
			get
			{
				return this._runtimeConfigProvider;
			}
			set
			{
				this._runtimeConfigProvider = value;
			}
		}

		public WitRuntimeConfiguration RuntimeConfiguration
		{
			get
			{
				IWitRuntimeConfigProvider runtimeConfigProvider = this._runtimeConfigProvider;
				if (runtimeConfigProvider == null)
				{
					return null;
				}
				return runtimeConfigProvider.RuntimeConfiguration;
			}
		}

		public VoiceEvents VoiceEvents
		{
			get
			{
				return this._voiceEventProvider.VoiceEvents;
			}
		}

		public TelemetryEvents TelemetryEvents
		{
			get
			{
				return this._telemetryEventsProvider.TelemetryEvents;
			}
		}

		public ITranscriptionProvider TranscriptionProvider
		{
			get
			{
				return this._activeTranscriptionProvider;
			}
			set
			{
				if (this._activeTranscriptionProvider != null)
				{
					this._activeTranscriptionProvider.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.OnPartialTranscription));
					this._activeTranscriptionProvider.OnMicLevelChanged.RemoveListener(new UnityAction<float>(this.OnTranscriptionMicLevelChanged));
				}
				this._activeTranscriptionProvider = value;
				if (this._activeTranscriptionProvider != null)
				{
					this._activeTranscriptionProvider.OnPartialTranscription.AddListener(new UnityAction<string>(this.OnPartialTranscription));
					this._activeTranscriptionProvider.OnMicLevelChanged.AddListener(new UnityAction<float>(this.OnTranscriptionMicLevelChanged));
				}
			}
		}

		public IVoiceServiceRequestProvider RequestProvider { get; set; }

		public bool MicActive
		{
			get
			{
				return this._buffer.IsRecording(this);
			}
		}

		protected bool ShouldSendMicData
		{
			get
			{
				return this.RuntimeConfiguration.sendAudioToWit || this._activeTranscriptionProvider == null;
			}
		}

		public virtual bool IsConfigurationValid()
		{
			return this.RuntimeConfiguration.witConfiguration != null && !string.IsNullOrEmpty(this.RuntimeConfiguration.witConfiguration.GetClientAccessToken());
		}

		private VoiceServiceRequest GetTextRequest(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			WitConfiguration configuration = this.Configuration;
			WitRequestOptions setupOptions = WitRequestFactory.GetSetupOptions(configuration, requestOptions, this._dynamicEntityProviders);
			VoiceServiceRequestEvents voiceServiceRequestEvents = requestEvents ?? new VoiceServiceRequestEvents();
			setupOptions.InputType = NLPRequestInputType.Text;
			if (configuration != null && configuration.RequestType == WitRequestType.WebSocket)
			{
				this.SetupWebSockets();
			}
			if (this.RequestProvider != null)
			{
				VoiceServiceRequest voiceServiceRequest = this.RequestProvider.CreateRequest(this.RuntimeConfiguration, setupOptions, voiceServiceRequestEvents);
				if (voiceServiceRequest != null)
				{
					return voiceServiceRequest;
				}
			}
			if (configuration != null && configuration.RequestType == WitRequestType.WebSocket)
			{
				return WitSocketRequest.GetMessageRequest(configuration, this._webSocketAdapter, setupOptions, voiceServiceRequestEvents);
			}
			return configuration.CreateMessageRequest(setupOptions, voiceServiceRequestEvents, this._dynamicEntityProviders);
		}

		private VoiceServiceRequest GetAudioRequest(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			WitConfiguration configuration = this.Configuration;
			WitRequestOptions setupOptions = WitRequestFactory.GetSetupOptions(configuration, requestOptions, this._dynamicEntityProviders);
			VoiceServiceRequestEvents voiceServiceRequestEvents = requestEvents ?? new VoiceServiceRequestEvents();
			setupOptions.InputType = NLPRequestInputType.Audio;
			if (configuration != null && configuration.RequestType == WitRequestType.WebSocket)
			{
				this.SetupWebSockets();
			}
			if (this.RequestProvider != null)
			{
				VoiceServiceRequest voiceServiceRequest = this.RequestProvider.CreateRequest(this.RuntimeConfiguration, setupOptions, voiceServiceRequestEvents);
				if (voiceServiceRequest != null)
				{
					return voiceServiceRequest;
				}
			}
			if (!(configuration != null) || configuration.RequestType != WitRequestType.WebSocket)
			{
				if (this.RuntimeConfiguration.transcribeOnly)
				{
					this._log.Warning("Transcribe request is not available with HTTP.", Array.Empty<object>());
				}
				return configuration.CreateSpeechRequest(setupOptions, voiceServiceRequestEvents, this._dynamicEntityProviders);
			}
			if (this.RuntimeConfiguration.transcribeOnly)
			{
				return WitSocketRequest.GetTranscribeRequest(configuration, this._webSocketAdapter, this._buffer, setupOptions, voiceServiceRequestEvents);
			}
			return WitSocketRequest.GetSpeechRequest(configuration, this._webSocketAdapter, this._buffer, setupOptions, voiceServiceRequestEvents);
		}

		private int GetTimeoutMs()
		{
			if (this.RuntimeConfiguration.overrideTimeoutMs <= 0)
			{
				return this.RuntimeConfiguration.witConfiguration.RequestTimeoutMs;
			}
			return this.RuntimeConfiguration.overrideTimeoutMs;
		}

		protected void Awake()
		{
			this._dataReadyHandlers = base.GetComponents<IWitByteDataReadyHandler>();
			this._dataSentHandlers = base.GetComponents<IWitByteDataSentHandler>();
			this._runtimeConfigProvider = base.GetComponent<IWitRuntimeConfigProvider>();
		}

		protected void OnEnable()
		{
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			this._runtimeConfigProvider = base.GetComponent<IWitRuntimeConfigProvider>();
			this._voiceEventProvider = base.GetComponent<IVoiceEventProvider>();
			if (this._activeTranscriptionProvider == null && this.RuntimeConfiguration != null && this.RuntimeConfiguration.customTranscriptionProvider)
			{
				this.TranscriptionProvider = this.RuntimeConfiguration.customTranscriptionProvider;
			}
			if (this.RuntimeConfiguration != null)
			{
				WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
				runtimeConfiguration.OnConfigurationUpdated = (Action)Delegate.Combine(runtimeConfiguration.OnConfigurationUpdated, new Action(this.RefreshConfigurationSettings));
			}
			this.SetMicDelegates(true);
			this.SetupWebSockets();
			if (this._webSocketAdapter != null)
			{
				this._webSocketAdapter.OnProcessForwardedResponse += this.ProcessForwardedWebSocketResponse;
				this._webSocketAdapter.OnRequestGenerated += this.HandleWebSocketRequestGeneration;
			}
			this._dynamicEntityProviders = base.GetComponents<IDynamicEntitiesProvider>();
		}

		protected void OnDisable()
		{
			if (this.RuntimeConfiguration != null)
			{
				WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
				runtimeConfiguration.OnConfigurationUpdated = (Action)Delegate.Remove(runtimeConfiguration.OnConfigurationUpdated, new Action(this.RefreshConfigurationSettings));
			}
			if (this._webSocketAdapter != null)
			{
				this._webSocketAdapter.OnProcessForwardedResponse -= this.ProcessForwardedWebSocketResponse;
				this._webSocketAdapter.OnRequestGenerated -= this.HandleWebSocketRequestGeneration;
			}
			SceneManager.sceneLoaded -= this.OnSceneLoaded;
			this.SetMicDelegates(false);
		}

		protected virtual void RefreshConfigurationSettings()
		{
			this.SetupWebSockets();
		}

		protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			this.SetMicDelegates(true);
		}

		protected void SetMicDelegates(bool add)
		{
			if (this._buffer == null)
			{
				this._buffer = AudioBuffer.Instance;
				this._bufferDelegates = false;
			}
			AudioBuffer buffer = this._buffer;
			AudioBufferEvents audioBufferEvents = (buffer != null) ? buffer.Events : null;
			if (audioBufferEvents == null)
			{
				return;
			}
			if (this._bufferDelegates == add)
			{
				return;
			}
			this._bufferDelegates = add;
			if (add)
			{
				AudioBufferEvents audioBufferEvents2 = audioBufferEvents;
				audioBufferEvents2.OnAudioStateChange = (Action<VoiceAudioInputState>)Delegate.Combine(audioBufferEvents2.OnAudioStateChange, new Action<VoiceAudioInputState>(this.OnAudioBufferStateChange));
				audioBufferEvents.OnMicLevelChanged.AddListener(new UnityAction<float>(this.OnMicLevelChanged));
				audioBufferEvents.OnByteDataReady.AddListener(new UnityAction<byte[], int, int>(this.OnByteDataReady));
				AudioBufferEvents audioBufferEvents3 = audioBufferEvents;
				audioBufferEvents3.OnSampleReady = (AudioBufferEvents.OnSampleReadyEvent)Delegate.Combine(audioBufferEvents3.OnSampleReady, new AudioBufferEvents.OnSampleReadyEvent(this.OnMicSampleReady));
				return;
			}
			AudioBufferEvents audioBufferEvents4 = audioBufferEvents;
			audioBufferEvents4.OnAudioStateChange = (Action<VoiceAudioInputState>)Delegate.Remove(audioBufferEvents4.OnAudioStateChange, new Action<VoiceAudioInputState>(this.OnAudioBufferStateChange));
			audioBufferEvents.OnMicLevelChanged.RemoveListener(new UnityAction<float>(this.OnMicLevelChanged));
			audioBufferEvents.OnByteDataReady.RemoveListener(new UnityAction<byte[], int, int>(this.OnByteDataReady));
			AudioBufferEvents audioBufferEvents5 = audioBufferEvents;
			audioBufferEvents5.OnSampleReady = (AudioBufferEvents.OnSampleReadyEvent)Delegate.Remove(audioBufferEvents5.OnSampleReady, new AudioBufferEvents.OnSampleReadyEvent(this.OnMicSampleReady));
		}

		private void SetupWebSockets()
		{
			if (!this._webSocketAdapter)
			{
				this._webSocketAdapter = base.gameObject.GetOrAddComponent<WitWebSocketAdapter>();
			}
			WitConfiguration configuration = this.Configuration;
			bool flag = configuration != null && configuration.RequestType == WitRequestType.WebSocket;
			this._webSocketAdapter.SetClientProvider(flag ? configuration : null);
			this._webSocketAdapter.SetSettings(flag ? this.RuntimeConfiguration.pubSubSettings : default(PubSubSettings));
		}

		private bool ProcessForwardedWebSocketResponse(string topicId, string requestId, string clientUserId, WitChunk responseChunk)
		{
			WitWebSocketMessageRequest witWebSocketMessageRequest = new WitWebSocketMessageRequest(responseChunk.jsonData, requestId, clientUserId, null, this.RuntimeConfiguration.transcribeOnly);
			witWebSocketMessageRequest.TimeoutMs = this.GetTimeoutMs();
			witWebSocketMessageRequest.TopicId = topicId;
			this._webSocketAdapter.WebSocketClient.TrackRequest(witWebSocketMessageRequest);
			return true;
		}

		public void HandleWebSocketRequestGeneration(IWitWebSocketRequest webSocketRequest)
		{
			WitWebSocketMessageRequest witWebSocketMessageRequest = webSocketRequest as WitWebSocketMessageRequest;
			if (witWebSocketMessageRequest == null)
			{
				return;
			}
			if (this.IsWebSocketRequestWrapped(webSocketRequest))
			{
				return;
			}
			WitRequestOptions options = new WitRequestOptions(webSocketRequest.RequestId, webSocketRequest.ClientUserId, webSocketRequest.OperationId, Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			WitSocketRequest externalRequest = WitSocketRequest.GetExternalRequest(witWebSocketMessageRequest, this.RuntimeConfiguration.witConfiguration, this._webSocketAdapter, options, null);
			this.SetupRequest(externalRequest);
		}

		private bool IsWebSocketRequestWrapped(IWitWebSocketRequest webSocketRequest)
		{
			return this.IsWebSocketRequestWrapped(this._recordingRequest, webSocketRequest) || this._transmitRequests.ContainsKey(webSocketRequest.RequestId);
		}

		private bool IsWebSocketRequestWrapped(VoiceServiceRequest voiceServiceRequest, IWitWebSocketRequest webSocketRequest)
		{
			WitSocketRequest witSocketRequest = voiceServiceRequest as WitSocketRequest;
			return witSocketRequest != null && witSocketRequest.WebSocketRequest == webSocketRequest;
		}

		public void Activate()
		{
			this.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()));
		}

		public void Activate(WitRequestOptions requestOptions)
		{
			this.Activate(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (!this.IsConfigurationValid())
			{
				this._log.Error("Your AppVoiceExperience \"" + base.gameObject.name + "\" does not have a wit config assigned. Understanding Viewer activations will not trigger in game events..", Array.Empty<object>());
				return null;
			}
			if (this._isActive)
			{
				return null;
			}
			this.StopRecording();
			if (requestOptions == null)
			{
				requestOptions = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			this._isActive = true;
			this._lastSampleMarker = this._buffer.CreateMarker(this.ConfigurationProvider.RuntimeConfiguration.preferredActivationOffset);
			this._lastMinVolumeLevelTime = float.PositiveInfinity;
			this._lastWordTime = float.PositiveInfinity;
			this._receivedTranscription = false;
			VoiceServiceRequest audioRequest = this.GetAudioRequest(requestOptions, requestEvents);
			this.SetupRequest(audioRequest);
			if (this.ShouldSendMicData)
			{
				if (!this._buffer.IsRecording(this))
				{
					this._minKeepAliveWasHit = false;
					this._isSoundWakeActive = true;
					this.StartRecording();
				}
				else
				{
					audioRequest.ActivateAudio();
				}
			}
			ITranscriptionProvider activeTranscriptionProvider = this._activeTranscriptionProvider;
			if (activeTranscriptionProvider != null)
			{
				activeTranscriptionProvider.Activate();
			}
			return this._recordingRequest;
		}

		public void ActivateImmediately()
		{
			this.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()));
		}

		public void ActivateImmediately(WitRequestOptions requestOptions)
		{
			this.ActivateImmediately(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			VoiceServiceRequest voiceServiceRequest = this.Activate(requestOptions, requestEvents);
			if (voiceServiceRequest == null)
			{
				return null;
			}
			this.SendRecordingRequest();
			this._lastSampleMarker = this._buffer.CreateMarker(this.ConfigurationProvider.RuntimeConfiguration.preferredActivationOffset);
			return voiceServiceRequest;
		}

		protected virtual void SendRecordingRequest()
		{
			if (this._recordingRequest == null || this._recordingRequest.State != VoiceRequestState.Initialized)
			{
				return;
			}
			this._isSoundWakeActive = false;
			if (this.ShouldSendMicData)
			{
				this.ExecuteRequest(this._recordingRequest);
			}
		}

		protected void SetupRequest(VoiceServiceRequest newRequest)
		{
			newRequest.Options.TimeoutMs = this.GetTimeoutMs();
			if (newRequest.Options.InputType == NLPRequestInputType.Audio)
			{
				if (this._recordingRequest == newRequest)
				{
					return;
				}
				this._recordingRequest = newRequest;
				IAudioUploadHandler audioUploadHandler = this._recordingRequest as IAudioUploadHandler;
				if (audioUploadHandler != null)
				{
					audioUploadHandler.AudioEncoding = this._buffer.AudioEncoding;
					audioUploadHandler.OnInputStreamReady = new Action(this.OnWitReadyForData);
				}
				WitRequest witRequest = this._recordingRequest as WitRequest;
				if (witRequest != null)
				{
					WitRequest witRequest2 = witRequest;
					WitRequestOptions options = this._recordingRequest.Options;
					witRequest2.audioDurationTracker = new AudioDurationTracker((options != null) ? options.RequestId : null, witRequest.AudioEncoding);
				}
				this._recordingRequest.Events.OnPartialTranscription.AddListener(new UnityAction<string>(this.OnPartialTranscription));
			}
			else
			{
				this._transmitRequests[newRequest.Options.RequestId] = newRequest;
			}
			newRequest.Events.OnCancel.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			newRequest.Events.OnFailed.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			newRequest.Events.OnSuccess.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			newRequest.Events.OnComplete.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleComplete));
			VoiceEvents voiceEvents = this.VoiceEvents;
			if (voiceEvents != null)
			{
				Action<VoiceServiceRequest> onRequestFinalize = voiceEvents.OnRequestFinalize;
				if (onRequestFinalize != null)
				{
					onRequestFinalize(newRequest);
				}
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				VoiceEvents voiceEvents2 = this.VoiceEvents;
				if (voiceEvents2 == null)
				{
					return;
				}
				Meta.WitAi.Events.VoiceServiceRequestEvent onRequestInitialized = voiceEvents2.OnRequestInitialized;
				if (onRequestInitialized == null)
				{
					return;
				}
				onRequestInitialized.Invoke(newRequest);
			}).WrapErrors();
		}

		public void ExecuteRequest(VoiceServiceRequest newRequest)
		{
			if (newRequest == null || newRequest.State != VoiceRequestState.Initialized)
			{
				return;
			}
			this.SetupRequest(newRequest);
			this._timeLimitCoroutine = base.StartCoroutine(this.DeactivateDueToTimeLimit());
			newRequest.Send();
		}

		public void Activate(string text)
		{
			this.Activate(text, new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()));
		}

		public void Activate(string text, WitRequestOptions requestOptions)
		{
			this.Activate(text, requestOptions, new VoiceServiceRequestEvents()).WrapErrors();
		}

		public Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (!this.IsConfigurationValid())
			{
				this._log.Error("Your AppVoiceExperience \"" + base.gameObject.name + "\" does not have a wit config assigned. Understanding Viewer activations will not trigger in game events..", Array.Empty<object>());
				return null;
			}
			if (requestOptions == null)
			{
				requestOptions = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			requestOptions.Text = text;
			VoiceServiceRequest request = this.GetTextRequest(requestOptions, requestEvents);
			return ThreadUtility.BackgroundAsync<VoiceServiceRequest>(this._log, delegate()
			{
				this.SetupRequest(request);
				request.Send();
				return Task.FromResult<VoiceServiceRequest>(request);
			});
		}

		private void StopRecording()
		{
			if (!this._buffer.IsRecording(this))
			{
				return;
			}
			this._buffer.StopRecording(this);
		}

		private void OnWitReadyForData()
		{
			this._lastMinVolumeLevelTime = this._time;
			if (!this._buffer.IsRecording(this))
			{
				this.StartRecording();
			}
		}

		private void StartRecording()
		{
			if (!this._buffer.IsInputAvailable)
			{
				this.VoiceEvents.OnError.Invoke("Input Error", "No input source was available. Cannot activate for voice input.");
				return;
			}
			if (this._buffer.IsRecording(this))
			{
				return;
			}
			this._buffer.StartRecording(this);
		}

		private void OnAudioBufferStateChange(VoiceAudioInputState audioInputState)
		{
			if (this._recordingRequest != null)
			{
				if (this._buffer.IsRecording(this) && this._recordingRequest.AudioInputState == VoiceAudioInputState.Off)
				{
					this._recordingRequest.ActivateAudio();
					return;
				}
				if (!this._buffer.IsRecording(this))
				{
					if (this._recordingRequest.AudioInputState == VoiceAudioInputState.On || this._recordingRequest.AudioInputState == VoiceAudioInputState.Activating)
					{
						this._recordingRequest.DeactivateAudio();
						return;
					}
					if (this._recordingRequest.AudioInputState == VoiceAudioInputState.Off && this._recordingRequest.State == VoiceRequestState.Initialized)
					{
						this._recordingRequest.Cancel("Failed to start audio input");
					}
				}
			}
		}

		private void OnByteDataReady(byte[] buffer, int offset, int length)
		{
			VoiceEvents voiceEvents = this.VoiceEvents;
			if (voiceEvents != null)
			{
				voiceEvents.OnByteDataReady.Invoke(buffer, offset, length);
			}
			int num = 0;
			while (this._dataReadyHandlers != null && num < this._dataReadyHandlers.Length)
			{
				this._dataReadyHandlers[num].OnWitDataReady(buffer, offset, length);
				num++;
			}
		}

		private void OnMicSampleReady(RingBuffer<byte>.Marker marker, float levelMax)
		{
			if (this._lastSampleMarker == null || this._recordingRequest == null)
			{
				return;
			}
			if (this._minSampleByteCount > (long)this._lastSampleMarker.RingBuffer.Capacity)
			{
				this._minSampleByteCount = (long)this._lastSampleMarker.RingBuffer.Capacity;
			}
			if (this._recordingRequest.State == VoiceRequestState.Transmitting && this.IsInputStreamReady() && this._lastSampleMarker.AvailableByteCount >= this._minSampleByteCount)
			{
				this._lastSampleMarker.ReadIntoWriters(new RingBuffer<byte>.ByteDataWriter[]
				{
					new RingBuffer<byte>.ByteDataWriter(this.WriteAudio),
					delegate(byte[] buffer, int offset, int length)
					{
						VoiceEvents voiceEvents4 = this.VoiceEvents;
						if (voiceEvents4 == null)
						{
							return;
						}
						WitByteDataEvent onByteDataSent = voiceEvents4.OnByteDataSent;
						if (onByteDataSent == null)
						{
							return;
						}
						onByteDataSent.Invoke(buffer, offset, length);
					},
					delegate(byte[] buffer, int offset, int length)
					{
						for (int i = 0; i < this._dataSentHandlers.Length; i++)
						{
							IWitByteDataSentHandler witByteDataSentHandler = this._dataSentHandlers[i];
							if (witByteDataSentHandler != null)
							{
								witByteDataSentHandler.OnWitDataSent(buffer, offset, length);
							}
						}
					}
				});
				if (this._receivedTranscription)
				{
					float num = this._time - this._lastWordTime;
					if (num > this.RuntimeConfiguration.minTranscriptionKeepAliveTimeInSeconds)
					{
						this._log.Verbose(string.Format("Deactivated due to inactivity. No new words detected in {0:0.00} seconds.", num), null, null, null, null, "OnMicSampleReady", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\WitService.cs", 761);
						VoiceEvents voiceEvents = this.VoiceEvents;
						this.DeactivateRequest((voiceEvents != null) ? voiceEvents.OnStoppedListeningDueToInactivity : null, false);
						return;
					}
				}
				else
				{
					float num2 = this._time - this._lastMinVolumeLevelTime;
					if (num2 > this.RuntimeConfiguration.minKeepAliveTimeInSeconds)
					{
						this._log.Verbose(string.Format("Deactivated due to inactivity. No sound detected in {0:0.00} seconds.", num2), null, null, null, null, "OnMicSampleReady", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\WitService.cs", 771);
						VoiceEvents voiceEvents2 = this.VoiceEvents;
						this.DeactivateRequest((voiceEvents2 != null) ? voiceEvents2.OnStoppedListeningDueToInactivity : null, false);
						return;
					}
				}
			}
			else if (this._isSoundWakeActive && levelMax > this.RuntimeConfiguration.soundWakeThreshold)
			{
				VoiceEvents voiceEvents3 = this.VoiceEvents;
				if (voiceEvents3 != null)
				{
					UnityEvent onMinimumWakeThresholdHit = voiceEvents3.OnMinimumWakeThresholdHit;
					if (onMinimumWakeThresholdHit != null)
					{
						onMinimumWakeThresholdHit.Invoke();
					}
				}
				this.SendRecordingRequest();
				this._lastSampleMarker.Offset(this.RuntimeConfiguration.sampleLengthInMs * -2);
			}
		}

		private bool IsInputStreamReady()
		{
			IAudioUploadHandler audioUploadHandler = this._recordingRequest as IAudioUploadHandler;
			return audioUploadHandler != null && audioUploadHandler.IsInputStreamReady;
		}

		private void WriteAudio(byte[] buffer, int offset, int length)
		{
			IDataUploadHandler dataUploadHandler = this._recordingRequest as IDataUploadHandler;
			if (dataUploadHandler != null)
			{
				dataUploadHandler.Write(buffer, offset, length);
			}
		}

		private void Update()
		{
			this._time = Time.time;
		}

		private void OnMicLevelChanged(float level)
		{
			if (this.TranscriptionProvider != null && this.TranscriptionProvider.OverrideMicLevel)
			{
				return;
			}
			if (level > this.RuntimeConfiguration.minKeepAliveVolume)
			{
				this._lastMinVolumeLevelTime = this._time;
				this._minKeepAliveWasHit = true;
			}
			VoiceEvents voiceEvents = this.VoiceEvents;
			if (voiceEvents == null)
			{
				return;
			}
			WitMicLevelChangedEvent onMicLevelChanged = voiceEvents.OnMicLevelChanged;
			if (onMicLevelChanged == null)
			{
				return;
			}
			onMicLevelChanged.Invoke(level);
		}

		private void OnTranscriptionMicLevelChanged(float level)
		{
			if (this.TranscriptionProvider != null && this.TranscriptionProvider.OverrideMicLevel)
			{
				this.OnMicLevelChanged(level);
			}
		}

		private void FinalizeAudioDurationTracker()
		{
			if (this._recordingRequest == null)
			{
				return;
			}
			AudioDurationTracker audioDurationTracker = null;
			WitRequest witRequest = this._recordingRequest as WitRequest;
			if (witRequest != null)
			{
				audioDurationTracker = witRequest.audioDurationTracker;
			}
			if (audioDurationTracker == null)
			{
				return;
			}
			WitRequestOptions options = this._recordingRequest.Options;
			string text = (options != null) ? options.RequestId : null;
			if (!string.Equals(text, audioDurationTracker.GetRequestId()))
			{
				VLog.W("Mismatch in request IDs when finalizing AudioDurationTracker. Expected " + text + " but got " + audioDurationTracker.GetRequestId(), null);
				return;
			}
			audioDurationTracker.FinalizeAudio();
			AudioDurationTrackerFinishedEvent onAudioTrackerFinished = this.TelemetryEvents.OnAudioTrackerFinished;
			if (onAudioTrackerFinished == null)
			{
				return;
			}
			onAudioTrackerFinished.Invoke(audioDurationTracker.GetFinalizeTimeStamp(), audioDurationTracker.GetAudioDuration());
		}

		public void Deactivate()
		{
			UnityEvent onComplete;
			if (!this._buffer.IsRecording(this))
			{
				onComplete = null;
			}
			else
			{
				VoiceEvents voiceEvents = this.VoiceEvents;
				onComplete = ((voiceEvents != null) ? voiceEvents.OnStoppedListeningDueToDeactivation : null);
			}
			this.DeactivateRequest(onComplete, false);
		}

		public void DeactivateAndAbortRequest(VoiceServiceRequest request)
		{
			if (request != null)
			{
				VoiceEvents voiceEvents = this.VoiceEvents;
				if (voiceEvents != null)
				{
					UnityEvent onAborting = voiceEvents.OnAborting;
					if (onAborting != null)
					{
						onAborting.Invoke();
					}
				}
				request.Cancel("Request was cancelled.");
			}
		}

		public void DeactivateAndAbortRequest()
		{
			UnityEvent onComplete;
			if (!this._buffer.IsRecording(this))
			{
				onComplete = null;
			}
			else
			{
				VoiceEvents voiceEvents = this.VoiceEvents;
				onComplete = ((voiceEvents != null) ? voiceEvents.OnStoppedListeningDueToDeactivation : null);
			}
			this.DeactivateRequest(onComplete, true);
		}

		private IEnumerator DeactivateDueToTimeLimit()
		{
			yield return new WaitForSeconds(this.RuntimeConfiguration.maxRecordingTime);
			if (this.IsRequestActive)
			{
				this._log.Verbose(string.Format("Deactivated input due to timeout.\nMax Record Time: {0}", this.RuntimeConfiguration.maxRecordingTime), null, null, null, null, "DeactivateDueToTimeLimit", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\WitService.cs", 894);
				VoiceEvents voiceEvents = this.VoiceEvents;
				this.DeactivateRequest((voiceEvents != null) ? voiceEvents.OnStoppedListeningDueToTimeout : null, false);
			}
			yield break;
		}

		private void DeactivateRequest(UnityEvent onComplete = null, bool abort = false)
		{
			if (abort)
			{
				VoiceEvents voiceEvents = this.VoiceEvents;
				if (voiceEvents != null)
				{
					UnityEvent onAborting = voiceEvents.OnAborting;
					if (onAborting != null)
					{
						onAborting.Invoke();
					}
				}
			}
			if (this._timeLimitCoroutine != null)
			{
				base.StopCoroutine(this._timeLimitCoroutine);
				this._timeLimitCoroutine = null;
			}
			this._isActive = false;
			this.StopRecording();
			this.FinalizeAudioDurationTracker();
			ITranscriptionProvider activeTranscriptionProvider = this._activeTranscriptionProvider;
			if (activeTranscriptionProvider != null)
			{
				activeTranscriptionProvider.Deactivate();
			}
			VoiceServiceRequest recordingRequest = this._recordingRequest;
			this._recordingRequest = null;
			this.DeactivateWitRequest(recordingRequest, abort);
			if (abort)
			{
				foreach (string key in this._transmitRequests.Keys.ToArray<string>())
				{
					VoiceServiceRequest request;
					if (this._transmitRequests.TryRemove(key, out request))
					{
						this.DeactivateWitRequest(request, true);
					}
				}
			}
			else if (recordingRequest != null && recordingRequest.IsActive && this._minKeepAliveWasHit)
			{
				this._transmitRequests[recordingRequest.Options.RequestId] = recordingRequest;
				VoiceEvents voiceEvents2 = this.VoiceEvents;
				if (voiceEvents2 != null)
				{
					UnityEvent onMicDataSent = voiceEvents2.OnMicDataSent;
					if (onMicDataSent != null)
					{
						onMicDataSent.Invoke();
					}
				}
			}
			this._minKeepAliveWasHit = false;
			if (onComplete != null)
			{
				onComplete.Invoke();
			}
		}

		private void DeactivateWitRequest(VoiceServiceRequest request, bool abort)
		{
			if (request == null)
			{
				return;
			}
			if (abort)
			{
				request.Cancel("Request was aborted by user.");
				return;
			}
			if (request.IsAudioInputActivated)
			{
				request.DeactivateAudio();
			}
		}

		private void OnPartialTranscription(string transcription)
		{
			this._receivedTranscription = true;
			this._lastWordTime = this._time;
		}

		private void HandleResult(VoiceServiceRequest request)
		{
			if (request == this._recordingRequest)
			{
				this.DeactivateRequest(null, false);
			}
		}

		private void HandleComplete(VoiceServiceRequest request)
		{
			if (request.InputType == NLPRequestInputType.Audio)
			{
				request.Events.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.OnPartialTranscription));
			}
			request.Events.OnCancel.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			request.Events.OnFailed.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			request.Events.OnSuccess.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleResult));
			request.Events.OnComplete.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleComplete));
			VoiceServiceRequest voiceServiceRequest;
			this._transmitRequests.TryRemove(request.Options.RequestId, out voiceServiceRequest);
		}

		private float _lastMinVolumeLevelTime;

		private WitWebSocketAdapter _webSocketAdapter;

		private VoiceServiceRequest _recordingRequest;

		private bool _isSoundWakeActive;

		private RingBuffer<byte>.Marker _lastSampleMarker;

		private bool _minKeepAliveWasHit;

		private bool _isActive;

		private long _minSampleByteCount = 10240L;

		private IVoiceEventProvider _voiceEventProvider;

		private ITelemetryEventsProvider _telemetryEventsProvider;

		private IWitRuntimeConfigProvider _runtimeConfigProvider;

		private ITranscriptionProvider _activeTranscriptionProvider;

		private Coroutine _timeLimitCoroutine;

		private bool _receivedTranscription;

		private float _lastWordTime;

		private ConcurrentDictionary<string, VoiceServiceRequest> _transmitRequests = new ConcurrentDictionary<string, VoiceServiceRequest>();

		private Coroutine _queueHandler;

		private IWitByteDataReadyHandler[] _dataReadyHandlers;

		private IWitByteDataSentHandler[] _dataSentHandlers;

		private IDynamicEntitiesProvider[] _dynamicEntityProviders;

		private float _time;

		private AudioBuffer _buffer;

		private bool _bufferDelegates;
	}
}
