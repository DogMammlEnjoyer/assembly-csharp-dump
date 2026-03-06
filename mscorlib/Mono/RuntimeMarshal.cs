using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono
{
	internal static class RuntimeMarshal
	{
		internal unsafe static string PtrToUtf8String(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
			{
				return string.Empty;
			}
			byte* ptr2 = (byte*)((void*)ptr);
			int num = 0;
			try
			{
				while (*(ptr2++) != 0)
				{
					num++;
				}
			}
			catch (NullReferenceException)
			{
				throw new ArgumentOutOfRangeException("ptr", "Value does not refer to a valid string.");
			}
			return new string((sbyte*)((void*)ptr), 0, num, Encoding.UTF8);
		}

		internal static SafeStringMarshal MarshalString(string str)
		{
			return new SafeStringMarshal(str);
		}

		private unsafe static int DecodeBlobSize(IntPtr in_ptr, out IntPtr out_ptr)
		{
			byte* ptr = (byte*)((void*)in_ptr);
			uint result;
			if ((*ptr & 128) == 0)
			{
				result = (uint)(*ptr & 127);
				ptr++;
			}
			else if ((*ptr & 64) == 0)
			{
				result = (uint)(((int)(*ptr & 63) << 8) + (int)ptr[1]);
				ptr += 2;
			}
			else
			{
				result = (uint)(((int)(*ptr & 31) << 24) + ((int)ptr[1] << 16) + ((int)ptr[2] << 8) + (int)ptr[3]);
				ptr += 4;
			}
			out_ptr = (IntPtr)((void*)ptr);
			return (int)result;
		}

		internal static byte[] DecodeBlobArray(IntPtr ptr)
		{
			IntPtr source;
			int num = RuntimeMarshal.DecodeBlobSize(ptr, out source);
			byte[] array = new byte[num];
			Marshal.Copy(source, array, 0, num);
			return array;
		}

		internal static int AsciHexDigitValue(int c)
		{
			if (c >= 48 && c <= 57)
			{
				return c - 48;
			}
			if (c >= 97 && c <= 102)
			{
				return c - 97 + 10;
			}
			return c - 65 + 10;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FreeAssemblyName(ref MonoAssemblyName name, bool freeStruct);
	}
}
