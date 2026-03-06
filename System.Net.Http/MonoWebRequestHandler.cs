using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace System.Net.Http
{
	internal class MonoWebRequestHandler : IMonoHttpClientHandler, IDisposable
	{
		public MonoWebRequestHandler()
		{
			this.allowAutoRedirect = true;
			this.maxAutomaticRedirections = 50;
			this.maxRequestContentBufferSize = 2147483647L;
			this.useCookies = true;
			this.useProxy = true;
			this.allowPipelining = true;
			this.authenticationLevel = AuthenticationLevel.MutualAuthRequested;
			this.cachePolicy = WebRequest.DefaultCachePolicy;
			this.continueTimeout = TimeSpan.FromMilliseconds(350.0);
			this.impersonationLevel = TokenImpersonationLevel.Delegation;
			this.maxResponseHeadersLength = HttpWebRequest.DefaultMaximumResponseHeadersLength;
			this.readWriteTimeout = 300000;
			this.serverCertificateValidationCallback = null;
			this.unsafeAuthenticatedConnectionSharing = false;
			this.connectionGroupName = "HttpClientHandler" + Interlocked.Increment(ref MonoWebRequestHandler.groupCounter).ToString();
		}

		internal void EnsureModifiability()
		{
			if (this.sentRequest)
			{
				throw new InvalidOperationException("This instance has already started one or more requests. Properties can only be modified before sending the first request.");
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this.allowAutoRedirect;
			}
			set
			{
				this.EnsureModifiability();
				this.allowAutoRedirect = value;
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this.automaticDecompression;
			}
			set
			{
				this.EnsureModifiability();
				this.automaticDecompression = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				CookieContainer result;
				if ((result = this.cookieContainer) == null)
				{
					result = (this.cookieContainer = new CookieContainer());
				}
				return result;
			}
			set
			{
				this.EnsureModifiability();
				this.cookieContainer = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.EnsureModifiability();
				this.credentials = value;
			}
		}

		public int MaxAutomaticRedirections
		{
			get
			{
				return this.maxAutomaticRedirections;
			}
			set
			{
				this.EnsureModifiability();
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.maxAutomaticRedirections = value;
			}
		}

		public long MaxRequestContentBufferSize
		{
			get
			{
				return this.maxRequestContentBufferSize;
			}
			set
			{
				this.EnsureModifiability();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.maxRequestContentBufferSize = value;
			}
		}

		public bool PreAuthenticate
		{
			get
			{
				return this.preAuthenticate;
			}
			set
			{
				this.EnsureModifiability();
				this.preAuthenticate = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.EnsureModifiability();
				if (!this.UseProxy)
				{
					throw new InvalidOperationException();
				}
				this.proxy = value;
			}
		}

		public virtual bool SupportsAutomaticDecompression
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportsProxy
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportsRedirectConfiguration
		{
			get
			{
				return true;
			}
		}

		public bool UseCookies
		{
			get
			{
				return this.useCookies;
			}
			set
			{
				this.EnsureModifiability();
				this.useCookies = value;
			}
		}

		public bool UseProxy
		{
			get
			{
				return this.useProxy;
			}
			set
			{
				this.EnsureModifiability();
				this.useProxy = value;
			}
		}

		public bool AllowPipelining
		{
			get
			{
				return this.allowPipelining;
			}
			set
			{
				this.EnsureModifiability();
				this.allowPipelining = value;
			}
		}

		public RequestCachePolicy CachePolicy
		{
			get
			{
				return this.cachePolicy;
			}
			set
			{
				this.EnsureModifiability();
				this.cachePolicy = value;
			}
		}

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.authenticationLevel;
			}
			set
			{
				this.EnsureModifiability();
				this.authenticationLevel = value;
			}
		}

		[MonoTODO]
		public TimeSpan ContinueTimeout
		{
			get
			{
				return this.continueTimeout;
			}
			set
			{
				this.EnsureModifiability();
				this.continueTimeout = value;
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.impersonationLevel;
			}
			set
			{
				this.EnsureModifiability();
				this.impersonationLevel = value;
			}
		}

		public int MaxResponseHeadersLength
		{
			get
			{
				return this.maxResponseHeadersLength;
			}
			set
			{
				this.EnsureModifiability();
				this.maxResponseHeadersLength = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return this.readWriteTimeout;
			}
			set
			{
				this.EnsureModifiability();
				this.readWriteTimeout = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return this.serverCertificateValidationCallback;
			}
			set
			{
				this.EnsureModifiability();
				this.serverCertificateValidationCallback = value;
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafeAuthenticatedConnectionSharing;
			}
			set
			{
				this.EnsureModifiability();
				this.unsafeAuthenticatedConnectionSharing = value;
			}
		}

		public SslClientAuthenticationOptions SslOptions
		{
			get
			{
				SslClientAuthenticationOptions result;
				if ((result = this.sslOptions) == null)
				{
					result = (this.sslOptions = new SslClientAuthenticationOptions());
				}
				return result;
			}
			set
			{
				this.EnsureModifiability();
				this.sslOptions = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				Volatile.Write(ref this.disposed, true);
				ServicePointManager.CloseConnectionGroup(this.connectionGroupName);
			}
		}

		private bool GetConnectionKeepAlive(HttpRequestHeaders headers)
		{
			return headers.Connection.Any((string l) => string.Equals(l, "Keep-Alive", StringComparison.OrdinalIgnoreCase));
		}

		internal virtual HttpWebRequest CreateWebRequest(HttpRequestMessage request)
		{
			HttpWebRequest httpWebRequest;
			if (HttpUtilities.IsSupportedSecureScheme(request.RequestUri.Scheme))
			{
				httpWebRequest = new HttpWebRequest(request.RequestUri, MonoTlsProviderFactory.GetProviderInternal(), MonoTlsSettings.CopyDefaultSettings());
				httpWebRequest.TlsSettings.ClientCertificateSelectionCallback = ((string t, X509CertificateCollection lc, X509Certificate rc, string[] ai) => this.SslOptions.LocalCertificateSelectionCallback(this, t, lc, rc, ai));
			}
			else
			{
				httpWebRequest = new HttpWebRequest(request.RequestUri);
			}
			httpWebRequest.ThrowOnError = false;
			httpWebRequest.AllowWriteStreamBuffering = false;
			if (request.Version == HttpVersion.Version20)
			{
				httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			}
			else
			{
				httpWebRequest.ProtocolVersion = request.Version;
			}
			httpWebRequest.ConnectionGroupName = this.connectionGroupName;
			httpWebRequest.Method = request.Method.Method;
			bool? flag;
			bool flag2;
			if (httpWebRequest.ProtocolVersion == HttpVersion.Version10)
			{
				httpWebRequest.KeepAlive = this.GetConnectionKeepAlive(request.Headers);
			}
			else
			{
				HttpWebRequest httpWebRequest2 = httpWebRequest;
				flag = request.Headers.ConnectionClose;
				flag2 = true;
				httpWebRequest2.KeepAlive = !(flag.GetValueOrDefault() == flag2 & flag != null);
			}
			if (this.allowAutoRedirect)
			{
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.MaximumAutomaticRedirections = this.maxAutomaticRedirections;
			}
			else
			{
				httpWebRequest.AllowAutoRedirect = false;
			}
			httpWebRequest.AutomaticDecompression = this.automaticDecompression;
			httpWebRequest.PreAuthenticate = this.preAuthenticate;
			if (this.useCookies)
			{
				httpWebRequest.CookieContainer = this.CookieContainer;
			}
			httpWebRequest.Credentials = this.credentials;
			if (this.useProxy)
			{
				httpWebRequest.Proxy = this.proxy;
			}
			else
			{
				httpWebRequest.Proxy = null;
			}
			ServicePoint servicePoint = httpWebRequest.ServicePoint;
			flag = request.Headers.ExpectContinue;
			flag2 = true;
			servicePoint.Expect100Continue = (flag.GetValueOrDefault() == flag2 & flag != null);
			if (this.timeout != null)
			{
				httpWebRequest.Timeout = (int)this.timeout.Value.TotalMilliseconds;
			}
			httpWebRequest.ServerCertificateValidationCallback = this.SslOptions.RemoteCertificateValidationCallback;
			WebHeaderCollection headers = httpWebRequest.Headers;
			foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in request.Headers)
			{
				IEnumerable<string> enumerable = keyValuePair.Value;
				if (keyValuePair.Key == "Host")
				{
					httpWebRequest.Host = request.Headers.Host;
				}
				else
				{
					if (keyValuePair.Key == "Transfer-Encoding")
					{
						enumerable = from l in enumerable
						where l != "chunked"
						select l;
					}
					string singleHeaderString = PlatformHelper.GetSingleHeaderString(keyValuePair.Key, enumerable);
					if (singleHeaderString != null)
					{
						headers.AddInternal(keyValuePair.Key, singleHeaderString);
					}
				}
			}
			return httpWebRequest;
		}

		private HttpResponseMessage CreateResponseMessage(HttpWebResponse wr, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
		{
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(wr.StatusCode);
			httpResponseMessage.RequestMessage = requestMessage;
			httpResponseMessage.ReasonPhrase = wr.StatusDescription;
			httpResponseMessage.Content = PlatformHelper.CreateStreamContent(wr.GetResponseStream(), cancellationToken);
			WebHeaderCollection headers = wr.Headers;
			for (int i = 0; i < headers.Count; i++)
			{
				string key = headers.GetKey(i);
				string[] values = headers.GetValues(i);
				HttpHeaders headers2;
				if (PlatformHelper.IsContentHeader(key))
				{
					headers2 = httpResponseMessage.Content.Headers;
				}
				else
				{
					headers2 = httpResponseMessage.Headers;
				}
				headers2.TryAddWithoutValidation(key, values);
			}
			requestMessage.RequestUri = wr.ResponseUri;
			return httpResponseMessage;
		}

		private static bool MethodHasBody(HttpMethod method)
		{
			string method2 = method.Method;
			return !(method2 == "HEAD") && !(method2 == "GET") && !(method2 == "MKCOL") && !(method2 == "CONNECT") && !(method2 == "TRACE");
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			MonoWebRequestHandler.<SendAsync>d__99 <SendAsync>d__;
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.request = request;
			<SendAsync>d__.cancellationToken = cancellationToken;
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HttpResponseMessage>.Create();
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<MonoWebRequestHandler.<SendAsync>d__99>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.Task;
		}

		public ICredentials DefaultProxyCredentials
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int MaxConnectionsPerServer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IMonoHttpClientHandler.SetWebRequestTimeout(TimeSpan timeout)
		{
			this.timeout = new TimeSpan?(timeout);
		}

		private static long groupCounter;

		private bool allowAutoRedirect;

		private DecompressionMethods automaticDecompression;

		private CookieContainer cookieContainer;

		private ICredentials credentials;

		private int maxAutomaticRedirections;

		private long maxRequestContentBufferSize;

		private bool preAuthenticate;

		private IWebProxy proxy;

		private bool useCookies;

		private bool useProxy;

		private SslClientAuthenticationOptions sslOptions;

		private bool allowPipelining;

		private RequestCachePolicy cachePolicy;

		private AuthenticationLevel authenticationLevel;

		private TimeSpan continueTimeout;

		private TokenImpersonationLevel impersonationLevel;

		private int maxResponseHeadersLength;

		private int readWriteTimeout;

		private RemoteCertificateValidationCallback serverCertificateValidationCallback;

		private bool unsafeAuthenticatedConnectionSharing;

		private bool sentRequest;

		private string connectionGroupName;

		private TimeSpan? timeout;

		private bool disposed;
	}
}
