using System;
using System.Threading;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	public class UnityEventHandlerAsyncEnumerable<T> : IUniTaskAsyncEnumerable<T>
	{
		public UnityEventHandlerAsyncEnumerable(UnityEvent<T> unityEvent, CancellationToken cancellationToken)
		{
			this.unityEvent = unityEvent;
			this.cancellationToken1 = cancellationToken;
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.cancellationToken1 == cancellationToken)
			{
				return new UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator(this.unityEvent, this.cancellationToken1, CancellationToken.None);
			}
			return new UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator(this.unityEvent, this.cancellationToken1, cancellationToken);
		}

		private readonly UnityEvent<T> unityEvent;

		private readonly CancellationToken cancellationToken1;

		private class UnityEventHandlerAsyncEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable
		{
			public UnityEventHandlerAsyncEnumerator(UnityEvent<T> unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
			{
				this.unityEvent = unityEvent;
				this.cancellationToken1 = cancellationToken1;
				this.cancellationToken2 = cancellationToken2;
			}

			public T Current { get; private set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken1.ThrowIfCancellationRequested();
				this.cancellationToken2.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.unityAction == null)
				{
					this.unityAction = new UnityAction<T>(this.Invoke);
					this.unityEvent.AddListener(this.unityAction);
					if (this.cancellationToken1.CanBeCanceled)
					{
						this.registration1 = this.cancellationToken1.RegisterWithoutCaptureExecutionContext(UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator.cancel1, this);
					}
					if (this.cancellationToken2.CanBeCanceled)
					{
						this.registration2 = this.cancellationToken2.RegisterWithoutCaptureExecutionContext(UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator.cancel2, this);
					}
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void Invoke(T value)
			{
				this.Current = value;
				this.completionSource.TrySetResult(true);
			}

			private static void OnCanceled1(object state)
			{
				UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator unityEventHandlerAsyncEnumerator = (UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator)state;
				try
				{
					unityEventHandlerAsyncEnumerator.completionSource.TrySetCanceled(unityEventHandlerAsyncEnumerator.cancellationToken1);
				}
				finally
				{
					unityEventHandlerAsyncEnumerator.DisposeAsync().Forget();
				}
			}

			private static void OnCanceled2(object state)
			{
				UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator unityEventHandlerAsyncEnumerator = (UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator)state;
				try
				{
					unityEventHandlerAsyncEnumerator.completionSource.TrySetCanceled(unityEventHandlerAsyncEnumerator.cancellationToken2);
				}
				finally
				{
					unityEventHandlerAsyncEnumerator.DisposeAsync().Forget();
				}
			}

			public UniTask DisposeAsync()
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					this.registration1.Dispose();
					this.registration2.Dispose();
					IDisposable disposable = this.unityEvent as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.unityEvent.RemoveListener(this.unityAction);
					this.completionSource.TrySetCanceled(default(CancellationToken));
				}
				return default(UniTask);
			}

			private static readonly Action<object> cancel1 = new Action<object>(UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator.OnCanceled1);

			private static readonly Action<object> cancel2 = new Action<object>(UnityEventHandlerAsyncEnumerable<T>.UnityEventHandlerAsyncEnumerator.OnCanceled2);

			private readonly UnityEvent<T> unityEvent;

			private CancellationToken cancellationToken1;

			private CancellationToken cancellationToken2;

			private UnityAction<T> unityAction;

			private CancellationTokenRegistration registration1;

			private CancellationTokenRegistration registration2;

			private bool isDisposed;
		}
	}
}
