using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets
{
	/// <summary>This class contains extension methods to the <see cref="T:System.Net.Sockets.Socket" /> class.</summary>
	public static class SocketTaskExtensions
	{
		/// <summary>Performs an asynchronous operation on to accept an incoming connection attempt on the socket.</summary>
		/// <param name="socket">The socket that is listening for connections.</param>
		/// <returns>An asynchronous task that completes with a <see cref="T:System.Net.Sockets.Socket" /> to handle communication with the remote host.</returns>
		public static Task<Socket> AcceptAsync(this Socket socket)
		{
			return Task<Socket>.Factory.FromAsync((AsyncCallback callback, object state) => ((Socket)state).BeginAccept(callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndAccept(asyncResult), socket);
		}

		/// <summary>Performs an asynchronous operation on to accept an incoming connection attempt on the socket.</summary>
		/// <param name="socket">The socket that is listening for incoming connections.</param>
		/// <param name="acceptSocket">The accepted <see cref="T:System.Net.Sockets.Socket" /> object. This value may be <see langword="null" />.</param>
		/// <returns>An asynchronous task that completes with a <see cref="T:System.Net.Sockets.Socket" /> to handle communication with the remote host.</returns>
		public static Task<Socket> AcceptAsync(this Socket socket, Socket acceptSocket)
		{
			return Task<Socket>.Factory.FromAsync<Socket, int>((Socket socketForAccept, int receiveSize, AsyncCallback callback, object state) => ((Socket)state).BeginAccept(socketForAccept, receiveSize, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndAccept(asyncResult), acceptSocket, 0, socket);
		}

		/// <summary>Establishes a connection to a remote host.</summary>
		/// <param name="socket">The socket that is used for establishing a connection.</param>
		/// <param name="remoteEP">An EndPoint that represents the remote device.</param>
		/// <returns>An asynchronous Task.</returns>
		public static Task ConnectAsync(this Socket socket, EndPoint remoteEP)
		{
			return Task.Factory.FromAsync<EndPoint>((EndPoint targetEndPoint, AsyncCallback callback, object state) => ((Socket)state).BeginConnect(targetEndPoint, callback, state), delegate(IAsyncResult asyncResult)
			{
				((Socket)asyncResult.AsyncState).EndConnect(asyncResult);
			}, remoteEP, socket);
		}

		/// <summary>Establishes a connection to a remote host. The host is specified by an IP address and a port number.</summary>
		/// <param name="socket">The socket to perform the connect operation on.</param>
		/// <param name="address">The IP address of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		public static Task ConnectAsync(this Socket socket, IPAddress address, int port)
		{
			return Task.Factory.FromAsync<IPAddress, int>((IPAddress targetAddress, int targetPort, AsyncCallback callback, object state) => ((Socket)state).BeginConnect(targetAddress, targetPort, callback, state), delegate(IAsyncResult asyncResult)
			{
				((Socket)asyncResult.AsyncState).EndConnect(asyncResult);
			}, address, port, socket);
		}

		/// <summary>Establishes a connection to a remote host. The host is specified by an array of IP addresses and a port number.</summary>
		/// <param name="socket">The socket that the connect operation is performed on.</param>
		/// <param name="addresses">The IP addresses of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <returns>A task that represents the asynchronous connect operation.</returns>
		public static Task ConnectAsync(this Socket socket, IPAddress[] addresses, int port)
		{
			return Task.Factory.FromAsync<IPAddress[], int>((IPAddress[] targetAddresses, int targetPort, AsyncCallback callback, object state) => ((Socket)state).BeginConnect(targetAddresses, targetPort, callback, state), delegate(IAsyncResult asyncResult)
			{
				((Socket)asyncResult.AsyncState).EndConnect(asyncResult);
			}, addresses, port, socket);
		}

		/// <summary>Establishes a connection to a remote host. The host is specified by a host name and a port number.</summary>
		/// <param name="socket">The socket to perform the connect operation on.</param>
		/// <param name="host">The name of the remote host.</param>
		/// <param name="port">The port number of the remote host.</param>
		/// <returns>An asynchronous task.</returns>
		public static Task ConnectAsync(this Socket socket, string host, int port)
		{
			return Task.Factory.FromAsync<string, int>((string targetHost, int targetPort, AsyncCallback callback, object state) => ((Socket)state).BeginConnect(targetHost, targetPort, callback, state), delegate(IAsyncResult asyncResult)
			{
				((Socket)asyncResult.AsyncState).EndConnect(asyncResult);
			}, host, port, socket);
		}

		/// <summary>Receives data from a connected socket.</summary>
		/// <param name="socket">The socket to perform the receive operation on.</param>
		/// <param name="buffer">An array that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>A task that represents the asynchronous receive operation. The value of the <paramref name="TResult" /> parameter contains the number of bytes received.</returns>
		public static Task<int> ReceiveAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags)
		{
			return Task<int>.Factory.FromAsync<ArraySegment<byte>, SocketFlags>((ArraySegment<byte> targetBuffer, SocketFlags flags, AsyncCallback callback, object state) => ((Socket)state).BeginReceive(targetBuffer.Array, targetBuffer.Offset, targetBuffer.Count, flags, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndReceive(asyncResult), buffer, socketFlags, socket);
		}

		/// <summary>Receives data from a connected socket.</summary>
		/// <param name="socket">The socket to perform the receive operation on.</param>
		/// <param name="buffers">An array that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>A task that represents the asynchronous receive operation. The value of the <paramref name="TResult" /> parameter contains the number of bytes received.</returns>
		public static Task<int> ReceiveAsync(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return Task<int>.Factory.FromAsync<IList<ArraySegment<byte>>, SocketFlags>((IList<ArraySegment<byte>> targetBuffers, SocketFlags flags, AsyncCallback callback, object state) => ((Socket)state).BeginReceive(targetBuffers, flags, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndReceive(asyncResult), buffers, socketFlags, socket);
		}

		/// <summary>Receives data from a specified network device.</summary>
		/// <param name="socket">The socket to perform the ReceiveFrom operation on.</param>
		/// <param name="buffer">An array of type Byte that is the storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEndPoint">An EndPoint that represents the source of the data.</param>
		/// <returns>An asynchronous Task that completes with a SocketReceiveFromResult struct.</returns>
		public static Task<SocketReceiveFromResult> ReceiveFromAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint)
		{
			object[] state2 = new object[]
			{
				socket,
				remoteEndPoint
			};
			return Task<SocketReceiveFromResult>.Factory.FromAsync<ArraySegment<byte>, SocketFlags>(delegate(ArraySegment<byte> targetBuffer, SocketFlags flags, AsyncCallback callback, object state)
			{
				object[] array = (object[])state;
				Socket socket2 = (Socket)array[0];
				EndPoint endPoint = (EndPoint)array[1];
				IAsyncResult result = socket2.BeginReceiveFrom(targetBuffer.Array, targetBuffer.Offset, targetBuffer.Count, flags, ref endPoint, callback, state);
				array[1] = endPoint;
				return result;
			}, delegate(IAsyncResult asyncResult)
			{
				object[] array = (object[])asyncResult.AsyncState;
				Socket socket2 = (Socket)array[0];
				EndPoint remoteEndPoint2 = (EndPoint)array[1];
				int receivedBytes = socket2.EndReceiveFrom(asyncResult, ref remoteEndPoint2);
				return new SocketReceiveFromResult
				{
					ReceivedBytes = receivedBytes,
					RemoteEndPoint = remoteEndPoint2
				};
			}, buffer, socketFlags, state2);
		}

		/// <summary>Receives the specified number of bytes of data into the specified location of the data buffer, using the specified <see cref="T:System.Net.Sockets.SocketFlags" />, and stores the endpoint and packet information.</summary>
		/// <param name="socket">The socket to perform the operation on.</param>
		/// <param name="buffer">An array that is the storage location for received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEndPoint">An <see cref="T:System.Net.EndPoint" />, that represents the remote server.</param>
		/// <returns>An asynchronous Task that completes with a <see cref="T:System.Net.Sockets.SocketReceiveMessageFromResult" /> struct.</returns>
		public static Task<SocketReceiveMessageFromResult> ReceiveMessageFromAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEndPoint)
		{
			object[] state2 = new object[]
			{
				socket,
				socketFlags,
				remoteEndPoint
			};
			return Task<SocketReceiveMessageFromResult>.Factory.FromAsync<ArraySegment<byte>>(delegate(ArraySegment<byte> targetBuffer, AsyncCallback callback, object state)
			{
				object[] array = (object[])state;
				Socket socket2 = (Socket)array[0];
				SocketFlags socketFlags2 = (SocketFlags)array[1];
				EndPoint endPoint = (EndPoint)array[2];
				IAsyncResult result = socket2.BeginReceiveMessageFrom(targetBuffer.Array, targetBuffer.Offset, targetBuffer.Count, socketFlags2, ref endPoint, callback, state);
				array[2] = endPoint;
				return result;
			}, delegate(IAsyncResult asyncResult)
			{
				object[] array = (object[])asyncResult.AsyncState;
				Socket socket2 = (Socket)array[0];
				SocketFlags socketFlags2 = (SocketFlags)array[1];
				EndPoint remoteEndPoint2 = (EndPoint)array[2];
				IPPacketInformation packetInformation;
				int receivedBytes = socket2.EndReceiveMessageFrom(asyncResult, ref socketFlags2, ref remoteEndPoint2, out packetInformation);
				return new SocketReceiveMessageFromResult
				{
					PacketInformation = packetInformation,
					ReceivedBytes = receivedBytes,
					RemoteEndPoint = remoteEndPoint2,
					SocketFlags = socketFlags2
				};
			}, buffer, state2);
		}

		/// <summary>Sends data to a connected socket.</summary>
		/// <param name="socket">The socket to perform the operation on.</param>
		/// <param name="buffer">An array of type Byte that contains the data to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>An asynchronous task that completes with number of bytes sent to the socket if the operation was successful. Otherwise, the task will complete with an invalid socket error.</returns>
		public static Task<int> SendAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags)
		{
			return Task<int>.Factory.FromAsync<ArraySegment<byte>, SocketFlags>((ArraySegment<byte> targetBuffer, SocketFlags flags, AsyncCallback callback, object state) => ((Socket)state).BeginSend(targetBuffer.Array, targetBuffer.Offset, targetBuffer.Count, flags, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndSend(asyncResult), buffer, socketFlags, socket);
		}

		/// <summary>Sends data to a connected socket.</summary>
		/// <param name="socket">The socket to perform the operation on.</param>
		/// <param name="buffers">An array that contains the data to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <returns>An asynchronous task that completes with number of bytes sent to the socket if the operation was successful. Otherwise, the task will complete with an invalid socket error.</returns>
		public static Task<int> SendAsync(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			return Task<int>.Factory.FromAsync<IList<ArraySegment<byte>>, SocketFlags>((IList<ArraySegment<byte>> targetBuffers, SocketFlags flags, AsyncCallback callback, object state) => ((Socket)state).BeginSend(targetBuffers, flags, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndSend(asyncResult), buffers, socketFlags, socket);
		}

		/// <summary>Sends data asynchronously to a specific remote host.</summary>
		/// <param name="socket">The socket to perform the operation on.</param>
		/// <param name="buffer">An array that contains the data to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values.</param>
		/// <param name="remoteEP">An <see cref="T:System.Net.EndPoint" /> that represents the remote device.</param>
		/// <returns>An asynchronous task that completes with number of bytes sent if the operation was successful. Otherwise, the task will complete with an invalid socket error.</returns>
		public static Task<int> SendToAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEP)
		{
			return Task<int>.Factory.FromAsync<ArraySegment<byte>, SocketFlags, EndPoint>((ArraySegment<byte> targetBuffer, SocketFlags flags, EndPoint endPoint, AsyncCallback callback, object state) => ((Socket)state).BeginSendTo(targetBuffer.Array, targetBuffer.Offset, targetBuffer.Count, flags, endPoint, callback, state), (IAsyncResult asyncResult) => ((Socket)asyncResult.AsyncState).EndSendTo(asyncResult), buffer, socketFlags, remoteEP, socket);
		}

		public static ValueTask<int> SendAsync(this Socket socket, ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default(CancellationToken))
		{
			return socket.SendAsync(buffer, socketFlags, cancellationToken);
		}

		public static ValueTask<int> ReceiveAsync(this Socket socket, Memory<byte> memory, SocketFlags socketFlags, CancellationToken cancellationToken = default(CancellationToken))
		{
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>(socket);
			byte[] buffer = memory.ToArray();
			socket.BeginReceive(buffer, 0, memory.Length, socketFlags, delegate(IAsyncResult iar)
			{
				cancellationToken.ThrowIfCancellationRequested();
				Memory<byte> memory2 = new Memory<byte>(buffer);
				memory2.CopyTo(memory);
				TaskCompletionSource<int> taskCompletionSource2 = (TaskCompletionSource<int>)iar.AsyncState;
				Socket socket2 = (Socket)taskCompletionSource2.Task.AsyncState;
				try
				{
					taskCompletionSource2.TrySetResult(socket2.EndReceive(iar));
				}
				catch (Exception exception)
				{
					taskCompletionSource2.TrySetException(exception);
				}
			}, taskCompletionSource);
			cancellationToken.ThrowIfCancellationRequested();
			return new ValueTask<int>(taskCompletionSource.Task);
		}
	}
}
