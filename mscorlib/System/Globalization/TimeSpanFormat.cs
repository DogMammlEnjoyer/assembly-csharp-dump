using System;
using System.Text;

namespace System.Globalization
{
	internal static class TimeSpanFormat
	{
		private unsafe static void AppendNonNegativeInt32(StringBuilder sb, int n, int digits)
		{
			uint num = (uint)n;
			char* ptr = stackalloc char[(UIntPtr)20];
			int num2 = 0;
			do
			{
				uint num3 = num / 10U;
				ptr[(IntPtr)(num2++) * 2] = (char)(num - num3 * 10U + 48U);
				num = num3;
			}
			while (num != 0U);
			for (int i = digits - num2; i > 0; i--)
			{
				sb.Append('0');
			}
			for (int j = num2 - 1; j >= 0; j--)
			{
				sb.Append(ptr[j]);
			}
		}

		internal static string Format(TimeSpan value, string format, IFormatProvider formatProvider)
		{
			return StringBuilderCache.GetStringAndRelease(TimeSpanFormat.FormatToBuilder(value, format, formatProvider));
		}

		internal static bool TryFormat(TimeSpan value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
		{
			StringBuilder stringBuilder = TimeSpanFormat.FormatToBuilder(value, format, formatProvider);
			if (stringBuilder.Length <= destination.Length)
			{
				charsWritten = stringBuilder.Length;
				stringBuilder.CopyTo(0, destination, stringBuilder.Length);
				StringBuilderCache.Release(stringBuilder);
				return true;
			}
			StringBuilderCache.Release(stringBuilder);
			charsWritten = 0;
			return false;
		}

		private unsafe static StringBuilder FormatToBuilder(TimeSpan value, ReadOnlySpan<char> format, IFormatProvider formatProvider)
		{
			if (format.Length == 0)
			{
				format = "c";
			}
			if (format.Length == 1)
			{
				char c = (char)(*format[0]);
				if (c <= 'T')
				{
					if (c == 'G')
					{
						goto IL_53;
					}
					if (c != 'T')
					{
						goto IL_89;
					}
				}
				else if (c != 'c')
				{
					if (c == 'g')
					{
						goto IL_53;
					}
					if (c != 't')
					{
						goto IL_89;
					}
				}
				return TimeSpanFormat.FormatStandard(value, true, format, TimeSpanFormat.Pattern.Minimum);
				IL_53:
				DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(formatProvider);
				return TimeSpanFormat.FormatStandard(value, false, (value.Ticks < 0L) ? instance.FullTimeSpanNegativePattern : instance.FullTimeSpanPositivePattern, (c == 'g') ? TimeSpanFormat.Pattern.Minimum : TimeSpanFormat.Pattern.Full);
				IL_89:
				throw new FormatException("Input string was not in a correct format.");
			}
			return TimeSpanFormat.FormatCustomized(value, format, DateTimeFormatInfo.GetInstance(formatProvider), null);
		}

		private static StringBuilder FormatStandard(TimeSpan value, bool isInvariant, ReadOnlySpan<char> format, TimeSpanFormat.Pattern pattern)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			int num = (int)(value.Ticks / 864000000000L);
			long num2 = value.Ticks % 864000000000L;
			if (value.Ticks < 0L)
			{
				num = -num;
				num2 = -num2;
			}
			int n = (int)(num2 / 36000000000L % 24L);
			int n2 = (int)(num2 / 600000000L % 60L);
			int n3 = (int)(num2 / 10000000L % 60L);
			int num3 = (int)(num2 % 10000000L);
			TimeSpanFormat.FormatLiterals formatLiterals;
			if (isInvariant)
			{
				formatLiterals = ((value.Ticks < 0L) ? TimeSpanFormat.NegativeInvariantFormatLiterals : TimeSpanFormat.PositiveInvariantFormatLiterals);
			}
			else
			{
				formatLiterals = default(TimeSpanFormat.FormatLiterals);
				formatLiterals.Init(format, pattern == TimeSpanFormat.Pattern.Full);
			}
			if (num3 != 0)
			{
				num3 = (int)((long)num3 / TimeSpanParse.Pow10(7 - formatLiterals.ff));
			}
			stringBuilder.Append(formatLiterals.Start);
			if (pattern == TimeSpanFormat.Pattern.Full || num != 0)
			{
				stringBuilder.Append(num);
				stringBuilder.Append(formatLiterals.DayHourSep);
			}
			TimeSpanFormat.AppendNonNegativeInt32(stringBuilder, n, formatLiterals.hh);
			stringBuilder.Append(formatLiterals.HourMinuteSep);
			TimeSpanFormat.AppendNonNegativeInt32(stringBuilder, n2, formatLiterals.mm);
			stringBuilder.Append(formatLiterals.MinuteSecondSep);
			TimeSpanFormat.AppendNonNegativeInt32(stringBuilder, n3, formatLiterals.ss);
			if (!isInvariant && pattern == TimeSpanFormat.Pattern.Minimum)
			{
				int num4 = formatLiterals.ff;
				while (num4 > 0 && num3 % 10 == 0)
				{
					num3 /= 10;
					num4--;
				}
				if (num4 > 0)
				{
					stringBuilder.Append(formatLiterals.SecondFractionSep);
					stringBuilder.Append(num3.ToString(DateTimeFormat.fixedNumberFormats[num4 - 1], CultureInfo.InvariantCulture));
				}
			}
			else if (pattern == TimeSpanFormat.Pattern.Full || num3 != 0)
			{
				stringBuilder.Append(formatLiterals.SecondFractionSep);
				TimeSpanFormat.AppendNonNegativeInt32(stringBuilder, num3, formatLiterals.ff);
			}
			stringBuilder.Append(formatLiterals.End);
			return stringBuilder;
		}

