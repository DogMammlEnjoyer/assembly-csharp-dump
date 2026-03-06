using System;
using System.Runtime.InteropServices;

namespace System
{
	internal struct GuidEx
	{
		private unsafe static int HexsToChars(char* guidChars, int a, int b)
		{
			*guidChars = HexConverter.ToCharLower(a >> 4);
			guidChars[1] = HexConverter.ToCharLower(a);
			guidChars[2] = HexConverter.ToCharLower(b >> 4);
			guidChars[3] = HexConverter.ToCharLower(b);
			return 4;
		}

		private unsafe static int HexsToCharsHexOutput(char* guidChars, int a, int b)
		{
			*guidChars = '0';
			guidChars[1] = 'x';
			guidChars[2] = HexConverter.ToCharLower(a >> 4);
			guidChars[3] = HexConverter.ToCharLower(a);
			guidChars[4] = ',';
			guidChars[5] = '0';
			guidChars[6] = 'x';
			guidChars[7] = HexConverter.ToCharLower(b >> 4);
			guidChars[8] = HexConverter.ToCharLower(b);
			return 9;
		}

		public unsafe bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default(ReadOnlySpan<char>))
		{
			if (format.Length == 0)
			{
				format = "D".AsSpan();
			}
			if (format.Length != 1)
			{
				throw new FormatException("InvalidGuidFormatSpecification");
			}
			bool flag = true;
			bool flag2 = false;
			int num = 0;
			char c = (char)(*format[0]);
			if (c <= 'X')
			{
				if (c <= 'D')
				{
					if (c == 'B')
					{
						goto IL_9D;
					}
					if (c != 'D')
					{
						goto IL_C2;
					}
				}
				else
				{
					if (c == 'N')
					{
						goto IL_96;
					}
					if (c == 'P')
					{
						goto IL_A8;
					}
					if (c != 'X')
					{
						goto IL_C2;
					}
					goto IL_B3;
				}
			}
			else if (c <= 'd')
			{
				if (c == 'b')
				{
					goto IL_9D;
				}
				if (c != 'd')
				{
					goto IL_C2;
				}
			}
			else
			{
				if (c == 'n')
				{
					goto IL_96;
				}
				if (c == 'p')
				{
					goto IL_A8;
				}
				if (c != 'x')
				{
					goto IL_C2;
				}
				goto IL_B3;
			}
			int num2 = 36;
			goto IL_CD;
			IL_96:
			flag = false;
			num2 = 32;
			goto IL_CD;
			IL_9D:
			num = 8192123;
			num2 = 38;
			goto IL_CD;
			IL_A8:
			num = 2687016;
			num2 = 38;
			goto IL_CD;
			IL_B3:
			num = 8192123;
			flag = false;
			flag2 = true;
			num2 = 68;
			goto IL_CD;
			IL_C2:
			throw new FormatException("InvalidGuidFormatSpecification");
			IL_CD:
			if (destination.Length < num2)
			{
				charsWritten = 0;
				return false;
			}
			fixed (char* reference = MemoryMarshal.GetReference<char>(destination))
			{
				char* ptr = reference;
				if (num != 0)
				{
					*(ptr++) = (char)num;
				}
				if (flag2)
				{
					*(ptr++) = '0';
					*(ptr++) = 'x';
					ptr += GuidEx.HexsToChars(ptr, this._a >> 24, this._a >> 16);
					ptr += GuidEx.HexsToChars(ptr, this._a >> 8, this._a);
					*(ptr++) = ',';
					*(ptr++) = '0';
					*(ptr++) = 'x';
					ptr += GuidEx.HexsToChars(ptr, this._b >> 8, (int)this._b);
					*(ptr++) = ',';
					*(ptr++) = '0';
					*(ptr++) = 'x';
					ptr += GuidEx.HexsToChars(ptr, this._c >> 8, (int)this._c);
					*(ptr++) = ',';
					*(ptr++) = '{';
					ptr += GuidEx.HexsToCharsHexOutput(ptr, (int)this._d, (int)this._e);
					*(ptr++) = ',';
					ptr += GuidEx.HexsToCharsHexOutput(ptr, (int)this._f, (int)this._g);
					*(ptr++) = ',';
					ptr += GuidEx.HexsToCharsHexOutput(ptr, (int)this._h, (int)this._i);
					*(ptr++) = ',';
					ptr += GuidEx.HexsToCharsHexOutput(ptr, (int)this._j, (int)this._k);
					*(ptr++) = '}';
				}
				else
				{
					ptr += GuidEx.HexsToChars(ptr, this._a >> 24, this._a >> 16);
					ptr += GuidEx.HexsToChars(ptr, this._a >> 8, this._a);
					if (flag)
					{
						*(ptr++) = '-';
					}
					ptr += GuidEx.HexsToChars(ptr, this._b >> 8, (int)this._b);
					if (flag)
					{
						*(ptr++) = '-';
					}
					ptr += GuidEx.HexsToChars(ptr, this._c >> 8, (int)this._c);
					if (flag)
					{
						*(ptr++) = '-';
					}
					ptr += GuidEx.HexsToChars(ptr, (int)this._d, (int)this._e);
					if (flag)
					{
						*(ptr++) = '-';
					}
					ptr += GuidEx.HexsToChars(ptr, (int)this._f, (int)this._g);
					ptr += GuidEx.HexsToChars(ptr, (int)this._h, (int)this._i);
					ptr += GuidEx.HexsToChars(ptr, (int)this._j, (int)this._k);
				}
				if (num != 0)
				{
					*(ptr++) = (char)(num >> 16);
				}
			}
			charsWritten = num2;
			return true;
		}

		private int _a;

		private short _b;

		private short _c;

		private byte _d;

		private byte _e;

		private byte _f;

		private byte _g;

		private byte _h;

		private byte _i;

		private byte _j;

		private byte _k;
	}
}
