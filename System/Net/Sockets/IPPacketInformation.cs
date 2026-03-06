using System;

namespace System.Net.Sockets
{
	/// <summary>Presents the packet information from a call to <see cref="M:System.Net.Sockets.Socket.ReceiveMessageFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> or <see cref="M:System.Net.Sockets.Socket.EndReceiveMessageFrom(System.IAsyncResult,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" />.</summary>
	public struct IPPacketInformation
	{
		internal IPPacketInformation(IPAddress address, int networkInterface)
		{
			this.address = address;
			this.networkInterface = networkInterface;
		}

		/// <summary>Gets the origin information of the packet that was received as a result of calling the <see cref="M:System.Net.Sockets.Socket.ReceiveMessageFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> method or <see cref="M:System.Net.Sockets.Socket.EndReceiveMessageFrom(System.IAsyncResult,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> method.</summary>
		/// <returns>An <see cref="T:System.Net.IPAddress" /> that indicates the origin information of the packet that was received as a result of calling the <see cref="M:System.Net.Sockets.Socket.ReceiveMessageFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> method or <see cref="M:System.Net.Sockets.Socket.EndReceiveMessageFrom(System.IAsyncResult,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> method. For packets that were sent from a unicast address, the <see cref="P:System.Net.Sockets.IPPacketInformation.Address" /> property will return the <see cref="T:System.Net.IPAddress" /> of the sender; for multicast or broadcast packets, the <see cref="P:System.Net.Sockets.IPPacketInformation.Address" /> property will return the multicast or broadcast <see cref="T:System.Net.IPAddress" />.</returns>
		public IPAddress Address
		{
			get
			{
				return this.address;
			}
		}

		/// <summary>Gets the network interface information that is associated with a call to <see cref="M:System.Net.Sockets.Socket.ReceiveMessageFrom(System.Byte[],System.Int32,System.Int32,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" /> or <see cref="M:System.Net.Sockets.Socket.EndReceiveMessageFrom(System.IAsyncResult,System.Net.Sockets.SocketFlags@,System.Net.EndPoint@,System.Net.Sockets.IPPacketInformation@)" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value, which represents the index of the network interface. You can use this index with <see cref="M:System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces" /> to get more information about the relevant interface.</returns>
		public int Interface
		{
			get
			{
				return this.networkInterface;
			}
		}

		/// <summary>Tests whether two specified <see cref="T:System.Net.Sockets.IPPacketInformation" /> instances are equivalent.</summary>
		/// <param name="packetInformation1">The <see cref="T:System.Net.Sockets.IPPacketInformation" /> instance that is to the left of the equality operator.</param>
		/// <param name="packetInformation2">The <see cref="T:System.Net.Sockets.IPPacketInformation" /> instance that is to the right of the equality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="packetInformation1" /> and <paramref name="packetInformation2" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(IPPacketInformation packetInformation1, IPPacketInformation packetInformation2)
		{
			return packetInformation1.Equals(packetInformation2);
		}

		/// <summary>Tests whether two specified <see cref="T:System.Net.Sockets.IPPacketInformation" /> instances are not equal.</summary>
		/// <param name="packetInformation1">The <see cref="T:System.Net.Sockets.IPPacketInformation" /> instance that is to the left of the inequality operator.</param>
		/// <param name="packetInformation2">The <see cref="T:System.Net.Sockets.IPPacketInformation" /> instance that is to the right of the inequality operator.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="packetInformation1" /> and <paramref name="packetInformation2" /> are unequal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(IPPacketInformation packetInformation1, IPPacketInformation packetInformation2)
		{
			return !packetInformation1.Equals(packetInformation2);
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="comparand">The object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="comparand" /> is an instance of <see cref="T:System.Net.Sockets.IPPacketInformation" /> and equals the value of the instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object comparand)
		{
			if (comparand == null)
			{
				return false;
			}
			if (!(comparand is IPPacketInformation))
			{
				return false;
			}
			IPPacketInformation ippacketInformation = (IPPacketInformation)comparand;
			return this.address.Equals(ippacketInformation.address) && this.networkInterface == ippacketInformation.networkInterface;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>An Int32 hash code.</returns>
		public override int GetHashCode()
		{
			return this.address.GetHashCode() + this.networkInterface.GetHashCode();
		}

		private IPAddress address;

		private int networkInterface;
	}
}
