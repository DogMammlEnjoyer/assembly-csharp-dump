using System;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Poly2Tri
{
	public struct FixedBitArray3 : IEnumerable, IEnumerable<bool>
	{
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool this[int index]
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

		public bool Contains(bool value)
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

		public int IndexOf(bool value)
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
			this._0 = (this._1 = (this._2 = false));
		}

		public void Clear(bool value)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this[i] == value)
				{
					this[i] = false;
				}
			}
		}

		private IEnumerable<bool> Enumerate()
		{
			for (int i = 0; i < 3; i++)
			{
				yield return this[i];
			}
			yield break;
		}

		public IEnumerator<bool> GetEnumerator()
		{
			return this.Enumerate().GetEnumerator();
		}

		public bool _0;

		public bool _1;

		public bool _2;
	}
}
