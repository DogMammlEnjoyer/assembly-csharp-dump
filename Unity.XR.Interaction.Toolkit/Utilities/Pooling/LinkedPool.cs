using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling
{
	internal class LinkedPool<T> : IDisposable where T : class
	{
		public LinkedPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int maxSize = 10000)
		{
			if (createFunc == null)
			{
				throw new ArgumentNullException("createFunc");
			}
			if (maxSize <= 0)
			{
				throw new ArgumentException("Max size must be greater than 0", "maxSize");
			}
			this.m_CreateFunc = createFunc;
			this.m_ActionOnGet = actionOnGet;
			this.m_ActionOnRelease = actionOnRelease;
			this.m_ActionOnDestroy = actionOnDestroy;
			this.m_Limit = maxSize;
			this.m_CollectionCheck = collectionCheck;
		}

		public int countInactive { get; private set; }

		public T Get()
		{
			T t;
			if (this.m_PoolFirst == null)
			{
				t = this.m_CreateFunc();
			}
			else
			{
				LinkedPool<T>.LinkedPoolItem poolFirst = this.m_PoolFirst;
				t = poolFirst.value;
				this.m_PoolFirst = poolFirst.poolNext;
				poolFirst.poolNext = this.m_NextAvailableListItem;
				this.m_NextAvailableListItem = poolFirst;
				this.m_NextAvailableListItem.value = default(T);
				int countInactive = this.countInactive - 1;
				this.countInactive = countInactive;
			}
			Action<T> actionOnGet = this.m_ActionOnGet;
			if (actionOnGet != null)
			{
				actionOnGet(t);
			}
			return t;
		}

		public PooledObject<T> Get(out T v)
		{
			return new PooledObject<T>(v = this.Get(), this);
		}

		public void Release(T item)
		{
			Action<T> actionOnRelease = this.m_ActionOnRelease;
			if (actionOnRelease != null)
			{
				actionOnRelease(item);
			}
			if (this.countInactive < this.m_Limit)
			{
				LinkedPool<T>.LinkedPoolItem linkedPoolItem = this.m_NextAvailableListItem;
				if (linkedPoolItem == null)
				{
					linkedPoolItem = new LinkedPool<T>.LinkedPoolItem();
				}
				else
				{
					this.m_NextAvailableListItem = linkedPoolItem.poolNext;
				}
				linkedPoolItem.value = item;
				linkedPoolItem.poolNext = this.m_PoolFirst;
				this.m_PoolFirst = linkedPoolItem;
				int countInactive = this.countInactive + 1;
				this.countInactive = countInactive;
				return;
			}
			Action<T> actionOnDestroy = this.m_ActionOnDestroy;
			if (actionOnDestroy == null)
			{
				return;
			}
			actionOnDestroy(item);
		}

		public void Clear()
		{
			if (this.m_ActionOnDestroy != null)
			{
				for (LinkedPool<T>.LinkedPoolItem linkedPoolItem = this.m_PoolFirst; linkedPoolItem != null; linkedPoolItem = linkedPoolItem.poolNext)
				{
					this.m_ActionOnDestroy(linkedPoolItem.value);
				}
			}
			this.m_PoolFirst = null;
			this.m_NextAvailableListItem = null;
			this.countInactive = 0;
		}

		public void Dispose()
		{
			this.Clear();
		}

		private readonly Func<T> m_CreateFunc;

		private readonly Action<T> m_ActionOnGet;

		private readonly Action<T> m_ActionOnRelease;

		private readonly Action<T> m_ActionOnDestroy;

		private readonly int m_Limit;

		private LinkedPool<T>.LinkedPoolItem m_PoolFirst;

		private LinkedPool<T>.LinkedPoolItem m_NextAvailableListItem;

		private readonly bool m_CollectionCheck;

		internal class LinkedPoolItem
		{
			internal LinkedPool<T>.LinkedPoolItem poolNext;

			internal T value;
		}
	}
}
