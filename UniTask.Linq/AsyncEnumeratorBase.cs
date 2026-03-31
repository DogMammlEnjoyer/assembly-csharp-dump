using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal abstract class AsyncEnumeratorBase<TSource, TResult> : MoveNextSource, IUniTaskAsyncEnumerator<TResult>, IUniTaskAsyncDisposable
	{
		public AsyncEnumeratorBase(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			this.source = source;
			this.cancellationToken = cancellationToken;
		}

		protected abstract bool TryMoveNextCore(bool sourceHasCurrent, out bool result);

		protected TSource SourceCurrent
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		public TResult Current { get; protected set; }

		public UniTask<bool> MoveNextAsync()
		{
			if (this.enumerator == null)
			{
				this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
			}
			this.completionSource.Reset();
			if (!this.OnFirstIteration())
			{
				this.SourceMoveNext();
			}
			return new UniTask<bool>(this, this.completionSource.Version);
		}

		protected virtual bool OnFirstIteration()
		{
			return false;
		}

		protected void SourceMoveNext()
		{
			bool result;
			for (;;)
			{
				this.sourceMoveNext = this.enumerator.MoveNextAsync().GetAwaiter();
				if (this.sourceMoveNext.IsCompleted)
				{
					result = false;
					try
					{
						if (!this.TryMoveNextCore(this.sourceMoveNext.GetResult(), out result))
						{
							continue;
						}
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
						return;
					}
					break;
				}
				goto IL_7F;
			}
			if (this.cancellationToken.IsCancellationRequested)
			{
				this.completionSource.TrySetCanceled(this.cancellationToken);
				return;
			}
			this.completionSource.TrySetResult(result);
			return;
			IL_7F:
			this.sourceMoveNext.SourceOnCompleted(AsyncEnumeratorBase<TSource, TResult>.moveNextCallbackDelegate, this);
		}

		private static void MoveNextCallBack(object state)
		{
			AsyncEnumeratorBase<TSource, TResult> asyncEnumeratorBase = (AsyncEnumeratorBase<TSource, TResult>)state;
			bool result;
			try
			{
				if (!asyncEnumeratorBase.TryMoveNextCore(asyncEnumeratorBase.sourceMoveNext.GetResult(), out result))
				{
					asyncEnumeratorBase.SourceMoveNext();
					return;
				}
			}
			catch (Exception error)
			{
				asyncEnumeratorBase.completionSource.TrySetException(error);
				return;
			}
			if (asyncEnumeratorBase.cancellationToken.IsCancellationRequested)
			{
				asyncEnumeratorBase.completionSource.TrySetCanceled(asyncEnumeratorBase.cancellationToken);
				return;
			}
			asyncEnumeratorBase.completionSource.TrySetResult(result);
		}

		public virtual UniTask DisposeAsync()
		{
			if (this.enumerator != null)
			{
				return this.enumerator.DisposeAsync();
			}
			return default(UniTask);
		}

		private static readonly Action<object> moveNextCallbackDelegate = new Action<object>(AsyncEnumeratorBase<TSource, TResult>.MoveNextCallBack);

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		protected CancellationToken cancellationToken;

		private IUniTaskAsyncEnumerator<TSource> enumerator;

		private UniTask<bool>.Awaiter sourceMoveNext;
	}
}
