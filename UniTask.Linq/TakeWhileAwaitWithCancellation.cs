using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeWhileAwaitWithCancellation<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeWhileAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeWhileAwaitWithCancellation<TSource>._TakeWhileAwaitWithCancellation(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, CancellationToken, UniTask<bool>> predicate;

		private class _TakeWhileAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _TakeWhileAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				return this.predicate(sourceCurrent, this.cancellationToken);
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

			private Func<TSource, CancellationToken, UniTask<bool>> predicate;
		}
	}
}
