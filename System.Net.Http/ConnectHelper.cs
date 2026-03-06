using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	internal static class ConnectHelper
	{
		public static ValueTask<ValueTuple<Socket, Stream>> ConnectAsync(string host, int port, CancellationToken cancellationToken)
		{
			ConnectHelper.<ConnectAsync>d__2 <ConnectAsync>d__;
			<ConnectAsync>d__.host = host;
			<ConnectAsync>d__.port = port;
			<ConnectAsync>d__.cancellationToken = cancellationToken;
			<ConnectAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<ValueTuple<Socket, Stream>>.Create();
			<ConnectAsync>d__.<>1__state = -1;
			<ConnectAsync>d__.<>t__builder.Start<ConnectHelper.<ConnectAsync>d__2>(ref <ConnectAsync>d__);
			return <ConnectAsync>d__.<>t__builder.Task;
		}

		public static ValueTask<SslStream> EstablishSslConnectionAsync(SslClientAuthenticationOptions sslOptions, HttpRequestMessage request, Stream stream, CancellationToken cancellationToken)
		{
			RemoteCertificateValidationCallback remoteCertificateValidationCallback = sslOptions.RemoteCertificateValidationCallback;
			if (remoteCertificateValidationCallback != null)
			{
				ConnectHelper.CertificateCallbackMapper certificateCallbackMapper = remoteCertificateValidationCallback.Target as ConnectHelper.CertificateCallbackMapper;
				if (certificateCallbackMapper != null)
				{
					sslOptions = sslOptions.ShallowClone();
					Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> localFromHttpClientHandler = certificateCallbackMapper.FromHttpClientHandler;
					HttpRequestMessage localRequest = request;
					sslOptions.RemoteCertificateValidationCallback = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => localFromHttpClientHandler(localRequest, certificate as X509Certificate2, chain, sslPolicyErrors));
				}
			}
			return ConnectHelper.EstablishSslConnectionAsyncCore(stream, sslOptions, cancellationToken);
		}

		private static ValueTask<SslStream> EstablishSslConnectionAsyncCore(Stream stream, SslClientAuthenticationOptions sslOptions, CancellationToken cancellationToken)
		{
			ConnectHelper.<EstablishSslConnectionAsyncCore>d__5 <EstablishSslConnectionAsyncCore>d__;
			<EstablishSslConnectionAsyncCore>d__.stream = stream;
			<EstablishSslConnectionAsyncCore>d__.sslOptions = sslOptions;
			<EstablishSslConnectionAsyncCore>d__.cancellationToken = cancellationToken;
			<EstablishSslConnectionAsyncCore>d__.<>t__builder = AsyncValueTaskMethodBuilder<SslStream>.Create();
			<EstablishSslConnectionAsyncCore>d__.<>1__state = -1;
			<EstablishSslConnectionAsyncCore>d__.<>t__builder.Start<ConnectHelper.<EstablishSslConnectionAsyncCore>d__5>(ref <EstablishSslConnectionAsyncCore>d__);
			return <EstablishSslConnectionAsyncCore>d__.<>t__builder.Task;
		}

		private static readonly ConcurrentQueue<ConnectHelper.ConnectEventArgs>.Segment s_connectEventArgs = new ConcurrentQueue<ConnectHelper.ConnectEventArgs>.Segment(ConcurrentQueue<ConnectHelper.ConnectEventArgs>.Segment.RoundUpToPowerOf2(Math.Max(2, Environment.ProcessorCount)));

		internal sealed class CertificateCallbackMapper
		{
			public CertificateCallbackMapper(Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> fromHttpClientHandler)
			{
				this.FromHttpClientHandler = fromHttpClientHandler;
				this.ForSocketsHttpHandler = ((object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => this.FromHttpClientHandler(new HttpRequestMessage(HttpMethod.Get, (string)sender), certificate as X509Certificate2, chain, sslPolicyErrors));
			}

			public readonly Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> FromHttpClientHandler;

			public readonly RemoteCertificateValidationCallback ForSocketsHttpHandler;
		}

		private sealed class ConnectEventArgs : SocketAsyncEventArgs
		{
			public AsyncTaskMethodBuilder Builder { get; private set; }

			public CancellationToken CancellationToken { get; private set; }

			public void Initialize(CancellationToken cancellationToken)
			{
				this.CancellationToken = cancellationToken;
				AsyncTaskMethodBuilder builder = default(AsyncTaskMethodBuilder);
				Task task = builder.Task;
				this.Builder = builder;
			}

			public void Clear()
			{
				this.CancellationToken = default(CancellationToken);
			}

			protected override void OnCompleted(SocketAsyncEventArgs _)
			{
				SocketError socketError = base.SocketError;
				if (socketError != SocketError.Success)
				{
					if (socketError == SocketError.OperationAborted || socketError == SocketError.ConnectionAborted)
					{
						if (this.CancellationToken.IsCancellationRequested)
						{
							this.Builder.SetException(CancellationHelper.CreateOperationCanceledException(null, this.CancellationToken));
							return;
						}
					}
					this.Builder.SetException(new SocketException((int)base.SocketError));
					return;
				}
				this.Builder.SetResult();
			}
		}
	}
}