		private unsafe static StringBuilder FormatCustomized(TimeSpan value, ReadOnlySpan<char> format, DateTimeFormatInfo dtfi, StringBuilder result)
		{
			bool flag = false;
			if (result == null)
			{
				result = StringBuilderCache.Acquire(16);
				flag = true;
			}
			int num = (int)(value.Ticks / 864000000000L);
			long num2 = value.Ticks % 864000000000L;
			if (value.Ticks < 0L)
			{
				num = -num;
				num2 = -num2;
			}
			int value2 = (int)(num2 / 36000000000L % 24L);
			int value3 = (int)(num2 / 600000000L % 60L);
			int value4 = (int)(num2 / 10000000L % 60L);
			int num3 = (int)(num2 % 10000000L);
			int i = 0;
			while (i < format.Length)
			{
				char c = (char)(*format[i]);
				int num5;
				if (c <= 'F')
				{
					if (c <= '%')
					{
						if (c != '"')
						{
							if (c != '%')
							{
								goto IL_2B5;
							}
							int num4 = DateTimeFormat.ParseNextChar(format, i);
							if (num4 >= 0 && num4 != 37)
							{
								char c2 = (char)num4;
								ReadOnlySpan<char> format2 = new ReadOnlySpan<char>((void*)(&c2), 1);
								TimeSpanFormat.FormatCustomized(value, format2, dtfi, result);
								num5 = 2;
								goto IL_2C9;
							}
							goto IL_2B5;
						}
					}
					else if (c != '\'')
					{
						if (c != 'F')
						{
							goto IL_2B5;
						}
						num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
						if (num5 > 7)
						{
							goto IL_2B5;
						}
						long num6 = (long)num3;
						num6 /= TimeSpanParse.Pow10(7 - num5);
						int num7 = num5;
						while (num7 > 0 && num6 % 10L == 0L)
						{
							num6 /= 10L;
							num7--;
						}
						if (num7 > 0)
						{
							result.Append(num6.ToString(DateTimeFormat.fixedNumberFormats[num7 - 1], CultureInfo.InvariantCulture));
							goto IL_2C9;
						}
						goto IL_2C9;
					}
					num5 = DateTimeFormat.ParseQuoteString(format, i, result);
				}
				else if (c <= 'h')
				{
					if (c != '\\')
					{
						switch (c)
						{
						case 'd':
							num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
							if (num5 > 8)
							{
								goto IL_2B5;
							}
							DateTimeFormat.FormatDigits(result, num, num5, true);
							break;
						case 'e':
						case 'g':
							goto IL_2B5;
						case 'f':
						{
							num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
							if (num5 > 7)
							{
								goto IL_2B5;
							}
							long num6 = (long)num3;
							result.Append((num6 / TimeSpanParse.Pow10(7 - num5)).ToString(DateTimeFormat.fixedNumberFormats[num5 - 1], CultureInfo.InvariantCulture));
							break;
						}
						case 'h':
							num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
							if (num5 > 2)
							{
								goto IL_2B5;
							}
							DateTimeFormat.FormatDigits(result, value2, num5);
							break;
						default:
							goto IL_2B5;
						}
					}
					else
					{
						int num4 = DateTimeFormat.ParseNextChar(format, i);
						if (num4 < 0)
						{
							goto IL_2B5;
						}
						result.Append((char)num4);
						num5 = 2;
					}
				}
				else if (c != 'm')
				{
					if (c != 's')
					{
						goto IL_2B5;
					}
					num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
					if (num5 > 2)
					{
						goto IL_2B5;
					}
					DateTimeFormat.FormatDigits(result, value4, num5);
				}
				else
				{
					num5 = DateTimeFormat.ParseRepeatPattern(format, i, c);
					if (num5 > 2)
					{
						goto IL_2B5;
					}
					DateTimeFormat.FormatDigits(result, value3, num5);
				}
				IL_2C9:
				i += num5;
				continue;
				IL_2B5:
				if (flag)
				{
					StringBuilderCache.Release(result);
				}
				throw new FormatException("Input string was not in a correct format.");
			}
			return result;
		}

		internal static readonly TimeSpanFormat.FormatLiterals PositiveInvariantFormatLiterals = TimeSpanFormat.FormatLiterals.InitInvariant(false);

