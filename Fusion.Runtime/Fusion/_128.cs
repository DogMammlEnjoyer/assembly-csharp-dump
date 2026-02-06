using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(128)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 512)]
	public struct _128 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 512;

		[FixedBuffer(typeof(uint), 128)]
		[FieldOffset(0)]
		public _128.<Data>e__FixedBuffer Data;

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

		[NonSerialized]
		[FieldOffset(256)]
		private uint _data64;

		[NonSerialized]
		[FieldOffset(260)]
		private uint _data65;

		[NonSerialized]
		[FieldOffset(264)]
		private uint _data66;

		[NonSerialized]
		[FieldOffset(268)]
		private uint _data67;

		[NonSerialized]
		[FieldOffset(272)]
		private uint _data68;

		[NonSerialized]
		[FieldOffset(276)]
		private uint _data69;

		[NonSerialized]
		[FieldOffset(280)]
		private uint _data70;

		[NonSerialized]
		[FieldOffset(284)]
		private uint _data71;

		[NonSerialized]
		[FieldOffset(288)]
		private uint _data72;

		[NonSerialized]
		[FieldOffset(292)]
		private uint _data73;

		[NonSerialized]
		[FieldOffset(296)]
		private uint _data74;

		[NonSerialized]
		[FieldOffset(300)]
		private uint _data75;

		[NonSerialized]
		[FieldOffset(304)]
		private uint _data76;

		[NonSerialized]
		[FieldOffset(308)]
		private uint _data77;

		[NonSerialized]
		[FieldOffset(312)]
		private uint _data78;

		[NonSerialized]
		[FieldOffset(316)]
		private uint _data79;

		[NonSerialized]
		[FieldOffset(320)]
		private uint _data80;

		[NonSerialized]
		[FieldOffset(324)]
		private uint _data81;

		[NonSerialized]
		[FieldOffset(328)]
		private uint _data82;

		[NonSerialized]
		[FieldOffset(332)]
		private uint _data83;

		[NonSerialized]
		[FieldOffset(336)]
		private uint _data84;

		[NonSerialized]
		[FieldOffset(340)]
		private uint _data85;

		[NonSerialized]
		[FieldOffset(344)]
		private uint _data86;

		[NonSerialized]
		[FieldOffset(348)]
		private uint _data87;

		[NonSerialized]
		[FieldOffset(352)]
		private uint _data88;

		[NonSerialized]
		[FieldOffset(356)]
		private uint _data89;

		[NonSerialized]
		[FieldOffset(360)]
		private uint _data90;

		[NonSerialized]
		[FieldOffset(364)]
		private uint _data91;

		[NonSerialized]
		[FieldOffset(368)]
		private uint _data92;

		[NonSerialized]
		[FieldOffset(372)]
		private uint _data93;

		[NonSerialized]
		[FieldOffset(376)]
		private uint _data94;

		[NonSerialized]
		[FieldOffset(380)]
		private uint _data95;

		[NonSerialized]
		[FieldOffset(384)]
		private uint _data96;

		[NonSerialized]
		[FieldOffset(388)]
		private uint _data97;

		[NonSerialized]
		[FieldOffset(392)]
		private uint _data98;

		[NonSerialized]
		[FieldOffset(396)]
		private uint _data99;

		[NonSerialized]
		[FieldOffset(400)]
		private uint _data100;

		[NonSerialized]
		[FieldOffset(404)]
		private uint _data101;

		[NonSerialized]
		[FieldOffset(408)]
		private uint _data102;

		[NonSerialized]
		[FieldOffset(412)]
		private uint _data103;

		[NonSerialized]
		[FieldOffset(416)]
		private uint _data104;

		[NonSerialized]
		[FieldOffset(420)]
		private uint _data105;

		[NonSerialized]
		[FieldOffset(424)]
		private uint _data106;

		[NonSerialized]
		[FieldOffset(428)]
		private uint _data107;

		[NonSerialized]
		[FieldOffset(432)]
		private uint _data108;

		[NonSerialized]
		[FieldOffset(436)]
		private uint _data109;

		[NonSerialized]
		[FieldOffset(440)]
		private uint _data110;

		[NonSerialized]
		[FieldOffset(444)]
		private uint _data111;

		[NonSerialized]
		[FieldOffset(448)]
		private uint _data112;

		[NonSerialized]
		[FieldOffset(452)]
		private uint _data113;

		[NonSerialized]
		[FieldOffset(456)]
		private uint _data114;

		[NonSerialized]
		[FieldOffset(460)]
		private uint _data115;

		[NonSerialized]
		[FieldOffset(464)]
		private uint _data116;

		[NonSerialized]
		[FieldOffset(468)]
		private uint _data117;

		[NonSerialized]
		[FieldOffset(472)]
		private uint _data118;

		[NonSerialized]
		[FieldOffset(476)]
		private uint _data119;

		[NonSerialized]
		[FieldOffset(480)]
		private uint _data120;

		[NonSerialized]
		[FieldOffset(484)]
		private uint _data121;

		[NonSerialized]
		[FieldOffset(488)]
		private uint _data122;

		[NonSerialized]
		[FieldOffset(492)]
		private uint _data123;

		[NonSerialized]
		[FieldOffset(496)]
		private uint _data124;

		[NonSerialized]
		[FieldOffset(500)]
		private uint _data125;

		[NonSerialized]
		[FieldOffset(504)]
		private uint _data126;

		[NonSerialized]
		[FieldOffset(508)]
		private uint _data127;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 512)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
