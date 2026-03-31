using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Cast<TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public Cast(IUniTaskAsyncEnumerable<object> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Cast<TResult>._Cast(this.source, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<object> source;

		private class _Cast : AsyncEnumeratorBase<object, TResult>
		{
			public _Cast(IUniTaskAsyncEnumerable<object> source, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (sourceHasCurrent)
				{
					base.Current = (TResult)((object)base.SourceCurrent);
					result = true;
					return true;
				}
				result = false;
				return true;
			}
		}
	}
}
