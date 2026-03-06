using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cysharp.Text
{
	internal static class EnumUtil<[Nullable(2)] T>
	{
		static EnumUtil()
		{
			string[] array = Enum.GetNames(typeof(T));
			Array values = Enum.GetValues(typeof(T));
			EnumUtil<T>.names = new Dictionary<T, string>(array.Length);
			EnumUtil<T>.utf8names = new Dictionary<T, byte[]>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (EnumUtil<T>.names.ContainsKey((T)((object)values.GetValue(i))))
				{
					EnumUtil<T>.names[(T)((object)values.GetValue(i))] = "$";
					EnumUtil<T>.utf8names[(T)((object)values.GetValue(i))] = Array.Empty<byte>();
				}
				else
				{
					EnumUtil<T>.names.Add((T)((object)values.GetValue(i)), array[i]);
					EnumUtil<T>.utf8names.Add((T)((object)values.GetValue(i)), Encoding.UTF8.GetBytes(array[i]));
				}
			}
		}

		public static bool TryFormatUtf16([Nullable(1)] T value, Span<char> dest, out int written, ReadOnlySpan<char> _)
		{
			string text;
			if (!EnumUtil<T>.names.TryGetValue(value, out text) || text == "$")
			{
				text = value.ToString();
			}
			written = text.Length;
			return text.AsSpan().TryCopyTo(dest);
		}

		public static bool TryFormatUtf8([Nullable(1)] T value, Span<byte> dest, out int written, StandardFormat _)
		{
			byte[] bytes;
			if (!EnumUtil<T>.utf8names.TryGetValue(value, out bytes) || bytes.Length == 0)
			{
				bytes = Encoding.UTF8.GetBytes(value.ToString());
			}
			written = bytes.Length;
			return bytes.AsSpan<byte>().TryCopyTo(dest);
		}

		[Nullable(1)]
		private const string InvalidName = "$";

		[Nullable(1)]
		private static readonly Dictionary<T, string> names;

		[Nullable(1)]
		private static readonly Dictionary<T, byte[]> utf8names;
	}
}
