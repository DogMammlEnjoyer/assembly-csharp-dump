using System;
using System.Runtime.InteropServices;

namespace Mono
{
	internal class CFNumber : CFObject
	{
		public CFNumber(IntPtr handle, bool own) : base(handle, own)
		{
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CFNumberGetValue(IntPtr handle, IntPtr type, [MarshalAs(UnmanagedType.I1)] out bool value);

		public static bool AsBool(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return false;
			}
			bool result;
			CFNumber.CFNumberGetValue(handle, (IntPtr)1, out result);
			return result;
		}

		public static implicit operator bool(CFNumber number)
		{
			return CFNumber.AsBool(number.Handle);
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool CFNumberGetValue(IntPtr handle, IntPtr type, out int value);

		public static int AsInt32(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return 0;
			}
			int result;
			CFNumber.CFNumberGetValue(handle, (IntPtr)9, out result);
			return result;
		}

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFNumberCreate(IntPtr allocator, IntPtr theType, IntPtr valuePtr);

		public static CFNumber FromInt32(int number)
		{
			return new CFNumber(CFNumber.CFNumberCreate(IntPtr.Zero, (IntPtr)9, (IntPtr)number), true);
		}

		public static implicit operator int(CFNumber number)
		{
			return CFNumber.AsInt32(number.Handle);
		}
	}
}
