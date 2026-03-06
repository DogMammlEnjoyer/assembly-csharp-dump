using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>A type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.</summary>
	public abstract class DelegatingHandler : HttpMessageHandler
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.DelegatingHandler" /> class.</summary>
		protected DelegatingHandler()
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.DelegatingHandler" /> class with a specific inner handler.</summary>
		/// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
		protected DelegatingHandler(HttpMessageHandler innerHandler)
		{
			if (innerHandler == null)
			{
				throw new ArgumentNullException("innerHandler");
			}
			this.InnerHandler = innerHandler;
		}

		/// <summary>Gets or sets the inner handler which processes the HTTP response messages.</summary>
		/// <returns>The inner handler for HTTP response messages.</returns>
		public HttpMessageHandler InnerHandler
		{
			get
			{
				return this.handler;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("InnerHandler");
				}
				this.handler = value;
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.DelegatingHandler" />, and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.InnerHandler != null)
				{
					this.InnerHandler.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send to the server.</param>
		/// <param name="cancellationToken">A cancellation token to cancel operation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (this.InnerHandler == null)
			{
				throw new InvalidOperationException("The inner handler has not been assigned.");
			}
			return this.InnerHandler.SendAsync(request, cancellationToken);
		}

		private bool disposed;

		private HttpMessageHandler handler;
	}
}
