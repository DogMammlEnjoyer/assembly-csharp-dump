using System;
using System.Diagnostics;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class AutoResetUniTaskCompletionSource : IUniTaskSource, ITaskPoolNode<AutoResetUniTaskCompletionSource>, IPromise, IResolvePromise, IRejectPromise, ICancelPromise
	{
		public ref AutoResetUniTaskCompletionSource NextNode
		{
			get
			{
				return ref this.nextNode;
			}
		}

		static AutoResetUniTaskCompletionSource()
		{
			TaskPool.RegisterSizeGetter(typeof(AutoResetUniTaskCompletionSource), () => AutoResetUniTaskCompletionSource.pool.Size);
		}

		private AutoResetUniTaskCompletionSource()
		{
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource Create()
		{
			AutoResetUniTaskCompletionSource result;
			if (!AutoResetUniTaskCompletionSource.pool.TryPop(out result))
			{
				result = new AutoResetUniTaskCompletionSource();
			}
			return result;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource CreateFromCanceled(CancellationToken cancellationToken, out short token)
		{
			AutoResetUniTaskCompletionSource autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();
			autoResetUniTaskCompletionSource.TrySetCanceled(cancellationToken);
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource CreateFromException(Exception exception, out short token)
		{
			AutoResetUniTaskCompletionSource autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();
			autoResetUniTaskCompletionSource.TrySetException(exception);
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		[DebuggerHidden]
		public static AutoResetUniTaskCompletionSource CreateCompleted(out short token)
		{
			AutoResetUniTaskCompletionSource autoResetUniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();
			autoResetUniTaskCompletionSource.TrySetResult();
			token = autoResetUniTaskCompletionSource.core.Version;
			return autoResetUniTaskCompletionSource;
		}

		public UniTask Task
		{
			[DebuggerHidden]
			get
			{
				return new UniTask(this, this.core.Version);
			}
		}

		[DebuggerHidden]
		public bool TrySetResult()
		{
			return this.core.TrySetResult(AsyncUnit.Default);
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
		public void GetResult(short token)
		{
			try
			{
				this.core.GetResult(token);
			}
			finally
			{
				this.TryReturn();
			}
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
			return AutoResetUniTaskCompletionSource.pool.TryPush(this);
		}

		private static TaskPool<AutoResetUniTaskCompletionSource> pool;

		private AutoResetUniTaskCompletionSource nextNode;

		private UniTaskCompletionSourceCore<AsyncUnit> core;
	}
}
