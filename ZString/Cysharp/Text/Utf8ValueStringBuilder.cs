using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public struct Utf8ValueStringBuilder : IDisposable, IBufferWriter<byte>, IResettableBufferWriter<byte>
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						if (parseResult.Index == 0)
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
						}
						else
						{
							this.ThrowFormatException();
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						int num2 = parseResult.Index;
						if (num2 != 0)
						{
							if (num2 != 1)
							{
								this.ThrowFormatException();
							}
							else
							{
								this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							}
						}
						else
						{
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num3 = format.Length - num;
			if (num3 > 0)
			{
				this.Append(format, num, num3);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, format2, "arg12");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, format2, "arg13");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, format2, "arg14");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, format2, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, format2, "arg15");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
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
						int length = i - num;
						this.Append(format.AsSpan(num, length));
						FormatParser.ParseResult parseResult = FormatParser.Parse(format, i);
						num = parseResult.LastIndex;
						i = parseResult.LastIndex - 1;
						StandardFormat format2 = StandardFormat.Parse(parseResult.FormatString);
						switch (parseResult.Index)
						{
						case 0:
							this.AppendFormatInternal<T1>(arg1, parseResult.Alignment, format2, "arg1");
							break;
						case 1:
							this.AppendFormatInternal<T2>(arg2, parseResult.Alignment, format2, "arg2");
							break;
						case 2:
							this.AppendFormatInternal<T3>(arg3, parseResult.Alignment, format2, "arg3");
							break;
						case 3:
							this.AppendFormatInternal<T4>(arg4, parseResult.Alignment, format2, "arg4");
							break;
						case 4:
							this.AppendFormatInternal<T5>(arg5, parseResult.Alignment, format2, "arg5");
							break;
						case 5:
							this.AppendFormatInternal<T6>(arg6, parseResult.Alignment, format2, "arg6");
							break;
						case 6:
							this.AppendFormatInternal<T7>(arg7, parseResult.Alignment, format2, "arg7");
							break;
						case 7:
							this.AppendFormatInternal<T8>(arg8, parseResult.Alignment, format2, "arg8");
							break;
						case 8:
							this.AppendFormatInternal<T9>(arg9, parseResult.Alignment, format2, "arg9");
							break;
						case 9:
							this.AppendFormatInternal<T10>(arg10, parseResult.Alignment, format2, "arg10");
							break;
						case 10:
							this.AppendFormatInternal<T11>(arg11, parseResult.Alignment, format2, "arg11");
							break;
						case 11:
							this.AppendFormatInternal<T12>(arg12, parseResult.Alignment, format2, "arg12");
							break;
						case 12:
							this.AppendFormatInternal<T13>(arg13, parseResult.Alignment, format2, "arg13");
							break;
						case 13:
							this.AppendFormatInternal<T14>(arg14, parseResult.Alignment, format2, "arg14");
							break;
						case 14:
							this.AppendFormatInternal<T15>(arg15, parseResult.Alignment, format2, "arg15");
							break;
						case 15:
							this.AppendFormatInternal<T16>(arg16, parseResult.Alignment, format2, "arg16");
							break;
						default:
							this.ThrowFormatException();
							break;
						}
					}
				}
				else if (c == '}')
				{
					if (i + 1 < format.Length && format[i + 1] == '}')
					{
						int count2 = i - num;
						this.Append(format, num, count2);
						i++;
						num = i;
					}
					else
					{
						this.ThrowFormatException();
					}
				}
			}
			int num2 = format.Length - num;
			if (num2 > 0)
			{
				this.Append(format, num, num2);
			}
		}

		[return: Nullable(2)]
		private static object CreateFormatter(Type type)
		{
			if (type == typeof(byte))
			{
				return new Utf8ValueStringBuilder.TryFormat<byte>(delegate(byte x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(DateTime))
			{
				return new Utf8ValueStringBuilder.TryFormat<DateTime>(delegate(DateTime x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(DateTimeOffset))
			{
				return new Utf8ValueStringBuilder.TryFormat<DateTimeOffset>(delegate(DateTimeOffset x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(decimal))
			{
				return new Utf8ValueStringBuilder.TryFormat<decimal>(delegate(decimal x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(double))
			{
				return new Utf8ValueStringBuilder.TryFormat<double>(delegate(double x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(short))
			{
				return new Utf8ValueStringBuilder.TryFormat<short>(delegate(short x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(int))
			{
				return new Utf8ValueStringBuilder.TryFormat<int>(delegate(int x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(long))
			{
				return new Utf8ValueStringBuilder.TryFormat<long>(delegate(long x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(sbyte))
			{
				return new Utf8ValueStringBuilder.TryFormat<sbyte>(delegate(sbyte x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(float))
			{
				return new Utf8ValueStringBuilder.TryFormat<float>(delegate(float x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(TimeSpan))
			{
				return new Utf8ValueStringBuilder.TryFormat<TimeSpan>(delegate(TimeSpan x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(ushort))
			{
				return new Utf8ValueStringBuilder.TryFormat<ushort>(delegate(ushort x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(uint))
			{
				return new Utf8ValueStringBuilder.TryFormat<uint>(delegate(uint x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(ulong))
			{
				return new Utf8ValueStringBuilder.TryFormat<ulong>(delegate(ulong x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(Guid))
			{
				return new Utf8ValueStringBuilder.TryFormat<Guid>(delegate(Guid x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(bool))
			{
				return new Utf8ValueStringBuilder.TryFormat<bool>(delegate(bool x, Span<byte> dest, out int written, StandardFormat format)
				{
					return Utf8Formatter.TryFormat(x, dest, out written, format);
				});
			}
			if (type == typeof(byte?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<byte>();
			}
			if (type == typeof(DateTime?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<DateTime>();
			}
			if (type == typeof(DateTimeOffset?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<DateTimeOffset>();
			}
			if (type == typeof(decimal?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<decimal>();
			}
			if (type == typeof(double?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<double>();
			}
			if (type == typeof(short?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<short>();
			}
			if (type == typeof(int?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<int>();
			}
			if (type == typeof(long?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<long>();
			}
			if (type == typeof(sbyte?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<sbyte>();
			}
			if (type == typeof(float?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<float>();
			}
			if (type == typeof(TimeSpan?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<TimeSpan>();
			}
			if (type == typeof(ushort?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<ushort>();
			}
			if (type == typeof(uint?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<uint>();
			}
			if (type == typeof(ulong?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<ulong>();
			}
			if (type == typeof(Guid?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<Guid>();
			}
			if (type == typeof(bool?))
			{
				return Utf8ValueStringBuilder.CreateNullableFormatter<bool>();
			}
			if (type == typeof(IntPtr))
			{
				return new Utf8ValueStringBuilder.TryFormat<IntPtr>(delegate(IntPtr x, Span<byte> dest, out int written, StandardFormat format)
				{
					if (IntPtr.Size != 4)
					{
						return Utf8Formatter.TryFormat(x.ToInt64(), dest, out written, format);
					}
					return Utf8Formatter.TryFormat(x.ToInt32(), dest, out written, format);
				});
			}
			if (type == typeof(UIntPtr))
			{
				return new Utf8ValueStringBuilder.TryFormat<UIntPtr>(delegate(UIntPtr x, Span<byte> dest, out int written, StandardFormat format)
				{
					if (UIntPtr.Size != 4)
					{
						return Utf8Formatter.TryFormat(x.ToUInt64(), dest, out written, format);
					}
					return Utf8Formatter.TryFormat(x.ToUInt32(), dest, out written, format);
				});
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(byte value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(byte value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTime value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTime value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(DateTime value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTimeOffset value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(DateTimeOffset value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(DateTimeOffset value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(decimal value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(decimal value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(decimal value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(double value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(double value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(double value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(short value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(short value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(short value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(int value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(int value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(int value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(long value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(long value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(long value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(sbyte value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(sbyte value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(sbyte value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(float value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(float value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(float value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(TimeSpan value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(TimeSpan value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(TimeSpan value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ushort value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ushort value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(ushort value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(uint value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(uint value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(uint value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ulong value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ulong value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(ulong value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(Guid value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(Guid value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
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
		public void AppendLine(Guid value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(bool value)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(bool value, StandardFormat format)
		{
			int num;
			if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
			{
				this.Grow(num);
				if (!Utf8Formatter.TryFormat(value, this.buffer.AsSpan(this.index), out num, format))
				{
					this.ThrowArgumentException("value");
				}
			}
			this.index += num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(bool value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(bool value, StandardFormat format)
		{
			this.Append(value, format);
			this.AppendLine();
		}

		static Utf8ValueStringBuilder()
		{
			byte[] bytes = Utf8ValueStringBuilder.UTF8NoBom.GetBytes(Environment.NewLine);
			if (bytes.Length == 1)
			{
				Utf8ValueStringBuilder.newLine1 = bytes[0];
				Utf8ValueStringBuilder.crlf = false;
				return;
			}
			Utf8ValueStringBuilder.newLine1 = bytes[0];
			Utf8ValueStringBuilder.newLine2 = bytes[1];
			Utf8ValueStringBuilder.crlf = true;
		}

		public int Length
		{
			get
			{
				return this.index;
			}
		}

		[NullableContext(0)]
		public ReadOnlySpan<byte> AsSpan()
		{
			return this.buffer.AsSpan(0, this.index);
		}

		[NullableContext(0)]
		public ReadOnlyMemory<byte> AsMemory()
		{
			return this.buffer.AsMemory(0, this.index);
		}

		[NullableContext(0)]
		public ArraySegment<byte> AsArraySegment()
		{
			return new ArraySegment<byte>(this.buffer, 0, this.index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Utf8ValueStringBuilder(bool disposeImmediately)
		{
			if (disposeImmediately && Utf8ValueStringBuilder.scratchBufferUsed)
			{
				Utf8ValueStringBuilder.ThrowNestedException();
			}
			byte[] array;
			if (disposeImmediately)
			{
				array = Utf8ValueStringBuilder.scratchBuffer;
				if (array == null)
				{
					array = (Utf8ValueStringBuilder.scratchBuffer = new byte[64444]);
				}
				Utf8ValueStringBuilder.scratchBufferUsed = true;
			}
			else
			{
				array = ArrayPool<byte>.Shared.Rent(65536);
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
				if (this.buffer.Length != 64444)
				{
					ArrayPool<byte>.Shared.Return(this.buffer, false);
				}
				this.buffer = null;
				this.index = 0;
				if (this.disposeImmediately)
				{
					Utf8ValueStringBuilder.scratchBufferUsed = false;
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
			byte[] array = ArrayPool<byte>.Shared.Rent(num);
			this.buffer.CopyTo(array, 0);
			if (this.buffer.Length != 64444)
			{
				ArrayPool<byte>.Shared.Return(this.buffer, false);
			}
			this.buffer = array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine()
		{
			if (Utf8ValueStringBuilder.crlf)
			{
				if (this.buffer.Length - this.index < 2)
				{
					this.Grow(2);
				}
				this.buffer[this.index] = Utf8ValueStringBuilder.newLine1;
				this.buffer[this.index + 1] = Utf8ValueStringBuilder.newLine2;
				this.index += 2;
				return;
			}
			if (this.buffer.Length - this.index < 1)
			{
				this.Grow(1);
			}
			this.buffer[this.index] = Utf8ValueStringBuilder.newLine1;
			this.index++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Append(char value)
		{
			int maxByteCount = Utf8ValueStringBuilder.UTF8NoBom.GetMaxByteCount(1);
			if (this.buffer.Length - this.index < maxByteCount)
			{
				this.Grow(maxByteCount);
			}
			fixed (byte* ptr = &this.buffer[this.index])
			{
				byte* bytes = ptr;
				this.index += Utf8ValueStringBuilder.UTF8NoBom.GetBytes(&value, 1, bytes, maxByteCount);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Append(char value, int repeatCount)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException("repeatCount");
			}
			if (value <= '\u007f')
			{
				this.GetSpan(repeatCount).Fill((byte)value);
				this.Advance(repeatCount);
				return;
			}
			int maxByteCount = Utf8ValueStringBuilder.UTF8NoBom.GetMaxByteCount(1);
			Span<byte> bytes = new Span<byte>(stackalloc byte[(UIntPtr)maxByteCount], maxByteCount);
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)value;
			ReadOnlySpan<char> chars = new Span<char>(intPtr, 1);
			int bytes2 = Utf8ValueStringBuilder.UTF8NoBom.GetBytes(chars, bytes);
			this.TryGrow(bytes2 * repeatCount);
			for (int i = 0; i < repeatCount; i++)
			{
				bytes.CopyTo(this.GetSpan(bytes2));
				this.Advance(bytes2);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(char value)
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

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Append(ReadOnlySpan<char> value)
		{
			int maxByteCount = Utf8ValueStringBuilder.UTF8NoBom.GetMaxByteCount(value.Length);
			if (this.buffer.Length - this.index < maxByteCount)
			{
				this.Grow(maxByteCount);
			}
			this.index += Utf8ValueStringBuilder.UTF8NoBom.GetBytes(value, this.buffer.AsSpan(this.index));
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLine(ReadOnlySpan<char> value)
		{
			this.Append(value);
			this.AppendLine();
		}

		[NullableContext(0)]
		public void AppendLiteral(ReadOnlySpan<byte> value)
		{
			if (this.buffer.Length - this.index < value.Length)
			{
				this.Grow(value.Length);
			}
			value.CopyTo(this.buffer.AsSpan(this.index));
			this.index += value.Length;
		}

		public void Append<[Nullable(2)] T>(T value)
		{
			int num;
			if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
			{
				this.Grow(num);
				if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(value, this.buffer.AsSpan(this.index), out num, default(StandardFormat)))
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

		public void CopyTo(IBufferWriter<byte> bufferWriter)
		{
			Span<byte> span = bufferWriter.GetSpan(this.index);
			int count;
			this.TryCopyTo(span, out count);
			bufferWriter.Advance(count);
		}

		[NullableContext(0)]
		public bool TryCopyTo(Span<byte> destination, out int bytesWritten)
		{
			if (destination.Length < this.index)
			{
				bytesWritten = 0;
				return false;
			}
			bytesWritten = this.index;
			this.buffer.AsSpan(0, this.index).CopyTo(destination);
			return true;
		}

		public Task WriteToAsync(Stream stream)
		{
			return stream.WriteAsync(this.buffer, 0, this.index);
		}

		public Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
		{
			return stream.WriteAsync(this.buffer, 0, this.index, cancellationToken);
		}

		public override string ToString()
		{
			if (this.index == 0)
			{
				return string.Empty;
			}
			return Utf8ValueStringBuilder.UTF8NoBom.GetString(this.buffer, 0, this.index);
		}

		[NullableContext(0)]
		public Memory<byte> GetMemory(int sizeHint)
		{
			if (this.buffer.Length - this.index < sizeHint)
			{
				this.Grow(sizeHint);
			}
			return this.buffer.AsMemory(this.index);
		}

		[NullableContext(0)]
		public Span<byte> GetSpan(int sizeHint)
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

		void IResettableBufferWriter<byte>.Reset()
		{
			this.index = 0;
		}

		private void ThrowArgumentException(string paramName)
		{
			throw new ArgumentException("Can't format argument.", paramName);
		}

		private void ThrowFormatException()
		{
			throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
		}

		private static void ThrowNestedException()
		{
			throw new NestedStringBuilderCreationException("Utf16ValueStringBuilder", "");
		}

		private unsafe void AppendFormatInternal<[Nullable(2)] T>(T arg, int width, StandardFormat format, string argName)
		{
			if (width <= 0)
			{
				width *= -1;
				int num;
				if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, this.buffer.AsSpan(this.index), out num, format))
				{
					this.Grow(num);
					if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, this.buffer.AsSpan(this.index), out num, format))
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
				Span<byte> destination = new Span<byte>(stackalloc byte[(UIntPtr)num4], num4);
				int num5;
				if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num5, format))
				{
					num4 = destination.Length * 2;
					destination = new Span<byte>(stackalloc byte[(UIntPtr)num4], num4);
					if (!Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(arg, destination, out num5, format))
					{
						this.ThrowArgumentException(argName);
					}
				}
				int num6 = width - num5;
				if (num6 > 0)
				{
					this.Append(' ', num6);
				}
				destination.CopyTo(this.GetSpan(num5));
				this.Advance(num5);
			}
		}

		public static void RegisterTryFormat<[Nullable(2)] T>(Utf8ValueStringBuilder.TryFormat<T> formatMethod)
		{
			Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate = formatMethod;
		}

		[NullableContext(0)]
		[return: Nullable(new byte[]
		{
			1,
			0
		})]
		private static Utf8ValueStringBuilder.TryFormat<T?> CreateNullableFormatter<T>() where T : struct
		{
			return delegate(T? x, Span<byte> destination, out int written, StandardFormat format)
			{
				if (x == null)
				{
					written = 0;
					return true;
				}
				return Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate(x.Value, destination, out written, format);
			};
		}

		[NullableContext(0)]
		public static void EnableNullableFormat<T>() where T : struct
		{
			Utf8ValueStringBuilder.RegisterTryFormat<T?>(Utf8ValueStringBuilder.CreateNullableFormatter<T>());
		}

		private const int ThreadStaticBufferSize = 64444;

		private const int DefaultBufferSize = 65536;

		private static Encoding UTF8NoBom = new UTF8Encoding(false);

		private static byte newLine1;

		private static byte newLine2;

		private static bool crlf;

		[Nullable(2)]
		[ThreadStatic]
		private static byte[] scratchBuffer;

		[ThreadStatic]
		internal static bool scratchBufferUsed;

		[Nullable(2)]
		private byte[] buffer;

		private int index;

		private bool disposeImmediately;

		[NullableContext(0)]
		public delegate bool TryFormat<[Nullable(2)] T>([Nullable(1)] T value, Span<byte> destination, out int written, StandardFormat format);

		[NullableContext(0)]
		public static class FormatterCache<[Nullable(2)] T>
		{
			static FormatterCache()
			{
				Utf8ValueStringBuilder.TryFormat<T> tryFormat = (Utf8ValueStringBuilder.TryFormat<T>)Utf8ValueStringBuilder.CreateFormatter(typeof(T));
				if (tryFormat == null)
				{
					if (typeof(T).IsEnum)
					{
						tryFormat = new Utf8ValueStringBuilder.TryFormat<T>(EnumUtil<T>.TryFormatUtf8);
					}
					else
					{
						tryFormat = new Utf8ValueStringBuilder.TryFormat<T>(Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDefault);
					}
				}
				Utf8ValueStringBuilder.FormatterCache<T>.TryFormatDelegate = tryFormat;
			}

			private static bool TryFormatDefault([Nullable(1)] T value, Span<byte> dest, out int written, StandardFormat format)
			{
				if (value == null)
				{
					written = 0;
					return true;
				}
				string text;
				if (!(typeof(T) == typeof(string)))
				{
					IFormattable formattable = value as IFormattable;
					text = ((formattable != null && format != default(StandardFormat)) ? formattable.ToString(format.ToString(), null) : value.ToString());
				}
				else
				{
					text = Unsafe.As<string>(value);
				}
				string text2 = text;
				written = Utf8ValueStringBuilder.UTF8NoBom.GetMaxByteCount(text2.Length);
				if (dest.Length < written)
				{
					return false;
				}
				written = Utf8ValueStringBuilder.UTF8NoBom.GetBytes(text2.AsSpan(), dest);
				return true;
			}

			[Nullable(1)]
			public static Utf8ValueStringBuilder.TryFormat<T> TryFormatDelegate;
		}
	}
}
