using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public readonly struct YieldAwaitable
	{
		public YieldAwaitable(PlayerLoopTiming timing)
		{
			this.timing = timing;
		}

		public YieldAwaitable.Awaiter GetAwaiter()
		{
			return new YieldAwaitable.Awaiter(this.timing);
		}

		public UniTask ToUniTask()
		{
			return UniTask.Yield(this.timing, CancellationToken.None);
		}

		private readonly PlayerLoopTiming timing;

		public readonly struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(PlayerLoopTiming timing)
			{
				this.timing = timing;
			}

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
				PlayerLoopHelper.AddContinuation(this.timing, continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				PlayerLoopHelper.AddContinuation(this.timing, continuation);
			}

			private readonly PlayerLoopTiming timing;
		}
	}
}
