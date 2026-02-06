using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(32)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 128)]
	public struct _32 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 128;

		[FixedBuffer(typeof(uint), 32)]
		[FieldOffset(0)]
		public _32.<Data>e__FixedBuffer Data;

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

		[NonSerialized]
		[FieldOffset(64)]
		private uint _data16;

		[NonSerialized]
		[FieldOffset(68)]
		private uint _data17;

		[NonSerialized]
		[FieldOffset(72)]
		private uint _data18;

		[NonSerialized]
		[FieldOffset(76)]
		private uint _data19;

		[NonSerialized]
		[FieldOffset(80)]
		private uint _data20;

		[NonSerialized]
		[FieldOffset(84)]
		private uint _data21;

		[NonSerialized]
		[FieldOffset(88)]
		private uint _data22;

		[NonSerialized]
		[FieldOffset(92)]
		private uint _data23;

		[NonSerialized]
		[FieldOffset(96)]
		private uint _data24;

		[NonSerialized]
		[FieldOffset(100)]
		private uint _data25;

		[NonSerialized]
		[FieldOffset(104)]
		private uint _data26;

		[NonSerialized]
		[FieldOffset(108)]
		private uint _data27;

		[NonSerialized]
		[FieldOffset(112)]
		private uint _data28;

		[NonSerialized]
		[FieldOffset(116)]
		private uint _data29;

		[NonSerialized]
		[FieldOffset(120)]
		private uint _data30;

		[NonSerialized]
		[FieldOffset(124)]
		private uint _data31;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
