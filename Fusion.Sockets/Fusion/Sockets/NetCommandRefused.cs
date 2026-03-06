using System;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetCommandRefused
	{
		public static NetCommandRefused Create(NetConnectFailedReason reason)
		{
			return new NetCommandRefused
			{
				Header = NetCommands.Refused,
				Reason = reason
			};
		}

		public const int SIZE_IN_BYTES = 3;

		public const int SIZE_IN_BITS = 24;

		[FieldOffset(0)]
		public NetCommandHeader Header;

		[FieldOffset(2)]
		public NetConnectFailedReason Reason;
	}
}
