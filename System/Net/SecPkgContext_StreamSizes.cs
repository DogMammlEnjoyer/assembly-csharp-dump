using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_StreamSizes
	{
		internal unsafe SecPkgContext_StreamSizes(byte[] memory)
		{
			fixed (byte[] array = memory)
			{
				void* value;
				if (memory == null || array.Length == 0)
				{
					value = null;
				}
				else
				{
					value = (void*)(&array[0]);
				}
				IntPtr ptr = new IntPtr(value);
				checked
				{
					try
					{
						this.cbHeader = (int)((uint)Marshal.ReadInt32(ptr));
						this.cbTrailer = (int)((uint)Marshal.ReadInt32(ptr, 4));
						this.cbMaximumMessage = (int)((uint)Marshal.ReadInt32(ptr, 8));
						this.cBuffers = (int)((uint)Marshal.ReadInt32(ptr, 12));
						this.cbBlockSize = (int)((uint)Marshal.ReadInt32(ptr, 16));
					}
					catch (OverflowException)
					{
						NetEventSource.Fail(this, "Negative size.", ".ctor");
						throw;
					}
				}
			}
		}

		public int cbHeader;

		public int cbTrailer;

		public int cbMaximumMessage;

		public int cBuffers;

		public int cbBlockSize;

		public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_StreamSizes>();
	}
}
