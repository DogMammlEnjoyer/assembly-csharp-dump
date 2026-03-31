using System;

namespace Cysharp.Threading.Tasks
{
	public static class Channel
	{
		public static Channel<T> CreateSingleConsumerUnbounded<T>()
		{
			return new SingleConsumerUnboundedChannel<T>();
		}
	}
}
