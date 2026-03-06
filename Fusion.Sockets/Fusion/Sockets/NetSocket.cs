using System;
using System.Runtime.InteropServices;
using NanoSockets;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct NetSocket
	{
		public bool IsCreated
		{
			get
			{
				return this.NativeSocket.IsCreated;
			}
		}

		[FieldOffset(0)]
		public long Handle;

		[FieldOffset(0)]
		public Socket NativeSocket;
	}
}
