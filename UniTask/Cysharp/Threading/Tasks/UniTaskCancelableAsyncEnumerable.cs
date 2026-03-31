using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	[StructLayout(LayoutKind.Auto)]
	public readonly struct UniTaskCancelableAsyncEnumerable<T>
	{
		internal UniTaskCancelableAsyncEnumerable(IUniTaskAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
		{
			this.enumerable = enumerable;
			this.cancellationToken = cancellationToken;
		}

		public UniTaskCancelableAsyncEnumerable<T>.Enumerator GetAsyncEnumerator()
		{
			return new UniTaskCancelableAsyncEnumerable<T>.Enumerator(this.enumerable.GetAsyncEnumerator(this.cancellationToken));
		}

		private readonly IUniTaskAsyncEnumerable<T> enumerable;

		private readonly CancellationToken cancellationToken;

		[StructLayout(LayoutKind.Auto)]
		public readonly struct Enumerator
		{
			internal Enumerator(IUniTaskAsyncEnumerator<T> enumerator)
			{
				this.enumerator = enumerator;
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
				return this.enumerator.MoveNextAsync();
			}

			public UniTask DisposeAsync()
			{
				return this.enumerator.DisposeAsync();
			}

			private readonly IUniTaskAsyncEnumerator<T> enumerator;
		}
	}
}
