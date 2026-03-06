using System;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice;
using Meta.Voice.Net.WebSockets;
using Meta.Voice.Net.WebSockets.Requests;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;

namespace Meta.WitAi.Requests
{
	[Serializable]
	public class WitSocketRequest : VoiceServiceRequest, IAudioUploadHandler, IDataUploadHandler, ILogSource
	{
		public WitConfiguration Configuration { get; private set; }

		public WitWebSocketAdapter WebSocketAdapter { get; private set; }

		public AudioBuffer AudioInput { get; private set; }

		public string Endpoint { get; set; }

		public WitAudioRequestOption AudioRequestOption { get; private set; }

		public AudioEncoding AudioEncoding { get; set; }

		public bool IsInputStreamReady { get; private set; }

		public Action OnInputStreamReady { get; set; }

		protected override bool DecodeRawResponses
		{
			get
			{
				return false;
			}
		}

		public WitWebSocketMessageRequest WebSocketRequest { get; private set; }

		private WitSocketRequest(NLPRequestInputType inputType, WitRequestOptions options = null, VoiceServiceRequestEvents events = null) : base(NLPRequestInputType.Text, options, events)
		{
		}

		~WitSocketRequest()
		{
			this.SetWebSocketRequest(null);
		}

		public static WitSocketRequest GetMessageRequest(WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, WitRequestOptions options = null, VoiceServiceRequestEvents events = null)
		{
			WitSocketRequest witSocketRequest = new WitSocketRequest(NLPRequestInputType.Text, options, events);
			witSocketRequest.Init(configuration.GetEndpointInfo().Message, WitAudioRequestOption.None, configuration, webSocketAdapter, null);
			return witSocketRequest;
		}

		public static WitSocketRequest GetSpeechRequest(WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, AudioBuffer audioBuffer, WitRequestOptions options = null, VoiceServiceRequestEvents events = null)
		{
			WitSocketRequest witSocketRequest = new WitSocketRequest(NLPRequestInputType.Audio, options, events);
			witSocketRequest.Init(configuration.GetEndpointInfo().Speech, WitAudioRequestOption.Speech, configuration, webSocketAdapter, audioBuffer);
			return witSocketRequest;
		}

		public static WitSocketRequest GetExternalRequest(WitWebSocketMessageRequest webSocketRequest, WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, WitRequestOptions options = null, VoiceServiceRequestEvents events = null)
		{
			WitSocketRequest witSocketRequest = new WitSocketRequest(NLPRequestInputType.Text, options, events);
			witSocketRequest.SetWebSocketRequest(webSocketRequest);
			witSocketRequest.Init(webSocketRequest.Endpoint, WitAudioRequestOption.None, configuration, webSocketAdapter, null);
			witSocketRequest.Results.ResponseData = webSocketRequest.ResponseData;
			return witSocketRequest;
		}

		public static WitSocketRequest GetTranscribeRequest(WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, AudioBuffer audioBuffer, WitRequestOptions options = null, VoiceServiceRequestEvents events = null)
		{
			WitSocketRequest witSocketRequest = new WitSocketRequest(NLPRequestInputType.Audio, options, events);
			witSocketRequest.Init("transcribe", WitAudioRequestOption.Transcribe, configuration, webSocketAdapter, audioBuffer);
			return witSocketRequest;
		}

		public static WitSocketRequest GetDictationRequest(WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, AudioBuffer audioBuffer, WitRequestOptions options = null, VoiceServiceRequestEvents events = null)
		{
			WitSocketRequest witSocketRequest = new WitSocketRequest(NLPRequestInputType.Audio, options, events);
			witSocketRequest.Init("transcribe", WitAudioRequestOption.Dictation, configuration, webSocketAdapter, audioBuffer);
			return witSocketRequest;
		}

		private void Init(string endpoint, WitAudioRequestOption audioOption, WitConfiguration configuration, WitWebSocketAdapter webSocketAdapter, AudioBuffer audioBuffer)
		{
			this.Endpoint = endpoint;
			this.AudioRequestOption = audioOption;
			this.Configuration = configuration;
			this.WebSocketAdapter = webSocketAdapter;
			this.AudioInput = audioBuffer;
			base.Options.InputType = ((audioOption == WitAudioRequestOption.None) ? NLPRequestInputType.Text : NLPRequestInputType.Audio);
			this._initialized = true;
			this.SetState(VoiceRequestState.Initialized);
		}

