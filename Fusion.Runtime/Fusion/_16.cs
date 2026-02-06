using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(16)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 64)]
	public struct _16 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 64;

		[FixedBuffer(typeof(uint), 16)]
		[FieldOffset(0)]
		public _16.<Data>e__FixedBuffer Data;

		[NonSerialized]
		[FieldOffset(0)]
		private uint _data0;

		[NonSerialized]
		[FieldOffset(4)]
		private uint _data1;

		[NonSerialized]
		[FieldOffset(8)]
		private uint _data2;

		[NonSerialized]
		[FieldOffset(12)]
		private uint _data3;

		[NonSerialized]
		[FieldOffset(16)]
		private uint _data4;

		[NonSerialized]
		[FieldOffset(20)]
		private uint _data5;

		[NonSerialized]
		[FieldOffset(24)]
		private uint _data6;

		[NonSerialized]
		[FieldOffset(28)]
		private uint _data7;

		[NonSerialized]
		[FieldOffset(32)]
		private uint _data8;

		[NonSerialized]
		[FieldOffset(36)]
		private uint _data9;

		[NonSerialized]
		[FieldOffset(40)]
		private uint _data10;

		[NonSerialized]
		[FieldOffset(44)]
		private uint _data11;

		[NonSerialized]
		[FieldOffset(48)]
		private uint _data12;

		[NonSerialized]
		[FieldOffset(52)]
		private uint _data13;

		[NonSerialized]
		[FieldOffset(56)]
		private uint _data14;

		[NonSerialized]
		[FieldOffset(60)]
		private uint _data15;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
