using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	[StructLayout(LayoutKind.Auto)]
	public struct TaskPool<T> where T : class, ITaskPoolNode<T>
	{
		public int Size
		{
			get
			{
				return this.size;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryPop(out T result)
		{
			if (Interlocked.CompareExchange(ref this.gate, 1, 0) == 0)
			{
				T t = this.root;
				if (t != null)
				{
					ref T nextNode = ref t.NextNode;
					this.root = nextNode;
					nextNode = default(T);
					this.size--;
					result = t;
					Volatile.Write(ref this.gate, 0);
					return true;
				}
				Volatile.Write(ref this.gate, 0);
			}
			result = default(T);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryPush(T item)
		{
			if (Interlocked.CompareExchange(ref this.gate, 1, 0) == 0)
			{
				if (this.size < TaskPool.MaxPoolSize)
				{
					*item.NextNode = this.root;
					this.root = item;
					this.size++;
					Volatile.Write(ref this.gate, 0);
					return true;
				}
				Volatile.Write(ref this.gate, 0);
			}
			return false;
		}

		private int gate;

		private int size;

		private T root;
	}
}
