using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VYaml.Internal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ReusableByteSequenceBuilderPool
	{
		public static ReusableByteSequenceBuilder Rent()
		{
			ReusableByteSequenceBuilder result;
			if (ReusableByteSequenceBuilderPool.queue.TryDequeue(out result))
			{
				return result;
			}
			return new ReusableByteSequenceBuilder();
		}

		public static void Return(ReusableByteSequenceBuilder builder)
		{
			builder.Reset();
			ReusableByteSequenceBuilderPool.queue.Enqueue(builder);
		}

		private static readonly ConcurrentQueue<ReusableByteSequenceBuilder> queue = new ConcurrentQueue<ReusableByteSequenceBuilder>();
	}
}
