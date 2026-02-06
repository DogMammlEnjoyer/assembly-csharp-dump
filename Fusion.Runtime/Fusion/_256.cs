using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(256)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 1024)]
	public struct _256 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 1024;

		[FixedBuffer(typeof(uint), 256)]
		[FieldOffset(0)]
		public _256.<Data>e__FixedBuffer Data;

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

		[NonSerialized]
		[FieldOffset(512)]
		private uint _data128;

		[NonSerialized]
		[FieldOffset(516)]
		private uint _data129;

		[NonSerialized]
		[FieldOffset(520)]
		private uint _data130;

		[NonSerialized]
		[FieldOffset(524)]
		private uint _data131;

		[NonSerialized]
		[FieldOffset(528)]
		private uint _data132;

		[NonSerialized]
		[FieldOffset(532)]
		private uint _data133;

		[NonSerialized]
		[FieldOffset(536)]
		private uint _data134;

		[NonSerialized]
		[FieldOffset(540)]
		private uint _data135;

		[NonSerialized]
		[FieldOffset(544)]
		private uint _data136;

		[NonSerialized]
		[FieldOffset(548)]
		private uint _data137;

		[NonSerialized]
		[FieldOffset(552)]
		private uint _data138;

		[NonSerialized]
		[FieldOffset(556)]
		private uint _data139;

		[NonSerialized]
		[FieldOffset(560)]
		private uint _data140;

		[NonSerialized]
		[FieldOffset(564)]
		private uint _data141;

		[NonSerialized]
		[FieldOffset(568)]
		private uint _data142;

		[NonSerialized]
		[FieldOffset(572)]
		private uint _data143;

		[NonSerialized]
		[FieldOffset(576)]
		private uint _data144;

		[NonSerialized]
		[FieldOffset(580)]
		private uint _data145;

		[NonSerialized]
		[FieldOffset(584)]
		private uint _data146;

		[NonSerialized]
		[FieldOffset(588)]
		private uint _data147;

		[NonSerialized]
		[FieldOffset(592)]
		private uint _data148;

		[NonSerialized]
		[FieldOffset(596)]
		private uint _data149;

		[NonSerialized]
		[FieldOffset(600)]
		private uint _data150;

		[NonSerialized]
		[FieldOffset(604)]
		private uint _data151;

		[NonSerialized]
		[FieldOffset(608)]
		private uint _data152;

		[NonSerialized]
		[FieldOffset(612)]
		private uint _data153;

		[NonSerialized]
		[FieldOffset(616)]
		private uint _data154;

		[NonSerialized]
		[FieldOffset(620)]
		private uint _data155;

		[NonSerialized]
		[FieldOffset(624)]
		private uint _data156;

		[NonSerialized]
		[FieldOffset(628)]
		private uint _data157;

		[NonSerialized]
		[FieldOffset(632)]
		private uint _data158;

		[NonSerialized]
		[FieldOffset(636)]
		private uint _data159;

		[NonSerialized]
		[FieldOffset(640)]
		private uint _data160;

		[NonSerialized]
		[FieldOffset(644)]
		private uint _data161;

		[NonSerialized]
		[FieldOffset(648)]
		private uint _data162;

		[NonSerialized]
		[FieldOffset(652)]
		private uint _data163;

		[NonSerialized]
		[FieldOffset(656)]
		private uint _data164;

		[NonSerialized]
		[FieldOffset(660)]
		private uint _data165;

		[NonSerialized]
		[FieldOffset(664)]
		private uint _data166;

		[NonSerialized]
		[FieldOffset(668)]
		private uint _data167;

		[NonSerialized]
		[FieldOffset(672)]
		private uint _data168;

		[NonSerialized]
		[FieldOffset(676)]
		private uint _data169;

		[NonSerialized]
		[FieldOffset(680)]
		private uint _data170;

		[NonSerialized]
		[FieldOffset(684)]
		private uint _data171;

		[NonSerialized]
		[FieldOffset(688)]
		private uint _data172;

		[NonSerialized]
		[FieldOffset(692)]
		private uint _data173;

		[NonSerialized]
		[FieldOffset(696)]
		private uint _data174;

		[NonSerialized]
		[FieldOffset(700)]
		private uint _data175;

		[NonSerialized]
		[FieldOffset(704)]
		private uint _data176;

		[NonSerialized]
		[FieldOffset(708)]
		private uint _data177;

		[NonSerialized]
		[FieldOffset(712)]
		private uint _data178;

		[NonSerialized]
		[FieldOffset(716)]
		private uint _data179;

		[NonSerialized]
		[FieldOffset(720)]
		private uint _data180;

		[NonSerialized]
		[FieldOffset(724)]
		private uint _data181;

		[NonSerialized]
		[FieldOffset(728)]
		private uint _data182;

		[NonSerialized]
		[FieldOffset(732)]
		private uint _data183;

		[NonSerialized]
		[FieldOffset(736)]
		private uint _data184;

		[NonSerialized]
		[FieldOffset(740)]
		private uint _data185;

		[NonSerialized]
		[FieldOffset(744)]
		private uint _data186;

		[NonSerialized]
		[FieldOffset(748)]
		private uint _data187;

		[NonSerialized]
		[FieldOffset(752)]
		private uint _data188;

		[NonSerialized]
		[FieldOffset(756)]
		private uint _data189;

		[NonSerialized]
		[FieldOffset(760)]
		private uint _data190;

		[NonSerialized]
		[FieldOffset(764)]
		private uint _data191;

		[NonSerialized]
		[FieldOffset(768)]
		private uint _data192;

		[NonSerialized]
		[FieldOffset(772)]
		private uint _data193;

		[NonSerialized]
		[FieldOffset(776)]
		private uint _data194;

		[NonSerialized]
		[FieldOffset(780)]
		private uint _data195;

		[NonSerialized]
		[FieldOffset(784)]
		private uint _data196;

		[NonSerialized]
		[FieldOffset(788)]
		private uint _data197;

		[NonSerialized]
		[FieldOffset(792)]
		private uint _data198;

		[NonSerialized]
		[FieldOffset(796)]
		private uint _data199;

		[NonSerialized]
		[FieldOffset(800)]
		private uint _data200;

		[NonSerialized]
		[FieldOffset(804)]
		private uint _data201;

		[NonSerialized]
		[FieldOffset(808)]
		private uint _data202;

		[NonSerialized]
		[FieldOffset(812)]
		private uint _data203;

		[NonSerialized]
		[FieldOffset(816)]
		private uint _data204;

		[NonSerialized]
		[FieldOffset(820)]
		private uint _data205;

		[NonSerialized]
		[FieldOffset(824)]
		private uint _data206;

		[NonSerialized]
		[FieldOffset(828)]
		private uint _data207;

		[NonSerialized]
		[FieldOffset(832)]
		private uint _data208;

		[NonSerialized]
		[FieldOffset(836)]
		private uint _data209;

		[NonSerialized]
		[FieldOffset(840)]
		private uint _data210;

		[NonSerialized]
		[FieldOffset(844)]
		private uint _data211;

		[NonSerialized]
		[FieldOffset(848)]
		private uint _data212;

		[NonSerialized]
		[FieldOffset(852)]
		private uint _data213;

		[NonSerialized]
		[FieldOffset(856)]
		private uint _data214;

		[NonSerialized]
		[FieldOffset(860)]
		private uint _data215;

		[NonSerialized]
		[FieldOffset(864)]
		private uint _data216;

		[NonSerialized]
		[FieldOffset(868)]
		private uint _data217;

		[NonSerialized]
		[FieldOffset(872)]
		private uint _data218;

		[NonSerialized]
		[FieldOffset(876)]
		private uint _data219;

		[NonSerialized]
		[FieldOffset(880)]
		private uint _data220;

		[NonSerialized]
		[FieldOffset(884)]
		private uint _data221;

		[NonSerialized]
		[FieldOffset(888)]
		private uint _data222;

		[NonSerialized]
		[FieldOffset(892)]
		private uint _data223;

		[NonSerialized]
		[FieldOffset(896)]
		private uint _data224;

		[NonSerialized]
		[FieldOffset(900)]
		private uint _data225;

		[NonSerialized]
		[FieldOffset(904)]
		private uint _data226;

		[NonSerialized]
		[FieldOffset(908)]
		private uint _data227;

		[NonSerialized]
		[FieldOffset(912)]
		private uint _data228;

		[NonSerialized]
		[FieldOffset(916)]
		private uint _data229;

		[NonSerialized]
		[FieldOffset(920)]
		private uint _data230;

		[NonSerialized]
		[FieldOffset(924)]
		private uint _data231;

		[NonSerialized]
		[FieldOffset(928)]
		private uint _data232;

		[NonSerialized]
		[FieldOffset(932)]
		private uint _data233;

		[NonSerialized]
		[FieldOffset(936)]
		private uint _data234;

		[NonSerialized]
		[FieldOffset(940)]
		private uint _data235;

		[NonSerialized]
		[FieldOffset(944)]
		private uint _data236;

		[NonSerialized]
		[FieldOffset(948)]
		private uint _data237;

		[NonSerialized]
		[FieldOffset(952)]
		private uint _data238;

		[NonSerialized]
		[FieldOffset(956)]
		private uint _data239;

		[NonSerialized]
		[FieldOffset(960)]
		private uint _data240;

		[NonSerialized]
		[FieldOffset(964)]
		private uint _data241;

		[NonSerialized]
		[FieldOffset(968)]
		private uint _data242;

		[NonSerialized]
		[FieldOffset(972)]
		private uint _data243;

		[NonSerialized]
		[FieldOffset(976)]
		private uint _data244;

		[NonSerialized]
		[FieldOffset(980)]
		private uint _data245;

		[NonSerialized]
		[FieldOffset(984)]
		private uint _data246;

		[NonSerialized]
		[FieldOffset(988)]
		private uint _data247;

		[NonSerialized]
		[FieldOffset(992)]
		private uint _data248;

		[NonSerialized]
		[FieldOffset(996)]
		private uint _data249;

		[NonSerialized]
		[FieldOffset(1000)]
		private uint _data250;

		[NonSerialized]
		[FieldOffset(1004)]
		private uint _data251;

		[NonSerialized]
		[FieldOffset(1008)]
		private uint _data252;

		[NonSerialized]
		[FieldOffset(1012)]
		private uint _data253;

		[NonSerialized]
		[FieldOffset(1016)]
		private uint _data254;

		[NonSerialized]
		[FieldOffset(1020)]
		private uint _data255;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1024)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
