using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal abstract class BaseRegistrationList<T>
	{
		public List<T> registeredSnapshot { get; } = new List<T>();

		public int flushedCount
		{
			get
			{
				return this.registeredSnapshot.Count - this.bufferedRemoveCount + this.bufferedAddCount;
			}
		}

		protected int bufferedAddCount
		{
			get
			{
				List<T> bufferedAdd = this.m_BufferedAdd;
				if (bufferedAdd == null)
				{
					return 0;
				}
				return bufferedAdd.Count;
			}
		}

		protected int bufferedRemoveCount
		{
			get
			{
				List<T> bufferedRemove = this.m_BufferedRemove;
				if (bufferedRemove == null)
				{
					return 0;
				}
				return bufferedRemove.Count;
			}
		}

		protected void AddToBufferedAdd(T item)
		{
			if (this.m_BufferedAdd == null)
			{
				this.m_BufferedAdd = BaseRegistrationList<T>.s_BufferedListPool.Get();
			}
			this.m_BufferedAdd.Add(item);
		}

		protected bool RemoveFromBufferedAdd(T item)
		{
			return this.m_BufferedAdd != null && this.m_BufferedAdd.Remove(item);
		}

		protected void ClearBufferedAdd()
		{
			if (this.m_BufferedAdd == null)
			{
				return;
			}
			BaseRegistrationList<T>.s_BufferedListPool.Release(this.m_BufferedAdd);
			this.m_BufferedAdd = null;
		}

		protected void AddToBufferedRemove(T item)
		{
			if (this.m_BufferedRemove == null)
			{
				this.m_BufferedRemove = BaseRegistrationList<T>.s_BufferedListPool.Get();
			}
			this.m_BufferedRemove.Add(item);
		}

		protected bool RemoveFromBufferedRemove(T item)
		{
			return this.m_BufferedRemove != null && this.m_BufferedRemove.Remove(item);
		}

		protected void ClearBufferedRemove()
		{
			if (this.m_BufferedRemove == null)
			{
				return;
			}
			BaseRegistrationList<T>.s_BufferedListPool.Release(this.m_BufferedRemove);
			this.m_BufferedRemove = null;
		}

		public abstract bool IsRegistered(T item);

		public abstract bool IsStillRegistered(T item);

		public abstract bool Register(T item);

		public abstract bool Unregister(T item);

		public abstract void Flush();

		public abstract void GetRegisteredItems(List<T> results);

		public abstract T GetRegisteredItemAt(int index);

		public bool MoveItemImmediately(T item, int newIndex)
		{
			if (this.bufferedRemoveCount != 0 || this.bufferedAddCount != 0)
			{
				throw new InvalidOperationException("Cannot move item when there are pending registration changes that have not been flushed.");
			}
			int num = this.registeredSnapshot.IndexOf(item);
			if (num == newIndex)
			{
				return false;
			}
			if (num >= 0)
			{
				this.registeredSnapshot.RemoveAt(num);
			}
			this.registeredSnapshot.Insert(newIndex, item);
			this.OnItemMovedImmediately(item, newIndex);
			return num < 0;
		}

		protected virtual void OnItemMovedImmediately(T item, int newIndex)
		{
		}

		public void UnregisterAll()
		{
			List<T> list;
			using (BaseRegistrationList<T>.s_BufferedListPool.Get(out list))
			{
				this.GetRegisteredItems(list);
				for (int i = list.Count - 1; i >= 0; i--)
				{
					this.Unregister(list[i]);
				}
			}
		}

		protected static void EnsureCapacity(List<T> list, int capacity)
		{
			if (list.Capacity < capacity)
			{
				list.Capacity = capacity;
			}
		}

		private static readonly LinkedPool<List<T>> s_BufferedListPool = new LinkedPool<List<T>>(() => new List<T>(), null, delegate(List<T> list)
		{
			list.Clear();
		}, null, false, 10000);

		protected List<T> m_BufferedAdd;

		protected List<T> m_BufferedRemove;
	}
}
