using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : SerializableDictionary, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable, ISerializationCallbackReceiver
	{
		private Dictionary<TKey, TValue> Inner
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = this._dictionary == null;
				if (flag)
				{
					this._dictionary = this.CreateDictionary();
				}
				return this._dictionary;
			}
		}

		private ICollection<KeyValuePair<TKey, TValue>> DictionaryAsCollection
		{
			get
			{
				return this.Inner;
			}
		}

		public static SerializableDictionary<TKey, TValue> Wrap(Dictionary<TKey, TValue> dictionary)
		{
			return new SerializableDictionary<TKey, TValue>
			{
				_dictionary = dictionary
			};
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.Inner[key];
			}
			set
			{
				this.Inner[key] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.Inner.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(TKey key, TValue value)
		{
			this.Inner.Add(key, value);
		}

		public virtual void Clear()
		{
			List<ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>> duplicatesAndNulls = this._duplicatesAndNulls;
			if (duplicatesAndNulls != null)
			{
				duplicatesAndNulls.Clear();
			}
			this.Inner.Clear();
		}

		public bool ContainsKey(TKey key)
		{
			return this.Inner.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			return this.Inner.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.Inner.TryGetValue(key, out value);
		}

		public Dictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				return this.Inner.Keys;
			}
		}

		public Dictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				return this.Inner.Values;
			}
		}

		public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return this.Inner.GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.Inner.GetEnumerator();
		}

		ICollection<TKey> IDictionary<!0, !1>.Keys
		{
			get
			{
				return this.Inner.Keys;
			}
		}

		ICollection<TValue> IDictionary<!0, !1>.Values
		{
			get
			{
				return this.Inner.Values;
			}
		}

		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> item)
		{
			this.DictionaryAsCollection.Add(item);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.DictionaryAsCollection.Contains(item);
		}

		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.DictionaryAsCollection.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			return this.DictionaryAsCollection.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Inner.GetEnumerator();
		}

		private Dictionary<TKey, TValue> CreateDictionary()
		{
			List<ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>> duplicatesAndNulls = this._duplicatesAndNulls;
			if (duplicatesAndNulls != null)
			{
				duplicatesAndNulls.Clear();
			}
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			bool flag = !typeof(TKey).IsValueType;
			bool flag2 = this._items != null;
			if (flag2)
			{
				for (int i = 0; i < this._items.Length; i++)
				{
					SerializableDictionary<TKey, TValue>.Entry entry = this._items[i];
					TKey key = entry.Key;
					TValue value = entry.Value;
					TKey tkey = key;
					bool flag3 = (flag && dictionary.Comparer.Equals(tkey, default(TKey))) || dictionary.ContainsKey(tkey);
					if (flag3)
					{
						bool flag4 = this._duplicatesAndNulls == null;
						if (flag4)
						{
							this._duplicatesAndNulls = new List<ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>>();
						}
						this._duplicatesAndNulls.Add(new ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>(entry, i));
					}
					else
					{
						dictionary.Add(tkey, value);
					}
				}
			}
			return dictionary;
		}

		public void Reset()
		{
			this._dictionary = null;
		}

		public void Store()
		{
			int count = this.Count;
			List<ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>> duplicatesAndNulls = this._duplicatesAndNulls;
			int num = count + ((duplicatesAndNulls != null) ? duplicatesAndNulls.Count : 0);
			SerializableDictionary<TKey, TValue>.Entry[] items = this._items;
			bool flag = items == null || items.Length != num;
			if (flag)
			{
				Array.Resize<SerializableDictionary<TKey, TValue>.Entry>(ref this._items, num);
			}
			int num2 = 0;
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				this._items[num2] = new SerializableDictionary<TKey, TValue>.Entry
				{
					Key = keyValuePair.Key,
					Value = keyValuePair.Value
				};
				num2++;
			}
			bool flag2 = this._duplicatesAndNulls != null;
			if (flag2)
			{
				foreach (ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int> valueTuple in this._duplicatesAndNulls)
				{
					for (int i = num2 - 1; i > valueTuple.Item2; i--)
					{
						this._items[i] = this._items[i - 1];
					}
					this._items[valueTuple.Item2] = valueTuple.Item1;
					num2++;
				}
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.Reset();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			this.Store();
		}

		public const string ItemsPropertyPath = "_items";

		public const string EntryKeyPropertyPath = "Key";

		[SerializeField]
		private SerializableDictionary<TKey, TValue>.Entry[] _items;

		[NonSerialized]
		private List<ValueTuple<SerializableDictionary<TKey, TValue>.Entry, int>> _duplicatesAndNulls;

		[NonSerialized]
		private Dictionary<TKey, TValue> _dictionary;

		[Serializable]
		private struct Entry
		{
			public TKey Key;

			public TValue Value;
		}
	}
}
