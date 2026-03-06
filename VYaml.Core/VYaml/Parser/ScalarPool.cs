using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace VYaml.Parser
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ScalarPool
	{
		public Scalar Rent()
		{
			Scalar result;
			if (this.queue.TryDequeue(out result))
			{
				return result;
			}
			return new Scalar(256);
		}

		public void Return(Scalar scalar)
		{
			scalar.Clear();
			this.queue.Enqueue(scalar);
		}

		public static readonly ScalarPool Shared = new ScalarPool();

		private readonly ConcurrentQueue<Scalar> queue = new ConcurrentQueue<Scalar>();
	}
}
