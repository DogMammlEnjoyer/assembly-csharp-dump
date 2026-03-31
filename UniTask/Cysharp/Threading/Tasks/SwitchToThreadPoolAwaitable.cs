using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct SwitchToThreadPoolAwaitable
	{
		public SwitchToThreadPoolAwaitable.Awaiter GetAwaiter()
		{
			return default(SwitchToThreadPoolAwaitable.Awaiter);
		}

		public struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			public void GetResult()
			{
			}

			public void OnCompleted(Action continuation)
			{
				ThreadPool.QueueUserWorkItem(SwitchToThreadPoolAwaitable.Awaiter.switchToCallback, continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				ThreadPool.UnsafeQueueUserWorkItem(SwitchToThreadPoolAwaitable.Awaiter.switchToCallback, continuation);
			}

			private static void Callback(object state)
			{
				((Action)state)();
			}

			private static readonly WaitCallback switchToCallback = new WaitCallback(SwitchToThreadPoolAwaitable.Awaiter.Callback);
		}
	}
}
