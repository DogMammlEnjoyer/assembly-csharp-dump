using System;
using System.Collections.Generic;
using System.Threading;

namespace Valve.Newtonsoft.Json.Utilities
{
	internal class ThreadSafeStore<TKey, TValue>
	{
		public ThreadSafeStore(Func<TKey, TValue> creator)
		{
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			this._creator = creator;
			this._store = new Dictionary<TKey, TValue>();
		}

		public TValue Get(TKey key)
		{
			TValue result;
			if (!this._store.TryGetValue(key, out result))
			{
				return this.AddValue(key);
			}
			return result;
		}

		private TValue AddValue(TKey key)
		{
			TValue tvalue = this._creator(key);
			object @lock = this._lock;
			TValue result2;
			lock (@lock)
			{
				if (this._store == null)
				{
					this._store = new Dictionary<TKey, TValue>();
					this._store[key] = tvalue;
				}
				else
				{
					TValue result;
					if (this._store.TryGetValue(key, out result))
					{
						return result;
					}
					Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._store);
					dictionary[key] = tvalue;
					Thread.MemoryBarrier();
					this._store = dictionary;
				}
				result2 = tvalue;
			}
			return result2;
		}

		private readonly object _lock = new object();

		private Dictionary<TKey, TValue> _store;

		private readonly Func<TKey, TValue> _creator;
	}
}
