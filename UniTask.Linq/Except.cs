using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Except<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Except(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			this.first = first;
			this.second = second;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Except<TSource>._Except(this.first, this.second, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> first;

		private readonly IUniTaskAsyncEnumerable<TSource> second;

		private readonly IEqualityComparer<TSource> comparer;

		private class _Except : AsyncEnumeratorBase<TSource, TSource>
		{
			public _Except(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken) : base(first, cancellationToken)
			{
				this.second = second;
				this.comparer = comparer;
			}

			protected override bool OnFirstIteration()
			{
				if (this.set != null)
				{
					return false;
				}
				this.awaiter = this.second.ToHashSetAsync(this.cancellationToken).GetAwaiter();
				if (this.awaiter.IsCompleted)
				{
					this.set = this.awaiter.GetResult();
					base.SourceMoveNext();
				}
				else
				{
					this.awaiter.SourceOnCompleted(Except<TSource>._Except.HashSetAsyncCoreDelegate, this);
				}
				return true;
			}

			private static void HashSetAsyncCore(object state)
			{
				Except<TSource>._Except except = (Except<TSource>._Except)state;
				HashSet<TSource> hashSet;
				if (except.TryGetResult<HashSet<TSource>>(except.awaiter, out hashSet))
				{
					except.set = hashSet;
					except.SourceMoveNext();
				}
			}

			protected override bool TryMoveNextCore(bool sourceHasCurrent, out bool result)
			{
				if (!sourceHasCurrent)
				{
					result = false;
					return true;
				}
				TSource sourceCurrent = base.SourceCurrent;
				if (this.set.Add(sourceCurrent))
				{
					base.Current = sourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private static Action<object> HashSetAsyncCoreDelegate = new Action<object>(Except<TSource>._Except.HashSetAsyncCore);

			private readonly IEqualityComparer<TSource> comparer;

			private readonly IUniTaskAsyncEnumerable<TSource> second;

			private HashSet<TSource> set;

			private UniTask<HashSet<TSource>>.Awaiter awaiter;
		}
	}
}
