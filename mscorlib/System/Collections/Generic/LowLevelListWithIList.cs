using System;

namespace System.Collections.Generic
{
	internal sealed class LowLevelListWithIList<T> : LowLevelList<T>, IList<!0>, ICollection<!0>, IEnumerable<!0>, IEnumerable
	{
		public LowLevelListWithIList()
		{
		}

		public LowLevelListWithIList(int capacity) : base(capacity)
		{
		}

		public LowLevelListWithIList(IEnumerable<T> collection) : base(collection)
		{
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return new LowLevelListWithIList<T>.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new LowLevelListWithIList<T>.Enumerator(this);
		}

		private struct Enumerator : IEnumerator<!0>, IDisposable, IEnumerator
		{
			internal Enumerator(LowLevelListWithIList<T> list)
			{
				this._list = list;
				this._index = 0;
				this._version = list._version;
				this._current = default(T);
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				LowLevelListWithIList<T> list = this._list;
				if (this._version == list._version && this._index < list._size)
				{
					this._current = list._items[this._index];
					this._index++;
					return true;
				}
				return this.MoveNextRare();
			}

			private bool MoveNextRare()
			{
				if (this._version != this._list._version)
				{
					throw new InvalidOperationException();
				}
				this._index = this._list._size + 1;
				this._current = default(T);
				return false;
			}

			public T Current
			{
				get
				{
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index == this._list._size + 1)
					{
						throw new InvalidOperationException();
					}
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				if (this._version != this._list._version)
				{
					throw new InvalidOperationException();
				}
				this._index = 0;
				this._current = default(T);
			}

			private LowLevelListWithIList<T> _list;

			private int _index;

			private int _version;

			private T _current;
		}
	}
}
