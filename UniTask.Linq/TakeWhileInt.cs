using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class TakeWhileInt<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public TakeWhileInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new TakeWhileInt<TSource>._TakeWhileInt(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, bool> predicate;

		private class _TakeWhileInt : AsyncEnumeratorBase<TSource, TSource>
		{
			public _TakeWhileInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (sourceHasCurrent)
				{
					Func<TSource, int, bool> func = this.predicate;
					TSource sourceCurrent = base.SourceCurrent;
					int num = this.index;
					this.index = checked(num + 1);
					if (func(sourceCurrent, num))
					{
						base.Current = base.SourceCurrent;
						result = true;
						return true;
					}
				}
				result = false;
				return true;
			}

			private readonly Func<TSource, int, bool> predicate;

			private int index;
		}
	}
}
