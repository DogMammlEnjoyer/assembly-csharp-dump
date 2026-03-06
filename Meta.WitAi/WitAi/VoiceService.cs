using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Conduit;
using Meta.Voice;
using Meta.Voice.TelemetryUtilities;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Events;
using Meta.WitAi.Events.UnityEventListeners;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi
{
	public abstract class VoiceService : BaseSpeechService, IVoiceService, IVoiceEventProvider, ITelemetryEventsProvider, IVoiceActivationHandler, IInstanceResolver, IAudioEventProvider
	{
		private bool UseIntentAttributes
		{
			get
			{
				return this.WitConfiguration && this.WitConfiguration.useIntentAttributes;
			}
		}

		private bool UseConduit
		{
			get
			{
				return this.UseIntentAttributes && this.WitConfiguration.useConduit;
			}
		}

		public virtual bool UsePlatformIntegrations
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public WitConfiguration WitConfiguration
		{
			get
			{
				if (this._witConfiguration == null)
				{
					IWitConfigurationProvider component = base.GetComponent<IWitConfigurationProvider>();
					this._witConfiguration = ((component != null) ? component.Configuration : null);
				}
				return this._witConfiguration;
			}
			set
			{
				this._witConfiguration = value;
			}
		}

		internal IConduitDispatcher ConduitDispatcher { get; set; }

		public virtual bool IsRequestActive
		{
			get
			{
				return base.Active;
			}
		}

		public abstract ITranscriptionProvider TranscriptionProvider { get; set; }

		public abstract bool MicActive { get; }

		public virtual VoiceEvents VoiceEvents
		{
			get
			{
				return this.events;
			}
			set
			{
				this.events = value;
			}
		}

		protected override SpeechEvents GetSpeechEvents()
		{
			return this.VoiceEvents;
		}

		public virtual TelemetryEvents TelemetryEvents
		{
			get
			{
				return this.telemetryEvents;
			}
			set
			{
				this.telemetryEvents = value;
			}
		}

		public IAudioInputEvents AudioEvents
		{
			get
			{
				return this.VoiceEvents;
			}
		}

		public ITranscriptionEvent TranscriptionEvents
		{
			get
			{
				return this.VoiceEvents;
			}
		}

		protected abstract bool ShouldSendMicData { get; }

		protected VoiceService()
		{
			this._conduitParameterProvider.SetSpecializedParameter("@WitResponseNode", typeof(WitResponseNode));
			this._conduitParameterProvider.SetSpecializedParameter("@VoiceSession", typeof(VoiceSession));
			ConduitDispatcherFactory conduitDispatcherFactory = new ConduitDispatcherFactory(this);
			this.ConduitDispatcher = conduitDispatcherFactory.GetDispatcher();
		}

		public void Activate(string text)
		{
			VoiceService.<>c__DisplayClass39_0 CS$<>8__locals1 = new VoiceService.<>c__DisplayClass39_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.text = text;
			ThreadUtility.BackgroundAsync<VoiceServiceRequest>(base.Logger, delegate()
			{
				VoiceService.<>c__DisplayClass39_0.<<Activate>b__0>d <<Activate>b__0>d;
				<<Activate>b__0>d.<>t__builder = AsyncTaskMethodBuilder<VoiceServiceRequest>.Create();
				<<Activate>b__0>d.<>4__this = CS$<>8__locals1;
				<<Activate>b__0>d.<>1__state = -1;
				<<Activate>b__0>d.<>t__builder.Start<VoiceService.<>c__DisplayClass39_0.<<Activate>b__0>d>(ref <<Activate>b__0>d);
				return <<Activate>b__0>d.<>t__builder.Task;
			}).WrapErrors();
		}

		public Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions)
		{
			return this.Activate(text, requestOptions, new VoiceServiceRequestEvents());
		}

		public Task<VoiceServiceRequest> Activate(string text, VoiceServiceRequestEvents requestEvents)
		{
			return this.Activate(text, new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), requestEvents);
		}

		public abstract Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents);

		public void Activate()
		{
			this.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()));
		}

		public void Activate(WitRequestOptions requestOptions)
		{
			this.Activate(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest Activate(VoiceServiceRequestEvents requestEvents)
		{
			return this.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), requestEvents);
		}

		public abstract VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents);

		public void ActivateImmediately()
		{
			this.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()));
		}

		public void ActivateImmediately(WitRequestOptions requestOptions)
		{
			this.ActivateImmediately(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest ActivateImmediately(VoiceServiceRequestEvents requestEvents)
		{
			return this.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), requestEvents);
		}

		public abstract VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents);

		protected override void OnRequestPartialResponse(VoiceServiceRequest request, WitResponseNode responseNode)
		{
			if (this._waitingForFirstPartialAudio)
			{
				this._waitingForFirstPartialAudio = false;
				RuntimeTelemetry.Instance.LogPoint((OperationID)request.Options.OperationId, RuntimeTelemetryPoint.FirstPartialAudioFromServer);
			}
			base.OnRequestPartialResponse(request, responseNode);
			this.OnValidateEarly(request, responseNode);
		}

		protected override void OnRequestSend(VoiceServiceRequest request)
		{
			this._waitingForFirstPartialAudio = true;
			base.OnRequestSend(request);
		}

		protected virtual void OnValidateEarly(VoiceServiceRequest request, WitResponseNode responseNode)
		{
			if (request == null || request.State != VoiceRequestState.Transmitting || responseNode == null || this.VoiceEvents.OnValidatePartialResponse == null)
			{
				return;
			}
			VoiceSession voiceSession = this.GetVoiceSession(responseNode);
			this.VoiceEvents.OnValidatePartialResponse.Invoke(voiceSession);
			if (this.UseConduit)
			{
				WitIntentData firstIntentData = responseNode.GetFirstIntentData();
				if (firstIntentData != null)
				{
					this._conduitParameterProvider.PopulateParametersFromNode(responseNode);
					this._conduitParameterProvider.AddParameter("@VoiceSession", voiceSession);
					this._conduitParameterProvider.AddParameter("@WitResponseNode", responseNode);
					this.ConduitDispatcher.InvokeAction(this._conduitParameterProvider, firstIntentData.name, this._witConfiguration.relaxedResolution, firstIntentData.confidence, true);
				}
			}
			if (voiceSession.validResponse)
			{
				VLog.I("Validated Early");
				request.CompleteEarly();
			}
		}

		public IEnumerable<object> GetObjectsOfType(Type type)
		{
			return Object.FindObjectsByType(type, FindObjectsSortMode.None);
		}

		protected virtual void Awake()
		{
			this.InitializeEventListeners();
		}

		private void InitializeEventListeners()
		{
			if (!base.GetComponent<AudioEventListener>())
			{
				base.gameObject.AddComponent<AudioEventListener>();
			}
			if (!base.GetComponent<TranscriptionEventListener>())
			{
				base.gameObject.AddComponent<TranscriptionEventListener>();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.UseConduit)
			{
				this.InitializeConduit().WrapErrors();
			}
			else if (this.UseIntentAttributes)
			{
				MatchIntentRegistry.Initialize();
			}
			ITranscriptionProvider transcriptionProvider = this.TranscriptionProvider;
			if (transcriptionProvider != null)
			{
				transcriptionProvider.OnFullTranscription.AddListener(new UnityAction<string>(this.OnFinalTranscription));
			}
			this.VoiceEvents.OnResponse.AddListener(new UnityAction<WitResponseNode>(this.HandleResponse));
		}

		private Task InitializeConduit()
		{
			VoiceService.<InitializeConduit>d__59 <InitializeConduit>d__;
			<InitializeConduit>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<InitializeConduit>d__.<>4__this = this;
			<InitializeConduit>d__.<>1__state = -1;
			<InitializeConduit>d__.<>t__builder.Start<VoiceService.<InitializeConduit>d__59>(ref <InitializeConduit>d__);
			return <InitializeConduit>d__.<>t__builder.Task;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			ITranscriptionProvider transcriptionProvider = this.TranscriptionProvider;
			if (transcriptionProvider != null)
			{
				transcriptionProvider.OnFullTranscription.RemoveListener(new UnityAction<string>(this.OnFinalTranscription));
			}
			this.VoiceEvents.OnResponse.RemoveListener(new UnityAction<WitResponseNode>(this.HandleResponse));
		}

		protected virtual void OnFinalTranscription(string transcription)
		{
			if (this.TranscriptionProvider != null)
			{
				this.Activate(transcription);
			}
		}

		private VoiceSession GetVoiceSession(WitResponseNode response)
		{
			return new VoiceSession
			{
				service = this,
				response = response,
				validResponse = false
			};
		}

		protected virtual void HandleResponse(WitResponseNode response)
		{
			this.HandleIntents(response);
		}

		private void HandleIntents(WitResponseNode response)
		{
			foreach (WitIntentData intent in response.GetIntents())
			{
				this.HandleIntent(intent, response);
			}
		}

		private void HandleIntent(WitIntentData intent, WitResponseNode response)
		{
			if (this.UseConduit)
			{
				this._conduitParameterProvider.PopulateParametersFromNode(response);
				this._conduitParameterProvider.AddParameter("@WitResponseNode", response);
				this.ConduitDispatcher.InvokeAction(this._conduitParameterProvider, intent.name, this._witConfiguration.relaxedResolution, intent.confidence, false);
				return;
			}
			if (this.UseIntentAttributes)
			{
				foreach (RegisteredMatchIntent registeredMethod in MatchIntentRegistry.RegisteredMethods[intent.name])
				{
					this.ExecuteRegisteredMatch(registeredMethod, intent, response);
				}
			}
		}

		private void ExecuteRegisteredMatch(RegisteredMatchIntent registeredMethod, WitIntentData intent, WitResponseNode response)
		{
			if (intent.confidence >= registeredMethod.matchIntent.MinConfidence && intent.confidence <= registeredMethod.matchIntent.MaxConfidence)
			{
				foreach (object obj in this.GetObjectsOfType(registeredMethod.type))
				{
					ParameterInfo[] parameters = registeredMethod.method.GetParameters();
					if (parameters.Length == 0)
					{
						registeredMethod.method.Invoke(obj, Array.Empty<object>());
					}
					else if (parameters[0].ParameterType != typeof(WitResponseNode) || parameters.Length > 2)
					{
						VLog.E("Match intent only supports methods with no parameters or with a WitResponseNode parameter. Enable Conduit or adjust the parameters", null);
					}
					else if (parameters.Length == 1)
					{
						registeredMethod.method.Invoke(obj, new object[]
						{
							response
						});
					}
				}
			}
		}

		private WitConfiguration _witConfiguration;

		private readonly IParameterProvider _conduitParameterProvider = new ParameterProvider();

		[Tooltip("Events that will fire before, during and after an activation")]
		[SerializeField]
		protected VoiceEvents events = new VoiceEvents();

		protected TelemetryEvents telemetryEvents = new TelemetryEvents();

		protected bool _waitingForFirstPartialAudio = true;
	}
}
