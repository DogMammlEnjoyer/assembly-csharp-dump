using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Repeat<TElement> : IUniTaskAsyncEnumerable<TElement>
	{
		public Repeat(TElement element, int count)
		{
			this.element = element;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Repeat<TElement>._Repeat(this.element, this.count, cancellationToken);
		}

		private readonly TElement element;

		private readonly int count;

		private class _Repeat : IUniTaskAsyncEnumerator<TElement>, IUniTaskAsyncDisposable
		{
			public _Repeat(TElement element, int count, CancellationToken cancellationToken)
			{
				this.element = element;
				this.count = count;
				this.cancellationToken = cancellationToken;
				this.remaining = count;
			}

			public TElement Current
			{
				get
				{
					return this.element;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				int num = this.remaining;
				this.remaining = num - 1;
				if (num != 0)
				{
					return CompletedTasks.True;
				}
				return CompletedTasks.False;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly TElement element;

			private readonly int count;

			private int remaining;

			private CancellationToken cancellationToken;
		}
	}
}
