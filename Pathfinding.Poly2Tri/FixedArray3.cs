using System;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Poly2Tri
{
	public struct FixedArray3<T> : IEnumerable, IEnumerable<T> where T : class
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this._0;
				case 1:
					return this._1;
				case 2:
					return this._2;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this._0 = value;
					break;
				case 1:
					this._1 = value;
					break;
				case 2:
					this._2 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		public bool Contains(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		public void Clear()
		{
			this._0 = (this._1 = (this._2 = (T)((object)null)));
		}

		public void Clear(T value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					this[i] = (T)((object)null);
				}
			}
		}

		private IEnumerable<T> Enumerate()
		{
			for (int i = 0; i < 3; i++)
			{
				yield return this[i];
			}
			yield break;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.Enumerate().GetEnumerator();
		}

		public T _0;

		public T _1;

		public T _2;
	}
}
