using System;
using System.Diagnostics;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class AutoResetUniTaskCompletionSource<T> : IUniTaskSource<!0>, IUniTaskSource, ITaskPoolNode<AutoResetUniTaskCompletionSource<T>>, IPromise<T>, IResolvePromise<T>, IRejectPromise, ICancelPromise
	{
		public ref AutoResetUniTaskCompletionSource<T> NextNode
		{
			get
			{
				return ref this.nextNode;
			}
		}

		static AutoResetUniTaskCompletionSource()
		{
			TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSource<T>), () => AutoResetUniTaskCompletionSource<T>.pool.Size);
		}

		private AutoResetUniTaskCompletionSource()
		{
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource<T> Create()
		{
			AutoResetUniTaskCompletionSource<T> result;
			if (!AutoResetUniTaskCompletionSource<T>.pool.TryPop(out result))
			{
				result = new AutoResetUniTaskCompletionSource<T>();
			}
			return result;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource<T> CreateFromCanceled(CancellationToken cancellationToken, out short token)
		{
			AutoResetUniTaskCompletionSource<T> autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource<T>.Create();
			autoResetUniTaskCompletionSource.TrySetCanceled(cancellationToken);
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource<T> CreateFromException(Exception exception, out short token)
		{
			AutoResetUniTaskCompletionSource<T> autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource<T>.Create();
			autoResetUniTaskCompletionSource.TrySetException(exception);
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource<T> CreateFromResult(T result, out short token)
		{
			AutoResetUniTaskCompletionSource<T> autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource<T>.Create();
			autoResetUniTaskCompletionSource.TrySetResult(result);
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		public UniTask<T> Task
		{
			[DebuggerHidden]
			get
			{
				return new UniTask<T>(this, this.core.Version);
			}
		}

		[DebuggerHidden]
		public bool TrySetResult(T result)
		{
			return this.core.TrySetResult(result);
		}

		[DebuggerHidden]
		public bool TrySetCanceled(CancellationToken cancellationToken = default(CancellationToken))
		{
			return this.core.TrySetCanceled(cancellationToken);
		}

		[DebuggerHidden]
		public bool TrySetException(Exception exception)
		{
			return this.core.TrySetException(exception);
		}

		[DebuggerHidden]
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

		[DebuggerHidden]
		void IUniTaskSource.GetResult(short token)
		{
			this.GetResult(token);
		}

		[DebuggerHidden]
		public UniTaskStatus GetStatus(short token)
		{
			return this.core.GetStatus(token);
		}

		[DebuggerHidden]
		public UniTaskStatus UnsafeGetStatus()
		{
			return this.core.UnsafeGetStatus();
		}

		[DebuggerHidden]
		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			this.core.OnCompleted(continuation, state, token);
		}

		[DebuggerHidden]
		private bool TryReturn()
		{
			this.core.Reset();
			return AutoResetUniTaskCompletionSource<T>.pool.TryPush(this);
		}

		private static TaskPool<AutoResetUniTaskCompletionSource<T>> pool;

		private AutoResetUniTaskCompletionSource<T> nextNode;

		private UniTaskCompletionSourceCore<T> core;
	}
}
