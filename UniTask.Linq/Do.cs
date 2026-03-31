using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class Do<TSource> : IUniTaskAsyncEnumerable<TSource>
	{
		public Do(IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted)
		{
			this.source = source;
			this.onNext = onNext;
			this.onError = onError;
			this.onCompleted = onCompleted;
		}

		public IUniTaskAsyncEnumerator<TSource> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new Do<TSource>._Do(this.source, this.onNext, this.onError, this.onCompleted, cancellationToken);
		}

		private readonly IUniTaskAsyncEnumerable<TSource> source;

		private readonly Action<TSource> onNext;

		private readonly Action<Exception> onError;

		private readonly Action onCompleted;

		private sealed class _Do : MoveNextSource, IUniTaskAsyncEnumerator<TSource>, IUniTaskAsyncDisposable
		{
			public _Do(IUniTaskAsyncEnumerable<TSource> source, Action<TSource> onNext, Action<Exception> onError, Action onCompleted, CancellationToken cancellationToken)
			{
				this.source = source;
				this.onNext = onNext;
				this.onError = onError;
				this.onCompleted = onCompleted;
				this.cancellationToken = cancellationToken;
			}

			public TSource Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				bool flag = false;
				try
				{
					if (this.enumerator == null)
					{
						this.enumerator = this.source.GetAsyncEnumerator(this.cancellationToken);
					}
					this.awaiter = this.enumerator.MoveNextAsync().GetAwaiter();
					flag = this.awaiter.IsCompleted;
				}
				catch (Exception ex)
				{
					this.CallTrySetExceptionAfterNotification(ex);
					return new UniTask<bool>(this, this.completionSource.Version);
				}
				if (flag)
				{
					Do<TSource>._Do.MoveNextCore(this);
				}
				else
				{
					this.awaiter.SourceOnCompleted(Do<TSource>._Do.MoveNextCoreDelegate, this);
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void CallTrySetExceptionAfterNotification(Exception ex)
			{
				if (this.onError != null)
				{
					try
					{
						this.onError(ex);
					}
					catch (Exception error)
					{
						this.completionSource.TrySetException(error);
						return;
					}
				}
				this.completionSource.TrySetException(ex);
			}

			private bool TryGetResultWithNotification<T>(UniTask<T>.Awaiter awaiter, out T result)
			{
				bool result2;
				try
				{
					result = awaiter.GetResult();
					result2 = true;
				}
				catch (Exception ex)
				{
					this.CallTrySetExceptionAfterNotification(ex);
					result = default(T);
					result2 = false;
				}
				return result2;
			}

			private static void MoveNextCore(object state)
			{
				Do<TSource>._Do @do = (Do<TSource>._Do)state;
				bool flag;
				if (@do.TryGetResultWithNotification<bool>(@do.awaiter, out flag))
				{
					if (flag)
					{
						TSource tsource = @do.enumerator.Current;
						if (@do.onNext != null)
						{
							try
							{
								@do.onNext(tsource);
							}
							catch (Exception ex)
							{
								@do.CallTrySetExceptionAfterNotification(ex);
							}
						}
						@do.Current = tsource;
						@do.completionSource.TrySetResult(true);
						return;
					}
					if (@do.onCompleted != null)
					{
						try
						{
							@do.onCompleted();
						}
						catch (Exception ex2)
						{
							@do.CallTrySetExceptionAfterNotification(ex2);
							return;
						}
					}
					@do.completionSource.TrySetResult(false);
				}
			}

			public UniTask DisposeAsync()
			{
				if (this.enumerator != null)
				{
					return this.enumerator.DisposeAsync();
				}
				return default(UniTask);
			}

			private static readonly Action<object> MoveNextCoreDelegate = new Action<object>(Do<TSource>._Do.MoveNextCore);

			private readonly IUniTaskAsyncEnumerable<TSource> source;

			private readonly Action<TSource> onNext;

			private readonly Action<Exception> onError;

			private readonly Action onCompleted;

			private CancellationToken cancellationToken;

			private IUniTaskAsyncEnumerator<TSource> enumerator;

			private UniTask<bool>.Awaiter awaiter;
		}
	}
}
