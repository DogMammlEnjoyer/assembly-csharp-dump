using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Intersect<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Intersect(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
		{
			this.first = first;
			this.second = second;
			this.comparer = comparer;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Intersect<TSource>._Intersect(this.first, this.second, this.comparer, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> first;

		private readonly IUniTaskAsyncEnumerable<TSource> second;

		private readonly IEqualityComparer<TSource> comparer;

		private class _Intersect : AsyncEnumeratorBase<TSource, TSource>
		{
			public _Intersect(IUniTaskAsyncEnumerable<TSource> first, IUniTaskAsyncEnumerable<TSource> second, IEqualityComparer<TSource> comparer, CancellationToken cancellationToken) : base(first, cancellationToken)
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
					this.awaiter.SourceOnCompleted(Intersect<TSource>._Intersect.HashSetAsyncCoreDelegate, this);
				}
				return true;
			}

			private static void HashSetAsyncCore(object state)
			{
				Intersect<TSource>._Intersect intersect = (Intersect<TSource>._Intersect)state;
				HashSet<TSource> hashSet;
				if (intersect.TryGetResult<HashSet<TSource>>(intersect.awaiter, out hashSet))
				{
					intersect.set = hashSet;
					intersect.SourceMoveNext();
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
				if (this.set.Remove(sourceCurrent))
				{
					base.Current = sourceCurrent;
					result = true;
					return true;
				}
				result = false;
				return false;
			}

			private static Action<object> HashSetAsyncCoreDelegate = new Action<object>(Intersect<TSource>._Intersect.HashSetAsyncCore);

			private readonly IEqualityComparer<TSource> comparer;

			private readonly IUniTaskAsyncEnumerable<TSource> second;

			private HashSet<TSource> set;

			private UniTask<HashSet<TSource>>.Awaiter awaiter;
		}
	}
}
