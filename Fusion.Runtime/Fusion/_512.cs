using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(512)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 2048)]
	public struct _512 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 2048;

		[FixedBuffer(typeof(uint), 512)]
		[FieldOffset(0)]
		public _512.<Data>e__FixedBuffer Data;

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

		[NonSerialized]
		[FieldOffset(1024)]
		private uint _data256;

		[NonSerialized]
		[FieldOffset(1028)]
		private uint _data257;

		[NonSerialized]
		[FieldOffset(1032)]
		private uint _data258;

		[NonSerialized]
		[FieldOffset(1036)]
		private uint _data259;

		[NonSerialized]
		[FieldOffset(1040)]
		private uint _data260;

		[NonSerialized]
		[FieldOffset(1044)]
		private uint _data261;

		[NonSerialized]
		[FieldOffset(1048)]
		private uint _data262;

		[NonSerialized]
		[FieldOffset(1052)]
		private uint _data263;

		[NonSerialized]
		[FieldOffset(1056)]
		private uint _data264;

		[NonSerialized]
		[FieldOffset(1060)]
		private uint _data265;

		[NonSerialized]
		[FieldOffset(1064)]
		private uint _data266;

		[NonSerialized]
		[FieldOffset(1068)]
		private uint _data267;

		[NonSerialized]
		[FieldOffset(1072)]
		private uint _data268;

		[NonSerialized]
		[FieldOffset(1076)]
		private uint _data269;

		[NonSerialized]
		[FieldOffset(1080)]
		private uint _data270;

		[NonSerialized]
		[FieldOffset(1084)]
		private uint _data271;

		[NonSerialized]
		[FieldOffset(1088)]
		private uint _data272;

		[NonSerialized]
		[FieldOffset(1092)]
		private uint _data273;

		[NonSerialized]
		[FieldOffset(1096)]
		private uint _data274;

		[NonSerialized]
		[FieldOffset(1100)]
		private uint _data275;

		[NonSerialized]
		[FieldOffset(1104)]
		private uint _data276;

		[NonSerialized]
		[FieldOffset(1108)]
		private uint _data277;

		[NonSerialized]
		[FieldOffset(1112)]
		private uint _data278;

		[NonSerialized]
		[FieldOffset(1116)]
		private uint _data279;

		[NonSerialized]
		[FieldOffset(1120)]
		private uint _data280;

		[NonSerialized]
		[FieldOffset(1124)]
		private uint _data281;

		[NonSerialized]
		[FieldOffset(1128)]
		private uint _data282;

		[NonSerialized]
		[FieldOffset(1132)]
		private uint _data283;

		[NonSerialized]
		[FieldOffset(1136)]
		private uint _data284;

		[NonSerialized]
		[FieldOffset(1140)]
		private uint _data285;

		[NonSerialized]
		[FieldOffset(1144)]
		private uint _data286;

		[NonSerialized]
		[FieldOffset(1148)]
		private uint _data287;

		[NonSerialized]
		[FieldOffset(1152)]
		private uint _data288;

		[NonSerialized]
		[FieldOffset(1156)]
		private uint _data289;

		[NonSerialized]
		[FieldOffset(1160)]
		private uint _data290;

		[NonSerialized]
		[FieldOffset(1164)]
		private uint _data291;

		[NonSerialized]
		[FieldOffset(1168)]
		private uint _data292;

		[NonSerialized]
		[FieldOffset(1172)]
		private uint _data293;

		[NonSerialized]
		[FieldOffset(1176)]
		private uint _data294;

		[NonSerialized]
		[FieldOffset(1180)]
		private uint _data295;

		[NonSerialized]
		[FieldOffset(1184)]
		private uint _data296;

		[NonSerialized]
		[FieldOffset(1188)]
		private uint _data297;

		[NonSerialized]
		[FieldOffset(1192)]
		private uint _data298;

		[NonSerialized]
		[FieldOffset(1196)]
		private uint _data299;

		[NonSerialized]
		[FieldOffset(1200)]
		private uint _data300;

		[NonSerialized]
		[FieldOffset(1204)]
		private uint _data301;

		[NonSerialized]
		[FieldOffset(1208)]
		private uint _data302;

		[NonSerialized]
		[FieldOffset(1212)]
		private uint _data303;

		[NonSerialized]
		[FieldOffset(1216)]
		private uint _data304;

		[NonSerialized]
		[FieldOffset(1220)]
		private uint _data305;

		[NonSerialized]
		[FieldOffset(1224)]
		private uint _data306;

		[NonSerialized]
		[FieldOffset(1228)]
		private uint _data307;

		[NonSerialized]
		[FieldOffset(1232)]
		private uint _data308;

		[NonSerialized]
		[FieldOffset(1236)]
		private uint _data309;

		[NonSerialized]
		[FieldOffset(1240)]
		private uint _data310;

		[NonSerialized]
		[FieldOffset(1244)]
		private uint _data311;

		[NonSerialized]
		[FieldOffset(1248)]
		private uint _data312;

		[NonSerialized]
		[FieldOffset(1252)]
		private uint _data313;

		[NonSerialized]
		[FieldOffset(1256)]
		private uint _data314;

		[NonSerialized]
		[FieldOffset(1260)]
		private uint _data315;

		[NonSerialized]
		[FieldOffset(1264)]
		private uint _data316;

		[NonSerialized]
		[FieldOffset(1268)]
		private uint _data317;

		[NonSerialized]
		[FieldOffset(1272)]
		private uint _data318;

		[NonSerialized]
		[FieldOffset(1276)]
		private uint _data319;

		[NonSerialized]
		[FieldOffset(1280)]
		private uint _data320;

		[NonSerialized]
		[FieldOffset(1284)]
		private uint _data321;

		[NonSerialized]
		[FieldOffset(1288)]
		private uint _data322;

		[NonSerialized]
		[FieldOffset(1292)]
		private uint _data323;

		[NonSerialized]
		[FieldOffset(1296)]
		private uint _data324;

		[NonSerialized]
		[FieldOffset(1300)]
		private uint _data325;

		[NonSerialized]
		[FieldOffset(1304)]
		private uint _data326;

		[NonSerialized]
		[FieldOffset(1308)]
		private uint _data327;

		[NonSerialized]
		[FieldOffset(1312)]
		private uint _data328;

		[NonSerialized]
		[FieldOffset(1316)]
		private uint _data329;

		[NonSerialized]
		[FieldOffset(1320)]
		private uint _data330;

		[NonSerialized]
		[FieldOffset(1324)]
		private uint _data331;

		[NonSerialized]
		[FieldOffset(1328)]
		private uint _data332;

		[NonSerialized]
		[FieldOffset(1332)]
		private uint _data333;

		[NonSerialized]
		[FieldOffset(1336)]
		private uint _data334;

		[NonSerialized]
		[FieldOffset(1340)]
		private uint _data335;

		[NonSerialized]
		[FieldOffset(1344)]
		private uint _data336;

		[NonSerialized]
		[FieldOffset(1348)]
		private uint _data337;

		[NonSerialized]
		[FieldOffset(1352)]
		private uint _data338;

		[NonSerialized]
		[FieldOffset(1356)]
		private uint _data339;

		[NonSerialized]
		[FieldOffset(1360)]
		private uint _data340;

		[NonSerialized]
		[FieldOffset(1364)]
		private uint _data341;

		[NonSerialized]
		[FieldOffset(1368)]
		private uint _data342;

		[NonSerialized]
		[FieldOffset(1372)]
		private uint _data343;

		[NonSerialized]
		[FieldOffset(1376)]
		private uint _data344;

		[NonSerialized]
		[FieldOffset(1380)]
		private uint _data345;

		[NonSerialized]
		[FieldOffset(1384)]
		private uint _data346;

		[NonSerialized]
		[FieldOffset(1388)]
		private uint _data347;

		[NonSerialized]
		[FieldOffset(1392)]
		private uint _data348;

		[NonSerialized]
		[FieldOffset(1396)]
		private uint _data349;

		[NonSerialized]
		[FieldOffset(1400)]
		private uint _data350;

		[NonSerialized]
		[FieldOffset(1404)]
		private uint _data351;

		[NonSerialized]
		[FieldOffset(1408)]
		private uint _data352;

		[NonSerialized]
		[FieldOffset(1412)]
		private uint _data353;

		[NonSerialized]
		[FieldOffset(1416)]
		private uint _data354;

		[NonSerialized]
		[FieldOffset(1420)]
		private uint _data355;

		[NonSerialized]
		[FieldOffset(1424)]
		private uint _data356;

		[NonSerialized]
		[FieldOffset(1428)]
		private uint _data357;

		[NonSerialized]
		[FieldOffset(1432)]
		private uint _data358;

		[NonSerialized]
		[FieldOffset(1436)]
		private uint _data359;

		[NonSerialized]
		[FieldOffset(1440)]
		private uint _data360;

		[NonSerialized]
		[FieldOffset(1444)]
		private uint _data361;

		[NonSerialized]
		[FieldOffset(1448)]
		private uint _data362;

		[NonSerialized]
		[FieldOffset(1452)]
		private uint _data363;

		[NonSerialized]
		[FieldOffset(1456)]
		private uint _data364;

		[NonSerialized]
		[FieldOffset(1460)]
		private uint _data365;

		[NonSerialized]
		[FieldOffset(1464)]
		private uint _data366;

		[NonSerialized]
		[FieldOffset(1468)]
		private uint _data367;

		[NonSerialized]
		[FieldOffset(1472)]
		private uint _data368;

		[NonSerialized]
		[FieldOffset(1476)]
		private uint _data369;

		[NonSerialized]
		[FieldOffset(1480)]
		private uint _data370;

		[NonSerialized]
		[FieldOffset(1484)]
		private uint _data371;

		[NonSerialized]
		[FieldOffset(1488)]
		private uint _data372;

		[NonSerialized]
		[FieldOffset(1492)]
		private uint _data373;

		[NonSerialized]
		[FieldOffset(1496)]
		private uint _data374;

		[NonSerialized]
		[FieldOffset(1500)]
		private uint _data375;

		[NonSerialized]
		[FieldOffset(1504)]
		private uint _data376;

		[NonSerialized]
		[FieldOffset(1508)]
		private uint _data377;

		[NonSerialized]
		[FieldOffset(1512)]
		private uint _data378;

		[NonSerialized]
		[FieldOffset(1516)]
		private uint _data379;

		[NonSerialized]
		[FieldOffset(1520)]
		private uint _data380;

		[NonSerialized]
		[FieldOffset(1524)]
		private uint _data381;

		[NonSerialized]
		[FieldOffset(1528)]
		private uint _data382;

		[NonSerialized]
		[FieldOffset(1532)]
		private uint _data383;

		[NonSerialized]
		[FieldOffset(1536)]
		private uint _data384;

		[NonSerialized]
		[FieldOffset(1540)]
		private uint _data385;

		[NonSerialized]
		[FieldOffset(1544)]
		private uint _data386;

		[NonSerialized]
		[FieldOffset(1548)]
		private uint _data387;

		[NonSerialized]
		[FieldOffset(1552)]
		private uint _data388;

		[NonSerialized]
		[FieldOffset(1556)]
		private uint _data389;

		[NonSerialized]
		[FieldOffset(1560)]
		private uint _data390;

		[NonSerialized]
		[FieldOffset(1564)]
		private uint _data391;

		[NonSerialized]
		[FieldOffset(1568)]
		private uint _data392;

		[NonSerialized]
		[FieldOffset(1572)]
		private uint _data393;

		[NonSerialized]
		[FieldOffset(1576)]
		private uint _data394;

		[NonSerialized]
		[FieldOffset(1580)]
		private uint _data395;

		[NonSerialized]
		[FieldOffset(1584)]
		private uint _data396;

		[NonSerialized]
		[FieldOffset(1588)]
		private uint _data397;

		[NonSerialized]
		[FieldOffset(1592)]
		private uint _data398;

		[NonSerialized]
		[FieldOffset(1596)]
		private uint _data399;

		[NonSerialized]
		[FieldOffset(1600)]
		private uint _data400;

		[NonSerialized]
		[FieldOffset(1604)]
		private uint _data401;

		[NonSerialized]
		[FieldOffset(1608)]
		private uint _data402;

		[NonSerialized]
		[FieldOffset(1612)]
		private uint _data403;

		[NonSerialized]
		[FieldOffset(1616)]
		private uint _data404;

		[NonSerialized]
		[FieldOffset(1620)]
		private uint _data405;

		[NonSerialized]
		[FieldOffset(1624)]
		private uint _data406;

		[NonSerialized]
		[FieldOffset(1628)]
		private uint _data407;

		[NonSerialized]
		[FieldOffset(1632)]
		private uint _data408;

		[NonSerialized]
		[FieldOffset(1636)]
		private uint _data409;

		[NonSerialized]
		[FieldOffset(1640)]
		private uint _data410;

		[NonSerialized]
		[FieldOffset(1644)]
		private uint _data411;

		[NonSerialized]
		[FieldOffset(1648)]
		private uint _data412;

		[NonSerialized]
		[FieldOffset(1652)]
		private uint _data413;

		[NonSerialized]
		[FieldOffset(1656)]
		private uint _data414;

		[NonSerialized]
		[FieldOffset(1660)]
		private uint _data415;

		[NonSerialized]
		[FieldOffset(1664)]
		private uint _data416;

		[NonSerialized]
		[FieldOffset(1668)]
		private uint _data417;

		[NonSerialized]
		[FieldOffset(1672)]
		private uint _data418;

		[NonSerialized]
		[FieldOffset(1676)]
		private uint _data419;

		[NonSerialized]
		[FieldOffset(1680)]
		private uint _data420;

		[NonSerialized]
		[FieldOffset(1684)]
		private uint _data421;

		[NonSerialized]
		[FieldOffset(1688)]
		private uint _data422;

		[NonSerialized]
		[FieldOffset(1692)]
		private uint _data423;

		[NonSerialized]
		[FieldOffset(1696)]
		private uint _data424;

		[NonSerialized]
		[FieldOffset(1700)]
		private uint _data425;

		[NonSerialized]
		[FieldOffset(1704)]
		private uint _data426;

		[NonSerialized]
		[FieldOffset(1708)]
		private uint _data427;

		[NonSerialized]
		[FieldOffset(1712)]
		private uint _data428;

		[NonSerialized]
		[FieldOffset(1716)]
		private uint _data429;

		[NonSerialized]
		[FieldOffset(1720)]
		private uint _data430;

		[NonSerialized]
		[FieldOffset(1724)]
		private uint _data431;

		[NonSerialized]
		[FieldOffset(1728)]
		private uint _data432;

		[NonSerialized]
		[FieldOffset(1732)]
		private uint _data433;

		[NonSerialized]
		[FieldOffset(1736)]
		private uint _data434;

		[NonSerialized]
		[FieldOffset(1740)]
		private uint _data435;

		[NonSerialized]
		[FieldOffset(1744)]
		private uint _data436;

		[NonSerialized]
		[FieldOffset(1748)]
		private uint _data437;

		[NonSerialized]
		[FieldOffset(1752)]
		private uint _data438;

		[NonSerialized]
		[FieldOffset(1756)]
		private uint _data439;

		[NonSerialized]
		[FieldOffset(1760)]
		private uint _data440;

		[NonSerialized]
		[FieldOffset(1764)]
		private uint _data441;

		[NonSerialized]
		[FieldOffset(1768)]
		private uint _data442;

		[NonSerialized]
		[FieldOffset(1772)]
		private uint _data443;

		[NonSerialized]
		[FieldOffset(1776)]
		private uint _data444;

		[NonSerialized]
		[FieldOffset(1780)]
		private uint _data445;

		[NonSerialized]
		[FieldOffset(1784)]
		private uint _data446;

		[NonSerialized]
		[FieldOffset(1788)]
		private uint _data447;

		[NonSerialized]
		[FieldOffset(1792)]
		private uint _data448;

		[NonSerialized]
		[FieldOffset(1796)]
		private uint _data449;

		[NonSerialized]
		[FieldOffset(1800)]
		private uint _data450;

		[NonSerialized]
		[FieldOffset(1804)]
		private uint _data451;

		[NonSerialized]
		[FieldOffset(1808)]
		private uint _data452;

		[NonSerialized]
		[FieldOffset(1812)]
		private uint _data453;

		[NonSerialized]
		[FieldOffset(1816)]
		private uint _data454;

		[NonSerialized]
		[FieldOffset(1820)]
		private uint _data455;

		[NonSerialized]
		[FieldOffset(1824)]
		private uint _data456;

		[NonSerialized]
		[FieldOffset(1828)]
		private uint _data457;

		[NonSerialized]
		[FieldOffset(1832)]
		private uint _data458;

		[NonSerialized]
		[FieldOffset(1836)]
		private uint _data459;

		[NonSerialized]
		[FieldOffset(1840)]
		private uint _data460;

		[NonSerialized]
		[FieldOffset(1844)]
		private uint _data461;

		[NonSerialized]
		[FieldOffset(1848)]
		private uint _data462;

		[NonSerialized]
		[FieldOffset(1852)]
		private uint _data463;

		[NonSerialized]
		[FieldOffset(1856)]
		private uint _data464;

		[NonSerialized]
		[FieldOffset(1860)]
		private uint _data465;

		[NonSerialized]
		[FieldOffset(1864)]
		private uint _data466;

		[NonSerialized]
		[FieldOffset(1868)]
		private uint _data467;

		[NonSerialized]
		[FieldOffset(1872)]
		private uint _data468;

		[NonSerialized]
		[FieldOffset(1876)]
		private uint _data469;

		[NonSerialized]
		[FieldOffset(1880)]
		private uint _data470;

		[NonSerialized]
		[FieldOffset(1884)]
		private uint _data471;

		[NonSerialized]
		[FieldOffset(1888)]
		private uint _data472;

		[NonSerialized]
		[FieldOffset(1892)]
		private uint _data473;

		[NonSerialized]
		[FieldOffset(1896)]
		private uint _data474;

		[NonSerialized]
		[FieldOffset(1900)]
		private uint _data475;

		[NonSerialized]
		[FieldOffset(1904)]
		private uint _data476;

		[NonSerialized]
		[FieldOffset(1908)]
		private uint _data477;

		[NonSerialized]
		[FieldOffset(1912)]
		private uint _data478;

		[NonSerialized]
		[FieldOffset(1916)]
		private uint _data479;

		[NonSerialized]
		[FieldOffset(1920)]
		private uint _data480;

		[NonSerialized]
		[FieldOffset(1924)]
		private uint _data481;

		[NonSerialized]
		[FieldOffset(1928)]
		private uint _data482;

		[NonSerialized]
		[FieldOffset(1932)]
		private uint _data483;

		[NonSerialized]
		[FieldOffset(1936)]
		private uint _data484;

		[NonSerialized]
		[FieldOffset(1940)]
		private uint _data485;

		[NonSerialized]
		[FieldOffset(1944)]
		private uint _data486;

		[NonSerialized]
		[FieldOffset(1948)]
		private uint _data487;

		[NonSerialized]
		[FieldOffset(1952)]
		private uint _data488;

		[NonSerialized]
		[FieldOffset(1956)]
		private uint _data489;

		[NonSerialized]
		[FieldOffset(1960)]
		private uint _data490;

		[NonSerialized]
		[FieldOffset(1964)]
		private uint _data491;

		[NonSerialized]
		[FieldOffset(1968)]
		private uint _data492;

		[NonSerialized]
		[FieldOffset(1972)]
		private uint _data493;

		[NonSerialized]
		[FieldOffset(1976)]
		private uint _data494;

		[NonSerialized]
		[FieldOffset(1980)]
		private uint _data495;

		[NonSerialized]
		[FieldOffset(1984)]
		private uint _data496;

		[NonSerialized]
		[FieldOffset(1988)]
		private uint _data497;

		[NonSerialized]
		[FieldOffset(1992)]
		private uint _data498;

		[NonSerialized]
		[FieldOffset(1996)]
		private uint _data499;

		[NonSerialized]
		[FieldOffset(2000)]
		private uint _data500;

		[NonSerialized]
		[FieldOffset(2004)]
		private uint _data501;

		[NonSerialized]
		[FieldOffset(2008)]
		private uint _data502;

		[NonSerialized]
		[FieldOffset(2012)]
		private uint _data503;

		[NonSerialized]
		[FieldOffset(2016)]
		private uint _data504;

		[NonSerialized]
		[FieldOffset(2020)]
		private uint _data505;

		[NonSerialized]
		[FieldOffset(2024)]
		private uint _data506;

		[NonSerialized]
		[FieldOffset(2028)]
		private uint _data507;

		[NonSerialized]
		[FieldOffset(2032)]
		private uint _data508;

		[NonSerialized]
		[FieldOffset(2036)]
		private uint _data509;

		[NonSerialized]
		[FieldOffset(2040)]
		private uint _data510;

		[NonSerialized]
		[FieldOffset(2044)]
		private uint _data511;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 2048)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
