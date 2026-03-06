using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>A specialty class that allows applications to call the <see cref="M:System.Net.Http.HttpMessageInvoker.SendAsync(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)" /> method on an HTTP handler chain.</summary>
	public class HttpMessageInvoker : IDisposable
	{
		/// <summary>Initializes an instance of a <see cref="T:System.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
		/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
		public HttpMessageInvoker(HttpMessageHandler handler) : this(handler, true)
		{
		}

		/// <summary>Initializes an instance of a <see cref="T:System.Net.Http.HttpMessageInvoker" /> class with a specific <see cref="T:System.Net.Http.HttpMessageHandler" />.</summary>
		/// <param name="handler">The <see cref="T:System.Net.Http.HttpMessageHandler" /> responsible for processing the HTTP response messages.</param>
		/// <param name="disposeHandler">
		///   <see langword="true" /> if the inner handler should be disposed of by Dispose(), <see langword="false" /> if you intend to reuse the inner handler.</param>
		public HttpMessageInvoker(HttpMessageHandler handler, bool disposeHandler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this.handler = handler;
			this.disposeHandler = disposeHandler;
		}

		/// <summary>Releases the unmanaged resources and disposes of the managed resources used by the <see cref="T:System.Net.Http.HttpMessageInvoker" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.HttpMessageInvoker" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.disposeHandler && this.handler != null)
			{
				this.handler.Dispose();
				this.handler = null;
			}
		}

		/// <summary>Send an HTTP request as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send.</param>
		/// <param name="cancellationToken">The cancellation token to cancel operation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
		public virtual Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this.handler.SendAsync(request, cancellationToken);
		}

		private protected HttpMessageHandler handler;

		private readonly bool disposeHandler;
	}
}
