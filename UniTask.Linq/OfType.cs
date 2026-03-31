using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class OfType<TResult> : IUniTaskAsyncEnumerable<TResult>
	{
		public OfType(IUniTaskAsyncEnumerable<object> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new OfType<TResult>._OfType(this.source, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<object> source;

		private class _OfType : AsyncEnumeratorBase<object, TResult>
		{
			public _OfType(IUniTaskAsyncEnumerable<object> source, CancellationToken cancellationToken) : base(source, cancellationToken)
			{
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				object sourceCurrent = base.SourceCurrent;
				if (sourceCurrent is TResult)
				{
					TResult value = (TResult)((object)sourceCurrent);
					base.Current = value;
					result = true;
					return true;
				}
				result = false;
				return false;
			}
		}
	}
}
