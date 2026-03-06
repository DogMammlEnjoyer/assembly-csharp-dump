using System;
using System.Runtime.CompilerServices;
using VYaml.Annotations;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class KeyNameMutator
	{
		public static string Mutate(string s, NamingConvention namingConvention)
		{
			string result;
			switch (namingConvention)
			{
			case NamingConvention.LowerCamelCase:
				result = KeyNameMutator.ToLowerCamelCase(s);
				break;
			case NamingConvention.UpperCamelCase:
				result = s;
				break;
			case NamingConvention.SnakeCase:
				result = KeyNameMutator.ToSnakeCase(s, '_');
				break;
			case NamingConvention.KebabCase:
				result = KeyNameMutator.ToSnakeCase(s, '-');
				break;
			default:
				throw new ArgumentOutOfRangeException("namingConvention", namingConvention, null);
			}
			return result;
		}

		public unsafe static string ToLowerCamelCase(string s)
		{
			ReadOnlySpan<char> readOnlySpan = s.AsSpan();
			if (readOnlySpan.Length <= 0 || (readOnlySpan.Length <= 1 && char.IsLower((char)(*readOnlySpan[0]))))
			{
				return s;
			}
			int length = readOnlySpan.Length;
			Span<char> span = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)length) * 2)], length);
			*span[0] = char.ToLowerInvariant((char)(*readOnlySpan[0]));
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
			ReadOnlySpan<char> readOnlySpan3 = readOnlySpan2.Slice(1, readOnlySpan2.Length - 1);
			Span<char> span2 = span;
			readOnlySpan3.CopyTo(span2.Slice(1, span2.Length - 1));
			return span.ToString();
		}

		public unsafe static string ToSnakeCase(string s, char separator = '_')
		{
			ReadOnlySpan<char> readOnlySpan = s.AsSpan();
			if (readOnlySpan.Length <= 0)
			{
				return s;
			}
			int i = readOnlySpan.Length * 2;
			Span<char> span = new Span<char>(stackalloc byte[checked(unchecked((UIntPtr)i) * 2)], i);
			int num = 0;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
			for (i = 0; i < readOnlySpan2.Length; i++)
			{
				char c = (char)(*readOnlySpan2[i]);
				if (char.IsUpper(c))
				{
					if (num == 0 || char.IsUpper((char)(*readOnlySpan[num - 1])))
					{
						*span[num++] = char.ToLowerInvariant(c);
					}
					else
					{
						*span[num++] = separator;
						if (span.Length <= num)
						{
							span = new char[span.Length * 2];
						}
						*span[num++] = char.ToLowerInvariant(c);
					}
				}
				else
				{
					*span[num++] = c;
				}
			}
			Span<char> span2 = span;
			return span2.Slice(0, num).ToString();
		}
	}
}
