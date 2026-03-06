using System;
using System.Runtime.InteropServices;

namespace NanoSockets
{
	[StructLayout(LayoutKind.Explicit, Size = 24)]
	public struct Address : IEquatable<Address>
	{
		public bool Equals(Address other)
		{
			return this._address0 == other._address0 && this._address1 == other._address1 && this.Port == other.Port;
		}

		public override bool Equals(object obj)
		{
			if (obj is Address)
			{
				Address other = (Address)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((17 * 31 + this._address0.GetHashCode()) * 31 + this._address1.GetHashCode()) * 31 + this.Port.GetHashCode();
		}

		public override string ToString()
		{
			Address address = this;
			int num = 64;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			UDP.GetIP(ref address, intPtr, num);
			string arg = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return string.Format("[{0} Ip={1} Port={2}]", "Address", arg, this.Port);
		}

		public static Address LocalhostIPv4(ushort port = 0)
		{
			return Address.CreateFromIpPort("127.0.0.1", port);
		}

		public static Address Any(ushort port = 0)
		{
			return Address.CreateFromIpPort("0.0.0.0", port);
		}

		public static Address CreateFromIpPort(string ip, ushort port)
		{
			Address result = default(Address);
			if (UDP.SetIP(ref result, ip) != Status.Ok)
			{
				throw new Exception("Can not CreateFromIpPort. IP not Set");
			}
			result.Port = port;
			return result;
		}

		[FieldOffset(0)]
		public ulong _address0;

		[FieldOffset(8)]
		public ulong _address1;

		[FieldOffset(16)]
		public ushort Port;
	}
}
