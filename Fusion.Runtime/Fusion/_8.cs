using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(8)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 32)]
	public struct _8 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 32;

		[FixedBuffer(typeof(uint), 8)]
		[FieldOffset(0)]
		public _8.<Data>e__FixedBuffer Data;

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

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
