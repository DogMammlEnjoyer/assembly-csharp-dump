using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Return<TValue> : IUniTaskAsyncEnumerable<TValue>
	{
		public Return(TValue value)
		{
			this.value = value;
		}

		public IUniTaskAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Return<TValue>._Return(this.value, cancellationToken);
		}

		private readonly TValue value;

		private class _Return : IUniTaskAsyncEnumerator<TValue>, IUniTaskAsyncDisposable
		{
			public _Return(TValue value, CancellationToken cancellationToken)
			{
				this.value = value;
				this.cancellationToken = cancellationToken;
				this.called = false;
			}

			public TValue Current
			{
				get
				{
					return this.value;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (!this.called)
				{
					this.called = true;
					return CompletedTasks.True;
				}
				return CompletedTasks.False;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly TValue value;

			private CancellationToken cancellationToken;

			private bool called;
		}
	}
}
