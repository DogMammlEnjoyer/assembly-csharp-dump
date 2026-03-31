using System;
using System.Threading;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	public class AsyncUnityEventHandler : IUniTaskSource, IDisposable, IAsyncClickEventHandler
	{
		public AsyncUnityEventHandler(UnityEvent unityEvent, CancellationToken cancellationToken, bool callOnce)
		{
			this.cancellationToken = cancellationToken;
			if (cancellationToken.IsCancellationRequested)
			{
				this.isDisposed = true;
				return;
			}
			this.action = new UnityAction(this.Invoke);
			this.unityEvent = unityEvent;
			this.callOnce = callOnce;
			unityEvent.AddListener(this.action);
			if (cancellationToken.CanBeCanceled)
			{
				this.registration = cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncUnityEventHandler.cancellationCallback, this);
			}
		}

		public UniTask OnInvokeAsync()
		{
			this.core.Reset();
			if (this.isDisposed)
			{
				this.core.TrySetCanceled(this.cancellationToken);
			}
			return new UniTask(this, this.core.Version);
		}

		private void Invoke()
		{
			this.core.TrySetResult(AsyncUnit.Default);
		}

		private static void CancellationCallback(object state)
		{
			((AsyncUnityEventHandler)state).Dispose();
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.registration.Dispose();
				if (this.unityEvent != null)
				{
					this.unityEvent.RemoveListener(this.action);
				}
				this.core.TrySetCanceled(this.cancellationToken);
			}
		}

		UniTask IAsyncClickEventHandler.OnClickAsync()
		{
			return this.OnInvokeAsync();
		}

		void IUniTaskSource.GetResult(short token)
		{
			try
			{
				this.core.GetResult(token);
			}
			finally
			{
				if (this.callOnce)
				{
					this.Dispose();
				}
			}
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

		private static Action<object> cancellationCallback = new Action<object>(AsyncUnityEventHandler.CancellationCallback);

		private readonly UnityAction action;

		private readonly UnityEvent unityEvent;

		private CancellationToken cancellationToken;

		private CancellationTokenRegistration registration;

		private bool isDisposed;

		private bool callOnce;

		private UniTaskCompletionSourceCore<AsyncUnit> core;
	}
}
