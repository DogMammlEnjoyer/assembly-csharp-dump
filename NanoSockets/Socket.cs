using System;
using System.Runtime.InteropServices;

namespace NanoSockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Socket
	{
		public bool IsCreated
		{
			get
			{
				return this.handle > 0L;
			}
		}

		public static implicit operator long(Socket socket)
		{
			return socket.handle;
		}

		public static implicit operator Socket(long handle)
		{
			Socket result;
			result.handle = handle;
			return result;
		}

		[FieldOffset(0)]
		public long handle;
	}
}
