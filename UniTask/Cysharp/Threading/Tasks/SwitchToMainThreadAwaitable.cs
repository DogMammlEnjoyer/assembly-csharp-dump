using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct SwitchToMainThreadAwaitable
	{
		public SwitchToMainThreadAwaitable(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
		{
			this.playerLoopTiming = playerLoopTiming;
			this.cancellationToken = cancellationToken;
		}

		public SwitchToMainThreadAwaitable.Awaiter GetAwaiter()
		{
			return new SwitchToMainThreadAwaitable.Awaiter(this.playerLoopTiming, this.cancellationToken);
		}

		private readonly PlayerLoopTiming playerLoopTiming;

		private readonly CancellationToken cancellationToken;

		public struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
			{
				this.playerLoopTiming = playerLoopTiming;
				this.cancellationToken = cancellationToken;
			}

			public bool IsCompleted
			{
				get
				{
					int managedThreadId = Thread.CurrentThread.ManagedThreadId;
					return PlayerLoopHelper.MainThreadId == managedThreadId;
				}
			}

			public void GetResult()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
			}

			public void OnCompleted(Action continuation)
			{
				PlayerLoopHelper.AddContinuation(this.playerLoopTiming, continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				PlayerLoopHelper.AddContinuation(this.playerLoopTiming, continuation);
			}

			private readonly PlayerLoopTiming playerLoopTiming;

			private readonly CancellationToken cancellationToken;
		}
	}
}
