using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class SkipWhileInt<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public SkipWhileInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate)
		{
			this.source = source;
			this.predicate = predicate;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new SkipWhileInt<TSource>._SkipWhileInt(this.source, this.predicate, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Func<TSource, int, bool> predicate;

		private class _SkipWhileInt : AsyncEnumeratorBase<TSource, TSource>
		{
			public _SkipWhileInt(IUniTaskAsyncEnumerable<TSource> source, Func<TSource, int, bool> predicate, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.predicate = predicate;
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (sourceHasCurrent)
				{
					if (this.predicate != null)
					{
						Func<TSource, int, bool> func = this.predicate;
						TSource sourceCurrent = base.SourceCurrent;
						int num = this.index;
						this.index = checked(num + 1);
						if (func(sourceCurrent, num))
						{
							result = false;
							return false;
						}
					}
					this.predicate = null;
					base.Current = base.SourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return true;
			}

			private Func<TSource, int, bool> predicate;

			private int index;
		}
	}
}
