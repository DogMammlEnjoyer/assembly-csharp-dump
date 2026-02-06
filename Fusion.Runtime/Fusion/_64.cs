using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(64)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 256)]
	public struct _64 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 256;

		[FixedBuffer(typeof(uint), 64)]
		[FieldOffset(0)]
		public _64.<Data>e__FixedBuffer Data;

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

		[NonSerialized]
		[FieldOffset(128)]
		private uint _data32;

		[NonSerialized]
		[FieldOffset(132)]
		private uint _data33;

		[NonSerialized]
		[FieldOffset(136)]
		private uint _data34;

		[NonSerialized]
		[FieldOffset(140)]
		private uint _data35;

		[NonSerialized]
		[FieldOffset(144)]
		private uint _data36;

		[NonSerialized]
		[FieldOffset(148)]
		private uint _data37;

		[NonSerialized]
		[FieldOffset(152)]
		private uint _data38;

		[NonSerialized]
		[FieldOffset(156)]
		private uint _data39;

		[NonSerialized]
		[FieldOffset(160)]
		private uint _data40;

		[NonSerialized]
		[FieldOffset(164)]
		private uint _data41;

		[NonSerialized]
		[FieldOffset(168)]
		private uint _data42;

		[NonSerialized]
		[FieldOffset(172)]
		private uint _data43;

		[NonSerialized]
		[FieldOffset(176)]
		private uint _data44;

		[NonSerialized]
		[FieldOffset(180)]
		private uint _data45;

		[NonSerialized]
		[FieldOffset(184)]
		private uint _data46;

		[NonSerialized]
		[FieldOffset(188)]
		private uint _data47;

		[NonSerialized]
		[FieldOffset(192)]
		private uint _data48;

		[NonSerialized]
		[FieldOffset(196)]
		private uint _data49;

		[NonSerialized]
		[FieldOffset(200)]
		private uint _data50;

		[NonSerialized]
		[FieldOffset(204)]
		private uint _data51;

		[NonSerialized]
		[FieldOffset(208)]
		private uint _data52;

		[NonSerialized]
		[FieldOffset(212)]
		private uint _data53;

		[NonSerialized]
		[FieldOffset(216)]
		private uint _data54;

		[NonSerialized]
		[FieldOffset(220)]
		private uint _data55;

		[NonSerialized]
		[FieldOffset(224)]
		private uint _data56;

		[NonSerialized]
		[FieldOffset(228)]
		private uint _data57;

		[NonSerialized]
		[FieldOffset(232)]
		private uint _data58;

		[NonSerialized]
		[FieldOffset(236)]
		private uint _data59;

		[NonSerialized]
		[FieldOffset(240)]
		private uint _data60;

		[NonSerialized]
		[FieldOffset(244)]
		private uint _data61;

		[NonSerialized]
		[FieldOffset(248)]
		private uint _data62;

		[NonSerialized]
		[FieldOffset(252)]
		private uint _data63;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 256)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
