using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal class ToUniTaskAsyncEnumerableObservable<T> : IUniTaskAsyncEnumerable<T>
	{
		public ToUniTaskAsyncEnumerableObservable(IObservable<T> source)
		{
			this.source = source;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new ToUniTaskAsyncEnumerableObservable<T>._ToUniTaskAsyncEnumerableObservable(this.source, cancellationToken);
		}

		private readonly IObservable<T> source;

		private class _ToUniTaskAsyncEnumerableObservable : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable, IObserver<T>
		{
			public _ToUniTaskAsyncEnumerableObservable(IObservable<T> source, CancellationToken cancellationToken)
			{
				this.source = source;
				this.cancellationToken = cancellationToken;
				this.queuedResult = new Queue<T>();
				if (cancellationToken.CanBeCanceled)
				{
					this.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(ToUniTaskAsyncEnumerableObservable<T>._ToUniTaskAsyncEnumerableObservable.OnCanceledDelegate, this);
				}
			}

			public T Current
			{
				get
				{
					if (this.useCachedCurrent)
					{
						return this.current;
					}
					Queue<T> obj = this.queuedResult;
					T result;
					lock (obj)
					{
						if (this.queuedResult.Count != 0)
						{
							this.current = this.queuedResult.Dequeue();
							this.useCachedCurrent = true;
							result = this.current;
						}
						else
						{
							result = default(T);
						}
					}
					return result;
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				Queue<T> obj = this.queuedResult;
				UniTask<bool> result;
				lock (obj)
				{
					this.useCachedCurrent = false;
					if (this.cancellationToken.IsCancellationRequested)
					{
						result = UniTask.FromCanceled<bool>(this.cancellationToken);
					}
					else
					{
						if (this.subscription == null)
						{
							this.subscription = this.source.Subscribe(this);
						}
						if (this.error != null)
						{
							result = UniTask.FromException<bool>(this.error);
						}
						else if (this.queuedResult.Count != 0)
						{
							result = CompletedTasks.True;
						}
						else if (this.subscribeCompleted)
						{
							result = CompletedTasks.False;
						}
						else
						{
							this.completionSource.Reset();
							result = new UniTask<bool>(this, this.completionSource.Version);
						}
					}
				}
				return result;
			}

			public UniTask DisposeAsync()
			{
				this.subscription.Dispose();
				this.cancellationTokenRegistration.Dispose();
				this.completionSource.Reset();
				return default(UniTask);
			}

			public void OnCompleted()
			{
				Queue<T> obj = this.queuedResult;
				lock (obj)
				{
					this.subscribeCompleted = true;
					this.completionSource.TrySetResult(false);
				}
			}

			public void OnError(Exception error)
			{
				Queue<T> obj = this.queuedResult;
				lock (obj)
				{
					this.error = error;
					this.completionSource.TrySetException(error);
				}
			}

			public void OnNext(T value)
			{
				Queue<T> obj = this.queuedResult;
				lock (obj)
				{
					this.queuedResult.Enqueue(value);
					this.completionSource.TrySetResult(true);
				}
			}

			private static void OnCanceled(object state)
			{
				ToUniTaskAsyncEnumerableObservable<T>._ToUniTaskAsyncEnumerableObservable toUniTaskAsyncEnumerableObservable = (ToUniTaskAsyncEnumerableObservable<T>._ToUniTaskAsyncEnumerableObservable)state;
				Queue<T> obj = toUniTaskAsyncEnumerableObservable.queuedResult;
				lock (obj)
				{
					toUniTaskAsyncEnumerableObservable.completionSource.TrySetCanceled(toUniTaskAsyncEnumerableObservable.cancellationToken);
				}
			}

			private static readonly Action<object> OnCanceledDelegate = new Action<object>(ToUniTaskAsyncEnumerableObservable<T>._ToUniTaskAsyncEnumerableObservable.OnCanceled);

			private readonly IObservable<T> source;

			private CancellationToken cancellationToken;

			private bool useCachedCurrent;

			private T current;

			private bool subscribeCompleted;

			private readonly Queue<T> queuedResult;

			private Exception error;

			private IDisposable subscription;

			private CancellationTokenRegistration cancellationTokenRegistration;
		}
	}
}
