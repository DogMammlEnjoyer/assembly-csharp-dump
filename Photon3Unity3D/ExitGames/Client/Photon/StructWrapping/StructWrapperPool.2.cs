using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon.StructWrapping
{
	public class StructWrapperPool<T> : StructWrapperPool
	{
		public StructWrapperPool(bool isStaticPool)
		{
			this.pool = new Stack<StructWrapper<T>>();
			this.isStaticPool = isStaticPool;
		}

		public StructWrapper<T> Acquire()
		{
			bool flag = this.pool.Count == 0;
			StructWrapper<T> structWrapper;
			if (flag)
			{
				int num = 1;
				for (;;)
				{
					Pooling releasing = this.isStaticPool ? ((Pooling)3) : Pooling.Connected;
					structWrapper = new StructWrapper<T>(releasing, this.tType, this.wType);
					structWrapper.ReturnPool = this;
					bool flag2 = num == 4;
					if (flag2)
					{
						break;
					}
					this.pool.Push(structWrapper);
					num++;
				}
			}
			else
			{
				structWrapper = this.pool.Pop();
			}
			structWrapper.pooling |= Pooling.CheckedOut;
			return structWrapper;
		}

		public StructWrapper<T> Acquire(T value)
		{
			StructWrapper<T> structWrapper = this.Acquire();
			structWrapper.value = value;
			return structWrapper;
		}

		public int Count
		{
			get
			{
				return this.pool.Count;
			}
		}

		internal void Release(StructWrapper<T> obj)
		{
			obj.pooling &= (Pooling)(-9);
			this.pool.Push(obj);
		}

		public const int GROWBY = 4;

		public readonly Type tType = typeof(T);

		public readonly WrappedType wType = StructWrapperPool.GetWrappedType(typeof(T));

		public Stack<StructWrapper<T>> pool;

		public readonly bool isStaticPool;
	}
}
