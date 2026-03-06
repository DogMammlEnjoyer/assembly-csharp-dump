using System;

namespace System.Net.Sockets
{
	/// <summary>The result of a <see cref="M:System.Net.Sockets.SocketTaskExtensions.ReceiveMessageFromAsync(System.Net.Sockets.Socket,System.ArraySegment{System.Byte},System.Net.Sockets.SocketFlags,System.Net.EndPoint)" /> operation.</summary>
	public struct SocketReceiveMessageFromResult
	{
		/// <summary>The number of bytes received. If the <see cref="M:System.Net.Sockets.SocketTaskExtensions.ReceiveMessageFromAsync(System.Net.Sockets.Socket,System.ArraySegment{System.Byte},System.Net.Sockets.SocketFlags,System.Net.EndPoint)" /> operation is unsuccessful, this value will be 0.</summary>
		public int ReceivedBytes;

		/// <summary>A bitwise combination of the <see cref="T:System.Net.Sockets.SocketFlags" /> values for the received packet.</summary>
		public SocketFlags SocketFlags;

		/// <summary>The source <see cref="T:System.Net.EndPoint" />.</summary>
		public EndPoint RemoteEndPoint;

		/// <summary>An <see cref="T:System.Net.Sockets.IPPacketInformation" /> holding address and interface information.</summary>
		public IPPacketInformation PacketInformation;
	}
}
