using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Voice;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using Oculus.Voice.Bindings.Android;
using Oculus.Voice.Core.Bindings.Android.PlatformLogger;
using Oculus.Voice.Core.Bindings.Interfaces;
using Oculus.VoiceSDK.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice
{
	[HelpURL("https://developer.oculus.com/experimental/voice-sdk/tutorial-overview/")]
	public class AppVoiceExperience : VoiceService, IWitRuntimeConfigProvider, IWitConfigurationProvider
	{
		public WitRuntimeConfiguration RuntimeConfiguration
		{
			get
			{
				return this.witRuntimeConfiguration;
			}
			set
			{
				this.witRuntimeConfiguration = value;
				IWitRuntimeConfigSetter witRuntimeConfigSetter = this.voiceServiceImpl as IWitRuntimeConfigSetter;
				if (witRuntimeConfigSetter != null)
				{
					witRuntimeConfigSetter.RuntimeConfiguration = this.witRuntimeConfiguration;
				}
			}
		}

		public WitConfiguration Configuration
		{
			get
			{
				WitRuntimeConfiguration witRuntimeConfiguration = this.witRuntimeConfiguration;
				if (witRuntimeConfiguration == null)
				{
					return null;
				}
				return witRuntimeConfiguration.witConfiguration;
			}
		}

		private static string PACKAGE_VERSION
		{
			get
			{
				return VoiceSDKConstants.SdkVersion;
			}
		}

		private bool Initialized
		{
			get
			{
				return this.voiceServiceImpl != null;
			}
		}

		public event Action OnInitialized;

		public override bool Active
		{
			get
			{
				return base.Active || (this.voiceServiceImpl != null && this.voiceServiceImpl.Active);
			}
		}

		public override bool IsRequestActive
		{
			get
			{
				return base.IsRequestActive || (this.voiceServiceImpl != null && this.voiceServiceImpl.IsRequestActive);
			}
		}

		public override ITranscriptionProvider TranscriptionProvider
		{
			get
			{
				IVoiceService voiceService = this.voiceServiceImpl;
				if (voiceService == null)
				{
					return null;
				}
				return voiceService.TranscriptionProvider;
			}
			set
			{
				if (this.voiceServiceImpl != null)
				{
					this.voiceServiceImpl.TranscriptionProvider = value;
				}
			}
		}

		public override bool MicActive
		{
			get
			{
				return this.voiceServiceImpl != null && this.voiceServiceImpl.MicActive;
			}
		}

		protected override bool ShouldSendMicData
		{
			get
			{
				return this.witRuntimeConfiguration.sendAudioToWit || this.TranscriptionProvider == null;
			}
		}

		public bool HasPlatformIntegrations
		{
			get
			{
				return false;
			}
		}

		public bool EnableConsoleLogging
		{
			get
			{
				return this.enableConsoleLogging;
			}
		}

		public override bool UsePlatformIntegrations
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

		public override bool CanSend()
		{
			return base.CanSend() && this.voiceServiceImpl != null && this.voiceServiceImpl.CanSend();
		}

		public override Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			AppVoiceExperience.<Activate>d__37 <Activate>d__;
			<Activate>d__.<>t__builder = AsyncTaskMethodBuilder<VoiceServiceRequest>.Create();
			<Activate>d__.<>4__this = this;
			<Activate>d__.text = text;
			<Activate>d__.requestOptions = requestOptions;
			<Activate>d__.requestEvents = requestEvents;
			<Activate>d__.<>1__state = -1;
			<Activate>d__.<>t__builder.Start<AppVoiceExperience.<Activate>d__37>(ref <Activate>d__);
			return <Activate>d__.<>t__builder.Task;
		}

		public override bool CanActivateAudio()
		{
			return base.CanActivateAudio() && this.voiceServiceImpl != null && this.voiceServiceImpl.CanActivateAudio();
		}

		public override string GetActivateAudioError()
		{
			if (!this.HasPlatformIntegrations && !AudioBuffer.Instance.IsInputAvailable)
			{
				return "No Microphone(s)/recording devices found.  You will be unable to capture audio on this device.";
			}
			return base.GetActivateAudioError();
		}

		public override VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			if (this.CanActivateAudio() && this.CanSend())
			{
				return this.voiceServiceImpl.Activate(requestOptions, requestEvents);
			}
			if (this.voiceServiceImpl == null)
			{
				VLog.D("Voice is not initialized. Attempting to initialize before activating.");
				this.InitVoiceSDK();
				if (this.CanActivateAudio() && this.CanSend())
				{
					IVoiceService voiceService = this.voiceServiceImpl;
					if (voiceService == null)
					{
						return null;
					}
					return voiceService.Activate(requestOptions, requestEvents);
				}
			}
			VLog.W("Cannot currently activate\nAudio Activation Error: " + this.GetActivateAudioError() + "\nSend Error: " + this.GetSendError(), null);
			return null;
		}

		public override VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			if (this.CanActivateAudio() && this.CanSend())
			{
				return this.voiceServiceImpl.ActivateImmediately(requestOptions, requestEvents);
			}
			if (this.voiceServiceImpl == null)
			{
				VLog.D("Voice is not initialized. Attempting to initialize before immediate activation");
				this.InitVoiceSDK();
				if (this.CanActivateAudio() && this.CanSend())
				{
					IVoiceService voiceService = this.voiceServiceImpl;
					if (voiceService == null)
					{
						return null;
					}
					return voiceService.ActivateImmediately(requestOptions, requestEvents);
				}
			}
			VLog.W("Cannot currently activate\nAudio Activation Error: " + this.GetActivateAudioError() + "\nSend Error: " + this.GetSendError(), null);
			return null;
		}

		public override void Deactivate()
		{
			IVoiceService voiceService = this.voiceServiceImpl;
			if (voiceService == null)
			{
				return;
			}
			voiceService.Deactivate();
		}

		public override void DeactivateAndAbortRequest()
		{
			IVoiceService voiceService = this.voiceServiceImpl;
			if (voiceService == null)
			{
				return;
			}
			voiceService.DeactivateAndAbortRequest();
		}

		private void InitVoiceSDK()
		{
			if (string.IsNullOrEmpty(AppVoiceExperience.PACKAGE_VERSION))
			{
				VLog.E("No SDK Version Set", null);
			}
			if (!this.UsePlatformIntegrations)
			{
				if (this.voiceServiceImpl is VoiceSDKImpl)
				{
					((VoiceSDKImpl)this.voiceServiceImpl).Disconnect();
				}
				if (this.voiceSDKLoggerImpl is VoiceSDKPlatformLoggerImpl)
				{
					try
					{
						((VoiceSDKPlatformLoggerImpl)this.voiceSDKLoggerImpl).Disconnect();
					}
					catch (Exception ex)
					{
						VLog.E("Disconnection error: " + ex.Message, null);
					}
				}
			}
			bool flag = this.voiceServiceImpl != null;
			if (!flag)
			{
				this.voiceServiceImpl = base.gameObject.GetComponent<IPlatformIntegrationOverride>();
				flag = (this.voiceServiceImpl != null);
				if (flag)
				{
					VLog.I(string.Format("Using PI override\nClass: {0}", this.voiceServiceImpl.GetType()));
					this.UsePlatformIntegrations = false;
				}
			}
			this.voiceSDKLoggerImpl = new VoiceSDKConsoleLoggerImpl();
			if (!flag)
			{
				this.RevertToWitUnity();
			}
			IWitRuntimeConfigSetter witRuntimeConfigSetter = this.voiceServiceImpl as IWitRuntimeConfigSetter;
			if (witRuntimeConfigSetter != null)
			{
				witRuntimeConfigSetter.RuntimeConfiguration = this.witRuntimeConfiguration;
			}
			this.voiceServiceImpl.VoiceEvents = this.VoiceEvents;
			this.voiceServiceImpl.TelemetryEvents = this.TelemetryEvents;
			this.voiceSDKLoggerImpl.IsUsingPlatformIntegration = this.UsePlatformIntegrations;
			IVoiceSDKLogger voiceSDKLogger = this.voiceSDKLoggerImpl;
			WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
			string witApplication;
			if (runtimeConfiguration == null)
			{
				witApplication = null;
			}
			else
			{
				WitConfiguration witConfiguration = runtimeConfiguration.witConfiguration;
				witApplication = ((witConfiguration != null) ? witConfiguration.GetLoggerAppId() : null);
			}
			voiceSDKLogger.WitApplication = witApplication;
			this.voiceSDKLoggerImpl.ShouldLogToConsole = this.EnableConsoleLogging;
			Action onInitialized = this.OnInitialized;
			if (onInitialized == null)
			{
				return;
			}
			onInitialized();
		}

		private void RevertToWitUnity()
		{
			VLog.I("Initializing Wit Unity...");
			Wit wit = base.GetComponent<Wit>();
			if (null == wit)
			{
				wit = base.gameObject.AddComponent<Wit>();
				wit.hideFlags = HideFlags.HideInInspector;
			}
			wit.ShouldWrap = false;
			this.voiceServiceImpl = wit;
			this.UsePlatformIntegrations = false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (MicPermissionsManager.HasMicPermission())
			{
				this.InitVoiceSDK();
			}
			else
			{
				MicPermissionsManager.RequestMicPermission(null);
			}
			UnityEvent onMinimumWakeThresholdHit = this.VoiceEvents.OnMinimumWakeThresholdHit;
			if (onMinimumWakeThresholdHit != null)
			{
				onMinimumWakeThresholdHit.AddListener(new UnityAction(this.OnMinimumWakeThresholdHit));
			}
			UnityEvent onMicDataSent = this.VoiceEvents.OnMicDataSent;
			if (onMicDataSent != null)
			{
				onMicDataSent.AddListener(new UnityAction(this.OnMicDataSent));
			}
			UnityEvent onStoppedListeningDueToTimeout = this.VoiceEvents.OnStoppedListeningDueToTimeout;
			if (onStoppedListeningDueToTimeout != null)
			{
				onStoppedListeningDueToTimeout.AddListener(new UnityAction(this.OnStoppedListeningDueToTimeout));
			}
			UnityEvent onStoppedListeningDueToInactivity = this.VoiceEvents.OnStoppedListeningDueToInactivity;
			if (onStoppedListeningDueToInactivity != null)
			{
				onStoppedListeningDueToInactivity.AddListener(new UnityAction(this.OnStoppedListeningDueToInactivity));
			}
			UnityEvent onStoppedListeningDueToDeactivation = this.VoiceEvents.OnStoppedListeningDueToDeactivation;
			if (onStoppedListeningDueToDeactivation != null)
			{
				onStoppedListeningDueToDeactivation.AddListener(new UnityAction(this.OnStoppedListeningDueToDeactivation));
			}
			AudioDurationTrackerFinishedEvent onAudioTrackerFinished = this.TelemetryEvents.OnAudioTrackerFinished;
			if (onAudioTrackerFinished != null)
			{
				onAudioTrackerFinished.AddListener(new UnityAction<long, double>(this.OnAudioDurationTrackerFinished));
			}
			base.StartCoroutine(this.RetryInit());
		}

		private IEnumerator RetryInit()
		{
			int waitSeconds = 1;
			while (this.voiceServiceImpl == null)
			{
				VLog.W(string.Format("Voice Service still not initialized yet. Retrying in {0} seconds.", waitSeconds), null);
				yield return new WaitForSeconds((float)waitSeconds);
				if (this.voiceServiceImpl != null)
				{
					break;
				}
				this.InitVoiceSDK();
				int num = waitSeconds;
				waitSeconds = num + 1;
				if (waitSeconds == 10)
				{
					waitSeconds = 1;
				}
			}
			yield break;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.voiceServiceImpl = null;
			this.voiceSDKLoggerImpl = null;
			UnityEvent onMinimumWakeThresholdHit = this.VoiceEvents.OnMinimumWakeThresholdHit;
			if (onMinimumWakeThresholdHit != null)
			{
				onMinimumWakeThresholdHit.RemoveListener(new UnityAction(this.OnMinimumWakeThresholdHit));
			}
			UnityEvent onMicDataSent = this.VoiceEvents.OnMicDataSent;
			if (onMicDataSent != null)
			{
				onMicDataSent.RemoveListener(new UnityAction(this.OnMicDataSent));
			}
			UnityEvent onStoppedListeningDueToTimeout = this.VoiceEvents.OnStoppedListeningDueToTimeout;
			if (onStoppedListeningDueToTimeout != null)
			{
				onStoppedListeningDueToTimeout.RemoveListener(new UnityAction(this.OnStoppedListeningDueToTimeout));
			}
			UnityEvent onStoppedListeningDueToInactivity = this.VoiceEvents.OnStoppedListeningDueToInactivity;
			if (onStoppedListeningDueToInactivity != null)
			{
				onStoppedListeningDueToInactivity.RemoveListener(new UnityAction(this.OnStoppedListeningDueToInactivity));
			}
			UnityEvent onStoppedListeningDueToDeactivation = this.VoiceEvents.OnStoppedListeningDueToDeactivation;
			if (onStoppedListeningDueToDeactivation != null)
			{
				onStoppedListeningDueToDeactivation.RemoveListener(new UnityAction(this.OnStoppedListeningDueToDeactivation));
			}
			AudioDurationTrackerFinishedEvent onAudioTrackerFinished = this.TelemetryEvents.OnAudioTrackerFinished;
			if (onAudioTrackerFinished == null)
			{
				return;
			}
			onAudioTrackerFinished.RemoveListener(new UnityAction<long, double>(this.OnAudioDurationTrackerFinished));
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (base.enabled && hasFocus && !this.Initialized && MicPermissionsManager.HasMicPermission())
			{
				this.InitVoiceSDK();
			}
		}

		protected override void OnRequestInit(VoiceServiceRequest request)
		{
			base.OnRequestInit(request);
			this._waitingForFirstPartialAudio = true;
			IVoiceSDKLogger voiceSDKLogger = this.voiceSDKLoggerImpl;
			WitRequestOptions options = request.Options;
			voiceSDKLogger.LogInteractionStart((options != null) ? options.RequestId : null, (request.InputType == NLPRequestInputType.Text) ? "message" : "speech");
			IVoiceSDKLogger voiceSDKLogger2 = this.voiceSDKLoggerImpl;
			string annotationKey = "minWakeThreshold";
			WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
			voiceSDKLogger2.LogAnnotation(annotationKey, (runtimeConfiguration != null) ? runtimeConfiguration.soundWakeThreshold.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger3 = this.voiceSDKLoggerImpl;
			string annotationKey2 = "minKeepAliveTimeSec";
			WitRuntimeConfiguration runtimeConfiguration2 = this.RuntimeConfiguration;
			voiceSDKLogger3.LogAnnotation(annotationKey2, (runtimeConfiguration2 != null) ? runtimeConfiguration2.minKeepAliveTimeInSeconds.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger4 = this.voiceSDKLoggerImpl;
			string annotationKey3 = "minTranscriptionKeepAliveTimeSec";
			WitRuntimeConfiguration runtimeConfiguration3 = this.RuntimeConfiguration;
			voiceSDKLogger4.LogAnnotation(annotationKey3, (runtimeConfiguration3 != null) ? runtimeConfiguration3.minTranscriptionKeepAliveTimeInSeconds.ToString(CultureInfo.InvariantCulture) : null);
			IVoiceSDKLogger voiceSDKLogger5 = this.voiceSDKLoggerImpl;
			string annotationKey4 = "maxRecordingTime";
			WitRuntimeConfiguration runtimeConfiguration4 = this.RuntimeConfiguration;
			voiceSDKLogger5.LogAnnotation(annotationKey4, (runtimeConfiguration4 != null) ? runtimeConfiguration4.maxRecordingTime.ToString(CultureInfo.InvariantCulture) : null);
		}

		protected override void OnRequestStartListening(VoiceServiceRequest request)
		{
			base.OnRequestStartListening(request);
			this.voiceSDKLoggerImpl.LogInteractionPoint("startedListening");
		}

		protected override void OnRequestStopListening(VoiceServiceRequest request)
		{
			base.OnRequestStopListening(request);
			this.voiceSDKLoggerImpl.LogInteractionPoint("stoppedListening");
		}

		protected override void OnRequestSend(VoiceServiceRequest request)
		{
			base.OnRequestSend(request);
			this.voiceSDKLoggerImpl.LogInteractionPoint("witRequestCreated");
			if (request != null)
			{
				IVoiceSDKLogger voiceSDKLogger = this.voiceSDKLoggerImpl;
				string annotationKey = "requestIdOverride";
				WitRequestOptions options = request.Options;
				voiceSDKLogger.LogAnnotation(annotationKey, (options != null) ? options.RequestId : null);
			}
		}

		protected override void OnRequestPartialTranscription(VoiceServiceRequest request, string transcription)
		{
			base.OnRequestPartialTranscription(request, transcription);
			this.voiceSDKLoggerImpl.LogFirstTranscriptionTime();
		}

		protected override void OnRequestFullTranscription(VoiceServiceRequest request, string transcription)
		{
			base.OnRequestFullTranscription(request, transcription);
			this.voiceSDKLoggerImpl.LogInteractionPoint("fullTranscriptionTime");
		}

		private void OnMinimumWakeThresholdHit()
		{
			this.voiceSDKLoggerImpl.LogInteractionPoint("minWakeThresholdHit");
		}

		private void OnStoppedListeningDueToTimeout()
		{
			this.voiceSDKLoggerImpl.LogInteractionPoint("stoppedListeningTimeout");
		}

		private void OnStoppedListeningDueToInactivity()
		{
			this.voiceSDKLoggerImpl.LogInteractionPoint("stoppedListeningInactivity");
		}

		private void OnStoppedListeningDueToDeactivation()
		{
			this.voiceSDKLoggerImpl.LogInteractionPoint("stoppedListeningDeactivate");
		}

		private void OnMicDataSent()
		{
			this.voiceSDKLoggerImpl.LogInteractionPoint("micDataSent");
		}

		private void OnAudioDurationTrackerFinished(long timestamp, double audioDuration)
		{
			this.voiceSDKLoggerImpl.LogAnnotation("adt_duration", audioDuration.ToString(CultureInfo.InvariantCulture));
			this.voiceSDKLoggerImpl.LogAnnotation("adt_finished", timestamp.ToString());
		}

		protected override void OnRequestSuccess(VoiceServiceRequest request)
		{
			base.OnRequestSuccess(request);
			WitResponseNode witResponseNode = (request != null) ? request.ResponseData : null;
			WitResponseNode witResponseNode2;
			if (witResponseNode == null)
			{
				witResponseNode2 = null;
			}
			else
			{
				WitResponseNode witResponseNode3 = witResponseNode["speech"];
				witResponseNode2 = ((witResponseNode3 != null) ? witResponseNode3["tokens"] : null);
			}
			WitResponseNode witResponseNode4 = witResponseNode2;
			if (witResponseNode4 != null)
			{
				int count = witResponseNode4.Count;
				WitResponseNode witResponseNode5 = witResponseNode4[count - 1];
				string text;
				if (witResponseNode5 == null)
				{
					text = null;
				}
				else
				{
					WitResponseNode witResponseNode6 = witResponseNode5["end"];
					text = ((witResponseNode6 != null) ? witResponseNode6.Value : null);
				}
				string annotationValue = text;
				this.voiceSDKLoggerImpl.LogAnnotation("audioLength", annotationValue);
			}
		}

		protected override void OnRequestComplete(VoiceServiceRequest request)
		{
			base.OnRequestComplete(request);
			if (this.voiceSDKLoggerImpl == null)
			{
				VLog.W("voiceSDKLoggerImpl is null", null);
				return;
			}
			if (request.State == VoiceRequestState.Failed)
			{
				this.voiceSDKLoggerImpl.LogInteractionEndFailure(request.Results.Message);
				return;
			}
			if (request.State == VoiceRequestState.Canceled)
			{
				this.voiceSDKLoggerImpl.LogInteractionEndFailure("aborted");
				return;
			}
			this.voiceSDKLoggerImpl.LogInteractionEndSuccess();
		}

		[SerializeField]
		private WitRuntimeConfiguration witRuntimeConfiguration;

		[Tooltip("Uses platform services to access wit.ai instead of accessing wit directly from within the application.")]
		[SerializeField]
		private bool usePlatformServices;

		[Tooltip("Enables logs related to the interaction to be displayed on console")]
		[SerializeField]
		private bool enableConsoleLogging;

		[Tooltip("If true, the OnFullTranscriptionEvent events will be triggered when calling Activate(string)")]
		[SerializeField]
		private bool sendTranscriptionEventsForMessages;

		private IVoiceService voiceServiceImpl;

		private IVoiceSDKLogger voiceSDKLoggerImpl;
	}
}
