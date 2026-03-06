using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>The default message handler used by <see cref="T:System.Net.Http.HttpClient" />.</summary>
	public class HttpClientHandler : HttpMessageHandler
	{
		private static IMonoHttpClientHandler CreateDefaultHandler()
		{
			return new MonoWebRequestHandler();
		}

		/// <summary>Gets a cached delegate that always returns <see langword="true" />.</summary>
		/// <returns>A cached delegate that always returns <see langword="true" />.</returns>
		public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		/// <summary>Creates an instance of a <see cref="T:System.Net.Http.HttpClientHandler" /> class.</summary>
		public HttpClientHandler() : this(HttpClientHandler.CreateDefaultHandler())
		{
		}

		internal HttpClientHandler(IMonoHttpClientHandler handler)
		{
			this._delegatingHandler = handler;
			this.ClientCertificateOptions = ClientCertificateOption.Manual;
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClientHandler" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._delegatingHandler.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>Gets a value that indicates whether the handler supports automatic response content decompression.</summary>
		/// <returns>
		///   <see langword="true" /> if the if the handler supports automatic response content decompression; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public virtual bool SupportsAutomaticDecompression
		{
			get
			{
				return this._delegatingHandler.SupportsAutomaticDecompression;
			}
		}

		/// <summary>Gets a value that indicates whether the handler supports proxy settings.</summary>
		/// <returns>
		///   <see langword="true" /> if the if the handler supports proxy settings; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public virtual bool SupportsProxy
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets a value that indicates whether the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties.</summary>
		/// <returns>
		///   <see langword="true" /> if the if the handler supports configuration settings for the <see cref="P:System.Net.Http.HttpClientHandler.AllowAutoRedirect" /> and <see cref="P:System.Net.Http.HttpClientHandler.MaxAutomaticRedirections" /> properties; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public virtual bool SupportsRedirectConfiguration
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the handler uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests.</summary>
		/// <returns>
		///   <see langword="true" /> if the if the handler supports uses the  <see cref="P:System.Net.Http.HttpClientHandler.CookieContainer" /> property  to store server cookies and uses these cookies when sending requests; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public bool UseCookies
		{
			get
			{
				return this._delegatingHandler.UseCookies;
			}
			set
			{
				this._delegatingHandler.UseCookies = value;
			}
		}

		/// <summary>Gets or sets the cookie container used to store server cookies by the handler.</summary>
		/// <returns>The cookie container used to store server cookies by the handler.</returns>
		public CookieContainer CookieContainer
		{
			get
			{
				return this._delegatingHandler.CookieContainer;
			}
			set
			{
				this._delegatingHandler.CookieContainer = value;
			}
		}

		private void ThrowForModifiedManagedSslOptionsIfStarted()
		{
			this._delegatingHandler.SslOptions = this._delegatingHandler.SslOptions;
		}

		/// <summary>Gets or sets a value that indicates if the certificate is automatically picked from the certificate store or if the caller is allowed to pass in a specific client certificate.</summary>
		/// <returns>The collection of security certificates associated with this handler.</returns>
		public ClientCertificateOption ClientCertificateOptions
		{
			get
			{
				return this._clientCertificateOptions;
			}
			set
			{
				if (value == ClientCertificateOption.Manual)
				{
					this.ThrowForModifiedManagedSslOptionsIfStarted();
					this._clientCertificateOptions = value;
					this._delegatingHandler.SslOptions.LocalCertificateSelectionCallback = ((object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate(this.ClientCertificates));
					return;
				}
				if (value != ClientCertificateOption.Automatic)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._clientCertificateOptions = value;
				this._delegatingHandler.SslOptions.LocalCertificateSelectionCallback = ((object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate());
			}
		}

		/// <summary>Gets the collection of security certificates that are associated requests to the server.</summary>
		/// <returns>The X509CertificateCollection that is presented to the server when performing certificate based client authentication.</returns>
		public X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this.ClientCertificateOptions != ClientCertificateOption.Manual)
				{
					throw new InvalidOperationException(SR.Format("The {0} property must be set to '{1}' to use this property.", "ClientCertificateOptions", "Manual"));
				}
				X509CertificateCollection result;
				if ((result = this._delegatingHandler.SslOptions.ClientCertificates) == null)
				{
					result = (this._delegatingHandler.SslOptions.ClientCertificates = new X509CertificateCollection());
				}
				return result;
			}
		}

		/// <summary>Gets or sets a callback method to validate the server certificate.</summary>
		/// <returns>A callback method to validate the server certificate.</returns>
		public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
		{
			get
			{
				RemoteCertificateValidationCallback remoteCertificateValidationCallback = this._delegatingHandler.SslOptions.RemoteCertificateValidationCallback;
				ConnectHelper.CertificateCallbackMapper certificateCallbackMapper = ((remoteCertificateValidationCallback != null) ? remoteCertificateValidationCallback.Target : null) as ConnectHelper.CertificateCallbackMapper;
				if (certificateCallbackMapper == null)
				{
					return null;
				}
				return certificateCallbackMapper.FromHttpClientHandler;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.RemoteCertificateValidationCallback = ((value != null) ? new ConnectHelper.CertificateCallbackMapper(value).ForSocketsHttpHandler : null);
			}
		}

		/// <summary>Gets or sets a value that indicates whether the certificate is checked against the certificate authority revocation list.</summary>
		/// <returns>
		///   <see langword="true" /> if the certificate revocation list is checked; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Framework 4.7.1 only: This property is not implemented.</exception>
		public bool CheckCertificateRevocationList
		{
			get
			{
				return this._delegatingHandler.SslOptions.CertificateRevocationCheckMode == X509RevocationMode.Online;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.CertificateRevocationCheckMode = (value ? X509RevocationMode.Online : X509RevocationMode.NoCheck);
			}
		}

		/// <summary>Gets or sets the TLS/SSL protocol used by the <see cref="T:System.Net.Http.HttpClient" /> objects managed by the HttpClientHandler object.</summary>
		/// <returns>One of the values defined in the <see cref="T:System.Security.Authentication.SslProtocols" /> enumeration.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">.NET Framework 4.7.1 only: This property is not implemented.</exception>
		public SslProtocols SslProtocols
		{
			get
			{
				return this._delegatingHandler.SslOptions.EnabledSslProtocols;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.EnabledSslProtocols = value;
			}
		}

		/// <summary>Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.</summary>
		/// <returns>The automatic decompression method used by the handler.</returns>
		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this._delegatingHandler.AutomaticDecompression;
			}
			set
			{
				this._delegatingHandler.AutomaticDecompression = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the handler uses a proxy for requests.</summary>
		/// <returns>
		///   <see langword="true" /> if the handler should use a proxy for requests; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public bool UseProxy
		{
			get
			{
				return this._delegatingHandler.UseProxy;
			}
			set
			{
				this._delegatingHandler.UseProxy = value;
			}
		}

		/// <summary>Gets or sets proxy information used by the handler.</summary>
		/// <returns>The proxy information used by the handler. The default value is <see langword="null" />.</returns>
		public IWebProxy Proxy
		{
			get
			{
				return this._delegatingHandler.Proxy;
			}
			set
			{
				this._delegatingHandler.Proxy = value;
			}
		}

		/// <summary>When the default (system) proxy is being used, gets or sets the credentials to submit to the default proxy server for authentication. The default proxy is used only when <see cref="P:System.Net.Http.HttpClientHandler.UseProxy" /> is set to <see langword="true" /> and <see cref="P:System.Net.Http.HttpClientHandler.Proxy" /> is set to <see langword="null" />.</summary>
		/// <returns>The credentials needed to authenticate a request to the default proxy server.</returns>
		public ICredentials DefaultProxyCredentials
		{
			get
			{
				return this._delegatingHandler.DefaultProxyCredentials;
			}
			set
			{
				this._delegatingHandler.DefaultProxyCredentials = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the handler sends an Authorization header with the request.</summary>
		/// <returns>
		///   <see langword="true" /> for the handler to send an HTTP Authorization header with requests after authentication has taken place; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool PreAuthenticate
		{
			get
			{
				return this._delegatingHandler.PreAuthenticate;
			}
			set
			{
				this._delegatingHandler.PreAuthenticate = value;
			}
		}

		/// <summary>Gets or sets a value that controls whether default credentials are sent with requests by the handler.</summary>
		/// <returns>
		///   <see langword="true" /> if the default credentials are used; otherwise <see langword="false" />. The default value is <see langword="false" />.</returns>
		public bool UseDefaultCredentials
		{
			get
			{
				return this._delegatingHandler.Credentials == CredentialCache.DefaultCredentials;
			}
			set
			{
				if (value)
				{
					this._delegatingHandler.Credentials = CredentialCache.DefaultCredentials;
					return;
				}
				if (this._delegatingHandler.Credentials == CredentialCache.DefaultCredentials)
				{
					this._delegatingHandler.Credentials = null;
				}
			}
		}

		/// <summary>Gets or sets authentication information used by this handler.</summary>
		/// <returns>The authentication credentials associated with the handler. The default is <see langword="null" />.</returns>
		public ICredentials Credentials
		{
			get
			{
				return this._delegatingHandler.Credentials;
			}
			set
			{
				this._delegatingHandler.Credentials = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the handler should follow redirection responses.</summary>
		/// <returns>
		///   <see langword="true" /> if the handler should follow redirection responses; otherwise <see langword="false" />. The default value is <see langword="true" />.</returns>
		public bool AllowAutoRedirect
		{
			get
			{
				return this._delegatingHandler.AllowAutoRedirect;
			}
			set
			{
				this._delegatingHandler.AllowAutoRedirect = value;
			}
		}

		/// <summary>Gets or sets the maximum number of redirects that the handler follows.</summary>
		/// <returns>The maximum number of redirection responses that the handler follows. The default value is 50.</returns>
		public int MaxAutomaticRedirections
		{
			get
			{
				return this._delegatingHandler.MaxAutomaticRedirections;
			}
			set
			{
				this._delegatingHandler.MaxAutomaticRedirections = value;
			}
		}

		/// <summary>Gets or sets the maximum number of concurrent connections (per server endpoint) allowed when making requests using an <see cref="T:System.Net.Http.HttpClient" /> object. Note that the limit is per server endpoint, so for example a value of 256 would permit 256 concurrent connections to http://www.adatum.com/ and another 256 to http://www.adventure-works.com/.</summary>
		/// <returns>The maximum number of concurrent connections (per server endpoint) allowed by an <see cref="T:System.Net.Http.HttpClient" /> object.</returns>
		public int MaxConnectionsPerServer
		{
			get
			{
				return this._delegatingHandler.MaxConnectionsPerServer;
			}
			set
			{
				this._delegatingHandler.MaxConnectionsPerServer = value;
			}
		}

		/// <summary>Gets or sets the maximum length, in kilobytes (1024 bytes), of the response headers. For example, if the value is 64, then 65536 bytes are allowed for the maximum response headers' length.</summary>
		/// <returns>The maximum length, in kilobytes (1024 bytes), of the response headers.</returns>
		public int MaxResponseHeadersLength
		{
			get
			{
				return this._delegatingHandler.MaxResponseHeadersLength;
			}
			set
			{
				this._delegatingHandler.MaxResponseHeadersLength = value;
			}
		}

		/// <summary>Gets or sets the maximum request content buffer size used by the handler.</summary>
		/// <returns>The maximum request content buffer size in bytes. The default value is 2 gigabytes.</returns>
		public long MaxRequestContentBufferSize
		{
			get
			{
				return this._delegatingHandler.MaxRequestContentBufferSize;
			}
			set
			{
				this._delegatingHandler.MaxRequestContentBufferSize = value;
			}
		}

		/// <summary>Gets a writable dictionary (that is, a map) of custom properties for the <see cref="T:System.Net.Http.HttpClient" /> requests. The dictionary is initialized empty; you can insert and query key-value pairs for your custom handlers and special processing.</summary>
		/// <returns>a writable dictionary of custom properties.</returns>
		public IDictionary<string, object> Properties
		{
			get
			{
				return this._delegatingHandler.Properties;
			}
		}

		internal void SetWebRequestTimeout(TimeSpan timeout)
		{
			this._delegatingHandler.SetWebRequestTimeout(timeout);
		}

		/// <summary>Creates an instance of  <see cref="T:System.Net.Http.HttpResponseMessage" /> based on the information provided in the <see cref="T:System.Net.Http.HttpRequestMessage" /> as an operation that will not block.</summary>
		/// <param name="request">The HTTP request message.</param>
		/// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this._delegatingHandler.SendAsync(request, cancellationToken);
		}

		private readonly IMonoHttpClientHandler _delegatingHandler;

		private ClientCertificateOption _clientCertificateOptions;
	}
}
