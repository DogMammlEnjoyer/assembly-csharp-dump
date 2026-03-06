using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Meta.WitAi.Configuration;

namespace Meta.WitAi.Requests
{
	internal class WitVRequest : VRequest, IWitVRequest, IVRequest
	{
		[Obsolete("Use WitRequestSettings.OnProvideCustomUri instead.")]
		public static Func<UriBuilder, UriBuilder> OnProvideCustomUri
		{
			get
			{
				return WitRequestSettings.OnProvideCustomUri;
			}
		}

		[Obsolete("Use WitRequestSettings.OnProvideCustomHeaders instead.")]
		public static Action<Dictionary<string, string>> OnProvideCustomHeaders
		{
			get
			{
				return WitRequestSettings.OnProvideCustomHeaders;
			}
		}

		[Obsolete("Use WitRequestSettings.OnProvideCustomUserAgent instead.")]
		public static Action<StringBuilder> OnProvideCustomUserAgent
		{
			get
			{
				return WitRequestSettings.OnProvideCustomUserAgent;
			}
		}

		public WitRequestOptions RequestOptions { get; private set; }

		public string RequestId
		{
			get
			{
				return this.RequestOptions.RequestId;
			}
		}

		public IWitRequestConfiguration Configuration { get; private set; }

		public WitVRequest(IWitRequestConfiguration configuration, string requestId, string operationId = null, bool useServerToken = false)
		{
			this.Configuration = configuration;
			this.RequestOptions = new WitRequestOptions(requestId, WitRequestSettings.LocalClientUserId, operationId, Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			base.TimeoutMs = configuration.RequestTimeoutMs;
			this._useServerToken = useServerToken;
		}

		protected bool IsLocalFile()
		{
			return !string.IsNullOrEmpty(base.Url) && base.Url.StartsWith("file://");
		}

		protected override Uri GetUri()
		{
			if (this.IsLocalFile())
			{
				return base.GetUri();
			}
			return WitRequestSettings.GetUri(this.Configuration, base.Url, base.UrlParameters);
		}

		protected override Dictionary<string, string> GetHeaders()
		{
			if (this.IsLocalFile())
			{
				return base.GetHeaders();
			}
			return WitRequestSettings.GetHeaders(this.Configuration, this.RequestOptions, this._useServerToken);
		}

		public override Task<VRequestResponse<TValue>> Request<TValue>(VRequestDecodeDelegate<TValue> decoder)
		{
			WitVRequest.<Request>d__21<TValue> <Request>d__;
			<Request>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TValue>>.Create();
			<Request>d__.<>4__this = this;
			<Request>d__.decoder = decoder;
			<Request>d__.<>1__state = -1;
			<Request>d__.<>t__builder.Start<WitVRequest.<Request>d__21<TValue>>(ref <Request>d__);
			return <Request>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TValue>> RequestWitGet<TValue>(string endpoint, Dictionary<string, string> urlParameters = null, Action<TValue> onPartial = null)
		{
			WitVRequest.<RequestWitGet>d__22<TValue> <RequestWitGet>d__;
			<RequestWitGet>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TValue>>.Create();
			<RequestWitGet>d__.<>4__this = this;
			<RequestWitGet>d__.endpoint = endpoint;
			<RequestWitGet>d__.urlParameters = urlParameters;
			<RequestWitGet>d__.onPartial = onPartial;
			<RequestWitGet>d__.<>1__state = -1;
			<RequestWitGet>d__.<>t__builder.Start<WitVRequest.<RequestWitGet>d__22<TValue>>(ref <RequestWitGet>d__);
			return <RequestWitGet>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TValue>> RequestWitPost<TValue>(string endpoint, Dictionary<string, string> urlParameters, string payload, Action<TValue> onPartial = null)
		{
			WitVRequest.<RequestWitPost>d__23<TValue> <RequestWitPost>d__;
			<RequestWitPost>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TValue>>.Create();
			<RequestWitPost>d__.<>4__this = this;
			<RequestWitPost>d__.endpoint = endpoint;
			<RequestWitPost>d__.urlParameters = urlParameters;
			<RequestWitPost>d__.payload = payload;
			<RequestWitPost>d__.onPartial = onPartial;
			<RequestWitPost>d__.<>1__state = -1;
			<RequestWitPost>d__.<>t__builder.Start<WitVRequest.<RequestWitPost>d__23<TValue>>(ref <RequestWitPost>d__);
			return <RequestWitPost>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TValue>> RequestWitPut<TValue>(string endpoint, Dictionary<string, string> urlParameters, string payload, Action<TValue> onPartial = null)
		{
			WitVRequest.<RequestWitPut>d__24<TValue> <RequestWitPut>d__;
			<RequestWitPut>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TValue>>.Create();
			<RequestWitPut>d__.<>4__this = this;
			<RequestWitPut>d__.endpoint = endpoint;
			<RequestWitPut>d__.urlParameters = urlParameters;
			<RequestWitPut>d__.payload = payload;
			<RequestWitPut>d__.onPartial = onPartial;
			<RequestWitPut>d__.<>1__state = -1;
			<RequestWitPut>d__.<>t__builder.Start<WitVRequest.<RequestWitPut>d__24<TValue>>(ref <RequestWitPut>d__);
			return <RequestWitPut>d__.<>t__builder.Task;
		}

		private bool _useServerToken;
	}
}
