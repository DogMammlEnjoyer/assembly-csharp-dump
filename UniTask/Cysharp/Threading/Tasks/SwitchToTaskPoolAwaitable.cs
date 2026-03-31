using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cysharp.Threading.Tasks
{
	public struct SwitchToTaskPoolAwaitable
	{
		public SwitchToTaskPoolAwaitable.Awaiter GetAwaiter()
		{
			return default(SwitchToTaskPoolAwaitable.Awaiter);
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
				Task.Factory.StartNew(SwitchToTaskPoolAwaitable.Awaiter.switchToCallback, continuation, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				Task.Factory.StartNew(SwitchToTaskPoolAwaitable.Awaiter.switchToCallback, continuation, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}

			private static void Callback(object state)
			{
				((Action)state)();
			}

			private static readonly Action<object> switchToCallback = new Action<object>(SwitchToTaskPoolAwaitable.Awaiter.Callback);
		}
	}
}
