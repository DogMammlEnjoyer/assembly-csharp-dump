using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	[StructLayout(LayoutKind.Auto)]
	public struct UniTaskCompletionSourceCore<TResult>
	{
		[DebuggerHidden]
		public void Reset()
		{
			this.ReportUnhandledError();
			this.version += 1;
			this.completedCount = 0;
			this.result = default(TResult);
			this.error = null;
			this.hasUnhandledError = false;
			this.continuation = null;
			this.continuationState = null;
		}

		private void ReportUnhandledError()
		{
			if (this.hasUnhandledError)
			{
				try
				{
					OperationCanceledException ex = this.error as OperationCanceledException;
					if (ex != null)
					{
						UniTaskScheduler.PublishUnobservedTaskException(ex);
					}
					else
					{
						ExceptionHolder exceptionHolder = this.error as ExceptionHolder;
						if (exceptionHolder != null)
						{
							UniTaskScheduler.PublishUnobservedTaskException(exceptionHolder.GetException().SourceException);
						}
					}
				}
				catch
				{
				}
			}
		}

		internal void MarkHandled()
		{
			this.hasUnhandledError = false;
		}

		[DebuggerHidden]
		public bool TrySetResult(TResult result)
		{
			if (Interlocked.Increment(ref this.completedCount) == 1)
			{
				this.result = result;
				if (this.continuation != null || Interlocked.CompareExchange<Action<object>>(ref this.continuation, UniTaskCompletionSourceCoreShared.s_sentinel, null) != null)
				{
					this.continuation(this.continuationState);
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public bool TrySetException(Exception error)
		{
			if (Interlocked.Increment(ref this.completedCount) == 1)
			{
				this.hasUnhandledError = true;
				if (error is OperationCanceledException)
				{
					this.error = error;
				}
				else
				{
					this.error = new ExceptionHolder(ExceptionDispatchInfo.Capture(error));
				}
				if (this.continuation != null || Interlocked.CompareExchange<Action<object>>(ref this.continuation, UniTaskCompletionSourceCoreShared.s_sentinel, null) != null)
				{
					this.continuation(this.continuationState);
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public bool TrySetCanceled(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (Interlocked.Increment(ref this.completedCount) == 1)
			{
				this.hasUnhandledError = true;
				this.error = new OperationCanceledException(cancellationToken);
				if (this.continuation != null || Interlocked.CompareExchange<Action<object>>(ref this.continuation, UniTaskCompletionSourceCoreShared.s_sentinel, null) != null)
				{
					this.continuation(this.continuationState);
					return true;
				}
			}
			return false;
		}

		[DebuggerHidden]
		public short Version
		{
			get
			{
				return this.version;
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTaskStatus GetStatus(short token)
		{
			this.ValidateToken(token);
			if (this.continuation == null || this.completedCount == 0)
			{
				return UniTaskStatus.Pending;
			}
			if (this.error == null)
			{
				return UniTaskStatus.Succeeded;
			}
			if (!(this.error is OperationCanceledException))
			{
				return UniTaskStatus.Faulted;
			}
			return UniTaskStatus.Canceled;
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTaskStatus UnsafeGetStatus()
		{
			if (this.continuation == null || this.completedCount == 0)
			{
				return UniTaskStatus.Pending;
			}
			if (this.error == null)
			{
				return UniTaskStatus.Succeeded;
			}
			if (!(this.error is OperationCanceledException))
			{
				return UniTaskStatus.Faulted;
			}
			return UniTaskStatus.Canceled;
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TResult GetResult(short token)
		{
			this.ValidateToken(token);
			if (this.completedCount == 0)
			{
				throw new InvalidOperationException("Not yet completed, UniTask only allow to use await.");
			}
			if (this.error == null)
			{
				return this.result;
			}
			this.hasUnhandledError = false;
			OperationCanceledException ex = this.error as OperationCanceledException;
			if (ex != null)
			{
				throw ex;
			}
			ExceptionHolder exceptionHolder = this.error as ExceptionHolder;
			if (exceptionHolder != null)
			{
				exceptionHolder.GetException().Throw();
			}
			throw new InvalidOperationException("Critical: invalid exception type was held.");
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnCompleted(Action<object> continuation, object state, short token)
		{
			if (continuation == null)
			{
				throw new ArgumentNullException("continuation");
			}
			this.ValidateToken(token);
			object obj = this.continuation;
			if (obj == null)
			{
				this.continuationState = state;
				obj = Interlocked.CompareExchange<Action<object>>(ref this.continuation, continuation, null);
			}
			if (obj != null)
			{
				if (obj != UniTaskCompletionSourceCoreShared.s_sentinel)
				{
					throw new InvalidOperationException("Already continuation registered, can not await twice or get Status after await.");
				}
				continuation(state);
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ValidateToken(short token)
		{
			if (token != this.version)
			{
				throw new InvalidOperationException("Token version is not matched, can not await twice or get Status after await.");
			}
		}

		private TResult result;

		private object error;

		private short version;

		private bool hasUnhandledError;

		private int completedCount;

		private Action<object> continuation;

		private object continuationState;
	}
}
