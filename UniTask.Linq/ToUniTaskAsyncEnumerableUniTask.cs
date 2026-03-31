using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class ToUniTaskAsyncEnumerableUniTask<T> : IUniTaskAsyncEnumerable<T>
	{
		public ToUniTaskAsyncEnumerableUniTask(UniTask<T> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ToUniTaskAsyncEnumerableUniTask<T>._ToUniTaskAsyncEnumerableUniTask(this.source, cancellationToken);
		}

		private readonly UniTask<T> source;

		private class _ToUniTaskAsyncEnumerableUniTask : IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public _ToUniTaskAsyncEnumerableUniTask(UniTask<T> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
				this.called = false;
			}

			public T Current
			{
				get
				{
					return this.current;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				ToUniTaskAsyncEnumerableUniTask<T>._ToUniTaskAsyncEnumerableUniTask.<MoveNextAsync>d__7 <MoveNextAsync>d__;
				<MoveNextAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
				<MoveNextAsync>d__.<>4__this = this;
				<MoveNextAsync>d__.<>1__state = -1;
				<MoveNextAsync>d__.<>t__builder.Start<ToUniTaskAsyncEnumerableUniTask<T>._ToUniTaskAsyncEnumerableUniTask.<MoveNextAsync>d__7>(ref <MoveNextAsync>d__);
				return <MoveNextAsync>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly UniTask<T> source;

			private CancellationToken cancellationToken;

			private T current;

			private bool called;
		}
	}
}
