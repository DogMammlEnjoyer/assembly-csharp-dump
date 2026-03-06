using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ReliableKey
	{
		public unsafe void GetInts(out int key0, out int key1, out int key2, out int key3)
		{
			ReliableKey reliableKey = this;
			key0 = *(int*)(&reliableKey.Data.FixedElementField);
			key1 = *(ref reliableKey.Data.FixedElementField + 4);
			key2 = *(ref reliableKey.Data.FixedElementField + 8);
			key3 = *(ref reliableKey.Data.FixedElementField + 12);
		}

		public unsafe void GetUlongs(out ulong key0, out ulong key1)
		{
			ReliableKey reliableKey = this;
			key0 = (ulong)(*(long*)(&reliableKey.Data.FixedElementField));
			key1 = (ulong)(*(ref reliableKey.Data.FixedElementField + 8));
		}

		public unsafe static ReliableKey FromInts(int key0 = 0, int key1 = 0, int key2 = 0, int key3 = 0)
		{
			ReliableKey result = default(ReliableKey);
			*(int*)(&result.Data.FixedElementField) = key0;
			*(ref result.Data.FixedElementField + 4) = key1;
			*(ref result.Data.FixedElementField + 8) = key2;
			*(ref result.Data.FixedElementField + 12) = key3;
			return result;
		}

		public unsafe static ReliableKey FromULongs(ulong key0 = 0UL, ulong key1 = 0UL)
		{
			ReliableKey result = default(ReliableKey);
			*(long*)(&result.Data.FixedElementField) = (long)key0;
			*(ref result.Data.FixedElementField + 8) = (long)key1;
			return result;
		}

		public const int SIZE = 16;

		[FixedBuffer(typeof(byte), 16)]
		[FieldOffset(0)]
		public ReliableKey.<Data>e__FixedBuffer Data;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Data>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
