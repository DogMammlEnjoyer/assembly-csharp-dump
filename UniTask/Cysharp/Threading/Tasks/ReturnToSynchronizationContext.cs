using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public struct ReturnToSynchronizationContext
	{
		public ReturnToSynchronizationContext(SynchronizationContext syncContext, bool dontPostWhenSameContext, CancellationToken cancellationToken)
		{
			this.syncContext = syncContext;
			this.dontPostWhenSameContext = dontPostWhenSameContext;
			this.cancellationToken = cancellationToken;
		}

		public ReturnToSynchronizationContext.Awaiter DisposeAsync()
		{
			return new ReturnToSynchronizationContext.Awaiter(this.syncContext, this.dontPostWhenSameContext, this.cancellationToken);
		}

		private readonly SynchronizationContext syncContext;

		private readonly bool dontPostWhenSameContext;

		private readonly CancellationToken cancellationToken;

		public struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public Awaiter(SynchronizationContext synchronizationContext, bool dontPostWhenSameContext, CancellationToken cancellationToken)
			{
				this.synchronizationContext = synchronizationContext;
				this.dontPostWhenSameContext = dontPostWhenSameContext;
				this.cancellationToken = cancellationToken;
			}

			public ReturnToSynchronizationContext.Awaiter GetAwaiter()
			{
				return this;
			}

			public bool IsCompleted
			{
				get
				{
					return this.dontPostWhenSameContext && SynchronizationContext.Current == this.synchronizationContext;
				}
			}

			public void GetResult()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
			}

			public void OnCompleted(Action continuation)
			{
				this.synchronizationContext.Post(ReturnToSynchronizationContext.Awaiter.switchToCallback, continuation);
			}

			public void UnsafeOnCompleted(Action continuation)
			{
				this.synchronizationContext.Post(ReturnToSynchronizationContext.Awaiter.switchToCallback, continuation);
			}

			private static void Callback(object state)
			{
				((Action)state)();
			}

			private static readonly SendOrPostCallback switchToCallback = new SendOrPostCallback(ReturnToSynchronizationContext.Awaiter.Callback);

			private readonly SynchronizationContext synchronizationContext;

			private readonly bool dontPostWhenSameContext;

			private readonly CancellationToken cancellationToken;
		}
	}
}
