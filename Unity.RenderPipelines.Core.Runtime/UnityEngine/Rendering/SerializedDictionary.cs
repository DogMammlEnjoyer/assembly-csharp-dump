using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(SerializedDictionaryDebugView<, >))]
	[Serializable]
	public class SerializedDictionary<K, V> : SerializedDictionary<K, V, K, V>
	{
		public override K SerializeKey(K key)
		{
			return key;
		}

		public override V SerializeValue(V val)
		{
			return val;
		}

		public override K DeserializeKey(K key)
		{
			return key;
		}

		public override V DeserializeValue(V val)
		{
			return val;
		}
	}
}
