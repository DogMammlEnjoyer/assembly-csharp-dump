using System;
using System.Globalization;
using Meta.Voice;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Dictation;
using Meta.WitAi.Dictation.Data;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Oculus.Voice.Core.Bindings.Android.PlatformLogger;
using Oculus.Voice.Core.Bindings.Interfaces;
using Oculus.Voice.Dictation.Bindings.Android;
using Oculus.VoiceSDK.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice.Dictation
{
	public class AppDictationExperience : DictationService, IWitRuntimeConfigProvider, IWitConfigurationProvider
	{
		public WitRuntimeConfiguration RuntimeConfiguration
		{
			get
			{
				return this.runtimeConfiguration;
			}
		}

		public WitDictationRuntimeConfiguration RuntimeDictationConfiguration
		{
			get
			{
				return this.runtimeConfiguration;
			}
			set
			{
				this.runtimeConfiguration = value;
			}
		}

		public WitConfiguration Configuration
		{
			get
			{
				WitRuntimeConfiguration witRuntimeConfiguration = this.RuntimeConfiguration;
				if (witRuntimeConfiguration == null)
				{
					return null;
				}
				return witRuntimeConfiguration.witConfiguration;
			}
		}

		public DictationSession ActiveSession
		{
			get
			{
				return this._activeSession;
			}
		}

		public WitRequestOptions ActiveRequestOptions
		{
			get
			{
				return this._activeRequestOptions;
			}
		}

		public event Action OnInitialized;

		private static string PACKAGE_VERSION
		{
			get
			{
				return VoiceSDKConstants.SdkVersion;
			}
		}

		public bool HasPlatformIntegrations
		{
			get
			{
				return false;
			}
		}

		public bool UsePlatformIntegrations
		{
			get
			{
				return this.usePlatformServices;
			}
			set
			{
				if (this.usePlatformServices != value || this.HasPlatformIntegrations != value)
				{
					this.usePlatformServices = value;
				}
			}
		}

		public bool DoNotFallbackToWit
		{
			get
			{
				return this.doNotFallbackToWit;
			}
			set
			{
				this.doNotFallbackToWit = value;
			}
		}

		private void InitDictation()
		{
			if (string.IsNullOrEmpty(AppDictationExperience.PACKAGE_VERSION))
			{
				VLog.E("No SDK Version Set", null);
			}
			if (!this.UsePlatformIntegrations)
			{
				if (this._dictationServiceImpl is PlatformDictationImpl)
				{
					((PlatformDictationImpl)this._dictationServiceImpl).Disconnect();
				}
				if (this._voiceSDKLogger is VoiceSDKPlatformLoggerImpl)
				{
					try
					{
						((VoiceSDKPlatformLoggerImpl)this._voiceSDKLogger).Disconnect();
					}
					catch (Exception ex)
					{
						VLog.E("Disconnection error: " + ex.Message, null);
					}
				}
			}
			this._voiceSDKLogger = new VoiceSDKConsoleLoggerImpl();
			this.RevertToWitDictation();
			IVoiceSDKLogger voiceSDKLogger = this._voiceSDKLogger;
			WitDictationRuntimeConfiguration runtimeDictationConfiguration = this.RuntimeDictationConfiguration;
			string witApplication;
			if (runtimeDictationConfiguration == null)
			{
				witApplication = null;
			}
			else
			{
				WitConfiguration witConfiguration = runtimeDictationConfiguration.witConfiguration;
				witApplication = ((witConfiguration != null) ? witConfiguration.GetLoggerAppId() : null);
			}
			voiceSDKLogger.WitApplication = witApplication;
			this._voiceSDKLogger.ShouldLogToConsole = this.enableConsoleLogging;
			Action onInitialized = this.OnInitialized;
			if (onInitialized == null)
			{
				return;
			}
			onInitialized();
		}

		private void OnPlatformServiceNotAvailable()
		{
			if (!this.DoNotFallbackToWit)
			{
				VLog.D("Platform dictation service unavailable. Falling back to WitDictation");
				this.RevertToWitDictation();
				return;
			}
			VLog.D("Platform dictation service unavailable. Falling back to WitDictation is disabled");
			WitErrorEvent onError = this.DictationEvents.OnError;
			if (onError == null)
			{
				return;
			}
			onError.Invoke("Platform dictation unavailable", "Platform dictation service is not available");
		}

		private void OnDictationServiceNotAvailable()
		{
			VLog.D("Dictation service unavailable");
			WitErrorEvent onError = this.DictationEvents.OnError;
			if (onError == null)
			{
				return;
			}
			onError.Invoke("Dictation unavailable", "Dictation service is not available");
		}

		private void RevertToWitDictation()
		{
			WitDictation witDictation = base.GetComponent<WitDictation>();
			if (null == witDictation)
			{
				witDictation = base.gameObject.AddComponent<WitDictation>();
				witDictation.hideFlags = HideFlags.HideInInspector;
			}
			witDictation.RuntimeConfiguration = this.RuntimeDictationConfiguration;
			witDictation.DictationEvents = this.DictationEvents;
			witDictation.TelemetryEvents = base.TelemetryEvents;
			witDictation.ShouldWrap = false;
			this._dictationServiceImpl = witDictation;
			VLog.D("WitDictation init complete");
			this._voiceSDKLogger.IsUsingPlatformIntegration = false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (MicPermissionsManager.HasMicPermission())
			{
				this.InitDictation();
			}
			else
			{
				MicPermissionsManager.RequestMicPermission(delegate(string e)
				{
					this.InitDictation();
				});
			}
			this.DictationEvents.OnDictationSessionStarted.AddListener(new UnityAction<DictationSession>(this.OnDictationSessionStarted));
			base.TelemetryEvents.OnAudioTrackerFinished.AddListener(new UnityAction<long, double>(this.OnAudioDurationTrackerFinished));
		}

		protected override void OnDisable()
		{
			this._dictationServiceImpl = null;
			this._voiceSDKLogger = null;
			this.DictationEvents.OnDictationSessionStarted.RemoveListener(new UnityAction<DictationSession>(this.OnDictationSessionStarted));
			base.TelemetryEvents.OnAudioTrackerFinished.RemoveListener(new UnityAction<long, double>(this.OnAudioDurationTrackerFinished));
			base.OnDisable();
		}

		public override bool Active
		{
			get
			{
				return this._dictationServiceImpl != null && this._dictationServiceImpl.Active;
			}
		}

		public override bool IsRequestActive
		{
			get
			{
				return this._dictationServiceImpl != null && this._dictationServiceImpl.IsRequestActive;
			}
		}

		public override ITranscriptionProvider TranscriptionProvider
		{
			get
			{
				return this._dictationServiceImpl.TranscriptionProvider;
			}
			set
			{
				this._dictationServiceImpl.TranscriptionProvider = value;
			}
		}

		public override bool MicActive
		{
			get
			{
				return this._dictationServiceImpl != null && this._dictationServiceImpl.MicActive;
			}
		}

		protected override bool ShouldSendMicData
		{
			get
			{
				return this.RuntimeConfiguration.sendAudioToWit || this.TranscriptionProvider == null;
			}
		}

		public void Toggle()
		{
			if (this.Active)
			{
				this.Deactivate();
				return;
			}
			base.Activate();
		}

		public override VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (this._dictationServiceImpl == null)
			{
				this.OnDictationServiceNotAvailable();
				return null;
			}
			if (!this._isActive)
			{
				this._activeSession = new DictationSession();
				this.DictationEvents.OnDictationSessionStarted.Invoke(this._activeSession);
			}
			this._isActive = true;
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			return this._dictationServiceImpl.Activate(requestOptions, requestEvents);
		}

		public override VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (this._dictationServiceImpl == null)
			{
				this.OnDictationServiceNotAvailable();
				return null;
			}
			if (!this._isActive)
			{
				this._activeSession = new DictationSession();
				this.DictationEvents.OnDictationSessionStarted.Invoke(this._activeSession);
			}
			this._isActive = true;
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			return this._dictationServiceImpl.ActivateImmediately(requestOptions, requestEvents);
		}

		public override void Deactivate()
		{
			if (this._dictationServiceImpl == null)
			{
				this.OnDictationServiceNotAvailable();
				return;
			}
			this._isActive = false;
			this._dictationServiceImpl.Deactivate();
		}

		public override void Cancel()
		{
			if (this._dictationServiceImpl == null)
			{
				this.OnDictationServiceNotAvailable();
				return;
			}
			this._dictationServiceImpl.Cancel();
			this.CleanupSession();
		}

		protected override void OnRequestInit(VoiceServiceRequest request)
		{
			base.OnRequestInit(request);
			this._activeRequestOptions = ((request != null) ? request.Options : null);
			IVoiceSDKLogger voiceSDKLogger = this._voiceSDKLogger;
			string requestId;
			if (request == null)
			{
				requestId = null;
			}
			else
			{
				WitRequestOptions options = request.Options;
				requestId = ((options != null) ? options.RequestId : null);
			}
			voiceSDKLogger.LogInteractionStart(requestId, "dictation");
			IVoiceSDKLogger voiceSDKLogger2 = this._voiceSDKLogger;
			string annotationKey = "minWakeThreshold";
			WitRuntimeConfiguration witRuntimeConfiguration = this.RuntimeConfiguration;
			voiceSDKLogger2.LogAnnotation(annotationKey, (witRuntimeConfiguration != null) ? witRuntimeConfiguration.soundWakeThreshold.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger3 = this._voiceSDKLogger;
			string annotationKey2 = "minKeepAliveTimeSec";
			WitRuntimeConfiguration witRuntimeConfiguration2 = this.RuntimeConfiguration;
			voiceSDKLogger3.LogAnnotation(annotationKey2, (witRuntimeConfiguration2 != null) ? witRuntimeConfiguration2.minKeepAliveTimeInSeconds.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger4 = this._voiceSDKLogger;
			string annotationKey3 = "minTranscriptionKeepAliveTimeSec";
			WitRuntimeConfiguration witRuntimeConfiguration3 = this.RuntimeConfiguration;
			voiceSDKLogger4.LogAnnotation(annotationKey3, (witRuntimeConfiguration3 != null) ? witRuntimeConfiguration3.minTranscriptionKeepAliveTimeInSeconds.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger5 = this._voiceSDKLogger;
			string annotationKey4 = "maxRecordingTime";
			WitRuntimeConfiguration witRuntimeConfiguration4 = this.RuntimeConfiguration;
			voiceSDKLogger5.LogAnnotation(annotationKey4, (witRuntimeConfiguration4 != null) ? witRuntimeConfiguration4.maxRecordingTime.ToString(CultureInfo.InvariantCulture) : null);
		}

		protected override void OnRequestStartListening(VoiceServiceRequest request)
		{
			base.OnRequestStartListening(request);
			this._voiceSDKLogger.LogInteractionPoint("startedListening");
		}

		protected override void OnRequestStopListening(VoiceServiceRequest request)
		{
			base.OnRequestStopListening(request);
			this._voiceSDKLogger.LogInteractionPoint("stoppedListening");
			if (this.RuntimeDictationConfiguration.dictationConfiguration.multiPhrase && this._isActive)
			{
				base.Activate(this._activeRequestOptions);
			}
		}

		private void OnDictationSessionStarted(DictationSession session)
		{
			PlatformDictationSession platformDictationSession = session as PlatformDictationSession;
			if (platformDictationSession != null)
			{
				this._activeSession = session;
				this._voiceSDKLogger.LogAnnotation("platformInteractionId", platformDictationSession.platformSessionId);
			}
		}

		private void OnAudioDurationTrackerFinished(long timestamp, double audioDuration)
		{
			this._voiceSDKLogger.LogAnnotation("adt_duration", audioDuration.ToString(CultureInfo.InvariantCulture));
			this._voiceSDKLogger.LogAnnotation("adt_finished", timestamp.ToString());
		}

		protected override void OnRequestPartialTranscription(VoiceServiceRequest request, string transcription)
		{
			base.OnRequestPartialTranscription(request, transcription);
			this._voiceSDKLogger.LogFirstTranscriptionTime();
		}

		protected override void OnRequestFullTranscription(VoiceServiceRequest request, string transcription)
		{
			base.OnRequestFullTranscription(request, transcription);
			this._voiceSDKLogger.LogInteractionPoint("fullTranscriptionTime");
		}

		protected override void OnRequestComplete(VoiceServiceRequest request)
		{
			base.OnRequestComplete(request);
			if (request.State == VoiceRequestState.Failed)
			{
				this._voiceSDKLogger.LogInteractionEndFailure(request.Results.Message);
			}
			else if (request.State == VoiceRequestState.Canceled)
			{
				this._voiceSDKLogger.LogInteractionEndFailure("aborted");
			}
			else
			{
				WitResponseNode responseData = request.ResponseData;
				WitResponseNode witResponseNode;
				if (responseData == null)
				{
					witResponseNode = null;
				}
				else
				{
					WitResponseNode witResponseNode2 = responseData["speech"];
					witResponseNode = ((witResponseNode2 != null) ? witResponseNode2["tokens"] : null);
				}
				WitResponseNode witResponseNode3 = witResponseNode;
				if (witResponseNode3 != null)
				{
					int count = witResponseNode3.Count;
					WitResponseNode witResponseNode4 = request.ResponseData["speech"]["tokens"][count - 1];
					string text;
					if (witResponseNode4 == null)
					{
						text = null;
					}
					else
					{
						WitResponseNode witResponseNode5 = witResponseNode4["end"];
						text = ((witResponseNode5 != null) ? witResponseNode5.Value : null);
					}
					string annotationValue = text;
					this._voiceSDKLogger.LogAnnotation("audioLength", annotationValue);
				}
				this._voiceSDKLogger.LogInteractionEndSuccess();
			}
			if (!this._isActive)
			{
				DictationSessionEvent onDictationSessionStopped = this.DictationEvents.OnDictationSessionStopped;
				if (onDictationSessionStopped != null)
				{
					onDictationSessionStopped.Invoke(this._activeSession);
				}
				this.CleanupSession();
			}
		}

		private void CleanupSession()
		{
			this._activeSession = null;
			this._activeRequestOptions = null;
			this._isActive = false;
		}

		[SerializeField]
		private WitDictationRuntimeConfiguration runtimeConfiguration;

		[Tooltip("Uses platform dictation service instead of accessing wit directly from within the application.")]
		[SerializeField]
		private bool usePlatformServices;

		[Tooltip("Dictation will not fallback to Wit if platform dictation is not available. Not applicable in Unity Editor")]
		[SerializeField]
		private bool doNotFallbackToWit;

		[Tooltip("Enables logs related to the interaction to be displayed on console")]
		[SerializeField]
		private bool enableConsoleLogging;

		private IDictationService _dictationServiceImpl;

		private IVoiceSDKLogger _voiceSDKLogger;

		private bool _isActive;

		private DictationSession _activeSession;

		private WitRequestOptions _activeRequestOptions;
	}
}
