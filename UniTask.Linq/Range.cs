using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Range : IUniTaskAsyncEnumerable<int>
	{
		public Range(int start, int count)
		{
			this.start = start;
			this.end = start + count;
		}

		public IUniTaskAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Range._Range(this.start, this.end, cancellationToken);
		}

		private readonly int start;

		private readonly int end;

		private class _Range : IUniTaskAsyncEnumerator<int>, IUniTaskAsyncDisposable
		{
			public _Range(int start, int end, CancellationToken cancellationToken)
			{
				this.start = start;
				this.end = end;
				this.cancellationToken = cancellationToken;
				this.current = start - 1;
			}

			public int Current
			{
				get
				{
					return this.current;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.current++;
				if (this.current != this.end)
				{
					return CompletedTasks.True;
				}
				return CompletedTasks.False;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly int start;

			private readonly int end;

			private int current;

			private CancellationToken cancellationToken;
		}
	}
}
