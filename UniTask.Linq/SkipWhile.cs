using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipWhile<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipWhile(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipWhile<TSource>._SkipWhile(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, bool> predicate;

		private class _SkipWhile : AsyncEnumeratorBase<TSource, TSource>
		{
			public _SkipWhile(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, bool> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				if (this.predicate == null || !this.predicate(base.SourceCurrent))
				{
					this.predicate = null;
					base.Current = base.SourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private Func<TSource, bool> predicate;
		}
	}
}
