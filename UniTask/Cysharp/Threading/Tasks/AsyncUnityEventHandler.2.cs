using System;
using System.Threading;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	public class AsyncUnityEventHandler<T> : IUniTaskSource<T>, IUniTaskSource, IDisposable, IAsyncValueChangedEventHandler<T>, IAsyncEndEditEventHandler<T>, IAsyncEndTextSelectionEventHandler<T>, IAsyncTextSelectionEventHandler<T>, IAsyncDeselectEventHandler<T>, IAsyncSelectEventHandler<T>, IAsyncSubmitEventHandler<T>
	{
		public AsyncUnityEventHandler(UnityEvent<T> unityEvent, CancellationToken cancellationToken, bool callOnce)
		{
			this.cancellationToken = cancellationToken;
			if (cancellationToken.IsCancellationRequested)
			{
				this.isDisposed = true;
				return;
			}
			this.action = new UnityAction<T>(this.Invoke);
			this.unityEvent = unityEvent;
			this.callOnce = callOnce;
			unityEvent.AddListener(this.action);
			if (cancellationToken.CanBeCanceled)
			{
				this.registration = cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncUnityEventHandler<T>.cancellationCallback, this);
			}
		}

		public UniTask<T> OnInvokeAsync()
		{
			this.core.Reset();
			if (this.isDisposed)
			{
				this.core.TrySetCanceled(this.cancellationToken);
			}
			return new UniTask<T>(this, this.core.Version);
		}

		private void Invoke(T result)
		{
			this.core.TrySetResult(result);
		}

		private static void CancellationCallback(object state)
		{
			((AsyncUnityEventHandler<T>)state).Dispose();
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.registration.Dispose();
				if (this.unityEvent != null)
				{
					IDisposable disposable = this.unityEvent as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
					this.unityEvent.RemoveListener(this.action);
				}
				this.core.TrySetCanceled(default(CancellationToken));
			}
		}

		UniTask<T> IAsyncValueChangedEventHandler<!0>.OnValueChangedAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncEndEditEventHandler<!0>.OnEndEditAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncEndTextSelectionEventHandler<!0>.OnEndTextSelectionAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncTextSelectionEventHandler<!0>.OnTextSelectionAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncDeselectEventHandler<!0>.OnDeselectAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncSelectEventHandler<!0>.OnSelectAsync()
		{
			return this.OnInvokeAsync();
		}

		UniTask<T> IAsyncSubmitEventHandler<!0>.OnSubmitAsync()
		{
			return this.OnInvokeAsync();
		}

		T IUniTaskSource<!0>.GetResult(short token)
		{
			T result;
			try
			{
				result = this.core.GetResult(token);
			}
			finally
			{
				if (this.callOnce)
				{
					this.Dispose();
				}
			}
			return result;
		}

		void IUniTaskSource.GetResult(short token)
		{
			((IUniTaskSource<!0>)this).GetResult(token);
		}

		UniTaskStatus IUniTaskSource.GetStatus(short token)
		{
			return this.core.GetStatus(token);
		}

		UniTaskStatus IUniTaskSource.UnsafeGetStatus()
		{
			return this.core.UnsafeGetStatus();
		}

		void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
		{
			this.core.OnCompleted(continuation, state, token);
		}

		private static Action<object> cancellationCallback = new Action<object>(AsyncUnityEventHandler<T>.CancellationCallback);

		private readonly UnityAction<T> action;

		private readonly UnityEvent<T> unityEvent;

		private CancellationToken cancellationToken;

		private CancellationTokenRegistration registration;

		private bool isDisposed;

		private bool callOnce;

		private UniTaskCompletionSourceCore<T> core;
	}
}
