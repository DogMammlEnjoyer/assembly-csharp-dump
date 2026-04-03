using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks
{
	[AsyncMethodBuilder(typeof(AsyncUniTaskMethodBuilder<>))]
	[StructLayout(LayoutKind.Auto)]
	public readonly struct UniTask<T>
	{
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTask(T result)
		{
			this.source = null;
			this.token = 0;
			this.result = result;
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTask(IUniTaskSource<T> source, short token)
		{
			this.source = source;
			this.token = token;
			this.result = default(T);
		}

		public UniTaskStatus Status
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (this.source != null)
				{
					return this.source.GetStatus(this.token);
				}
				return UniTaskStatus.Succeeded;
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public UniTask<T>.Awaiter GetAwaiter()
		{
			return new UniTask<T>.Awaiter(ref this);
		}

		public UniTask<T> Preserve()
		{
			if (this.source == null)
			{
				return this;
			}
			return new UniTask<T>(new UniTask<T>.MemoizeSource(this.source), this.token);
		}

		public UniTask AsUniTask()
		{
			if (this.source == null)
			{
				return UniTask.CompletedTask;
			}
			if (this.source.GetStatus(this.token).IsCompletedSuccessfully())
			{
				this.source.GetResult(this.token);
				return UniTask.CompletedTask;
			}
			return new UniTask(this.source, this.token);
		}

		public static implicit operator UniTask(UniTask<T> self)
		{
			return self.AsUniTask();
		}

		[return: TupleElementNames(new string[]
		{
			"IsCanceled",
			"Result"
		})]
		public UniTask<ValueTuple<bool, T>> SuppressCancellationThrow()
		{
			if (this.source == null)
			{
				return new UniTask<ValueTuple<bool, T>>(new ValueTuple<bool, T>(false, this.result));
			}
			return new UniTask<ValueTuple<bool, T>>(new UniTask<T>.IsCanceledSource(this.source), this.token);
		}

		public override string ToString()
		{
			if (this.source != null)
			{
				return "(" + this.source.UnsafeGetStatus().ToString() + ")";
			}
			T t = this.result;
			if (t == null)
			{
				return null;
			}
			return t.ToString();
		}

		private readonly IUniTaskSource<T> source;

		private readonly T result;

		private readonly short token;

		private sealed class IsCanceledSource : IUniTaskSource<ValueTuple<bool, T>>, IUniTaskSource
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public IsCanceledSource(IUniTaskSource<T> source)
			{
				this.source = source;
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ValueTuple<bool, T> GetResult(short token)
			{
				if (this.source.GetStatus(token) == UniTaskStatus.Canceled)
				{
					return new ValueTuple<bool, T>(true, default(T));
				}
				T result = this.source.GetResult(token);
				return new ValueTuple<bool, T>(false, result);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public UniTaskStatus GetStatus(short token)
			{
				return this.source.GetStatus(token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public UniTaskStatus UnsafeGetStatus()
			{
				return this.source.UnsafeGetStatus();
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				this.source.OnCompleted(continuation, state, token);
			}

			private readonly IUniTaskSource<T> source;
		}

		private sealed class MemoizeSource : IUniTaskSource<T>, IUniTaskSource
		{
			public MemoizeSource(IUniTaskSource<T> source)
			{
				this.source = source;
			}

			public T GetResult(short token)
			{
				if (this.source == null)
				{
					if (this.exception != null)
					{
						this.exception.Throw();
					}
					return this.result;
				}
				T t;
				try
				{
					this.result = this.source.GetResult(token);
					this.status = UniTaskStatus.Succeeded;
					t = this.result;
				}
				catch (Exception ex)
				{
					this.exception = ExceptionDispatchInfo.Capture(ex);
					if (ex is OperationCanceledException)
					{
						this.status = UniTaskStatus.Canceled;
					}
					else
					{
						this.status = UniTaskStatus.Faulted;
					}
					throw;
				}
				finally
				{
					this.source = null;
				}
				return t;
			}

			void IUniTaskSource.GetResult(short token)
			{
				this.GetResult(token);
			}

			public UniTaskStatus GetStatus(short token)
			{
				if (this.source == null)
				{
					return this.status;
				}
				return this.source.GetStatus(token);
			}

			public void OnCompleted(Action<object> continuation, object state, short token)
			{
				if (this.source == null)
				{
					continuation(state);
					return;
				}
				this.source.OnCompleted(continuation, state, token);
			}

			public UniTaskStatus UnsafeGetStatus()
			{
				if (this.source == null)
				{
					return this.status;
				}
				return this.source.UnsafeGetStatus();
			}

			private IUniTaskSource<T> source;

			private T result;

			private ExceptionDispatchInfo exception;

			private UniTaskStatus status;
		}

		public readonly struct Awaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Awaiter(in UniTask<T> task)
			{
				this.task = task;
			}

			public bool IsCompleted
			{
				[DebuggerHidden]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return this.task.Status.IsCompleted();
				}
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public T GetResult()
			{
				IUniTaskSource<T> source = this.task.source;
				if (source == null)
				{
					return this.task.result;
				}
				return source.GetResult(this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void OnCompleted(Action continuation)
			{
				IUniTaskSource<T> source = this.task.source;
				if (source == null)
				{
					continuation();
					return;
				}
				source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void UnsafeOnCompleted(Action continuation)
			{
				IUniTaskSource<T> source = this.task.source;
				if (source == null)
				{
					continuation();
					return;
				}
				source.OnCompleted(AwaiterActions.InvokeContinuationDelegate, continuation, this.task.token);
			}

			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SourceOnCompleted(Action<object> continuation, object state)
			{
				IUniTaskSource<T> source = this.task.source;
				if (source == null)
				{
					continuation(state);
					return;
				}
				source.OnCompleted(continuation, state, this.task.token);
			}

			private readonly UniTask<T> task;
		}
	}
}
