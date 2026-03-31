using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Throw<TValue> : IUniTaskAsyncEnumerable<TValue>
	{
		public Throw(Exception exception)
		{
			this.exception = exception;
		}

		public IUniTaskAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Throw<TValue>._Throw(this.exception, cancellationToken);
		}

		private readonly Exception exception;

		private class _Throw : IUniTaskAsyncEnumerator<TValue>, IUniTaskAsyncDisposable
		{
			public _Throw(Exception exception, CancellationToken cancellationToken)
			{
				this.exception = exception;
				this.cancellationToken = cancellationToken;
			}

			public TValue Current
			{
				get
				{
					return default(TValue);
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				return UniTask.FromException<bool>(this.exception);
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly Exception exception;

			private CancellationToken cancellationToken;
		}
	}
}
