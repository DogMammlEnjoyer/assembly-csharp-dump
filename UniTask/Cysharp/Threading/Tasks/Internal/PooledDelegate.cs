using System;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal sealed class PooledDelegate<T> : ITaskPoolNode<PooledDelegate<T>>
	{
		public ref PooledDelegate<T> NextNode
		{
			get
			{
				return ref this.nextNode;
			}
		}

		static PooledDelegate()
		{
			TaskPool.RegisterSizeGetter(typeof(PooledDelegate<T>), () => PooledDelegate<T>.pool.Size);
		}

		private PooledDelegate()
		{
			this.runDelegate = new Action<T>(this.Run);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Action<T> Create(Action continuation)
		{
			PooledDelegate<T> pooledDelegate;
			if (!PooledDelegate<T>.pool.TryPop(out pooledDelegate))
			{
				pooledDelegate = new PooledDelegate<T>();
			}
			pooledDelegate.continuation = continuation;
			return pooledDelegate.runDelegate;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Run(T _)
		{
			Action action = this.continuation;
			this.continuation = null;
			if (action != null)
			{
				PooledDelegate<T>.pool.TryPush(this);
				action();
			}
		}

		private static TaskPool<PooledDelegate<T>> pool;

		private PooledDelegate<T> nextNode;

		private readonly Action<T> runDelegate;

		private Action continuation;
	}
}
