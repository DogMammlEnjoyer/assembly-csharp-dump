using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
	[VisibleToOtherModules]
	internal readonly ref struct ManagedSpanWrapper
	{
		public unsafe ManagedSpanWrapper(void* begin, int length)
		{
			this.begin = begin;
			this.length = length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> ToSpan<T>(ManagedSpanWrapper spanWrapper)
		{
			return new Span<T>(spanWrapper.begin, spanWrapper.length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> ToReadOnlySpan<T>(ManagedSpanWrapper spanWrapper)
		{
			return new ReadOnlySpan<T>(spanWrapper.begin, spanWrapper.length);
		}

		public unsafe readonly void* begin;

		public readonly int length;
	}
}
