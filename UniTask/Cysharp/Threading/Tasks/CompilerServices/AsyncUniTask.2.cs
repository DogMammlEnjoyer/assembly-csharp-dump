using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	internal sealed class AsyncUniTask<TStateMachine, T> : IStateMachineRunnerPromise<T>, IUniTaskSource<T>, IUniTaskSource, ITaskPoolNode<AsyncUniTask<TStateMachine, T>> where TStateMachine : IAsyncStateMachine
	{
		public Action MoveNext { get; }

		private AsyncUniTask()
		{
			this.MoveNext = new Action(this.Run);
		}

		public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunnerPromise<T> runnerPromiseFieldRef)
		{
			AsyncUniTask<TStateMachine, T> asyncUniTask;
			if (!AsyncUniTask<TStateMachine, T>.pool.TryPop(out asyncUniTask))
			{
				asyncUniTask = new AsyncUniTask<TStateMachine, T>();
			}
			runnerPromiseFieldRef = asyncUniTask;
			asyncUniTask.stateMachine = stateMachine;
		}

		public ref AsyncUniTask<TStateMachine, T> NextNode
		{
			get
			{
				return ref this.nextNode;
			}
		}

		static AsyncUniTask()
		{
			TaskPool.RegisterSizeGetter(typeof(AsyncUniTask<TStateMachine, T>), () => AsyncUniTask<TStateMachine, T>.pool.Size);
		}

		private void Return()
		{
			this.core.Reset();
			this.stateMachine = default(TStateMachine);
			AsyncUniTask<TStateMachine, T>.pool.TryPush(this);
		}

		private bool TryReturn()
		{
			this.core.Reset();
			this.stateMachine = default(TStateMachine);
			return AsyncUniTask<TStateMachine, T>.pool.TryPush(this);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Run()
		{
			this.stateMachine.MoveNext();
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
		public void SetResult(T result)
		{
			this.core.TrySetResult(result);
		}

		[DebuggerHidden]
		public void SetException(Exception exception)
		{
			this.core.TrySetException(exception);
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

		private static TaskPool<AsyncUniTask<TStateMachine, T>> pool;

		private TStateMachine stateMachine;

		private UniTaskCompletionSourceCore<T> core;

		private AsyncUniTask<TStateMachine, T> nextNode;
	}
}