		protected override void SetState(VoiceRequestState newState)
		{
			if (this._initialized)
			{
				base.SetState(newState);
			}
		}

		protected override string GetSendError()
		{
			if (this.Configuration == null)
			{
				return "Cannot send request without a valid configuration.";
			}
			if (this.AudioInput == null && base.Options.InputType == NLPRequestInputType.Audio)
			{
				return "No audio input provided";
			}
			return base.GetSendError();
		}

		protected override void HandleSend()
		{
			if (base.Options.InputType == NLPRequestInputType.Text)
			{
				base.Options.QueryParams["q"] = base.Options.Text;
				WitWebSocketMessageRequest webSocketRequest = new WitWebSocketMessageRequest(this.Endpoint, base.Options.QueryParams, base.Options.RequestId, base.Options.ClientUserId, base.Options.OperationId, false);
				this.SetWebSocketRequest(webSocketRequest);
			}
			else if (base.Options.InputType == NLPRequestInputType.Audio)
			{
				base.Options.QueryParams["content_type"] = this.AudioEncoding.ToString();
				WitWebSocketMessageRequest witWebSocketMessageRequest = this.CreateAudioWebSocketRequest();
				if (witWebSocketMessageRequest != null)
				{
					this.SetWebSocketRequest(witWebSocketMessageRequest);
				}
			}
			if (this.WebSocketRequest == null || this.WebSocketAdapter == null)
			{
				return;
			}
			this.WebSocketRequest.TimeoutMs = base.Options.TimeoutMs;
			this.WebSocketAdapter.SendRequest(this.WebSocketRequest);
		}

		private WitWebSocketMessageRequest CreateAudioWebSocketRequest()
		{
			switch (this.AudioRequestOption)
			{
			case WitAudioRequestOption.Speech:
				return new WitWebSocketSpeechRequest(this.Endpoint, base.Options.QueryParams, base.Options.RequestId, base.Options.ClientUserId, base.Options.OperationId, false);
			case WitAudioRequestOption.Transcribe:
				return new WitWebSocketTranscribeRequest(this.Endpoint, base.Options.QueryParams, base.Options.RequestId, base.Options.ClientUserId, base.Options.OperationId, false);
			case WitAudioRequestOption.Dictation:
				return new WitWebSocketTranscribeRequest(this.Endpoint, base.Options.QueryParams, base.Options.RequestId, base.Options.ClientUserId, base.Options.OperationId, true);
			default:
				return null;
			}
		}

		private void SetWebSocketRequest(WitWebSocketMessageRequest request)
		{
			if (this.WebSocketRequest != null)
			{
				WitWebSocketMessageRequest webSocketRequest = this.WebSocketRequest;
				webSocketRequest.OnRawResponse = (Action<string>)Delegate.Remove(webSocketRequest.OnRawResponse, new Action<string>(this.ReturnRawResponse));
				WitWebSocketMessageRequest webSocketRequest2 = this.WebSocketRequest;
				webSocketRequest2.OnFirstResponse = (Action<IWitWebSocketRequest>)Delegate.Remove(webSocketRequest2.OnFirstResponse, new Action<IWitWebSocketRequest>(this.ReturnInputReady));
				this.WebSocketRequest.OnDecodedResponse -= this.ReturnDecodedResponse;
				WitWebSocketMessageRequest webSocketRequest3 = this.WebSocketRequest;
				webSocketRequest3.OnComplete = (Action<IWitWebSocketRequest>)Delegate.Remove(webSocketRequest3.OnComplete, new Action<IWitWebSocketRequest>(this.ReturnSuccessOrError));
			}
			this.WebSocketRequest = request;
			if (this.WebSocketRequest != null)
			{
				WitWebSocketMessageRequest webSocketRequest4 = this.WebSocketRequest;
				webSocketRequest4.OnRawResponse = (Action<string>)Delegate.Combine(webSocketRequest4.OnRawResponse, new Action<string>(this.ReturnRawResponse));
				WitWebSocketMessageRequest webSocketRequest5 = this.WebSocketRequest;
				webSocketRequest5.OnFirstResponse = (Action<IWitWebSocketRequest>)Delegate.Combine(webSocketRequest5.OnFirstResponse, new Action<IWitWebSocketRequest>(this.ReturnInputReady));
				this.WebSocketRequest.OnDecodedResponse += this.ReturnDecodedResponse;
				WitWebSocketMessageRequest webSocketRequest6 = this.WebSocketRequest;
				webSocketRequest6.OnComplete = (Action<IWitWebSocketRequest>)Delegate.Combine(webSocketRequest6.OnComplete, new Action<IWitWebSocketRequest>(this.ReturnSuccessOrError));
			}
			if (this._simulatedErrorType != (VoiceErrorSimulationType)(-1))
			{
				this.WebSocketRequest.SimulatedErrorType = this._simulatedErrorType;
			}
		}

