using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipWhileIntAwait<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipWhileIntAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<bool>> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipWhileIntAwait<TSource>._SkipWhileIntAwait(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, UniTask<bool>> predicate;

		private class _SkipWhileIntAwait : AsyncEnumeratorAwaitSelectorBase<TSource, TSource, bool>
		{
			public _SkipWhileIntAwait(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, UniTask<bool>> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override UniTask<bool> TransformAsync(TSource sourceCurrent)
			{
				if (this.predicate == null)
				{
					return CompletedTasks.False;
				}
				Func<TSource, int, UniTask<bool>> func = this.predicate;
				int num = this.index;
				this.index = checked(num + 1);
				return func(sourceCurrent, num);
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

			private Func<TSource, int, UniTask<bool>> predicate;

			private int index;
		}
	}
}
