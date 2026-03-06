using System;

namespace System.Net.NetworkInformation
{
	/// <summary>Provides information about a network interface address.</summary>
	public abstract class IPAddressInformation
	{
		/// <summary>Gets the Internet Protocol (IP) address.</summary>
		/// <returns>An <see cref="T:System.Net.IPAddress" /> instance that contains the IP address of an interface.</returns>
		public abstract IPAddress Address { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the Internet Protocol (IP) address is valid to appear in a Domain Name System (DNS) server database.</summary>
		/// <returns>
		///   <see langword="true" /> if the address can appear in a DNS database; otherwise, <see langword="false" />.</returns>
		public abstract bool IsDnsEligible { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the Internet Protocol (IP) address is transient (a cluster address).</summary>
		/// <returns>
		///   <see langword="true" /> if the address is transient; otherwise, <see langword="false" />.</returns>
		public abstract bool IsTransient { get; }
	}
}
