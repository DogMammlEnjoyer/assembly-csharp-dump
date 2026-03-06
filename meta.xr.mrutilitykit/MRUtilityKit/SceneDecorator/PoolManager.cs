using System;
using System.Collections.Generic;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class PoolManager<K, P> where K : class where P : Pool<K>
	{
		public void AddPool(K primitive, P pool)
		{
			this.pools.Add(primitive, pool);
		}

		public bool ContainsPool(K primitive)
		{
			return this.pools.ContainsKey(primitive);
		}

		public P GetPool(K primitive)
		{
			P result;
			this.pools.TryGetValue(primitive, out result);
			return result;
		}

		private Dictionary<K, P> pools = new Dictionary<K, P>();
	}
}
