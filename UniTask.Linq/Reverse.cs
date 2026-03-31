using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Reverse<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Reverse(IUniTaskAsyncEnumerable<TSource> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Reverse<TSource>._Reverse(this.source, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private sealed class _Reverse : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Reverse(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				Reverse<TSource>._Reverse.<MoveNextAsync>d__9 <MoveNextAsync>d__;
				<MoveNextAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
				<MoveNextAsync>d__.<>4__this = this;
				<MoveNextAsync>d__.<>1__state = -1;
				<MoveNextAsync>d__.<>t__builder.Start<Reverse<TSource>._Reverse.<MoveNextAsync>d__9>(ref <MoveNextAsync>d__);
				return <MoveNextAsync>d__.<>t__builder.Task;
			}

			public UniTask DisposeAsync()
			{
				return default(UniTask);
			}

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private CancellationToken cancellationToken;

			private TSource[] array;

			private int index;
		}
	}
}