		private void ReturnRawResponse(string rawResponse)
		{
			this.HandleRawResponse(rawResponse, false);
		}

		private void ReturnInputReady(IWitWebSocketRequest request)
		{
			WitWebSocketSpeechRequest witWebSocketSpeechRequest = request as WitWebSocketSpeechRequest;
			if (witWebSocketSpeechRequest != null && witWebSocketSpeechRequest.IsReadyForInput)
			{
				this.IsInputStreamReady = true;
				Action onInputStreamReady = this.OnInputStreamReady;
				if (onInputStreamReady == null)
				{
					return;
				}
				onInputStreamReady();
			}
		}

		private void ReturnDecodedResponse(WitResponseNode responseNode)
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.ApplyResponseData(responseNode, false);
			});
		}

		private void ReturnSuccessOrError(IWitWebSocketRequest request)
		{
			if (!base.IsActive)
			{
				return;
			}
			if (string.IsNullOrEmpty(request.Error))
			{
				this.ApplyResponseData(base.ResponseData, true);
				return;
			}
			int num = request.Code;
			if (num == 0)
			{
				num = -1;
			}
			this.HandleFailure(num, request.Error);
		}

		protected override void HandleCancel()
		{
			if (this.WebSocketRequest != null)
			{
				this.WebSocketRequest.Cancel();
			}
		}

		protected override string GetActivateAudioError()
		{
			string activateAudioError = base.GetActivateAudioError();
			if (!string.IsNullOrEmpty(activateAudioError))
			{
				return activateAudioError;
			}
			if (this.AudioInput == null && base.Options.InputType == NLPRequestInputType.Audio)
			{
				return "No audio input provided";
			}
			return string.Empty;
		}

		protected override void HandleAudioActivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.On);
		}

		public void Write(byte[] buffer, int offset, int length)
		{
			if (!base.IsListening)
			{
				return;
			}
			WitWebSocketSpeechRequest witWebSocketSpeechRequest = this.WebSocketRequest as WitWebSocketSpeechRequest;
			if (witWebSocketSpeechRequest != null)
			{
				witWebSocketSpeechRequest.SendAudioData(buffer, offset, length);
			}
		}

		protected override void HandleAudioDeactivation()
		{
			bool flag = base.InputType == NLPRequestInputType.Audio && this.WebSocketRequest == null;
			WitWebSocketSpeechRequest witWebSocketSpeechRequest = this.WebSocketRequest as WitWebSocketSpeechRequest;
			if (witWebSocketSpeechRequest != null)
			{
				witWebSocketSpeechRequest.CloseAudioStream();
				flag = (!witWebSocketSpeechRequest.HasSentAudio && !witWebSocketSpeechRequest.IsComplete);
			}
			this.SetAudioInputState(VoiceAudioInputState.Off);
			if (flag)
			{
				this.Logger.Verbose("Audio input disabled prior to transmission\nRequest Id: {0}\n", base.Options.RequestId, null, null, null, "HandleAudioDeactivation", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Requests\\WitSocketRequest.cs", 458);
				this.Cancel("Request cancelled prior to transmission begin");
			}
		}

		internal override void SimulateError(VoiceErrorSimulationType errorType)
		{
			this._simulatedErrorType = errorType;
			if (this.WebSocketRequest != null)
			{
				this.WebSocketRequest.SimulatedErrorType = this._simulatedErrorType;
			}
		}

		private bool _initialized;

		internal VoiceErrorSimulationType _simulatedErrorType = (VoiceErrorSimulationType)(-1);
	}
}
