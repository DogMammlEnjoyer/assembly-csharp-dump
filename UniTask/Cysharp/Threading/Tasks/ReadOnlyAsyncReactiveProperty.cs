using System;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks
{
	public class ReadOnlyAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>, IUniTaskAsyncEnumerable<T>, IDisposable
	{
		public T Value
		{
			get
			{
				return this.latestValue;
			}
		}

		public ReadOnlyAsyncReactiveProperty(T initialValue, IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
		{
			this.latestValue = initialValue;
			this.ConsumeEnumerator(source, cancellationToken).Forget();
		}

		public ReadOnlyAsyncReactiveProperty(IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
		{
			this.ConsumeEnumerator(source, cancellationToken).Forget();
		}

		private UniTaskVoid ConsumeEnumerator(IUniTaskAsyncEnumerable<T> source, CancellationToken cancellationToken)
		{
			ReadOnlyAsyncReactiveProperty<T>.<ConsumeEnumerator>d__7 <ConsumeEnumerator>d__;
			<ConsumeEnumerator>d__.<>t__builder = AsyncUniTaskVoidMethodBuilder.Create();
			<ConsumeEnumerator>d__.<>4__this = this;
			<ConsumeEnumerator>d__.source = source;
			<ConsumeEnumerator>d__.cancellationToken = cancellationToken;
			<ConsumeEnumerator>d__.<>1__state = -1;
			<ConsumeEnumerator>d__.<>t__builder.Start<ReadOnlyAsyncReactiveProperty<T>.<ConsumeEnumerator>d__7>(ref <ConsumeEnumerator>d__);
			return <ConsumeEnumerator>d__.<>t__builder.Task;
		}

		public IUniTaskAsyncEnumerable<T> WithoutCurrent()
		{
			return new ReadOnlyAsyncReactiveProperty<T>.WithoutCurrentEnumerable(this);
		}

		public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			return new ReadOnlyAsyncReactiveProperty<T>.Enumerator(this, cancellationToken, true);
		}

		public void Dispose()
		{
			if (this.enumerator != null)
			{
				this.enumerator.DisposeAsync().Forget();
			}
			this.triggerEvent.SetCompleted();
		}

		public static implicit operator T(ReadOnlyAsyncReactiveProperty<T> value)
		{
			return value.Value;
		}

		public override string ToString()
		{
			if (ReadOnlyAsyncReactiveProperty<T>.isValueType)
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
			return new UniTask<T>(ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.Create(this, cancellationToken, out token), token);
		}

		private TriggerEvent<T> triggerEvent;

		private T latestValue;

		private IUniTaskAsyncEnumerator<T> enumerator;

		private static bool isValueType = typeof(T).IsValueType;

		private sealed class WaitAsyncSource : IUniTaskSource<!0>, IUniTaskSource, ITriggerHandler<!0>, ITaskPoolNode<ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource>
		{
			ref ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource ITaskPoolNode<ReadOnlyAsyncReactiveProperty<!0>.WaitAsyncSource>.NextNode
			{
				get
				{
					return ref this.nextNode;
				}
			}

			static WaitAsyncSource()
			{
				TaskPool.RegisterSizeGetter(typeof(ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource), () => ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.pool.Size);
			}

			private WaitAsyncSource()
			{
			}

			public static IUniTaskSource<T> Create(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, out short token)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return AutoResetUniTaskCompletionSource<T>.CreateFromCanceled(cancellationToken, out token);
				}
				ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource waitAsyncSource;
				if (!ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.pool.TryPop(out waitAsyncSource))
				{
					waitAsyncSource = new ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource();
				}
				waitAsyncSource.parent = parent;
				waitAsyncSource.cancellationToken = cancellationToken;
				if (cancellationToken.CanBeCanceled)
				{
					waitAsyncSource.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.cancellationCallback, waitAsyncSource);
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
				return ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.pool.TryPush(this);
			}

			private static void CancellationCallback(object state)
			{
				ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource waitAsyncSource = (ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource)state;
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

			private static Action<object> cancellationCallback = new Action<object>(ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource.CancellationCallback);

			private static TaskPool<ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource> pool;

			private ReadOnlyAsyncReactiveProperty<T>.WaitAsyncSource nextNode;

			private ReadOnlyAsyncReactiveProperty<T> parent;

			private CancellationToken cancellationToken;

			private CancellationTokenRegistration cancellationTokenRegistration;

			private UniTaskCompletionSourceCore<T> core;
		}

		private sealed class WithoutCurrentEnumerable : IUniTaskAsyncEnumerable<T>
		{
			public WithoutCurrentEnumerable(ReadOnlyAsyncReactiveProperty<T> parent)
			{
				this.parent = parent;
			}

			public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
			{
				return new ReadOnlyAsyncReactiveProperty<T>.Enumerator(this.parent, cancellationToken, false);
			}

			private readonly ReadOnlyAsyncReactiveProperty<T> parent;
		}

		private sealed class Enumerator : MoveNextSource, IUniTaskAsyncEnumerator<T>, IUniTaskAsyncDisposable, ITriggerHandler<!0>
		{
			public Enumerator(ReadOnlyAsyncReactiveProperty<T> parent, CancellationToken cancellationToken, bool publishCurrentValue)
			{
				this.parent = parent;
				this.cancellationToken = cancellationToken;
				this.firstCall = publishCurrentValue;
				parent.triggerEvent.Add(this);
				if (cancellationToken.CanBeCanceled)
				{
					this.cancellationTokenRegistration = cancellationToken.RegisterWithoutCaptureExecutionContext(ReadOnlyAsyncReactiveProperty<T>.Enumerator.cancellationCallback, this);
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
				((ReadOnlyAsyncReactiveProperty<T>.Enumerator)state).DisposeAsync().Forget();
			}

			private static Action<object> cancellationCallback = new Action<object>(ReadOnlyAsyncReactiveProperty<T>.Enumerator.CancellationCallback);

			private readonly ReadOnlyAsyncReactiveProperty<T> parent;

			private readonly CancellationToken cancellationToken;

			private readonly CancellationTokenRegistration cancellationTokenRegistration;

			private T value;

			private bool isDisposed;

			private bool firstCall;
		}
	}
}
