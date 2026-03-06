using System;
using System.Collections.Generic;

namespace Meta.WitAi.Utilities
{
	public class DictionaryList<T, U>
	{
		public void Add(T key, U value)
		{
			List<U> list;
			if (!this.TryGetValue(key, out list))
			{
				this.dictionary[key] = list;
			}
			list.Add(value);
		}

		public void RemoveAt(T key, int index)
		{
			List<U> list;
			if (this.TryGetValue(key, out list))
			{
				list.RemoveAt(index);
			}
		}

		public void Remove(T key, U value)
		{
			List<U> list;
			if (this.TryGetValue(key, out list))
			{
				list.Remove(value);
			}
		}

		public List<U> this[T key]
		{
			get
			{
				List<U> list;
				if (!this.TryGetValue(key, out list))
				{
					list = new List<U>();
					this.dictionary[key] = list;
				}
				return list;
			}
			set
			{
				this.dictionary[key] = value;
			}
		}

		public bool TryGetValue(T key, out List<U> values)
		{
			if (!this.dictionary.TryGetValue(key, out values))
			{
				values = new List<U>();
				return false;
			}
			return true;
		}

		private Dictionary<T, List<U>> dictionary = new Dictionary<T, List<U>>();
	}
}
