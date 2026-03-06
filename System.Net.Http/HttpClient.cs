using System;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.</summary>
	public class HttpClient : HttpMessageInvoker
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class.</summary>
		public HttpClient() : this(new HttpClientHandler(), true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class with a specific handler.</summary>
		/// <param name="handler">The HTTP handler stack to use for sending requests.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="handler" /> is <see langword="null" />.</exception>
		public HttpClient(HttpMessageHandler handler) : this(handler, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.HttpClient" /> class with a specific handler.</summary>
		/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
		/// <param name="disposeHandler">
		///   <see langword="true" /> if the inner handler should be disposed of by HttpClient.Dispose, <see langword="false" /> if you intend to reuse the inner handler.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="handler" /> is <see langword="null" />.</exception>
		public HttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
		{
			this.buffer_size = 2147483647L;
			this.timeout = HttpClient.TimeoutDefault;
			this.cts = new CancellationTokenSource();
		}

		/// <summary>Gets or sets the base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</summary>
		/// <returns>The base address of Uniform Resource Identifier (URI) of the Internet resource used when sending requests.</returns>
		public Uri BaseAddress
		{
			get
			{
				return this.base_address;
			}
			set
			{
				this.base_address = value;
			}
		}

		/// <summary>Gets the headers which should be sent with each request.</summary>
		/// <returns>The headers which should be sent with each request.</returns>
		public HttpRequestHeaders DefaultRequestHeaders
		{
			get
			{
				HttpRequestHeaders result;
				if ((result = this.headers) == null)
				{
					result = (this.headers = new HttpRequestHeaders());
				}
				return result;
			}
		}

		/// <summary>Gets or sets the maximum number of bytes to buffer when reading the response content.</summary>
		/// <returns>The maximum number of bytes to buffer when reading the response content. The default value for this property is 2 gigabytes.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The size specified is less than or equal to zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public long MaxResponseContentBufferSize
		{
			get
			{
				return this.buffer_size;
			}
			set
			{
				if (value <= 0L)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.buffer_size = value;
			}
		}

		/// <summary>Gets or sets the timespan to wait before the request times out.</summary>
		/// <returns>The timespan to wait before the request times out.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The timeout specified is less than or equal to zero and is not <see cref="F:System.Threading.Timeout.InfiniteTimeSpan" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An operation has already been started on the current instance.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The current instance has been disposed.</exception>
		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value != System.Threading.Timeout.InfiniteTimeSpan && (value <= TimeSpan.Zero || value.TotalMilliseconds > 2147483647.0))
				{
					throw new ArgumentOutOfRangeException();
				}
				this.timeout = value;
			}
		}

		/// <summary>Cancel all pending requests on this instance.</summary>
		public void CancelPendingRequests()
		{
			using (CancellationTokenSource cancellationTokenSource = Interlocked.Exchange<CancellationTokenSource>(ref this.cts, new CancellationTokenSource()))
			{
				cancellationTokenSource.Cancel();
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpClient" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				this.cts.Cancel();
				this.cts.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> DeleteAsync(string requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		/// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		/// <summary>Send a DELETE request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri));
		}

		/// <summary>Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri), cancellationToken);
		}

		/// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(string requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		/// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
		}

		/// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		/// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		/// <summary>Send a GET request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(Uri requestUri)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri));
		}

		/// <summary>Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), cancellationToken);
		}

		/// <summary>Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption);
		}

		/// <summary>Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri), completionOption, cancellationToken);
		}

		/// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			});
		}

		/// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		/// <summary>Send a POST request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			});
		}

		/// <summary>Send a POST request with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		/// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			});
		}

		/// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		/// <summary>Send a PUT request to the specified Uri as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			});
		}

		/// <summary>Send a PUT request with a cancellation token as an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <param name="content">The HTTP request content sent to the server.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			return this.SendAsync(new HttpRequestMessage(HttpMethod.Put, requestUri)
			{
				Content = content
			}, cancellationToken);
		}

		/// <summary>Send an HTTP request as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
		}

		/// <summary>Send an HTTP request as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send.</param>
		/// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
		{
			return this.SendAsync(request, completionOption, CancellationToken.None);
		}

		/// <summary>Send an HTTP request as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send.</param>
		/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
		}

		/// <summary>Send an HTTP request as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send.</param>
		/// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
		/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (request.SetIsUsed())
			{
				throw new InvalidOperationException("Cannot send the same request message multiple times");
			}
			Uri requestUri = request.RequestUri;
			if (requestUri == null)
			{
				if (this.base_address == null)
				{
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}
				request.RequestUri = this.base_address;
			}
			else if (!requestUri.IsAbsoluteUri || (requestUri.Scheme == Uri.UriSchemeFile && requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal)))
			{
				if (this.base_address == null)
				{
					throw new InvalidOperationException("The request URI must either be an absolute URI or BaseAddress must be set");
				}
				request.RequestUri = new Uri(this.base_address, requestUri);
			}
			if (this.headers != null)
			{
				request.Headers.AddHeaders(this.headers);
			}
			return this.SendAsyncWorker(request, completionOption, cancellationToken);
		}

		private Task<HttpResponseMessage> SendAsyncWorker(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
		{
			HttpClient.<SendAsyncWorker>d__47 <SendAsyncWorker>d__;
			<SendAsyncWorker>d__.<>4__this = this;
			<SendAsyncWorker>d__.request = request;
			<SendAsyncWorker>d__.completionOption = completionOption;
			<SendAsyncWorker>d__.cancellationToken = cancellationToken;
			<SendAsyncWorker>d__.<>t__builder = AsyncTaskMethodBuilder<HttpResponseMessage>.Create();
			<SendAsyncWorker>d__.<>1__state = -1;
			<SendAsyncWorker>d__.<>t__builder.Start<HttpClient.<SendAsyncWorker>d__47>(ref <SendAsyncWorker>d__);
			return <SendAsyncWorker>d__.<>t__builder.Task;
		}

		/// <summary>Sends a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<byte[]> GetByteArrayAsync(string requestUri)
		{
			HttpClient.<GetByteArrayAsync>d__48 <GetByteArrayAsync>d__;
			<GetByteArrayAsync>d__.<>4__this = this;
			<GetByteArrayAsync>d__.requestUri = requestUri;
			<GetByteArrayAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<GetByteArrayAsync>d__.<>1__state = -1;
			<GetByteArrayAsync>d__.<>t__builder.Start<HttpClient.<GetByteArrayAsync>d__48>(ref <GetByteArrayAsync>d__);
			return <GetByteArrayAsync>d__.<>t__builder.Task;
		}

		/// <summary>Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<byte[]> GetByteArrayAsync(Uri requestUri)
		{
			HttpClient.<GetByteArrayAsync>d__49 <GetByteArrayAsync>d__;
			<GetByteArrayAsync>d__.<>4__this = this;
			<GetByteArrayAsync>d__.requestUri = requestUri;
			<GetByteArrayAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<GetByteArrayAsync>d__.<>1__state = -1;
			<GetByteArrayAsync>d__.<>t__builder.Start<HttpClient.<GetByteArrayAsync>d__49>(ref <GetByteArrayAsync>d__);
			return <GetByteArrayAsync>d__.<>t__builder.Task;
		}

		/// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<Stream> GetStreamAsync(string requestUri)
		{
			HttpClient.<GetStreamAsync>d__50 <GetStreamAsync>d__;
			<GetStreamAsync>d__.<>4__this = this;
			<GetStreamAsync>d__.requestUri = requestUri;
			<GetStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<GetStreamAsync>d__.<>1__state = -1;
			<GetStreamAsync>d__.<>t__builder.Start<HttpClient.<GetStreamAsync>d__50>(ref <GetStreamAsync>d__);
			return <GetStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<Stream> GetStreamAsync(Uri requestUri)
		{
			HttpClient.<GetStreamAsync>d__51 <GetStreamAsync>d__;
			<GetStreamAsync>d__.<>4__this = this;
			<GetStreamAsync>d__.requestUri = requestUri;
			<GetStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<GetStreamAsync>d__.<>1__state = -1;
			<GetStreamAsync>d__.<>t__builder.Start<HttpClient.<GetStreamAsync>d__51>(ref <GetStreamAsync>d__);
			return <GetStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<string> GetStringAsync(string requestUri)
		{
			HttpClient.<GetStringAsync>d__52 <GetStringAsync>d__;
			<GetStringAsync>d__.<>4__this = this;
			<GetStringAsync>d__.requestUri = requestUri;
			<GetStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetStringAsync>d__.<>1__state = -1;
			<GetStringAsync>d__.<>t__builder.Start<HttpClient.<GetStringAsync>d__52>(ref <GetStringAsync>d__);
			return <GetStringAsync>d__.<>t__builder.Task;
		}

		/// <summary>Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.</summary>
		/// <param name="requestUri">The Uri the request is sent to.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="requestUri" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Http.HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
		public Task<string> GetStringAsync(Uri requestUri)
		{
			HttpClient.<GetStringAsync>d__53 <GetStringAsync>d__;
			<GetStringAsync>d__.<>4__this = this;
			<GetStringAsync>d__.requestUri = requestUri;
			<GetStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetStringAsync>d__.<>1__state = -1;
			<GetStringAsync>d__.<>t__builder.Start<HttpClient.<GetStringAsync>d__53>(ref <GetStringAsync>d__);
			return <GetStringAsync>d__.<>t__builder.Task;
		}

		public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content)
		{
			throw new PlatformNotSupportedException();
		}

		public Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
		{
			throw new PlatformNotSupportedException();
		}

		private static readonly TimeSpan TimeoutDefault = TimeSpan.FromSeconds(100.0);

		private Uri base_address;

		private CancellationTokenSource cts;

		private bool disposed;

		private HttpRequestHeaders headers;

		private long buffer_size;

		private TimeSpan timeout;
	}
}
