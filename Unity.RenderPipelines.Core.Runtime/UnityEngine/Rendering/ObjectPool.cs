using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.Rendering
{
	public class ObjectPool<T> where T : new()
	{
		public int countAll { get; private set; }

		public int countActive
		{
			get
			{
				return this.countAll - this.countInactive;
			}
		}

		public int countInactive
		{
			get
			{
				return this.m_Stack.Count;
			}
		}

		public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease, bool collectionCheck = true)
		{
			this.m_ActionOnGet = actionOnGet;
			this.m_ActionOnRelease = actionOnRelease;
			this.m_CollectionCheck = collectionCheck;
		}

		public T Get()
		{
			T t;
			if (this.m_Stack.Count == 0)
			{
				t = Activator.CreateInstance<T>();
				int countAll = this.countAll;
				this.countAll = countAll + 1;
			}
			else
			{
				t = this.m_Stack.Pop();
			}
			if (this.m_ActionOnGet != null)
			{
				this.m_ActionOnGet(t);
			}
			return t;
		}

		public ObjectPool<T>.PooledObject Get(out T v)
		{
			return new ObjectPool<T>.PooledObject(v = this.Get(), this);
		}

		public void Release(T element)
		{
			if (this.m_ActionOnRelease != null)
			{
				this.m_ActionOnRelease(element);
			}
			this.m_Stack.Push(element);
		}

		private readonly Stack<T> m_Stack = new Stack<T>();

		private readonly UnityAction<T> m_ActionOnGet;

		private readonly UnityAction<T> m_ActionOnRelease;

		private readonly bool m_CollectionCheck = true;

		public struct PooledObject : IDisposable
		{
			internal PooledObject(T value, ObjectPool<T> pool)
			{
				this.m_ToReturn = value;
				this.m_Pool = pool;
			}

			void IDisposable.Dispose()
			{
				this.m_Pool.Release(this.m_ToReturn);
			}

			private readonly T m_ToReturn;

			private readonly ObjectPool<T> m_Pool;
		}
	}
}
