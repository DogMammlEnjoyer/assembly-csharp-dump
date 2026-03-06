using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public struct Utf16ValueStringBuilder : IDisposable, IBufferWriter<char>, IResettableBufferWriter<char>
	{
		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, params T[] values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values.AsSpan<T>());
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, List<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values);
		}

		[NullableContext(2)]
		public unsafe void AppendJoin<T>(char separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values);
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, IEnumerable<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values);
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, ICollection<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values.AsEnumerable<T>());
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, IList<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values);
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, IReadOnlyList<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values);
		}

		public unsafe void AppendJoin<[Nullable(2)] T>(char separator, IReadOnlyCollection<T> values)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)separator;
			ReadOnlySpan<char> separator2 = new Span<char>(intPtr, 1);
			this.AppendJoinInternal<T>(separator2, values.AsEnumerable<T>());
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, params T[] values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values.AsSpan<T>());
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, List<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values);
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values);
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, IEnumerable<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values);
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, ICollection<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values.AsEnumerable<T>());
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, IList<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values);
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, IReadOnlyList<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values);
		}

		public void AppendJoin<[Nullable(2)] T>(string separator, IReadOnlyCollection<T> values)
		{
			this.AppendJoinInternal<T>(separator.AsSpan(), values.AsEnumerable<T>());
		}

		[NullableContext(0)]
		internal void AppendJoinInternal<[Nullable(2)] T>(ReadOnlySpan<char> separator, [Nullable(1)] IList<T> values)
		{
			IReadOnlyList<T> readOnlyList = values as IReadOnlyList<T>;
			readOnlyList = (readOnlyList ?? new ReadOnlyListAdaptor<T>(values));
			this.AppendJoinInternal<T>(separator, readOnlyList);
		}

		[NullableContext(0)]
		internal void AppendJoinInternal<[Nullable(2)] T>(ReadOnlySpan<char> separator, [Nullable(1)] IReadOnlyList<T> values)
		{
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				if (i != 0)
				{
					this.Append(separator);
				}
				T t = values[i];
				if (typeof(T) == typeof(string))
				{
					string value = Unsafe.As<string>(t);
					if (!string.IsNullOrEmpty(value))
					{
						this.Append(value);
					}
				}
				else
				{
					this.Append<T>(t);
				}
			}
		}

		[NullableContext(0)]
		internal unsafe void AppendJoinInternal<[Nullable(2)] T>(ReadOnlySpan<char> separator, [Nullable(new byte[]
		{
			0,
			1
		})] ReadOnlySpan<T> values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (i != 0)
				{
					this.Append(separator);
				}
				T t = *values[i];
				if (typeof(T) == typeof(string))
				{
					string value = Unsafe.As<string>(t);
					if (!string.IsNullOrEmpty(value))
					{
						this.Append(value);
					}
				}
				else
				{
					this.Append<T>(t);
				}
			}
		}

		[NullableContext(0)]
		internal void AppendJoinInternal<[Nullable(2)] T>(ReadOnlySpan<char> separator, [Nullable(1)] IEnumerable<T> values)
		{
			bool flag = true;
			foreach (T t in values)
			{
				if (!flag)
				{
					this.Append(separator);
				}
				else
				{
					flag = false;
				}
				if (typeof(T) == typeof(string))
				{
					string value = Unsafe.As<string>(t);
					if (!string.IsNullOrEmpty(value))
					{
						this.Append(value);
					}
				}
				else
				{
					this.Append<T>(t);
				}
			}
		}

		public void AppendFormat<[Nullable(2)] T1>(string format, T1 arg1)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						if (parseResult.Index == 0)
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
						}
						else
						{
							Utf16ValueStringBuilder.ThrowFormatException();
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		[NullableContext(0)]
		public unsafe void AppendFormat<[Nullable(2)] T1>(ReadOnlySpan<char> format, [Nullable(1)] T1 arg1)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						if (parseResult.Index == 0)
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
						}
						else
						{
							Utf16ValueStringBuilder.ThrowFormatException();
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2>(string format, T1 arg1, T2 arg2)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						int num2 = parseResult.Index;
						if (num2 != 0)
						{
							if (num2 != 1)
							{
								Utf16ValueStringBuilder.ThrowFormatException();
							}
							else
							{
								this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							}
						}
						else
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num3 = format.Length - num;
			if (num3 > 0)
			{
				this.Append(format, num, num3);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						int num2 = parseResult.Index;
						if (num2 != 0)
						{
							if (num2 != 1)
							{
								Utf16ValueStringBuilder.ThrowFormatException();
							}
							else
							{
								this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							}
						}
						else
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num3 = format.Length - num;
			if (num3 > 0)
			{
				this.Append(format.Slice(num, num3));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(string format, T1 arg1, T2 arg2, T3 arg3)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, parseResult.FormatString, "arg15");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, parseResult.FormatString, "arg15");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		public void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && format[i + 1] == '{')
					{
						int count = i - num;
						this.Append(format, num, count);
						i++;
						num = i;
					}
					else
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, parseResult.FormatString, "arg15");
							break;
						case 15:
							this.AppendFormatInternal<T16>(arg16, parseResult.Alignment, parseResult.FormatString, "arg16");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count3 = i - num;
						this.Append(format, num, count3);
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		public unsafe void AppendFormat<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8, [Nullable(2)] T9, [Nullable(2)] T10, [Nullable(2)] T11, [Nullable(2)] T12, [Nullable(2)] T13, [Nullable(2)] T14, [Nullable(2)] T15, [Nullable(2)] T16>([Nullable(0)] ReadOnlySpan<char> format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
		{
			int num = 0;
			for (int i = 0; i < format.Length; i++)
			{
				char c = (char)(*format[i]);
				if (c == '{')
				{
					if (i == format.Length - 1)
					{
						throw new FormatException("invalid format");
					}
					if (i != format.Length && *format[i + 1] == 123)
					{
						int length = i - num;
						this.Append(format.Slice(num, length));
						i++;
						num = i;
					}
					else
					{
						int length2 = i - num;
						this.Append(format.Slice(num, length2));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, parseResult.FormatString, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, parseResult.FormatString, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, parseResult.FormatString, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, parseResult.FormatString, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, parseResult.FormatString, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, parseResult.FormatString, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, parseResult.FormatString, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, parseResult.FormatString, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, parseResult.FormatString, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, parseResult.FormatString, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, parseResult.FormatString, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, parseResult.FormatString, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, parseResult.FormatString, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, parseResult.FormatString, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, parseResult.FormatString, "arg15");
							break;
						case 15:
							this.AppendFormatInternal<T16>(arg16, parseResult.Alignment, parseResult.FormatString, "arg16");
							break;
						default:
							Utf16ValueStringBuilder.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && *format[i + 1] == 125)
					{
						int length3 = i - num;
						this.Append(format.Slice(num, length3));
						i++;
						num = i;
					}
					else
					{
						Utf16ValueStringBuilder.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format.Slice(num, num2));
			}
		}

		[return: Nullable(2)]
		private static object CreateFormatter(Type type)
		{
			if (type == typeof(sbyte))
			{
				return new Utf16ValueStringBuilder.TryFormat<sbyte>(delegate(sbyte x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteInt64(dest, out written, (long)x);
				});
			}
			if (type == typeof(short))
			{
				return new Utf16ValueStringBuilder.TryFormat<short>(delegate(short x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteInt64(dest, out written, (long)x);
				});
			}
			if (type == typeof(int))
			{
				return new Utf16ValueStringBuilder.TryFormat<int>(delegate(int x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteInt64(dest, out written, (long)x);
				});
			}
			if (type == typeof(long))
			{
				return new Utf16ValueStringBuilder.TryFormat<long>(delegate(long x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteInt64(dest, out written, x);
				});
			}
			if (type == typeof(byte))
			{
				return new Utf16ValueStringBuilder.TryFormat<byte>(delegate(byte x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteUInt64(dest, out written, (ulong)x);
				});
			}
			if (type == typeof(ushort))
			{
				return new Utf16ValueStringBuilder.TryFormat<ushort>(delegate(ushort x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteUInt64(dest, out written, (ulong)x);
				});
			}
			if (type == typeof(uint))
			{
				return new Utf16ValueStringBuilder.TryFormat<uint>(delegate(uint x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteUInt64(dest, out written, (ulong)x);
				});
			}
			if (type == typeof(ulong))
			{
				return new Utf16ValueStringBuilder.TryFormat<ulong>(delegate(ulong x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (format.Length != 0)
					{
						return x.TryFormat(dest, out written, format, null);
					}
					return FastNumberWriter.TryWriteUInt64(dest, out written, x);
				});
			}
			if (type == typeof(float))
			{
				return new Utf16ValueStringBuilder.TryFormat<float>(delegate(float x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(double))
			{
				return new Utf16ValueStringBuilder.TryFormat<double>(delegate(double x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(TimeSpan))
			{
				return new Utf16ValueStringBuilder.TryFormat<TimeSpan>(delegate(TimeSpan x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(DateTime))
			{
				return new Utf16ValueStringBuilder.TryFormat<DateTime>(delegate(DateTime x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(DateTimeOffset))
			{
				return new Utf16ValueStringBuilder.TryFormat<DateTimeOffset>(delegate(DateTimeOffset x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(decimal))
			{
				return new Utf16ValueStringBuilder.TryFormat<decimal>(delegate(decimal x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(Guid))
			{
				return new Utf16ValueStringBuilder.TryFormat<Guid>(delegate(Guid x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					return x.TryFormat(dest, out written, format);
				});
			}
			if (type == typeof(byte?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<byte>();
			}
			if (type == typeof(DateTime?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<DateTime>();
			}
			if (type == typeof(DateTimeOffset?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<DateTimeOffset>();
			}
			if (type == typeof(decimal?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<decimal>();
			}
			if (type == typeof(double?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<double>();
			}
			if (type == typeof(short?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<short>();
			}
			if (type == typeof(int?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<int>();
			}
			if (type == typeof(long?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<long>();
			}
			if (type == typeof(sbyte?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<sbyte>();
			}
			if (type == typeof(float?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<float>();
			}
			if (type == typeof(TimeSpan?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<TimeSpan>();
			}
			if (type == typeof(ushort?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<ushort>();
			}
			if (type == typeof(uint?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<uint>();
			}
			if (type == typeof(ulong?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<ulong>();
			}
			if (type == typeof(Guid?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<Guid>();
			}
			if (type == typeof(bool?))
			{
				return Utf16ValueStringBuilder.CreateNullableFormatter<bool>();
			}
			if (type == typeof(IntPtr))
			{
				return new Utf16ValueStringBuilder.TryFormat<IntPtr>(delegate(IntPtr x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (IntPtr.Size != 4)
					{
						return x.ToInt64().TryFormat(dest, out written, format, null);
					}
					return x.ToInt32().TryFormat(dest, out written, format, null);
				});
			}
			if (type == typeof(UIntPtr))
			{
				return new Utf16ValueStringBuilder.TryFormat<UIntPtr>(delegate(UIntPtr x, Span<char> dest, out int written, ReadOnlySpan<char> format)
				{
					if (UIntPtr.Size != 4)
					{
						return x.ToUInt64().TryFormat(dest, out written, format, null);
					}
					return x.ToUInt32().TryFormat(dest, out written, format, null);
				});
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(byte value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(byte value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTime value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTime value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(DateTime value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(DateTime value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTimeOffset value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTimeOffset value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(DateTimeOffset value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(DateTimeOffset value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(decimal value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(decimal value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(decimal value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(decimal value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(double value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(double value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(double value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(double value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(short value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(short value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(short value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(short value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(int value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(int value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(int value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(int value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(long value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(long value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(long value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(long value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(sbyte value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(sbyte value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(sbyte value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(sbyte value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(float value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(float value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(float value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(float value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(TimeSpan value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(TimeSpan value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(TimeSpan value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(TimeSpan value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ushort value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ushort value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ushort value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ushort value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(uint value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(uint value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(uint value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(uint value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ulong value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ulong value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan(), null))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ulong value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ulong value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(Guid value)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>)))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(Guid value, string format)
		{
			int num;
			if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan()))
			{
				this.Grow(num);
				if (!value.TryFormat(this.buffer.AsSpan(this.index), out num, format.AsSpan()))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(Guid value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(Guid value, string format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		static Utf16ValueStringBuilder()
		{
			char[] array = Environment.NewLine.ToCharArray();
			if (array.Length == 1)
			{
				Utf16ValueStringBuilder.newLine1 = array[0];
				Utf16ValueStringBuilder.crlf = false;
				return;
			}
			Utf16ValueStringBuilder.newLine1 = array[0];
			Utf16ValueStringBuilder.newLine2 = array[1];
			Utf16ValueStringBuilder.crlf = true;
		}

		public int Length
		{
			get
			{
				return this.index;
			}
		}

		[NullableContext(0)]
		public ReadOnlySpan<char> AsSpan()
		{
			return this.buffer.AsSpan(0, this.index);
		}

		[NullableContext(0)]
		public ReadOnlyMemory<char> AsMemory()
		{
			return this.buffer.AsMemory(0, this.index);
		}

		[NullableContext(0)]
		public ArraySegment<char> AsArraySegment()
		{
			return new ArraySegment<char>(this.buffer, 0, this.index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Utf16ValueStringBuilder(bool disposeImmediately)
		{
			if (disposeImmediately && Utf16ValueStringBuilder.scratchBufferUsed)
			{
				Utf16ValueStringBuilder.ThrowNestedException();
			}
			char[] array;
			if (disposeImmediately)
			{
				array = Utf16ValueStringBuilder.scratchBuffer;
				if (array == null)
				{
					array = (Utf16ValueStringBuilder.scratchBuffer = new char[31111]);
				}
				Utf16ValueStringBuilder.scratchBufferUsed = true;
			}
			else
			{
				array = ArrayPool<char>.Shared.Rent(32768);
			}
			this.buffer = array;
			this.index = 0;
			this.disposeImmediately = disposeImmediately;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (this.buffer != null)
			{
				if (this.buffer.Length != 31111)
				{
					ArrayPool<char>.Shared.Return(this.buffer, false);
				}
				this.buffer = null;
				this.index = 0;
				if (this.disposeImmediately)
				{
					Utf16ValueStringBuilder.scratchBufferUsed = false;
				}
			}
		}

		public void Clear()
		{
			this.index = 0;
		}

		public void TryGrow(int sizeHint)
		{
			if (this.buffer.Length < this.index + sizeHint)
			{
				this.Grow(sizeHint);
			}
		}

		public void Grow(int sizeHint)
		{
			int num = this.buffer.Length * 2;
			if (sizeHint != 0)
			{
				num = Math.Max(num, this.index + sizeHint);
			}
			char[] array = ArrayPool<char>.Shared.Rent(num);
			this.buffer.CopyTo(array, 0);
			if (this.buffer.Length != 31111)
			{
				ArrayPool<char>.Shared.Return(this.buffer, false);
			}
			this.buffer = array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine()
		{
			if (Utf16ValueStringBuilder.crlf)
			{
				if (this.buffer.Length - this.index < 2)
				{
					this.Grow(2);
				}
				this.buffer[this.index] = Utf16ValueStringBuilder.newLine1;
				this.buffer[this.index + 1] = Utf16ValueStringBuilder.newLine2;
				this.index += 2;
				return;
			}
			if (this.buffer.Length - this.index < 1)
			{
				this.Grow(1);
			}
			this.buffer[this.index] = Utf16ValueStringBuilder.newLine1;
			this.index++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char value)
		{
			if (this.buffer.Length - this.index < 1)
			{
				this.Grow(1);
			}
			char[] array = this.buffer;
			int num = this.index;
			this.index = num + 1;
			array[num] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char value, int repeatCount)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException("repeatCount");
			}
			this.GetSpan(repeatCount).Fill(value);
			this.Advance(repeatCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(char value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(string value)
		{
			this.Append(value.AsSpan());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(string value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(string value, int startIndex, int count)
		{
			if (value != null)
			{
				this.Append(value.AsSpan(startIndex, count));
				return;
			}
			if (startIndex == 0 && count == 0)
			{
				return;
			}
			throw new ArgumentNullException("value");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(char[] value, int startIndex, int charCount)
		{
			if (this.buffer.Length - this.index < charCount)
			{
				this.Grow(charCount);
			}
			Array.Copy(value, startIndex, this.buffer, this.index, charCount);
			this.index += charCount;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ReadOnlySpan<char> value)
		{
			if (this.buffer.Length - this.index < value.Length)
			{
				this.Grow(value.Length);
			}
			value.CopyTo(this.buffer.AsSpan(this.index));
			this.index += value.Length;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ReadOnlySpan<char> value)
		{
			this.Append(value);
			this.AppendLine();
		}

		public void Append<[Nullable(2)] T>(T value)
		{
			int num;
			if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(value, this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>)))
			{
				this.Grow(num);
				if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(value, this.buffer.AsSpan(this.index), out num, default(ReadOnlySpan<char>)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		public void AppendLine<[Nullable(2)] T>(T value)
		{
			this.Append<T>(value);
			this.AppendLine();
		}

		public void Insert(int index, string value, int count)
		{
			this.Insert(index, value.AsSpan(), count);
		}

		public void Insert(int index, string value)
		{
			this.Insert(index, value.AsSpan(), 1);
		}

		[NullableContext(0)]
		public void Insert(int index, ReadOnlySpan<char> value, int count)
		{
			if (count < 0)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("count");
			}
			int length = this.Length;
			if (index > length)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("index");
			}
			if (value.Length == 0 || count == 0)
			{
				return;
			}
			int val = index + value.Length * count;
			char[] array = ArrayPool<char>.Shared.Rent(Math.Max(32768, val));
			this.buffer.AsSpan(0, index).CopyTo(array);
			int num = index;
			for (int i = 0; i < count; i++)
			{
				value.CopyTo(array.AsSpan(num));
				num += value.Length;
			}
			int num2 = this.index - index;
			this.buffer.AsSpan(index, num2).CopyTo(array.AsSpan(num));
			if (this.buffer.Length != 31111 && this.buffer != null)
			{
				ArrayPool<char>.Shared.Return(this.buffer, false);
			}
			this.buffer = array;
			this.index = num + num2;
		}

		public void Replace(char oldChar, char newChar)
		{
			this.Replace(oldChar, newChar, 0, this.Length);
		}

		public void Replace(char oldChar, char newChar, int startIndex, int count)
		{
			int length = this.Length;
			if (startIndex > length)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex > length - count)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("count");
			}
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (this.buffer[i] == oldChar)
				{
					this.buffer[i] = newChar;
				}
			}
		}

		public void Replace(string oldValue, string newValue)
		{
			this.Replace(oldValue, newValue, 0, this.Length);
		}

		[NullableContext(0)]
		public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue)
		{
			this.Replace(oldValue, newValue, 0, this.Length);
		}

		public void Replace(string oldValue, string newValue, int startIndex, int count)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			this.Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
		}

		[NullableContext(0)]
		public void Replace(ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, int startIndex, int count)
		{
			int length = this.Length;
			if (startIndex > length)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex > length - count)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("count");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("oldValue.Length is 0", "oldValue");
			}
			ReadOnlySpan<char> readOnlySpan = this.AsSpan();
			int num = startIndex + count;
			int num2 = 0;
			for (int i = startIndex; i < num; i += oldValue.Length)
			{
				int num3 = readOnlySpan.Slice(i, num - i).IndexOf(oldValue, StringComparison.Ordinal);
				if (num3 == -1)
				{
					break;
				}
				i += num3;
				num2++;
			}
			if (num2 == 0)
			{
				return;
			}
			char[] array = ArrayPool<char>.Shared.Rent(Math.Max(32768, this.Length + (newValue.Length - oldValue.Length) * num2));
			this.buffer.AsSpan(0, startIndex).CopyTo(array);
			int num4 = startIndex;
			for (int j = startIndex; j < num; j += oldValue.Length)
			{
				int num5 = readOnlySpan.Slice(j, num - j).IndexOf(oldValue, StringComparison.Ordinal);
				if (num5 == -1)
				{
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(j);
					readOnlySpan2.CopyTo(array.AsSpan(num4));
					num4 += readOnlySpan2.Length;
					break;
				}
				readOnlySpan.Slice(j, num5).CopyTo(array.AsSpan(num4));
				newValue.CopyTo(array.AsSpan(num4 + num5));
				num4 += num5 + newValue.Length;
				j += num5;
			}
			if (this.buffer.Length != 31111)
			{
				ArrayPool<char>.Shared.Return(this.buffer, false);
			}
			this.buffer = array;
			this.index = num4;
		}

		public void ReplaceAt(char newChar, int replaceIndex)
		{
			int length = this.Length;
			if (replaceIndex > length)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("replaceIndex");
			}
			this.buffer[replaceIndex] = newChar;
		}

		public void Remove(int startIndex, int length)
		{
			if (length < 0)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("length");
			}
			if (startIndex < 0)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("startIndex");
			}
			if (length > this.Length - startIndex)
			{
				Utf16ValueStringBuilder.ExceptionUtil.ThrowArgumentOutOfRangeException("length");
			}
			if (this.Length == length && startIndex == 0)
			{
				this.index = 0;
				return;
			}
			if (length == 0)
			{
				return;
			}
			int num = startIndex + length;
			this.buffer.AsSpan(num, this.Length - num).CopyTo(this.buffer.AsSpan(startIndex));
			this.index -= length;
		}

		[NullableContext(0)]
		public bool TryCopyTo(Span<char> destination, out int charsWritten)
		{
			if (destination.Length < this.index)
			{
				charsWritten = 0;
				return false;
			}
			charsWritten = this.index;
			this.buffer.AsSpan(0, this.index).CopyTo(destination);
			return true;
		}

		public override string ToString()
		{
			if (this.index == 0)
			{
				return string.Empty;
			}
			return new string(this.buffer, 0, this.index);
		}

		[NullableContext(0)]
		public Memory<char> GetMemory(int sizeHint)
		{
			if (this.buffer.Length - this.index < sizeHint)
			{
				this.Grow(sizeHint);
			}
			return this.buffer.AsMemory(this.index);
		}

		[NullableContext(0)]
		public Span<char> GetSpan(int sizeHint)
		{
			if (this.buffer.Length - this.index < sizeHint)
			{
				this.Grow(sizeHint);
			}
			return this.buffer.AsSpan(this.index);
		}

		public void Advance(int count)
		{
			this.index += count;
		}

		void IResettableBufferWriter<char>.Reset()
		{
			this.index = 0;
		}

		private void ThrowArgumentException(string paramName)
		{
			throw new ArgumentException("Can't format argument.", paramName);
		}

		private static void ThrowFormatException()
		{
			throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
		}

		private unsafe void AppendFormatInternal<[Nullable(2)] T>(T arg, int width, [Nullable(0)] ReadOnlySpan<char> format, string argName)
		{
			if (width <= 0)
			{
				width *= -1;
				int num;
				if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, this.buffer.AsSpan(this.index), out num, format))
				{
					this.Grow(num);
					if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, this.buffer.AsSpan(this.index), out num, format))
					{
						this.ThrowArgumentException(argName);
					}
				}
				this.index += num;
				int num2 = width - num;
				if (width > 0 && num2 > 0)
				{
					this.Append(' ', num2);
					return;
				}
			}
			else
			{
				if (typeof(T) == typeof(string))
				{
					string text = Unsafe.As<string>(arg);
					int num3 = width - text.Length;
					if (num3 > 0)
					{
						this.Append(' ', num3);
					}
					this.Append(text);
					return;
				}
				int num4 = typeof(T).IsValueType ? (Unsafe.SizeOf<T>() * 8) : 1024;
				Span<char> destination = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)num4) * 2)], num4);
				int num5;
				if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num5, format))
				{
					num4 = destination.Length * 2;
					destination = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)num4) * 2)], num4);
					if (!Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num5, format))
					{
						this.ThrowArgumentException(argName);
					}
				}
				int num6 = width - num5;
				if (num6 > 0)
				{
					this.Append(' ', num6);
				}
				this.Append(destination.Slice(0, num5));
			}
		}

		private static void ThrowNestedException()
		{
			throw new NestedStringBuilderCreationException("Utf16ValueStringBuilder", "");
		}

		public static void RegisterTryFormat<[Nullable(2)] T>(Utf16ValueStringBuilder.TryFormat<T> formatMethod)
		{
			Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate = formatMethod;
		}

		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			0
		})]
		private static Utf16ValueStringBuilder.TryFormat<T?> CreateNullableFormatter<T>() where T : struct
		{
			return delegate(T? x, Span<char> dest, out int written, ReadOnlySpan<char> format)
			{
				if (x == null)
				{
					written = 0;
					return true;
				}
				return Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(x.Value, dest, out written, format);
			};
		}

		[NullableContext(0)]
		public static void EnableNullableFormat<T>() where T : struct
		{
			Utf16ValueStringBuilder.RegisterTryFormat<T?>(Utf16ValueStringBuilder.CreateNullableFormatter<T>());
		}

		private const int ThreadStaticBufferSize = 31111;

		private const int DefaultBufferSize = 32768;

		private static char newLine1;

		private static char newLine2;

		private static bool crlf;

		[Nullable(2)]
		[ThreadStatic]
		private static char[] scratchBuffer;

		[ThreadStatic]
		internal static bool scratchBufferUsed;

		[Nullable(2)]
		private char[] buffer;

		private int index;

		private bool disposeImmediately;

		[NullableContext(0)]
		public delegate bool TryFormat<[Nullable(2)] T>([Nullable(1)] T value, Span<char> destination, out int charsWritten, ReadOnlySpan<char> format);

		[NullableContext(0)]
		private static class ExceptionUtil
		{
			[NullableContext(1)]
			public static void ThrowArgumentOutOfRangeException(string paramName)
			{
				throw new ArgumentOutOfRangeException(paramName);
			}
		}

		[NullableContext(0)]
		public static class FormatterCache<[Nullable(2)] T>
		{
			static FormatterCache()
			{
				Utf16ValueStringBuilder.TryFormat<T> tryFormat = (Utf16ValueStringBuilder.TryFormat<T>)Utf16ValueStringBuilder.CreateFormatter(typeof(T));
				if (tryFormat == null)
				{
					if (typeof(T).IsEnum)
					{
						tryFormat = new Utf16ValueStringBuilder.TryFormat<T>(EnumUtil<T>.TryFormatUtf16);
					}
					else if (typeof(T) == typeof(string))
					{
						tryFormat = new Utf16ValueStringBuilder.TryFormat<T>(Utf16ValueStringBuilder.FormatterCache<T>.TryFormatString);
					}
					else
					{
						tryFormat = new Utf16ValueStringBuilder.TryFormat<T>(Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDefault);
					}
				}
				Utf16ValueStringBuilder.FormatterCache<T>.TryFormatDelegate = tryFormat;
			}

			private static bool TryFormatString([Nullable(1)] T value, Span<char> dest, out int written, ReadOnlySpan<char> format)
			{
				string text = value as string;
				if (text == null)
				{
					written = 0;
					return true;
				}
				written = text.Length;
				return text.AsSpan().TryCopyTo(dest);
			}

			private static bool TryFormatDefault([Nullable(1)] T value, Span<char> dest, out int written, ReadOnlySpan<char> format)
			{
				if (value == null)
				{
					written = 0;
					return true;
				}
				IFormattable formattable = value as IFormattable;
				string text = (formattable != null && format.Length != 0) ? formattable.ToString(format.ToString(), null) : value.ToString();
				written = text.Length;
				return text.AsSpan().TryCopyTo(dest);
			}

			[Nullable(1)]
			public static Utf16ValueStringBuilder.TryFormat<T> TryFormatDelegate;
		}
	}
}
