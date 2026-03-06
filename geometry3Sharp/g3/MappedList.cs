using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MappedList : IList<int>, ICollection<int>, IEnumerable<int>, IEnumerable
	{
		public MappedList(IList<int> list, int[] map)
		{
			this.BaseList = list;
			this.MapF = ((int v) => map[v]);
		}

		public int this[int index]
		{
			get
			{
				return this.MapF(this.BaseList[index]);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				return this.BaseList.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void Add(int item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, int item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(int item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public bool Contains(int item)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(int item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(int[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<int> GetEnumerator()
		{
			int N = this.BaseList.Count;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return this.MapF(this.BaseList[i]);
				num = i + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IList<int> BaseList;

		public Func<int, int> MapF = (int i) => i;
	}
}
