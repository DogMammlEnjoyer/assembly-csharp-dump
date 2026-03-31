using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeWhileIntAwaitWithCancellation<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeWhileIntAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeWhileIntAwaitWithCancellation<TSource>._TakeWhileIntAwaitWithCancellation(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, CancellationToken, UniTask<bool>> predicate;

		private class _TakeWhileIntAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _TakeWhileIntAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				Func<TSource, int, CancellationToken, UniTask<bool>> func = this.predicate;
				int num = this.index;
				this.index = checked(num + 1);
				return func(sourceCurrent, num, this.cancellationToken);
			}

			protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
			{
				if (awaitResult)
				{
					base.Current = base.SourceCurrent;
					terminateIteration = false;
					return true;
				}
				terminateIteration = true;
				return false;
			}

			private readonly Func<TSource, int, CancellationToken, UniTask<bool>> predicate;

			private int index;
		}
	}
}
