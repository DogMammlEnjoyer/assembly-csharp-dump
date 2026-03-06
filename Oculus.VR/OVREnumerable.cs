using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal readonly struct OVREnumerable<T> : IEnumerable<!0>, IEnumerable
{
	public OVREnumerable(IEnumerable<T> enumerable)
	{
		this._enumerable = enumerable;
	}

	public OVREnumerable<T>.Enumerator GetEnumerator()
	{
		return new OVREnumerable<T>.Enumerator(this._enumerable);
	}

	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	public bool TryGetCount(out int count)
	{
		int? count2 = this.Count;
		count = count2.GetValueOrDefault();
		return count2 != null;
	}

	public int? Count
	{
		get
		{
			IEnumerable<T> enumerable = this._enumerable;
			int? result;
			if (enumerable != null)
			{
				ICollection collection = enumerable as ICollection;
				if (collection == null)
				{
					ICollection<T> collection2 = enumerable as ICollection<T>;
					if (collection2 == null)
					{
						IReadOnlyCollection<T> readOnlyCollection = enumerable as IReadOnlyCollection<T>;
						if (readOnlyCollection == null)
						{
							result = null;
						}
						else
						{
							result = new int?(readOnlyCollection.Count);
						}
					}
					else
					{
						result = new int?(collection2.Count);
					}
				}
				else
				{
					result = new int?(collection.Count);
				}
			}
			else
			{
				result = new int?(0);
			}
			return result;
		}
	}

	[Obsolete("This method may enumerate the collection. Consider Count or TryGetCount instead.")]
	public int GetCount()
	{
		int num;
		if (!this.TryGetCount(out num))
		{
			num = 0;
			foreach (T t in this._enumerable)
			{
				num++;
			}
		}
		return num;
	}

	private readonly IEnumerable<T> _enumerable;

	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		public Enumerator(IEnumerable<T> enumerable)
		{
			this._setEnumerator = default(HashSet<T>.Enumerator);
			this._queueEnumerator = default(Queue<T>.Enumerator);
			this._listEnumerator = default(List<T>.Enumerator);
			this._enumerator = null;
			this._readOnlyList = null;
			this._listIndex = -1;
			this._listCount = 0;
			if (enumerable == null)
			{
				this._type = OVREnumerable<T>.Enumerator.CollectionType.None;
				return;
			}
			List<T> list = enumerable as List<T>;
			if (list != null)
			{
				this._listEnumerator = list.GetEnumerator();
				this._type = OVREnumerable<T>.Enumerator.CollectionType.List;
				return;
			}
			IReadOnlyList<T> readOnlyList = enumerable as IReadOnlyList<T>;
			if (readOnlyList != null)
			{
				this._readOnlyList = readOnlyList;
				this._listCount = readOnlyList.Count;
				this._type = OVREnumerable<T>.Enumerator.CollectionType.ReadOnlyList;
				return;
			}
			HashSet<T> hashSet = enumerable as HashSet<T>;
			if (hashSet != null)
			{
				this._setEnumerator = hashSet.GetEnumerator();
				this._type = OVREnumerable<T>.Enumerator.CollectionType.Set;
				return;
			}
			Queue<T> queue = enumerable as Queue<T>;
			if (queue == null)
			{
				this._enumerator = enumerable.GetEnumerator();
				this._type = OVREnumerable<T>.Enumerator.CollectionType.Enumerable;
				return;
			}
			this._queueEnumerator = queue.GetEnumerator();
			this._type = OVREnumerable<T>.Enumerator.CollectionType.Queue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			bool result;
			switch (this._type)
			{
			case OVREnumerable<T>.Enumerator.CollectionType.None:
				result = false;
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.ReadOnlyList:
				result = this.MoveNextReadOnlyList();
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.List:
				result = this._listEnumerator.MoveNext();
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.Set:
				result = this._setEnumerator.MoveNext();
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.Queue:
				result = this._queueEnumerator.MoveNext();
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.Enumerable:
				result = this._enumerator.MoveNext();
				break;
			default:
				throw new InvalidOperationException(string.Format("Unsupported collection type {0}.", this._type));
			}
			return result;
		}

		private bool MoveNextReadOnlyList()
		{
			this.ValidateAndThrow();
			int num = this._listIndex + 1;
			this._listIndex = num;
			return num < this._listCount;
		}

		public void Reset()
		{
			switch (this._type)
			{
			case OVREnumerable<T>.Enumerator.CollectionType.ReadOnlyList:
				this.ValidateAndThrow();
				this._listIndex = -1;
				return;
			case OVREnumerable<T>.Enumerator.CollectionType.List:
			case OVREnumerable<T>.Enumerator.CollectionType.Set:
			case OVREnumerable<T>.Enumerator.CollectionType.Queue:
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.Enumerable:
				this._enumerator.Reset();
				break;
			default:
				return;
			}
		}

		public T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				T result;
				switch (this._type)
				{
				case OVREnumerable<T>.Enumerator.CollectionType.ReadOnlyList:
					result = this._readOnlyList[this._listIndex];
					break;
				case OVREnumerable<T>.Enumerator.CollectionType.List:
					result = this._listEnumerator.Current;
					break;
				case OVREnumerable<T>.Enumerator.CollectionType.Set:
					result = this._setEnumerator.Current;
					break;
				case OVREnumerable<T>.Enumerator.CollectionType.Queue:
					result = this._queueEnumerator.Current;
					break;
				case OVREnumerable<T>.Enumerator.CollectionType.Enumerable:
					result = this._enumerator.Current;
					break;
				default:
					throw new InvalidOperationException(string.Format("Unsupported collection type {0}.", this._type));
				}
				return result;
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
			switch (this._type)
			{
			case OVREnumerable<T>.Enumerator.CollectionType.ReadOnlyList:
				break;
			case OVREnumerable<T>.Enumerator.CollectionType.List:
				this._listEnumerator.Dispose();
				return;
			case OVREnumerable<T>.Enumerator.CollectionType.Set:
				this._setEnumerator.Dispose();
				return;
			case OVREnumerable<T>.Enumerator.CollectionType.Queue:
				this._queueEnumerator.Dispose();
				return;
			case OVREnumerable<T>.Enumerator.CollectionType.Enumerable:
				this._enumerator.Dispose();
				break;
			default:
				return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ValidateAndThrow()
		{
			if (this._listCount != this._readOnlyList.Count)
			{
				throw new InvalidOperationException("The list changed length during enumeration.");
			}
		}

		private int _listIndex;

		private readonly OVREnumerable<T>.Enumerator.CollectionType _type;

		private readonly int _listCount;

		private readonly IEnumerator<T> _enumerator;

		private readonly IReadOnlyList<T> _readOnlyList;

		private HashSet<T>.Enumerator _setEnumerator;

		private Queue<T>.Enumerator _queueEnumerator;

		private List<T>.Enumerator _listEnumerator;

		private enum CollectionType
		{
			None,
			ReadOnlyList,
			List,
			Set,
			Queue,
			Enumerable
		}
	}
}
