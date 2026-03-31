using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	[StructLayout(LayoutKind.Auto)]
	public struct AsyncUniTaskMethodBuilder
	{
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncUniTaskMethodBuilder Create()
		{
			return default(AsyncUniTaskMethodBuilder);
		}

		public UniTask Task
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
					return UniTask.FromException(this.ex);
				}
				return UniTask.CompletedTask;
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
		public void SetResult()
		{
			if (this.runnerPromise != null)
			{
				this.runnerPromise.SetResult();
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (this.runnerPromise == null)
			{
				AsyncUniTask<TStateMachine>.SetStateMachine(ref stateMachine, ref this.runnerPromise);
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
				AsyncUniTask<TStateMachine>.SetStateMachine(ref stateMachine, ref this.runnerPromise);
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

		private IStateMachineRunnerPromise runnerPromise;

		private Exception ex;
	}
}
