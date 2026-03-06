using System;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Net
{
	/// <summary>Provides an Internet Protocol (IP) address.</summary>
	[Serializable]
	public class IPAddress
	{
		private bool IsIPv4
		{
			get
			{
				return this._numbers == null;
			}
		}

		private bool IsIPv6
		{
			get
			{
				return this._numbers != null;
			}
		}

		private uint PrivateAddress
		{
			get
			{
				return this._addressOrScopeId;
			}
			set
			{
				this._toString = null;
				this._hashCode = 0;
				this._addressOrScopeId = value;
			}
		}

		private uint PrivateScopeId
		{
			get
			{
				return this._addressOrScopeId;
			}
			set
			{
				this._toString = null;
				this._hashCode = 0;
				this._addressOrScopeId = value;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.IPAddress" /> class with the address specified as an <see cref="T:System.Int64" />.</summary>
		/// <param name="newAddress">The long value of the IP address. For example, the value 0x2414188f in big-endian format would be the IP address "143.24.20.36".</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="newAddress" /> &lt; 0 or  
		/// <paramref name="newAddress" /> &gt; 0x00000000FFFFFFFF</exception>
		public IPAddress(long newAddress)
		{
			if (newAddress < 0L || newAddress > (long)((ulong)-1))
			{
				throw new ArgumentOutOfRangeException("newAddress");
			}
			this.PrivateAddress = (uint)newAddress;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.IPAddress" /> class with the address specified as a <see cref="T:System.Byte" /> array and the specified scope identifier.</summary>
		/// <param name="address">The byte array value of the IP address.</param>
		/// <param name="scopeid">The long value of the scope identifier.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="address" /> contains a bad IP address.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="scopeid" /> &lt; 0 or  
		/// <paramref name="scopeid" /> &gt; 0x00000000FFFFFFFF</exception>
		public IPAddress(byte[] address, long scopeid) : this(new ReadOnlySpan<byte>(address ?? IPAddress.ThrowAddressNullException()), scopeid)
		{
		}

		public unsafe IPAddress(ReadOnlySpan<byte> address, long scopeid)
		{
			if (address.Length != 16)
			{
				throw new ArgumentException("An invalid IP address was specified.", "address");
			}
			if (scopeid < 0L || scopeid > (long)((ulong)-1))
			{
				throw new ArgumentOutOfRangeException("scopeid");
			}
			this._numbers = new ushort[8];
			for (int i = 0; i < 8; i++)
			{
				this._numbers[i] = (ushort)((int)(*address[i * 2]) * 256 + (int)(*address[i * 2 + 1]));
			}
			this.PrivateScopeId = (uint)scopeid;
		}

		internal unsafe IPAddress(ushort* numbers, int numbersLength, uint scopeid)
		{
			ushort[] array = new ushort[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = numbers[i];
			}
			this._numbers = array;
			this.PrivateScopeId = scopeid;
		}

		private IPAddress(ushort[] numbers, uint scopeid)
		{
			this._numbers = numbers;
			this.PrivateScopeId = scopeid;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.IPAddress" /> class with the address specified as a <see cref="T:System.Byte" /> array.</summary>
		/// <param name="address">The byte array value of the IP address.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="address" /> contains a bad IP address.</exception>
		public IPAddress(byte[] address) : this(new ReadOnlySpan<byte>(address ?? IPAddress.ThrowAddressNullException()))
		{
		}

		public unsafe IPAddress(ReadOnlySpan<byte> address)
		{
			if (address.Length == 4)
			{
				this.PrivateAddress = (uint)((long)((int)(*address[3]) << 24 | (int)(*address[2]) << 16 | (int)(*address[1]) << 8 | (int)(*address[0])) & (long)((ulong)-1));
				return;
			}
			if (address.Length == 16)
			{
				this._numbers = new ushort[8];
				for (int i = 0; i < 8; i++)
				{
					this._numbers[i] = (ushort)((int)(*address[i * 2]) * 256 + (int)(*address[i * 2 + 1]));
				}
				return;
			}
			throw new ArgumentException("An invalid IP address was specified.", "address");
		}

		internal IPAddress(int newAddress)
		{
			this.PrivateAddress = (uint)newAddress;
		}

		/// <summary>Determines whether a string is a valid IP address.</summary>
		/// <param name="ipString">The string to validate.</param>
		/// <param name="address">The <see cref="T:System.Net.IPAddress" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="ipString" /> was able to be parsed as an IP address; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="ipString" /> is null.</exception>
		public static bool TryParse(string ipString, out IPAddress address)
		{
			if (ipString == null)
			{
				address = null;
				return false;
			}
			address = IPAddressParser.Parse(ipString.AsSpan(), true);
			return address != null;
		}

		public static bool TryParse(ReadOnlySpan<char> ipSpan, out IPAddress address)
		{
			address = IPAddressParser.Parse(ipSpan, true);
			return address != null;
		}

		/// <summary>Converts an IP address string to an <see cref="T:System.Net.IPAddress" /> instance.</summary>
		/// <param name="ipString">A string that contains an IP address in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.</param>
		/// <returns>An <see cref="T:System.Net.IPAddress" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="ipString" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="ipString" /> is not a valid IP address.</exception>
		public static IPAddress Parse(string ipString)
		{
			if (ipString == null)
			{
				throw new ArgumentNullException("ipString");
			}
			return IPAddressParser.Parse(ipString.AsSpan(), false);
		}

		public static IPAddress Parse(ReadOnlySpan<char> ipSpan)
		{
			return IPAddressParser.Parse(ipSpan, false);
		}

		public bool TryWriteBytes(Span<byte> destination, out int bytesWritten)
		{
			if (this.IsIPv6)
			{
				if (destination.Length < 16)
				{
					bytesWritten = 0;
					return false;
				}
				this.WriteIPv6Bytes(destination);
				bytesWritten = 16;
			}
			else
			{
				if (destination.Length < 4)
				{
					bytesWritten = 0;
					return false;
				}
				this.WriteIPv4Bytes(destination);
				bytesWritten = 4;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteIPv6Bytes(Span<byte> destination)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				*destination[num++] = (byte)(this._numbers[i] >> 8 & 255);
				*destination[num++] = (byte)(this._numbers[i] & 255);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void WriteIPv4Bytes(Span<byte> destination)
		{
			uint privateAddress = this.PrivateAddress;
			*destination[0] = (byte)privateAddress;
			*destination[1] = (byte)(privateAddress >> 8);
			*destination[2] = (byte)(privateAddress >> 16);
			*destination[3] = (byte)(privateAddress >> 24);
		}

		/// <summary>Provides a copy of the <see cref="T:System.Net.IPAddress" /> as an array of bytes.</summary>
		/// <returns>A <see cref="T:System.Byte" /> array.</returns>
		public byte[] GetAddressBytes()
		{
			if (this.IsIPv6)
			{
				byte[] array = new byte[16];
				this.WriteIPv6Bytes(array);
				return array;
			}
			byte[] array2 = new byte[4];
			this.WriteIPv4Bytes(array2);
			return array2;
		}

		/// <summary>Gets the address family of the IP address.</summary>
		/// <returns>Returns <see cref="F:System.Net.Sockets.AddressFamily.InterNetwork" /> for IPv4 or <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> for IPv6.</returns>
		public AddressFamily AddressFamily
		{
			get
			{
				if (!this.IsIPv4)
				{
					return AddressFamily.InterNetworkV6;
				}
				return AddressFamily.InterNetwork;
			}
		}

		/// <summary>Gets or sets the IPv6 address scope identifier.</summary>
		/// <returns>A long integer that specifies the scope of the address.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <see langword="AddressFamily" /> = <see langword="InterNetwork" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="scopeId" /> &lt; 0  
		/// -or-
		///
		/// <paramref name="scopeId" /> &gt; 0x00000000FFFFFFFF</exception>
		public long ScopeId
		{
			get
			{
				if (this.IsIPv4)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				return (long)((ulong)this.PrivateScopeId);
			}
			set
			{
				if (this.IsIPv4)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				if (value < 0L || value > (long)((ulong)-1))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.PrivateScopeId = (uint)value;
			}
		}

		/// <summary>Converts an Internet address to its standard notation.</summary>
		/// <returns>A string that contains the IP address in either IPv4 dotted-quad or in IPv6 colon-hexadecimal notation.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">The address family is <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" /> and the address is bad.</exception>
		public override string ToString()
		{
			if (this._toString == null)
			{
				this._toString = (this.IsIPv4 ? IPAddressParser.IPv4AddressToString(this.PrivateAddress) : IPAddressParser.IPv6AddressToString(this._numbers, this.PrivateScopeId));
			}
			return this._toString;
		}

		public bool TryFormat(Span<char> destination, out int charsWritten)
		{
			if (!this.IsIPv4)
			{
				return IPAddressParser.IPv6AddressToString(this._numbers, this.PrivateScopeId, destination, out charsWritten);
			}
			return IPAddressParser.IPv4AddressToString(this.PrivateAddress, destination, out charsWritten);
		}

		/// <summary>Converts a long value from host byte order to network byte order.</summary>
		/// <param name="host">The number to convert, expressed in host byte order.</param>
		/// <returns>A long value, expressed in network byte order.</returns>
		public static long HostToNetworkOrder(long host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return BinaryPrimitives.ReverseEndianness(host);
		}

		/// <summary>Converts an integer value from host byte order to network byte order.</summary>
		/// <param name="host">The number to convert, expressed in host byte order.</param>
		/// <returns>An integer value, expressed in network byte order.</returns>
		public static int HostToNetworkOrder(int host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return BinaryPrimitives.ReverseEndianness(host);
		}

		/// <summary>Converts a short value from host byte order to network byte order.</summary>
		/// <param name="host">The number to convert, expressed in host byte order.</param>
		/// <returns>A short value, expressed in network byte order.</returns>
		public static short HostToNetworkOrder(short host)
		{
			if (!BitConverter.IsLittleEndian)
			{
				return host;
			}
			return BinaryPrimitives.ReverseEndianness(host);
		}

		/// <summary>Converts a long value from network byte order to host byte order.</summary>
		/// <param name="network">The number to convert, expressed in network byte order.</param>
		/// <returns>A long value, expressed in host byte order.</returns>
		public static long NetworkToHostOrder(long network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		/// <summary>Converts an integer value from network byte order to host byte order.</summary>
		/// <param name="network">The number to convert, expressed in network byte order.</param>
		/// <returns>An integer value, expressed in host byte order.</returns>
		public static int NetworkToHostOrder(int network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		/// <summary>Converts a short value from network byte order to host byte order.</summary>
		/// <param name="network">The number to convert, expressed in network byte order.</param>
		/// <returns>A short value, expressed in host byte order.</returns>
		public static short NetworkToHostOrder(short network)
		{
			return IPAddress.HostToNetworkOrder(network);
		}

		/// <summary>Indicates whether the specified IP address is the loopback address.</summary>
		/// <param name="address">An IP address.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="address" /> is the loopback address; otherwise, <see langword="false" />.</returns>
		public static bool IsLoopback(IPAddress address)
		{
			if (address == null)
			{
				IPAddress.ThrowAddressNullException();
			}
			if (address.IsIPv6)
			{
				return address.Equals(IPAddress.IPv6Loopback);
			}
			return ((ulong)address.PrivateAddress & 255UL) == ((ulong)IPAddress.Loopback.PrivateAddress & 255UL);
		}

		/// <summary>Gets whether the address is an IPv6 multicast global address.</summary>
		/// <returns>
		///   <see langword="true" /> if the IP address is an IPv6 multicast global address; otherwise, <see langword="false" />.</returns>
		public bool IsIPv6Multicast
		{
			get
			{
				return this.IsIPv6 && (this._numbers[0] & 65280) == 65280;
			}
		}

		/// <summary>Gets whether the address is an IPv6 link local address.</summary>
		/// <returns>
		///   <see langword="true" /> if the IP address is an IPv6 link local address; otherwise, <see langword="false" />.</returns>
		public bool IsIPv6LinkLocal
		{
			get
			{
				return this.IsIPv6 && (this._numbers[0] & 65472) == 65152;
			}
		}

		/// <summary>Gets whether the address is an IPv6 site local address.</summary>
		/// <returns>
		///   <see langword="true" /> if the IP address is an IPv6 site local address; otherwise, <see langword="false" />.</returns>
		public bool IsIPv6SiteLocal
		{
			get
			{
				return this.IsIPv6 && (this._numbers[0] & 65472) == 65216;
			}
		}

		/// <summary>Gets whether the address is an IPv6 Teredo address.</summary>
		/// <returns>
		///   <see langword="true" /> if the IP address is an IPv6 Teredo address; otherwise, <see langword="false" />.</returns>
		public bool IsIPv6Teredo
		{
			get
			{
				return this.IsIPv6 && this._numbers[0] == 8193 && this._numbers[1] == 0;
			}
		}

		/// <summary>Gets whether the IP address is an IPv4-mapped IPv6 address.</summary>
		/// <returns>Returns <see cref="T:System.Boolean" />.  
		///  <see langword="true" /> if the IP address is an IPv4-mapped IPv6 address; otherwise, <see langword="false" />.</returns>
		public bool IsIPv4MappedToIPv6
		{
			get
			{
				if (this.IsIPv4)
				{
					return false;
				}
				for (int i = 0; i < 5; i++)
				{
					if (this._numbers[i] != 0)
					{
						return false;
					}
				}
				return this._numbers[5] == ushort.MaxValue;
			}
		}

		/// <summary>An Internet Protocol (IP) address.</summary>
		/// <returns>The long value of the IP address.</returns>
		/// <exception cref="T:System.Net.Sockets.SocketException">The address family is <see cref="F:System.Net.Sockets.AddressFamily.InterNetworkV6" />.</exception>
		[Obsolete("This property has been deprecated. It is address family dependent. Please use IPAddress.Equals method to perform comparisons. https://go.microsoft.com/fwlink/?linkid=14202")]
		public long Address
		{
			get
			{
				if (this.AddressFamily == AddressFamily.InterNetworkV6)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				return (long)((ulong)this.PrivateAddress);
			}
			set
			{
				if (this.AddressFamily == AddressFamily.InterNetworkV6)
				{
					throw new SocketException(SocketError.OperationNotSupported);
				}
				if ((ulong)this.PrivateAddress != (ulong)value)
				{
					if (this is IPAddress.ReadOnlyIPAddress)
					{
						throw new SocketException(SocketError.OperationNotSupported);
					}
					this.PrivateAddress = (uint)value;
				}
			}
		}

		internal bool Equals(object comparandObj, bool compareScopeId)
		{
			IPAddress ipaddress = comparandObj as IPAddress;
			if (ipaddress == null)
			{
				return false;
			}
			if (this.AddressFamily != ipaddress.AddressFamily)
			{
				return false;
			}
			if (this.IsIPv6)
			{
				for (int i = 0; i < 8; i++)
				{
					if (ipaddress._numbers[i] != this._numbers[i])
					{
						return false;
					}
				}
				return ipaddress.PrivateScopeId == this.PrivateScopeId || !compareScopeId;
			}
			return ipaddress.PrivateAddress == this.PrivateAddress;
		}

		/// <summary>Compares two IP addresses.</summary>
		/// <param name="comparand">An <see cref="T:System.Net.IPAddress" /> instance to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the two addresses are equal; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object comparand)
		{
			return this.Equals(comparand, true);
		}

		/// <summary>Returns a hash value for an IP address.</summary>
		/// <returns>An integer hash value.</returns>
		public unsafe override int GetHashCode()
		{
			if (this._hashCode != 0)
			{
				return this._hashCode;
			}
			int hashCode;
			if (this.IsIPv6)
			{
				Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)20], 20);
				MemoryMarshal.AsBytes<ushort>(new ReadOnlySpan<ushort>(this._numbers)).CopyTo(span);
				BitConverter.TryWriteBytes(span.Slice(16), this._addressOrScopeId);
				hashCode = Marvin.ComputeHash32(span, Marvin.DefaultSeed);
			}
			else
			{
				hashCode = Marvin.ComputeHash32(MemoryMarshal.AsBytes<uint>(MemoryMarshal.CreateReadOnlySpan<uint>(ref this._addressOrScopeId, 1)), Marvin.DefaultSeed);
			}
			this._hashCode = hashCode;
			return this._hashCode;
		}

		/// <summary>Maps the <see cref="T:System.Net.IPAddress" /> object to an IPv6 address.</summary>
		/// <returns>Returns <see cref="T:System.Net.IPAddress" />.  
		///  An IPv6 address.</returns>
		public IPAddress MapToIPv6()
		{
			if (this.IsIPv6)
			{
				return this;
			}
			uint privateAddress = this.PrivateAddress;
			return new IPAddress(new ushort[]
			{
				0,
				0,
				0,
				0,
				0,
				ushort.MaxValue,
				(ushort)((privateAddress & 65280U) >> 8 | (privateAddress & 255U) << 8),
				(ushort)((privateAddress & 4278190080U) >> 24 | (privateAddress & 16711680U) >> 8)
			}, 0U);
		}

		/// <summary>Maps the <see cref="T:System.Net.IPAddress" /> object to an IPv4 address.</summary>
		/// <returns>Returns <see cref="T:System.Net.IPAddress" />.  
		///  An IPv4 address.</returns>
		public IPAddress MapToIPv4()
		{
			if (this.IsIPv4)
			{
				return this;
			}
			return new IPAddress((long)((ulong)((uint)(this._numbers[6] & 65280) >> 8 | (uint)((uint)(this._numbers[6] & 255) << 8) | ((uint)(this._numbers[7] & 65280) >> 8 | (uint)((uint)(this._numbers[7] & 255) << 8)) << 16)));
		}

		private static byte[] ThrowAddressNullException()
		{
			throw new ArgumentNullException("address");
		}

		/// <summary>Provides an IP address that indicates that the server must listen for client activity on all network interfaces. This field is read-only.</summary>
		public static readonly IPAddress Any = new IPAddress.ReadOnlyIPAddress(0L);

		/// <summary>Provides the IP loopback address. This field is read-only.</summary>
		public static readonly IPAddress Loopback = new IPAddress.ReadOnlyIPAddress(16777343L);

		/// <summary>Provides the IP broadcast address. This field is read-only.</summary>
		public static readonly IPAddress Broadcast = new IPAddress.ReadOnlyIPAddress((long)((ulong)-1));

		/// <summary>Provides an IP address that indicates that no network interface should be used. This field is read-only.</summary>
		public static readonly IPAddress None = IPAddress.Broadcast;

		internal const long LoopbackMask = 255L;

		/// <summary>The <see cref="M:System.Net.Sockets.Socket.Bind(System.Net.EndPoint)" /> method uses the <see cref="F:System.Net.IPAddress.IPv6Any" /> field to indicate that a <see cref="T:System.Net.Sockets.Socket" /> must listen for client activity on all network interfaces.</summary>
		public static readonly IPAddress IPv6Any = new IPAddress(new byte[16], 0L);

		/// <summary>Provides the IP loopback address. This property is read-only.</summary>
		public static readonly IPAddress IPv6Loopback = new IPAddress(new byte[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1
		}, 0L);

		/// <summary>Provides an IP address that indicates that no network interface should be used. This property is read-only.</summary>
		public static readonly IPAddress IPv6None = new IPAddress(new byte[16], 0L);

		private uint _addressOrScopeId;

		private readonly ushort[] _numbers;

		private string _toString;

		private int _hashCode;

		internal const int NumberOfLabels = 8;

		private sealed class ReadOnlyIPAddress : IPAddress
		{
			public ReadOnlyIPAddress(long newAddress) : base(newAddress)
			{
			}
		}
	}
}
