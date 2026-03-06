using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct ReadOnlyListAdaptor<[Nullable(2)] T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
	{
		public ReadOnlyListAdaptor(IList<T> list)
		{
			this._list = list;
		}

		public T this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly IList<T> _list;
	}
}
