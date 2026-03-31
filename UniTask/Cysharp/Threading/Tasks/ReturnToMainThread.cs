using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct ReturnToMainThread
	{
		public ReturnToMainThread(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
		{
			this.playerLoopTiming = playerLoopTiming;
			this.cancellationToken = cancellationToken;
		}

		public ReturnToMainThread.Awaiter DisposeAsync()
		{
			return new ReturnToMainThread.Awaiter(this.playerLoopTiming, this.cancellationToken);
		}

		private readonly PlayerLoopTiming playerLoopTiming;

		private readonly CancellationToken cancellationToken;

		public readonly struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(PlayerLoopTiming timing, CancellationToken cancellationToken)
			{
				this.timing = timing;
				this.cancellationToken = cancellationToken;
			}

			public ReturnToMainThread.Awaiter GetAwaiter()
			{
				return this;
			}

			public bool IsCompleted
			{
				get
				{
					return PlayerLoopHelper.MainThreadId == Thread.CurrentThread.ManagedThreadId;
				}
			}

			public void GetResult()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
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

			private readonly CancellationToken cancellationToken;
		}
	}
}
