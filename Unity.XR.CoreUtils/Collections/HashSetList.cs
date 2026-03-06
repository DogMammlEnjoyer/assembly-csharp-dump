using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.XR.CoreUtils.Collections
{
	public class HashSetList<T> : ICollection<T>, IEnumerable<!0>, IEnumerable, ISerializable, IDeserializationCallback, ISet<T>, IReadOnlyCollection<T>
	{
		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.m_InternalList[index];
			}
		}

		public HashSetList(int capacity = 0)
		{
			this.m_InternalList = new List<T>(capacity);
			this.m_InternalHashSet = new HashSet<T>();
		}

		public List<T>.Enumerator GetEnumerator()
		{
			return this.m_InternalList.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.m_InternalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void ICollection<!0>.Add(T item)
		{
			if (this.m_InternalHashSet.Add(item))
			{
				this.m_InternalList.Add(item);
				this.m_Count++;
			}
		}

		public bool Add(T item)
		{
			bool flag = this.m_InternalHashSet.Add(item);
			if (flag)
			{
				this.m_InternalList.Add(item);
				this.m_Count++;
			}
			return flag;
		}

		public bool Remove(T item)
		{
			if (this.m_Count == 0)
			{
				return false;
			}
			bool flag = this.m_InternalHashSet.Remove(item);
			if (flag)
			{
				this.m_InternalList.Remove(item);
				this.m_Count--;
			}
			return flag;
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			this.m_InternalHashSet.ExceptWith(other);
			this.RefreshList();
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			this.m_InternalHashSet.IntersectWith(other);
			this.RefreshList();
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return this.m_InternalHashSet.SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			this.m_InternalHashSet.SymmetricExceptWith(other);
			this.RefreshList();
		}

		public void UnionWith(IEnumerable<T> other)
		{
			this.m_InternalHashSet.UnionWith(other);
			this.RefreshList();
		}

		public void Clear()
		{
			this.m_InternalHashSet.Clear();
			this.m_InternalList.Clear();
		}

		public bool Contains(T item)
		{
			return this.m_InternalHashSet.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_InternalList.CopyTo(array, arrayIndex);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.m_InternalHashSet.GetObjectData(info, context);
			this.RefreshList();
		}

		public void OnDeserialization(object sender)
		{
			this.m_InternalHashSet.OnDeserialization(sender);
			this.RefreshList();
		}

		private void RefreshList()
		{
			this.m_InternalList.Clear();
			this.m_InternalList.AddRange(this.m_InternalHashSet);
			this.m_Count = this.m_InternalList.Count;
		}

		public IReadOnlyList<T> AsList()
		{
			return this.m_InternalList;
		}

		private readonly List<T> m_InternalList;

		private readonly HashSet<T> m_InternalHashSet;

		private int m_Count;
	}
}
