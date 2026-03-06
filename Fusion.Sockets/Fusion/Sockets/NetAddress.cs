using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Fusion.Protocol;
using NanoSockets;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetAddress : IEquatable<NetAddress>
	{
		public int ActorId
		{
			get
			{
				return this._actorId - 1;
			}
		}

		public bool IsRelayAddr
		{
			get
			{
				return this.ActorId >= 0;
			}
		}

		public bool IsIPv6
		{
			get
			{
				return !this.IsRelayAddr && (this.NativeAddress._address0 != 0UL || (this.NativeAddress._address1 & (ulong)-65536) != (ulong)-65536);
			}
		}

		public bool IsIPv4
		{
			get
			{
				return !this.IsRelayAddr && (this.NativeAddress._address1 & (ulong)-65536) == (ulong)-65536;
			}
		}

		public bool IsValid
		{
			get
			{
				return !this.Equals(NetAddress.AnyIPv4Addr) && !this.Equals(NetAddress.AnyIPv6Addr);
			}
		}

		public bool HasAddress
		{
			get
			{
				bool isRelayAddr = this.IsRelayAddr;
				bool result;
				if (isRelayAddr)
				{
					result = false;
				}
				else
				{
					bool isIPv = this.IsIPv6;
					if (isIPv)
					{
						result = (this.NativeAddress._address0 != 0UL || this.NativeAddress._address1 > 0UL);
					}
					else
					{
						bool isIPv2 = this.IsIPv4;
						result = (isIPv2 && this.NativeAddress._address0 == 0UL && this.NativeAddress._address1 >> 32 > 0UL);
					}
				}
				return result;
			}
		}

		public static NetAddress FromActorId(int actorId)
		{
			Assert.Check(actorId >= 0, "ActorID must be 0 or greater");
			return new NetAddress
			{
				_actorId = actorId + 1
			};
		}

		internal static ulong Hash64(NetAddress address)
		{
			ulong num = 17UL;
			num = num * 31UL + address.Block0;
			num = num * 31UL + address.Block1;
			return num * 31UL + address.Block2;
		}

		public static NetAddress LocalhostIPv4(ushort port = 0)
		{
			return NetAddress.CreateFromIpPort("127.0.0.1", port);
		}

		public static NetAddress LocalhostIPv6(ushort port = 0)
		{
			return NetAddress.CreateFromIpPort("::1", port);
		}

		public static NetAddress Any(ushort port = 0)
		{
			return NetAddress.CreateFromIpPort("0.0.0.0", port);
		}

		public static NetAddress AnyIPv6(ushort port = 0)
		{
			return NetAddress.CreateFromIpPort("::", port);
		}

		public static NetAddress CreateFromIpPort(string ip, ushort port)
		{
			IPAddress ipaddress;
			bool flag = string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out ipaddress);
			if (flag)
			{
				throw new ArgumentException("IP/Port passed are invalid.");
			}
			ip = ip.Split('%', StringSplitOptions.None)[0];
			Address nativeAddress = default(Address);
			Assert.Always(UDP.SetIP(ref nativeAddress, ip) == Status.Ok, "Unable to parse IP. Verify if it represents a valid IP.");
			nativeAddress.Port = port;
			return new NetAddress
			{
				NativeAddress = nativeAddress,
				_actorId = 0
			};
		}

		internal void Serialize(BitStream stream)
		{
			stream.Serialize(ref this.Block0);
			stream.Serialize(ref this.Block1);
			stream.Serialize(ref this.Block2);
		}

		public bool Equals(NetAddress other)
		{
			return this.Block0 == other.Block0 && this.Block1 == other.Block1 && this.Block2 == other.Block2;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is NetAddress)
			{
				NetAddress other = (NetAddress)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			int num = this.Block0.GetHashCode();
			num = (num * 397 ^ this.Block1.GetHashCode());
			return num * 397 ^ this.Block2.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}: {2}, {3}: {4}, {5}: {6}, {7}: {8}, {9}: {10}, [{11},{12},{13}]]", new object[]
			{
				"NetAddress",
				"IsValid",
				this.IsValid,
				"ActorId",
				this.ActorId,
				"NativeAddress",
				this.NativeAddress,
				"IsIPv6",
				this.IsIPv6,
				"IsRelayAddr",
				this.IsRelayAddr,
				this.Block0,
				this.Block1,
				this.Block2
			});
		}

		[FieldOffset(0)]
		internal Address NativeAddress;

		[FieldOffset(0)]
		internal ulong Block0;

		[FieldOffset(8)]
		internal ulong Block1;

		[FieldOffset(16)]
		internal ulong Block2;

		[FieldOffset(20)]
		private int _actorId;

		internal static NetAddress AnyIPv4Addr = new NetAddress
		{
			NativeAddress = new Address
			{
				_address0 = 0UL,
				_address1 = (ulong)-65536,
				Port = 0
			},
			_actorId = 0
		};

		internal static NetAddress AnyIPv6Addr = new NetAddress
		{
			NativeAddress = new Address
			{
				_address0 = 0UL,
				_address1 = 0UL,
				Port = 0
			},
			_actorId = 0
		};

		public sealed class EqualityComparer : IEqualityComparer<NetAddress>
		{
			public bool Equals(NetAddress x, NetAddress y)
			{
				return x.Block0 == y.Block0 && x.Block1 == y.Block1 && x.Block2 == y.Block2;
			}

			public int GetHashCode(NetAddress obj)
			{
				int num = obj.Block0.GetHashCode();
				num = (num * 397 ^ obj.Block1.GetHashCode());
				return num * 397 ^ obj.Block2.GetHashCode();
			}
		}

		internal static class SubnetMask
		{
			public static NetAddress[] SubnetMasks { get; private set; } = new NetAddress[]
			{
				NetAddress.CreateFromIpPort("255.0.0.0", 0),
				NetAddress.CreateFromIpPort("255.255.0.0", 0),
				NetAddress.CreateFromIpPort("255.255.255.0", 0)
			};

			public static bool IsSameSubNet(NetAddress addressA, NetAddress addressB)
			{
				bool flag = !addressA.IsValid || !addressB.IsValid;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = addressA.IsIPv6 ^ addressB.IsIPv6;
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = addressA.IsIPv6 && addressB.IsIPv6;
						if (flag3)
						{
							result = true;
						}
						else
						{
							bool flag4 = !addressA.IsIPv6 && !addressB.IsIPv6;
							if (flag4)
							{
								foreach (NetAddress subnetMask in NetAddress.SubnetMask.SubnetMasks)
								{
									NetAddress networkAddress = NetAddress.SubnetMask.GetNetworkAddress(addressA, subnetMask);
									NetAddress networkAddress2 = NetAddress.SubnetMask.GetNetworkAddress(addressB, subnetMask);
									bool flag5 = networkAddress.Equals(networkAddress2);
									if (flag5)
									{
										return true;
									}
								}
							}
							result = false;
						}
					}
				}
				return result;
			}

			public static NetAddress GetNetworkAddress(NetAddress netAddress, NetAddress subnetMask)
			{
				NetAddress result = NetAddress.Any(0);
				result.Block0 = (netAddress.Block0 & subnetMask.Block0);
				result.Block1 = (netAddress.Block1 & subnetMask.Block1);
				result.Block2 = (netAddress.Block2 & subnetMask.Block2);
				return result;
			}
		}
	}
}
