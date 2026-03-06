using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>A base type for handlers which only do some small processing of request and/or response messages.</summary>
	public abstract class MessageProcessingHandler : DelegatingHandler
	{
		/// <summary>Creates an instance of a <see cref="T:System.Net.Http.MessageProcessingHandler" /> class.</summary>
		protected MessageProcessingHandler()
		{
		}

		/// <summary>Creates an instance of a <see cref="T:System.Net.Http.MessageProcessingHandler" /> class with a specific inner handler.</summary>
		/// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
		protected MessageProcessingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
		{
		}

		/// <summary>Performs processing on each request sent to the server.</summary>
		/// <param name="request">The HTTP request message to process.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The HTTP request message that was processed.</returns>
		protected abstract HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken);

		/// <summary>Perform processing on each response from the server.</summary>
		/// <param name="response">The HTTP response message to process.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The HTTP response message that was processed.</returns>
		protected abstract HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken);

		/// <summary>Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.</summary>
		/// <param name="request">The HTTP request message to send to the server.</param>
		/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="request" /> was <see langword="null" />.</exception>
		protected internal sealed override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			MessageProcessingHandler.<SendAsync>d__4 <SendAsync>d__;
			<SendAsync>d__.<>4__this = this;
			<SendAsync>d__.request = request;
			<SendAsync>d__.cancellationToken = cancellationToken;
			<SendAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HttpResponseMessage>.Create();
			<SendAsync>d__.<>1__state = -1;
			<SendAsync>d__.<>t__builder.Start<MessageProcessingHandler.<SendAsync>d__4>(ref <SendAsync>d__);
			return <SendAsync>d__.<>t__builder.Task;
		}
	}
}
