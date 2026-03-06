using System;

namespace System.Net.NetworkInformation
{
	/// <summary>Provides information about network interfaces that support Internet Protocol version 6 (IPv6).</summary>
	public abstract class IPv6InterfaceProperties
	{
		/// <summary>Gets the index of the network interface associated with an Internet Protocol version 6 (IPv6) address.</summary>
		/// <returns>An <see cref="T:System.Int32" /> value that contains the index of the network interface for IPv6 address.</returns>
		public abstract int Index { get; }

		/// <summary>Gets the maximum transmission unit (MTU) for this network interface.</summary>
		/// <returns>An <see cref="T:System.Int64" /> value that specifies the MTU.</returns>
		public abstract int Mtu { get; }

		/// <summary>Gets the scope ID of the network interface associated with an Internet Protocol version 6 (IPv6) address.</summary>
		/// <param name="scopeLevel">The scope level.</param>
		/// <returns>The scope ID of the network interface associated with an IPv6 address.</returns>
		public virtual long GetScopeId(ScopeLevel scopeLevel)
		{
			throw new NotImplementedException();
		}
	}
}
