using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	[StructLayout(LayoutKind.Auto)]
	public struct AsyncUniTaskVoidMethodBuilder
	{
		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static AsyncUniTaskVoidMethodBuilder Create()
		{
			return default(AsyncUniTaskVoidMethodBuilder);
		}

		public UniTaskVoid Task
		{
			[DebuggerHidden]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return default(UniTaskVoid);
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetException(Exception exception)
		{
			if (this.runner != null)
			{
				this.runner.Return();
				this.runner = null;
			}
			UniTaskScheduler.PublishUnobservedTaskException(exception);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetResult()
		{
			if (this.runner != null)
			{
				this.runner.Return();
				this.runner = null;
			}
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (this.runner == null)
			{
				AsyncUniTaskVoid<TStateMachine>.SetStateMachine(ref stateMachine, ref this.runner);
			}
			awaiter.OnCompleted(this.runner.MoveNext);
		}

		[DebuggerHidden]
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			if (this.runner == null)
			{
				AsyncUniTaskVoid<TStateMachine>.SetStateMachine(ref stateMachine, ref this.runner);
			}
			awaiter.UnsafeOnCompleted(this.runner.MoveNext);
		}

		[DebuggerHidden]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			stateMachine.MoveNext();
		}

		[DebuggerHidden]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		private IStateMachineRunner runner;
	}
}
