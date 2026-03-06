using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security.Private;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal class MonoTlsStream : IDisposable
	{
		internal HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		internal SslStream SslStream
		{
			get
			{
				return this.sslStream;
			}
		}

		internal WebExceptionStatus ExceptionStatus
		{
			get
			{
				return this.status;
			}
		}

		internal bool CertificateValidationFailed { get; set; }

		public MonoTlsStream(HttpWebRequest request, NetworkStream networkStream)
		{
			this.request = request;
			this.networkStream = networkStream;
			this.settings = request.TlsSettings;
			if (this.settings == null)
			{
				this.settings = MonoTlsSettings.CopyDefaultSettings();
			}
			if (this.settings.RemoteCertificateValidationCallback == null)
			{
				this.settings.RemoteCertificateValidationCallback = CallbackHelpers.PublicToMono(request.ServerCertificateValidationCallback);
			}
			this.provider = (request.TlsProvider ?? MonoTlsProviderFactory.GetProviderInternal());
			this.status = WebExceptionStatus.SecureChannelFailure;
			ChainValidationHelper.Create(this.provider, ref this.settings, this);
		}

		internal Task<Stream> CreateStream(WebConnectionTunnel tunnel, CancellationToken cancellationToken)
		{
			MonoTlsStream.<CreateStream>d__18 <CreateStream>d__;
			<CreateStream>d__.<>4__this = this;
			<CreateStream>d__.tunnel = tunnel;
			<CreateStream>d__.cancellationToken = cancellationToken;
			<CreateStream>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<CreateStream>d__.<>1__state = -1;
			<CreateStream>d__.<>t__builder.Start<MonoTlsStream.<CreateStream>d__18>(ref <CreateStream>d__);
			return <CreateStream>d__.<>t__builder.Task;
		}

		public void Dispose()
		{
			this.CloseSslStream();
		}

		private void CloseSslStream()
		{
			object obj = this.sslStreamLock;
			lock (obj)
			{
				if (this.sslStream != null)
				{
					this.sslStream.Dispose();
					this.sslStream = null;
				}
			}
		}

		private readonly MobileTlsProvider provider;

		private readonly NetworkStream networkStream;

		private readonly HttpWebRequest request;

		private readonly MonoTlsSettings settings;

		private SslStream sslStream;

		private readonly object sslStreamLock = new object();

		private WebExceptionStatus status;
	}
}
