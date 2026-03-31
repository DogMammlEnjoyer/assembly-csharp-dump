using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Never<T> : IUniTaskAsyncEnumerable<T>
	{
		private Never()
		{
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Never<T>._Never(cancellationToken);
		}

		public static readonly IUniTaskAsyncEnumerable<T> Instance = new Never<T>();

		private class _Never : IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public _Never(CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;
			}

			public T Current
			{
				get
				{
					return default(T);
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				UniTaskCompletionSource<bool> uniTaskCompletionSource = new UniTaskCompletionSource<bool>();
				this.cancellationToken.Register(delegate(object state)
				{
					((UniTaskCompletionSource<bool>)state).TrySetCanceled(this.cancellationToken);
				}, uniTaskCompletionSource);
				return uniTaskCompletionSource.Task;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private CancellationToken cancellationToken;
		}
	}
}
