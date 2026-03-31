using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	public abstract class AsyncTriggerBase<T> : MonoBehaviour, IUniTaskAsyncEnumerable<T>
	{
		private void Awake()
		{
			this.calledAwake = true;
		}

		private void OnDestroy()
		{
			if (this.calledDestroy)
			{
				return;
			}
			this.calledDestroy = true;
			this.triggerEvent.SetCompleted();
		}

		internal void AddHandler(ITriggerHandler<T> handler)
		{
			if (!this.calledAwake)
			{
				PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AsyncTriggerBase<T>.AwakeMonitor(this));
			}
			this.triggerEvent.Add(handler);
		}

		internal void RemoveHandler(ITriggerHandler<T> handler)
		{
			if (!this.calledAwake)
			{
				PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AsyncTriggerBase<T>.AwakeMonitor(this));
			}
			this.triggerEvent.Remove(handler);
		}

		protected void RaiseEvent(T value)
		{
			this.triggerEvent.SetResult(value);
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			return new AsyncTriggerBase<T>.AsyncTriggerEnumerator(this, cancellationToken);
		}

		private TriggerEvent<T> triggerEvent;

		protected internal bool calledAwake;

		protected internal bool calledDestroy;

		private sealed class AsyncTriggerEnumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable, ITriggerHandler<!0>
		{
			public AsyncTriggerEnumerator(AsyncTriggerBase<T> parent, CancellationToken cancellationToken)
			{
				this.parent = parent;
				this.cancellationToken = cancellationToken;
			}

			public void OnCanceled(CancellationToken cancellationToken = default(CancellationToken))
			{
				this.completionSource.TrySetCanceled(cancellationToken);
			}

			public void OnNext(T value)
			{
				this.Current = value;
				this.completionSource.TrySetResult(true);
			}

			public void OnCompleted()
			{
				this.completionSource.TrySetResult(false);
			}

			public void OnError(Exception ex)
			{
				this.completionSource.TrySetException(ex);
			}

			private static void CancellationCallback(object state)
			{
				AsyncTriggerBase<T>.AsyncTriggerEnumerator asyncTriggerEnumerator = (AsyncTriggerBase<T>.AsyncTriggerEnumerator)state;
				asyncTriggerEnumerator.DisposeAsync().Forget();
				asyncTriggerEnumerator.completionSource.TrySetCanceled(asyncTriggerEnumerator.cancellationToken);
			}

			public T Current { get; private set; }

			ITriggerHandler<T> ITriggerHandler<!0>.Prev { get; set; }

			ITriggerHandler<T> ITriggerHandler<!0>.Next { get; set; }

			public UniTask<bool> MoveNextAsync()
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				this.completionSource.Reset();
				if (!this.called)
				{
					this.called = true;
					this.parent.AddHandler(this);
					if (this.cancellationToken.CanBeCanceled)
					{
						this.registration = this.cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncTriggerBase<T>.AsyncTriggerEnumerator.cancellationCallback, this);
					}
				}
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			public UniTask DisposeAsync()
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					this.registration.Dispose();
					this.parent.RemoveHandler(this);
				}
				return default(UniTask);
			}

			private static Action<object> cancellationCallback = new Action<object>(AsyncTriggerBase<T>.AsyncTriggerEnumerator.CancellationCallback);

			private readonly AsyncTriggerBase<T> parent;

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration registration;

			private bool called;

			private bool isDisposed;
		}

		private class AwakeMonitor : IPlayerLoopItem
		{
			public AwakeMonitor(AsyncTriggerBase<T> trigger)
			{
				this.trigger = trigger;
			}

			public bool MoveNext()
			{
				if (this.trigger.calledAwake)
				{
					return false;
				}
				if (this.trigger == null)
				{
					this.trigger.OnDestroy();
					return false;
				}
				return true;
			}

			private readonly AsyncTriggerBase<T> trigger;
		}
	}
}
