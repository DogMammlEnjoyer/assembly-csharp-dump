using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	[StructLayout(LayoutKind.Auto)]
	public struct AsyncUniTaskMethodBuilder<T>
	{
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncUniTaskMethodBuilder<T> Create()
		{
			return default(AsyncUniTaskMethodBuilder<T>);
		}

		public UniTask<T> Task
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				if (this.runnerPromise != null)
				{
					return this.runnerPromise.Task;
				}
				if (this.ex != null)
				{
					return UniTask.FromException<T>(this.ex);
				}
				return UniTask.FromResult<T>(this.result);
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetException(Exception exception)
		{
			if (this.runnerPromise == null)
			{
				this.ex = exception;
				return;
			}
			this.runnerPromise.SetException(exception);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetResult(T result)
		{
			if (this.runnerPromise == null)
			{
				this.result = result;
				return;
			}
			this.runnerPromise.SetResult(result);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (this.runnerPromise == null)
			{
				AsyncUniTask<TStateMachine, T>.SetStateMachine(ref stateMachine, ref this.runnerPromise);
			}
			awaiter.OnCompleted(this.runnerPromise.MoveNext);
		}

		[DebuggerHidden]
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (this.runnerPromise == null)
			{
				AsyncUniTask<TStateMachine, T>.SetStateMachine(ref stateMachine, ref this.runnerPromise);
			}
			awaiter.UnsafeOnCompleted(this.runnerPromise.MoveNext);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		private IStateMachineRunnerPromise<T> runnerPromise;

		private Exception ex;

		private T result;
	}
}
