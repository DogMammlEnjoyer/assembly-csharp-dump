using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class ToUniTaskAsyncEnumerable<T> : IUniTaskAsyncEnumerable<T>
	{
		public ToUniTaskAsyncEnumerable(IEnumerable<T> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ToUniTaskAsyncEnumerable<T>._ToUniTaskAsyncEnumerable(this.source, cancellationToken);
		}

		private readonly IEnumerable<T> source;

		private class _ToUniTaskAsyncEnumerable : IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public _ToUniTaskAsyncEnumerable(IEnumerable<T> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
			}

			public T Current
			{
				get
				{
					return this.enumerator.Current;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				if (this.enumerator == null)
				{
					this.enumerator = this.source.GetEnumerator();
				}
				if (this.enumerator.MoveNext())
				{
					return CompletedTasks.True;
				}
				return CompletedTasks.False;
			}

			public UniTask DisposeAsync()
			{
				this.enumerator.Dispose();
				return default(UniTask);
			}

			private readonly IEnumerable<T> source;

			private CancellationToken cancellationToken;

			private IEnumerator<T> enumerator;
		}
	}
}
