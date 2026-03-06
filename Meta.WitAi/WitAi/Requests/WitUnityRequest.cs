using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data.Configuration;

namespace Meta.WitAi.Requests
{
	[Serializable]
	public class WitUnityRequest : VoiceServiceRequest
	{
		public WitConfiguration Configuration { get; private set; }

		public string Endpoint { get; set; }

		public bool ShouldPost { get; set; }

		protected override bool DecodeRawResponses
		{
			get
			{
				return true;
			}
		}

		public WitUnityRequest(WitConfiguration newConfiguration, NLPRequestInputType newDataType, WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents) : base(newDataType, newOptions, newEvents)
		{
			this.Configuration = newConfiguration;
			if (base.InputType == NLPRequestInputType.Text)
			{
				this._request = new WitMessageVRequest(this.Configuration, newOptions.RequestId, newOptions.OperationId);
				this._request.OnDownloadProgress += base.SetDownloadProgress;
				this.Endpoint = this.Configuration.GetEndpointInfo().Message;
				this.ShouldPost = false;
			}
			else if (base.InputType == NLPRequestInputType.Audio)
			{
				this.Endpoint = this.Configuration.GetEndpointInfo().Speech;
				this.ShouldPost = true;
			}
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
			if (this._request == null)
			{
				return "Request creation failed.";
			}
			return base.GetSendError();
		}

		protected override void HandleSend()
		{
			WitUnityRequest.<>c__DisplayClass19_0 CS$<>8__locals1 = new WitUnityRequest.<>c__DisplayClass19_0();
			CS$<>8__locals1.<>4__this = this;
			WitVRequest request = this._request;
			CS$<>8__locals1.messageRequest = (request as WitMessageVRequest);
			if (CS$<>8__locals1.messageRequest != null)
			{
				this._request.TimeoutMs = base.Options.TimeoutMs;
				ThreadUtility.BackgroundAsync(this.Logger, delegate()
				{
					WitUnityRequest.<>c__DisplayClass19_0.<<HandleSend>b__0>d <<HandleSend>b__0>d;
					<<HandleSend>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<HandleSend>b__0>d.<>4__this = CS$<>8__locals1;
					<<HandleSend>b__0>d.<>1__state = -1;
					<<HandleSend>b__0>d.<>t__builder.Start<WitUnityRequest.<>c__DisplayClass19_0.<<HandleSend>b__0>d>(ref <<HandleSend>b__0>d);
					return <<HandleSend>b__0>d.<>t__builder.Task;
				});
			}
		}

		private Task SendMessageAsync(WitMessageVRequest messageRequest)
		{
			WitUnityRequest.<SendMessageAsync>d__20 <SendMessageAsync>d__;
			<SendMessageAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendMessageAsync>d__.<>4__this = this;
			<SendMessageAsync>d__.messageRequest = messageRequest;
			<SendMessageAsync>d__.<>1__state = -1;
			<SendMessageAsync>d__.<>t__builder.Start<WitUnityRequest.<SendMessageAsync>d__20>(ref <SendMessageAsync>d__);
			return <SendMessageAsync>d__.<>t__builder.Task;
		}

		private void HandlePartialResponse(string rawResponse)
		{
			this.HandleResponse(rawResponse, null, false);
		}

		private void HandleFinalResponse(string rawResponse, string error)
		{
			this.HandleResponse(rawResponse, error, true);
		}

		protected void HandleResponse(string rawResponse, string error, bool final)
		{
			if (!string.IsNullOrEmpty(error))
			{
				if (final)
				{
					int errorStatusCode = (this._request == null) ? -1 : this._request.ResponseCode;
					this.HandleFailure(errorStatusCode, error);
				}
				return;
			}
			if (final)
			{
				this.MakeLastResponseFinal();
				return;
			}
			string[] array = rawResponse.Split("\r\n", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.HandleRawResponse(array[i], final && i == array.Length - 1);
			}
		}

		protected override void HandleCancel()
		{
			if (this._request != null)
			{
				this._request.Cancel();
			}
		}

		protected override void OnComplete()
		{
			base.OnComplete();
			if (!this._request.IsComplete)
			{
				this._request.Cancel();
			}
		}

		protected override string GetActivateAudioError()
		{
			return "Audio request not yet implemented";
		}

		protected override void HandleAudioActivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.On);
		}

		protected override void HandleAudioDeactivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.Off);
		}

		private readonly WitVRequest _request;

		private bool _initialized;
	}
}
