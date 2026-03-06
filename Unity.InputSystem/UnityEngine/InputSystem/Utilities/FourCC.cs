using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.InputSystem.Utilities
{
	public struct FourCC : IEquatable<FourCC>
	{
		public FourCC(int code)
		{
			this.m_Code = code;
		}

		public FourCC(char a, char b = ' ', char c = ' ', char d = ' ')
		{
			this.m_Code = (int)((int)a << 24 | (int)b << 16 | (int)c << 8 | d);
		}

		public FourCC(string str)
		{
			this = default(FourCC);
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			int length = str.Length;
			if (length < 1 || length > 4)
			{
				throw new ArgumentException("FourCC string must be one to four characters long!", "str");
			}
			char c = str[0];
			char c2 = (length > 1) ? str[1] : ' ';
			char c3 = (length > 2) ? str[2] : ' ';
			char c4 = (length > 3) ? str[3] : ' ';
			this.m_Code = (int)((int)c << 24 | (int)c2 << 16 | (int)c3 << 8 | c4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(FourCC fourCC)
		{
			return fourCC.m_Code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator FourCC(int i)
		{
			return new FourCC(i);
		}

		public override string ToString()
		{
			return string.Format("{0}{1}{2}{3}", new object[]
			{
				(char)(this.m_Code >> 24),
				(char)((this.m_Code & 16711680) >> 16),
				(char)((this.m_Code & 65280) >> 8),
				(char)(this.m_Code & 255)
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FourCC other)
		{
			return this.m_Code == other.m_Code;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is FourCC)
			{
				FourCC other = (FourCC)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.m_Code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FourCC left, FourCC right)
		{
			return left.m_Code == right.m_Code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FourCC left, FourCC right)
		{
			return left.m_Code != right.m_Code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FourCC FromInt32(int i)
		{
			return i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt32(FourCC fourCC)
		{
			return fourCC.m_Code;
		}

		private int m_Code;
	}
}
