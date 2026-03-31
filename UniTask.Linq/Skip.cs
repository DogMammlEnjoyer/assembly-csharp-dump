using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Skip<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Skip(IUniTaskAsyncEnumerable<TSource> source, int count)
		{
			this.source = source;
			this.count = count;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Skip<TSource>._Skip(this.source, this.count, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly int count;

		private sealed class _Skip : AsyncEnumeratorBase<TSource, TSource>
		{
			public _Skip(IUniTaskAsyncEnumerable<TSource> source, int count, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
				this.count = count;
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				int num = this.count;
				int num2 = this.index;
				this.index = checked(num2 + 1);
				if (num <= num2)
				{
					base.Current = base.SourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private readonly int count;

			private int index;
		}
	}
}
