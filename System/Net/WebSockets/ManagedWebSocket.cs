using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.WebSockets
{
	internal sealed class ManagedWebSocket : WebSocket
	{
		public static ManagedWebSocket CreateFromConnectedStream(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval)
		{
			return new ManagedWebSocket(stream, isServer, subprotocol, keepAliveInterval);
		}

		private object StateUpdateLock
		{
			get
			{
				return this._abortSource;
			}
		}

		private object ReceiveAsyncLock
		{
			get
			{
				return this._utf8TextState;
			}
		}

		private ManagedWebSocket(Stream stream, bool isServer, string subprotocol, TimeSpan keepAliveInterval)
		{
			this._stream = stream;
			this._isServer = isServer;
			this._subprotocol = subprotocol;
			this._receiveBuffer = new byte[125];
			this._abortSource.Token.Register(delegate(object s)
			{
				ManagedWebSocket managedWebSocket = (ManagedWebSocket)s;
				object stateUpdateLock = managedWebSocket.StateUpdateLock;
				lock (stateUpdateLock)
				{
					WebSocketState state = managedWebSocket._state;
					if (state != WebSocketState.Closed && state != WebSocketState.Aborted)
					{
						managedWebSocket._state = ((state != WebSocketState.None && state != WebSocketState.Connecting) ? WebSocketState.Aborted : WebSocketState.Closed);
					}
				}
			}, this);
			if (keepAliveInterval > TimeSpan.Zero)
			{
				this._keepAliveTimer = new Timer(delegate(object s)
				{
					((ManagedWebSocket)s).SendKeepAliveFrameAsync();
				}, this, keepAliveInterval, keepAliveInterval);
			}
		}

		public override void Dispose()
		{
			object stateUpdateLock = this.StateUpdateLock;
			lock (stateUpdateLock)
			{
				this.DisposeCore();
			}
		}

		private void DisposeCore()
		{
			if (!this._disposed)
			{
				this._disposed = true;
				Timer keepAliveTimer = this._keepAliveTimer;
				if (keepAliveTimer != null)
				{
					keepAliveTimer.Dispose();
				}
				Stream stream = this._stream;
				if (stream != null)
				{
					stream.Dispose();
				}
				if (this._state < WebSocketState.Aborted)
				{
					this._state = WebSocketState.Closed;
				}
			}
		}

		public override WebSocketCloseStatus? CloseStatus
		{
			get
			{
				return this._closeStatus;
			}
		}

		public override string CloseStatusDescription
		{
			get
			{
				return this._closeStatusDescription;
			}
		}

		public override WebSocketState State
		{
			get
			{
				return this._state;
			}
		}

		public override string SubProtocol
		{
			get
			{
				return this._subprotocol;
			}
		}

		public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			if (messageType != WebSocketMessageType.Text && messageType != WebSocketMessageType.Binary)
			{
				throw new ArgumentException(SR.Format("The message type '{0}' is not allowed for the '{1}' operation. Valid message types are: '{2}, {3}'. To close the WebSocket, use the '{4}' operation instead. ", new object[]
				{
					"Close",
					"SendAsync",
					"Binary",
					"Text",
					"CloseOutputAsync"
				}), "messageType");
			}
			WebSocketValidate.ValidateArraySegment(buffer, "buffer");
			return this.SendPrivateAsync(buffer, messageType, endOfMessage, cancellationToken).AsTask();
		}

		private ValueTask SendPrivateAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
		{
			if (messageType != WebSocketMessageType.Text && messageType != WebSocketMessageType.Binary)
			{
				throw new ArgumentException(SR.Format("The message type '{0}' is not allowed for the '{1}' operation. Valid message types are: '{2}, {3}'. To close the WebSocket, use the '{4}' operation instead. ", new object[]
				{
					"Close",
					"SendAsync",
					"Binary",
					"Text",
					"CloseOutputAsync"
				}), "messageType");
			}
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validSendStates);
			}
			catch (Exception exception)
			{
				return new ValueTask(Task.FromException(exception));
			}
			ManagedWebSocket.MessageOpcode opcode = this._lastSendWasFragment ? ManagedWebSocket.MessageOpcode.Continuation : ((messageType == WebSocketMessageType.Binary) ? ManagedWebSocket.MessageOpcode.Binary : ManagedWebSocket.MessageOpcode.Text);
			ValueTask result = this.SendFrameAsync(opcode, endOfMessage, buffer, cancellationToken);
			this._lastSendWasFragment = !endOfMessage;
			return result;
		}

		public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateArraySegment(buffer, "buffer");
			Task<WebSocketReceiveResult> result;
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validReceiveStates);
				object receiveAsyncLock = this.ReceiveAsyncLock;
				lock (receiveAsyncLock)
				{
					this.ThrowIfOperationInProgress(this._lastReceiveAsync.IsCompleted, "ReceiveAsync");
					Task<WebSocketReceiveResult> task = this.ReceiveAsyncPrivate<ManagedWebSocket.WebSocketReceiveResultGetter, WebSocketReceiveResult>(buffer, cancellationToken, default(ManagedWebSocket.WebSocketReceiveResultGetter)).AsTask();
					this._lastReceiveAsync = task;
					result = task;
				}
			}
			catch (Exception exception)
			{
				result = Task.FromException<WebSocketReceiveResult>(exception);
			}
			return result;
		}

		public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validCloseStates);
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
			return this.CloseAsyncPrivate(closeStatus, statusDescription, cancellationToken);
		}

		public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			WebSocketValidate.ValidateCloseStatus(closeStatus, statusDescription);
			try
			{
				WebSocketValidate.ThrowIfInvalidState(this._state, this._disposed, ManagedWebSocket.s_validCloseOutputStates);
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
			return this.SendCloseFrameAsync(closeStatus, statusDescription, cancellationToken);
		}

		public override void Abort()
		{
			this._abortSource.Cancel();
			this.Dispose();
		}

		private ValueTask SendFrameAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer, CancellationToken cancellationToken)
		{
			if (!cancellationToken.CanBeCanceled && this._sendFrameAsyncLock.Wait(0))
			{
				return this.SendFrameLockAcquiredNonCancelableAsync(opcode, endOfMessage, payloadBuffer);
			}
			return new ValueTask(this.SendFrameFallbackAsync(opcode, endOfMessage, payloadBuffer, cancellationToken));
		}

		private ValueTask SendFrameLockAcquiredNonCancelableAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer)
		{
			ValueTask valueTask = default(ValueTask);
			bool flag = true;
			try
			{
				int length = this.WriteFrameToSendBuffer(opcode, endOfMessage, payloadBuffer.Span);
				valueTask = this._stream.WriteAsync(new ReadOnlyMemory<byte>(this._sendBuffer, 0, length), default(CancellationToken));
				if (valueTask.IsCompleted)
				{
					return valueTask;
				}
				flag = false;
			}
			catch (Exception ex)
			{
				return new ValueTask(Task.FromException((ex is OperationCanceledException) ? ex : ((this._state == WebSocketState.Aborted) ? ManagedWebSocket.CreateOperationCanceledException(ex, default(CancellationToken)) : new WebSocketException(WebSocketError.ConnectionClosedPrematurely, ex))));
			}
			finally
			{
				if (flag)
				{
					this.ReleaseSendBuffer();
					this._sendFrameAsyncLock.Release();
				}
			}
			return new ValueTask(this.WaitForWriteTaskAsync(valueTask));
		}

		private Task WaitForWriteTaskAsync(ValueTask writeTask)
		{
			ManagedWebSocket.<WaitForWriteTaskAsync>d__55 <WaitForWriteTaskAsync>d__;
			<WaitForWriteTaskAsync>d__.<>4__this = this;
			<WaitForWriteTaskAsync>d__.writeTask = writeTask;
			<WaitForWriteTaskAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForWriteTaskAsync>d__.<>1__state = -1;
			<WaitForWriteTaskAsync>d__.<>t__builder.Start<ManagedWebSocket.<WaitForWriteTaskAsync>d__55>(ref <WaitForWriteTaskAsync>d__);
			return <WaitForWriteTaskAsync>d__.<>t__builder.Task;
		}

		private Task SendFrameFallbackAsync(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ReadOnlyMemory<byte> payloadBuffer, CancellationToken cancellationToken)
		{
			ManagedWebSocket.<SendFrameFallbackAsync>d__56 <SendFrameFallbackAsync>d__;
			<SendFrameFallbackAsync>d__.<>4__this = this;
			<SendFrameFallbackAsync>d__.opcode = opcode;
			<SendFrameFallbackAsync>d__.endOfMessage = endOfMessage;
			<SendFrameFallbackAsync>d__.payloadBuffer = payloadBuffer;
			<SendFrameFallbackAsync>d__.cancellationToken = cancellationToken;
			<SendFrameFallbackAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendFrameFallbackAsync>d__.<>1__state = -1;
			<SendFrameFallbackAsync>d__.<>t__builder.Start<ManagedWebSocket.<SendFrameFallbackAsync>d__56>(ref <SendFrameFallbackAsync>d__);
			return <SendFrameFallbackAsync>d__.<>t__builder.Task;
		}

		private int WriteFrameToSendBuffer(ManagedWebSocket.MessageOpcode opcode, bool endOfMessage, ReadOnlySpan<byte> payloadBuffer)
		{
			this.AllocateSendBuffer(payloadBuffer.Length + 14);
			int? num = null;
			int num2;
			if (this._isServer)
			{
				num2 = ManagedWebSocket.WriteHeader(opcode, this._sendBuffer, payloadBuffer, endOfMessage, false);
			}
			else
			{
				num = new int?(ManagedWebSocket.WriteHeader(opcode, this._sendBuffer, payloadBuffer, endOfMessage, true));
				num2 = num.GetValueOrDefault() + 4;
			}
			if (payloadBuffer.Length > 0)
			{
				payloadBuffer.CopyTo(new Span<byte>(this._sendBuffer, num2, payloadBuffer.Length));
				if (num != null)
				{
					ManagedWebSocket.ApplyMask(new Span<byte>(this._sendBuffer, num2, payloadBuffer.Length), this._sendBuffer, num.Value, 0);
				}
			}
			return num2 + payloadBuffer.Length;
		}

		private void SendKeepAliveFrameAsync()
		{
			if (this._sendFrameAsyncLock.Wait(0))
			{
				ValueTask valueTask = this.SendFrameLockAcquiredNonCancelableAsync(ManagedWebSocket.MessageOpcode.Ping, true, Memory<byte>.Empty);
				if (valueTask.IsCompletedSuccessfully)
				{
					valueTask.GetAwaiter().GetResult();
					return;
				}
				valueTask.AsTask().ContinueWith(delegate(Task p)
				{
					AggregateException exception = p.Exception;
				}, CancellationToken.None, TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			}
		}

		private static int WriteHeader(ManagedWebSocket.MessageOpcode opcode, byte[] sendBuffer, ReadOnlySpan<byte> payload, bool endOfMessage, bool useMask)
		{
			sendBuffer[0] = (byte)opcode;
			if (endOfMessage)
			{
				int num = 0;
				sendBuffer[num] |= 128;
			}
			int num2;
			if (payload.Length <= 125)
			{
				sendBuffer[1] = (byte)payload.Length;
				num2 = 2;
			}
			else if (payload.Length <= 65535)
			{
				sendBuffer[1] = 126;
				sendBuffer[2] = (byte)(payload.Length / 256);
				sendBuffer[3] = (byte)payload.Length;
				num2 = 4;
			}
			else
			{
				sendBuffer[1] = 127;
				int num3 = payload.Length;
				for (int i = 9; i >= 2; i--)
				{
					sendBuffer[i] = (byte)num3;
					num3 /= 256;
				}
				num2 = 10;
			}
			if (useMask)
			{
				int num4 = 1;
				sendBuffer[num4] |= 128;
				ManagedWebSocket.WriteRandomMask(sendBuffer, num2);
			}
			return num2;
		}

		private static void WriteRandomMask(byte[] buffer, int offset)
		{
			ManagedWebSocket.s_random.GetBytes(buffer, offset, 4);
		}

		private ValueTask<TWebSocketReceiveResult> ReceiveAsyncPrivate<TWebSocketReceiveResultGetter, TWebSocketReceiveResult>(Memory<byte> payloadBuffer, CancellationToken cancellationToken, TWebSocketReceiveResultGetter resultGetter = default(TWebSocketReceiveResultGetter)) where TWebSocketReceiveResultGetter : struct, ManagedWebSocket.IWebSocketReceiveResultGetter<TWebSocketReceiveResult>
		{
			ManagedWebSocket.<ReceiveAsyncPrivate>d__61<TWebSocketReceiveResultGetter, TWebSocketReceiveResult> <ReceiveAsyncPrivate>d__;
			<ReceiveAsyncPrivate>d__.<>4__this = this;
			<ReceiveAsyncPrivate>d__.payloadBuffer = payloadBuffer;
			<ReceiveAsyncPrivate>d__.cancellationToken = cancellationToken;
			<ReceiveAsyncPrivate>d__.resultGetter = resultGetter;
			<ReceiveAsyncPrivate>d__.<>t__builder = AsyncValueTaskMethodBuilder<TWebSocketReceiveResult>.Create();
			<ReceiveAsyncPrivate>d__.<>1__state = -1;
			<ReceiveAsyncPrivate>d__.<>t__builder.Start<ManagedWebSocket.<ReceiveAsyncPrivate>d__61<TWebSocketReceiveResultGetter, TWebSocketReceiveResult>>(ref <ReceiveAsyncPrivate>d__);
			return <ReceiveAsyncPrivate>d__.<>t__builder.Task;
		}

		private Task HandleReceivedCloseAsync(ManagedWebSocket.MessageHeader header, CancellationToken cancellationToken)
		{
			ManagedWebSocket.<HandleReceivedCloseAsync>d__62 <HandleReceivedCloseAsync>d__;
			<HandleReceivedCloseAsync>d__.<>4__this = this;
			<HandleReceivedCloseAsync>d__.header = header;
			<HandleReceivedCloseAsync>d__.cancellationToken = cancellationToken;
			<HandleReceivedCloseAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleReceivedCloseAsync>d__.<>1__state = -1;
			<HandleReceivedCloseAsync>d__.<>t__builder.Start<ManagedWebSocket.<HandleReceivedCloseAsync>d__62>(ref <HandleReceivedCloseAsync>d__);
			return <HandleReceivedCloseAsync>d__.<>t__builder.Task;
		}

		private Task WaitForServerToCloseConnectionAsync(CancellationToken cancellationToken)
		{
			ManagedWebSocket.<WaitForServerToCloseConnectionAsync>d__63 <WaitForServerToCloseConnectionAsync>d__;
			<WaitForServerToCloseConnectionAsync>d__.<>4__this = this;
			<WaitForServerToCloseConnectionAsync>d__.cancellationToken = cancellationToken;
			<WaitForServerToCloseConnectionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForServerToCloseConnectionAsync>d__.<>1__state = -1;
			<WaitForServerToCloseConnectionAsync>d__.<>t__builder.Start<ManagedWebSocket.<WaitForServerToCloseConnectionAsync>d__63>(ref <WaitForServerToCloseConnectionAsync>d__);
			return <WaitForServerToCloseConnectionAsync>d__.<>t__builder.Task;
		}

		private Task HandleReceivedPingPongAsync(ManagedWebSocket.MessageHeader header, CancellationToken cancellationToken)
		{
			ManagedWebSocket.<HandleReceivedPingPongAsync>d__64 <HandleReceivedPingPongAsync>d__;
			<HandleReceivedPingPongAsync>d__.<>4__this = this;
			<HandleReceivedPingPongAsync>d__.header = header;
			<HandleReceivedPingPongAsync>d__.cancellationToken = cancellationToken;
			<HandleReceivedPingPongAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleReceivedPingPongAsync>d__.<>1__state = -1;
			<HandleReceivedPingPongAsync>d__.<>t__builder.Start<ManagedWebSocket.<HandleReceivedPingPongAsync>d__64>(ref <HandleReceivedPingPongAsync>d__);
			return <HandleReceivedPingPongAsync>d__.<>t__builder.Task;
		}

		private static bool IsValidCloseStatus(WebSocketCloseStatus closeStatus)
		{
			return closeStatus >= WebSocketCloseStatus.NormalClosure && closeStatus < (WebSocketCloseStatus)5000 && (closeStatus >= (WebSocketCloseStatus)3000 || (closeStatus - WebSocketCloseStatus.NormalClosure <= 3 || closeStatus - WebSocketCloseStatus.InvalidPayloadData <= 4));
		}

		private Task CloseWithReceiveErrorAndThrowAsync(WebSocketCloseStatus closeStatus, WebSocketError error, Exception innerException = null)
		{
			ManagedWebSocket.<CloseWithReceiveErrorAndThrowAsync>d__66 <CloseWithReceiveErrorAndThrowAsync>d__;
			<CloseWithReceiveErrorAndThrowAsync>d__.<>4__this = this;
			<CloseWithReceiveErrorAndThrowAsync>d__.closeStatus = closeStatus;
			<CloseWithReceiveErrorAndThrowAsync>d__.error = error;
			<CloseWithReceiveErrorAndThrowAsync>d__.innerException = innerException;
			<CloseWithReceiveErrorAndThrowAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CloseWithReceiveErrorAndThrowAsync>d__.<>1__state = -1;
			<CloseWithReceiveErrorAndThrowAsync>d__.<>t__builder.Start<ManagedWebSocket.<CloseWithReceiveErrorAndThrowAsync>d__66>(ref <CloseWithReceiveErrorAndThrowAsync>d__);
			return <CloseWithReceiveErrorAndThrowAsync>d__.<>t__builder.Task;
		}

		private unsafe bool TryParseMessageHeaderFromReceiveBuffer(out ManagedWebSocket.MessageHeader resultHeader)
		{
			ManagedWebSocket.MessageHeader messageHeader = default(ManagedWebSocket.MessageHeader);
			Span<byte> span = this._receiveBuffer.Span;
			messageHeader.Fin = ((*span[this._receiveBufferOffset] & 128) > 0);
			bool flag = (*span[this._receiveBufferOffset] & 112) > 0;
			messageHeader.Opcode = (ManagedWebSocket.MessageOpcode)(*span[this._receiveBufferOffset] & 15);
			bool flag2 = (*span[this._receiveBufferOffset + 1] & 128) > 0;
			messageHeader.PayloadLength = (long)(*span[this._receiveBufferOffset + 1] & 127);
			this.ConsumeFromBuffer(2);
			if (messageHeader.PayloadLength == 126L)
			{
				messageHeader.PayloadLength = (long)((int)(*span[this._receiveBufferOffset]) << 8 | (int)(*span[this._receiveBufferOffset + 1]));
				this.ConsumeFromBuffer(2);
			}
			else if (messageHeader.PayloadLength == 127L)
			{
				messageHeader.PayloadLength = 0L;
				for (int i = 0; i < 8; i++)
				{
					messageHeader.PayloadLength = (messageHeader.PayloadLength << 8 | (long)((ulong)(*span[this._receiveBufferOffset + i])));
				}
				this.ConsumeFromBuffer(8);
			}
			bool flag3 = flag;
			if (flag2)
			{
				if (!this._isServer)
				{
					flag3 = true;
				}
				messageHeader.Mask = ManagedWebSocket.CombineMaskBytes(span, this._receiveBufferOffset);
				this.ConsumeFromBuffer(4);
			}
			switch (messageHeader.Opcode)
			{
			case ManagedWebSocket.MessageOpcode.Continuation:
				if (this._lastReceiveHeader.Fin)
				{
					flag3 = true;
					goto IL_1CD;
				}
				goto IL_1CD;
			case ManagedWebSocket.MessageOpcode.Text:
			case ManagedWebSocket.MessageOpcode.Binary:
				if (!this._lastReceiveHeader.Fin)
				{
					flag3 = true;
					goto IL_1CD;
				}
				goto IL_1CD;
			case ManagedWebSocket.MessageOpcode.Close:
			case ManagedWebSocket.MessageOpcode.Ping:
			case ManagedWebSocket.MessageOpcode.Pong:
				if (messageHeader.PayloadLength > 125L || !messageHeader.Fin)
				{
					flag3 = true;
					goto IL_1CD;
				}
				goto IL_1CD;
			}
			flag3 = true;
			IL_1CD:
			resultHeader = messageHeader;
			return !flag3;
		}

		private Task CloseAsyncPrivate(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
		{
			ManagedWebSocket.<CloseAsyncPrivate>d__68 <CloseAsyncPrivate>d__;
			<CloseAsyncPrivate>d__.<>4__this = this;
			<CloseAsyncPrivate>d__.closeStatus = closeStatus;
			<CloseAsyncPrivate>d__.statusDescription = statusDescription;
			<CloseAsyncPrivate>d__.cancellationToken = cancellationToken;
			<CloseAsyncPrivate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CloseAsyncPrivate>d__.<>1__state = -1;
			<CloseAsyncPrivate>d__.<>t__builder.Start<ManagedWebSocket.<CloseAsyncPrivate>d__68>(ref <CloseAsyncPrivate>d__);
			return <CloseAsyncPrivate>d__.<>t__builder.Task;
		}

		private Task SendCloseFrameAsync(WebSocketCloseStatus closeStatus, string closeStatusDescription, CancellationToken cancellationToken)
		{
			ManagedWebSocket.<SendCloseFrameAsync>d__69 <SendCloseFrameAsync>d__;
			<SendCloseFrameAsync>d__.<>4__this = this;
			<SendCloseFrameAsync>d__.closeStatus = closeStatus;
			<SendCloseFrameAsync>d__.closeStatusDescription = closeStatusDescription;
			<SendCloseFrameAsync>d__.cancellationToken = cancellationToken;
			<SendCloseFrameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendCloseFrameAsync>d__.<>1__state = -1;
			<SendCloseFrameAsync>d__.<>t__builder.Start<ManagedWebSocket.<SendCloseFrameAsync>d__69>(ref <SendCloseFrameAsync>d__);
			return <SendCloseFrameAsync>d__.<>t__builder.Task;
		}

		private void ConsumeFromBuffer(int count)
		{
			this._receiveBufferCount -= count;
			this._receiveBufferOffset += count;
		}

		private Task EnsureBufferContainsAsync(int minimumRequiredBytes, CancellationToken cancellationToken, bool throwOnPrematureClosure = true)
		{
			ManagedWebSocket.<EnsureBufferContainsAsync>d__71 <EnsureBufferContainsAsync>d__;
			<EnsureBufferContainsAsync>d__.<>4__this = this;
			<EnsureBufferContainsAsync>d__.minimumRequiredBytes = minimumRequiredBytes;
			<EnsureBufferContainsAsync>d__.cancellationToken = cancellationToken;
			<EnsureBufferContainsAsync>d__.throwOnPrematureClosure = throwOnPrematureClosure;
			<EnsureBufferContainsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EnsureBufferContainsAsync>d__.<>1__state = -1;
			<EnsureBufferContainsAsync>d__.<>t__builder.Start<ManagedWebSocket.<EnsureBufferContainsAsync>d__71>(ref <EnsureBufferContainsAsync>d__);
			return <EnsureBufferContainsAsync>d__.<>t__builder.Task;
		}

		private void ThrowIfEOFUnexpected(bool throwOnPrematureClosure)
		{
			if (this._disposed)
			{
				throw new ObjectDisposedException("WebSocket");
			}
			if (throwOnPrematureClosure)
			{
				throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely);
			}
		}

		private void AllocateSendBuffer(int minLength)
		{
			this._sendBuffer = ArrayPool<byte>.Shared.Rent(minLength);
		}

		private void ReleaseSendBuffer()
		{
			byte[] sendBuffer = this._sendBuffer;
			if (sendBuffer != null)
			{
				this._sendBuffer = null;
				ArrayPool<byte>.Shared.Return(sendBuffer, false);
			}
		}

		private static int CombineMaskBytes(Span<byte> buffer, int maskOffset)
		{
			return BitConverter.ToInt32(buffer.Slice(maskOffset));
		}

		private static int ApplyMask(Span<byte> toMask, byte[] mask, int maskOffset, int maskOffsetIndex)
		{
			return ManagedWebSocket.ApplyMask(toMask, ManagedWebSocket.CombineMaskBytes(mask, maskOffset), maskOffsetIndex);
		}

		private unsafe static int ApplyMask(Span<byte> toMask, int mask, int maskIndex)
		{
			int num = maskIndex * 8;
			int num2 = (int)((uint)mask >> num | (uint)((uint)mask << 32 - num));
			int i = toMask.Length;
			if (i > 0)
			{
				fixed (byte* reference = MemoryMarshal.GetReference<byte>(toMask))
				{
					byte* ptr = reference;
					if (ptr % 4L == null)
					{
						while (i >= 4)
						{
							i -= 4;
							*(int*)ptr ^= num2;
							ptr += 4;
						}
					}
					if (i > 0)
					{
						byte* ptr2 = (byte*)(&mask);
						byte* ptr3 = ptr + i;
						while (ptr < ptr3)
						{
							byte* ptr4 = ptr++;
							*ptr4 ^= ptr2[maskIndex];
							maskIndex = (maskIndex + 1 & 3);
						}
					}
				}
			}
			return maskIndex;
		}

		private void ThrowIfOperationInProgress(bool operationCompleted, [CallerMemberName] string methodName = null)
		{
			if (!operationCompleted)
			{
				this.Abort();
				this.ThrowOperationInProgress(methodName);
			}
		}

		private void ThrowOperationInProgress(string methodName)
		{
			throw new InvalidOperationException(SR.Format("There is already one outstanding '{0}' call for this WebSocket instance. ReceiveAsync and SendAsync can be called simultaneously, but at most one outstanding operation for each of them is allowed at the same time.", methodName));
		}

		private static Exception CreateOperationCanceledException(Exception innerException, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new OperationCanceledException(new OperationCanceledException().Message, innerException, cancellationToken);
		}

		private unsafe static bool TryValidateUtf8(Span<byte> span, bool endOfMessage, ManagedWebSocket.Utf8MessageState state)
		{
			int i = 0;
			while (i < span.Length)
			{
				if (!state.SequenceInProgress)
				{
					state.SequenceInProgress = true;
					byte b = *span[i];
					i++;
					if ((b & 128) == 0)
					{
						state.AdditionalBytesExpected = 0;
						state.CurrentDecodeBits = (int)(b & 127);
						state.ExpectedValueMin = 0;
					}
					else
					{
						if ((b & 192) == 128)
						{
							return false;
						}
						if ((b & 224) == 192)
						{
							state.AdditionalBytesExpected = 1;
							state.CurrentDecodeBits = (int)(b & 31);
							state.ExpectedValueMin = 128;
						}
						else if ((b & 240) == 224)
						{
							state.AdditionalBytesExpected = 2;
							state.CurrentDecodeBits = (int)(b & 15);
							state.ExpectedValueMin = 2048;
						}
						else
						{
							if ((b & 248) != 240)
							{
								return false;
							}
							state.AdditionalBytesExpected = 3;
							state.CurrentDecodeBits = (int)(b & 7);
							state.ExpectedValueMin = 65536;
						}
					}
				}
				while (state.AdditionalBytesExpected > 0 && i < span.Length)
				{
					byte b2 = *span[i];
					if ((b2 & 192) != 128)
					{
						return false;
					}
					i++;
					state.AdditionalBytesExpected--;
					state.CurrentDecodeBits = (state.CurrentDecodeBits << 6 | (int)(b2 & 63));
					if (state.AdditionalBytesExpected == 1 && state.CurrentDecodeBits >= 864 && state.CurrentDecodeBits <= 895)
					{
						return false;
					}
					if (state.AdditionalBytesExpected == 2 && state.CurrentDecodeBits >= 272)
					{
						return false;
					}
				}
				if (state.AdditionalBytesExpected == 0)
				{
					state.SequenceInProgress = false;
					if (state.CurrentDecodeBits < state.ExpectedValueMin)
					{
						return false;
					}
				}
			}
			return !endOfMessage || !state.SequenceInProgress;
		}

		private Task ValidateAndReceiveAsync(Task receiveTask, byte[] buffer, CancellationToken cancellationToken)
		{
			if (receiveTask != null)
			{
				if (receiveTask.Status != TaskStatus.RanToCompletion)
				{
					return receiveTask;
				}
				Task<WebSocketReceiveResult> task = receiveTask as Task<WebSocketReceiveResult>;
				if (task != null && task.Result.MessageType == WebSocketMessageType.Close)
				{
					return receiveTask;
				}
			}
			receiveTask = this.ReceiveAsyncPrivate<ManagedWebSocket.WebSocketReceiveResultGetter, WebSocketReceiveResult>(new ArraySegment<byte>(buffer), cancellationToken, default(ManagedWebSocket.WebSocketReceiveResultGetter)).AsTask();
			return receiveTask;
		}

		private static readonly RandomNumberGenerator s_random = RandomNumberGenerator.Create();

		private static readonly UTF8Encoding s_textEncoding = new UTF8Encoding(false, true);

		private static readonly WebSocketState[] s_validSendStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived
		};

		private static readonly WebSocketState[] s_validReceiveStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseSent
		};

		private static readonly WebSocketState[] s_validCloseOutputStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived
		};

		private static readonly WebSocketState[] s_validCloseStates = new WebSocketState[]
		{
			WebSocketState.Open,
			WebSocketState.CloseReceived,
			WebSocketState.CloseSent
		};

		private static readonly Task<WebSocketReceiveResult> s_cachedCloseTask = Task.FromResult<WebSocketReceiveResult>(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true));

		internal const int MaxMessageHeaderLength = 14;

		private const int MaxControlPayloadLength = 125;

		private const int MaskLength = 4;

		private readonly Stream _stream;

		private readonly bool _isServer;

		private readonly string _subprotocol;

		private readonly Timer _keepAliveTimer;

		private readonly CancellationTokenSource _abortSource = new CancellationTokenSource();

		private Memory<byte> _receiveBuffer;

		private readonly ManagedWebSocket.Utf8MessageState _utf8TextState = new ManagedWebSocket.Utf8MessageState();

		private readonly SemaphoreSlim _sendFrameAsyncLock = new SemaphoreSlim(1, 1);

		private WebSocketState _state = WebSocketState.Open;

		private bool _disposed;

		private bool _sentCloseFrame;

		private bool _receivedCloseFrame;

		private WebSocketCloseStatus? _closeStatus;

		private string _closeStatusDescription;

		private ManagedWebSocket.MessageHeader _lastReceiveHeader = new ManagedWebSocket.MessageHeader
		{
			Opcode = ManagedWebSocket.MessageOpcode.Text,
			Fin = true
		};

		private int _receiveBufferOffset;

		private int _receiveBufferCount;

		private int _receivedMaskOffsetOffset;

		private byte[] _sendBuffer;

		private bool _lastSendWasFragment;

		private Task _lastReceiveAsync = Task.CompletedTask;

		private sealed class Utf8MessageState
		{
			internal bool SequenceInProgress;

			internal int AdditionalBytesExpected;

			internal int ExpectedValueMin;

			internal int CurrentDecodeBits;
		}

		private enum MessageOpcode : byte
		{
			Continuation,
			Text,
			Binary,
			Close = 8,
			Ping,
			Pong
		}

		[StructLayout(LayoutKind.Auto)]
		private struct MessageHeader
		{
			internal ManagedWebSocket.MessageOpcode Opcode;

			internal bool Fin;

			internal long PayloadLength;

			internal int Mask;
		}

		private interface IWebSocketReceiveResultGetter<TResult>
		{
			TResult GetResult(int count, WebSocketMessageType messageType, bool endOfMessage, WebSocketCloseStatus? closeStatus, string closeDescription);
		}

		private readonly struct WebSocketReceiveResultGetter : ManagedWebSocket.IWebSocketReceiveResultGetter<WebSocketReceiveResult>
		{
			public WebSocketReceiveResult GetResult(int count, WebSocketMessageType messageType, bool endOfMessage, WebSocketCloseStatus? closeStatus, string closeDescription)
			{
				return new WebSocketReceiveResult(count, messageType, endOfMessage, closeStatus, closeDescription);
			}
		}
	}
}