		internal static readonly TimeSpanFormat.FormatLiterals NegativeInvariantFormatLiterals = TimeSpanFormat.FormatLiterals.InitInvariant(true);

		internal enum Pattern
		{
			None,
			Minimum,
			Full
		}

		internal struct FormatLiterals
		{
			internal string Start
			{
				get
				{
					return this._literals[0];
				}
			}

			internal string DayHourSep
			{
				get
				{
					return this._literals[1];
				}
			}

			internal string HourMinuteSep
			{
				get
				{
					return this._literals[2];
				}
			}

			internal string MinuteSecondSep
			{
				get
				{
					return this._literals[3];
				}
			}

			internal string SecondFractionSep
			{
				get
				{
					return this._literals[4];
				}
			}

			internal string End
			{
				get
				{
					return this._literals[5];
				}
			}

			internal static TimeSpanFormat.FormatLiterals InitInvariant(bool isNegative)
			{
				TimeSpanFormat.FormatLiterals formatLiterals = new TimeSpanFormat.FormatLiterals
				{
					_literals = new string[6]
				};
				formatLiterals._literals[0] = (isNegative ? "-" : string.Empty);
				formatLiterals._literals[1] = ".";
				formatLiterals._literals[2] = ":";
				formatLiterals._literals[3] = ":";
				formatLiterals._literals[4] = ".";
				formatLiterals._literals[5] = string.Empty;
				formatLiterals.AppCompatLiteral = ":.";
				formatLiterals.dd = 2;
				formatLiterals.hh = 2;
				formatLiterals.mm = 2;
				formatLiterals.ss = 2;
				formatLiterals.ff = 7;
				return formatLiterals;
			}

			internal unsafe void Init(ReadOnlySpan<char> format, bool useInvariantFieldLengths)
			{
				this.dd = (this.hh = (this.mm = (this.ss = (this.ff = 0))));
				this._literals = new string[6];
				for (int i = 0; i < this._literals.Length; i++)
				{
					this._literals[i] = string.Empty;
				}
				StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
				bool flag = false;
				char c = '\'';
				int num = 0;
				int j = 0;
				while (j < format.Length)
				{
					char c2 = (char)(*format[j]);
					if (c2 <= 'F')
					{
						if (c2 <= '%')
						{
							if (c2 != '"')
							{
								if (c2 != '%')
								{
									goto IL_1C5;
								}
								goto IL_1C5;
							}
						}
						else if (c2 != '\'')
						{
							if (c2 != 'F')
							{
								goto IL_1C5;
							}
							goto IL_1B0;
						}
						if (flag && c == (char)(*format[j]))
						{
							if (num < 0 || num > 5)
							{
								return;
							}
							this._literals[num] = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag = false;
						}
						else if (!flag)
						{
							c = (char)(*format[j]);
							flag = true;
						}
					}
					else if (c2 <= 'h')
					{
						if (c2 != '\\')
						{
							switch (c2)
							{
							case 'd':
								if (!flag)
								{
									num = 1;
									this.dd++;
								}
								break;
							case 'e':
							case 'g':
								goto IL_1C5;
							case 'f':
								goto IL_1B0;
							case 'h':
								if (!flag)
								{
									num = 2;
									this.hh++;
								}
								break;
							default:
								goto IL_1C5;
							}
						}
						else
						{
							if (flag)
							{
								goto IL_1C5;
							}
							j++;
						}
					}
					else if (c2 != 'm')
					{
						if (c2 != 's')
						{
							goto IL_1C5;
						}
						if (!flag)
						{
							num = 4;
							this.ss++;
						}
					}
					else if (!flag)
					{
						num = 3;
						this.mm++;
					}
					IL_1D6:
					j++;
					continue;
					IL_1B0:
					if (!flag)
					{
						num = 5;
						this.ff++;
						goto IL_1D6;
					}
					goto IL_1D6;
					IL_1C5:
					stringBuilder.Append((char)(*format[j]));
					goto IL_1D6;
				}
				this.AppCompatLiteral = this.MinuteSecondSep + this.SecondFractionSep;
				if (useInvariantFieldLengths)
				{
					this.dd = 2;
					this.hh = 2;
					this.mm = 2;
					this.ss = 2;
					this.ff = 7;
				}
				else
				{
					if (this.dd < 1 || this.dd > 2)
					{
						this.dd = 2;
					}
					if (this.hh < 1 || this.hh > 2)
					{
						this.hh = 2;
					}
					if (this.mm < 1 || this.mm > 2)
					{
						this.mm = 2;
					}
					if (this.ss < 1 || this.ss > 2)
					{
						this.ss = 2;
					}
					if (this.ff < 1 || this.ff > 7)
					{
						this.ff = 7;
					}
				}
				StringBuilderCache.Release(stringBuilder);
			}

			internal string AppCompatLiteral;

			internal int dd;

			internal int hh;

			internal int mm;

			internal int ss;

			internal int ff;

			private string[] _literals;
		}
	}
}
