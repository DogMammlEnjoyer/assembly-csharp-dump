using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal class FastKeyboard : Keyboard
	{
		public FastKeyboard()
		{
			InputControlExtensions.DeviceBuilder deviceBuilder = this.Setup(127, 15, 7).WithName("Keyboard").WithDisplayName("Keyboard").WithChildren(0, 127).WithLayout(new InternedString("Keyboard")).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1262836051),
				sizeInBits = 128U
			});
			InternedString kAnyKeyLayout = new InternedString("AnyKey");
			InternedString kKeyLayout = new InternedString("Key");
			InternedString kDiscreteButtonLayout = new InternedString("DiscreteButton");
			InternedString kButtonLayout = new InternedString("Button");
			AnyKeyControl anyKey = this.Initialize_ctrlKeyboardanyKey(kAnyKeyLayout, this);
			KeyControl keyControl = this.Initialize_ctrlKeyboardescape(kKeyLayout, this);
			KeyControl keyControl2 = this.Initialize_ctrlKeyboardspace(kKeyLayout, this);
			KeyControl keyControl3 = this.Initialize_ctrlKeyboardenter(kKeyLayout, this);
			KeyControl keyControl4 = this.Initialize_ctrlKeyboardtab(kKeyLayout, this);
			KeyControl keyControl5 = this.Initialize_ctrlKeyboardbackquote(kKeyLayout, this);
			KeyControl keyControl6 = this.Initialize_ctrlKeyboardquote(kKeyLayout, this);
			KeyControl keyControl7 = this.Initialize_ctrlKeyboardsemicolon(kKeyLayout, this);
			KeyControl keyControl8 = this.Initialize_ctrlKeyboardcomma(kKeyLayout, this);
			KeyControl keyControl9 = this.Initialize_ctrlKeyboardperiod(kKeyLayout, this);
			KeyControl keyControl10 = this.Initialize_ctrlKeyboardslash(kKeyLayout, this);
			KeyControl keyControl11 = this.Initialize_ctrlKeyboardbackslash(kKeyLayout, this);
			KeyControl keyControl12 = this.Initialize_ctrlKeyboardleftBracket(kKeyLayout, this);
			KeyControl keyControl13 = this.Initialize_ctrlKeyboardrightBracket(kKeyLayout, this);
			KeyControl keyControl14 = this.Initialize_ctrlKeyboardminus(kKeyLayout, this);
			KeyControl keyControl15 = this.Initialize_ctrlKeyboardequals(kKeyLayout, this);
			KeyControl keyControl16 = this.Initialize_ctrlKeyboardupArrow(kKeyLayout, this);
			KeyControl keyControl17 = this.Initialize_ctrlKeyboarddownArrow(kKeyLayout, this);
			KeyControl keyControl18 = this.Initialize_ctrlKeyboardleftArrow(kKeyLayout, this);
			KeyControl keyControl19 = this.Initialize_ctrlKeyboardrightArrow(kKeyLayout, this);
			KeyControl keyControl20 = this.Initialize_ctrlKeyboarda(kKeyLayout, this);
			KeyControl keyControl21 = this.Initialize_ctrlKeyboardb(kKeyLayout, this);
			KeyControl keyControl22 = this.Initialize_ctrlKeyboardc(kKeyLayout, this);
			KeyControl keyControl23 = this.Initialize_ctrlKeyboardd(kKeyLayout, this);
			KeyControl keyControl24 = this.Initialize_ctrlKeyboarde(kKeyLayout, this);
			KeyControl keyControl25 = this.Initialize_ctrlKeyboardf(kKeyLayout, this);
			KeyControl keyControl26 = this.Initialize_ctrlKeyboardg(kKeyLayout, this);
			KeyControl keyControl27 = this.Initialize_ctrlKeyboardh(kKeyLayout, this);
			KeyControl keyControl28 = this.Initialize_ctrlKeyboardi(kKeyLayout, this);
			KeyControl keyControl29 = this.Initialize_ctrlKeyboardj(kKeyLayout, this);
			KeyControl keyControl30 = this.Initialize_ctrlKeyboardk(kKeyLayout, this);
			KeyControl keyControl31 = this.Initialize_ctrlKeyboardl(kKeyLayout, this);
			KeyControl keyControl32 = this.Initialize_ctrlKeyboardm(kKeyLayout, this);
			KeyControl keyControl33 = this.Initialize_ctrlKeyboardn(kKeyLayout, this);
			KeyControl keyControl34 = this.Initialize_ctrlKeyboardo(kKeyLayout, this);
			KeyControl keyControl35 = this.Initialize_ctrlKeyboardp(kKeyLayout, this);
			KeyControl keyControl36 = this.Initialize_ctrlKeyboardq(kKeyLayout, this);
			KeyControl keyControl37 = this.Initialize_ctrlKeyboardr(kKeyLayout, this);
			KeyControl keyControl38 = this.Initialize_ctrlKeyboards(kKeyLayout, this);
			KeyControl keyControl39 = this.Initialize_ctrlKeyboardt(kKeyLayout, this);
			KeyControl keyControl40 = this.Initialize_ctrlKeyboardu(kKeyLayout, this);
			KeyControl keyControl41 = this.Initialize_ctrlKeyboardv(kKeyLayout, this);
			KeyControl keyControl42 = this.Initialize_ctrlKeyboardw(kKeyLayout, this);
			KeyControl keyControl43 = this.Initialize_ctrlKeyboardx(kKeyLayout, this);
			KeyControl keyControl44 = this.Initialize_ctrlKeyboardy(kKeyLayout, this);
			KeyControl keyControl45 = this.Initialize_ctrlKeyboardz(kKeyLayout, this);
			KeyControl keyControl46 = this.Initialize_ctrlKeyboard1(kKeyLayout, this);
			KeyControl keyControl47 = this.Initialize_ctrlKeyboard2(kKeyLayout, this);
			KeyControl keyControl48 = this.Initialize_ctrlKeyboard3(kKeyLayout, this);
			KeyControl keyControl49 = this.Initialize_ctrlKeyboard4(kKeyLayout, this);
			KeyControl keyControl50 = this.Initialize_ctrlKeyboard5(kKeyLayout, this);
			KeyControl keyControl51 = this.Initialize_ctrlKeyboard6(kKeyLayout, this);
			KeyControl keyControl52 = this.Initialize_ctrlKeyboard7(kKeyLayout, this);
			KeyControl keyControl53 = this.Initialize_ctrlKeyboard8(kKeyLayout, this);
			KeyControl keyControl54 = this.Initialize_ctrlKeyboard9(kKeyLayout, this);
			KeyControl keyControl55 = this.Initialize_ctrlKeyboard0(kKeyLayout, this);
			KeyControl keyControl56 = this.Initialize_ctrlKeyboardleftShift(kKeyLayout, this);
			KeyControl keyControl57 = this.Initialize_ctrlKeyboardrightShift(kKeyLayout, this);
			DiscreteButtonControl discreteButtonControl = this.Initialize_ctrlKeyboardshift(kDiscreteButtonLayout, this);
			KeyControl keyControl58 = this.Initialize_ctrlKeyboardleftAlt(kKeyLayout, this);
			KeyControl keyControl59 = this.Initialize_ctrlKeyboardrightAlt(kKeyLayout, this);
			DiscreteButtonControl discreteButtonControl2 = this.Initialize_ctrlKeyboardalt(kDiscreteButtonLayout, this);
			KeyControl keyControl60 = this.Initialize_ctrlKeyboardleftCtrl(kKeyLayout, this);
			KeyControl keyControl61 = this.Initialize_ctrlKeyboardrightCtrl(kKeyLayout, this);
			DiscreteButtonControl discreteButtonControl3 = this.Initialize_ctrlKeyboardctrl(kDiscreteButtonLayout, this);
			KeyControl keyControl62 = this.Initialize_ctrlKeyboardleftMeta(kKeyLayout, this);
			KeyControl keyControl63 = this.Initialize_ctrlKeyboardrightMeta(kKeyLayout, this);
			KeyControl keyControl64 = this.Initialize_ctrlKeyboardcontextMenu(kKeyLayout, this);
			KeyControl keyControl65 = this.Initialize_ctrlKeyboardbackspace(kKeyLayout, this);
			KeyControl keyControl66 = this.Initialize_ctrlKeyboardpageDown(kKeyLayout, this);
			KeyControl keyControl67 = this.Initialize_ctrlKeyboardpageUp(kKeyLayout, this);
			KeyControl keyControl68 = this.Initialize_ctrlKeyboardhome(kKeyLayout, this);
			KeyControl keyControl69 = this.Initialize_ctrlKeyboardend(kKeyLayout, this);
			KeyControl keyControl70 = this.Initialize_ctrlKeyboardinsert(kKeyLayout, this);
			KeyControl keyControl71 = this.Initialize_ctrlKeyboarddelete(kKeyLayout, this);
			KeyControl keyControl72 = this.Initialize_ctrlKeyboardcapsLock(kKeyLayout, this);
			KeyControl keyControl73 = this.Initialize_ctrlKeyboardnumLock(kKeyLayout, this);
			KeyControl keyControl74 = this.Initialize_ctrlKeyboardprintScreen(kKeyLayout, this);
			KeyControl keyControl75 = this.Initialize_ctrlKeyboardscrollLock(kKeyLayout, this);
			KeyControl keyControl76 = this.Initialize_ctrlKeyboardpause(kKeyLayout, this);
			KeyControl keyControl77 = this.Initialize_ctrlKeyboardnumpadEnter(kKeyLayout, this);
			KeyControl keyControl78 = this.Initialize_ctrlKeyboardnumpadDivide(kKeyLayout, this);
			KeyControl keyControl79 = this.Initialize_ctrlKeyboardnumpadMultiply(kKeyLayout, this);
			KeyControl keyControl80 = this.Initialize_ctrlKeyboardnumpadPlus(kKeyLayout, this);
			KeyControl keyControl81 = this.Initialize_ctrlKeyboardnumpadMinus(kKeyLayout, this);
			KeyControl keyControl82 = this.Initialize_ctrlKeyboardnumpadPeriod(kKeyLayout, this);
			KeyControl keyControl83 = this.Initialize_ctrlKeyboardnumpadEquals(kKeyLayout, this);
			KeyControl keyControl84 = this.Initialize_ctrlKeyboardnumpad1(kKeyLayout, this);
			KeyControl keyControl85 = this.Initialize_ctrlKeyboardnumpad2(kKeyLayout, this);
			KeyControl keyControl86 = this.Initialize_ctrlKeyboardnumpad3(kKeyLayout, this);
			KeyControl keyControl87 = this.Initialize_ctrlKeyboardnumpad4(kKeyLayout, this);
			KeyControl keyControl88 = this.Initialize_ctrlKeyboardnumpad5(kKeyLayout, this);
			KeyControl keyControl89 = this.Initialize_ctrlKeyboardnumpad6(kKeyLayout, this);
			KeyControl keyControl90 = this.Initialize_ctrlKeyboardnumpad7(kKeyLayout, this);
			KeyControl keyControl91 = this.Initialize_ctrlKeyboardnumpad8(kKeyLayout, this);
			KeyControl keyControl92 = this.Initialize_ctrlKeyboardnumpad9(kKeyLayout, this);
			KeyControl keyControl93 = this.Initialize_ctrlKeyboardnumpad0(kKeyLayout, this);
			KeyControl keyControl94 = this.Initialize_ctrlKeyboardf1(kKeyLayout, this);
			KeyControl keyControl95 = this.Initialize_ctrlKeyboardf2(kKeyLayout, this);
			KeyControl keyControl96 = this.Initialize_ctrlKeyboardf3(kKeyLayout, this);
			KeyControl keyControl97 = this.Initialize_ctrlKeyboardf4(kKeyLayout, this);
			KeyControl keyControl98 = this.Initialize_ctrlKeyboardf5(kKeyLayout, this);
			KeyControl keyControl99 = this.Initialize_ctrlKeyboardf6(kKeyLayout, this);
			KeyControl keyControl100 = this.Initialize_ctrlKeyboardf7(kKeyLayout, this);
			KeyControl keyControl101 = this.Initialize_ctrlKeyboardf8(kKeyLayout, this);
			KeyControl keyControl102 = this.Initialize_ctrlKeyboardf9(kKeyLayout, this);
			KeyControl keyControl103 = this.Initialize_ctrlKeyboardf10(kKeyLayout, this);
			KeyControl keyControl104 = this.Initialize_ctrlKeyboardf11(kKeyLayout, this);
			KeyControl keyControl105 = this.Initialize_ctrlKeyboardf12(kKeyLayout, this);
			KeyControl keyControl106 = this.Initialize_ctrlKeyboardOEM1(kKeyLayout, this);
			KeyControl keyControl107 = this.Initialize_ctrlKeyboardOEM2(kKeyLayout, this);
			KeyControl keyControl108 = this.Initialize_ctrlKeyboardOEM3(kKeyLayout, this);
			KeyControl keyControl109 = this.Initialize_ctrlKeyboardOEM4(kKeyLayout, this);
			KeyControl keyControl110 = this.Initialize_ctrlKeyboardOEM5(kKeyLayout, this);
			KeyControl keyControl111 = this.Initialize_ctrlKeyboardf13(kKeyLayout, this);
			KeyControl keyControl112 = this.Initialize_ctrlKeyboardf14(kKeyLayout, this);
			KeyControl keyControl113 = this.Initialize_ctrlKeyboardf15(kKeyLayout, this);
			KeyControl keyControl114 = this.Initialize_ctrlKeyboardf16(kKeyLayout, this);
			KeyControl keyControl115 = this.Initialize_ctrlKeyboardf17(kKeyLayout, this);
			KeyControl keyControl116 = this.Initialize_ctrlKeyboardf18(kKeyLayout, this);
			KeyControl keyControl117 = this.Initialize_ctrlKeyboardf19(kKeyLayout, this);
			KeyControl keyControl118 = this.Initialize_ctrlKeyboardf20(kKeyLayout, this);
			KeyControl keyControl119 = this.Initialize_ctrlKeyboardf21(kKeyLayout, this);
			KeyControl keyControl120 = this.Initialize_ctrlKeyboardf22(kKeyLayout, this);
			KeyControl keyControl121 = this.Initialize_ctrlKeyboardf23(kKeyLayout, this);
			KeyControl keyControl122 = this.Initialize_ctrlKeyboardf24(kKeyLayout, this);
			ButtonControl imeSelected = this.Initialize_ctrlKeyboardIMESelected(kButtonLayout, this);
			deviceBuilder.WithControlUsage(0, new InternedString("Back"), keyControl);
			deviceBuilder.WithControlUsage(1, new InternedString("Cancel"), keyControl);
			deviceBuilder.WithControlUsage(2, new InternedString("Submit"), keyControl3);
			deviceBuilder.WithControlUsage(3, new InternedString("Modifier"), keyControl56);
			deviceBuilder.WithControlUsage(4, new InternedString("Modifier"), keyControl57);
			deviceBuilder.WithControlUsage(5, new InternedString("Modifier"), discreteButtonControl);
			deviceBuilder.WithControlUsage(6, new InternedString("Modifier"), keyControl58);
			deviceBuilder.WithControlUsage(7, new InternedString("Modifier"), keyControl59);
			deviceBuilder.WithControlUsage(8, new InternedString("Modifier"), discreteButtonControl2);
			deviceBuilder.WithControlUsage(9, new InternedString("Modifier"), keyControl60);
			deviceBuilder.WithControlUsage(10, new InternedString("Modifier"), keyControl61);
			deviceBuilder.WithControlUsage(11, new InternedString("Modifier"), discreteButtonControl3);
			deviceBuilder.WithControlUsage(12, new InternedString("Modifier"), keyControl62);
			deviceBuilder.WithControlUsage(13, new InternedString("Modifier"), keyControl63);
			deviceBuilder.WithControlUsage(14, new InternedString("Modifier"), keyControl64);
			deviceBuilder.WithControlAlias(0, new InternedString("AltGr"));
			deviceBuilder.WithControlAlias(1, new InternedString("LeftWindows"));
			deviceBuilder.WithControlAlias(2, new InternedString("LeftApple"));
			deviceBuilder.WithControlAlias(3, new InternedString("LeftCommand"));
			deviceBuilder.WithControlAlias(4, new InternedString("RightWindows"));
			deviceBuilder.WithControlAlias(5, new InternedString("RightApple"));
			deviceBuilder.WithControlAlias(6, new InternedString("RightCommand"));
			base.keys = new KeyControl[123];
			base.keys[0] = keyControl2;
			base.keys[1] = keyControl3;
			base.keys[2] = keyControl4;
			base.keys[3] = keyControl5;
			base.keys[4] = keyControl6;
			base.keys[5] = keyControl7;
			base.keys[6] = keyControl8;
			base.keys[7] = keyControl9;
			base.keys[8] = keyControl10;
			base.keys[9] = keyControl11;
			base.keys[10] = keyControl12;
			base.keys[11] = keyControl13;
			base.keys[12] = keyControl14;
			base.keys[13] = keyControl15;
			base.keys[14] = keyControl20;
			base.keys[15] = keyControl21;
			base.keys[16] = keyControl22;
			base.keys[17] = keyControl23;
			base.keys[18] = keyControl24;
			base.keys[19] = keyControl25;
			base.keys[20] = keyControl26;
			base.keys[21] = keyControl27;
			base.keys[22] = keyControl28;
			base.keys[23] = keyControl29;
			base.keys[24] = keyControl30;
			base.keys[25] = keyControl31;
			base.keys[26] = keyControl32;
			base.keys[27] = keyControl33;
			base.keys[28] = keyControl34;
			base.keys[29] = keyControl35;
			base.keys[30] = keyControl36;
			base.keys[31] = keyControl37;
			base.keys[32] = keyControl38;
			base.keys[33] = keyControl39;
			base.keys[34] = keyControl40;
			base.keys[35] = keyControl41;
			base.keys[36] = keyControl42;
			base.keys[37] = keyControl43;
			base.keys[38] = keyControl44;
			base.keys[39] = keyControl45;
			base.keys[40] = keyControl46;
			base.keys[41] = keyControl47;
			base.keys[42] = keyControl48;
			base.keys[43] = keyControl49;
			base.keys[44] = keyControl50;
			base.keys[45] = keyControl51;
			base.keys[46] = keyControl52;
			base.keys[47] = keyControl53;
			base.keys[48] = keyControl54;
			base.keys[49] = keyControl55;
			base.keys[50] = keyControl56;
			base.keys[51] = keyControl57;
			base.keys[52] = keyControl58;
			base.keys[53] = keyControl59;
			base.keys[54] = keyControl60;
			base.keys[55] = keyControl61;
			base.keys[56] = keyControl62;
			base.keys[57] = keyControl63;
			base.keys[58] = keyControl64;
			base.keys[59] = keyControl;
			base.keys[60] = keyControl18;
			base.keys[61] = keyControl19;
			base.keys[62] = keyControl16;
			base.keys[63] = keyControl17;
			base.keys[64] = keyControl65;
			base.keys[65] = keyControl66;
			base.keys[66] = keyControl67;
			base.keys[67] = keyControl68;
			base.keys[68] = keyControl69;
			base.keys[69] = keyControl70;
			base.keys[70] = keyControl71;
			base.keys[71] = keyControl72;
			base.keys[72] = keyControl73;
			base.keys[73] = keyControl74;
			base.keys[74] = keyControl75;
			base.keys[75] = keyControl76;
			base.keys[76] = keyControl77;
			base.keys[77] = keyControl78;
			base.keys[78] = keyControl79;
			base.keys[79] = keyControl80;
			base.keys[80] = keyControl81;
			base.keys[81] = keyControl82;
			base.keys[82] = keyControl83;
			base.keys[83] = keyControl93;
			base.keys[84] = keyControl84;
			base.keys[85] = keyControl85;
			base.keys[86] = keyControl86;
			base.keys[87] = keyControl87;
			base.keys[88] = keyControl88;
			base.keys[89] = keyControl89;
			base.keys[90] = keyControl90;
			base.keys[91] = keyControl91;
			base.keys[92] = keyControl92;
			base.keys[93] = keyControl94;
			base.keys[94] = keyControl95;
			base.keys[95] = keyControl96;
			base.keys[96] = keyControl97;
			base.keys[97] = keyControl98;
			base.keys[98] = keyControl99;
			base.keys[99] = keyControl100;
			base.keys[100] = keyControl101;
			base.keys[101] = keyControl102;
			base.keys[102] = keyControl103;
			base.keys[103] = keyControl104;
			base.keys[104] = keyControl105;
			base.keys[105] = keyControl106;
			base.keys[106] = keyControl107;
			base.keys[107] = keyControl108;
			base.keys[108] = keyControl109;
			base.keys[109] = keyControl110;
			base.keys[111] = keyControl111;
			base.keys[112] = keyControl112;
			base.keys[113] = keyControl113;
			base.keys[114] = keyControl114;
			base.keys[115] = keyControl115;
			base.keys[116] = keyControl116;
			base.keys[117] = keyControl117;
			base.keys[118] = keyControl118;
			base.keys[119] = keyControl119;
			base.keys[120] = keyControl120;
			base.keys[121] = keyControl121;
			base.keys[122] = keyControl122;
			base.anyKey = anyKey;
			base.shiftKey = discreteButtonControl;
			base.ctrlKey = discreteButtonControl3;
			base.altKey = discreteButtonControl2;
			base.imeSelected = imeSelected;
			deviceBuilder.WithStateOffsetToControlIndexMap(new uint[]
			{
				525314U,
				650240U,
				1049603U,
				1573892U,
				2098181U,
				2622470U,
				3146759U,
				3671048U,
				4195337U,
				4719626U,
				5243915U,
				5768204U,
				6292493U,
				6816782U,
				7341071U,
				7865364U,
				8389653U,
				8913942U,
				9438231U,
				9962520U,
				10486809U,
				11011098U,
				11535387U,
				12059676U,
				12583965U,
				13108254U,
				13632543U,
				14156832U,
				14681121U,
				15205410U,
				15729699U,
				16253988U,
				16778277U,
				17302566U,
				17826855U,
				18351144U,
				18875433U,
				19399722U,
				19924011U,
				20448300U,
				20972589U,
				21496878U,
				22021167U,
				22545456U,
				23069745U,
				23594034U,
				24118323U,
				24642612U,
				25166901U,
				25691190U,
				26215479U,
				26739768U,
				26740794U,
				27264057U,
				27788347U,
				27789373U,
				28312636U,
				28836926U,
				28837952U,
				29361215U,
				29885505U,
				30409794U,
				30934083U,
				31458305U,
				31982610U,
				32506899U,
				33031184U,
				33555473U,
				34079812U,
				34604101U,
				35128390U,
				35652679U,
				36176968U,
				36701257U,
				37225546U,
				37749835U,
				38274124U,
				38798413U,
				39322702U,
				39846991U,
				40371280U,
				40895569U,
				41419858U,
				41944147U,
				42468436U,
				42992725U,
				43517014U,
				44041312U,
				44565591U,
				45089880U,
				45614169U,
				46138458U,
				46662747U,
				47187036U,
				47711325U,
				48235614U,
				48759903U,
				49284193U,
				49808482U,
				50332771U,
				50857060U,
				51381349U,
				51905638U,
				52429927U,
				52954216U,
				53478505U,
				54002794U,
				54527083U,
				55051372U,
				55575661U,
				56099950U,
				56624239U,
				57148528U,
				57672817U,
				58721394U,
				59245683U,
				59769972U,
				60294261U,
				60818550U,
				61342839U,
				61867128U,
				62391417U,
				62915706U,
				63439995U,
				63964284U,
				64488573U,
				66585726U
			});
			deviceBuilder.WithControlTree(new byte[]
			{
				127,
				0,
				1,
				0,
				0,
				0,
				0,
				64,
				0,
				3,
				0,
				0,
				0,
				1,
				127,
				0,
				49,
				0,
				1,
				0,
				1,
				32,
				0,
				15,
				0,
				0,
				0,
				0,
				64,
				0,
				5,
				0,
				0,
				0,
				0,
				48,
				0,
				91,
				0,
				0,
				0,
				0,
				64,
				0,
				7,
				0,
				0,
				0,
				0,
				56,
				0,
				121,
				0,
				67,
				0,
				1,
				64,
				0,
				9,
				0,
				68,
				0,
				1,
				60,
				0,
				135,
				0,
				0,
				0,
				0,
				64,
				0,
				11,
				0,
				0,
				0,
				0,
				62,
				0,
				13,
				0,
				0,
				0,
				0,
				64,
				0,
				47,
				0,
				0,
				0,
				0,
				61,
				0,
				byte.MaxValue,
				byte.MaxValue,
				2,
				0,
				1,
				62,
				0,
				byte.MaxValue,
				byte.MaxValue,
				19,
				0,
				1,
				16,
				0,
				17,
				0,
				0,
				0,
				0,
				32,
				0,
				61,
				0,
				0,
				0,
				0,
				8,
				0,
				19,
				0,
				0,
				0,
				0,
				16,
				0,
				33,
				0,
				0,
				0,
				0,
				4,
				0,
				21,
				0,
				0,
				0,
				0,
				8,
				0,
				27,
				0,
				0,
				0,
				0,
				2,
				0,
				23,
				0,
				0,
				0,
				0,
				4,
				0,
				25,
				0,
				0,
				0,
				0,
				1,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0,
				2,
				0,
				byte.MaxValue,
				byte.MaxValue,
				3,
				0,
				1,
				3,
				0,
				byte.MaxValue,
				byte.MaxValue,
				4,
				0,
				1,
				4,
				0,
				byte.MaxValue,
				byte.MaxValue,
				5,
				0,
				1,
				6,
				0,
				29,
				0,
				0,
				0,
				0,
				8,
				0,
				31,
				0,
				0,
				0,
				0,
				5,
				0,
				byte.MaxValue,
				byte.MaxValue,
				6,
				0,
				1,
				6,
				0,
				byte.MaxValue,
				byte.MaxValue,
				7,
				0,
				1,
				7,
				0,
				byte.MaxValue,
				byte.MaxValue,
				8,
				0,
				1,
				8,
				0,
				byte.MaxValue,
				byte.MaxValue,
				9,
				0,
				1,
				12,
				0,
				35,
				0,
				0,
				0,
				0,
				16,
				0,
				41,
				0,
				0,
				0,
				0,
				10,
				0,
				37,
				0,
				0,
				0,
				0,
				12,
				0,
				39,
				0,
				0,
				0,
				0,
				9,
				0,
				byte.MaxValue,
				byte.MaxValue,
				10,
				0,
				1,
				10,
				0,
				byte.MaxValue,
				byte.MaxValue,
				11,
				0,
				1,
				11,
				0,
				byte.MaxValue,
				byte.MaxValue,
				12,
				0,
				1,
				12,
				0,
				byte.MaxValue,
				byte.MaxValue,
				13,
				0,
				1,
				14,
				0,
				43,
				0,
				0,
				0,
				0,
				16,
				0,
				45,
				0,
				0,
				0,
				0,
				13,
				0,
				byte.MaxValue,
				byte.MaxValue,
				14,
				0,
				1,
				14,
				0,
				byte.MaxValue,
				byte.MaxValue,
				15,
				0,
				1,
				15,
				0,
				byte.MaxValue,
				byte.MaxValue,
				16,
				0,
				1,
				16,
				0,
				byte.MaxValue,
				byte.MaxValue,
				21,
				0,
				1,
				63,
				0,
				byte.MaxValue,
				byte.MaxValue,
				20,
				0,
				1,
				64,
				0,
				byte.MaxValue,
				byte.MaxValue,
				17,
				0,
				1,
				96,
				0,
				51,
				0,
				0,
				0,
				0,
				127,
				0,
				193,
				0,
				0,
				0,
				0,
				80,
				0,
				53,
				0,
				0,
				0,
				0,
				96,
				0,
				163,
				0,
				0,
				0,
				0,
				72,
				0,
				55,
				0,
				0,
				0,
				0,
				80,
				0,
				149,
				0,
				0,
				0,
				0,
				68,
				0,
				57,
				0,
				0,
				0,
				0,
				72,
				0,
				143,
				0,
				0,
				0,
				0,
				66,
				0,
				59,
				0,
				0,
				0,
				0,
				68,
				0,
				141,
				0,
				0,
				0,
				0,
				65,
				0,
				byte.MaxValue,
				byte.MaxValue,
				18,
				0,
				1,
				66,
				0,
				byte.MaxValue,
				byte.MaxValue,
				72,
				0,
				1,
				24,
				0,
				63,
				0,
				0,
				0,
				0,
				32,
				0,
				77,
				0,
				0,
				0,
				0,
				20,
				0,
				65,
				0,
				0,
				0,
				0,
				24,
				0,
				71,
				0,
				0,
				0,
				0,
				18,
				0,
				67,
				0,
				0,
				0,
				0,
				20,
				0,
				69,
				0,
				0,
				0,
				0,
				17,
				0,
				byte.MaxValue,
				byte.MaxValue,
				22,
				0,
				1,
				18,
				0,
				byte.MaxValue,
				byte.MaxValue,
				23,
				0,
				1,
				19,
				0,
				byte.MaxValue,
				byte.MaxValue,
				24,
				0,
				1,
				20,
				0,
				byte.MaxValue,
				byte.MaxValue,
				25,
				0,
				1,
				22,
				0,
				73,
				0,
				0,
				0,
				0,
				24,
				0,
				75,
				0,
				0,
				0,
				0,
				21,
				0,
				byte.MaxValue,
				byte.MaxValue,
				26,
				0,
				1,
				22,
				0,
				byte.MaxValue,
				byte.MaxValue,
				27,
				0,
				1,
				23,
				0,
				byte.MaxValue,
				byte.MaxValue,
				28,
				0,
				1,
				24,
				0,
				byte.MaxValue,
				byte.MaxValue,
				29,
				0,
				1,
				28,
				0,
				79,
				0,
				0,
				0,
				0,
				32,
				0,
				85,
				0,
				0,
				0,
				0,
				26,
				0,
				81,
				0,
				0,
				0,
				0,
				28,
				0,
				83,
				0,
				0,
				0,
				0,
				25,
				0,
				byte.MaxValue,
				byte.MaxValue,
				30,
				0,
				1,
				26,
				0,
				byte.MaxValue,
				byte.MaxValue,
				31,
				0,
				1,
				27,
				0,
				byte.MaxValue,
				byte.MaxValue,
				32,
				0,
				1,
				28,
				0,
				byte.MaxValue,
				byte.MaxValue,
				33,
				0,
				1,
				30,
				0,
				87,
				0,
				0,
				0,
				0,
				32,
				0,
				89,
				0,
				0,
				0,
				0,
				29,
				0,
				byte.MaxValue,
				byte.MaxValue,
				34,
				0,
				1,
				30,
				0,
				byte.MaxValue,
				byte.MaxValue,
				35,
				0,
				1,
				31,
				0,
				byte.MaxValue,
				byte.MaxValue,
				36,
				0,
				1,
				32,
				0,
				byte.MaxValue,
				byte.MaxValue,
				37,
				0,
				1,
				40,
				0,
				93,
				0,
				0,
				0,
				0,
				48,
				0,
				107,
				0,
				0,
				0,
				0,
				36,
				0,
				95,
				0,
				0,
				0,
				0,
				40,
				0,
				101,
				0,
				0,
				0,
				0,
				34,
				0,
				97,
				0,
				0,
				0,
				0,
				36,
				0,
				99,
				0,
				0,
				0,
				0,
				33,
				0,
				byte.MaxValue,
				byte.MaxValue,
				38,
				0,
				1,
				34,
				0,
				byte.MaxValue,
				byte.MaxValue,
				39,
				0,
				1,
				35,
				0,
				byte.MaxValue,
				byte.MaxValue,
				40,
				0,
				1,
				36,
				0,
				byte.MaxValue,
				byte.MaxValue,
				41,
				0,
				1,
				38,
				0,
				103,
				0,
				0,
				0,
				0,
				40,
				0,
				105,
				0,
				0,
				0,
				0,
				37,
				0,
				byte.MaxValue,
				byte.MaxValue,
				42,
				0,
				1,
				38,
				0,
				byte.MaxValue,
				byte.MaxValue,
				43,
				0,
				1,
				39,
				0,
				byte.MaxValue,
				byte.MaxValue,
				44,
				0,
				1,
				40,
				0,
				byte.MaxValue,
				byte.MaxValue,
				45,
				0,
				1,
				44,
				0,
				109,
				0,
				0,
				0,
				0,
				48,
				0,
				115,
				0,
				0,
				0,
				0,
				42,
				0,
				111,
				0,
				0,
				0,
				0,
				44,
				0,
				113,
				0,
				0,
				0,
				0,
				41,
				0,
				byte.MaxValue,
				byte.MaxValue,
				46,
				0,
				1,
				42,
				0,
				byte.MaxValue,
				byte.MaxValue,
				47,
				0,
				1,
				43,
				0,
				byte.MaxValue,
				byte.MaxValue,
				48,
				0,
				1,
				44,
				0,
				byte.MaxValue,
				byte.MaxValue,
				49,
				0,
				1,
				46,
				0,
				117,
				0,
				0,
				0,
				0,
				48,
				0,
				119,
				0,
				0,
				0,
				0,
				45,
				0,
				byte.MaxValue,
				byte.MaxValue,
				50,
				0,
				1,
				46,
				0,
				byte.MaxValue,
				byte.MaxValue,
				51,
				0,
				1,
				47,
				0,
				byte.MaxValue,
				byte.MaxValue,
				52,
				0,
				1,
				48,
				0,
				byte.MaxValue,
				byte.MaxValue,
				53,
				0,
				1,
				52,
				0,
				123,
				0,
				59,
				0,
				1,
				56,
				0,
				129,
				0,
				60,
				0,
				1,
				50,
				0,
				125,
				0,
				0,
				0,
				0,
				52,
				0,
				127,
				0,
				0,
				0,
				0,
				49,
				0,
				byte.MaxValue,
				byte.MaxValue,
				54,
				0,
				1,
				50,
				0,
				byte.MaxValue,
				byte.MaxValue,
				55,
				0,
				1,
				51,
				0,
				byte.MaxValue,
				byte.MaxValue,
				56,
				0,
				1,
				52,
				0,
				byte.MaxValue,
				byte.MaxValue,
				57,
				0,
				1,
				54,
				0,
				131,
				0,
				63,
				0,
				1,
				56,
				0,
				133,
				0,
				64,
				0,
				1,
				53,
				0,
				byte.MaxValue,
				byte.MaxValue,
				58,
				0,
				1,
				54,
				0,
				byte.MaxValue,
				byte.MaxValue,
				61,
				0,
				1,
				55,
				0,
				byte.MaxValue,
				byte.MaxValue,
				62,
				0,
				1,
				56,
				0,
				byte.MaxValue,
				byte.MaxValue,
				65,
				0,
				1,
				58,
				0,
				137,
				0,
				0,
				0,
				0,
				60,
				0,
				139,
				0,
				0,
				0,
				0,
				57,
				0,
				byte.MaxValue,
				byte.MaxValue,
				66,
				0,
				1,
				58,
				0,
				byte.MaxValue,
				byte.MaxValue,
				69,
				0,
				1,
				59,
				0,
				byte.MaxValue,
				byte.MaxValue,
				70,
				0,
				1,
				60,
				0,
				byte.MaxValue,
				byte.MaxValue,
				71,
				0,
				1,
				67,
				0,
				byte.MaxValue,
				byte.MaxValue,
				73,
				0,
				1,
				68,
				0,
				byte.MaxValue,
				byte.MaxValue,
				74,
				0,
				1,
				70,
				0,
				145,
				0,
				0,
				0,
				0,
				72,
				0,
				147,
				0,
				0,
				0,
				0,
				69,
				0,
				byte.MaxValue,
				byte.MaxValue,
				75,
				0,
				1,
				70,
				0,
				byte.MaxValue,
				byte.MaxValue,
				76,
				0,
				1,
				71,
				0,
				byte.MaxValue,
				byte.MaxValue,
				77,
				0,
				1,
				72,
				0,
				byte.MaxValue,
				byte.MaxValue,
				78,
				0,
				1,
				76,
				0,
				151,
				0,
				0,
				0,
				0,
				80,
				0,
				157,
				0,
				0,
				0,
				0,
				74,
				0,
				153,
				0,
				0,
				0,
				0,
				76,
				0,
				155,
				0,
				0,
				0,
				0,
				73,
				0,
				byte.MaxValue,
				byte.MaxValue,
				79,
				0,
				1,
				74,
				0,
				byte.MaxValue,
				byte.MaxValue,
				80,
				0,
				1,
				75,
				0,
				byte.MaxValue,
				byte.MaxValue,
				81,
				0,
				1,
				76,
				0,
				byte.MaxValue,
				byte.MaxValue,
				82,
				0,
				1,
				78,
				0,
				159,
				0,
				0,
				0,
				0,
				80,
				0,
				161,
				0,
				0,
				0,
				0,
				77,
				0,
				byte.MaxValue,
				byte.MaxValue,
				83,
				0,
				1,
				78,
				0,
				byte.MaxValue,
				byte.MaxValue,
				84,
				0,
				1,
				79,
				0,
				byte.MaxValue,
				byte.MaxValue,
				85,
				0,
				1,
				80,
				0,
				byte.MaxValue,
				byte.MaxValue,
				86,
				0,
				1,
				88,
				0,
				165,
				0,
				0,
				0,
				0,
				96,
				0,
				179,
				0,
				0,
				0,
				0,
				84,
				0,
				167,
				0,
				0,
				0,
				0,
				88,
				0,
				173,
				0,
				0,
				0,
				0,
				82,
				0,
				169,
				0,
				0,
				0,
				0,
				84,
				0,
				171,
				0,
				0,
				0,
				0,
				81,
				0,
				byte.MaxValue,
				byte.MaxValue,
				87,
				0,
				1,
				82,
				0,
				byte.MaxValue,
				byte.MaxValue,
				88,
				0,
				1,
				83,
				0,
				byte.MaxValue,
				byte.MaxValue,
				89,
				0,
				1,
				84,
				0,
				byte.MaxValue,
				byte.MaxValue,
				90,
				0,
				1,
				86,
				0,
				175,
				0,
				0,
				0,
				0,
				88,
				0,
				177,
				0,
				0,
				0,
				0,
				85,
				0,
				byte.MaxValue,
				byte.MaxValue,
				100,
				0,
				1,
				86,
				0,
				byte.MaxValue,
				byte.MaxValue,
				91,
				0,
				1,
				87,
				0,
				byte.MaxValue,
				byte.MaxValue,
				92,
				0,
				1,
				88,
				0,
				byte.MaxValue,
				byte.MaxValue,
				93,
				0,
				1,
				92,
				0,
				181,
				0,
				0,
				0,
				0,
				96,
				0,
				187,
				0,
				0,
				0,
				0,
				90,
				0,
				183,
				0,
				0,
				0,
				0,
				92,
				0,
				185,
				0,
				0,
				0,
				0,
				89,
				0,
				byte.MaxValue,
				byte.MaxValue,
				94,
				0,
				1,
				90,
				0,
				byte.MaxValue,
				byte.MaxValue,
				95,
				0,
				1,
				91,
				0,
				byte.MaxValue,
				byte.MaxValue,
				96,
				0,
				1,
				92,
				0,
				byte.MaxValue,
				byte.MaxValue,
				97,
				0,
				1,
				94,
				0,
				189,
				0,
				0,
				0,
				0,
				96,
				0,
				191,
				0,
				0,
				0,
				0,
				93,
				0,
				byte.MaxValue,
				byte.MaxValue,
				98,
				0,
				1,
				94,
				0,
				byte.MaxValue,
				byte.MaxValue,
				99,
				0,
				1,
				95,
				0,
				byte.MaxValue,
				byte.MaxValue,
				101,
				0,
				1,
				96,
				0,
				byte.MaxValue,
				byte.MaxValue,
				102,
				0,
				1,
				111,
				0,
				195,
				0,
				0,
				0,
				0,
				127,
				0,
				223,
				0,
				0,
				0,
				0,
				104,
				0,
				197,
				0,
				0,
				0,
				0,
				111,
				0,
				211,
				0,
				0,
				0,
				0,
				100,
				0,
				199,
				0,
				0,
				0,
				0,
				104,
				0,
				205,
				0,
				0,
				0,
				0,
				98,
				0,
				201,
				0,
				0,
				0,
				0,
				100,
				0,
				203,
				0,
				0,
				0,
				0,
				97,
				0,
				byte.MaxValue,
				byte.MaxValue,
				103,
				0,
				1,
				98,
				0,
				byte.MaxValue,
				byte.MaxValue,
				104,
				0,
				1,
				99,
				0,
				byte.MaxValue,
				byte.MaxValue,
				105,
				0,
				1,
				100,
				0,
				byte.MaxValue,
				byte.MaxValue,
				106,
				0,
				1,
				102,
				0,
				207,
				0,
				0,
				0,
				0,
				104,
				0,
				209,
				0,
				0,
				0,
				0,
				101,
				0,
				byte.MaxValue,
				byte.MaxValue,
				107,
				0,
				1,
				102,
				0,
				byte.MaxValue,
				byte.MaxValue,
				108,
				0,
				1,
				103,
				0,
				byte.MaxValue,
				byte.MaxValue,
				109,
				0,
				1,
				104,
				0,
				byte.MaxValue,
				byte.MaxValue,
				110,
				0,
				1,
				108,
				0,
				213,
				0,
				0,
				0,
				0,
				111,
				0,
				219,
				0,
				0,
				0,
				0,
				106,
				0,
				215,
				0,
				0,
				0,
				0,
				108,
				0,
				217,
				0,
				0,
				0,
				0,
				105,
				0,
				byte.MaxValue,
				byte.MaxValue,
				111,
				0,
				1,
				106,
				0,
				byte.MaxValue,
				byte.MaxValue,
				112,
				0,
				1,
				107,
				0,
				byte.MaxValue,
				byte.MaxValue,
				113,
				0,
				1,
				108,
				0,
				byte.MaxValue,
				byte.MaxValue,
				114,
				0,
				1,
				110,
				0,
				221,
				0,
				0,
				0,
				0,
				111,
				0,
				byte.MaxValue,
				byte.MaxValue,
				117,
				0,
				1,
				109,
				0,
				byte.MaxValue,
				byte.MaxValue,
				115,
				0,
				1,
				110,
				0,
				byte.MaxValue,
				byte.MaxValue,
				116,
				0,
				1,
				119,
				0,
				225,
				0,
				0,
				0,
				0,
				127,
				0,
				239,
				0,
				0,
				0,
				0,
				115,
				0,
				227,
				0,
				0,
				0,
				0,
				119,
				0,
				233,
				0,
				0,
				0,
				0,
				113,
				0,
				229,
				0,
				0,
				0,
				0,
				115,
				0,
				231,
				0,
				0,
				0,
				0,
				112,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0,
				113,
				0,
				byte.MaxValue,
				byte.MaxValue,
				118,
				0,
				1,
				114,
				0,
				byte.MaxValue,
				byte.MaxValue,
				119,
				0,
				1,
				115,
				0,
				byte.MaxValue,
				byte.MaxValue,
				120,
				0,
				1,
				117,
				0,
				235,
				0,
				0,
				0,
				0,
				119,
				0,
				237,
				0,
				0,
				0,
				0,
				116,
				0,
				byte.MaxValue,
				byte.MaxValue,
				121,
				0,
				1,
				117,
				0,
				byte.MaxValue,
				byte.MaxValue,
				122,
				0,
				1,
				118,
				0,
				byte.MaxValue,
				byte.MaxValue,
				123,
				0,
				1,
				119,
				0,
				byte.MaxValue,
				byte.MaxValue,
				124,
				0,
				1,
				123,
				0,
				241,
				0,
				0,
				0,
				0,
				127,
				0,
				247,
				0,
				0,
				0,
				0,
				121,
				0,
				243,
				0,
				0,
				0,
				0,
				123,
				0,
				245,
				0,
				0,
				0,
				0,
				120,
				0,
				byte.MaxValue,
				byte.MaxValue,
				125,
				0,
				1,
				121,
				0,
				byte.MaxValue,
				byte.MaxValue,
				126,
				0,
				1,
				122,
				0,
				byte.MaxValue,
				byte.MaxValue,
				127,
				0,
				1,
				123,
				0,
				byte.MaxValue,
				byte.MaxValue,
				128,
				0,
				1,
				125,
				0,
				249,
				0,
				0,
				0,
				0,
				127,
				0,
				251,
				0,
				0,
				0,
				0,
				124,
				0,
				byte.MaxValue,
				byte.MaxValue,
				129,
				0,
				1,
				125,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0,
				126,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0,
				127,
				0,
				253,
				0,
				0,
				0,
				0,
				127,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0,
				127,
				0,
				byte.MaxValue,
				0,
				0,
				0,
				0,
				128,
				0,
				byte.MaxValue,
				byte.MaxValue,
				130,
				0,
				1,
				127,
				0,
				byte.MaxValue,
				byte.MaxValue,
				0,
				0,
				0
			}, new ushort[]
			{
				0,
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15,
				16,
				17,
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				37,
				38,
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49,
				50,
				51,
				52,
				53,
				54,
				55,
				56,
				57,
				58,
				58,
				59,
				60,
				61,
				61,
				62,
				63,
				64,
				64,
				65,
				66,
				67,
				68,
				69,
				70,
				71,
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				79,
				80,
				81,
				82,
				83,
				84,
				85,
				86,
				87,
				88,
				89,
				90,
				91,
				92,
				93,
				94,
				95,
				96,
				97,
				98,
				99,
				100,
				101,
				102,
				103,
				104,
				105,
				106,
				107,
				108,
				109,
				110,
				111,
				112,
				113,
				114,
				115,
				116,
				117,
				118,
				119,
				120,
				121,
				122,
				123,
				124,
				125,
				126
			});
			deviceBuilder.Finish();
		}

		private AnyKeyControl Initialize_ctrlKeyboardanyKey(InternedString kAnyKeyLayout, InputControl parent)
		{
			AnyKeyControl anyKeyControl = new AnyKeyControl();
			anyKeyControl.Setup().At(this, 0).WithParent(parent).WithName("anyKey").WithDisplayName("Any Key").WithLayout(kAnyKeyLayout).IsSynthetic(true).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 1U,
				sizeInBits = 123U
			}).WithMinAndMax(0, 1).Finish();
			return anyKeyControl;
		}

		private KeyControl Initialize_ctrlKeyboardescape(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 1).WithParent(parent).WithName("escape").WithDisplayName("Escape").WithLayout(kKeyLayout).WithUsages(0, 2).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 60U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Escape;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardspace(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 2).WithParent(parent).WithName("space").WithDisplayName("Space").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 1U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Space;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardenter(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 3).WithParent(parent).WithName("enter").WithDisplayName("Enter").WithLayout(kKeyLayout).WithUsages(2, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 2U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Enter;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardtab(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 4).WithParent(parent).WithName("tab").WithDisplayName("Tab").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 3U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Tab;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardbackquote(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 5).WithParent(parent).WithName("backquote").WithDisplayName("`").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 4U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Backquote;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardquote(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 6).WithParent(parent).WithName("quote").WithDisplayName("'").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 5U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Quote;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardsemicolon(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 7).WithParent(parent).WithName("semicolon").WithDisplayName(";").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 6U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Semicolon;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardcomma(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 8).WithParent(parent).WithName("comma").WithDisplayName(",").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 7U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Comma;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardperiod(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 9).WithParent(parent).WithName("period").WithDisplayName(".").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 8U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Period;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardslash(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 10).WithParent(parent).WithName("slash").WithDisplayName("/").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 9U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Slash;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardbackslash(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 11).WithParent(parent).WithName("backslash").WithDisplayName("\\").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 10U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Backslash;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftBracket(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 12).WithParent(parent).WithName("leftBracket").WithDisplayName("[").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 11U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftBracket;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightBracket(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 13).WithParent(parent).WithName("rightBracket").WithDisplayName("]").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 12U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightBracket;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardminus(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 14).WithParent(parent).WithName("minus").WithDisplayName("-").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 13U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Minus;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardequals(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 15).WithParent(parent).WithName("equals").WithDisplayName("=").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 14U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Equals;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardupArrow(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 16).WithParent(parent).WithName("upArrow").WithDisplayName("Up Arrow").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 63U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.UpArrow;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboarddownArrow(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 17).WithParent(parent).WithName("downArrow").WithDisplayName("Down Arrow").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 64U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.DownArrow;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftArrow(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 18).WithParent(parent).WithName("leftArrow").WithDisplayName("Left Arrow").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 61U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftArrow;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightArrow(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 19).WithParent(parent).WithName("rightArrow").WithDisplayName("Right Arrow").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 62U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightArrow;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboarda(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 20).WithParent(parent).WithName("a").WithDisplayName("A").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 15U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.A;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardb(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 21).WithParent(parent).WithName("b").WithDisplayName("B").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 16U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.B;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardc(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 22).WithParent(parent).WithName("c").WithDisplayName("C").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 17U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.C;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardd(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 23).WithParent(parent).WithName("d").WithDisplayName("D").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 18U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.D;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboarde(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 24).WithParent(parent).WithName("e").WithDisplayName("E").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 19U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.E;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 25).WithParent(parent).WithName("f").WithDisplayName("F").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 20U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardg(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 26).WithParent(parent).WithName("g").WithDisplayName("G").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 21U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.G;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardh(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 27).WithParent(parent).WithName("h").WithDisplayName("H").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 22U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.H;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardi(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 28).WithParent(parent).WithName("i").WithDisplayName("I").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 23U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.I;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardj(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 29).WithParent(parent).WithName("j").WithDisplayName("J").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 24U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.J;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardk(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 30).WithParent(parent).WithName("k").WithDisplayName("K").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 25U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.K;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardl(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 31).WithParent(parent).WithName("l").WithDisplayName("L").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 26U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.L;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardm(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 32).WithParent(parent).WithName("m").WithDisplayName("M").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 27U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.M;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardn(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 33).WithParent(parent).WithName("n").WithDisplayName("N").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 28U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.N;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardo(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 34).WithParent(parent).WithName("o").WithDisplayName("O").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 29U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.O;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardp(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 35).WithParent(parent).WithName("p").WithDisplayName("P").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 30U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.P;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardq(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 36).WithParent(parent).WithName("q").WithDisplayName("Q").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 31U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Q;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardr(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 37).WithParent(parent).WithName("r").WithDisplayName("R").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 32U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.R;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboards(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 38).WithParent(parent).WithName("s").WithDisplayName("S").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 33U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.S;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardt(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 39).WithParent(parent).WithName("t").WithDisplayName("T").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 34U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.T;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardu(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 40).WithParent(parent).WithName("u").WithDisplayName("U").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 35U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.U;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardv(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 41).WithParent(parent).WithName("v").WithDisplayName("V").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 36U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.V;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardw(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 42).WithParent(parent).WithName("w").WithDisplayName("W").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 37U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.W;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardx(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 43).WithParent(parent).WithName("x").WithDisplayName("X").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 38U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.X;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardy(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 44).WithParent(parent).WithName("y").WithDisplayName("Y").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 39U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Y;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardz(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 45).WithParent(parent).WithName("z").WithDisplayName("Z").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 40U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Z;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard1(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 46).WithParent(parent).WithName("1").WithDisplayName("1").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 41U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit1;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard2(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 47).WithParent(parent).WithName("2").WithDisplayName("2").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 42U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit2;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard3(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 48).WithParent(parent).WithName("3").WithDisplayName("3").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 43U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit3;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard4(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 49).WithParent(parent).WithName("4").WithDisplayName("4").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 44U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit4;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard5(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 50).WithParent(parent).WithName("5").WithDisplayName("5").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 45U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit5;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard6(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 51).WithParent(parent).WithName("6").WithDisplayName("6").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 46U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit6;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard7(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 52).WithParent(parent).WithName("7").WithDisplayName("7").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 47U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit7;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard8(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 53).WithParent(parent).WithName("8").WithDisplayName("8").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 48U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit8;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard9(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 54).WithParent(parent).WithName("9").WithDisplayName("9").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 49U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit9;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboard0(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 55).WithParent(parent).WithName("0").WithDisplayName("0").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 50U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Digit0;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftShift(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 56).WithParent(parent).WithName("leftShift").WithDisplayName("Left Shift").WithLayout(kKeyLayout).WithUsages(3, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 51U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftShift;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightShift(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 57).WithParent(parent).WithName("rightShift").WithDisplayName("Right Shift").WithLayout(kKeyLayout).WithUsages(4, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 52U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightShift;
			return keyControl;
		}

		private DiscreteButtonControl Initialize_ctrlKeyboardshift(InternedString kDiscreteButtonLayout, InputControl parent)
		{
			DiscreteButtonControl discreteButtonControl = new DiscreteButtonControl();
			discreteButtonControl.minValue = 1;
			discreteButtonControl.maxValue = 3;
			discreteButtonControl.writeMode = DiscreteButtonControl.WriteMode.WriteNullAndMaxValue;
			discreteButtonControl.Setup().At(this, 58).WithParent(parent).WithName("shift").WithDisplayName("Shift").WithLayout(kDiscreteButtonLayout).WithUsages(5, 1).IsSynthetic(true).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 51U,
				sizeInBits = 2U
			}).WithMinAndMax(0, 1).Finish();
			return discreteButtonControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftAlt(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 59).WithParent(parent).WithName("leftAlt").WithDisplayName("Left Alt").WithLayout(kKeyLayout).WithUsages(6, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 53U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftAlt;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightAlt(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 60).WithParent(parent).WithName("rightAlt").WithDisplayName("Right Alt").WithLayout(kKeyLayout).WithUsages(7, 1).WithAliases(0, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 54U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightAlt;
			return keyControl;
		}

		private DiscreteButtonControl Initialize_ctrlKeyboardalt(InternedString kDiscreteButtonLayout, InputControl parent)
		{
			DiscreteButtonControl discreteButtonControl = new DiscreteButtonControl();
			discreteButtonControl.minValue = 1;
			discreteButtonControl.maxValue = 3;
			discreteButtonControl.writeMode = DiscreteButtonControl.WriteMode.WriteNullAndMaxValue;
			discreteButtonControl.Setup().At(this, 61).WithParent(parent).WithName("alt").WithDisplayName("Alt").WithLayout(kDiscreteButtonLayout).WithUsages(8, 1).IsSynthetic(true).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 53U,
				sizeInBits = 2U
			}).WithMinAndMax(0, 1).Finish();
			return discreteButtonControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftCtrl(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 62).WithParent(parent).WithName("leftCtrl").WithDisplayName("Left Control").WithLayout(kKeyLayout).WithUsages(9, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 55U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftCtrl;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightCtrl(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 63).WithParent(parent).WithName("rightCtrl").WithDisplayName("Right Control").WithLayout(kKeyLayout).WithUsages(10, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 56U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightCtrl;
			return keyControl;
		}

		private DiscreteButtonControl Initialize_ctrlKeyboardctrl(InternedString kDiscreteButtonLayout, InputControl parent)
		{
			DiscreteButtonControl discreteButtonControl = new DiscreteButtonControl();
			discreteButtonControl.minValue = 1;
			discreteButtonControl.maxValue = 3;
			discreteButtonControl.writeMode = DiscreteButtonControl.WriteMode.WriteNullAndMaxValue;
			discreteButtonControl.Setup().At(this, 64).WithParent(parent).WithName("ctrl").WithDisplayName("Control").WithLayout(kDiscreteButtonLayout).WithUsages(11, 1).IsSynthetic(true).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 55U,
				sizeInBits = 2U
			}).WithMinAndMax(0, 1).Finish();
			return discreteButtonControl;
		}

		private KeyControl Initialize_ctrlKeyboardleftMeta(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 65).WithParent(parent).WithName("leftMeta").WithDisplayName("Left System").WithLayout(kKeyLayout).WithUsages(12, 1).WithAliases(1, 3).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 57U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.LeftMeta;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardrightMeta(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 66).WithParent(parent).WithName("rightMeta").WithDisplayName("Right System").WithLayout(kKeyLayout).WithUsages(13, 1).WithAliases(4, 3).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 58U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.RightMeta;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardcontextMenu(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 67).WithParent(parent).WithName("contextMenu").WithDisplayName("Context Menu").WithLayout(kKeyLayout).WithUsages(14, 1).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 59U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.ContextMenu;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardbackspace(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 68).WithParent(parent).WithName("backspace").WithDisplayName("Backspace").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 65U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Backspace;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardpageDown(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 69).WithParent(parent).WithName("pageDown").WithDisplayName("Page Down").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 66U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.PageDown;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardpageUp(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 70).WithParent(parent).WithName("pageUp").WithDisplayName("Page Up").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 67U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.PageUp;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardhome(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 71).WithParent(parent).WithName("home").WithDisplayName("Home").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 68U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Home;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardend(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 72).WithParent(parent).WithName("end").WithDisplayName("End").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 69U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.End;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardinsert(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 73).WithParent(parent).WithName("insert").WithDisplayName("Insert").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 70U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Insert;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboarddelete(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 74).WithParent(parent).WithName("delete").WithDisplayName("Delete").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 71U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Delete;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardcapsLock(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 75).WithParent(parent).WithName("capsLock").WithDisplayName("Caps Lock").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 72U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.CapsLock;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumLock(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 76).WithParent(parent).WithName("numLock").WithDisplayName("Num Lock").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 73U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumLock;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardprintScreen(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 77).WithParent(parent).WithName("printScreen").WithDisplayName("Print Screen").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 74U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.PrintScreen;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardscrollLock(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 78).WithParent(parent).WithName("scrollLock").WithDisplayName("Scroll Lock").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 75U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.ScrollLock;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardpause(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 79).WithParent(parent).WithName("pause").WithDisplayName("Pause/Break").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 76U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Pause;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadEnter(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 80).WithParent(parent).WithName("numpadEnter").WithDisplayName("Numpad Enter").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 77U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadEnter;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadDivide(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 81).WithParent(parent).WithName("numpadDivide").WithDisplayName("Numpad /").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 78U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadDivide;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadMultiply(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 82).WithParent(parent).WithName("numpadMultiply").WithDisplayName("Numpad *").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 79U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadMultiply;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadPlus(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 83).WithParent(parent).WithName("numpadPlus").WithDisplayName("Numpad +").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 80U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadPlus;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadMinus(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 84).WithParent(parent).WithName("numpadMinus").WithDisplayName("Numpad -").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 81U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadMinus;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadPeriod(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 85).WithParent(parent).WithName("numpadPeriod").WithDisplayName("Numpad .").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 82U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadPeriod;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpadEquals(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 86).WithParent(parent).WithName("numpadEquals").WithDisplayName("Numpad =").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 83U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.NumpadEquals;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad1(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 87).WithParent(parent).WithName("numpad1").WithDisplayName("Numpad 1").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 85U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad1;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad2(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 88).WithParent(parent).WithName("numpad2").WithDisplayName("Numpad 2").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 86U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad2;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad3(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 89).WithParent(parent).WithName("numpad3").WithDisplayName("Numpad 3").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 87U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad3;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad4(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 90).WithParent(parent).WithName("numpad4").WithDisplayName("Numpad 4").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 88U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad4;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad5(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 91).WithParent(parent).WithName("numpad5").WithDisplayName("Numpad 5").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 89U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad5;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad6(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 92).WithParent(parent).WithName("numpad6").WithDisplayName("Numpad 6").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 90U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad6;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad7(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 93).WithParent(parent).WithName("numpad7").WithDisplayName("Numpad 7").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 91U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad7;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad8(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 94).WithParent(parent).WithName("numpad8").WithDisplayName("Numpad 8").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 92U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad8;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad9(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 95).WithParent(parent).WithName("numpad9").WithDisplayName("Numpad 9").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 93U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad9;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardnumpad0(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 96).WithParent(parent).WithName("numpad0").WithDisplayName("Numpad 0").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 84U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.Numpad0;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf1(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 97).WithParent(parent).WithName("f1").WithDisplayName("F1").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 94U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F1;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf2(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 98).WithParent(parent).WithName("f2").WithDisplayName("F2").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 95U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F2;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf3(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 99).WithParent(parent).WithName("f3").WithDisplayName("F3").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 96U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F3;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf4(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 100).WithParent(parent).WithName("f4").WithDisplayName("F4").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 97U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F4;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf5(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 101).WithParent(parent).WithName("f5").WithDisplayName("F5").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 98U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F5;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf6(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 102).WithParent(parent).WithName("f6").WithDisplayName("F6").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 99U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F6;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf7(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 103).WithParent(parent).WithName("f7").WithDisplayName("F7").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 100U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F7;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf8(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 104).WithParent(parent).WithName("f8").WithDisplayName("F8").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 101U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F8;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf9(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 105).WithParent(parent).WithName("f9").WithDisplayName("F9").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 102U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F9;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf10(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 106).WithParent(parent).WithName("f10").WithDisplayName("F10").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 103U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F10;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf11(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 107).WithParent(parent).WithName("f11").WithDisplayName("F11").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 104U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F11;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf12(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 108).WithParent(parent).WithName("f12").WithDisplayName("F12").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 105U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F12;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardOEM1(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 109).WithParent(parent).WithName("OEM1").WithDisplayName("OEM1").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 106U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.OEM1;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardOEM2(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 110).WithParent(parent).WithName("OEM2").WithDisplayName("OEM2").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 107U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.OEM2;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardOEM3(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 111).WithParent(parent).WithName("OEM3").WithDisplayName("OEM3").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 108U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.OEM3;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardOEM4(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 112).WithParent(parent).WithName("OEM4").WithDisplayName("OEM4").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 109U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.OEM4;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardOEM5(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 113).WithParent(parent).WithName("OEM5").WithDisplayName("OEM5").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 110U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.OEM5;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf13(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 114).WithParent(parent).WithName("f13").WithDisplayName("F13").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 112U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F13;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf14(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 115).WithParent(parent).WithName("f14").WithDisplayName("F14").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 113U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F14;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf15(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 116).WithParent(parent).WithName("f15").WithDisplayName("F15").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 114U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F15;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf16(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 117).WithParent(parent).WithName("f16").WithDisplayName("F16").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 115U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F16;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf17(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 118).WithParent(parent).WithName("f17").WithDisplayName("F17").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 116U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F17;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf18(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 119).WithParent(parent).WithName("f18").WithDisplayName("F18").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 117U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F18;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf19(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 120).WithParent(parent).WithName("f19").WithDisplayName("F19").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 118U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F19;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf20(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 121).WithParent(parent).WithName("f20").WithDisplayName("F20").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 119U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F20;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf21(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 122).WithParent(parent).WithName("f21").WithDisplayName("F21").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 120U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F21;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf22(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 123).WithParent(parent).WithName("f22").WithDisplayName("F22").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 121U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F22;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf23(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 124).WithParent(parent).WithName("f23").WithDisplayName("F23").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 122U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F23;
			return keyControl;
		}

		private KeyControl Initialize_ctrlKeyboardf24(InternedString kKeyLayout, InputControl parent)
		{
			KeyControl keyControl = new KeyControl();
			keyControl.Setup().At(this, 125).WithParent(parent).WithName("f24").WithDisplayName("F24").WithLayout(kKeyLayout).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 123U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			keyControl.keyCode = Key.F24;
			return keyControl;
		}

		private ButtonControl Initialize_ctrlKeyboardIMESelected(InternedString kButtonLayout, InputControl parent)
		{
			ButtonControl buttonControl = new ButtonControl();
			buttonControl.Setup().At(this, 126).WithParent(parent).WithName("IMESelected").WithDisplayName("IMESelected").WithLayout(kButtonLayout).IsSynthetic(true).IsButton(true).WithStateBlock(new InputStateBlock
			{
				format = new FourCC(1112101920),
				byteOffset = 0U,
				bitOffset = 127U,
				sizeInBits = 1U
			}).WithMinAndMax(0, 1).Finish();
			return buttonControl;
		}

		public const string metadata = ";AnyKey;Button;Axis;Key;DiscreteButton;Keyboard";
	}
}
