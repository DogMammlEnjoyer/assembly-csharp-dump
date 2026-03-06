using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	internal sealed class WebSocketHandle
	{
		public static WebSocketHandle Create()
		{
			return new WebSocketHandle();
		}

		public static bool IsValid(WebSocketHandle handle)
		{
			return handle != null;
		}

		public WebSocketCloseStatus? CloseStatus
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.CloseStatus;
			}
		}

		public string CloseStatusDescription
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.CloseStatusDescription;
			}
		}

		public WebSocketState State
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return this._state;
				}
				return webSocket.State;
			}
		}

		public string SubProtocol
		{
			get
			{
				WebSocket webSocket = this._webSocket;
				if (webSocket == null)
				{
					return null;
				}
				return webSocket.SubProtocol;
			}
		}

		public static void CheckPlatformSupport()
		{
		}

		public void Dispose()
		{
			this._state = WebSocketState.Closed;
			WebSocket webSocket = this._webSocket;
			if (webSocket == null)
			{
				return;
			}
			webSocket.Dispose();
		}

		public void Abort()
		{
			this._abortSource.Cancel();
			WebSocket webSocket = this._webSocket;
			if (webSocket == null)
			{
				return;
			}
			webSocket.Abort();
		}

		public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			return this._webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		}

		public ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			return this._webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
		}

		public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			return this._webSocket.ReceiveAsync(buffer, cancellationToken);
		}

		public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			return this._webSocket.ReceiveAsync(buffer, cancellationToken);
		}

		public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			return this._webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
		}

		public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			return this._webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);
		}

		public Task ConnectAsyncCore(Uri uri, CancellationToken cancellationToken, ClientWebSocketOptions options)
		{
			WebSocketHandle.<ConnectAsyncCore>d__26 <ConnectAsyncCore>d__;
			<ConnectAsyncCore>d__.<>4__this = this;
			<ConnectAsyncCore>d__.uri = uri;
			<ConnectAsyncCore>d__.cancellationToken = cancellationToken;
			<ConnectAsyncCore>d__.options = options;
			<ConnectAsyncCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ConnectAsyncCore>d__.<>1__state = -1;
			<ConnectAsyncCore>d__.<>t__builder.Start<WebSocketHandle.<ConnectAsyncCore>d__26>(ref <ConnectAsyncCore>d__);
			return <ConnectAsyncCore>d__.<>t__builder.Task;
		}

		private Task<Socket> ConnectSocketAsync(string host, int port, CancellationToken cancellationToken)
		{
			WebSocketHandle.<ConnectSocketAsync>d__27 <ConnectSocketAsync>d__;
			<ConnectSocketAsync>d__.<>4__this = this;
			<ConnectSocketAsync>d__.host = host;
			<ConnectSocketAsync>d__.port = port;
			<ConnectSocketAsync>d__.cancellationToken = cancellationToken;
			<ConnectSocketAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Socket>.Create();
			<ConnectSocketAsync>d__.<>1__state = -1;
			<ConnectSocketAsync>d__.<>t__builder.Start<WebSocketHandle.<ConnectSocketAsync>d__27>(ref <ConnectSocketAsync>d__);
			return <ConnectSocketAsync>d__.<>t__builder.Task;
		}

		private static byte[] BuildRequestHeader(Uri uri, ClientWebSocketOptions options, string secKey)
		{
			StringBuilder stringBuilder;
			if ((stringBuilder = WebSocketHandle.t_cachedStringBuilder) == null)
			{
				stringBuilder = (WebSocketHandle.t_cachedStringBuilder = new StringBuilder());
			}
			StringBuilder stringBuilder2 = stringBuilder;
			byte[] bytes;
			try
			{
				stringBuilder2.Append("GET ").Append(uri.PathAndQuery).Append(" HTTP/1.1\r\n");
				string value = options.RequestHeaders["Host"];
				stringBuilder2.Append("Host: ");
				if (string.IsNullOrEmpty(value))
				{
					stringBuilder2.Append(uri.IdnHost).Append(':').Append(uri.Port).Append("\r\n");
				}
				else
				{
					stringBuilder2.Append(value).Append("\r\n");
				}
				stringBuilder2.Append("Connection: Upgrade\r\n");
				stringBuilder2.Append("Upgrade: websocket\r\n");
				stringBuilder2.Append("Sec-WebSocket-Version: 13\r\n");
				stringBuilder2.Append("Sec-WebSocket-Key: ").Append(secKey).Append("\r\n");
				foreach (string text in options.RequestHeaders.AllKeys)
				{
					if (!string.Equals(text, "Host", StringComparison.OrdinalIgnoreCase))
					{
						stringBuilder2.Append(text).Append(": ").Append(options.RequestHeaders[text]).Append("\r\n");
					}
				}
				if (options.RequestedSubProtocols.Count > 0)
				{
					stringBuilder2.Append("Sec-WebSocket-Protocol").Append(": ");
					stringBuilder2.Append(options.RequestedSubProtocols[0]);
					for (int j = 1; j < options.RequestedSubProtocols.Count; j++)
					{
						stringBuilder2.Append(", ").Append(options.RequestedSubProtocols[j]);
					}
					stringBuilder2.Append("\r\n");
				}
				if (options.Cookies != null)
				{
					string cookieHeader = options.Cookies.GetCookieHeader(uri);
					if (!string.IsNullOrWhiteSpace(cookieHeader))
					{
						stringBuilder2.Append("Cookie").Append(": ").Append(cookieHeader).Append("\r\n");
					}
				}
				stringBuilder2.Append("\r\n");
				bytes = WebSocketHandle.s_defaultHttpEncoding.GetBytes(stringBuilder2.ToString());
			}
			finally
			{
				stringBuilder2.Clear();
			}
			return bytes;
		}

		private static KeyValuePair<string, string> CreateSecKeyAndSecWebSocketAccept()
		{
			string text = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
			KeyValuePair<string, string> result;
			using (SHA1 sha = SHA1.Create())
			{
				result = new KeyValuePair<string, string>(text, Convert.ToBase64String(sha.ComputeHash(Encoding.ASCII.GetBytes(text + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))));
			}
			return result;
		}

		private Task<string> ParseAndValidateConnectResponseAsync(Stream stream, ClientWebSocketOptions options, string expectedSecWebSocketAccept, CancellationToken cancellationToken)
		{
			WebSocketHandle.<ParseAndValidateConnectResponseAsync>d__30 <ParseAndValidateConnectResponseAsync>d__;
			<ParseAndValidateConnectResponseAsync>d__.stream = stream;
			<ParseAndValidateConnectResponseAsync>d__.options = options;
			<ParseAndValidateConnectResponseAsync>d__.expectedSecWebSocketAccept = expectedSecWebSocketAccept;
			<ParseAndValidateConnectResponseAsync>d__.cancellationToken = cancellationToken;
			<ParseAndValidateConnectResponseAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ParseAndValidateConnectResponseAsync>d__.<>1__state = -1;
			<ParseAndValidateConnectResponseAsync>d__.<>t__builder.Start<WebSocketHandle.<ParseAndValidateConnectResponseAsync>d__30>(ref <ParseAndValidateConnectResponseAsync>d__);
			return <ParseAndValidateConnectResponseAsync>d__.<>t__builder.Task;
		}

		private static void ValidateAndTrackHeader(string targetHeaderName, string targetHeaderValue, string foundHeaderName, string foundHeaderValue, ref bool foundHeader)
		{
			bool flag = string.Equals(targetHeaderName, foundHeaderName, StringComparison.OrdinalIgnoreCase);
			if (!foundHeader)
			{
				if (flag)
				{
					if (!string.Equals(targetHeaderValue, foundHeaderValue, StringComparison.OrdinalIgnoreCase))
					{
						throw new WebSocketException(SR.Format("The '{0}' header value '{1}' is invalid.", targetHeaderName, foundHeaderValue));
					}
					foundHeader = true;
					return;
				}
			}
			else if (flag)
			{
				throw new WebSocketException(SR.Format("Unable to connect to the remote server", Array.Empty<object>()));
			}
		}

		private static Task<string> ReadResponseHeaderLineAsync(Stream stream, CancellationToken cancellationToken)
		{
			WebSocketHandle.<ReadResponseHeaderLineAsync>d__32 <ReadResponseHeaderLineAsync>d__;
			<ReadResponseHeaderLineAsync>d__.stream = stream;
			<ReadResponseHeaderLineAsync>d__.cancellationToken = cancellationToken;
			<ReadResponseHeaderLineAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadResponseHeaderLineAsync>d__.<>1__state = -1;
			<ReadResponseHeaderLineAsync>d__.<>t__builder.Start<WebSocketHandle.<ReadResponseHeaderLineAsync>d__32>(ref <ReadResponseHeaderLineAsync>d__);
			return <ReadResponseHeaderLineAsync>d__.<>t__builder.Task;
		}

		[ThreadStatic]
		private static StringBuilder t_cachedStringBuilder;

		private static readonly Encoding s_defaultHttpEncoding = Encoding.GetEncoding(28591);

		private const int DefaultReceiveBufferSize = 4096;

		private const string WSServerGuid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

		private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

		private WebSocketState _state = WebSocketState.Connecting;

		private WebSocket _webSocket;
	}
}
