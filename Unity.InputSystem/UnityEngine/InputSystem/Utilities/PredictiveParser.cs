using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct PredictiveParser
	{
		public unsafe void ExpectSingleChar(ReadOnlySpan<char> str, char c)
		{
			if (*str[this.m_Position] != (ushort)c)
			{
				throw new InvalidOperationException(string.Format("Expected a '{0}' character at position {1} in : {2}", c, this.m_Position, str.ToString()));
			}
			this.m_Position++;
		}

		public unsafe int ExpectInt(ReadOnlySpan<char> str)
		{
			int num = this.m_Position;
			int num2 = 1;
			if (*str[num] == 45)
			{
				num2 = -1;
				num++;
			}
			int num3 = 0;
			for (;;)
			{
				char c = (char)(*str[num]);
				if (c < '0' || c > '9')
				{
					break;
				}
				num3 *= 10;
				num3 += (int)(c - '0');
				num++;
			}
			if (this.m_Position == num)
			{
				throw new InvalidOperationException(string.Format("Expected an int at position {0} in {1}", this.m_Position, str.ToString()));
			}
			this.m_Position = num;
			return num3 * num2;
		}

		public unsafe ReadOnlySpan<char> ExpectString(ReadOnlySpan<char> str)
		{
			int position = this.m_Position;
			if (*str[position] != 34)
			{
				throw new InvalidOperationException(string.Format("Expected a '\"' character at position {0} in {1}", this.m_Position, str.ToString()));
			}
			this.m_Position++;
			for (;;)
			{
				char c = (char)(*str[this.m_Position]);
				c |= ' ';
				if (c < 'a' || c > 'z')
				{
					break;
				}
				this.m_Position++;
			}
			if (*str[this.m_Position] != 34)
			{
				throw new InvalidOperationException(string.Format("Expected a closing '\"' character at position {0} in string: {1}", this.m_Position, str.ToString()));
			}
			if (this.m_Position - position == 1)
			{
				return ReadOnlySpan<char>.Empty;
			}
			ReadOnlySpan<char> result = str.Slice(position + 1, this.m_Position - position - 1);
			this.m_Position++;
			return result;
		}

		public unsafe bool AcceptSingleChar(ReadOnlySpan<char> str, char c)
		{
			if (*str[this.m_Position] != (ushort)c)
			{
				return false;
			}
			this.m_Position++;
			return true;
		}

		public unsafe bool AcceptString(ReadOnlySpan<char> input, out ReadOnlySpan<char> output)
		{
			output = default(ReadOnlySpan<char>);
			int position = this.m_Position;
			int num = position;
			if (*input[num] != 34)
			{
				return false;
			}
			num++;
			for (;;)
			{
				char c = (char)(*input[num]);
				c |= ' ';
				if (c < 'a' || c > 'z')
				{
					break;
				}
				num++;
			}
			if (*input[num] != 34)
			{
				return false;
			}
			if (this.m_Position - position == 1)
			{
				output = ReadOnlySpan<char>.Empty;
			}
			else
			{
				output = input.Slice(position + 1, num - position - 1);
			}
			this.m_Position = num + 1;
			return true;
		}

		public unsafe void AcceptInt(ReadOnlySpan<char> str)
		{
			if (*str[this.m_Position] == 45)
			{
				this.m_Position++;
			}
			for (;;)
			{
				char c = (char)(*str[this.m_Position]);
				if (c < '0' || c > '9')
				{
					break;
				}
				this.m_Position++;
			}
		}

		private int m_Position;
	}
}
