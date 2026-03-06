using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Mono;

namespace System.Net.Sockets
{
	/// <summary>Implements the Berkeley sockets interface.</summary>
	public class Socket : IDisposable
	{
		internal Task<Socket> AcceptAsync(Socket acceptSocket)
		{
			Socket.TaskSocketAsyncEventArgs<Socket> taskSocketAsyncEventArgs = Interlocked.Exchange<Socket.TaskSocketAsyncEventArgs<Socket>>(ref LazyInitializer.EnsureInitialized<Socket.CachedEventArgs>(ref this._cachedTaskEventArgs, () => new Socket.CachedEventArgs()).TaskAccept, Socket.s_rentedSocketSentinel);
			if (taskSocketAsyncEventArgs == Socket.s_rentedSocketSentinel)
			{
				return this.AcceptAsyncApm(acceptSocket);
			}
			if (taskSocketAsyncEventArgs == null)
			{
				taskSocketAsyncEventArgs = new Socket.TaskSocketAsyncEventArgs<Socket>();
				taskSocketAsyncEventArgs.Completed += Socket.AcceptCompletedHandler;
			}
			taskSocketAsyncEventArgs.AcceptSocket = acceptSocket;
			Task<Socket> result;
			if (this.AcceptAsync(taskSocketAsyncEventArgs))
			{
				bool flag;
				result = taskSocketAsyncEventArgs.GetCompletionResponsibility(out flag).Task;
				if (flag)
				{
					this.ReturnSocketAsyncEventArgs(taskSocketAsyncEventArgs);
				}
			}
			else
			{
				result = ((taskSocketAsyncEventArgs.SocketError == SocketError.Success) ? Task.FromResult<Socket>(taskSocketAsyncEventArgs.AcceptSocket) : Task.FromException<Socket>(Socket.GetException(taskSocketAsyncEventArgs.SocketError, false)));
				this.ReturnSocketAsyncEventArgs(taskSocketAsyncEventArgs);
			}
			return result;
		}

