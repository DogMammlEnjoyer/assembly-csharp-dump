using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebConnectionTunnel
	{
		public HttpWebRequest Request { get; }

		public Uri ConnectUri { get; }

		public WebConnectionTunnel(HttpWebRequest request, Uri connectUri)
		{
			this.Request = request;
			this.ConnectUri = connectUri;
		}

		public bool Success { get; private set; }

		public bool CloseConnection { get; private set; }

		public int StatusCode { get; private set; }

		public string StatusDescription { get; private set; }

		public string[] Challenge { get; private set; }

		public WebHeaderCollection Headers { get; private set; }

		public Version ProxyVersion { get; private set; }

		public byte[] Data { get; private set; }

		internal Task Initialize(Stream stream, CancellationToken cancellationToken)
		{
			WebConnectionTunnel.<Initialize>d__42 <Initialize>d__;
			<Initialize>d__.<>4__this = this;
			<Initialize>d__.stream = stream;
			<Initialize>d__.cancellationToken = cancellationToken;
			<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Initialize>d__.<>1__state = -1;
			<Initialize>d__.<>t__builder.Start<WebConnectionTunnel.<Initialize>d__42>(ref <Initialize>d__);
			return <Initialize>d__.<>t__builder.Task;
		}

		private Task<ValueTuple<WebHeaderCollection, byte[], int>> ReadHeaders(Stream stream, CancellationToken cancellationToken)
		{
			WebConnectionTunnel.<ReadHeaders>d__43 <ReadHeaders>d__;
			<ReadHeaders>d__.<>4__this = this;
			<ReadHeaders>d__.stream = stream;
			<ReadHeaders>d__.cancellationToken = cancellationToken;
			<ReadHeaders>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<WebHeaderCollection, byte[], int>>.Create();
			<ReadHeaders>d__.<>1__state = -1;
			<ReadHeaders>d__.<>t__builder.Start<WebConnectionTunnel.<ReadHeaders>d__43>(ref <ReadHeaders>d__);
			return <ReadHeaders>d__.<>t__builder.Task;
		}

		private void FlushContents(Stream stream, int contentLength)
		{
			while (contentLength > 0)
			{
				byte[] buffer = new byte[contentLength];
				int num = stream.Read(buffer, 0, contentLength);
				if (num <= 0)
				{
					break;
				}
				contentLength -= num;
			}
		}

		private HttpWebRequest connectRequest;

		private WebConnectionTunnel.NtlmAuthState ntlmAuthState;

		private enum NtlmAuthState
		{
			None,
			Challenge,
			Response
		}
	}
}
