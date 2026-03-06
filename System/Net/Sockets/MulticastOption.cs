using System;

namespace System.Net.Sockets
{
	/// <summary>Contains <see cref="T:System.Net.IPAddress" /> values used to join and drop multicast groups.</summary>
	public class MulticastOption
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.MulticastOption" /> class with the specified IP multicast group address and local IP address associated with a network interface.</summary>
		/// <param name="group">The group <see cref="T:System.Net.IPAddress" />.</param>
		/// <param name="mcint">The local <see cref="T:System.Net.IPAddress" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="group" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="mcint" /> is <see langword="null" />.</exception>
		public MulticastOption(IPAddress group, IPAddress mcint)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (mcint == null)
			{
				throw new ArgumentNullException("mcint");
			}
			this.Group = group;
			this.LocalAddress = mcint;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.MulticastOption" /> class with the specified IP multicast group address and interface index.</summary>
		/// <param name="group">The <see cref="T:System.Net.IPAddress" /> of the multicast group.</param>
		/// <param name="interfaceIndex">The index of the interface that is used to send and receive multicast packets.</param>
		public MulticastOption(IPAddress group, int interfaceIndex)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (interfaceIndex < 0 || interfaceIndex > 16777215)
			{
				throw new ArgumentOutOfRangeException("interfaceIndex");
			}
			this.Group = group;
			this.ifIndex = interfaceIndex;
		}

		/// <summary>Initializes a new version of the <see cref="T:System.Net.Sockets.MulticastOption" /> class for the specified IP multicast group.</summary>
		/// <param name="group">The <see cref="T:System.Net.IPAddress" /> of the multicast group.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="group" /> is <see langword="null" />.</exception>
		public MulticastOption(IPAddress group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.Group = group;
			this.LocalAddress = IPAddress.Any;
		}

		/// <summary>Gets or sets the IP address of a multicast group.</summary>
		/// <returns>An <see cref="T:System.Net.IPAddress" /> that contains the Internet address of a multicast group.</returns>
		public IPAddress Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		/// <summary>Gets or sets the local address associated with a multicast group.</summary>
		/// <returns>An <see cref="T:System.Net.IPAddress" /> that contains the local address associated with a multicast group.</returns>
		public IPAddress LocalAddress
		{
			get
			{
				return this.localAddress;
			}
			set
			{
				this.ifIndex = 0;
				this.localAddress = value;
			}
		}

		/// <summary>Gets or sets the index of the interface that is used to send and receive multicast packets.</summary>
		/// <returns>An integer that represents the index of a <see cref="T:System.Net.NetworkInformation.NetworkInterface" /> array element.</returns>
		public int InterfaceIndex
		{
			get
			{
				return this.ifIndex;
			}
			set
			{
				if (value < 0 || value > 16777215)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.localAddress = null;
				this.ifIndex = value;
			}
		}

		private IPAddress group;

		private IPAddress localAddress;

		private int ifIndex;
	}
}
