using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class EmitStringAnalyzer
	{
		public unsafe static EmitStringInfo Analyze(string value)
		{
			ReadOnlySpan<char> readOnlySpan = value.AsSpan();
			if (readOnlySpan.Length <= 0)
			{
				return new EmitStringInfo(0, true, false);
			}
			bool flag = EmitStringAnalyzer.IsReservedWord(value);
			char c = (char)(*readOnlySpan[0]);
			char c2 = (char)(*readOnlySpan[readOnlySpan.Length - 1]);
			bool needsQuotes = flag || c == ' ' || c2 == ' ' || c == '&' || c == '*' || c == '?' || c == '|' || c == '-' || c == '<' || c == '>' || c == '=' || c == '!' || c == '%' || c == '@' || c == '.';
			int num = 1;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
			int i = 0;
			while (i < readOnlySpan2.Length)
			{
				char c3 = (char)(*readOnlySpan2[i]);
				if (c3 <= ',')
				{
					if (c3 <= '"')
					{
						if (c3 != '\n')
						{
							if (c3 == '"')
							{
								goto IL_F8;
							}
						}
						else
						{
							num++;
						}
					}
					else if (c3 == '#' || c3 == '\'' || c3 == ',')
					{
						goto IL_F8;
					}
				}
				else if (c3 <= '[')
				{
					if (c3 == ':' || c3 == '[')
					{
						goto IL_F8;
					}
				}
				else if (c3 == ']' || c3 == '`' || c3 == '{')
				{
					goto IL_F8;
				}
				IL_103:
				i++;
				continue;
				IL_F8:
				needsQuotes = true;
				goto IL_103;
			}
			if (c2 == '\n')
			{
				num--;
			}
			return new EmitStringInfo(num, needsQuotes, flag);
		}

		[NullableContext(0)]
		[return: Nullable(1)]
		internal unsafe static StringBuilder BuildLiteralScalar(ReadOnlySpan<char> originalValue, int indentCharCount)
		{
			char c = '\0';
			if (originalValue.Length > 0 && *originalValue[originalValue.Length - 1] == 10)
			{
				if (*originalValue[originalValue.Length - 2] == 10 || (*originalValue[originalValue.Length - 2] == 13 && *originalValue[originalValue.Length - 3] == 10))
				{
					c = '+';
				}
			}
			else
			{
				c = '-';
			}
			StringBuilder stringBuilder;
			if ((stringBuilder = EmitStringAnalyzer.stringBuilderThreadStatic) == null)
			{
				stringBuilder = (EmitStringAnalyzer.stringBuilderThreadStatic = new StringBuilder(1024));
			}
			StringBuilder stringBuilder2 = stringBuilder.Clear();
			stringBuilder2.Append('|');
			if (c > '\0')
			{
				stringBuilder2.Append(c);
			}
			stringBuilder2.Append('\n');
			EmitStringAnalyzer.AppendWhiteSpace(stringBuilder2, indentCharCount);
			for (int i = 0; i < originalValue.Length; i++)
			{
				char c2 = (char)(*originalValue[i]);
				stringBuilder2.Append(c2);
				if (c2 == '\n' && i < originalValue.Length - 1)
				{
					EmitStringAnalyzer.AppendWhiteSpace(stringBuilder2, indentCharCount);
				}
			}
			if (c == '-')
			{
				stringBuilder2.Append('\n');
			}
			return stringBuilder2;
		}

		[NullableContext(0)]
		[return: Nullable(1)]
		internal unsafe static StringBuilder BuildQuotedScalar(ReadOnlySpan<char> originalValue, bool doubleQuote = true)
		{
			StringBuilder stringBuilder = EmitStringAnalyzer.GetStringBuilder();
			char value = doubleQuote ? '"' : '\'';
			stringBuilder.Append(value);
			ReadOnlySpan<char> readOnlySpan = originalValue;
			int i = 0;
			while (i < readOnlySpan.Length)
			{
				char c = (char)(*readOnlySpan[i]);
				char c2 = c;
				if (c2 <= '\u007f')
				{
					switch (c2)
					{
					case '\0':
						stringBuilder.Append("\\0");
						break;
					case '\u0001':
						stringBuilder.Append("\\1");
						break;
					case '\u0002':
						stringBuilder.Append("\\2");
						break;
					case '\u0003':
						stringBuilder.Append("\\3");
						break;
					case '\u0004':
						stringBuilder.Append("\\4");
						break;
					case '\u0005':
						stringBuilder.Append("\\5");
						break;
					case '\u0006':
						stringBuilder.Append("\\6");
						break;
					case '\a':
						stringBuilder.Append("\\a");
						break;
					case '\b':
						stringBuilder.Append("\\b");
						break;
					case '\t':
						stringBuilder.Append("\\t");
						break;
					case '\n':
						stringBuilder.Append("\\n");
						break;
					case '\v':
						stringBuilder.Append("\\v");
						break;
					case '\f':
						stringBuilder.Append("\\f");
						break;
					case '\r':
						stringBuilder.Append("\\r");
						break;
					case '\u000e':
						stringBuilder.Append("\\r");
						break;
					case '\u000f':
						stringBuilder.Append("\\u000f");
						break;
					case '\u0010':
						stringBuilder.Append("\\u0010");
						break;
					case '\u0011':
						stringBuilder.Append("\\u0011");
						break;
					case '\u0012':
						stringBuilder.Append("\\u0012");
						break;
					case '\u0013':
						stringBuilder.Append("\\u0013");
						break;
					case '\u0014':
						stringBuilder.Append("\\u0014");
						break;
					case '\u0015':
						stringBuilder.Append("\\u0015");
						break;
					case '\u0016':
						stringBuilder.Append("\\u0016");
						break;
					case '\u0017':
						stringBuilder.Append("\\u0017");
						break;
					case '\u0018':
						stringBuilder.Append("\\u0018");
						break;
					case '\u0019':
						stringBuilder.Append("\\u0019");
						break;
					case '\u001a':
						stringBuilder.Append("\\u001a");
						break;
					case '\u001b':
						stringBuilder.Append("\\u001b");
						break;
					case '\u001c':
						stringBuilder.Append("\\u001c");
						break;
					case '\u001d':
						stringBuilder.Append("\\u001d");
						break;
					case '\u001e':
						stringBuilder.Append("\\u001e");
						break;
					case '\u001f':
						stringBuilder.Append("\\u001f");
						break;
					case ' ':
					case '!':
					case '#':
					case '$':
					case '%':
					case '&':
						goto IL_3D3;
					case '"':
						if (!doubleQuote)
						{
							goto IL_3D3;
						}
						stringBuilder.Append("\\\"");
						break;
					case '\'':
						if (doubleQuote)
						{
							goto IL_3D3;
						}
						stringBuilder.Append("\\'");
						break;
					default:
						if (c2 != '\\')
						{
							if (c2 != '\u007f')
							{
								goto IL_3D3;
							}
							stringBuilder.Append("\\u007F");
						}
						else
						{
							stringBuilder.Append("\\\\");
						}
						break;
					}
				}
				else if (c2 <= '\u00a0')
				{
					if (c2 != '\u0085')
					{
						if (c2 != '\u00a0')
						{
							goto IL_3D3;
						}
						stringBuilder.Append("\\_");
					}
					else
					{
						stringBuilder.Append("\\N");
					}
				}
				else if (c2 != '\u2028')
				{
					if (c2 != '\u2029')
					{
						goto IL_3D3;
					}
					stringBuilder.Append("\\P");
				}
				else
				{
					stringBuilder.Append("\\L");
				}
				IL_3DC:
				i++;
				continue;
				IL_3D3:
				stringBuilder.Append(c);
				goto IL_3DC;
			}
			stringBuilder.Append(value);
			return stringBuilder;
		}

		private static bool IsReservedWord(string value)
		{
			new StringBuilder().Append('\n');
			switch (value.Length)
			{
			case 1:
				if (value == "~")
				{
					return true;
				}
				break;
			case 4:
				if (value == "null" || value == "Null" || value == "NULL" || value == "true" || value == "True" || value == "TRUE")
				{
					return true;
				}
				break;
			case 5:
				if (value == "false" || value == "False" || value == "FALSE")
				{
					return true;
				}
				break;
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static StringBuilder GetStringBuilder()
		{
			StringBuilder stringBuilder;
			if ((stringBuilder = EmitStringAnalyzer.stringBuilderThreadStatic) == null)
			{
				stringBuilder = (EmitStringAnalyzer.stringBuilderThreadStatic = new StringBuilder(1024));
			}
			return stringBuilder.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AppendWhiteSpace(StringBuilder stringBuilder, int length)
		{
			if (length > EmitStringAnalyzer.whiteSpaces.Length)
			{
				EmitStringAnalyzer.whiteSpaces = Enumerable.Repeat<char>(' ', length * 2).ToArray<char>();
			}
			stringBuilder.Append(EmitStringAnalyzer.whiteSpaces.AsSpan(0, length));
		}

		[Nullable(2)]
		[ThreadStatic]
		private static StringBuilder stringBuilderThreadStatic;

		private static char[] whiteSpaces = new char[]
		{
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' ',
			' '
		};
	}
}
