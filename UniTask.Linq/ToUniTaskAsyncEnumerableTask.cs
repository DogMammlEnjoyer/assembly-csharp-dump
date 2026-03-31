using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class ToUniTaskAsyncEnumerableTask<T> : IUniTaskAsyncEnumerable<T>
	{
		public ToUniTaskAsyncEnumerableTask(Task<T> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ToUniTaskAsyncEnumerableTask<T>._ToUniTaskAsyncEnumerableTask(this.source, cancellationToken);
		}

		private readonly Task<T> source;

		private class _ToUniTaskAsyncEnumerableTask : IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public _ToUniTaskAsyncEnumerableTask(Task<T> source, CancellationToken cancellationToken)
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
				ToUniTaskAsyncEnumerableTask<T>._ToUniTaskAsyncEnumerableTask.<MoveNextAsync>d__7 <MoveNextAsync>d__;
				<MoveNextAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
				<MoveNextAsync>d__.<>4__this = this;
				<MoveNextAsync>d__.<>1__state = -1;
				<MoveNextAsync>d__.<>t__builder.Start<ToUniTaskAsyncEnumerableTask<T>._ToUniTaskAsyncEnumerableTask.<MoveNextAsync>d__7>(ref <MoveNextAsync>d__);
				return <MoveNextAsync>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly Task<T> source;

			private CancellationToken cancellationToken;

			private T current;

			private bool called;
		}
	}
}
