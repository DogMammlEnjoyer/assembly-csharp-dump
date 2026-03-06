using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_ConnectionInfo
	{
		internal unsafe SecPkgContext_ConnectionInfo(byte[] nativeBuffer)
		{
			fixed (byte[] array = nativeBuffer)
			{
				void* value;
				if (nativeBuffer == null || array.Length == 0)
				{
					value = null;
				}
				else
				{
					value = (void*)(&array[0]);
				}
				try
				{
					IntPtr ptr = new IntPtr(value);
					this.Protocol = Marshal.ReadInt32(ptr);
					this.DataCipherAlg = Marshal.ReadInt32(ptr, 4);
					this.DataKeySize = Marshal.ReadInt32(ptr, 8);
					this.DataHashAlg = Marshal.ReadInt32(ptr, 12);
					this.DataHashKeySize = Marshal.ReadInt32(ptr, 16);
					this.KeyExchangeAlg = Marshal.ReadInt32(ptr, 20);
					this.KeyExchKeySize = Marshal.ReadInt32(ptr, 24);
				}
				catch (OverflowException)
				{
					NetEventSource.Fail(this, "Negative size", ".ctor");
					throw;
				}
			}
		}

		public readonly int Protocol;

		public readonly int DataCipherAlg;

		public readonly int DataKeySize;

		public readonly int DataHashAlg;

		public readonly int DataHashKeySize;

		public readonly int KeyExchangeAlg;

		public readonly int KeyExchKeySize;
	}
}
