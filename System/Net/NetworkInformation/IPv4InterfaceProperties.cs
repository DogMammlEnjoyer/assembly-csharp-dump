using System;

namespace System.Net.NetworkInformation
{
	/// <summary>Provides information about network interfaces that support Internet Protocol version 4 (IPv4).</summary>
	public abstract class IPv4InterfaceProperties
	{
		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether an interface uses Windows Internet Name Service (WINS).</summary>
		/// <returns>
		///   <see langword="true" /> if the interface uses WINS; otherwise, <see langword="false" />.</returns>
		public abstract bool UsesWins { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the interface is configured to use a Dynamic Host Configuration Protocol (DHCP) server to obtain an IP address.</summary>
		/// <returns>
		///   <see langword="true" /> if the interface is configured to obtain an IP address from a DHCP server; otherwise, <see langword="false" />.</returns>
		public abstract bool IsDhcpEnabled { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether this interface has an automatic private IP addressing (APIPA) address.</summary>
		/// <returns>
		///   <see langword="true" /> if the interface uses an APIPA address; otherwise, <see langword="false" />.</returns>
		public abstract bool IsAutomaticPrivateAddressingActive { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether this interface has automatic private IP addressing (APIPA) enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if the interface uses APIPA; otherwise, <see langword="false" />.</returns>
		public abstract bool IsAutomaticPrivateAddressingEnabled { get; }

		/// <summary>Gets the index of the network interface associated with the Internet Protocol version 4 (IPv4) address.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the index of the IPv4 interface.</returns>
		public abstract int Index { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether this interface can forward (route) packets.</summary>
		/// <returns>
		///   <see langword="true" /> if this interface routes packets; otherwise <see langword="false" />.</returns>
		public abstract bool IsForwardingEnabled { get; }

		/// <summary>Gets the maximum transmission unit (MTU) for this network interface.</summary>
		/// <returns>An <see cref="T:System.Int64" /> value that specifies the MTU.</returns>
		public abstract int Mtu { get; }
	}
}
