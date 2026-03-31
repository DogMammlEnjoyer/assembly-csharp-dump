using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	[Serializable]
	public class AsyncReactiveProperty<T> : IAsyncReactiveProperty<T>, IReadOnlyAsyncReactiveProperty<T>, IUniTaskAsyncEnumerable<T>, IDisposable
	{
		public T Value
		{
			get
			{
				return this.latestValue;
			}
			set
			{
				this.latestValue = value;
				this.triggerEvent.SetResult(value);
			}
		}

		public AsyncReactiveProperty(T value)
		{
			this.latestValue = value;
			this.triggerEvent = default(TriggerEvent<T>);
		}

		public IUniTaskAsyncEnumerable<T> WithoutCurrent()
		{
			return new AsyncReactiveProperty<T>.WithoutCurrentEnumerable(this);
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			return new AsyncReactiveProperty<T>.Enumerator(this, cancellationToken, true);
		}

		public void Dispose()
		{
			this.triggerEvent.SetCompleted();
		}

		public static implicit operator T(AsyncReactiveProperty<T> value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			if (AsyncReactiveProperty<T>.isValueType)
			{
				return this.latestValue.ToString();
			}
			ref T ptr = ref this.latestValue;
			T t = default(T);
			if (t == null)
			{
				t = this.latestValue;
				ptr = ref t;
				if (t == null)
				{
					return null;
				}
			}
			return ptr.ToString();
		}

		public UniTask<T> WaitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			short token;
			return new UniTask<T>(AsyncReactiveProperty<T>.WaitAsyncSource.Create(this, cancellationToken, out token), token);
		}

		private TriggerEvent<T> triggerEvent;

		[SerializeField]
		private T latestValue;

		private static bool isValueType = typeof(T).IsValueType;

		private sealed class WaitAsyncSource : IUniTaskSource<!0>, IUniTaskSource, ITriggerHandler<!0>, ITaskPoolNode<AsyncReactiveProperty<T>.WaitAsyncSource>
		{
			ref AsyncReactiveProperty<T>.WaitAsyncSource ITaskPoolNode<AsyncReactiveProperty<!0>.WaitAsyncSource>.NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitAsyncSource()
			{
				TaskPool.RegisterSizeGetter(typeof(AsyncReactiveProperty<T>.WaitAsyncSource), () => AsyncReactiveProperty<T>.WaitAsyncSource.pool.Size);
			}

			private WaitAsyncSource()
			{
			}

			public static IUniTaskSource<T> Create(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
				}
				AsyncReactiveProperty<T>.WaitAsyncSource waitAsyncSource;
				if (!AsyncReactiveProperty<T>.WaitAsyncSource.pool.TryPop(out waitAsyncSource))
				{
					waitAsyncSource = new AsyncReactiveProperty<T>.WaitAsyncSource();
				}
				waitAsyncSource.parent = parent;
				waitAsyncSource.cancellationToken = cancellationToken;
				if (cancellationToken.CanBeCanceled)
				{
					waitAsyncSource.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncReactiveProperty<T>.WaitAsyncSource.cancellationCallback, waitAsyncSource);
				}
				waitAsyncSource.parent.triggerEvent.Add(waitAsyncSource);
				token = waitAsyncSource.core.Version;
				return waitAsyncSource;
			}

			private bool TryReturn()
			{
				this.core.Reset();
				this.cancellationTokenRegistration.Dispose();
				this.cancellationTokenRegistration = default(CancellationTokenRegistration);
				this.parent.triggerEvent.Remove(this);
				this.parent = null;
				this.cancellationToken = default(CancellationToken);
				return AsyncReactiveProperty<T>.WaitAsyncSource.pool.TryPush(this);
			}

			private static void CancellationCallback(object state)
			{
				AsyncReactiveProperty<T>.WaitAsyncSource waitAsyncSource = (AsyncReactiveProperty<T>.WaitAsyncSource)state;
				waitAsyncSource.OnCanceled(waitAsyncSource.cancellationToken);
			}

			public T GetResult(short token)
			{
				T result;
				try
				{
					result = this.core.GetResult(token);
				}
				finally
				{
					this.TryReturn();
				}
				return result;
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.core.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				return this.core.GetStatus(token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				return this.core.UnsafeGetStatus();
			}

			ITriggerHandler<T> ITriggerHandler<!0>.Prev { get; set; }

			ITriggerHandler<T> ITriggerHandler<!0>.Next { get; set; }

			public void OnCanceled(CancellationToken cancellationToken)
			{
				this.core.TrySetCanceled(cancellationToken);
			}

			public void OnCompleted()
			{
				this.core.TrySetCanceled(CancellationToken.None);
			}

			public void OnError(Exception ex)
			{
				this.core.TrySetException(ex);
			}

			public void OnNext(T value)
			{
				this.core.TrySetResult(value);
			}

			private static Action<object> cancellationCallback = new Action<object>(AsyncReactiveProperty<T>.WaitAsyncSource.CancellationCallback);

			private static TaskPool<AsyncReactiveProperty<T>.WaitAsyncSource> pool;

			private AsyncReactiveProperty<T>.WaitAsyncSource nextNode;

			private AsyncReactiveProperty<T> parent;

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration cancellationTokenRegistration;

			private UniTaskCompletionSourceCore<T> core;
		}

		private sealed class WithoutCurrentEnumerable : IUniTaskAsyncEnumerable<T>
		{
			public WithoutCurrentEnumerable(AsyncReactiveProperty<T> parent)
			{
				this.parent = parent;
			}

			public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
			{
				return new AsyncReactiveProperty<T>.Enumerator(this.parent, cancellationToken, false);
			}

			private readonly AsyncReactiveProperty<T> parent;
		}

		private sealed class Enumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable, ITriggerHandler<!0>
		{
			public Enumerator(AsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
			{
				this.parent = parent;
				this.cancellationToken = cancellationToken;
				this.firstCall = publishCurrentValue;
				parent.triggerEvent.Add(this);
				if (cancellationToken.CanBeCanceled)
				{
					this.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(AsyncReactiveProperty<T>.Enumerator.cancellationCallback, this);
				}
			}

			public T Current
			{
				get
				{
					return this.value;
				}
			}

			ITriggerHandler<T> ITriggerHandler<!0>.Prev { get; set; }

			ITriggerHandler<T> ITriggerHandler<!0>.Next { get; set; }

			public UniTask<bool> MoveNextAsync()
			{
				if (this.firstCall)
				{
					this.firstCall = false;
					this.value = this.parent.Value;
					return CompletedTasks.True;
				}
				this.completionSource.Reset();
				return new UniTask<bool>(this, this.completionSource.Version);
			}

			public UniTask DisposeAsync()
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
					this.completionSource.TrySetCanceled(this.cancellationToken);
					this.parent.triggerEvent.Remove(this);
				}
				return default(UniTask);
			}

			public void OnNext(T value)
			{
				this.value = value;
				this.completionSource.TrySetResult(true);
			}

			public void OnCanceled(CancellationToken cancellationToken)
			{
				this.DisposeAsync().Forget();
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
				((AsyncReactiveProperty<T>.Enumerator)state).DisposeAsync().Forget();
			}

			private static Action<object> cancellationCallback = new Action<object>(AsyncReactiveProperty<T>.Enumerator.CancellationCallback);

			private readonly AsyncReactiveProperty<T> parent;

			private readonly CancellationToken cancellationToken;

			private readonly CancellationTokenRegistration cancellationTokenRegistration;

			private T value;

			private bool isDisposed;

			private bool firstCall;
		}
	}
}
