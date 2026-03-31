using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipWhileAwaitWithCancellation<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipWhileAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipWhileAwaitWithCancellation<TSource>._SkipWhileAwaitWithCancellation(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, CancellationToken, UniTask<bool>> predicate;

		private class _SkipWhileAwaitWithCancellation : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _SkipWhileAwaitWithCancellation(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, CancellationToken, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				if (this.predicate == null)
				{
					return CompletedTasks.False;
				}
				return this.predicate(sourceCurrent, this.cancellationToken);
			}

			protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
			{
				terminateIteration = false;
				if (!awaitResult)
				{
					this.predicate = null;
					base.Current = base.SourceCurrent;
					return true;
				}
				return false;
			}

			private Func<TSource, CancellationToken, UniTask<bool>> predicate;
		}
	}
}
