using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Meta.Voice.Logging;
using Meta.WitAi;
using UnityEngine.Events;

namespace Meta.Voice
{
	[LogCategory(LogCategory.Network)]
	public abstract class NLPRequest<TUnityEvent, TOptions, TEvents, TResults, TResponseData> : TranscriptionRequest<TUnityEvent, TOptions, TEvents, TResults> where TUnityEvent : UnityEventBase where TOptions : INLPRequestOptions where TEvents : NLPRequestEvents<TUnityEvent, TResponseData> where TResults : INLPRequestResults<TResponseData>
	{
		public override IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Network, null);

		public NLPRequestInputType InputType
		{
			get
			{
				if (base.Options != null)
				{
					TOptions options = base.Options;
					return options.InputType;
				}
				return NLPRequestInputType.Audio;
			}
		}

		public TResponseData ResponseData
		{
			get
			{
				if (base.Results != null)
				{
					TResults results = base.Results;
					return results.ResponseData;
				}
				return default(TResponseData);
			}
		}

		protected NLPRequest(NLPRequestInputType inputType, TOptions options, TEvents newEvents) : base(options, newEvents)
		{
			TOptions options2 = base.Options;
			options2.InputType = inputType;
			this._initialized = true;
			this._finalized = false;
			this.SetState(VoiceRequestState.Initialized);
		}

		protected override void SetState(VoiceRequestState newState)
		{
			if (this._initialized)
			{
				base.SetState(newState);
			}
		}

		protected override void Log(string log, VLoggerVerbosity logLevel = VLoggerVerbosity.Info)
		{
			ICoreLogger logger = this.Logger;
			CorrelationID correlationID = this.Logger.CorrelationID;
			string message = "{0}\nRequest Id: {1}\nRequest State: {2}\nAudio Input State: {3}\nTranscription: {4}\nInput: {5}";
			object[] array = new object[6];
			array[0] = log;
			int num = 1;
			TOptions options = base.Options;
			array[num] = ((options != null) ? options.RequestId : null);
			array[2] = base.State;
			array[3] = base.AudioInputState;
			int num2 = 4;
			TResults results = base.Results;
			array[num2] = ((results != null) ? results.Transcription : null);
			array[5] = this.InputType;
			logger.Log(correlationID, logLevel, message, array);
		}

		protected override string GetActivateAudioError()
		{
			if (this.InputType == NLPRequestInputType.Text)
			{
				return "Cannot activate audio on a text request";
			}
			return string.Empty;
		}

		protected override string GetSendError()
		{
			if (this.InputType == NLPRequestInputType.Audio && !base.IsAudioInputActivated)
			{
				return "Cannot send audio without activation";
			}
			return base.GetSendError();
		}

		protected virtual INLPRequestResponseDecoder<TResponseData> ResponseDecoder
		{
			get
			{
				return null;
			}
		}

		protected virtual bool DecodeRawResponses
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsDecoding
		{
			get
			{
				return this._rawQueued > this._rawDecoded;
			}
		}

		protected virtual void HandleRawResponse(string rawResponse, bool final)
		{
			if (!base.IsActive)
			{
				return;
			}
			if (string.IsNullOrEmpty(rawResponse))
			{
				if (final && this.DecodeRawResponses)
				{
					this.HandleFailure("Final response is empty");
				}
				return;
			}
			if (string.Equals(this._rawResponseLast, rawResponse))
			{
				return;
			}
			this._rawResponseFinal = (this._rawResponseFinal || final);
			this._rawResponseLast = rawResponse;
			this.OnRawResponse(rawResponse);
			if (this.DecodeRawResponses && this.ResponseDecoder != null)
			{
				this.EnqueueDecode(rawResponse, final);
			}
		}

