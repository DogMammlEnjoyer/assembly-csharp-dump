using System;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	public static class YamlCodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlphaNumericDashOrUnderscore(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122)
					{
						goto IL_32;
					}
				}
				else if (code > 90 && code != 95)
				{
					goto IL_32;
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					goto IL_32;
				}
			}
			else if (code != 45)
			{
				goto IL_32;
			}
			return true;
			IL_32:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWordChar(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122)
					{
						goto IL_2D;
					}
				}
				else if (code > 90)
				{
					goto IL_2D;
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					goto IL_2D;
				}
			}
			else if (code != 45)
			{
				goto IL_2D;
			}
			return true;
			IL_2D:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUriChar(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122 && code != 126)
					{
						goto IL_C2;
					}
				}
				else if (code > 90)
				{
					switch (code)
					{
					case 91:
					case 93:
					case 95:
						break;
					case 92:
					case 94:
						goto IL_C2;
					default:
						goto IL_C2;
					}
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					switch (code)
					{
					case 58:
					case 59:
					case 61:
					case 63:
					case 64:
						break;
					case 60:
					case 62:
						goto IL_C2;
					default:
						goto IL_C2;
					}
				}
			}
			else
			{
				switch (code)
				{
				case 33:
				case 35:
				case 36:
				case 38:
				case 39:
				case 40:
				case 41:
				case 42:
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
					break;
				case 34:
				case 37:
					goto IL_C2;
				default:
					goto IL_C2;
				}
			}
			return true;
			IL_C2:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTagChar(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 122 && code != 126)
					{
						goto IL_9F;
					}
				}
				else if (code > 90 && code != 95)
				{
					goto IL_9F;
				}
			}
			else if (code >= 48)
			{
				if (code > 57)
				{
					switch (code)
					{
					case 58:
					case 59:
					case 61:
					case 63:
					case 64:
						break;
					case 60:
					case 62:
						goto IL_9F;
					default:
						goto IL_9F;
					}
				}
			}
			else
			{
				switch (code)
				{
				case 35:
				case 36:
				case 38:
				case 39:
				case 42:
				case 43:
				case 45:
				case 46:
				case 47:
					break;
				case 37:
				case 40:
				case 41:
				case 44:
					goto IL_9F;
				default:
					goto IL_9F;
				}
			}
			return true;
			IL_9F:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAscii(byte code)
		{
			return code <= 127;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumber(byte code)
		{
			return code >= 48 && code <= 57;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty(byte code)
		{
			return code == 32 || code == 9 || code == 10 || code == 13;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLineBreak(byte code)
		{
			return code == 10 || code == 13;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlank(byte code)
		{
			return code == 32 || code == 9;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumberRepresentation(byte code)
		{
			if (code >= 48)
			{
				if (code > 57)
				{
					goto IL_1C;
				}
			}
			else if (code != 43 && code - 45 > 1)
			{
				goto IL_1C;
			}
			return true;
			IL_1C:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsHex(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code > 102)
					{
						goto IL_26;
					}
				}
				else if (code > 70)
				{
					goto IL_26;
				}
			}
			else if (code < 48 || code > 57)
			{
				goto IL_26;
			}
			return true;
			IL_26:
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAnyFlowSymbol(byte code)
		{
			return code == 44 || code == 91 || code == 93 || code == 123 || code == 125;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte AsHex(byte code)
		{
			if (code >= 65)
			{
				if (code >= 97)
				{
					if (code <= 102)
					{
						return code - 97 + 10;
					}
				}
				else if (code <= 70)
				{
					return code - 65 + 10;
				}
			}
			else if (code >= 48 && code <= 57)
			{
				return code - 48;
			}
			throw new InvalidOperationException();
		}

		public static readonly byte[] YamlDirectiveName = new byte[]
		{
			89,
			65,
			77,
			76
		};

		public static readonly byte[] TagDirectiveName = new byte[]
		{
			84,
			65,
			71
		};

		public static readonly byte[] Utf8Bom = new byte[]
		{
			239,
			187,
			191
		};

		public static readonly byte[] StreamStart = new byte[]
		{
			45,
			45,
			45
		};

		public static readonly byte[] DocStart = new byte[]
		{
			46,
			46,
			46
		};

		public static readonly byte[] CrLf = new byte[]
		{
			13,
			10
		};

		public static readonly byte[] Null0 = new byte[]
		{
			110,
			117,
			108,
			108
		};

		public static readonly byte[] Null1 = new byte[]
		{
			78,
			117,
			108,
			108
		};

		public static readonly byte[] Null2 = new byte[]
		{
			78,
			85,
			76,
			76
		};

		public const byte NullAlias = 126;

		public static readonly byte[] True0 = new byte[]
		{
			116,
			114,
			117,
			101
		};

		public static readonly byte[] True1 = new byte[]
		{
			84,
			114,
			117,
			101
		};

		public static readonly byte[] True2 = new byte[]
		{
			84,
			82,
			85,
			69
		};

		public static readonly byte[] False0 = new byte[]
		{
			102,
			97,
			108,
			115,
			101
		};

		public static readonly byte[] False1 = new byte[]
		{
			70,
			97,
			108,
			115,
			101
		};

		public static readonly byte[] False2 = new byte[]
		{
			70,
			65,
			76,
			83,
			69
		};

		public static readonly byte[] Inf0 = new byte[]
		{
			46,
			105,
			110,
			102
		};

		public static readonly byte[] Inf1 = new byte[]
		{
			46,
			73,
			110,
			102
		};

		public static readonly byte[] Inf2 = new byte[]
		{
			46,
			73,
			78,
			70
		};

		public static readonly byte[] Inf3 = new byte[]
		{
			43,
			46,
			105,
			110,
			102
		};

		public static readonly byte[] Inf4 = new byte[]
		{
			43,
			46,
			73,
			110,
			102
		};

		public static readonly byte[] Inf5 = new byte[]
		{
			43,
			46,
			73,
			78,
			70
		};

		public static readonly byte[] Yes0 = new byte[]
		{
			121,
			101,
			115
		};

		public static readonly byte[] Yes1 = new byte[]
		{
			89,
			101,
			115
		};

		public static readonly byte[] Yes2 = new byte[]
		{
			89,
			69,
			83
		};

		public static readonly byte[] No0 = new byte[]
		{
			110,
			111
		};

		public static readonly byte[] No1 = new byte[]
		{
			78,
			111
		};

		public static readonly byte[] No2 = new byte[]
		{
			78,
			79
		};

		public static readonly byte[] On0 = new byte[]
		{
			111,
			110
		};

		public static readonly byte[] On1 = new byte[]
		{
			79,
			110
		};

		public static readonly byte[] On2 = new byte[]
		{
			79,
			78
		};

		public static readonly byte[] Off0 = new byte[]
		{
			111,
			102,
			102
		};

		public static readonly byte[] Off1 = new byte[]
		{
			79,
			102,
			102
		};

		public static readonly byte[] Off2 = new byte[]
		{
			79,
			70,
			70
		};

		public static readonly byte[] NegInf0 = new byte[]
		{
			45,
			46,
			105,
			110,
			102
		};

		public static readonly byte[] NegInf1 = new byte[]
		{
			45,
			46,
			73,
			110,
			102
		};

		public static readonly byte[] NegInf2 = new byte[]
		{
			45,
			46,
			73,
			78,
			70
		};

		public static readonly byte[] Nan0 = new byte[]
		{
			46,
			110,
			97,
			110
		};

		public static readonly byte[] Nan1 = new byte[]
		{
			46,
			78,
			97,
			78
		};

		public static readonly byte[] Nan2 = new byte[]
		{
			46,
			78,
			65,
			78
		};

		public static readonly byte[] HexPrefix = new byte[]
		{
			48,
			120
		};

		public static readonly byte[] HexPrefixNegative = new byte[]
		{
			45,
			48,
			120
		};

		public static readonly byte[] OctalPrefix = new byte[]
		{
			48,
			111
		};

		public static readonly byte[] UnityStrippedSymbol = new byte[]
		{
			115,
			116,
			114,
			105,
			112,
			112,
			101,
			100
		};

		public const byte Space = 32;

		public const byte Tab = 9;

		public const byte Lf = 10;

		public const byte Cr = 13;

		public const byte Comment = 35;

		public const byte DirectiveLine = 37;

		public const byte Alias = 42;

		public const byte Anchor = 38;

		public const byte Tag = 33;

		public const byte SingleQuote = 39;

		public const byte DoubleQuote = 34;

		public const byte LiteralScalerHeader = 124;

		public const byte FoldedScalerHeader = 62;

		public const byte Comma = 44;

		public const byte BlockEntryIndent = 45;

		public const byte ExplicitKeyIndent = 63;

		public const byte MapValueIndent = 58;

		public const byte FlowMapStart = 123;

		public const byte FlowMapEnd = 125;

		public const byte FlowSequenceStart = 91;

		public const byte FlowSequenceEnd = 93;
	}
}
