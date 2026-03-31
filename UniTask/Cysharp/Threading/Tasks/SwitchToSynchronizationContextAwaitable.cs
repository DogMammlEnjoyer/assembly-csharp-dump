using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct SwitchToSynchronizationContextAwaitable
	{
		public SwitchToSynchronizationContextAwaitable(SynchronizationContext synchronizationContext, CancellationToken cancellationToken)
		{
			this.synchronizationContext = synchronizationContext;
			this.cancellationToken = cancellationToken;
		}

		public SwitchToSynchronizationContextAwaitable.Awaiter GetAwaiter()
		{
			return new SwitchToSynchronizationContextAwaitable.Awaiter(this.synchronizationContext, this.cancellationToken);
		}

		private readonly SynchronizationContext synchronizationContext;

		private readonly CancellationToken cancellationToken;

		public struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(SynchronizationContext synchronizationContext, CancellationToken cancellationToken)
			{
				this.synchronizationContext = synchronizationContext;
				this.cancellationToken = cancellationToken;
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
				this.cancellationToken.ThrowIfCancellationRequested();
			}

			public void OnCompleted(Action continuation)
			{
				this.synchronizationContext.Post(SwitchToSynchronizationContextAwaitable.Awaiter.switchToCallback, continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				this.synchronizationContext.Post(SwitchToSynchronizationContextAwaitable.Awaiter.switchToCallback, continuation);
			}

			private static void Callback(object state)
			{
				((Action)state)();
			}

			private static readonly SendOrPostCallback switchToCallback = new SendOrPostCallback(SwitchToSynchronizationContextAwaitable.Awaiter.Callback);

			private readonly SynchronizationContext synchronizationContext;

			private readonly CancellationToken cancellationToken;
		}
	}
}
