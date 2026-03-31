using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeWhileAwait<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeWhileAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeWhileAwait<TSource>._TakeWhileAwait(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<bool>> predicate;

		private class _TakeWhileAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _TakeWhileAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				return this.predicate(sourceCurrent);
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

			private Func<TSource, UniTask<bool>> predicate;
		}
	}
}
