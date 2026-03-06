using System;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetCommandAccepted
	{
		public static NetCommandAccepted Create(NetConnectionId localId, NetConnectionId remoteId, uint counter)
		{
			return new NetCommandAccepted
			{
				Header = NetCommands.Accepted,
				AcceptedLocalId = localId,
				AcceptedRemoteId = remoteId,
				Counter = counter
			};
		}

		[FieldOffset(0)]
		public NetCommandHeader Header;

		[FieldOffset(8)]
		public NetConnectionId AcceptedLocalId;

		[FieldOffset(16)]
		public NetConnectionId AcceptedRemoteId;

		[FieldOffset(24)]
		public uint Counter;
	}
}