		private Task<Socket> AcceptAsyncApm(Socket acceptSocket)
		{
			TaskCompletionSource<Socket> taskCompletionSource = new TaskCompletionSource<Socket>(this);
			this.BeginAccept(acceptSocket, 0, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<Socket> taskCompletionSource2 = (TaskCompletionSource<Socket>)iar.AsyncState;
				try
				{
					taskCompletionSource2.TrySetResult(((Socket)taskCompletionSource2.Task.AsyncState).EndAccept(iar));
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task ConnectAsync(EndPoint remoteEP)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(this);
			this.BeginConnect(remoteEP, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<bool> taskCompletionSource2 = (TaskCompletionSource<bool>)iar.AsyncState;
				try
				{
					((Socket)taskCompletionSource2.Task.AsyncState).EndConnect(iar);
					taskCompletionSource2.TrySetResult(true);
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task ConnectAsync(IPAddress address, int port)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(this);
			this.BeginConnect(address, port, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<bool> taskCompletionSource2 = (TaskCompletionSource<bool>)iar.AsyncState;
				try
				{
					((Socket)taskCompletionSource2.Task.AsyncState).EndConnect(iar);
					taskCompletionSource2.TrySetResult(true);
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task ConnectAsync(IPAddress[] addresses, int port)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(this);
			this.BeginConnect(addresses, port, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<bool> taskCompletionSource2 = (TaskCompletionSource<bool>)iar.AsyncState;
				try
				{
					((Socket)taskCompletionSource2.Task.AsyncState).EndConnect(iar);
					taskCompletionSource2.TrySetResult(true);
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task ConnectAsync(string host, int port)
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>(this);
			this.BeginConnect(host, port, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<bool> taskCompletionSource2 = (TaskCompletionSource<bool>)iar.AsyncState;
				try
				{
					((Socket)taskCompletionSource2.Task.AsyncState).EndConnect(iar);
					taskCompletionSource2.TrySetResult(true);
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task<int> ReceiveAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, bool fromNetworkStream)
		{
			Socket.ValidateBuffer(buffer);
			return this.ReceiveAsync(buffer, socketFlags, fromNetworkStream, default(CancellationToken)).AsTask();
		}

		internal ValueTask<int> ReceiveAsync(Memory<byte> buffer, SocketFlags socketFlags, bool fromNetworkStream, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			Socket.AwaitableSocketAsyncEventArgs awaitableSocketAsyncEventArgs = LazyInitializer.EnsureInitialized<Socket.AwaitableSocketAsyncEventArgs>(ref LazyInitializer.EnsureInitialized<Socket.CachedEventArgs>(ref this._cachedTaskEventArgs, () => new Socket.CachedEventArgs()).ValueTaskReceive, () => new Socket.AwaitableSocketAsyncEventArgs());
			if (awaitableSocketAsyncEventArgs.Reserve())
			{
				awaitableSocketAsyncEventArgs.SetBuffer(buffer);
				awaitableSocketAsyncEventArgs.SocketFlags = socketFlags;
				awaitableSocketAsyncEventArgs.WrapExceptionsInIOExceptions = fromNetworkStream;
				return awaitableSocketAsyncEventArgs.ReceiveAsync(this);
			}
			return new ValueTask<int>(this.ReceiveAsyncApm(buffer, socketFlags));
		}

		private Task<int> ReceiveAsyncApm(Memory<byte> buffer, SocketFlags socketFlags)
		{
			ArraySegment<byte> arraySegment;
			if (MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment))
			{
				TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(this);
				this.BeginReceive(arraySegment.Array, arraySegment.Offset, arraySegment.Count, socketFlags, delegate(IAsyncResult iar)
				{
					TaskCompletionSource<int> taskCompletionSource3 = (TaskCompletionSource<int>)iar.AsyncState;
					try
					{
						taskCompletionSource3.TrySetResult(((Socket)taskCompletionSource3.Task.AsyncState).EndReceive(iar));
					}
					catch (Exception exception)
					{
						taskCompletionSource3.TrySetException(exception);
					}
				}, taskCompletionSource);
				return taskCompletionSource.Task;
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			TaskCompletionSource<int> taskCompletionSource2 = new TaskCompletionSource<int>(this);
			this.BeginReceive(array, 0, buffer.Length, socketFlags, delegate(IAsyncResult iar)
			{
				Tuple<TaskCompletionSource<int>, Memory<byte>, byte[]> tuple = (Tuple<TaskCompletionSource<int>, Memory<byte>, byte[]>)iar.AsyncState;
				try
				{
					int num = ((Socket)tuple.Item1.Task.AsyncState).EndReceive(iar);
					new ReadOnlyMemory<byte>(tuple.Item3, 0, num).Span.CopyTo(tuple.Item2.Span);
					tuple.Item1.TrySetResult(num);
				}
				catch (Exception exception)
				{
					tuple.Item1.TrySetException(exception);
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(tuple.Item3, false);
				}
			}, Tuple.Create<TaskCompletionSource<int>, Memory<byte>, byte[]>(taskCompletionSource2, buffer, array));
			return taskCompletionSource2.Task;
		}

		internal Task<int> ReceiveAsync(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			Socket.ValidateBuffersList(buffers);
			Socket.Int32TaskSocketAsyncEventArgs int32TaskSocketAsyncEventArgs = this.RentSocketAsyncEventArgs(true);
			if (int32TaskSocketAsyncEventArgs != null)
			{
				Socket.ConfigureBufferList(int32TaskSocketAsyncEventArgs, buffers, socketFlags);
				return this.GetTaskForSendReceive(this.ReceiveAsync(int32TaskSocketAsyncEventArgs), int32TaskSocketAsyncEventArgs, false, true);
			}
			return this.ReceiveAsyncApm(buffers, socketFlags);
		}

		private Task<int> ReceiveAsyncApm(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(this);
			this.BeginReceive(buffers, socketFlags, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<int> taskCompletionSource2 = (TaskCompletionSource<int>)iar.AsyncState;
				try
				{
					taskCompletionSource2.TrySetResult(((Socket)taskCompletionSource2.Task.AsyncState).EndReceive(iar));
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task<SocketReceiveFromResult> ReceiveFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint)
		{
			Socket.StateTaskCompletionSource<EndPoint, SocketReceiveFromResult> stateTaskCompletionSource = new Socket.StateTaskCompletionSource<EndPoint, SocketReceiveFromResult>(this)
			{
				_field1 = remoteEndPoint
			};
			this.BeginReceiveFrom(buffer.Array, buffer.Offset, buffer.Count, socketFlags, ref stateTaskCompletionSource._field1, delegate(IAsyncResult iar)
			{
				Socket.StateTaskCompletionSource<EndPoint, SocketReceiveFromResult> stateTaskCompletionSource2 = (Socket.StateTaskCompletionSource<EndPoint, SocketReceiveFromResult>)iar.AsyncState;
				try
				{
					int receivedBytes = ((Socket)stateTaskCompletionSource2.Task.AsyncState).EndReceiveFrom(iar, ref stateTaskCompletionSource2._field1);
					stateTaskCompletionSource2.TrySetResult(new SocketReceiveFromResult
					{
						ReceivedBytes = receivedBytes,
						RemoteEndPoint = stateTaskCompletionSource2._field1
					});
				}
				catch (Exception exception)
				{
					stateTaskCompletionSource2.TrySetException(exception);
				}
			}, stateTaskCompletionSource);
			return stateTaskCompletionSource.Task;
		}

		internal Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint)
		{
			Socket.StateTaskCompletionSource<SocketFlags, EndPoint, SocketReceiveMessageFromResult> stateTaskCompletionSource = new Socket.StateTaskCompletionSource<SocketFlags, EndPoint, SocketReceiveMessageFromResult>(this)
			{
				_field1 = socketFlags,
				_field2 = remoteEndPoint
			};
			this.BeginReceiveMessageFrom(buffer.Array, buffer.Offset, buffer.Count, socketFlags, ref stateTaskCompletionSource._field2, delegate(IAsyncResult iar)
			{
				Socket.StateTaskCompletionSource<SocketFlags, EndPoint, SocketReceiveMessageFromResult> stateTaskCompletionSource2 = (Socket.StateTaskCompletionSource<SocketFlags, EndPoint, SocketReceiveMessageFromResult>)iar.AsyncState;
				try
				{
					IPPacketInformation packetInformation;
					int receivedBytes = ((Socket)stateTaskCompletionSource2.Task.AsyncState).EndReceiveMessageFrom(iar, ref stateTaskCompletionSource2._field1, ref stateTaskCompletionSource2._field2, out packetInformation);
					stateTaskCompletionSource2.TrySetResult(new SocketReceiveMessageFromResult
					{
						ReceivedBytes = receivedBytes,
						RemoteEndPoint = stateTaskCompletionSource2._field2,
						SocketFlags = stateTaskCompletionSource2._field1,
						PacketInformation = packetInformation
					});
				}
				catch (Exception exception)
				{
					stateTaskCompletionSource2.TrySetException(exception);
				}
			}, stateTaskCompletionSource);
			return stateTaskCompletionSource.Task;
		}

		internal Task<int> SendAsync(ArraySegment<byte> buffer, SocketFlags socketFlags)
		{
			Socket.ValidateBuffer(buffer);
			return this.SendAsync(buffer, socketFlags, default(CancellationToken)).AsTask();
		}

		internal ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			Socket.AwaitableSocketAsyncEventArgs awaitableSocketAsyncEventArgs = LazyInitializer.EnsureInitialized<Socket.AwaitableSocketAsyncEventArgs>(ref LazyInitializer.EnsureInitialized<Socket.CachedEventArgs>(ref this._cachedTaskEventArgs, () => new Socket.CachedEventArgs()).ValueTaskSend, () => new Socket.AwaitableSocketAsyncEventArgs());
			if (awaitableSocketAsyncEventArgs.Reserve())
			{
				awaitableSocketAsyncEventArgs.SetBuffer(MemoryMarshal.AsMemory<byte>(buffer));
				awaitableSocketAsyncEventArgs.SocketFlags = socketFlags;
				awaitableSocketAsyncEventArgs.WrapExceptionsInIOExceptions = false;
				return awaitableSocketAsyncEventArgs.SendAsync(this);
			}
			return new ValueTask<int>(this.SendAsyncApm(buffer, socketFlags));
		}

		internal ValueTask SendAsyncForNetworkStream(ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask(Task.FromCanceled(cancellationToken));
			}
			Socket.AwaitableSocketAsyncEventArgs awaitableSocketAsyncEventArgs = LazyInitializer.EnsureInitialized<Socket.AwaitableSocketAsyncEventArgs>(ref LazyInitializer.EnsureInitialized<Socket.CachedEventArgs>(ref this._cachedTaskEventArgs, () => new Socket.CachedEventArgs()).ValueTaskSend, () => new Socket.AwaitableSocketAsyncEventArgs());
			if (awaitableSocketAsyncEventArgs.Reserve())
			{
				awaitableSocketAsyncEventArgs.SetBuffer(MemoryMarshal.AsMemory<byte>(buffer));
				awaitableSocketAsyncEventArgs.SocketFlags = socketFlags;
				awaitableSocketAsyncEventArgs.WrapExceptionsInIOExceptions = true;
				return awaitableSocketAsyncEventArgs.SendAsyncForNetworkStream(this);
			}
			return new ValueTask(this.SendAsyncApm(buffer, socketFlags));
		}

		private Task<int> SendAsyncApm(ReadOnlyMemory<byte> buffer, SocketFlags socketFlags)
		{
			ArraySegment<byte> arraySegment;
			if (MemoryMarshal.TryGetArray<byte>(buffer, out arraySegment))
			{
				TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(this);
				this.BeginSend(arraySegment.Array, arraySegment.Offset, arraySegment.Count, socketFlags, delegate(IAsyncResult iar)
				{
					TaskCompletionSource<int> taskCompletionSource3 = (TaskCompletionSource<int>)iar.AsyncState;
					try
					{
						taskCompletionSource3.TrySetResult(((Socket)taskCompletionSource3.Task.AsyncState).EndSend(iar));
					}
					catch (Exception exception)
					{
						taskCompletionSource3.TrySetException(exception);
					}
				}, taskCompletionSource);
				return taskCompletionSource.Task;
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(buffer.Length);
			buffer.Span.CopyTo(array);
			TaskCompletionSource<int> taskCompletionSource2 = new TaskCompletionSource<int>(this);
			this.BeginSend(array, 0, buffer.Length, socketFlags, delegate(IAsyncResult iar)
			{
				Tuple<TaskCompletionSource<int>, byte[]> tuple = (Tuple<TaskCompletionSource<int>, byte[]>)iar.AsyncState;
				try
				{
					tuple.Item1.TrySetResult(((Socket)tuple.Item1.Task.AsyncState).EndSend(iar));
				}
				catch (Exception exception)
				{
					tuple.Item1.TrySetException(exception);
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(tuple.Item2, false);
				}
			}, Tuple.Create<TaskCompletionSource<int>, byte[]>(taskCompletionSource2, array));
			return taskCompletionSource2.Task;
		}

		internal Task<int> SendAsync(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			Socket.ValidateBuffersList(buffers);
			Socket.Int32TaskSocketAsyncEventArgs int32TaskSocketAsyncEventArgs = this.RentSocketAsyncEventArgs(false);
			if (int32TaskSocketAsyncEventArgs != null)
			{
				Socket.ConfigureBufferList(int32TaskSocketAsyncEventArgs, buffers, socketFlags);
				return this.GetTaskForSendReceive(this.SendAsync(int32TaskSocketAsyncEventArgs), int32TaskSocketAsyncEventArgs, false, false);
			}
			return this.SendAsyncApm(buffers, socketFlags);
		}

		private Task<int> SendAsyncApm(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(this);
			this.BeginSend(buffers, socketFlags, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<int> taskCompletionSource2 = (TaskCompletionSource<int>)iar.AsyncState;
				try
				{
					taskCompletionSource2.TrySetResult(((Socket)taskCompletionSource2.Task.AsyncState).EndSend(iar));
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		internal Task<int> SendToAsync(ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(this);
			this.BeginSendTo(buffer.Array, buffer.Offset, buffer.Count, socketFlags, remoteEP, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<int> taskCompletionSource2 = (TaskCompletionSource<int>)iar.AsyncState;
				try
				{
					taskCompletionSource2.TrySetResult(((Socket)taskCompletionSource2.Task.AsyncState).EndSendTo(iar));
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			return taskCompletionSource.Task;
		}

		private static void ValidateBuffer(ArraySegment<byte> buffer)
		{
			if (buffer.Array == null)
			{
				throw new ArgumentNullException("Array");
			}
			if (buffer.Offset < 0 || buffer.Offset > buffer.Array.Length)
			{
				throw new ArgumentOutOfRangeException("Offset");
			}
			if (buffer.Count < 0 || buffer.Count > buffer.Array.Length - buffer.Offset)
			{
				throw new ArgumentOutOfRangeException("Count");
			}
		}

		private static void ValidateBuffersList(IList<ArraySegment<byte>> buffers)
		{
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (buffers.Count == 0)
			{
				throw new ArgumentException(SR.Format("The parameter {0} must contain one or more elements.", "buffers"), "buffers");
			}
		}

		private static void ConfigureBufferList(Socket.Int32TaskSocketAsyncEventArgs saea, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			if (!saea.MemoryBuffer.Equals(default(Memory<byte>)))
			{
				saea.SetBuffer(default(Memory<byte>));
			}
			saea.BufferList = buffers;
			saea.SocketFlags = socketFlags;
		}

		private Task<int> GetTaskForSendReceive(bool pending, Socket.Int32TaskSocketAsyncEventArgs saea, bool fromNetworkStream, bool isReceive)
		{
			Task<int> result;
			if (pending)
			{
				bool flag;
				result = saea.GetCompletionResponsibility(out flag).Task;
				if (flag)
				{
					this.ReturnSocketAsyncEventArgs(saea, isReceive);
				}
			}
			else
			{
				if (saea.SocketError == SocketError.Success)
				{
					int bytesTransferred = saea.BytesTransferred;
					if (bytesTransferred == 0 || (fromNetworkStream & !isReceive))
					{
						result = Socket.s_zeroTask;
					}
					else
					{
						result = Task.FromResult<int>(bytesTransferred);
					}
				}
				else
				{
					result = Task.FromException<int>(Socket.GetException(saea.SocketError, fromNetworkStream));
				}
				this.ReturnSocketAsyncEventArgs(saea, isReceive);
			}
			return result;
		}

		private static void CompleteAccept(Socket s, Socket.TaskSocketAsyncEventArgs<Socket> saea)
		{
			SocketError socketError = saea.SocketError;
			Socket acceptSocket = saea.AcceptSocket;
			bool flag;
			AsyncTaskMethodBuilder<Socket> completionResponsibility = saea.GetCompletionResponsibility(out flag);
			if (flag)
			{
				s.ReturnSocketAsyncEventArgs(saea);
			}
			if (socketError == SocketError.Success)
			{
				completionResponsibility.SetResult(acceptSocket);
				return;
			}
			completionResponsibility.SetException(Socket.GetException(socketError, false));
		}

		private static void CompleteSendReceive(Socket s, Socket.Int32TaskSocketAsyncEventArgs saea, bool isReceive)
		{
			SocketError socketError = saea.SocketError;
			int bytesTransferred = saea.BytesTransferred;
			bool wrapExceptionsInIOExceptions = saea._wrapExceptionsInIOExceptions;
			bool flag;
			AsyncTaskMethodBuilder<int> completionResponsibility = saea.GetCompletionResponsibility(out flag);
			if (flag)
			{
				s.ReturnSocketAsyncEventArgs(saea, isReceive);
			}
			if (socketError == SocketError.Success)
			{
				completionResponsibility.SetResult(bytesTransferred);
				return;
			}
			completionResponsibility.SetException(Socket.GetException(socketError, wrapExceptionsInIOExceptions));
		}

		private static Exception GetException(SocketError error, bool wrapExceptionsInIOExceptions = false)
		{
			Exception ex = new SocketException((int)error);
			if (!wrapExceptionsInIOExceptions)
			{
				return ex;
			}
			return new IOException(SR.Format("Unable to transfer data on the transport connection: {0}.", ex.Message), ex);
		}

		private Socket.Int32TaskSocketAsyncEventArgs RentSocketAsyncEventArgs(bool isReceive)
		{
			Socket.CachedEventArgs cachedEventArgs = LazyInitializer.EnsureInitialized<Socket.CachedEventArgs>(ref this._cachedTaskEventArgs, () => new Socket.CachedEventArgs());
			Socket.Int32TaskSocketAsyncEventArgs int32TaskSocketAsyncEventArgs = isReceive ? Interlocked.Exchange<Socket.Int32TaskSocketAsyncEventArgs>(ref cachedEventArgs.TaskReceive, Socket.s_rentedInt32Sentinel) : Interlocked.Exchange<Socket.Int32TaskSocketAsyncEventArgs>(ref cachedEventArgs.TaskSend, Socket.s_rentedInt32Sentinel);
			if (int32TaskSocketAsyncEventArgs == Socket.s_rentedInt32Sentinel)
			{
				return null;
			}
			if (int32TaskSocketAsyncEventArgs == null)
			{
				int32TaskSocketAsyncEventArgs = new Socket.Int32TaskSocketAsyncEventArgs();
				int32TaskSocketAsyncEventArgs.Completed += (isReceive ? Socket.ReceiveCompletedHandler : Socket.SendCompletedHandler);
			}
			return int32TaskSocketAsyncEventArgs;
		}

		private void ReturnSocketAsyncEventArgs(Socket.Int32TaskSocketAsyncEventArgs saea, bool isReceive)
		{
			saea._accessed = false;
			saea._builder = default(AsyncTaskMethodBuilder<int>);
			saea._wrapExceptionsInIOExceptions = false;
			if (isReceive)
			{
				Volatile.Write<Socket.Int32TaskSocketAsyncEventArgs>(ref this._cachedTaskEventArgs.TaskReceive, saea);
				return;
			}
			Volatile.Write<Socket.Int32TaskSocketAsyncEventArgs>(ref this._cachedTaskEventArgs.TaskSend, saea);
		}

		private void ReturnSocketAsyncEventArgs(Socket.TaskSocketAsyncEventArgs<Socket> saea)
		{
			saea.AcceptSocket = null;
			saea._accessed = false;
			saea._builder = default(AsyncTaskMethodBuilder<Socket>);
			Volatile.Write<Socket.TaskSocketAsyncEventArgs<Socket>>(ref this._cachedTaskEventArgs.TaskAccept, saea);
		}

		private void DisposeCachedTaskSocketAsyncEventArgs()
		{
			Socket.CachedEventArgs cachedTaskEventArgs = this._cachedTaskEventArgs;
			if (cachedTaskEventArgs != null)
			{
				Socket.TaskSocketAsyncEventArgs<Socket> taskSocketAsyncEventArgs = Interlocked.Exchange<Socket.TaskSocketAsyncEventArgs<Socket>>(ref cachedTaskEventArgs.TaskAccept, Socket.s_rentedSocketSentinel);
				if (taskSocketAsyncEventArgs != null)
				{
					taskSocketAsyncEventArgs.Dispose();
				}
				Socket.Int32TaskSocketAsyncEventArgs int32TaskSocketAsyncEventArgs = Interlocked.Exchange<Socket.Int32TaskSocketAsyncEventArgs>(ref cachedTaskEventArgs.TaskReceive, Socket.s_rentedInt32Sentinel);
				if (int32TaskSocketAsyncEventArgs != null)
				{
					int32TaskSocketAsyncEventArgs.Dispose();
				}
				Socket.Int32TaskSocketAsyncEventArgs int32TaskSocketAsyncEventArgs2 = Interlocked.Exchange<Socket.Int32TaskSocketAsyncEventArgs>(ref cachedTaskEventArgs.TaskSend, Socket.s_rentedInt32Sentinel);
				if (int32TaskSocketAsyncEventArgs2 != null)
				{
					int32TaskSocketAsyncEventArgs2.Dispose();
				}
				Socket.AwaitableSocketAsyncEventArgs awaitableSocketAsyncEventArgs = Interlocked.Exchange<Socket.AwaitableSocketAsyncEventArgs>(ref cachedTaskEventArgs.ValueTaskReceive, Socket.AwaitableSocketAsyncEventArgs.Reserved);
				if (awaitableSocketAsyncEventArgs != null)
				{
					awaitableSocketAsyncEventArgs.Dispose();
				}
				Socket.AwaitableSocketAsyncEventArgs awaitableSocketAsyncEventArgs2 = Interlocked.Exchange<Socket.AwaitableSocketAsyncEventArgs>(ref cachedTaskEventArgs.ValueTaskSend, Socket.AwaitableSocketAsyncEventArgs.Reserved);
				if (awaitableSocketAsyncEventArgs2 == null)
				{
					return;
				}
				awaitableSocketAsyncEventArgs2.Dispose();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.Socket" /> class using the specified socket type and protocol.</summary>
		/// <param name="socketType">One of the <see cref="T:System.Net.Sockets.SocketType" /> values.</param>
		/// <param name="protocolType">One of the <see cref="T:System.Net.Sockets.ProtocolType" /> values.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">The combination of  <paramref name="socketType" /> and <paramref name="protocolType" /> results in an invalid socket.</exception>
		public Socket(SocketType socketType, ProtocolType protocolType) : this(AddressFamily.InterNetworkV6, socketType, protocolType)
		{
			this.DualMode = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.Socket" /> class using the specified address family, socket type and protocol.</summary>
		/// <param name="addressFamily">One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values.</param>
		/// <param name="socketType">One of the <see cref="T:System.Net.Sockets.SocketType" /> values.</param>
		/// <param name="protocolType">One of the <see cref="T:System.Net.Sockets.ProtocolType" /> values.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">The combination of <paramref name="addressFamily" />, <paramref name="socketType" />, and <paramref name="protocolType" /> results in an invalid socket.</exception>
		public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
		{
			this.ReadSem = new SemaphoreSlim(1, 1);
			this.WriteSem = new SemaphoreSlim(1, 1);
			this.is_blocking = true;
			base..ctor();
			Socket.s_LoggingEnabled = Logging.On;
			bool flag = Socket.s_LoggingEnabled;
			Socket.InitializeSockets();
			int num;
			this.m_Handle = new SafeSocketHandle(Socket.Socket_icall(addressFamily, socketType, protocolType, out num), true);
			if (this.m_Handle.IsInvalid)
			{
				throw new SocketException();
			}
			this.addressFamily = addressFamily;
			this.socketType = socketType;
			this.protocolType = protocolType;
			IPProtectionLevel ipprotectionLevel = SettingsSectionInternal.Section.IPProtectionLevel;
			if (ipprotectionLevel != IPProtectionLevel.Unspecified)
			{
				this.SetIPProtectionLevel(ipprotectionLevel);
			}
			this.SocketDefaults();
			bool flag2 = Socket.s_LoggingEnabled;
		}

		/// <summary>Gets a value indicating whether IPv4 support is available and enabled on the current host.</summary>
		/// <returns>
		///   <see langword="true" /> if the current host supports the IPv4 protocol; otherwise, <see langword="false" />.</returns>
		[Obsolete("SupportsIPv4 is obsoleted for this type, please use OSSupportsIPv4 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv4
		{
			get
			{
				Socket.InitializeSockets();
				return Socket.s_SupportsIPv4;
			}
		}

		/// <summary>Indicates whether the underlying operating system and network adaptors support Internet Protocol version 4 (IPv4).</summary>
		/// <returns>
		///   <see langword="true" /> if the operating system and network adaptors support the IPv4 protocol; otherwise, <see langword="false" />.</returns>
		public static bool OSSupportsIPv4
		{
			get
			{
				Socket.InitializeSockets();
				return Socket.s_SupportsIPv4;
			}
		}

		/// <summary>Gets a value that indicates whether the Framework supports IPv6 for certain obsolete <see cref="T:System.Net.Dns" /> members.</summary>
		/// <returns>
		///   <see langword="true" /> if the Framework supports IPv6 for certain obsolete <see cref="T:System.Net.Dns" /> methods; otherwise, <see langword="false" />.</returns>
		[Obsolete("SupportsIPv6 is obsoleted for this type, please use OSSupportsIPv6 instead. http://go.microsoft.com/fwlink/?linkid=14202")]
		public static bool SupportsIPv6
		{
			get
			{
				Socket.InitializeSockets();
				return Socket.s_SupportsIPv6;
			}
		}

		internal static bool LegacySupportsIPv6
		{
			get
			{
				Socket.InitializeSockets();
				return Socket.s_SupportsIPv6;
			}
		}

		/// <summary>Indicates whether the underlying operating system and network adaptors support Internet Protocol version 6 (IPv6).</summary>
		/// <returns>
		///   <see langword="true" /> if the operating system and network adaptors support the IPv6 protocol; otherwise, <see langword="false" />.</returns>
		public static bool OSSupportsIPv6
		{
			get
			{
				Socket.InitializeSockets();
				return Socket.s_OSSupportsIPv6;
			}
		}

		/// <summary>Gets the operating system handle for the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>An <see cref="T:System.IntPtr" /> that represents the operating system handle for the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		public IntPtr Handle
		{
			get
			{
				return this.m_Handle.DangerousGetHandle();
			}
		}

		/// <summary>Specifies whether the socket should only use Overlapped I/O mode.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> uses only overlapped I/O; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The socket has been bound to a completion port.</exception>
		public bool UseOnlyOverlappedIO
		{
			get
			{
				return this.useOverlappedIO;
			}
			set
			{
				this.useOverlappedIO = value;
			}
		}

		/// <summary>Gets the address family of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values.</returns>
		public AddressFamily AddressFamily
		{
			get
			{
				return this.addressFamily;
			}
		}

		/// <summary>Gets the type of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>One of the <see cref="T:System.Net.Sockets.SocketType" /> values.</returns>
		public SocketType SocketType
		{
			get
			{
				return this.socketType;
			}
		}

		/// <summary>Gets the protocol type of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>One of the <see cref="T:System.Net.Sockets.ProtocolType" /> values.</returns>
		public ProtocolType ProtocolType
		{
			get
			{
				return this.protocolType;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the <see cref="T:System.Net.Sockets.Socket" /> allows only one process to bind to a port.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> allows only one socket to bind to a specific port; otherwise, <see langword="false" />. The default is <see langword="true" /> for Windows Server 2003 and Windows XP Service Pack 2, and <see langword="false" /> for all other versions.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> has been called for this <see cref="T:System.Net.Sockets.Socket" />.</exception>
		public bool ExclusiveAddressUse
		{
			get
			{
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse) != 0;
			}
			set
			{
				if (this.IsBound)
				{
					throw new InvalidOperationException(SR.GetString("The socket must not be bound or connected."));
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, value ? 1 : 0);
			}
		}

		/// <summary>Gets or sets a value that specifies the size of the receive buffer of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the size, in bytes, of the receive buffer. The default is 8192.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than 0.</exception>
		public int ReceiveBufferSize
		{
			get
			{
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		/// <summary>Gets or sets a value that specifies the size of the send buffer of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the size, in bytes, of the send buffer. The default is 8192.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than 0.</exception>
		public int SendBufferSize
		{
			get
			{
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		/// <summary>Gets or sets a value that specifies the amount of time after which a synchronous <see cref="Overload:System.Net.Sockets.Socket.Receive" /> call will time out.</summary>
		/// <returns>The time-out value, in milliseconds. The default value is 0, which indicates an infinite time-out period. Specifying -1 also indicates an infinite time-out period.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than -1.</exception>
		public int ReceiveTimeout
		{
			get
			{
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value == -1)
				{
					value = 0;
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		/// <summary>Gets or sets a value that specifies the amount of time after which a synchronous <see cref="Overload:System.Net.Sockets.Socket.Send" /> call will time out.</summary>
		/// <returns>The time-out value, in milliseconds. If you set the property with a value between 1 and 499, the value will be changed to 500. The default value is 0, which indicates an infinite time-out period. Specifying -1 also indicates an infinite time-out period.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value specified for a set operation is less than -1.</exception>
		public int SendTimeout
		{
			get
			{
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (value == -1)
				{
					value = 0;
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		/// <summary>Gets or sets a value that specifies whether the <see cref="T:System.Net.Sockets.Socket" /> will delay closing a socket in an attempt to send all pending data.</summary>
		/// <returns>A <see cref="T:System.Net.Sockets.LingerOption" /> that specifies how to linger while closing a socket.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public LingerOption LingerState
		{
			get
			{
				return (LingerOption)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		/// <summary>Gets or sets a value that specifies the Time To Live (TTL) value of Internet Protocol (IP) packets sent by the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <returns>The TTL value.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The TTL value can't be set to a negative number.</exception>
		/// <exception cref="T:System.NotSupportedException">This property can be set only for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. This error is also returned when an attempt was made to set TTL to a value higher than 255.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public short Ttl
		{
			get
			{
				if (this.addressFamily == AddressFamily.InterNetwork)
				{
					return (short)((int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress));
				}
				if (this.addressFamily == AddressFamily.InterNetworkV6)
				{
					return (short)((int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.ReuseAddress));
				}
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				if (this.addressFamily == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, (int)value);
					return;
				}
				if (this.addressFamily == AddressFamily.InterNetworkV6)
				{
					this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.ReuseAddress, (int)value);
					return;
				}
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the <see cref="T:System.Net.Sockets.Socket" /> allows Internet Protocol (IP) datagrams to be fragmented.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> allows datagram fragmentation; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		/// <exception cref="T:System.NotSupportedException">This property can be set only for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool DontFragment
		{
			get
			{
				if (this.addressFamily == AddressFamily.InterNetwork)
				{
					return (int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment) != 0;
				}
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
			set
			{
				if (this.addressFamily == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, value ? 1 : 0);
					return;
				}
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the <see cref="T:System.Net.Sockets.Socket" /> is a dual-mode socket used for both IPv4 and IPv6.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> is a  dual-mode socket; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool DualMode
		{
			get
			{
				if (this.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
				}
				return (int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only) == 0;
			}
			set
			{
				if (this.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
				}
				this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, value ? 0 : 1);
			}
		}

		private bool IsDualMode
		{
			get
			{
				return this.AddressFamily == AddressFamily.InterNetworkV6 && this.DualMode;
			}
		}

		internal bool CanTryAddressFamily(AddressFamily family)
		{
			return family == this.addressFamily || (family == AddressFamily.InterNetwork && this.IsDualMode);
		}

		/// <summary>Establishes a connection to a remote host. The host is specified by an array of IP addresses and a port number.</summary>
		/// <param name="addresses">The IP addresses of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="addresses" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="address" /> is zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />.</exception>
		public void Connect(IPAddress[] addresses, int port)
		{
			bool flag = Socket.s_LoggingEnabled;
			if (this.CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (addresses.Length == 0)
			{
				throw new ArgumentException(SR.GetString("The number of specified IP addresses has to be greater than 0."), "addresses");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			if (this.addressFamily != AddressFamily.InterNetwork && this.addressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
			Exception ex = null;
			foreach (IPAddress ipaddress in addresses)
			{
				if (this.CanTryAddressFamily(ipaddress.AddressFamily))
				{
					try
					{
						this.Connect(new IPEndPoint(ipaddress, port));
						ex = null;
						break;
					}
					catch (Exception ex2)
					{
						if (NclUtilities.IsFatal(ex2))
						{
							throw;
						}
						ex = ex2;
					}
				}
			}
			if (ex != null)
			{
				throw ex;
			}
			if (!this.Connected)
			{
				throw new ArgumentException(SR.GetString("None of the discovered or specified addresses match the socket address family."), "addresses");
			}
			bool flag2 = Socket.s_LoggingEnabled;
		}

		/// <summary>Sends the specified number of bytes of data to a connected <see cref="T:System.Net.Sockets.Socket" />, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="size" /> is less than 0 or exceeds the size of the buffer.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// An operating system error occurs while accessing the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.Send(buffer, 0, size, socketFlags);
		}

		/// <summary>Sends data to a connected <see cref="T:System.Net.Sockets.Socket" /> using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(byte[] buffer, SocketFlags socketFlags)
		{
			return this.Send(buffer, 0, (buffer != null) ? buffer.Length : 0, socketFlags);
		}

		/// <summary>Sends data to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(byte[] buffer)
		{
			return this.Send(buffer, 0, (buffer != null) ? buffer.Length : 0, SocketFlags.None);
		}

		/// <summary>Sends the set of buffers in the list to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffers" /> is empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(IList<ArraySegment<byte>> buffers)
		{
			return this.Send(buffers, SocketFlags.None);
		}

		/// <summary>Sends the set of buffers in the list to a connected <see cref="T:System.Net.Sockets.Socket" />, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffers" /> is empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError socketError;
			int result = this.Send(buffers, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Sends the file <paramref name="fileName" /> to a connected <see cref="T:System.Net.Sockets.Socket" /> object with the <see cref="F:System.Net.Sockets.TransmitFileOptions.UseDefaultWorkerThread" /> transmit flag.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> that contains the path and name of the file to be sent. This parameter can be <see langword="null" />.</param>
		/// <exception cref="T:System.NotSupportedException">The socket is not connected to a remote host.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> object is not in blocking mode and cannot accept this synchronous call.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file <paramref name="fileName" /> was not found.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public void SendFile(string fileName)
		{
			this.SendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread);
		}

		/// <summary>Sends the specified number of bytes of data to a connected <see cref="T:System.Net.Sockets.Socket" />, starting at the specified offset, and using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="offset">The position in the data buffer at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			SocketError socketError;
			int result = this.Send(buffer, offset, size, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Sends the specified number of bytes of data to the specified endpoint using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">The <see cref="T:System.Net.EndPoint" /> that represents the destination location for the data.</param>
		/// <returns>The number of bytes sent.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The specified <paramref name="size" /> exceeds the size of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int SendTo(byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.SendTo(buffer, 0, size, socketFlags, remoteEP);
		}

		/// <summary>Sends data to a specific endpoint using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">The <see cref="T:System.Net.EndPoint" /> that represents the destination location for the data.</param>
		/// <returns>The number of bytes sent.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int SendTo(byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return this.SendTo(buffer, 0, (buffer != null) ? buffer.Length : 0, socketFlags, remoteEP);
		}

		/// <summary>Sends data to the specified endpoint.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="remoteEP">The <see cref="T:System.Net.EndPoint" /> that represents the destination for the data.</param>
		/// <returns>The number of bytes sent.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int SendTo(byte[] buffer, EndPoint remoteEP)
		{
			return this.SendTo(buffer, 0, (buffer != null) ? buffer.Length : 0, SocketFlags.None, remoteEP);
		}

		/// <summary>Receives the specified number of bytes of data from a bound <see cref="T:System.Net.Sockets.Socket" /> into a receive buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="size" /> exceeds the size of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
		{
			return this.Receive(buffer, 0, size, socketFlags);
		}

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into a receive buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int Receive(byte[] buffer, SocketFlags socketFlags)
		{
			return this.Receive(buffer, 0, (buffer != null) ? buffer.Length : 0, socketFlags);
		}

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into a receive buffer.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int Receive(byte[] buffer)
		{
			return this.Receive(buffer, 0, (buffer != null) ? buffer.Length : 0, SocketFlags.None);
		}

		/// <summary>Receives the specified number of bytes from a bound <see cref="T:System.Net.Sockets.Socket" /> into the specified offset position of the receive buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for received data.</param>
		/// <param name="offset">The location in <paramref name="buffer" /> to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// The <see cref="P:System.Net.Sockets.Socket.LocalEndPoint" /> property was not set.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags)
		{
			SocketError socketError;
			int result = this.Receive(buffer, offset, size, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into the list of receive buffers.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the received data.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred while attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Receive(IList<ArraySegment<byte>> buffers)
		{
			return this.Receive(buffers, SocketFlags.None);
		}

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into the list of receive buffers, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffers" />.Count is zero.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred while attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError socketError;
			int result = this.Receive(buffers, socketFlags, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Receives the specified number of bytes into the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" />, passed by reference, that represents the remote server.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// The <see cref="P:System.Net.Sockets.Socket.LocalEndPoint" /> property was not set.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.ReceiveFrom(buffer, 0, size, socketFlags, ref remoteEP);
		}

		/// <summary>Receives a datagram into the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" />, passed by reference, that represents the remote server.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			return this.ReceiveFrom(buffer, 0, (buffer != null) ? buffer.Length : 0, socketFlags, ref remoteEP);
		}

		/// <summary>Receives a datagram into the data buffer and stores the endpoint.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for received data.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" />, passed by reference, that represents the remote server.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
		{
			return this.ReceiveFrom(buffer, 0, (buffer != null) ? buffer.Length : 0, SocketFlags.None, ref remoteEP);
		}

		/// <summary>Sets low-level operating modes for the <see cref="T:System.Net.Sockets.Socket" /> using the <see cref="T:System.Net.Sockets.IOControlCode" /> enumeration to specify control codes.</summary>
		/// <param name="ioControlCode">A <see cref="T:System.Net.Sockets.IOControlCode" /> value that specifies the control code of the operation to perform.</param>
		/// <param name="optionInValue">An array of type <see cref="T:System.Byte" /> that contains the input data required by the operation.</param>
		/// <param name="optionOutValue">An array of type <see cref="T:System.Byte" /> that contains the output data returned by the operation.</param>
		/// <returns>The number of bytes in the <paramref name="optionOutValue" /> parameter.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to change the blocking mode without using the <see cref="P:System.Net.Sockets.Socket.Blocking" /> property.</exception>
		public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			return this.IOControl((int)ioControlCode, optionInValue, optionOutValue);
		}

		/// <summary>Set the IP protection level on a socket.</summary>
		/// <param name="level">The IP protection level to set on this socket.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="level" /> parameter cannot be <see cref="F:System.Net.Sockets.IPProtectionLevel.Unspecified" />. The IP protection level cannot be set to unspecified.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Net.Sockets.AddressFamily" /> of the socket must be either <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" />.</exception>
		public void SetIPProtectionLevel(IPProtectionLevel level)
		{
			if (level == IPProtectionLevel.Unspecified)
			{
				throw new ArgumentException(SR.GetString("The specified value is not valid."), "level");
			}
			if (this.addressFamily == AddressFamily.InterNetworkV6)
			{
				this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPProtectionLevel, (int)level);
				return;
			}
			if (this.addressFamily == AddressFamily.InterNetwork)
			{
				this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IPProtectionLevel, (int)level);
				return;
			}
			throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
		}

		/// <summary>Sends the file <paramref name="fileName" /> to a connected <see cref="T:System.Net.Sockets.Socket" /> object using the <see cref="F:System.Net.Sockets.TransmitFileOptions.UseDefaultWorkerThread" /> flag.</summary>
		/// <param name="fileName">A string that contains the path and name of the file to send. This parameter can be <see langword="null" />.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that represents the asynchronous send.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The socket is not connected to a remote host.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file <paramref name="fileName" /> was not found.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
		{
			return this.BeginSendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread, callback, state);
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The host is specified by an <see cref="T:System.Net.IPAddress" /> and a port number.</summary>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the connect operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Net.Sockets.Socket" /> is not in the socket family.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="address" /> is zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />, or an asynchronous operation is already in progress.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			bool flag = Socket.s_LoggingEnabled;
			if (this.CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!ValidationHelper.ValidateTcpPort(port))
			{
				throw new ArgumentOutOfRangeException("port");
			}
			if (!this.CanTryAddressFamily(address.AddressFamily))
			{
				throw new NotSupportedException(SR.GetString("This protocol version is not supported."));
			}
			IAsyncResult result = this.BeginConnect(new IPEndPoint(address, port), requestCallback, state);
			bool flag2 = Socket.s_LoggingEnabled;
			return result;
		}

		/// <summary>Sends data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to send.</param>
		/// <param name="offset">The zero-based position in the <paramref name="buffer" /> parameter at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous send.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is less than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			SocketError socketError;
			IAsyncResult result = this.BeginSend(buffer, offset, size, socketFlags, out socketError, callback, state);
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Sends data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffers">An array of type <see cref="T:System.Byte" /> that contains the data to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous send.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffers" /> is empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			SocketError socketError;
			IAsyncResult result = this.BeginSend(buffers, socketFlags, out socketError, callback, state);
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Ends a pending asynchronous send.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information for this asynchronous operation.</param>
		/// <returns>If successful, the number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />; otherwise, an invalid <see cref="T:System.Net.Sockets.Socket" /> error.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginSend(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndSend(System.IAsyncResult)" /> was previously called for the asynchronous send.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndSend(IAsyncResult asyncResult)
		{
			SocketError socketError;
			int result = this.EndSend(asyncResult, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Begins to asynchronously receive data from a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="offset">The zero-based position in the <paramref name="buffer" /> parameter at which to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the receive operation. This object is passed to the <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			SocketError socketError;
			IAsyncResult result = this.BeginReceive(buffer, offset, size, socketFlags, out socketError, callback, state);
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Begins to asynchronously receive data from a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffers">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the receive operation. This object is passed to the <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			SocketError socketError;
			IAsyncResult result = this.BeginReceive(buffers, socketFlags, out socketError, callback, state);
			if (socketError != SocketError.Success && socketError != SocketError.IOPending)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Ends a pending asynchronous read.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginReceive(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> was previously called for the asynchronous read.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndReceive(IAsyncResult asyncResult)
		{
			SocketError socketError;
			int result = this.EndReceive(asyncResult, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		/// <summary>Begins an asynchronous operation to accept an incoming connection attempt and receives the first block of data sent by the client application.</summary>
		/// <param name="receiveSize">The number of bytes to accept from the sender.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous <see cref="T:System.Net.Sockets.Socket" /> creation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The accepting socket is not listening for connections. You must call <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> and <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> before calling <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.  
		///  -or-  
		///  The accepted socket is bound.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="receiveSize" /> is less than 0.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
		{
			return this.BeginAccept(null, receiveSize, callback, state);
		}

		/// <summary>Asynchronously accepts an incoming connection attempt and creates a new <see cref="T:System.Net.Sockets.Socket" /> object to handle remote host communication. This method returns a buffer that contains the initial data transferred.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the bytes transferred.</param>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object that stores state information for this asynchronous operation as well as any user defined data.</param>
		/// <returns>A <see cref="T:System.Net.Sockets.Socket" /> object to handle communication with the remote host.</returns>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is empty.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not created by a call to <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndAccept(System.IAsyncResult)" /> method was previously called.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the <see cref="T:System.Net.Sockets.Socket" /></exception>
		public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
		{
			byte[] sourceArray;
			int num;
			Socket result = this.EndAccept(out sourceArray, out num, asyncResult);
			buffer = new byte[num];
			Array.Copy(sourceArray, buffer, num);
			return result;
		}

		private static object InternalSyncObject
		{
			get
			{
				if (Socket.s_InternalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange(ref Socket.s_InternalSyncObject, value, null);
				}
				return Socket.s_InternalSyncObject;
			}
		}

		internal bool CleanedUp
		{
			get
			{
				return this.m_IntCleanedUp == 1;
			}
		}

		internal static void InitializeSockets()
		{
			if (!Socket.s_Initialized)
			{
				object internalSyncObject = Socket.InternalSyncObject;
				lock (internalSyncObject)
				{
					if (!Socket.s_Initialized)
					{
						bool flag2 = Socket.IsProtocolSupported(NetworkInterfaceComponent.IPv4);
						bool flag3 = Socket.IsProtocolSupported(NetworkInterfaceComponent.IPv6);
						if (flag3)
						{
							Socket.s_OSSupportsIPv6 = true;
							flag3 = SettingsSectionInternal.Section.Ipv6Enabled;
						}
						Socket.s_SupportsIPv4 = flag2;
						Socket.s_SupportsIPv6 = flag3;
						Socket.s_Initialized = true;
					}
				}
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Net.Sockets.Socket" /> class.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Frees resources used by the <see cref="T:System.Net.Sockets.Socket" /> class.</summary>
		~Socket()
		{
			this.Dispose(false);
		}

		/// <summary>Begins an asynchronous request for a connection to a remote host.</summary>
		/// <param name="socketType">One of the <see cref="T:System.Net.Sockets.SocketType" /> values.</param>
		/// <param name="protocolType">One of the <see cref="T:System.Net.Sockets.ProtocolType" /> values.</param>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentException">An argument is not valid. This exception occurs if multiple buffers are specified, the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.BufferList" /> property is not null.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="e" /> parameter cannot be null and the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> is listening or a socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method. This exception also occurs if the local endpoint and the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> are not the same address family.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public static bool ConnectAsync(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs e)
		{
			bool flag = Socket.s_LoggingEnabled;
			if (e.BufferList != null)
			{
				throw new ArgumentException(SR.GetString("Multiple buffers cannot be used with this method."), "BufferList");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			EndPoint remoteEndPoint = e.RemoteEndPoint;
			DnsEndPoint dnsEndPoint = remoteEndPoint as DnsEndPoint;
			bool result;
			if (dnsEndPoint != null)
			{
				Socket socket = null;
				MultipleConnectAsync multipleConnectAsync;
				if (dnsEndPoint.AddressFamily == AddressFamily.Unspecified)
				{
					multipleConnectAsync = new DualSocketMultipleConnectAsync(socketType, protocolType);
				}
				else
				{
					socket = new Socket(dnsEndPoint.AddressFamily, socketType, protocolType);
					multipleConnectAsync = new SingleSocketMultipleConnectAsync(socket, false);
				}
				e.StartOperationCommon(socket);
				e.StartOperationWrapperConnect(multipleConnectAsync);
				try
				{
					result = multipleConnectAsync.StartConnectAsync(e, dnsEndPoint);
					goto IL_B7;
				}
				catch
				{
					Interlocked.Exchange(ref e.in_progress, 0);
					throw;
				}
			}
			result = new Socket(remoteEndPoint.AddressFamily, socketType, protocolType).ConnectAsync(e);
			IL_B7:
			bool flag2 = Socket.s_LoggingEnabled;
			return result;
		}

		internal void InternalShutdown(SocketShutdown how)
		{
			if (!this.is_connected || this.CleanedUp)
			{
				return;
			}
			int num;
			Socket.Shutdown_internal(this.m_Handle, how, out num);
		}

		internal IAsyncResult UnsafeBeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return this.BeginConnect(remoteEP, callback, state);
		}

		internal IAsyncResult UnsafeBeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.BeginSend(buffer, offset, size, socketFlags, callback, state);
		}

		internal IAsyncResult UnsafeBeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.BeginReceive(buffer, offset, size, socketFlags, callback, state);
		}

		internal IAsyncResult BeginMultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			ArraySegment<byte>[] array = new ArraySegment<byte>[buffers.Length];
			for (int i = 0; i < buffers.Length; i++)
			{
				array[i] = new ArraySegment<byte>(buffers[i].Buffer, buffers[i].Offset, buffers[i].Size);
			}
			return this.BeginSend(array, socketFlags, callback, state);
		}

		internal IAsyncResult UnsafeBeginMultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			return this.BeginMultipleSend(buffers, socketFlags, callback, state);
		}

		internal int EndMultipleSend(IAsyncResult asyncResult)
		{
			return this.EndSend(asyncResult);
		}

		internal void MultipleSend(BufferOffsetSize[] buffers, SocketFlags socketFlags)
		{
			ArraySegment<byte>[] array = new ArraySegment<byte>[buffers.Length];
			for (int i = 0; i < buffers.Length; i++)
			{
				array[i] = new ArraySegment<byte>(buffers[i].Buffer, buffers[i].Offset, buffers[i].Size);
			}
			this.Send(array, socketFlags);
		}

		internal void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue, bool silent)
		{
			if (this.CleanedUp && this.is_closed)
			{
				if (silent)
				{
					return;
				}
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			else
			{
				int num;
				Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, null, null, optionValue, out num);
				if (!silent && num != 0)
				{
					throw new SocketException(num);
				}
				return;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.Socket" /> class using the specified value returned from <see cref="M:System.Net.Sockets.Socket.DuplicateAndClose(System.Int32)" />.</summary>
		/// <param name="socketInformation">The socket information returned by <see cref="M:System.Net.Sockets.Socket.DuplicateAndClose(System.Int32)" />.</param>
		public Socket(SocketInformation socketInformation)
		{
			this.ReadSem = new SemaphoreSlim(1, 1);
			this.WriteSem = new SemaphoreSlim(1, 1);
			this.is_blocking = true;
			base..ctor();
			this.is_listening = ((socketInformation.Options & SocketInformationOptions.Listening) > (SocketInformationOptions)0);
			this.is_connected = ((socketInformation.Options & SocketInformationOptions.Connected) > (SocketInformationOptions)0);
			this.is_blocking = ((socketInformation.Options & SocketInformationOptions.NonBlocking) == (SocketInformationOptions)0);
			this.useOverlappedIO = ((socketInformation.Options & SocketInformationOptions.UseOnlyOverlappedIO) > (SocketInformationOptions)0);
			IList list = DataConverter.Unpack("iiiil", socketInformation.ProtocolInformation, 0);
			this.addressFamily = (AddressFamily)((int)list[0]);
			this.socketType = (SocketType)((int)list[1]);
			this.protocolType = (ProtocolType)((int)list[2]);
			this.is_bound = ((int)list[3] != 0);
			this.m_Handle = new SafeSocketHandle((IntPtr)((long)list[4]), true);
			Socket.InitializeSockets();
			this.SocketDefaults();
		}

		internal Socket(AddressFamily family, SocketType type, ProtocolType proto, SafeSocketHandle safe_handle)
		{
			this.ReadSem = new SemaphoreSlim(1, 1);
			this.WriteSem = new SemaphoreSlim(1, 1);
			this.is_blocking = true;
			base..ctor();
			this.addressFamily = family;
			this.socketType = type;
			this.protocolType = proto;
			this.m_Handle = safe_handle;
			this.is_connected = true;
			Socket.InitializeSockets();
		}

		private void SocketDefaults()
		{
			try
			{
				if (this.addressFamily == AddressFamily.InterNetwork)
				{
					this.DontFragment = false;
					if (this.protocolType == ProtocolType.Tcp)
					{
						this.NoDelay = false;
					}
				}
				else if (this.addressFamily == AddressFamily.InterNetworkV6 && this.socketType != SocketType.Raw)
				{
					this.DualMode = true;
				}
			}
			catch (SocketException)
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Socket_icall(AddressFamily family, SocketType type, ProtocolType proto, out int error);

		/// <summary>Gets the amount of data that has been received from the network and is available to be read.</summary>
		/// <returns>The number of bytes of data received from the network and available to be read.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int Available
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				int num;
				int result = Socket.Available_internal(this.m_Handle, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				return result;
			}
		}

		private static int Available_internal(SafeSocketHandle safeHandle, out int error)
		{
			bool flag = false;
			int result;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				result = Socket.Available_icall(safeHandle.DangerousGetHandle(), out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Available_icall(IntPtr socket, out int error);

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the <see cref="T:System.Net.Sockets.Socket" /> can send or receive broadcast packets.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> allows broadcast packets; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">This option is valid for a datagram socket only.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool EnableBroadcast
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				if (this.protocolType != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				return (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast) != 0;
			}
			set
			{
				this.ThrowIfDisposedAndClosed();
				if (this.protocolType != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				this.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Net.Sockets.Socket" /> is bound to a specific local port.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> is bound to a local port; otherwise, <see langword="false" />.</returns>
		public bool IsBound
		{
			get
			{
				return this.is_bound;
			}
		}

		/// <summary>Gets or sets a value that specifies whether outgoing multicast packets are delivered to the sending application.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> receives outgoing multicast packets; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool MulticastLoopback
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				if (this.protocolType == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				AddressFamily addressFamily = this.addressFamily;
				if (addressFamily == AddressFamily.InterNetwork)
				{
					return (int)this.GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback) != 0;
				}
				if (addressFamily != AddressFamily.InterNetworkV6)
				{
					throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
				}
				return (int)this.GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback) != 0;
			}
			set
			{
				this.ThrowIfDisposedAndClosed();
				if (this.protocolType == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				AddressFamily addressFamily = this.addressFamily;
				if (addressFamily == AddressFamily.InterNetwork)
				{
					this.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, value ? 1 : 0);
					return;
				}
				if (addressFamily != AddressFamily.InterNetworkV6)
				{
					throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
				}
				this.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, value ? 1 : 0);
			}
		}

		/// <summary>Gets the local endpoint.</summary>
		/// <returns>The <see cref="T:System.Net.EndPoint" /> that the <see cref="T:System.Net.Sockets.Socket" /> is using for communications.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public EndPoint LocalEndPoint
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				if (this.seed_endpoint == null)
				{
					return null;
				}
				int num;
				SocketAddress socketAddress = Socket.LocalEndPoint_internal(this.m_Handle, (int)this.addressFamily, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				return this.seed_endpoint.Create(socketAddress);
			}
		}

		private static SocketAddress LocalEndPoint_internal(SafeSocketHandle safeHandle, int family, out int error)
		{
			bool flag = false;
			SocketAddress result;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				result = Socket.LocalEndPoint_icall(safeHandle.DangerousGetHandle(), family, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress LocalEndPoint_icall(IntPtr socket, int family, out int error);

		/// <summary>Gets or sets a value that indicates whether the <see cref="T:System.Net.Sockets.Socket" /> is in blocking mode.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> will block; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool Blocking
		{
			get
			{
				return this.is_blocking;
			}
			set
			{
				this.ThrowIfDisposedAndClosed();
				int num;
				Socket.Blocking_internal(this.m_Handle, value, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				this.is_blocking = value;
			}
		}

		private static void Blocking_internal(SafeSocketHandle safeHandle, bool block, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.Blocking_icall(safeHandle.DangerousGetHandle(), block, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Blocking_icall(IntPtr socket, bool block, out int error);

		/// <summary>Gets a value that indicates whether a <see cref="T:System.Net.Sockets.Socket" /> is connected to a remote host as of the last <see cref="Overload:System.Net.Sockets.Socket.Send" /> or <see cref="Overload:System.Net.Sockets.Socket.Receive" /> operation.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> was connected to a remote resource as of the most recent operation; otherwise, <see langword="false" />.</returns>
		public bool Connected
		{
			get
			{
				return this.is_connected;
			}
			internal set
			{
				this.is_connected = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that specifies whether the stream <see cref="T:System.Net.Sockets.Socket" /> is using the Nagle algorithm.</summary>
		/// <returns>
		///   <see langword="false" /> if the <see cref="T:System.Net.Sockets.Socket" /> uses the Nagle algorithm; otherwise, <see langword="true" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool NoDelay
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				this.ThrowIfUdp();
				return (int)this.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug) != 0;
			}
			set
			{
				this.ThrowIfDisposedAndClosed();
				this.ThrowIfUdp();
				this.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
			}
		}

		/// <summary>Gets the remote endpoint.</summary>
		/// <returns>The <see cref="T:System.Net.EndPoint" /> with which the <see cref="T:System.Net.Sockets.Socket" /> is communicating.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public EndPoint RemoteEndPoint
		{
			get
			{
				this.ThrowIfDisposedAndClosed();
				if (!this.is_connected || this.seed_endpoint == null)
				{
					return null;
				}
				int num;
				SocketAddress socketAddress = Socket.RemoteEndPoint_internal(this.m_Handle, (int)this.addressFamily, out num);
				if (num != 0)
				{
					throw new SocketException(num);
				}
				return this.seed_endpoint.Create(socketAddress);
			}
		}

		private static SocketAddress RemoteEndPoint_internal(SafeSocketHandle safeHandle, int family, out int error)
		{
			bool flag = false;
			SocketAddress result;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				result = Socket.RemoteEndPoint_icall(safeHandle.DangerousGetHandle(), family, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress RemoteEndPoint_icall(IntPtr socket, int family, out int error);

		internal SafeHandle SafeHandle
		{
			get
			{
				return this.m_Handle;
			}
		}

		/// <summary>Determines the status of one or more sockets.</summary>
		/// <param name="checkRead">An <see cref="T:System.Collections.IList" /> of <see cref="T:System.Net.Sockets.Socket" /> instances to check for readability.</param>
		/// <param name="checkWrite">An <see cref="T:System.Collections.IList" /> of <see cref="T:System.Net.Sockets.Socket" /> instances to check for writability.</param>
		/// <param name="checkError">An <see cref="T:System.Collections.IList" /> of <see cref="T:System.Net.Sockets.Socket" /> instances to check for errors.</param>
		/// <param name="microSeconds">The time-out value, in microseconds. A -1 value indicates an infinite time-out.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="checkRead" /> parameter is <see langword="null" /> or empty.  
		///  -and-  
		///  The <paramref name="checkWrite" /> parameter is <see langword="null" /> or empty  
		///  -and-  
		///  The <paramref name="checkError" /> parameter is <see langword="null" /> or empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
		{
			List<Socket> list = new List<Socket>();
			Socket.AddSockets(list, checkRead, "checkRead");
			Socket.AddSockets(list, checkWrite, "checkWrite");
			Socket.AddSockets(list, checkError, "checkError");
			if (list.Count == 3)
			{
				throw new ArgumentNullException("checkRead, checkWrite, checkError", "All the lists are null or empty.");
			}
			Socket[] array = list.ToArray();
			int num;
			Socket.Select_icall(ref array, microSeconds, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (array == null)
			{
				if (checkRead != null)
				{
					checkRead.Clear();
				}
				if (checkWrite != null)
				{
					checkWrite.Clear();
				}
				if (checkError != null)
				{
					checkError.Clear();
				}
				return;
			}
			int num2 = 0;
			int num3 = array.Length;
			IList list2 = checkRead;
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				Socket socket = array[i];
				if (socket == null)
				{
					if (list2 != null)
					{
						int num5 = list2.Count - num4;
						for (int j = 0; j < num5; j++)
						{
							list2.RemoveAt(num4);
						}
					}
					list2 = ((num2 == 0) ? checkWrite : checkError);
					num4 = 0;
					num2++;
				}
				else
				{
					if (num2 == 1 && list2 == checkWrite && !socket.is_connected && (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
					{
						socket.is_connected = true;
					}
					while ((Socket)list2[num4] != socket)
					{
						list2.RemoveAt(num4);
					}
					num4++;
				}
			}
		}

		private static void AddSockets(List<Socket> sockets, IList list, string name)
		{
			if (list != null)
			{
				foreach (object obj in list)
				{
					Socket socket = (Socket)obj;
					if (socket == null)
					{
						throw new ArgumentNullException(name, "Contains a null element");
					}
					sockets.Add(socket);
				}
			}
			sockets.Add(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Select_icall(ref Socket[] sockets, int microSeconds, out int error);

		/// <summary>Determines the status of the <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="microSeconds">The time to wait for a response, in microseconds.</param>
		/// <param name="mode">One of the <see cref="T:System.Net.Sockets.SelectMode" /> values.</param>
		/// <returns>The status of the <see cref="T:System.Net.Sockets.Socket" /> based on the polling mode value passed in the <paramref name="mode" /> parameter.  
		///   Mode  
		///
		///   Return Value  
		///
		///  <see cref="F:System.Net.Sockets.SelectMode.SelectRead" /><see langword="true" /> if <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> has been called and a connection is pending;  
		///
		///  -or-  
		///
		///  <see langword="true" /> if data is available for reading;  
		///
		///  -or-  
		///
		///  <see langword="true" /> if the connection has been closed, reset, or terminated;  
		///
		///  otherwise, returns <see langword="false" />.  
		///
		///  <see cref="F:System.Net.Sockets.SelectMode.SelectWrite" /><see langword="true" />, if processing a <see cref="M:System.Net.Sockets.Socket.Connect(System.Net.EndPoint)" />, and the connection has succeeded;  
		///
		///  -or-  
		///
		///  <see langword="true" /> if data can be sent;  
		///
		///  otherwise, returns <see langword="false" />.  
		///
		///  <see cref="F:System.Net.Sockets.SelectMode.SelectError" /><see langword="true" /> if processing a <see cref="M:System.Net.Sockets.Socket.Connect(System.Net.EndPoint)" /> that does not block, and the connection has failed;  
		///
		///  -or-  
		///
		///  <see langword="true" /> if <see cref="F:System.Net.Sockets.SocketOptionName.OutOfBandInline" /> is not set and out-of-band data is available;  
		///
		///  otherwise, returns <see langword="false" />.</returns>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="mode" /> parameter is not one of the <see cref="T:System.Net.Sockets.SelectMode" /> values.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks below.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool Poll(int microSeconds, SelectMode mode)
		{
			this.ThrowIfDisposedAndClosed();
			if (mode != SelectMode.SelectRead && mode != SelectMode.SelectWrite && mode != SelectMode.SelectError)
			{
				throw new NotSupportedException("'mode' parameter is not valid.");
			}
			int num;
			bool flag = Socket.Poll_internal(this.m_Handle, mode, microSeconds, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (mode == SelectMode.SelectWrite && flag && !this.is_connected && (int)this.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
			{
				this.is_connected = true;
			}
			return flag;
		}

		private static bool Poll_internal(SafeSocketHandle safeHandle, SelectMode mode, int timeout, out int error)
		{
			bool flag = false;
			bool result;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				result = Socket.Poll_icall(safeHandle.DangerousGetHandle(), mode, timeout, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Poll_icall(IntPtr socket, SelectMode mode, int timeout, out int error);

		/// <summary>Creates a new <see cref="T:System.Net.Sockets.Socket" /> for a newly created connection.</summary>
		/// <returns>A <see cref="T:System.Net.Sockets.Socket" /> for a newly created connection.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The accepting socket is not listening for connections. You must call <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> and <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> before calling <see cref="M:System.Net.Sockets.Socket.Accept" />.</exception>
		public Socket Accept()
		{
			this.ThrowIfDisposedAndClosed();
			int num = 0;
			SafeSocketHandle safe_handle = Socket.Accept_internal(this.m_Handle, out num, this.is_blocking);
			if (num != 0)
			{
				if (this.is_closed)
				{
					num = 10004;
				}
				throw new SocketException(num);
			}
			return new Socket(this.AddressFamily, this.SocketType, this.ProtocolType, safe_handle)
			{
				seed_endpoint = this.seed_endpoint,
				Blocking = this.Blocking
			};
		}

		internal void Accept(Socket acceptSocket)
		{
			this.ThrowIfDisposedAndClosed();
			int num = 0;
			SafeSocketHandle handle = Socket.Accept_internal(this.m_Handle, out num, this.is_blocking);
			if (num != 0)
			{
				if (this.is_closed)
				{
					num = 10004;
				}
				throw new SocketException(num);
			}
			acceptSocket.addressFamily = this.AddressFamily;
			acceptSocket.socketType = this.SocketType;
			acceptSocket.protocolType = this.ProtocolType;
			acceptSocket.m_Handle = handle;
			acceptSocket.is_connected = true;
			acceptSocket.seed_endpoint = this.seed_endpoint;
			acceptSocket.Blocking = this.Blocking;
		}

		/// <summary>Begins an asynchronous operation to accept an incoming connection attempt.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentException">An argument is not valid. This exception occurs if the buffer provided is not large enough. The buffer must be at least 2 * (sizeof(SOCKADDR_STORAGE + 16) bytes.  
		///  This exception also occurs if multiple buffers are specified, the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.BufferList" /> property is not null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">An argument is out of range. The exception occurs if the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.Count" /> is less than 0.</exception>
		/// <exception cref="T:System.InvalidOperationException">An invalid operation was requested. This exception occurs if the accepting <see cref="T:System.Net.Sockets.Socket" /> is not listening for connections or the accepted socket is bound.  
		///  You must call the <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> and <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> method before calling the <see cref="M:System.Net.Sockets.Socket.AcceptAsync(System.Net.Sockets.SocketAsyncEventArgs)" /> method.  
		///  This exception also occurs if the socket is already connected or a socket operation was already in progress using the specified <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public bool AcceptAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_bound)
			{
				throw new InvalidOperationException("You must call the Bind method before performing this operation.");
			}
			if (!this.is_listening)
			{
				throw new InvalidOperationException("You must call the Listen method before performing this operation.");
			}
			if (e.BufferList != null)
			{
				throw new ArgumentException("Multiple buffers cannot be used with this method.");
			}
			if (e.Count < 0)
			{
				throw new ArgumentOutOfRangeException("e.Count");
			}
			Socket acceptSocket = e.AcceptSocket;
			if (acceptSocket != null && (acceptSocket.is_bound || acceptSocket.is_connected))
			{
				throw new InvalidOperationException("AcceptSocket: The socket must not be bound or connected.");
			}
			this.InitSocketAsyncEventArgs(e, Socket.AcceptAsyncCallback, e, SocketOperation.Accept);
			this.QueueIOSelectorJob(this.ReadSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginAcceptCallback, e.socket_async_result));
			return true;
		}

		/// <summary>Begins an asynchronous operation to accept an incoming connection attempt.</summary>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous <see cref="T:System.Net.Sockets.Socket" /> creation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The accepting socket is not listening for connections. You must call <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> and <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> before calling <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.  
		///  -or-  
		///  The accepted socket is bound.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="receiveSize" /> is less than 0.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public IAsyncResult BeginAccept(AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_bound || !this.is_listening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.Accept);
			this.QueueIOSelectorJob(this.ReadSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginAcceptCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Begins an asynchronous operation to accept an incoming connection attempt from a specified socket and receives the first block of data sent by the client application.</summary>
		/// <param name="acceptSocket">The accepted <see cref="T:System.Net.Sockets.Socket" /> object. This value may be <see langword="null" />.</param>
		/// <param name="receiveSize">The maximum number of bytes to receive.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous <see cref="T:System.Net.Sockets.Socket" /> object creation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The accepting socket is not listening for connections. You must call <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> and <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" /> before calling <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.  
		///  -or-  
		///  The accepted socket is bound.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="receiveSize" /> is less than 0.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (receiveSize < 0)
			{
				throw new ArgumentOutOfRangeException("receiveSize", "receiveSize is less than zero");
			}
			if (acceptSocket != null)
			{
				this.ThrowIfDisposedAndClosed(acceptSocket);
				if (acceptSocket.IsBound)
				{
					throw new InvalidOperationException();
				}
				if (acceptSocket.ProtocolType != ProtocolType.Tcp)
				{
					throw new SocketException(10022);
				}
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.AcceptReceive)
			{
				Buffer = new byte[receiveSize],
				Offset = 0,
				Size = receiveSize,
				SockFlags = SocketFlags.None,
				AcceptSocket = acceptSocket
			};
			this.QueueIOSelectorJob(this.ReadSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginAcceptReceiveCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Asynchronously accepts an incoming connection attempt and creates a new <see cref="T:System.Net.Sockets.Socket" /> to handle remote host communication.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information for this asynchronous operation as well as any user defined data.</param>
		/// <returns>A <see cref="T:System.Net.Sockets.Socket" /> to handle communication with the remote host.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not created by a call to <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See the Remarks section for more information.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndAccept(System.IAsyncResult)" /> method was previously called.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		public Socket EndAccept(IAsyncResult asyncResult)
		{
			byte[] array;
			int num;
			return this.EndAccept(out array, out num, asyncResult);
		}

		/// <summary>Asynchronously accepts an incoming connection attempt and creates a new <see cref="T:System.Net.Sockets.Socket" /> object to handle remote host communication. This method returns a buffer that contains the initial data and the number of bytes transferred.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the bytes transferred.</param>
		/// <param name="bytesTransferred">The number of bytes transferred.</param>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object that stores state information for this asynchronous operation as well as any user defined data.</param>
		/// <returns>A <see cref="T:System.Net.Sockets.Socket" /> object to handle communication with the remote host.</returns>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is empty.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not created by a call to <see cref="M:System.Net.Sockets.Socket.BeginAccept(System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndAccept(System.IAsyncResult)" /> method was previously called.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndAccept", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			buffer = socketAsyncResult.Buffer.ToArray();
			bytesTransferred = socketAsyncResult.Total;
			return socketAsyncResult.AcceptedSocket;
		}

		private static SafeSocketHandle Accept_internal(SafeSocketHandle safeHandle, out int error, bool blocking)
		{
			SafeSocketHandle result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = new SafeSocketHandle(Socket.Accept_icall(safeHandle.DangerousGetHandle(), out error, blocking), true);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Accept_icall(IntPtr sock, out int error, bool blocking);

		/// <summary>Associates a <see cref="T:System.Net.Sockets.Socket" /> with a local endpoint.</summary>
		/// <param name="localEP">The local <see cref="T:System.Net.EndPoint" /> to associate with the <see cref="T:System.Net.Sockets.Socket" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="localEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public void Bind(EndPoint localEP)
		{
			this.ThrowIfDisposedAndClosed();
			if (localEP == null)
			{
				throw new ArgumentNullException("localEP");
			}
			IPEndPoint ipendPoint = localEP as IPEndPoint;
			if (ipendPoint != null)
			{
				localEP = this.RemapIPEndPoint(ipendPoint);
			}
			int num;
			Socket.Bind_internal(this.m_Handle, localEP.Serialize(), out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (num == 0)
			{
				this.is_bound = true;
			}
			this.seed_endpoint = localEP;
		}

		private static void Bind_internal(SafeSocketHandle safeHandle, SocketAddress sa, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.Bind_icall(safeHandle.DangerousGetHandle(), sa, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Bind_icall(IntPtr sock, SocketAddress sa, out int error);

		/// <summary>Places a <see cref="T:System.Net.Sockets.Socket" /> in a listening state.</summary>
		/// <param name="backlog">The maximum length of the pending connections queue.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void Listen(int backlog)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_bound)
			{
				throw new SocketException(10022);
			}
			int num;
			Socket.Listen_internal(this.m_Handle, backlog, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			this.is_listening = true;
		}

		private static void Listen_internal(SafeSocketHandle safeHandle, int backlog, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.Listen_icall(safeHandle.DangerousGetHandle(), backlog, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Listen_icall(IntPtr sock, int backlog, out int error);

		/// <summary>Establishes a connection to a remote host. The host is specified by an IP address and a port number.</summary>
		/// <param name="address">The IP address of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="address" /> is zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />.</exception>
		public void Connect(IPAddress address, int port)
		{
			this.Connect(new IPEndPoint(address, port));
		}

		/// <summary>Establishes a connection to a remote host. The host is specified by a host name and a port number.</summary>
		/// <param name="host">The name of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="host" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />.</exception>
		public void Connect(string host, int port)
		{
			this.Connect(Dns.GetHostAddresses(host), port);
		}

		/// <summary>Establishes a connection to a remote host.</summary>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the remote device.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />.</exception>
		public void Connect(EndPoint remoteEP)
		{
			this.ThrowIfDisposedAndClosed();
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			IPEndPoint ipendPoint = remoteEP as IPEndPoint;
			if (ipendPoint != null && this.socketType != SocketType.Dgram && (ipendPoint.Address.Equals(IPAddress.Any) || ipendPoint.Address.Equals(IPAddress.IPv6Any)))
			{
				throw new SocketException(10049);
			}
			if (this.is_listening)
			{
				throw new InvalidOperationException();
			}
			if (ipendPoint != null)
			{
				remoteEP = this.RemapIPEndPoint(ipendPoint);
			}
			SocketAddress sa = remoteEP.Serialize();
			int num = 0;
			Socket.Connect_internal(this.m_Handle, sa, out num, this.is_blocking);
			if (num == 0 || num == 10035)
			{
				this.seed_endpoint = remoteEP;
			}
			if (num != 0)
			{
				if (this.is_closed)
				{
					num = 10004;
				}
				throw new SocketException(num);
			}
			this.is_connected = (this.socketType != SocketType.Dgram || ipendPoint == null || (!ipendPoint.Address.Equals(IPAddress.Any) && !ipendPoint.Address.Equals(IPAddress.IPv6Any)));
			this.is_bound = true;
		}

		/// <summary>Begins an asynchronous request for a connection to a remote host.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentException">An argument is not valid. This exception occurs if multiple buffers are specified, the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.BufferList" /> property is not null.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="e" /> parameter cannot be null and the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> is listening or a socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method. This exception also occurs if the local endpoint and the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> are not the same address family.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public bool ConnectAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (this.is_listening)
			{
				throw new InvalidOperationException("You may not perform this operation after calling the Listen method.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			this.InitSocketAsyncEventArgs(e, null, e, SocketOperation.Connect);
			bool result;
			try
			{
				IPAddress[] array;
				SocketAsyncResult socketAsyncResult;
				bool flag;
				if (!this.GetCheckedIPs(e, out array))
				{
					socketAsyncResult = new SocketAsyncResult(this, Socket.ConnectAsyncCallback, e, SocketOperation.Connect)
					{
						EndPoint = e.RemoteEndPoint
					};
					flag = Socket.BeginSConnect(socketAsyncResult);
				}
				else
				{
					DnsEndPoint dnsEndPoint = (DnsEndPoint)e.RemoteEndPoint;
					if (array == null)
					{
						throw new ArgumentNullException("addresses");
					}
					if (array.Length == 0)
					{
						throw new ArgumentException("Empty addresses list");
					}
					if (this.AddressFamily != AddressFamily.InterNetwork && this.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
					}
					if (dnsEndPoint.Port <= 0 || dnsEndPoint.Port > 65535)
					{
						throw new ArgumentOutOfRangeException("port", "Must be > 0 and < 65536");
					}
					socketAsyncResult = new SocketAsyncResult(this, Socket.ConnectAsyncCallback, e, SocketOperation.Connect)
					{
						Addresses = array,
						Port = dnsEndPoint.Port
					};
					this.is_connected = false;
					flag = Socket.BeginMConnect(socketAsyncResult);
				}
				if (!flag)
				{
					e.CurrentSocket.EndConnect(socketAsyncResult);
				}
				result = flag;
			}
			catch (SocketException ex)
			{
				e.SocketError = ex.SocketErrorCode;
				e.socket_async_result.Complete(ex, true);
				result = false;
			}
			catch (Exception e2)
			{
				e.socket_async_result.Complete(e2, true);
				result = false;
			}
			return result;
		}

		/// <summary>Cancels an asynchronous request for a remote host connection.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object used to request the connection to the remote host by calling one of the <see cref="M:System.Net.Sockets.Socket.ConnectAsync(System.Net.Sockets.SocketType,System.Net.Sockets.ProtocolType,System.Net.Sockets.SocketAsyncEventArgs)" /> methods.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="e" /> parameter cannot be null and the <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public static void CancelConnectAsync(SocketAsyncEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (e.in_progress != 0 && e.LastOperation == SocketAsyncOperation.Connect)
			{
				Socket currentSocket = e.CurrentSocket;
				if (currentSocket == null)
				{
					return;
				}
				currentSocket.Close();
			}
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The host is specified by a host name and a port number.</summary>
		/// <param name="host">The name of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the connect operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="host" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets in the <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> families.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />, or an asynchronous operation is already in progress.</exception>
		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (this.addressFamily != AddressFamily.InterNetwork && this.addressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is valid only for sockets in the InterNetwork and InterNetworkV6 families");
			}
			if (port <= 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port", "Must be > 0 and < 65536");
			}
			if (this.is_listening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult sockares = new SocketAsyncResult(this, callback, state, SocketOperation.Connect)
			{
				Port = port
			};
			Dns.GetHostAddressesAsync(host).ContinueWith(delegate(Task<IPAddress[]> t)
			{
				if (t.IsFaulted)
				{
					sockares.Complete(t.Exception.InnerException);
					return;
				}
				if (t.IsCanceled)
				{
					sockares.Complete(new OperationCanceledException());
					return;
				}
				sockares.Addresses = t.Result;
				Socket.BeginMConnect(sockares);
			}, TaskScheduler.Default);
			return sockares;
		}

		/// <summary>Begins an asynchronous request for a remote host connection.</summary>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the remote host.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous connection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />, or an asynchronous operation is already in progress.</exception>
		public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (this.is_listening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.Connect);
			socketAsyncResult.EndPoint = remoteEP;
			Socket.BeginSConnect(socketAsyncResult);
			return socketAsyncResult;
		}

		/// <summary>Begins an asynchronous request for a remote host connection. The host is specified by an <see cref="T:System.Net.IPAddress" /> array and a port number.</summary>
		/// <param name="addresses">At least one <see cref="T:System.Net.IPAddress" />, designating the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <param name="requestCallback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the connect operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the connect operation. This object is passed to the <paramref name="requestCallback" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous connections.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="addresses" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">This method is valid for sockets that use <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The port number is not valid.</exception>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="address" /> is zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> has been placed in a listening state by calling <see cref="M:System.Net.Sockets.Socket.Listen(System.Int32)" />, or an asynchronous operation is already in progress.</exception>
		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (addresses.Length == 0)
			{
				throw new ArgumentException("Empty addresses list");
			}
			if (this.AddressFamily != AddressFamily.InterNetwork && this.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
			}
			if (port <= 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port", "Must be > 0 and < 65536");
			}
			if (this.is_listening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, requestCallback, state, SocketOperation.Connect);
			socketAsyncResult.Addresses = addresses;
			socketAsyncResult.Port = port;
			this.is_connected = false;
			Socket.BeginMConnect(socketAsyncResult);
			return socketAsyncResult;
		}

		private static bool BeginMConnect(SocketAsyncResult sockares)
		{
			Exception e = null;
			for (int i = sockares.CurrentAddress; i < sockares.Addresses.Length; i++)
			{
				try
				{
					sockares.CurrentAddress++;
					sockares.EndPoint = new IPEndPoint(sockares.Addresses[i], sockares.Port);
					if (sockares.socket.CanTryAddressFamily(sockares.EndPoint.AddressFamily))
					{
						return Socket.BeginSConnect(sockares);
					}
				}
				catch (Exception e)
				{
				}
			}
			sockares.Complete(e, true);
			return false;
		}

		private static bool BeginSConnect(SocketAsyncResult sockares)
		{
			EndPoint endPoint = sockares.EndPoint;
			if (endPoint is IPEndPoint)
			{
				IPEndPoint ipendPoint = (IPEndPoint)endPoint;
				if (ipendPoint.Address.Equals(IPAddress.Any) || ipendPoint.Address.Equals(IPAddress.IPv6Any))
				{
					sockares.Complete(new SocketException(10049), true);
					return false;
				}
				endPoint = (sockares.EndPoint = sockares.socket.RemapIPEndPoint(ipendPoint));
			}
			if (!sockares.socket.CanTryAddressFamily(sockares.EndPoint.AddressFamily))
			{
				sockares.Complete(new ArgumentException("None of the discovered or specified addresses match the socket address family."), true);
				return false;
			}
			int num = 0;
			if (sockares.socket.connect_in_progress)
			{
				sockares.socket.connect_in_progress = false;
				sockares.socket.m_Handle.Dispose();
				sockares.socket.m_Handle = new SafeSocketHandle(Socket.Socket_icall(sockares.socket.addressFamily, sockares.socket.socketType, sockares.socket.protocolType, out num), true);
				if (num != 0)
				{
					sockares.Complete(new SocketException(num), true);
					return false;
				}
			}
			bool flag = sockares.socket.is_blocking;
			if (flag)
			{
				sockares.socket.Blocking = false;
			}
			Socket.Connect_internal(sockares.socket.m_Handle, endPoint.Serialize(), out num, false);
			if (flag)
			{
				sockares.socket.Blocking = true;
			}
			if (num == 0)
			{
				sockares.socket.is_connected = true;
				sockares.socket.is_bound = true;
				sockares.Complete(true);
				return false;
			}
			if (num != 10036 && num != 10035)
			{
				sockares.socket.is_connected = false;
				sockares.socket.is_bound = false;
				sockares.Complete(new SocketException(num), true);
				return false;
			}
			sockares.socket.is_connected = false;
			sockares.socket.is_bound = false;
			sockares.socket.connect_in_progress = true;
			IOSelector.Add(sockares.Handle, new IOSelectorJob(IOOperation.Write, Socket.BeginConnectCallback, sockares));
			return true;
		}

		/// <summary>Ends a pending asynchronous connection request.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginConnect(System.Net.EndPoint,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndConnect(System.IAsyncResult)" /> was previously called for the asynchronous connection.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void EndConnect(IAsyncResult asyncResult)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndConnect", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		private static void Connect_internal(SafeSocketHandle safeHandle, SocketAddress sa, out int error, bool blocking)
		{
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				Socket.Connect_icall(safeHandle.DangerousGetHandle(), sa, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Connect_icall(IntPtr sock, SocketAddress sa, out int error, bool blocking);

		private bool GetCheckedIPs(SocketAsyncEventArgs e, out IPAddress[] addresses)
		{
			addresses = null;
			DnsEndPoint dnsEndPoint = e.RemoteEndPoint as DnsEndPoint;
			if (dnsEndPoint == null)
			{
				e.SetConnectByNameError(null);
				return false;
			}
			addresses = Dns.GetHostAddresses(dnsEndPoint.Host);
			if (dnsEndPoint.AddressFamily == AddressFamily.Unspecified)
			{
				return true;
			}
			int num = 0;
			for (int i = 0; i < addresses.Length; i++)
			{
				if (addresses[i].AddressFamily == dnsEndPoint.AddressFamily)
				{
					addresses[num++] = addresses[i];
				}
			}
			if (num != addresses.Length)
			{
				Array.Resize<IPAddress>(ref addresses, num);
			}
			return true;
		}

		/// <summary>Closes the socket connection and allows reuse of the socket.</summary>
		/// <param name="reuseSocket">
		///   <see langword="true" /> if this socket can be reused after the current connection is closed; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.PlatformNotSupportedException">This method requires Windows 2000 or earlier, or the exception will be thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public void Disconnect(bool reuseSocket)
		{
			this.ThrowIfDisposedAndClosed();
			int num = 0;
			Socket.Disconnect_internal(this.m_Handle, reuseSocket, out num);
			if (num == 0)
			{
				this.is_connected = false;
				return;
			}
			if (num == 50)
			{
				throw new PlatformNotSupportedException();
			}
			throw new SocketException(num);
		}

		/// <summary>Begins an asynchronous request to disconnect from a remote endpoint.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="e" /> parameter cannot be null.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public bool DisconnectAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			this.InitSocketAsyncEventArgs(e, Socket.DisconnectAsyncCallback, e, SocketOperation.Disconnect);
			IOSelector.Add(e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Write, Socket.BeginDisconnectCallback, e.socket_async_result));
			return true;
		}

		/// <summary>Begins an asynchronous request to disconnect from a remote endpoint.</summary>
		/// <param name="reuseSocket">
		///   <see langword="true" /> if this socket can be reused after the connection is closed; otherwise, <see langword="false" />.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that references the asynchronous operation.</returns>
		/// <exception cref="T:System.NotSupportedException">The operating system is Windows 2000 or earlier, and this method requires Windows XP.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.Disconnect)
			{
				ReuseSocket = reuseSocket
			};
			IOSelector.Add(socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Write, Socket.BeginDisconnectCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Ends a pending asynchronous disconnect request.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object that stores state information and any user-defined data for this asynchronous operation.</param>
		/// <exception cref="T:System.NotSupportedException">The operating system is Windows 2000 or earlier, and this method requires Windows XP.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginDisconnect(System.Boolean,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndDisconnect(System.IAsyncResult)" /> was previously called for the asynchronous connection.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.Net.WebException">The disconnect request has timed out.</exception>
		public void EndDisconnect(IAsyncResult asyncResult)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndDisconnect", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		private static void Disconnect_internal(SafeSocketHandle safeHandle, bool reuse, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.Disconnect_icall(safeHandle.DangerousGetHandle(), reuse, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Disconnect_icall(IntPtr sock, bool reuse, out int error);

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into a receive buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="offset">The position in the <paramref name="buffer" /> parameter to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// The <see cref="P:System.Net.Sockets.Socket.LocalEndPoint" /> property is not set.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public unsafe int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			int num;
			int result;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				result = Socket.Receive_internal(this.m_Handle, ptr + offset, size, socketFlags, out num, this.is_blocking);
			}
			errorCode = (SocketError)num;
			if (errorCode != SocketError.Success && errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
			{
				this.is_connected = false;
				this.is_bound = false;
				return result;
			}
			this.is_connected = true;
			return result;
		}

		private unsafe int Receive(Memory<byte> buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			int num;
			int result;
			using (MemoryHandle memoryHandle = buffer.Slice(offset, size).Pin())
			{
				result = Socket.Receive_internal(this.m_Handle, (byte*)memoryHandle.Pointer, size, socketFlags, out num, this.is_blocking);
			}
			errorCode = (SocketError)num;
			if (errorCode != SocketError.Success && errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
			{
				this.is_connected = false;
				this.is_bound = false;
			}
			else
			{
				this.is_connected = true;
			}
			return result;
		}

		/// <summary>Receives data from a bound <see cref="T:System.Net.Sockets.Socket" /> into the list of receive buffers, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffers" />.Count is zero.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred while attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[CLSCompliant(false)]
		public unsafe int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			if (buffers == null || buffers.Count == 0)
			{
				throw new ArgumentNullException("buffers");
			}
			int count = buffers.Count;
			GCHandle[] array = new GCHandle[count];
			int num;
			int result;
			try
			{
				try
				{
					Socket.WSABUF[] array2;
					Socket.WSABUF* ptr;
					if ((array2 = new Socket.WSABUF[count]) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					for (int i = 0; i < count; i++)
					{
						ArraySegment<byte> arraySegment = buffers[i];
						if (arraySegment.Offset < 0 || arraySegment.Count < 0 || arraySegment.Count > arraySegment.Array.Length - arraySegment.Offset)
						{
							throw new ArgumentOutOfRangeException("segment");
						}
						try
						{
						}
						finally
						{
							array[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
						}
						ptr[i].len = arraySegment.Count;
						ptr[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(arraySegment.Array, arraySegment.Offset);
					}
					result = Socket.Receive_internal(this.m_Handle, ptr, count, socketFlags, out num, this.is_blocking);
				}
				finally
				{
					Socket.WSABUF[] array2 = null;
				}
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array[j].IsAllocated)
					{
						array[j].Free();
					}
				}
			}
			errorCode = (SocketError)num;
			return result;
		}

		public int Receive(Span<byte> buffer, SocketFlags socketFlags, out SocketError errorCode)
		{
			byte[] array = new byte[buffer.Length];
			int result = this.Receive(array, 0, array.Length, socketFlags, out errorCode);
			array.CopyTo(buffer);
			return result;
		}

		public int Send(ReadOnlySpan<byte> buffer, SocketFlags socketFlags, out SocketError errorCode)
		{
			byte[] array = buffer.ToArray();
			return this.Send(array, 0, array.Length, socketFlags, out errorCode);
		}

		public int Receive(Span<byte> buffer, SocketFlags socketFlags)
		{
			byte[] array = new byte[buffer.Length];
			int result = this.Receive(array, SocketFlags.None);
			array.CopyTo(buffer);
			return result;
		}

		public int Receive(Span<byte> buffer)
		{
			return this.Receive(buffer, SocketFlags.None);
		}

		/// <summary>Begins an asynchronous request to receive data from a connected <see cref="T:System.Net.Sockets.Socket" /> object.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentException">An argument was invalid. The <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.Buffer" /> or <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.BufferList" /> properties on the <paramref name="e" /> parameter must reference valid buffers. One or the other of these properties may be set, but not both at the same time.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public bool ReceiveAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (e.MemoryBuffer.Equals(default(Memory<byte>)) && e.BufferList == null)
			{
				throw new NullReferenceException("Either e.Buffer or e.BufferList must be valid buffers.");
			}
			if (e.BufferList != null)
			{
				this.InitSocketAsyncEventArgs(e, Socket.ReceiveAsyncCallback, e, SocketOperation.ReceiveGeneric);
				e.socket_async_result.Buffers = e.BufferList;
				this.QueueIOSelectorJob(this.ReadSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveGenericCallback, e.socket_async_result));
			}
			else
			{
				this.InitSocketAsyncEventArgs(e, Socket.ReceiveAsyncCallback, e, SocketOperation.Receive);
				e.socket_async_result.Buffer = e.MemoryBuffer;
				e.socket_async_result.Offset = e.Offset;
				e.socket_async_result.Size = e.Count;
				this.QueueIOSelectorJob(this.ReadSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveCallback, e.socket_async_result));
			}
			return true;
		}

		/// <summary>Begins to asynchronously receive data from a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="offset">The location in <paramref name="buffer" /> to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the receive operation. This object is passed to the <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			errorCode = SocketError.Success;
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.Receive)
			{
				Buffer = buffer,
				Offset = offset,
				Size = size,
				SockFlags = socketFlags
			};
			this.QueueIOSelectorJob(this.ReadSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Begins to asynchronously receive data from a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffers">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the receive operation. This object is passed to the <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> delegate when the operation is complete.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">
		///   <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[CLSCompliant(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			errorCode = SocketError.Success;
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.ReceiveGeneric)
			{
				Buffers = buffers,
				SockFlags = socketFlags
			};
			this.QueueIOSelectorJob(this.ReadSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveGenericCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Ends a pending asynchronous read.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginReceive(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndReceive(System.IAsyncResult)" /> was previously called for the asynchronous read.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndReceive", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			if (errorCode != SocketError.Success && errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
			{
				this.is_connected = false;
			}
			if (errorCode == SocketError.Success)
			{
				socketAsyncResult.CheckIfThrowDelayedException();
			}
			return socketAsyncResult.Total;
		}

		private unsafe static int Receive_internal(SafeSocketHandle safeHandle, Socket.WSABUF* bufarray, int count, SocketFlags flags, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.Receive_array_icall(safeHandle.DangerousGetHandle(), bufarray, count, flags, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int Receive_array_icall(IntPtr sock, Socket.WSABUF* bufarray, int count, SocketFlags flags, out int error, bool blocking);

		private unsafe static int Receive_internal(SafeSocketHandle safeHandle, byte* buffer, int count, SocketFlags flags, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.Receive_icall(safeHandle.DangerousGetHandle(), buffer, count, flags, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int Receive_icall(IntPtr sock, byte* buffer, int count, SocketFlags flags, out int error, bool blocking);

		/// <summary>Receives the specified number of bytes of data into the specified location of the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for received data.</param>
		/// <param name="offset">The position in the <paramref name="buffer" /> parameter to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" />, passed by reference, that represents the remote server.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of the <paramref name="buffer" /> minus the value of the offset parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// The <see cref="P:System.Net.Sockets.Socket.LocalEndPoint" /> property was not set.  
		/// -or-  
		/// An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			SocketError socketError;
			int result = this.ReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP, out socketError);
			if (socketError != SocketError.Success)
			{
				throw new SocketException(socketError);
			}
			return result;
		}

		internal unsafe int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, out SocketError errorCode)
		{
			SocketAddress socketAddress = remoteEP.Serialize();
			int num;
			int result;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				result = Socket.ReceiveFrom_internal(this.m_Handle, ptr + offset, size, socketFlags, ref socketAddress, out num, this.is_blocking);
			}
			errorCode = (SocketError)num;
			if (errorCode != SocketError.Success)
			{
				if (errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
				{
					this.is_connected = false;
				}
				else if (errorCode == SocketError.WouldBlock && this.is_blocking)
				{
					errorCode = SocketError.TimedOut;
				}
				return 0;
			}
			this.is_connected = true;
			this.is_bound = true;
			if (socketAddress != null)
			{
				remoteEP = remoteEP.Create(socketAddress);
			}
			this.seed_endpoint = remoteEP;
			return result;
		}

		private unsafe int ReceiveFrom(Memory<byte> buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, out SocketError errorCode)
		{
			SocketAddress socketAddress = remoteEP.Serialize();
			int num;
			int result;
			using (MemoryHandle memoryHandle = buffer.Slice(offset, size).Pin())
			{
				result = Socket.ReceiveFrom_internal(this.m_Handle, (byte*)memoryHandle.Pointer, size, socketFlags, ref socketAddress, out num, this.is_blocking);
			}
			errorCode = (SocketError)num;
			if (errorCode != SocketError.Success)
			{
				if (errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
				{
					this.is_connected = false;
				}
				else if (errorCode == SocketError.WouldBlock && this.is_blocking)
				{
					errorCode = SocketError.TimedOut;
				}
				return 0;
			}
			this.is_connected = true;
			this.is_bound = true;
			if (socketAddress != null)
			{
				remoteEP = remoteEP.Create(socketAddress);
			}
			this.seed_endpoint = remoteEP;
			return result;
		}

		/// <summary>Begins to asynchronously receive data from a specified network device.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public bool ReceiveFromAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			this.InitSocketAsyncEventArgs(e, Socket.ReceiveFromAsyncCallback, e, SocketOperation.ReceiveFrom);
			e.socket_async_result.Buffer = e.Buffer;
			e.socket_async_result.Offset = e.Offset;
			e.socket_async_result.Size = e.Count;
			e.socket_async_result.EndPoint = e.RemoteEndPoint;
			e.socket_async_result.SockFlags = e.SocketFlags;
			this.QueueIOSelectorJob(this.ReadSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveFromCallback, e.socket_async_result));
			return true;
		}

		/// <summary>Begins to asynchronously receive data from a specified network device.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="offset">The zero-based position in the <paramref name="buffer" /> parameter at which to store the data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the source of the data.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.ReceiveFrom)
			{
				Buffer = buffer,
				Offset = offset,
				Size = size,
				SockFlags = socketFlags,
				EndPoint = remoteEP
			};
			this.QueueIOSelectorJob(this.ReadSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Read, Socket.BeginReceiveFromCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Ends a pending asynchronous read from a specific endpoint.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <param name="endPoint">The source <see cref="T:System.Net.EndPoint" />.</param>
		/// <returns>If successful, the number of bytes received. If unsuccessful, returns 0.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginReceiveFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.Net.EndPoint@,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndReceiveFrom(System.IAsyncResult,System.Net.EndPoint@)" /> was previously called for the asynchronous read.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
		{
			this.ThrowIfDisposedAndClosed();
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndReceiveFrom", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			endPoint = socketAsyncResult.EndPoint;
			return socketAsyncResult.Total;
		}

		private int EndReceiveFrom_internal(SocketAsyncResult sockares, SocketAsyncEventArgs ares)
		{
			this.ThrowIfDisposedAndClosed();
			if (Interlocked.CompareExchange(ref sockares.EndCalled, 1, 0) == 1)
			{
				throw new InvalidOperationException("EndReceiveFrom can only be called once per asynchronous operation");
			}
			if (!sockares.IsCompleted)
			{
				sockares.AsyncWaitHandle.WaitOne();
			}
			sockares.CheckIfThrowDelayedException();
			ares.RemoteEndPoint = sockares.EndPoint;
			return sockares.Total;
		}

		private unsafe static int ReceiveFrom_internal(SafeSocketHandle safeHandle, byte* buffer, int count, SocketFlags flags, ref SocketAddress sockaddr, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.ReceiveFrom_icall(safeHandle.DangerousGetHandle(), buffer, count, flags, ref sockaddr, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int ReceiveFrom_icall(IntPtr sock, byte* buffer, int count, SocketFlags flags, ref SocketAddress sockaddr, out int error, bool blocking);

		/// <summary>Receives the specified number of bytes of data into the specified location of the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint and packet information.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for received data.</param>
		/// <param name="offset">The position in the <paramref name="buffer" /> parameter to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" />, passed by reference, that represents the remote server.</param>
		/// <param name="ipPacketInformation">An <see cref="T:System.Net.Sockets.IPPacketInformation" /> holding address and interface information.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// - or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of the <paramref name="buffer" /> minus the value of the offset parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// The <see cref="P:System.Net.Sockets.Socket.LocalEndPoint" /> property was not set.  
		/// -or-  
		/// The .NET Framework is running on an AMD 64-bit processor.  
		/// -or-  
		/// An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The operating system is Windows 2000 or earlier, and this method requires Windows XP.</exception>
		[MonoTODO("Not implemented")]
		public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			throw new NotImplementedException();
		}

		/// <summary>Begins to asynchronously receive the specified number of bytes of data into the specified location in the data buffer, using the specified <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.SocketFlags" />, and stores the endpoint and packet information.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		[MonoTODO("Not implemented")]
		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			throw new NotImplementedException();
		}

		/// <summary>Begins to asynchronously receive the specified number of bytes of data into the specified location of the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint and packet information.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that is the storage location for the received data.</param>
		/// <param name="offset">The zero-based position in the <paramref name="buffer" /> parameter at which to store the data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the source of the data.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.NotSupportedException">The operating system is Windows 2000 or earlier, and this method requires Windows XP.</exception>
		[MonoTODO]
		public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			throw new NotImplementedException();
		}

		/// <summary>Ends a pending asynchronous read from a specific endpoint. This method also reveals more information about the packet than <see cref="M:System.Net.Sockets.Socket.EndReceiveFrom(System.IAsyncResult,System.Net.EndPoint@)" />.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values for the received packet.</param>
		/// <param name="endPoint">The source <see cref="T:System.Net.EndPoint" />.</param>
		/// <param name="ipPacketInformation">The <see cref="T:System.Net.IPAddress" /> and interface of the received packet.</param>
		/// <returns>If successful, the number of bytes received. If unsuccessful, returns 0.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />  
		/// -or-  
		/// <paramref name="endPoint" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginReceiveMessageFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.Net.EndPoint@,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndReceiveMessageFrom(System.IAsyncResult,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> was previously called for the asynchronous read.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[MonoTODO]
		public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
		{
			this.ThrowIfDisposedAndClosed();
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			this.ValidateEndIAsyncResult(asyncResult, "EndReceiveMessageFrom", "asyncResult");
			throw new NotImplementedException();
		}

		/// <summary>Sends the specified number of bytes of data to a connected <see cref="T:System.Net.Sockets.Socket" />, starting at the specified offset, and using the specified <see cref="T:System.Net.Sockets.SocketFlags" /></summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="offset">The position in the data buffer at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public unsafe int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (size == 0)
			{
				errorCode = SocketError.Success;
				return 0;
			}
			int num = 0;
			for (;;)
			{
				int num2;
				fixed (byte[] array = buffer)
				{
					byte* ptr;
					if (buffer == null || array.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array[0];
					}
					num += Socket.Send_internal(this.m_Handle, ptr + (offset + num), size - num, socketFlags, out num2, this.is_blocking);
				}
				errorCode = (SocketError)num2;
				if (errorCode != SocketError.Success && errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
				{
					break;
				}
				this.is_connected = true;
				if (num >= size)
				{
					return num;
				}
			}
			this.is_connected = false;
			this.is_bound = false;
			return num;
		}

		/// <summary>Sends the set of buffers in the list to a connected <see cref="T:System.Net.Sockets.Socket" />, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffers">A list of <see cref="T:System.ArraySegment`1" />s of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>The number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffers" /> is empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[CLSCompliant(false)]
		public unsafe int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (buffers.Count == 0)
			{
				throw new ArgumentException("Buffer is empty", "buffers");
			}
			int count = buffers.Count;
			GCHandle[] array = new GCHandle[count];
			int num;
			int result;
			try
			{
				try
				{
					Socket.WSABUF[] array2;
					Socket.WSABUF* ptr;
					if ((array2 = new Socket.WSABUF[count]) == null || array2.Length == 0)
					{
						ptr = null;
					}
					else
					{
						ptr = &array2[0];
					}
					for (int i = 0; i < count; i++)
					{
						ArraySegment<byte> arraySegment = buffers[i];
						if (arraySegment.Offset < 0 || arraySegment.Count < 0 || arraySegment.Count > arraySegment.Array.Length - arraySegment.Offset)
						{
							throw new ArgumentOutOfRangeException("segment");
						}
						try
						{
						}
						finally
						{
							array[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
						}
						ptr[i].len = arraySegment.Count;
						ptr[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement<byte>(arraySegment.Array, arraySegment.Offset);
					}
					result = Socket.Send_internal(this.m_Handle, ptr, count, socketFlags, out num, this.is_blocking);
				}
				finally
				{
					Socket.WSABUF[] array2 = null;
				}
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array[j].IsAllocated)
					{
						array[j].Free();
					}
				}
			}
			errorCode = (SocketError)num;
			return result;
		}

		public int Send(ReadOnlySpan<byte> buffer, SocketFlags socketFlags)
		{
			return this.Send(buffer.ToArray(), socketFlags);
		}

		public int Send(ReadOnlySpan<byte> buffer)
		{
			return this.Send(buffer, SocketFlags.None);
		}

		/// <summary>Sends data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" /> object.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.Buffer" /> or <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.BufferList" /> properties on the <paramref name="e" /> parameter must reference valid buffers. One or the other of these properties may be set, but not both at the same time.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">The <see cref="T:System.Net.Sockets.Socket" /> is not yet connected or was not obtained via an <see cref="M:System.Net.Sockets.Socket.Accept" />, <see cref="M:System.Net.Sockets.Socket.AcceptAsync(System.Net.Sockets.SocketAsyncEventArgs)" />,or <see cref="Overload:System.Net.Sockets.Socket.BeginAccept" />, method.</exception>
		public bool SendAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (e.MemoryBuffer.Equals(default(Memory<byte>)) && e.BufferList == null)
			{
				throw new NullReferenceException("Either e.Buffer or e.BufferList must be valid buffers.");
			}
			if (e.BufferList != null)
			{
				this.InitSocketAsyncEventArgs(e, Socket.SendAsyncCallback, e, SocketOperation.SendGeneric);
				e.socket_async_result.Buffers = e.BufferList;
				this.QueueIOSelectorJob(this.WriteSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Write, Socket.BeginSendGenericCallback, e.socket_async_result));
			}
			else
			{
				this.InitSocketAsyncEventArgs(e, Socket.SendAsyncCallback, e, SocketOperation.Send);
				e.socket_async_result.Buffer = e.MemoryBuffer;
				e.socket_async_result.Offset = e.Offset;
				e.socket_async_result.Size = e.Count;
				this.QueueIOSelectorJob(this.WriteSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
				{
					Socket.BeginSendCallback((SocketAsyncResult)s, 0);
				}, e.socket_async_result));
			}
			return true;
		}

		/// <summary>Sends data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to send.</param>
		/// <param name="offset">The zero-based position in the <paramref name="buffer" /> parameter at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous send.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is less than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (!this.is_connected)
			{
				errorCode = SocketError.NotConnected;
				return null;
			}
			errorCode = SocketError.Success;
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.Send)
			{
				Buffer = buffer,
				Offset = offset,
				Size = size,
				SockFlags = socketFlags
			};
			this.QueueIOSelectorJob(this.WriteSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
			{
				Socket.BeginSendCallback((SocketAsyncResult)s, 0);
			}, socketAsyncResult));
			return socketAsyncResult;
		}

		private unsafe static void BeginSendCallback(SocketAsyncResult sockares, int sent_so_far)
		{
			int num = 0;
			try
			{
				using (MemoryHandle memoryHandle = sockares.Buffer.Slice(sockares.Offset, sockares.Size).Pin())
				{
					num = Socket.Send_internal(sockares.socket.m_Handle, (byte*)memoryHandle.Pointer, sockares.Size, sockares.SockFlags, out sockares.error, false);
				}
			}
			catch (Exception e)
			{
				sockares.Complete(e);
				return;
			}
			if (sockares.error == 0)
			{
				sent_so_far += num;
				sockares.Offset += num;
				sockares.Size -= num;
				if (sockares.socket.CleanedUp)
				{
					sockares.Complete(sent_so_far);
					return;
				}
				if (sockares.Size > 0)
				{
					IOSelector.Add(sockares.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
					{
						Socket.BeginSendCallback((SocketAsyncResult)s, sent_so_far);
					}, sockares));
					return;
				}
				sockares.Total = sent_so_far;
			}
			sockares.Complete(sent_so_far);
		}

		/// <summary>Sends data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="buffers">An array of type <see cref="T:System.Byte" /> that contains the data to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous send.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffers" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="buffers" /> is empty.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		[CLSCompliant(false)]
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (!this.is_connected)
			{
				errorCode = SocketError.NotConnected;
				return null;
			}
			errorCode = SocketError.Success;
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.SendGeneric)
			{
				Buffers = buffers,
				SockFlags = socketFlags
			};
			this.QueueIOSelectorJob(this.WriteSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Write, Socket.BeginSendGenericCallback, socketAsyncResult));
			return socketAsyncResult;
		}

		/// <summary>Ends a pending asynchronous send.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information for this asynchronous operation.</param>
		/// <param name="errorCode">A <see cref="T:System.Net.Sockets.SocketError" /> object that stores the socket error.</param>
		/// <returns>If successful, the number of bytes sent to the <see cref="T:System.Net.Sockets.Socket" />; otherwise, an invalid <see cref="T:System.Net.Sockets.Socket" /> error.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginSend(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndSend(System.IAsyncResult)" /> was previously called for the asynchronous send.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndSend", "asyncResult");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			if (errorCode != SocketError.Success && errorCode != SocketError.WouldBlock && errorCode != SocketError.InProgress)
			{
				this.is_connected = false;
			}
			if (errorCode == SocketError.Success)
			{
				socketAsyncResult.CheckIfThrowDelayedException();
			}
			return socketAsyncResult.Total;
		}

		private unsafe static int Send_internal(SafeSocketHandle safeHandle, Socket.WSABUF* bufarray, int count, SocketFlags flags, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.Send_array_icall(safeHandle.DangerousGetHandle(), bufarray, count, flags, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int Send_array_icall(IntPtr sock, Socket.WSABUF* bufarray, int count, SocketFlags flags, out int error, bool blocking);

		private unsafe static int Send_internal(SafeSocketHandle safeHandle, byte* buffer, int count, SocketFlags flags, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.Send_icall(safeHandle.DangerousGetHandle(), buffer, count, flags, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int Send_icall(IntPtr sock, byte* buffer, int count, SocketFlags flags, out int error, bool blocking);

		/// <summary>Sends the specified number of bytes of data to the specified endpoint, starting at the specified location in the buffer, and using the specified <see cref="T:System.Net.Sockets.SocketFlags" />.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to be sent.</param>
		/// <param name="offset">The position in the data buffer at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">The <see cref="T:System.Net.EndPoint" /> that represents the destination location for the data.</param>
		/// <returns>The number of bytes sent.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="socketFlags" /> is not a valid combination of values.  
		/// -or-  
		/// An operating system error occurs while accessing the <see cref="T:System.Net.Sockets.Socket" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public unsafe int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			int num;
			int result;
			fixed (byte[] array = buffer)
			{
				byte* ptr;
				if (buffer == null || array.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array[0];
				}
				result = Socket.SendTo_internal(this.m_Handle, ptr + offset, size, socketFlags, remoteEP.Serialize(), out num, this.is_blocking);
			}
			SocketError socketError = (SocketError)num;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					this.is_connected = false;
				}
				throw new SocketException(num);
			}
			this.is_connected = true;
			this.is_bound = true;
			this.seed_endpoint = remoteEP;
			return result;
		}

		private unsafe int SendTo(Memory<byte> buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
		{
			this.ThrowIfDisposedAndClosed();
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			int num;
			int result;
			using (MemoryHandle memoryHandle = buffer.Slice(offset, size).Pin())
			{
				result = Socket.SendTo_internal(this.m_Handle, (byte*)memoryHandle.Pointer, size, socketFlags, remoteEP.Serialize(), out num, this.is_blocking);
			}
			SocketError socketError = (SocketError)num;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					this.is_connected = false;
				}
				throw new SocketException(num);
			}
			this.is_connected = true;
			this.is_bound = true;
			this.seed_endpoint = remoteEP;
			return result;
		}

		/// <summary>Sends data asynchronously to a specific remote host.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Net.Sockets.SocketAsyncEventArgs.RemoteEndPoint" /> cannot be null.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">The protocol specified is connection-oriented, but the <see cref="T:System.Net.Sockets.Socket" /> is not yet connected.</exception>
		public bool SendToAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			this.InitSocketAsyncEventArgs(e, Socket.SendToAsyncCallback, e, SocketOperation.SendTo);
			e.socket_async_result.Buffer = e.Buffer;
			e.socket_async_result.Offset = e.Offset;
			e.socket_async_result.Size = e.Count;
			e.socket_async_result.SockFlags = e.SocketFlags;
			e.socket_async_result.EndPoint = e.RemoteEndPoint;
			this.QueueIOSelectorJob(this.WriteSem, e.socket_async_result.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
			{
				Socket.BeginSendToCallback((SocketAsyncResult)s, 0);
			}, e.socket_async_result));
			return true;
		}

		/// <summary>Sends data asynchronously to a specific remote host.</summary>
		/// <param name="buffer">An array of type <see cref="T:System.Byte" /> that contains the data to send.</param>
		/// <param name="offset">The zero-based position in <paramref name="buffer" /> at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the remote device.</param>
		/// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> that references the asynchronous send.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="remoteEP" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is less than 0.  
		/// -or-  
		/// <paramref name="offset" /> is greater than the length of <paramref name="buffer" />.  
		/// -or-  
		/// <paramref name="size" /> is less than 0.  
		/// -or-  
		/// <paramref name="size" /> is greater than the length of <paramref name="buffer" /> minus the value of the <paramref name="offset" /> parameter.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller higher in the call stack does not have permission for the requested operation.</exception>
		public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			this.ThrowIfBufferNull(buffer);
			this.ThrowIfBufferOutOfRange(buffer, offset, size);
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, callback, state, SocketOperation.SendTo)
			{
				Buffer = buffer,
				Offset = offset,
				Size = size,
				SockFlags = socketFlags,
				EndPoint = remoteEP
			};
			this.QueueIOSelectorJob(this.WriteSem, socketAsyncResult.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
			{
				Socket.BeginSendToCallback((SocketAsyncResult)s, 0);
			}, socketAsyncResult));
			return socketAsyncResult;
		}

		private static void BeginSendToCallback(SocketAsyncResult sockares, int sent_so_far)
		{
			try
			{
				int num = sockares.socket.SendTo(sockares.Buffer, sockares.Offset, sockares.Size, sockares.SockFlags, sockares.EndPoint);
				if (sockares.error == 0)
				{
					sent_so_far += num;
					sockares.Offset += num;
					sockares.Size -= num;
				}
				if (sockares.Size > 0)
				{
					IOSelector.Add(sockares.Handle, new IOSelectorJob(IOOperation.Write, delegate(IOAsyncResult s)
					{
						Socket.BeginSendToCallback((SocketAsyncResult)s, sent_so_far);
					}, sockares));
					return;
				}
				sockares.Total = sent_so_far;
			}
			catch (Exception e)
			{
				sockares.Complete(e);
				return;
			}
			sockares.Complete();
		}

		/// <summary>Ends a pending asynchronous send to a specific location.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that stores state information and any user defined data for this asynchronous operation.</param>
		/// <returns>If successful, the number of bytes sent; otherwise, an invalid <see cref="T:System.Net.Sockets.Socket" /> error.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginSendTo(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags,System.Net.EndPoint,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndSendTo(System.IAsyncResult)" /> was previously called for the asynchronous send.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public int EndSendTo(IAsyncResult asyncResult)
		{
			this.ThrowIfDisposedAndClosed();
			SocketAsyncResult socketAsyncResult = this.ValidateEndIAsyncResult(asyncResult, "EndSendTo", "result");
			if (!socketAsyncResult.IsCompleted)
			{
				socketAsyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		private unsafe static int SendTo_internal(SafeSocketHandle safeHandle, byte* buffer, int count, SocketFlags flags, SocketAddress sa, out int error, bool blocking)
		{
			int result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.SendTo_icall(safeHandle.DangerousGetHandle(), buffer, count, flags, sa, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int SendTo_icall(IntPtr sock, byte* buffer, int count, SocketFlags flags, SocketAddress sa, out int error, bool blocking);

		/// <summary>Sends the file <paramref name="fileName" /> and buffers of data to a connected <see cref="T:System.Net.Sockets.Socket" /> object using the specified <see cref="T:System.Net.Sockets.TransmitFileOptions" /> value.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> that contains the path and name of the file to be sent. This parameter can be <see langword="null" />.</param>
		/// <param name="preBuffer">A <see cref="T:System.Byte" /> array that contains data to be sent before the file is sent. This parameter can be <see langword="null" />.</param>
		/// <param name="postBuffer">A <see cref="T:System.Byte" /> array that contains data to be sent after the file is sent. This parameter can be <see langword="null" />.</param>
		/// <param name="flags">One or more of <see cref="T:System.Net.Sockets.TransmitFileOptions" /> values.</param>
		/// <exception cref="T:System.NotSupportedException">The operating system is not Windows NT or later.  
		/// -or-
		///  The socket is not connected to a remote host.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Net.Sockets.Socket" /> object is not in blocking mode and cannot accept this synchronous call.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file <paramref name="fileName" /> was not found.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_connected)
			{
				throw new NotSupportedException();
			}
			if (!this.is_blocking)
			{
				throw new InvalidOperationException();
			}
			int num = 0;
			if (Socket.SendFile_internal(this.m_Handle, fileName, preBuffer, postBuffer, flags, out num, this.is_blocking) && num == 0)
			{
				return;
			}
			SocketException ex = new SocketException(num);
			if (ex.ErrorCode == 2 || ex.ErrorCode == 3)
			{
				throw new FileNotFoundException();
			}
			throw ex;
		}

		/// <summary>Sends a file and buffers of data asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" /> object.</summary>
		/// <param name="fileName">A string that contains the path and name of the file to be sent. This parameter can be <see langword="null" />.</param>
		/// <param name="preBuffer">A <see cref="T:System.Byte" /> array that contains data to be sent before the file is sent. This parameter can be <see langword="null" />.</param>
		/// <param name="postBuffer">A <see cref="T:System.Byte" /> array that contains data to be sent after the file is sent. This parameter can be <see langword="null" />.</param>
		/// <param name="flags">A bitwise combination of <see cref="T:System.Net.Sockets.TransmitFileOptions" /> values.</param>
		/// <param name="callback">An <see cref="T:System.AsyncCallback" /> delegate to be invoked when this operation completes. This parameter can be <see langword="null" />.</param>
		/// <param name="state">A user-defined object that contains state information for this request. This parameter can be <see langword="null" />.</param>
		/// <returns>An <see cref="T:System.IAsyncResult" /> object that represents the asynchronous operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		/// <exception cref="T:System.NotSupportedException">The operating system is not Windows NT or later.  
		/// -or-
		///  The socket is not connected to a remote host.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file <paramref name="fileName" /> was not found.</exception>
		public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_connected)
			{
				throw new NotSupportedException();
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}
			Socket.SendFileHandler handler = new Socket.SendFileHandler(this.SendFile);
			return new Socket.SendFileAsyncResult(handler, handler.BeginInvoke(fileName, preBuffer, postBuffer, flags, delegate(IAsyncResult ar)
			{
				callback(new Socket.SendFileAsyncResult(handler, ar));
			}, state));
		}

		/// <summary>Ends a pending asynchronous send of a file.</summary>
		/// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> object that stores state information for this asynchronous operation.</param>
		/// <exception cref="T:System.NotSupportedException">Windows NT is required for this method.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult" /> is empty.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult" /> was not returned by a call to the <see cref="M:System.Net.Sockets.Socket.BeginSendFile(System.String,System.AsyncCallback,System.Object)" /> method.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Net.Sockets.Socket.EndSendFile(System.IAsyncResult)" /> was previously called for the asynchronous <see cref="M:System.Net.Sockets.Socket.BeginSendFile(System.String,System.AsyncCallback,System.Object)" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket. See remarks section below.</exception>
		public void EndSendFile(IAsyncResult asyncResult)
		{
			this.ThrowIfDisposedAndClosed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			Socket.SendFileAsyncResult sendFileAsyncResult = asyncResult as Socket.SendFileAsyncResult;
			if (sendFileAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			sendFileAsyncResult.Delegate.EndInvoke(sendFileAsyncResult.Original);
		}

		private static bool SendFile_internal(SafeSocketHandle safeHandle, string filename, byte[] pre_buffer, byte[] post_buffer, TransmitFileOptions flags, out int error, bool blocking)
		{
			bool result;
			try
			{
				safeHandle.RegisterForBlockingSyscall();
				result = Socket.SendFile_icall(safeHandle.DangerousGetHandle(), filename, pre_buffer, post_buffer, flags, out error, blocking);
			}
			finally
			{
				safeHandle.UnRegisterForBlockingSyscall();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendFile_icall(IntPtr sock, string filename, byte[] pre_buffer, byte[] post_buffer, TransmitFileOptions flags, out int error, bool blocking);

		/// <summary>Sends a collection of files or in memory data buffers asynchronously to a connected <see cref="T:System.Net.Sockets.Socket" /> object.</summary>
		/// <param name="e">The <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object to use for this asynchronous socket operation.</param>
		/// <returns>
		///   <see langword="true" /> if the I/O operation is pending. The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will be raised upon completion of the operation.  
		/// <see langword="false" /> if the I/O operation completed synchronously. In this case, The <see cref="E:System.Net.Sockets.SocketAsyncEventArgs.Completed" /> event on the <paramref name="e" /> parameter will not be raised and the <paramref name="e" /> object passed as a parameter may be examined immediately after the method call returns to retrieve the result of the operation.</returns>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified in the <see cref="P:System.Net.Sockets.SendPacketsElement.FilePath" /> property was not found.</exception>
		/// <exception cref="T:System.InvalidOperationException">A socket operation was already in progress using the <see cref="T:System.Net.Sockets.SocketAsyncEventArgs" /> object specified in the <paramref name="e" /> parameter.</exception>
		/// <exception cref="T:System.NotSupportedException">Windows XP or later is required for this method. This exception also occurs if the <see cref="T:System.Net.Sockets.Socket" /> is not connected to a remote host.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">A connectionless <see cref="T:System.Net.Sockets.Socket" /> is being used and the file being sent exceeds the maximum packet size of the underlying transport.</exception>
		[MonoTODO("Not implemented")]
		public bool SendPacketsAsync(SocketAsyncEventArgs e)
		{
			this.ThrowIfDisposedAndClosed();
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Duplicate_icall(IntPtr handle, int targetProcessId, out IntPtr duplicateHandle, out MonoIOError error);

		/// <summary>Duplicates the socket reference for the target process, and closes the socket for this process.</summary>
		/// <param name="targetProcessId">The ID of the target process where a duplicate of the socket reference is created.</param>
		/// <returns>The socket reference to be passed to the target process.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="targetProcessID" /> is not a valid process id.  
		/// -or-  
		/// Duplication of the socket reference failed.</exception>
		[MonoLimitation("We do not support passing sockets across processes, we merely allow this API to pass the socket across AppDomains")]
		public SocketInformation DuplicateAndClose(int targetProcessId)
		{
			SocketInformation result = default(SocketInformation);
			result.Options = ((this.is_listening ? SocketInformationOptions.Listening : ((SocketInformationOptions)0)) | (this.is_connected ? SocketInformationOptions.Connected : ((SocketInformationOptions)0)) | (this.is_blocking ? ((SocketInformationOptions)0) : SocketInformationOptions.NonBlocking) | (this.useOverlappedIO ? SocketInformationOptions.UseOnlyOverlappedIO : ((SocketInformationOptions)0)));
			IntPtr value;
			MonoIOError error;
			if (!Socket.Duplicate_icall(this.Handle, targetProcessId, out value, out error))
			{
				throw MonoIO.GetException(error);
			}
			result.ProtocolInformation = DataConverter.Pack("iiiil", new object[]
			{
				(int)this.addressFamily,
				(int)this.socketType,
				(int)this.protocolType,
				this.is_bound ? 1 : 0,
				(long)value
			});
			this.m_Handle = null;
			return result;
		}

		/// <summary>Returns the specified <see cref="T:System.Net.Sockets.Socket" /> option setting, represented as a byte array.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionValue">An array of type <see cref="T:System.Byte" /> that is to receive the option setting.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.  
		/// -or-
		///  In .NET Compact Framework applications, the Windows CE default buffer space is set to 32768 bytes. You can change the per socket buffer space by calling <see cref="Overload:System.Net.Sockets.Socket.SetSocketOption" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.ThrowIfDisposedAndClosed();
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int num;
			Socket.GetSocketOption_arr_internal(this.m_Handle, optionLevel, optionName, ref optionValue, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		/// <summary>Returns the value of the specified <see cref="T:System.Net.Sockets.Socket" /> option in an array.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionLength">The length, in bytes, of the expected return value.</param>
		/// <returns>An array of type <see cref="T:System.Byte" /> that contains the value of the socket option.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.  
		/// -or-
		///  In .NET Compact Framework applications, the Windows CE default buffer space is set to 32768 bytes. You can change the per socket buffer space by calling <see cref="Overload:System.Net.Sockets.Socket.SetSocketOption" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionLength)
		{
			this.ThrowIfDisposedAndClosed();
			byte[] result = new byte[optionLength];
			int num;
			Socket.GetSocketOption_arr_internal(this.m_Handle, optionLevel, optionName, ref result, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			return result;
		}

		/// <summary>Returns the value of a specified <see cref="T:System.Net.Sockets.Socket" /> option, represented as an object.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <returns>An object that represents the value of the option. When the <paramref name="optionName" /> parameter is set to <see cref="F:System.Net.Sockets.SocketOptionName.Linger" /> the return value is an instance of the <see cref="T:System.Net.Sockets.LingerOption" /> class. When <paramref name="optionName" /> is set to <see cref="F:System.Net.Sockets.SocketOptionName.AddMembership" /> or <see cref="F:System.Net.Sockets.SocketOptionName.DropMembership" />, the return value is an instance of the <see cref="T:System.Net.Sockets.MulticastOption" /> class. When <paramref name="optionName" /> is any other value, the return value is an integer.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.  
		///  -or-  
		///  <paramref name="optionName" /> was set to the unsupported value <see cref="F:System.Net.Sockets.SocketOptionName.MaxConnections" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			this.ThrowIfDisposedAndClosed();
			object obj;
			int num;
			Socket.GetSocketOption_obj_internal(this.m_Handle, optionLevel, optionName, out obj, out num);
			if (num != 0)
			{
				throw new SocketException(num);
			}
			if (optionName == SocketOptionName.Linger)
			{
				return (LingerOption)obj;
			}
			if (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership)
			{
				return (MulticastOption)obj;
			}
			if (obj is int)
			{
				return (int)obj;
			}
			return obj;
		}

		private static void GetSocketOption_arr_internal(SafeSocketHandle safeHandle, SocketOptionLevel level, SocketOptionName name, ref byte[] byte_val, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.GetSocketOption_arr_icall(safeHandle.DangerousGetHandle(), level, name, ref byte_val, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_arr_icall(IntPtr socket, SocketOptionLevel level, SocketOptionName name, ref byte[] byte_val, out int error);

		private static void GetSocketOption_obj_internal(SafeSocketHandle safeHandle, SocketOptionLevel level, SocketOptionName name, out object obj_val, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.GetSocketOption_obj_icall(safeHandle.DangerousGetHandle(), level, name, out obj_val, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_obj_icall(IntPtr socket, SocketOptionLevel level, SocketOptionName name, out object obj_val, out int error);

		/// <summary>Sets the specified <see cref="T:System.Net.Sockets.Socket" /> option to the specified value, represented as a byte array.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionValue">An array of type <see cref="T:System.Byte" /> that represents the value of the option.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			this.ThrowIfDisposedAndClosed();
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int num;
			Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, null, optionValue, 0, out num);
			if (num == 0)
			{
				return;
			}
			if (num == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num);
		}

		/// <summary>Sets the specified <see cref="T:System.Net.Sockets.Socket" /> option to the specified value, represented as an object.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionValue">A <see cref="T:System.Net.Sockets.LingerOption" /> or <see cref="T:System.Net.Sockets.MulticastOption" /> that contains the value of the option.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="optionValue" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
		{
			this.ThrowIfDisposedAndClosed();
			if (optionValue == null)
			{
				throw new ArgumentNullException("optionValue");
			}
			int num;
			if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
			{
				LingerOption lingerOption = optionValue as LingerOption;
				if (lingerOption == null)
				{
					throw new ArgumentException("A 'LingerOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, lingerOption, null, 0, out num);
			}
			else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
			{
				MulticastOption multicastOption = optionValue as MulticastOption;
				if (multicastOption == null)
				{
					throw new ArgumentException("A 'MulticastOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, multicastOption, null, 0, out num);
			}
			else
			{
				if (optionLevel != SocketOptionLevel.IPv6 || (optionName != SocketOptionName.AddMembership && optionName != SocketOptionName.DropMembership))
				{
					throw new ArgumentException("Invalid value specified.", "optionValue");
				}
				IPv6MulticastOption pv6MulticastOption = optionValue as IPv6MulticastOption;
				if (pv6MulticastOption == null)
				{
					throw new ArgumentException("A 'IPv6MulticastOption' value must be specified.", "optionValue");
				}
				Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, pv6MulticastOption, null, 0, out num);
			}
			if (num == 0)
			{
				return;
			}
			if (num == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num);
		}

		/// <summary>Sets the specified <see cref="T:System.Net.Sockets.Socket" /> option to the specified <see cref="T:System.Boolean" /> value.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionValue">The value of the option, represented as a <see cref="T:System.Boolean" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> object has been closed.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			int optionValue2 = optionValue ? 1 : 0;
			this.SetSocketOption(optionLevel, optionName, optionValue2);
		}

		/// <summary>Sets the specified <see cref="T:System.Net.Sockets.Socket" /> option to the specified integer value.</summary>
		/// <param name="optionLevel">One of the <see cref="T:System.Net.Sockets.SocketOptionLevel" /> values.</param>
		/// <param name="optionName">One of the <see cref="T:System.Net.Sockets.SocketOptionName" /> values.</param>
		/// <param name="optionValue">A value of the option.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
		{
			this.ThrowIfDisposedAndClosed();
			int num;
			Socket.SetSocketOption_internal(this.m_Handle, optionLevel, optionName, null, null, optionValue, out num);
			if (num == 0)
			{
				return;
			}
			if (num == 10022)
			{
				throw new ArgumentException();
			}
			throw new SocketException(num);
		}

		private static void SetSocketOption_internal(SafeSocketHandle safeHandle, SocketOptionLevel level, SocketOptionName name, object obj_val, byte[] byte_val, int int_val, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.SetSocketOption_icall(safeHandle.DangerousGetHandle(), level, name, obj_val, byte_val, int_val, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSocketOption_icall(IntPtr socket, SocketOptionLevel level, SocketOptionName name, object obj_val, byte[] byte_val, int int_val, out int error);

		/// <summary>Sets low-level operating modes for the <see cref="T:System.Net.Sockets.Socket" /> using numerical control codes.</summary>
		/// <param name="ioControlCode">An <see cref="T:System.Int32" /> value that specifies the control code of the operation to perform.</param>
		/// <param name="optionInValue">A <see cref="T:System.Byte" /> array that contains the input data required by the operation.</param>
		/// <param name="optionOutValue">A <see cref="T:System.Byte" /> array that contains the output data returned by the operation.</param>
		/// <returns>The number of bytes in the <paramref name="optionOutValue" /> parameter.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		/// <exception cref="T:System.InvalidOperationException">An attempt was made to change the blocking mode without using the <see cref="P:System.Net.Sockets.Socket.Blocking" /> property.</exception>
		/// <exception cref="T:System.Security.SecurityException">A caller in the call stack does not have the required permissions.</exception>
		public int IOControl(int ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			if (this.CleanedUp)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			int num2;
			int num = Socket.IOControl_internal(this.m_Handle, ioControlCode, optionInValue, optionOutValue, out num2);
			if (num2 != 0)
			{
				throw new SocketException(num2);
			}
			if (num == -1)
			{
				throw new InvalidOperationException("Must use Blocking property instead.");
			}
			return num;
		}

		private static int IOControl_internal(SafeSocketHandle safeHandle, int ioctl_code, byte[] input, byte[] output, out int error)
		{
			bool flag = false;
			int result;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				result = Socket.IOControl_icall(safeHandle.DangerousGetHandle(), ioctl_code, input, output, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IOControl_icall(IntPtr sock, int ioctl_code, byte[] input, byte[] output, out int error);

		/// <summary>Closes the <see cref="T:System.Net.Sockets.Socket" /> connection and releases all associated resources.</summary>
		public void Close()
		{
			this.linger_timeout = 0;
			this.Dispose();
		}

		/// <summary>Closes the <see cref="T:System.Net.Sockets.Socket" /> connection and releases all associated resources with a specified timeout to allow queued data to be sent.</summary>
		/// <param name="timeout">Wait up to <paramref name="timeout" /> seconds to send any remaining data, then close the socket.</param>
		public void Close(int timeout)
		{
			this.linger_timeout = timeout;
			this.Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Close_icall(IntPtr socket, out int error);

		/// <summary>Disables sends and receives on a <see cref="T:System.Net.Sockets.Socket" />.</summary>
		/// <param name="how">One of the <see cref="T:System.Net.Sockets.SocketShutdown" /> values that specifies the operation that will no longer be allowed.</param>
		/// <exception cref="T:System.Net.Sockets.SocketException">An error occurred when attempting to access the socket.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Net.Sockets.Socket" /> has been closed.</exception>
		public void Shutdown(SocketShutdown how)
		{
			this.ThrowIfDisposedAndClosed();
			if (!this.is_connected)
			{
				throw new SocketException(10057);
			}
			int num;
			Socket.Shutdown_internal(this.m_Handle, how, out num);
			if (num == 10057)
			{
				return;
			}
			if (num != 0)
			{
				throw new SocketException(num);
			}
		}

		private static void Shutdown_internal(SafeSocketHandle safeHandle, SocketShutdown how, out int error)
		{
			bool flag = false;
			try
			{
				safeHandle.DangerousAddRef(ref flag);
				Socket.Shutdown_icall(safeHandle.DangerousGetHandle(), how, out error);
			}
			finally
			{
				if (flag)
				{
					safeHandle.DangerousRelease();
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Shutdown_icall(IntPtr socket, SocketShutdown how, out int error);

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Sockets.Socket" />, and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.CleanedUp)
			{
				return;
			}
			this.m_IntCleanedUp = 1;
			bool flag = this.is_connected;
			this.is_connected = false;
			if (this.m_Handle != null)
			{
				this.is_closed = true;
				IntPtr handle = this.Handle;
				if (flag)
				{
					this.Linger(handle);
				}
				this.m_Handle.Dispose();
			}
		}

		private void Linger(IntPtr handle)
		{
			if (!this.is_connected || this.linger_timeout <= 0)
			{
				return;
			}
			int num;
			Socket.Shutdown_icall(handle, SocketShutdown.Receive, out num);
			if (num != 0)
			{
				return;
			}
			int num2 = this.linger_timeout / 1000;
			int num3 = this.linger_timeout % 1000;
			if (num3 > 0)
			{
				Socket.Poll_icall(handle, SelectMode.SelectRead, num3 * 1000, out num);
				if (num != 0)
				{
					return;
				}
			}
			if (num2 > 0)
			{
				LingerOption obj_val = new LingerOption(true, num2);
				Socket.SetSocketOption_icall(handle, SocketOptionLevel.Socket, SocketOptionName.Linger, obj_val, null, 0, out num);
			}
		}

		private void ThrowIfDisposedAndClosed(Socket socket)
		{
			if (socket.CleanedUp && socket.is_closed)
			{
				throw new ObjectDisposedException(socket.GetType().ToString());
			}
		}

		private void ThrowIfDisposedAndClosed()
		{
			if (this.CleanedUp && this.is_closed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void ThrowIfBufferNull(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
		}

		private void ThrowIfBufferOutOfRange(byte[] buffer, int offset, int size)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be <= buffer.Length");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("size", "size must be <= buffer.Length - offset");
			}
		}

		private void ThrowIfUdp()
		{
			if (this.protocolType == ProtocolType.Udp)
			{
				throw new SocketException(10042);
			}
		}

		private SocketAsyncResult ValidateEndIAsyncResult(IAsyncResult ares, string methodName, string argName)
		{
			if (ares == null)
			{
				throw new ArgumentNullException(argName);
			}
			SocketAsyncResult socketAsyncResult = ares as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", argName);
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw new InvalidOperationException(methodName + " can only be called once per asynchronous operation");
			}
			return socketAsyncResult;
		}

		private void QueueIOSelectorJob(SemaphoreSlim sem, IntPtr handle, IOSelectorJob job)
		{
			Task task = sem.WaitAsync();
			if (!task.IsCompleted)
			{
				task.ContinueWith(delegate(Task t)
				{
					if (this.CleanedUp)
					{
						job.MarkDisposed();
						return;
					}
					IOSelector.Add(handle, job);
				});
				return;
			}
			if (this.CleanedUp)
			{
				job.MarkDisposed();
				return;
			}
			IOSelector.Add(handle, job);
		}

		private void InitSocketAsyncEventArgs(SocketAsyncEventArgs e, AsyncCallback callback, object state, SocketOperation operation)
		{
			e.socket_async_result.Init(this, callback, state, operation);
			if (e.AcceptSocket != null)
			{
				e.socket_async_result.AcceptSocket = e.AcceptSocket;
			}
			e.SetCurrentSocket(this);
			e.SetLastOperation(this.SocketOperationToSocketAsyncOperation(operation));
			e.SocketError = SocketError.Success;
			e.SetBytesTransferred(0);
		}

		private SocketAsyncOperation SocketOperationToSocketAsyncOperation(SocketOperation op)
		{
			switch (op)
			{
			case SocketOperation.Accept:
				return SocketAsyncOperation.Accept;
			case SocketOperation.Connect:
				return SocketAsyncOperation.Connect;
			case SocketOperation.Receive:
			case SocketOperation.ReceiveGeneric:
				return SocketAsyncOperation.Receive;
			case SocketOperation.ReceiveFrom:
				return SocketAsyncOperation.ReceiveFrom;
			case SocketOperation.Send:
			case SocketOperation.SendGeneric:
				return SocketAsyncOperation.Send;
			case SocketOperation.SendTo:
				return SocketAsyncOperation.SendTo;
			case SocketOperation.Disconnect:
				return SocketAsyncOperation.Disconnect;
			}
			throw new NotImplementedException(string.Format("Operation {0} is not implemented", op));
		}

		private IPEndPoint RemapIPEndPoint(IPEndPoint input)
		{
			if (this.IsDualMode && input.AddressFamily == AddressFamily.InterNetwork)
			{
				return new IPEndPoint(input.Address.MapToIPv6(), input.Port);
			}
			return input;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void cancel_blocking_socket_operation(Thread thread);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool SupportsPortReuse(ProtocolType proto);

		internal static int FamilyHint
		{
			get
			{
				int num = 0;
				if (Socket.OSSupportsIPv4)
				{
					num = 1;
				}
				if (Socket.OSSupportsIPv6)
				{
					num = ((num == 0) ? 2 : 0);
				}
				return num;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsProtocolSupported_internal(NetworkInterfaceComponent networkInterface);

		private static bool IsProtocolSupported(NetworkInterfaceComponent networkInterface)
		{
			return Socket.IsProtocolSupported_internal(networkInterface);
		}

		internal void ReplaceHandleIfNecessaryAfterFailedConnect()
		{
		}

		private static readonly EventHandler<SocketAsyncEventArgs> AcceptCompletedHandler = delegate(object s, SocketAsyncEventArgs e)
		{
			Socket.CompleteAccept((Socket)s, (Socket.TaskSocketAsyncEventArgs<Socket>)e);
		};

		private static readonly EventHandler<SocketAsyncEventArgs> ReceiveCompletedHandler = delegate(object s, SocketAsyncEventArgs e)
		{
			Socket.CompleteSendReceive((Socket)s, (Socket.Int32TaskSocketAsyncEventArgs)e, true);
		};

		private static readonly EventHandler<SocketAsyncEventArgs> SendCompletedHandler = delegate(object s, SocketAsyncEventArgs e)
		{
			Socket.CompleteSendReceive((Socket)s, (Socket.Int32TaskSocketAsyncEventArgs)e, false);
		};

		private static readonly Socket.TaskSocketAsyncEventArgs<Socket> s_rentedSocketSentinel = new Socket.TaskSocketAsyncEventArgs<Socket>();

		private static readonly Socket.Int32TaskSocketAsyncEventArgs s_rentedInt32Sentinel = new Socket.Int32TaskSocketAsyncEventArgs();

		private static readonly Task<int> s_zeroTask = Task.FromResult<int>(0);

		private Socket.CachedEventArgs _cachedTaskEventArgs;

		private static object s_InternalSyncObject;

		internal static volatile bool s_SupportsIPv4;

		internal static volatile bool s_SupportsIPv6;

		internal static volatile bool s_OSSupportsIPv6;

		internal static volatile bool s_Initialized;

		private static volatile bool s_LoggingEnabled;

		internal static volatile bool s_PerfCountersEnabled;

		internal const int DefaultCloseTimeout = -1;

		private const int SOCKET_CLOSED_CODE = 10004;

		private const string TIMEOUT_EXCEPTION_MSG = "A connection attempt failed because the connected party did not properly respondafter a period of time, or established connection failed because connected host has failed to respond";

		private bool is_closed;

		private bool is_listening;

		private bool useOverlappedIO;

		private int linger_timeout;

		private AddressFamily addressFamily;

		private SocketType socketType;

		private ProtocolType protocolType;

		internal SafeSocketHandle m_Handle;

		internal EndPoint seed_endpoint;

		internal SemaphoreSlim ReadSem;

		internal SemaphoreSlim WriteSem;

		internal bool is_blocking;

		internal bool is_bound;

		internal bool is_connected;

		private int m_IntCleanedUp;

		internal bool connect_in_progress;

		internal readonly int ID;

		private static AsyncCallback AcceptAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.AcceptSocket = socketAsyncEventArgs.CurrentSocket.EndAccept(ares);
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				if (socketAsyncEventArgs.AcceptSocket == null)
				{
					socketAsyncEventArgs.AcceptSocket = new Socket(socketAsyncEventArgs.CurrentSocket.AddressFamily, socketAsyncEventArgs.CurrentSocket.SocketType, socketAsyncEventArgs.CurrentSocket.ProtocolType, null);
				}
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginAcceptCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			Socket socket = null;
			try
			{
				if (socketAsyncResult.AcceptSocket == null)
				{
					socket = socketAsyncResult.socket.Accept();
				}
				else
				{
					socket = socketAsyncResult.AcceptSocket;
					socketAsyncResult.socket.Accept(socket);
				}
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete(socket);
		};

		private static IOAsyncCallback BeginAcceptReceiveCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			Socket socket = null;
			try
			{
				if (socketAsyncResult.AcceptSocket == null)
				{
					socket = socketAsyncResult.socket.Accept();
				}
				else
				{
					socket = socketAsyncResult.AcceptSocket;
					socketAsyncResult.socket.Accept(socket);
				}
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			int total = 0;
			if (socketAsyncResult.Size > 0)
			{
				try
				{
					SocketError socketError;
					total = socket.Receive(socketAsyncResult.Buffer, socketAsyncResult.Offset, socketAsyncResult.Size, socketAsyncResult.SockFlags, out socketError);
					if (socketError != SocketError.Success)
					{
						socketAsyncResult.Complete(new SocketException((int)socketError));
						return;
					}
				}
				catch (Exception e2)
				{
					socketAsyncResult.Complete(e2);
					return;
				}
			}
			socketAsyncResult.Complete(socket, total);
		};

		private static AsyncCallback ConnectAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.CurrentSocket.EndConnect(ares);
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginConnectCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			if (socketAsyncResult.EndPoint == null)
			{
				socketAsyncResult.Complete(new SocketException(10049));
				return;
			}
			try
			{
				int num = (int)socketAsyncResult.socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
				if (num == 0)
				{
					socketAsyncResult.socket.seed_endpoint = socketAsyncResult.EndPoint;
					socketAsyncResult.socket.is_connected = true;
					socketAsyncResult.socket.is_bound = true;
					socketAsyncResult.socket.connect_in_progress = false;
					socketAsyncResult.error = 0;
					socketAsyncResult.Complete();
				}
				else if (socketAsyncResult.Addresses == null)
				{
					socketAsyncResult.socket.connect_in_progress = false;
					socketAsyncResult.Complete(new SocketException(num));
				}
				else if (socketAsyncResult.CurrentAddress >= socketAsyncResult.Addresses.Length)
				{
					socketAsyncResult.Complete(new SocketException(num));
				}
				else
				{
					Socket.BeginMConnect(socketAsyncResult);
				}
			}
			catch (Exception e)
			{
				socketAsyncResult.socket.connect_in_progress = false;
				socketAsyncResult.Complete(e);
			}
		};

		private static AsyncCallback DisconnectAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.CurrentSocket.EndDisconnect(ares);
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginDisconnectCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			try
			{
				socketAsyncResult.socket.Disconnect(socketAsyncResult.ReuseSocket);
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete();
		};

		private static AsyncCallback ReceiveAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.SetBytesTransferred(socketAsyncEventArgs.CurrentSocket.EndReceive(ares));
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginReceiveCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			int total = 0;
			try
			{
				using (MemoryHandle memoryHandle = socketAsyncResult.Buffer.Slice(socketAsyncResult.Offset, socketAsyncResult.Size).Pin())
				{
					total = Socket.Receive_internal(socketAsyncResult.socket.m_Handle, (byte*)memoryHandle.Pointer, socketAsyncResult.Size, socketAsyncResult.SockFlags, out socketAsyncResult.error, socketAsyncResult.socket.is_blocking);
				}
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete(total);
		};

		private static IOAsyncCallback BeginReceiveGenericCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			int total = 0;
			try
			{
				total = socketAsyncResult.socket.Receive(socketAsyncResult.Buffers, socketAsyncResult.SockFlags);
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete(total);
		};

		private static AsyncCallback ReceiveFromAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.SetBytesTransferred(socketAsyncEventArgs.CurrentSocket.EndReceiveFrom_internal((SocketAsyncResult)ares, socketAsyncEventArgs));
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginReceiveFromCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			int total = 0;
			try
			{
				SocketError socketError;
				total = socketAsyncResult.socket.ReceiveFrom(socketAsyncResult.Buffer, socketAsyncResult.Offset, socketAsyncResult.Size, socketAsyncResult.SockFlags, ref socketAsyncResult.EndPoint, out socketError);
				if (socketError != SocketError.Success)
				{
					socketAsyncResult.Complete(new SocketException(socketError));
					return;
				}
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete(total);
		};

		private static AsyncCallback SendAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.SetBytesTransferred(socketAsyncEventArgs.CurrentSocket.EndSend(ares));
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private static IOAsyncCallback BeginSendGenericCallback = delegate(IOAsyncResult ares)
		{
			SocketAsyncResult socketAsyncResult = (SocketAsyncResult)ares;
			int total = 0;
			try
			{
				total = socketAsyncResult.socket.Send(socketAsyncResult.Buffers, socketAsyncResult.SockFlags);
			}
			catch (Exception e)
			{
				socketAsyncResult.Complete(e);
				return;
			}
			socketAsyncResult.Complete(total);
		};

		private static AsyncCallback SendToAsyncCallback = delegate(IAsyncResult ares)
		{
			SocketAsyncEventArgs socketAsyncEventArgs = (SocketAsyncEventArgs)((SocketAsyncResult)ares).AsyncState;
			if (Interlocked.Exchange(ref socketAsyncEventArgs.in_progress, 0) != 1)
			{
				throw new InvalidOperationException("No operation in progress");
			}
			try
			{
				socketAsyncEventArgs.SetBytesTransferred(socketAsyncEventArgs.CurrentSocket.EndSendTo(ares));
			}
			catch (SocketException ex)
			{
				socketAsyncEventArgs.SocketError = ex.SocketErrorCode;
			}
			catch (ObjectDisposedException)
			{
				socketAsyncEventArgs.SocketError = SocketError.OperationAborted;
			}
			finally
			{
				socketAsyncEventArgs.Complete_internal();
			}
		};

		private class StateTaskCompletionSource<TField1, TResult> : TaskCompletionSource<TResult>
		{
			public StateTaskCompletionSource(object baseState) : base(baseState)
			{
			}

			internal TField1 _field1;
		}

		private class StateTaskCompletionSource<TField1, TField2, TResult> : Socket.StateTaskCompletionSource<TField1, TResult>
		{
			public StateTaskCompletionSource(object baseState) : base(baseState)
			{
			}

			internal TField2 _field2;
		}

		private sealed class CachedEventArgs
		{
			public Socket.TaskSocketAsyncEventArgs<Socket> TaskAccept;

			public Socket.Int32TaskSocketAsyncEventArgs TaskReceive;

			public Socket.Int32TaskSocketAsyncEventArgs TaskSend;

			public Socket.AwaitableSocketAsyncEventArgs ValueTaskReceive;

			public Socket.AwaitableSocketAsyncEventArgs ValueTaskSend;
		}

		private class TaskSocketAsyncEventArgs<TResult> : SocketAsyncEventArgs
		{
			internal TaskSocketAsyncEventArgs() : base(false)
			{
			}

			internal AsyncTaskMethodBuilder<TResult> GetCompletionResponsibility(out bool responsibleForReturningToPool)
			{
				AsyncTaskMethodBuilder<TResult> builder;
				lock (this)
				{
					responsibleForReturningToPool = this._accessed;
					this._accessed = true;
					Task<TResult> task = this._builder.Task;
					builder = this._builder;
				}
				return builder;
			}

			internal AsyncTaskMethodBuilder<TResult> _builder;

			internal bool _accessed;
		}

		private sealed class Int32TaskSocketAsyncEventArgs : Socket.TaskSocketAsyncEventArgs<int>
		{
			internal bool _wrapExceptionsInIOExceptions;
		}

		internal sealed class AwaitableSocketAsyncEventArgs : SocketAsyncEventArgs, IValueTaskSource, IValueTaskSource<int>
		{
			public AwaitableSocketAsyncEventArgs() : base(false)
			{
			}

			public bool WrapExceptionsInIOExceptions { get; set; }

			public bool Reserve()
			{
				return Interlocked.CompareExchange<Action<object>>(ref this._continuation, null, Socket.AwaitableSocketAsyncEventArgs.s_availableSentinel) == Socket.AwaitableSocketAsyncEventArgs.s_availableSentinel;
			}

			private void Release()
			{
				this._token += 1;
				Volatile.Write<Action<object>>(ref this._continuation, Socket.AwaitableSocketAsyncEventArgs.s_availableSentinel);
			}

			protected override void OnCompleted(SocketAsyncEventArgs _)
			{
				Action<object> action = this._continuation;
				if (action != null || (action = Interlocked.CompareExchange<Action<object>>(ref this._continuation, Socket.AwaitableSocketAsyncEventArgs.s_completedSentinel, null)) != null)
				{
					object userToken = base.UserToken;
					base.UserToken = null;
					this._continuation = Socket.AwaitableSocketAsyncEventArgs.s_completedSentinel;
					ExecutionContext executionContext = this._executionContext;
					if (executionContext == null)
					{
						this.InvokeContinuation(action, userToken, false);
						return;
					}
					this._executionContext = null;
					ExecutionContext.Run(executionContext, delegate(object runState)
					{
						Tuple<Socket.AwaitableSocketAsyncEventArgs, Action<object>, object> tuple = (Tuple<Socket.AwaitableSocketAsyncEventArgs, Action<object>, object>)runState;
						tuple.Item1.InvokeContinuation(tuple.Item2, tuple.Item3, false);
					}, Tuple.Create<Socket.AwaitableSocketAsyncEventArgs, Action<object>, object>(this, action, userToken));
				}
			}

			public ValueTask<int> ReceiveAsync(Socket socket)
			{
				if (socket.ReceiveAsync(this))
				{
					return new ValueTask<int>(this, this._token);
				}
				int bytesTransferred = base.BytesTransferred;
				SocketError socketError = base.SocketError;
				this.Release();
				if (socketError != SocketError.Success)
				{
					return new ValueTask<int>(Task.FromException<int>(this.CreateException(socketError)));
				}
				return new ValueTask<int>(bytesTransferred);
			}

			public ValueTask<int> SendAsync(Socket socket)
			{
				if (socket.SendAsync(this))
				{
					return new ValueTask<int>(this, this._token);
				}
				int bytesTransferred = base.BytesTransferred;
				SocketError socketError = base.SocketError;
				this.Release();
				if (socketError != SocketError.Success)
				{
					return new ValueTask<int>(Task.FromException<int>(this.CreateException(socketError)));
				}
				return new ValueTask<int>(bytesTransferred);
			}

			public ValueTask SendAsyncForNetworkStream(Socket socket)
			{
				if (socket.SendAsync(this))
				{
					return new ValueTask(this, this._token);
				}
				SocketError socketError = base.SocketError;
				this.Release();
				if (socketError != SocketError.Success)
				{
					return new ValueTask(Task.FromException(this.CreateException(socketError)));
				}
				return default(ValueTask);
			}

			public ValueTaskSourceStatus GetStatus(short token)
			{
				if (token != this._token)
				{
					this.ThrowIncorrectTokenException();
				}
				if (this._continuation != Socket.AwaitableSocketAsyncEventArgs.s_completedSentinel)
				{
					return ValueTaskSourceStatus.Pending;
				}
				if (base.SocketError != SocketError.Success)
				{
					return ValueTaskSourceStatus.Faulted;
				}
				return ValueTaskSourceStatus.Succeeded;
			}

			public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
			{
				if (token != this._token)
				{
					this.ThrowIncorrectTokenException();
				}
				if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != ValueTaskSourceOnCompletedFlags.None)
				{
					this._executionContext = ExecutionContext.Capture();
				}
				if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != ValueTaskSourceOnCompletedFlags.None)
				{
					SynchronizationContext synchronizationContext = SynchronizationContext.Current;
					if (synchronizationContext != null && synchronizationContext.GetType() != typeof(SynchronizationContext))
					{
						this._scheduler = synchronizationContext;
					}
					else
					{
						TaskScheduler taskScheduler = TaskScheduler.Current;
						if (taskScheduler != TaskScheduler.Default)
						{
							this._scheduler = taskScheduler;
						}
					}
				}
				base.UserToken = state;
				Action<object> action = Interlocked.CompareExchange<Action<object>>(ref this._continuation, continuation, null);
				if (action == Socket.AwaitableSocketAsyncEventArgs.s_completedSentinel)
				{
					this._executionContext = null;
					base.UserToken = null;
					this.InvokeContinuation(continuation, state, true);
					return;
				}
				if (action != null)
				{
					this.ThrowMultipleContinuationsException();
				}
			}

			private void InvokeContinuation(Action<object> continuation, object state, bool forceAsync)
			{
				object scheduler = this._scheduler;
				this._scheduler = null;
				if (scheduler != null)
				{
					SynchronizationContext synchronizationContext = scheduler as SynchronizationContext;
					if (synchronizationContext != null)
					{
						synchronizationContext.Post(delegate(object s)
						{
							Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>)s;
							tuple.Item1(tuple.Item2);
						}, Tuple.Create<Action<object>, object>(continuation, state));
						return;
					}
					Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, (TaskScheduler)scheduler);
					return;
				}
				else
				{
					if (forceAsync)
					{
						ThreadPool.QueueUserWorkItem<object>(continuation, state, true);
						return;
					}
					continuation(state);
					return;
				}
			}

			public int GetResult(short token)
			{
				if (token != this._token)
				{
					this.ThrowIncorrectTokenException();
				}
				SocketError socketError = base.SocketError;
				int bytesTransferred = base.BytesTransferred;
				this.Release();
				if (socketError != SocketError.Success)
				{
					this.ThrowException(socketError);
				}
				return bytesTransferred;
			}

			void IValueTaskSource.GetResult(short token)
			{
				if (token != this._token)
				{
					this.ThrowIncorrectTokenException();
				}
				SocketError socketError = base.SocketError;
				this.Release();
				if (socketError != SocketError.Success)
				{
					this.ThrowException(socketError);
				}
			}

			private void ThrowIncorrectTokenException()
			{
				throw new InvalidOperationException("The result of the operation was already consumed and may not be used again.");
			}

			private void ThrowMultipleContinuationsException()
			{
				throw new InvalidOperationException("Another continuation was already registered.");
			}

			private void ThrowException(SocketError error)
			{
				throw this.CreateException(error);
			}

			private Exception CreateException(SocketError error)
			{
				SocketException ex = new SocketException((int)error);
				if (!this.WrapExceptionsInIOExceptions)
				{
					return ex;
				}
				return new IOException(SR.Format("Unable to read data from the transport connection: {0}.", ex.Message), ex);
			}

			internal static readonly Socket.AwaitableSocketAsyncEventArgs Reserved = new Socket.AwaitableSocketAsyncEventArgs
			{
				_continuation = null
			};

			private static readonly Action<object> s_completedSentinel = delegate(object state)
			{
				throw new Exception("s_completedSentinel");
			};

			private static readonly Action<object> s_availableSentinel = delegate(object state)
			{
				throw new Exception("s_availableSentinel");
			};

			private Action<object> _continuation = Socket.AwaitableSocketAsyncEventArgs.s_availableSentinel;

			private ExecutionContext _executionContext;

			private object _scheduler;

			private short _token;
		}

		private delegate void SendFileHandler(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags);

		private sealed class SendFileAsyncResult : IAsyncResult
		{
			public SendFileAsyncResult(Socket.SendFileHandler d, IAsyncResult ares)
			{
				this.d = d;
				this.ares = ares;
			}

			public object AsyncState
			{
				get
				{
					return this.ares.AsyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return this.ares.AsyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return this.ares.CompletedSynchronously;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.ares.IsCompleted;
				}
			}

			public Socket.SendFileHandler Delegate
			{
				get
				{
					return this.d;
				}
			}

			public IAsyncResult Original
			{
				get
				{
					return this.ares;
				}
			}

			private IAsyncResult ares;

			private Socket.SendFileHandler d;
		}

		private struct WSABUF
		{
			public int len;

			public IntPtr buf;
		}
	}
}
