using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	internal sealed class AsyncUniTaskVoid<TStateMachine> : IStateMachineRunner, ITaskPoolNode<AsyncUniTaskVoid<TStateMachine>>, IUniTaskSource where TStateMachine : IAsyncStateMachine
	{
		public Action MoveNext { get; }

		public AsyncUniTaskVoid()
		{
			this.MoveNext = new Action(this.Run);
		}

		public static void SetStateMachine(ref TStateMachine stateMachine, ref IStateMachineRunner runnerFieldRef)
		{
			AsyncUniTaskVoid<TStateMachine> asyncUniTaskVoid;
			if (!AsyncUniTaskVoid<TStateMachine>.pool.TryPop(out asyncUniTaskVoid))
			{
				asyncUniTaskVoid = new AsyncUniTaskVoid<TStateMachine>();
			}
			runnerFieldRef = asyncUniTaskVoid;
			asyncUniTaskVoid.stateMachine = stateMachine;
		}

		static AsyncUniTaskVoid()
		{
			TaskPool.RegisterSizeGetter(typeof(AsyncUniTaskVoid<TStateMachine>), () => AsyncUniTaskVoid<TStateMachine>.pool.Size);
		}

		public ref AsyncUniTaskVoid<TStateMachine> NextNode
		{
			get
			{
				return ref this.nextNode;
			}
		}

		public void Return()
		{
			this.stateMachine = default(TStateMachine);
			AsyncUniTaskVoid<TStateMachine>.pool.TryPush(this);
		}

		[DebuggerHidden]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Run()
		{
			this.stateMachine.MoveNext();
		}

		UniTaskStatus IUniTaskSource.GetStatus(short token)
		{
			return UniTaskStatus.Pending;
		}

		UniTaskStatus IUniTaskSource.UnsafeGetStatus()
		{
			return UniTaskStatus.Pending;
		}

		void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
		{
		}

		void IUniTaskSource.GetResult(short token)
		{
		}

		private static TaskPool<AsyncUniTaskVoid<TStateMachine>> pool;

		private TStateMachine stateMachine;

		private AsyncUniTaskVoid<TStateMachine> nextNode;
	}
}
