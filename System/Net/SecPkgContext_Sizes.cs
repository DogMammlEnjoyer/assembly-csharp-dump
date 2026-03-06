using System;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecPkgContext_Sizes
	{
		internal unsafe SecPkgContext_Sizes(byte[] memory)
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
						this.cbMaxToken = (int)((uint)Marshal.ReadInt32(ptr));
						this.cbMaxSignature = (int)((uint)Marshal.ReadInt32(ptr, 4));
						this.cbBlockSize = (int)((uint)Marshal.ReadInt32(ptr, 8));
						this.cbSecurityTrailer = (int)((uint)Marshal.ReadInt32(ptr, 12));
					}
					catch (OverflowException)
					{
						NetEventSource.Fail(this, "Negative size.", ".ctor");
						throw;
					}
				}
			}
		}

		public readonly int cbMaxToken;

		public readonly int cbMaxSignature;

		public readonly int cbBlockSize;

		public readonly int cbSecurityTrailer;

		public static readonly int SizeOf = Marshal.SizeOf<SecPkgContext_Sizes>();
	}
}
