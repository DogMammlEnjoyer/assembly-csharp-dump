using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq
{
	[DebuggerDisplay("Key = {Key}")]
	[DebuggerTypeProxy(typeof(SystemLinq_GroupingDebugView<, >))]
	internal class Grouping<TKey, TElement> : IGrouping<!0, !1>, IEnumerable<!1>, IEnumerable, IList<TElement>, ICollection<TElement>
	{
		internal Grouping()
		{
		}

		internal void Add(TElement element)
		{
			if (this._elements.Length == this._count)
			{
				Array.Resize<TElement>(ref this._elements, checked(this._count * 2));
			}
			this._elements[this._count] = element;
			this._count++;
		}

		internal void Trim()
		{
			if (this._elements.Length != this._count)
			{
				Array.Resize<TElement>(ref this._elements, this._count);
			}
		}

		public IEnumerator<TElement> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this._count; i = num + 1)
			{
				yield return this._elements[i];
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public TKey Key
		{
			get
			{
				return this._key;
			}
		}

		int ICollection<!1>.Count
		{
			get
			{
				return this._count;
			}
		}

		bool ICollection<!1>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<!1>.Add(TElement item)
		{
			throw Error.NotSupported();
		}

		void ICollection<!1>.Clear()
		{
			throw Error.NotSupported();
		}

		bool ICollection<!1>.Contains(TElement item)
		{
			return Array.IndexOf<TElement>(this._elements, item, 0, this._count) >= 0;
		}

		void ICollection<!1>.CopyTo(TElement[] array, int arrayIndex)
		{
			Array.Copy(this._elements, 0, array, arrayIndex, this._count);
		}

		bool ICollection<!1>.Remove(TElement item)
		{
			throw Error.NotSupported();
		}

		int IList<!1>.IndexOf(TElement item)
		{
			return Array.IndexOf<TElement>(this._elements, item, 0, this._count);
		}

		void IList<!1>.Insert(int index, TElement item)
		{
			throw Error.NotSupported();
		}

		void IList<!1>.RemoveAt(int index)
		{
			throw Error.NotSupported();
		}

		TElement IList<!1>.this[int index]
		{
			get
			{
				if (index < 0 || index >= this._count)
				{
					throw Error.ArgumentOutOfRange("index");
				}
				return this._elements[index];
			}
			set
			{
				throw Error.NotSupported();
			}
		}

		internal TKey _key;

		internal int _hashCode;

		internal TElement[] _elements;

		internal int _count;

		internal Grouping<TKey, TElement> _hashNext;

		internal Grouping<TKey, TElement> _next;
	}
}
