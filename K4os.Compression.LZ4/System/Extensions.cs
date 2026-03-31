using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(2)]
	[Nullable(0)]
	internal static class Extensions
	{
		[NullableContext(1)]
		internal static T Required<[Nullable(2)] T>([Nullable(2)] [NotNull] this T value, [CallerArgumentExpression("value")] string name = null)
		{
			if (value == null)
			{
				return Extensions.ThrowArgumentNullException<T>(name);
			}
			return value;
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AssertTrue(this bool value, [CallerArgumentExpression("value")] string name = null)
		{
			if (!value)
			{
				Extensions.ThrowAssertionFailed(name);
			}
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowAssertionFailed(string name)
		{
			throw new ArgumentException((name ?? "<unknown>") + " assertion failed");
		}

		[NullableContext(1)]
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static T ThrowArgumentNullException<[Nullable(2)] T>(string name)
		{
			throw new ArgumentNullException(name);
		}

		internal static void Validate<T>([Nullable(new byte[]
		{
			2,
			1
		})] this T[] buffer, int offset, int length, bool allowNullIfEmpty = false)
		{
			if (allowNullIfEmpty && buffer == null && offset == 0 && length == 0)
			{
				return;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "cannot be null");
			}
			if (offset < 0 || length < 0 || offset + length > buffer.Length)
			{
				throw new ArgumentException(string.Format("invalid offset/length combination: {0}/{1}", offset, length));
			}
		}
	}
}
