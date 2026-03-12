using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal abstract class QueryResults<T> : IList<T>, ICollection<T>, IEnumerable<!0>, IEnumerable
	{
		internal abstract void GivePartitionedStream(IPartitionedStreamRecipient<T> recipient);

		internal virtual bool IsIndexible
		{
			get
			{
				return false;
			}
		}

		internal virtual T GetElement(int index)
		{
			throw new NotSupportedException();
		}

		internal virtual int ElementsCount
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		int IList<!0>.IndexOf(T item)
		{
			throw new NotSupportedException();
		}

		void IList<!0>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<!0>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get
			{
				return this.GetElement(index);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void ICollection<!0>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<!0>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<!0>.Contains(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<!0>.CopyTo(T[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				return this.ElementsCount;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<!0>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			int num;
			for (int index = 0; index < this.Count; index = num + 1)
			{
				yield return this[index];
				num = index;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}
	}
}
