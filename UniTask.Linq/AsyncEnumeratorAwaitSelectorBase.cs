using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal abstract class AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait> : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
	{
		public AsyncEnumeratorAwaitSelectorBase(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			this.source = source;
			this.cancellationToken = cancellationToken;
		}

		protected abstract UniTask<TAwait> TransformAsync(TSource sourceCurrent);

		protected abstract bool TrySetCurrentCore(TAwait awaitResult, out bool terminateIteration);

		private protected TSource SourceCurrent { protected get; private set; }

		[return: TupleElementNames(new string[]
		{
			"waitCallback",
			"requireNextIteration"
		})]
		protected ValueTuple<bool, bool> ActionCompleted(bool trySetCurrentResult, out bool moveNextResult)
		{
			if (trySetCurrentResult)
			{
				moveNextResult = true;
				return new ValueTuple<bool, bool>(false, false);
			}
			moveNextResult = false;
			return new ValueTuple<bool, bool>(false, true);
		}

		[return: TupleElementNames(new string[]
		{
			"waitCallback",
			"requireNextIteration"
		})]
		protected ValueTuple<bool, bool> WaitAwaitCallback(out bool moveNextResult)
		{
			moveNextResult = false;
			return new ValueTuple<bool, bool>(true, false);
		}

		[return: TupleElementNames(new string[]
		{
			"waitCallback",
			"requireNextIteration"
		})]
		protected ValueTuple<bool, bool> IterateFinished(out bool moveNextResult)
		{
			moveNextResult = false;
			return new ValueTuple<bool, bool>(false, false);
		}

		public TResult Current { get; protected set; }

		public UniTask<bool> MoveNextAsync()
		{
			if (this.enumerator == null)
			{
				this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
			}
			this.completionSource.Reset();
			this.SourceMoveNext();
			return new UniTask<bool>(this, this.completionSource.Version);
		}

		protected void SourceMoveNext()
		{
			for (;;)
			{
				this.sourceMoveNext = this.enumerator.MoveNextAsync().GetAwaiter();
				if (this.sourceMoveNext.IsCompleted)
				{
					bool result = false;
					try
					{
						ValueTuple<bool, bool> valueTuple = this.TryMoveNextCore(this.sourceMoveNext.GetResult(), out result);
						bool item = valueTuple.Item1;
						bool item2 = valueTuple.Item2;
						if (item)
						{
							return;
						}
						if (item2)
						{
							continue;
						}
						this.completionSource.TrySetResult(result);
						return;
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
						return;
					}
					break;
				}
				break;
			}
			this.sourceMoveNext.SourceOnCompleted(AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>.moveNextCallbackDelegate, this);
		}

		[return: TupleElementNames(new string[]
		{
			"waitCallback",
			"requireNextIteration"
		})]
		private ValueTuple<bool, bool> TryMoveNextCore(bool sourceHasCurrent, out bool result)
		{
			if (!sourceHasCurrent)
			{
				return this.IterateFinished(out result);
			}
			this.SourceCurrent = this.enumerator.Current;
			UniTask<TAwait> taskResult = this.TransformAsync(this.SourceCurrent);
			TAwait awaitResult;
			if (!this.UnwarapTask(taskResult, out awaitResult))
			{
				return this.WaitAwaitCallback(out result);
			}
			bool flag;
			bool trySetCurrentResult = this.TrySetCurrentCore(awaitResult, out flag);
			if (flag)
			{
				return this.IterateFinished(out result);
			}
			return this.ActionCompleted(trySetCurrentResult, out result);
		}

		protected bool UnwarapTask(UniTask<TAwait> taskResult, out TAwait result)
		{
			this.resultAwaiter = taskResult.GetAwaiter();
			if (this.resultAwaiter.IsCompleted)
			{
				result = this.resultAwaiter.GetResult();
				return true;
			}
			this.resultAwaiter.SourceOnCompleted(AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>.setCurrentCallbackDelegate, this);
			result = default(TAwait);
			return false;
		}

		private static void MoveNextCallBack(object state)
		{
			AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait> asyncEnumeratorAwaitSelectorBase = (AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>)state;
			bool result = false;
			try
			{
				ValueTuple<bool, bool> valueTuple = asyncEnumeratorAwaitSelectorBase.TryMoveNextCore(asyncEnumeratorAwaitSelectorBase.sourceMoveNext.GetResult(), out result);
				bool item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				if (!item)
				{
					if (item2)
					{
						asyncEnumeratorAwaitSelectorBase.SourceMoveNext();
					}
					else
					{
						asyncEnumeratorAwaitSelectorBase.completionSource.TrySetResult(result);
					}
				}
			}
			catch (Exception error)
			{
				asyncEnumeratorAwaitSelectorBase.completionSource.TrySetException(error);
			}
		}

		private static void SetCurrentCallBack(object state)
		{
			AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait> asyncEnumeratorAwaitSelectorBase = (AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>)state;
			bool flag2;
			bool flag;
			try
			{
				TAwait result = asyncEnumeratorAwaitSelectorBase.resultAwaiter.GetResult();
				flag = asyncEnumeratorAwaitSelectorBase.TrySetCurrentCore(result, out flag2);
			}
			catch (Exception error)
			{
				asyncEnumeratorAwaitSelectorBase.completionSource.TrySetException(error);
				return;
			}
			if (asyncEnumeratorAwaitSelectorBase.cancellationToken.IsCancellationRequested)
			{
				asyncEnumeratorAwaitSelectorBase.completionSource.TrySetCanceled(asyncEnumeratorAwaitSelectorBase.cancellationToken);
				return;
			}
			if (flag)
			{
				asyncEnumeratorAwaitSelectorBase.completionSource.TrySetResult(true);
				return;
			}
			if (flag2)
			{
				asyncEnumeratorAwaitSelectorBase.completionSource.TrySetResult(false);
				return;
			}
			asyncEnumeratorAwaitSelectorBase.SourceMoveNext();
		}

		public virtual UniTask DisposeAsync()
		{
			if (this.enumerator != null)
			{
				return this.enumerator.DisposeAsync();
			}
			return default(UniTask);
		}

		private static readonly Action<object> moveNextCallbackDelegate = new Action<object>(AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>.MoveNextCallBack);

		private static readonly Action<object> setCurrentCallbackDelegate = new Action<object>(AsyncEnumeratorAwaitSelectorBase<TSource, TResult, TAwait>.SetCurrentCallBack);

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		protected CancellationToken cancellationToken;

		private IUniTaskAsyncEnumerator<TSource> enumerator;

		private UniTask<bool>.Awaiter sourceMoveNext;

		private UniTask<TAwait>.Awaiter resultAwaiter;
	}
}
