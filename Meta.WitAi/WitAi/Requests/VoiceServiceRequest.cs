using System;
using System.Runtime.CompilerServices;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Json;

namespace Meta.WitAi.Requests
{
	[Serializable]
	public abstract class VoiceServiceRequest : NLPRequest<VoiceServiceRequestEvent, WitRequestOptions, VoiceServiceRequestEvents, VoiceServiceRequestResults, WitResponseNode>
	{
		protected VoiceServiceRequest(NLPRequestInputType newInputType, WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents) : base(newInputType, newOptions, newEvents)
		{
		}

		public bool IsLocalRequest
		{
			get
			{
				return string.Equals(base.Options.ClientUserId, WitRequestSettings.LocalClientUserId);
			}
		}

		public int StatusCode
		{
			get
			{
				return base.Results.StatusCode;
			}
		}

		protected override INLPRequestResponseDecoder<WitResponseNode> ResponseDecoder
		{
			get
			{
				return VoiceServiceRequest._responseDecoder;
			}
		}

		protected override bool ShouldIgnoreError(int errorStatusCode, string errorMessage)
		{
			return base.ShouldIgnoreError(errorStatusCode, errorMessage) || string.Equals(errorMessage, "Empty transcription.");
		}

		protected override bool OnSimulateResponse()
		{
			if (VoiceRequest<VoiceServiceRequestEvent, WitRequestOptions, VoiceServiceRequestEvents, VoiceServiceRequestResults>.simulatedResponse == null)
			{
				return false;
			}
			this.SimulateResponse();
			return true;
		}

		private void SimulateResponse()
		{
			VoiceServiceRequest.<SimulateResponse>d__10 <SimulateResponse>d__;
			<SimulateResponse>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SimulateResponse>d__.<>4__this = this;
			<SimulateResponse>d__.<>1__state = -1;
			<SimulateResponse>d__.<>t__builder.Start<VoiceServiceRequest.<SimulateResponse>d__10>(ref <SimulateResponse>d__);
		}

		internal virtual void SimulateError(VoiceErrorSimulationType errorType)
		{
			throw new NotImplementedException();
		}

		protected override void ApplyResponseData(WitResponseNode responseData, bool isFinal)
		{
			if (responseData != null)
			{
				string aKey = "client_request_id";
				WitRequestOptions options = base.Options;
				responseData[aKey] = ((options != null) ? options.RequestId : null);
				string aKey2 = "client_user_id";
				WitRequestOptions options2 = base.Options;
				responseData[aKey2] = ((options2 != null) ? options2.ClientUserId : null);
				string aKey3 = "operation_id";
				WitRequestOptions options3 = base.Options;
				responseData[aKey3] = ((options3 != null) ? options3.OperationId : null);
			}
			base.ApplyResponseData(responseData, isFinal);
		}

		protected override void SetEventListeners(VoiceServiceRequestEvents newEvents, bool add)
		{
			base.Events.SetListeners(newEvents, add);
		}

		protected override void RaiseEvent(VoiceServiceRequestEvent eventCallback)
		{
			ThreadUtility.CallOnMainThread(delegate()
			{
				VoiceServiceRequestEvent eventCallback2 = eventCallback;
				if (eventCallback2 == null)
				{
					return;
				}
				eventCallback2.Invoke(this);
			});
		}

		private static WitResponseDecoder _responseDecoder = new WitResponseDecoder();
	}
}
