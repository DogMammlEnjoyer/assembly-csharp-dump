using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class EveryUpdate : IUniTaskAsyncEnumerable<AsyncUnit>
	{
		public EveryUpdate(PlayerLoopTiming updateTiming)
		{
			this.updateTiming = updateTiming;
		}

		public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new EveryUpdate._EveryUpdate(this.updateTiming, cancellationToken);
		}

		private readonly PlayerLoopTiming updateTiming;

		private class _EveryUpdate : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IUniTaskAsyncDisposable, IPlayerLoopItem
		{
			public _EveryUpdate(PlayerLoopTiming updateTiming, CancellationToken cancellationToken)
			{
				this.updateTiming = updateTiming;
				this.cancellationToken = cancellationToken;
				PlayerLoopHelper.AddAction(updateTiming, this);
			}

			public AsyncUnit Current
			{
				get
				{
					return default(AsyncUnit);
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested)
				{
					return CompletedTasks.False;
				}
				this.completionSource.Reset();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			public UniTask DisposeAsync()
			{
				if (!this.disposed)
				{
					this.disposed = true;
				}
				return default(UniTask);
			}

			public bool MoveNext()
			{
				if (this.disposed || this.cancellationToken.IsCancellationRequested)
				{
					this.completionSource.TrySetResult(false);
					return false;
				}
				this.completionSource.TrySetResult(true);
				return true;
			}

			private readonly PlayerLoopTiming updateTiming;

			private CancellationToken cancellationToken;

			private bool disposed;
		}
	}
}
