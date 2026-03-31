using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct OneOrMore<TValue, TList> : IReadOnlyList<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyCollection<TValue> where TList : IReadOnlyList<TValue>
	{
		public int Count
		{
			get
			{
				if (!this.m_IsSingle)
				{
					TList multiple = this.m_Multiple;
					return multiple.Count;
				}
				return 1;
			}
		}

		public TValue this[int index]
		{
			get
			{
				if (!this.m_IsSingle)
				{
					TList multiple = this.m_Multiple;
					return multiple[index];
				}
				if (index < 0 || index > 1)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.m_Single;
			}
		}

		public OneOrMore(TValue single)
		{
			this.m_IsSingle = true;
			this.m_Single = single;
			this.m_Multiple = default(TList);
		}

		public OneOrMore(TList multiple)
		{
			this.m_IsSingle = false;
			this.m_Single = default(TValue);
			this.m_Multiple = multiple;
		}

		public static implicit operator OneOrMore<TValue, TList>(TValue single)
		{
			return new OneOrMore<TValue, TList>(single);
		}

		public static implicit operator OneOrMore<TValue, TList>(TList multiple)
		{
			return new OneOrMore<TValue, TList>(multiple);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return new OneOrMore<TValue, TList>.Enumerator
			{
				m_List = this
			};
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly bool m_IsSingle;

		private readonly TValue m_Single;

		private readonly TList m_Multiple;

		private class Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
		{
			public bool MoveNext()
			{
				this.m_Index++;
				return this.m_Index < this.m_List.Count;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public TValue Current
			{
				get
				{
					return this.m_List[this.m_Index];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			internal int m_Index = -1;

			internal OneOrMore<TValue, TList> m_List;
		}
	}
}
