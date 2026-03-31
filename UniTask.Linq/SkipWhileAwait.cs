using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipWhileAwait<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipWhileAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipWhileAwait<TSource>._SkipWhileAwait(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, UniTask<bool>> predicate;

		private class _SkipWhileAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _SkipWhileAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				if (this.predicate == null)
				{
					return CompletedTasks.False;
				}
				return this.predicate(sourceCurrent);
			}

			protected override bool TrySetCurrentCore(bool awaitResult, out bool terminateIteration)
			{
				if (!awaitResult)
				{
					this.predicate = null;
					base.Current = base.SourceCurrent;
					terminateIteration = false;
					return true;
				}
				terminateIteration = false;
				return false;
			}

			private Func<TSource, UniTask<bool>> predicate;
		}
	}
}
