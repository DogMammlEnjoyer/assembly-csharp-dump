using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	/// <summary>The WebSocket class allows applications to send and receive data after the WebSocket upgrade has completed.</summary>
	public abstract class WebSocket : IDisposable
	{
		/// <summary>Indicates the reason why the remote endpoint initiated the close handshake.</summary>
		/// <returns>Returns <see cref="T:System.Net.WebSockets.WebSocketCloseStatus" />.</returns>
		public abstract WebSocketCloseStatus? CloseStatus { get; }

		/// <summary>Allows the remote endpoint to describe the reason why the connection was closed.</summary>
		/// <returns>Returns <see cref="T:System.String" />.</returns>
		public abstract string CloseStatusDescription { get; }

		/// <summary>Gets the subprotocol that was negotiated during the opening handshake.</summary>
		/// <returns>The subprotocol that was negotiated during the opening handshake.</returns>
		public abstract string SubProtocol { get; }

		/// <summary>Returns the current state of the WebSocket connection.</summary>
		/// <returns>The current state of the WebSocket connection.</returns>
		public abstract WebSocketState State { get; }

		/// <summary>Aborts the WebSocket connection and cancels any pending IO operations.</summary>
		public abstract void Abort();

		/// <summary>Closes the WebSocket connection as an asynchronous operation using the close handshake defined in the WebSocket protocol specification section 7.</summary>
		/// <param name="closeStatus">Indicates the reason for closing the WebSocket connection.</param>
		/// <param name="statusDescription">Specifies a human readable explanation as to why the connection is closed.</param>
		/// <param name="cancellationToken">The token that can be used to propagate notification that operations should be canceled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public abstract Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

		/// <summary>Initiates or completes the close handshake defined in the WebSocket protocol specification section 7.</summary>
		/// <param name="closeStatus">Indicates the reason for closing the WebSocket connection.</param>
		/// <param name="statusDescription">Allows applications to specify a human readable explanation as to why the connection is closed.</param>
		/// <param name="cancellationToken">The token that can be used to propagate notification that operations should be canceled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public abstract Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

		/// <summary>Used to clean up unmanaged resources for ASP.NET and self-hosted implementations.</summary>
		public abstract void Dispose();

		/// <summary>Receives data from the <see cref="T:System.Net.WebSockets.WebSocket" /> connection asynchronously.</summary>
		/// <param name="buffer">References the application buffer that is the storage location for the received data.</param>
		/// <param name="cancellationToken">Propagates the notification that operations should be canceled.</param>
		/// <returns>The task object representing the asynchronous operation. The <see cref="P:System.Threading.Tasks.Task`1.Result" /> property on the task object returns a <see cref="T:System.Byte" /> array containing the received data.</returns>
		public abstract Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

		/// <summary>Sends data over the <see cref="T:System.Net.WebSockets.WebSocket" /> connection asynchronously.</summary>
		/// <param name="buffer">The buffer to be sent over the connection.</param>
		/// <param name="messageType">Indicates whether the application is sending a binary or text message.</param>
		/// <param name="endOfMessage">Indicates whether the data in "buffer" is the last part of a message.</param>
		/// <param name="cancellationToken">The token that propagates the notification that operations should be canceled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public abstract Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

		public virtual ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			WebSocket.<ReceiveAsync>d__14 <ReceiveAsync>d__;
			<ReceiveAsync>d__.<>4__this = this;
			<ReceiveAsync>d__.buffer = buffer;
			<ReceiveAsync>d__.cancellationToken = cancellationToken;
			<ReceiveAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<ValueWebSocketReceiveResult>.Create();
			<ReceiveAsync>d__.<>1__state = -1;
			<ReceiveAsync>d__.<>t__builder.Start<WebSocket.<ReceiveAsync>d__14>(ref <ReceiveAsync>d__);
			return <ReceiveAsync>d__.<>t__builder.Task;
		}

		public virtual ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			ArraySegment<byte> buffer2;
			return new ValueTask(MemoryMarshal.TryGetArray<byte>(buffer, out buffer2) ? this.SendAsync(buffer2, messageType, endOfMessage, cancellationToken) : this.SendWithArrayPoolAsync(buffer, messageType, endOfMessage, cancellationToken));
		}

		private Task SendWithArrayPoolAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			WebSocket.<SendWithArrayPoolAsync>d__16 <SendWithArrayPoolAsync>d__;
			<SendWithArrayPoolAsync>d__.<>4__this = this;
			<SendWithArrayPoolAsync>d__.buffer = buffer;
			<SendWithArrayPoolAsync>d__.messageType = messageType;
			<SendWithArrayPoolAsync>d__.endOfMessage = endOfMessage;
			<SendWithArrayPoolAsync>d__.cancellationToken = cancellationToken;
			<SendWithArrayPoolAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendWithArrayPoolAsync>d__.<>1__state = -1;
			<SendWithArrayPoolAsync>d__.<>t__builder.Start<WebSocket.<SendWithArrayPoolAsync>d__16>(ref <SendWithArrayPoolAsync>d__);
			return <SendWithArrayPoolAsync>d__.<>t__builder.Task;
		}

		/// <summary>Gets the default WebSocket protocol keep-alive interval.</summary>
		/// <returns>The default WebSocket protocol keep-alive interval. The typical value for this interval is 30 seconds (as defined by the OS or the .NET platform). It is used to initialize <see cref="P:System.Net.WebSockets.ClientWebSocketOptions.KeepAliveInterval" /> value.</returns>
		public static TimeSpan DefaultKeepAliveInterval
		{
			get
			{
				return TimeSpan.FromSeconds(30.0);
			}
		}

		/// <summary>Verifies that the connection is in an expected state.</summary>
		/// <param name="state">The current state of the WebSocket to be tested against the list of valid states.</param>
		/// <param name="validStates">List of valid connection states.</param>
		protected static void ThrowOnInvalidState(WebSocketState state, params WebSocketState[] validStates)
		{
			string p = string.Empty;
			if (validStates != null && validStates.Length != 0)
			{
				foreach (WebSocketState webSocketState in validStates)
				{
					if (state == webSocketState)
					{
						return;
					}
				}
				p = string.Join<WebSocketState>(", ", validStates);
			}
			throw new WebSocketException(SR.Format("The WebSocket is in an invalid state ('{0}') for this operation. Valid states are: '{1}'", state, p));
		}

		/// <summary>Returns a value that indicates if the state of the WebSocket instance is closed or aborted.</summary>
		/// <param name="state">The current state of the WebSocket.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.WebSockets.WebSocket" /> is closed or aborted; otherwise, <see langword="false" />.</returns>
		protected static bool IsStateTerminal(WebSocketState state)
		{
			return state == WebSocketState.Closed || state == WebSocketState.Aborted;
		}

		/// <summary>Create client buffers to use with this <see cref="T:System.Net.WebSockets.WebSocket" /> instance.</summary>
		/// <param name="receiveBufferSize">The size, in bytes, of the client receive buffer.</param>
		/// <param name="sendBufferSize">The size, in bytes, of the send buffer.</param>
		/// <returns>An array with the client buffers.</returns>
		public static ArraySegment<byte> CreateClientBuffer(int receiveBufferSize, int sendBufferSize)
		{
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			if (sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("sendBufferSize", sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			return new ArraySegment<byte>(new byte[Math.Max(receiveBufferSize, sendBufferSize)]);
		}

		/// <summary>Creates a WebSocket server buffer.</summary>
		/// <param name="receiveBufferSize">The size, in bytes, of the desired buffer.</param>
		/// <returns>Returns <see cref="T:System.ArraySegment`1" />.</returns>
		public static ArraySegment<byte> CreateServerBuffer(int receiveBufferSize)
		{
			if (receiveBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("receiveBufferSize", receiveBufferSize, SR.Format("The argument must be a value greater than {0}.", 1));
			}
			return new ArraySegment<byte>(new byte[receiveBufferSize]);
		}

		public static WebSocket CreateFromStream(Stream stream, bool isServer, string subProtocol, TimeSpan keepAliveInterval)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead || !stream.CanWrite)
			{
				throw new ArgumentException((!stream.CanRead) ? "The base stream is not readable." : "The base stream is not writeable.", "stream");
			}
			if (subProtocol != null)
			{
				WebSocketValidate.ValidateSubprotocol(subProtocol);
			}
			if (keepAliveInterval != Timeout.InfiniteTimeSpan && keepAliveInterval < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("keepAliveInterval", keepAliveInterval, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			return ManagedWebSocket.CreateFromConnectedStream(stream, isServer, subProtocol, keepAliveInterval);
		}

		/// <summary>Returns a value that indicates if the WebSocket instance is targeting .NET Framework 4.5.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.WebSockets.WebSocket" /> is targeting .NET Framework 4.5; otherwise, <see langword="false" />.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.")]
		public static bool IsApplicationTargeting45()
		{
			return true;
		}

		/// <summary>Allows callers to register prefixes for WebSocket requests (ws and wss).</summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RegisterPrefixes()
		{
			throw new PlatformNotSupportedException();
		}

		/// <summary>Allows callers to create a client side WebSocket class which will use the WSPC for framing purposes.</summary>
		/// <param name="innerStream">The connection to be used for IO operations.</param>
		/// <param name="subProtocol">The subprotocol accepted by the client.</param>
		/// <param name="receiveBufferSize">The size in bytes of the client WebSocket receive buffer.</param>
		/// <param name="sendBufferSize">The size in bytes of the client WebSocket send buffer.</param>
		/// <param name="keepAliveInterval">Determines how regularly a frame is sent over the connection as a keep-alive. Applies only when the connection is idle.</param>
		/// <param name="useZeroMaskingKey">Indicates whether a random key or a static key (just zeros) should be used for the WebSocket masking.</param>
		/// <param name="internalBuffer">Will be used as the internal buffer in the WPC. The size has to be at least <c>2 * ReceiveBufferSize + SendBufferSize + 256 + 20 (16 on 32-bit)</c>.</param>
		/// <returns>Returns <see cref="T:System.Net.WebSockets.WebSocket" />.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static WebSocket CreateClientWebSocket(Stream innerStream, string subProtocol, int receiveBufferSize, int sendBufferSize, TimeSpan keepAliveInterval, bool useZeroMaskingKey, ArraySegment<byte> internalBuffer)
		{
			if (innerStream == null)
			{
				throw new ArgumentNullException("innerStream");
			}
			if (!innerStream.CanRead || !innerStream.CanWrite)
			{
				throw new ArgumentException((!innerStream.CanRead) ? "The base stream is not readable." : "The base stream is not writeable.", "innerStream");
			}
			if (subProtocol != null)
			{
				WebSocketValidate.ValidateSubprotocol(subProtocol);
			}
			if (keepAliveInterval != Timeout.InfiniteTimeSpan && keepAliveInterval < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("keepAliveInterval", keepAliveInterval, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			if (receiveBufferSize <= 0 || sendBufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException((receiveBufferSize <= 0) ? "receiveBufferSize" : "sendBufferSize", (receiveBufferSize <= 0) ? receiveBufferSize : sendBufferSize, SR.Format("The argument must be a value greater than {0}.", 0));
			}
			return ManagedWebSocket.CreateFromConnectedStream(innerStream, false, subProtocol, keepAliveInterval);
		}
	}
}