		protected virtual void OnRawResponse(string rawResponse)
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				TEvents tevents = this.Events;
				if (tevents == null)
				{
					return;
				}
				TranscriptionRequestEvent onRawResponse = tevents.OnRawResponse;
				if (onRawResponse == null)
				{
					return;
				}
				onRawResponse.Invoke(rawResponse);
			});
		}

		private void EnqueueDecode(string rawResponse, bool final)
		{
			NLPRequest<TUnityEvent, TOptions, TEvents, TResults, TResponseData>.<>c__DisplayClass29_0 CS$<>8__locals1 = new NLPRequest<TUnityEvent, TOptions, TEvents, TResults, TResponseData>.<>c__DisplayClass29_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.rawResponse = rawResponse;
			CS$<>8__locals1.final = final;
			this._rawQueued++;
			CS$<>8__locals1.blockingTask = this._lastDecode;
			this._lastDecode = ThreadUtility.BackgroundAsync(this.Logger, delegate()
			{
				NLPRequest<TUnityEvent, TOptions, TEvents, TResults, TResponseData>.<>c__DisplayClass29_0.<<EnqueueDecode>b__0>d <<EnqueueDecode>b__0>d;
				<<EnqueueDecode>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<EnqueueDecode>b__0>d.<>4__this = CS$<>8__locals1;
				<<EnqueueDecode>b__0>d.<>1__state = -1;
				<<EnqueueDecode>b__0>d.<>t__builder.Start<NLPRequest<TUnityEvent, TOptions, TEvents, TResults, TResponseData>.<>c__DisplayClass29_0.<<EnqueueDecode>b__0>d>(ref <<EnqueueDecode>b__0>d);
				return <<EnqueueDecode>b__0>d.<>t__builder.Task;
			});
		}

		private void DecodeRawResponse(string rawResponse, bool final)
		{
			TResponseData tresponseData = this.ResponseDecoder.Decode(rawResponse);
			this._rawDecoded++;
			if (!base.IsActive)
			{
				return;
			}
			final |= (this._rawResponseFinal && !this.IsDecoding);
			this._lastResponse = tresponseData;
			this.ApplyResponseData(tresponseData, final);
		}

		protected virtual void ApplyResponseData(TResponseData responseData, bool final)
		{
			if (!base.IsActive)
			{
				return;
			}
			if (final)
			{
				if (this._finalized)
				{
					return;
				}
				this._finalized = true;
			}
			if (responseData == null)
			{
				if (final)
				{
					this.HandleFailure("Failed to decode partial raw response");
				}
				return;
			}
			INLPRequestResponseDecoder<TResponseData> responseDecoder = this.ResponseDecoder;
			string text = (responseDecoder != null) ? responseDecoder.GetResponseError(responseData) : null;
			if (!string.IsNullOrEmpty(text))
			{
				int errorStatusCode = (this.ResponseDecoder == null) ? -1 : this.ResponseDecoder.GetResponseStatusCode(responseData);
				this.HandleFailure(errorStatusCode, text);
				return;
			}
			TResults results = base.Results;
			bool flag = !responseData.Equals(results.ResponseData);
			results = base.Results;
			results.SetResponseData(responseData);
			INLPRequestResponseDecoder<TResponseData> responseDecoder2 = this.ResponseDecoder;
			string transcription = (responseDecoder2 != null) ? responseDecoder2.GetResponseTranscription(responseData) : null;
			bool flag2 = this.ResponseDecoder != null && this.ResponseDecoder.GetResponseHasTranscription(responseData);
			bool full = this.ResponseDecoder != null && this.ResponseDecoder.GetResponseIsTranscriptionFull(responseData);
			if (flag && flag2)
			{
				this.ApplyTranscription(transcription, full);
			}
			bool flag3 = this.ResponseDecoder != null && this.ResponseDecoder.GetResponseHasPartial(responseData);
			if (flag && flag3)
			{
				this.OnPartialResponse(responseData);
			}
			if (final)
			{
				if (!flag3)
				{
					this.OnPartialResponse(responseData);
				}
				StringBuilder stringBuilder = new StringBuilder();
				NLPRequestResponseValidatorEvent<TResponseData> onValidateResponse = base.Events.OnValidateResponse;
				if (onValidateResponse != null)
				{
					onValidateResponse.Invoke(responseData, stringBuilder);
				}
				if (stringBuilder.Length > 0)
				{
					this.HandleFailure(string.Format("Response validation failed due to {0}", stringBuilder));
					return;
				}
				this.OnFullResponse(responseData);
				this.HandleSuccess();
			}
		}

		protected virtual void OnPartialResponse(TResponseData responseData)
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				TEvents tevents = this.Events;
				if (tevents == null)
				{
					return;
				}
				NLPRequestResponseEvent<TResponseData> onPartialResponse = tevents.OnPartialResponse;
				if (onPartialResponse == null)
				{
					return;
				}
				onPartialResponse.Invoke(responseData);
			});
		}

		protected virtual void OnFullResponse(TResponseData responseData)
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				TEvents tevents = this.Events;
				if (tevents == null)
				{
					return;
				}
				NLPRequestResponseEvent<TResponseData> onFullResponse = tevents.OnFullResponse;
				if (onFullResponse == null)
				{
					return;
				}
				onFullResponse.Invoke(responseData);
			});
		}

		public virtual void CompleteEarly()
		{
			if (!base.IsActive || this._finalized)
			{
				return;
			}
			if (this.ResponseData == null)
			{
				this.Cancel("Cannot complete early without response data");
				return;
			}
			this.MakeLastResponseFinal();
		}

		protected virtual void MakeLastResponseFinal()
		{
			if (!base.IsActive)
			{
				return;
			}
			if (this.IsDecoding)
			{
				this._rawResponseFinal = true;
				return;
			}
			this.ApplyResponseData(this._lastResponse, true);
		}

		private bool _initialized;

		private bool _finalized;

		private const int DECODE_DELAY_MS = 5;

		private string _rawResponseLast;

		private int _rawQueued;

		private int _rawDecoded;

		private bool _rawResponseFinal;

		private Task _lastDecode;

		private TResponseData _lastResponse;
	}
}
