using System;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetUnreliableHeader
	{
		public static NetUnreliableHeader Create()
		{
			NetUnreliableHeader result;
			result.PacketType = NetPacketType.UnreliableData;
			return result;
		}

		public const int SIZE = 1;

		public const int SIZE_IN_BITS = 8;

		[FieldOffset(0)]
		public NetPacketType PacketType;
	}
}
