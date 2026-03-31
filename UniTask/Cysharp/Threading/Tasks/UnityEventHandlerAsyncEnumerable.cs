using System;
using System.Threading;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	public class UnityEventHandlerAsyncEnumerable : IUniTaskAsyncEnumerable<AsyncUnit>
	{
		public UnityEventHandlerAsyncEnumerable(UnityEvent unityEvent, CancellationToken cancellationToken)
		{
			this.unityEvent = unityEvent;
			this.cancellationToken1 = cancellationToken;
		}

		public IUniTaskAsyncEnumerator<AsyncUnit> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this.cancellationToken1 == cancellationToken)
			{
				return new UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator(this.unityEvent, this.cancellationToken1, CancellationToken.None);
			}
			return new UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator(this.unityEvent, this.cancellationToken1, cancellationToken);
		}

		private readonly UnityEvent unityEvent;

		private readonly CancellationToken cancellationToken1;

		private class UnityEventHandlerAsyncEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<AsyncUnit>, IUniTaskAsyncDisposable
		{
			public UnityEventHandlerAsyncEnumerator(UnityEvent unityEvent, CancellationToken cancellationToken1, CancellationToken cancellationToken2)
			{
				this.unityEvent = unityEvent;
				this.cancellationToken1 = cancellationToken1;
				this.cancellationToken2 = cancellationToken2;
			}

			public AsyncUnit Current
			{
				get
				{
					return default(AsyncUnit);
				}
			}

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken1.ThrowIfCancellationRequested();
				this.cancellationToken2.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (this.unityAction == null)
				{
					this.unityAction = new UnityAction(this.Invoke);
					this.unityEvent.AddListener(this.unityAction);
					if (this.cancellationToken1.CanBeCanceled)
					{
						this.registration1 = this.cancellationToken1.RegisterWithoutCaptureExecutionContext(UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator.cancel1, this);
					}
					if (this.cancellationToken2.CanBeCanceled)
					{
						this.registration2 = this.cancellationToken2.RegisterWithoutCaptureExecutionContext(UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator.cancel2, this);
					}
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			private void Invoke()
			{
				this.completionSource.TrySetResult(true);
			}

			private static void OnCanceled1(object state)
			{
				UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator unityEventHandlerAsyncEnumerator = (UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator)state;
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
				UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator unityEventHandlerAsyncEnumerator = (UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator)state;
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
					this.unityEvent.RemoveListener(this.unityAction);
					this.completionSource.TrySetCanceled(default(CancellationToken));
				}
				return default(UniTask);
			}

			private static readonly Action<object> cancel1 = new Action<object>(UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator.OnCanceled1);

			private static readonly Action<object> cancel2 = new Action<object>(UnityEventHandlerAsyncEnumerable.UnityEventHandlerAsyncEnumerator.OnCanceled2);

			private readonly UnityEvent unityEvent;

			private CancellationToken cancellationToken1;

			private CancellationToken cancellationToken2;

			private UnityAction unityAction;

			private CancellationTokenRegistration registration1;

			private CancellationTokenRegistration registration2;

			private bool isDisposed;
		}
	}
}
