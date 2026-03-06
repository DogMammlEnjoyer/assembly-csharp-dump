using System;
using System.Collections;

namespace System.Xml.Serialization
{
	internal class TempAssemblyCache
	{
		internal TempAssembly this[string ns, object o]
		{
			get
			{
				return (TempAssembly)this.cache[new TempAssemblyCacheKey(ns, o)];
			}
		}

		internal void Add(string ns, object o, TempAssembly assembly)
		{
			TempAssemblyCacheKey key = new TempAssemblyCacheKey(ns, o);
			lock (this)
			{
				if (this.cache[key] != assembly)
				{
					Hashtable hashtable = new Hashtable();
					foreach (object key2 in this.cache.Keys)
					{
						hashtable.Add(key2, this.cache[key2]);
					}
					this.cache = hashtable;
					this.cache[key] = assembly;
				}
			}
		}

		private Hashtable cache = new Hashtable();
	}
}
