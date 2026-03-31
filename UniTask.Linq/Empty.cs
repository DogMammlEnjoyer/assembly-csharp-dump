using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class Empty<T> : IUniTaskAsyncEnumerable<T>
	{
		private Empty()
		{
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Empty<T>._Empty.Instance;
		}

		public static readonly IUniTaskAsyncEnumerable<T> Instance = new Empty<T>();

		private class _Empty : IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			private _Empty()
			{
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
				return CompletedTasks.False;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			public static readonly IUniTaskAsyncEnumerator<T> Instance = new Empty<T>._Empty();
		}
	}
}
