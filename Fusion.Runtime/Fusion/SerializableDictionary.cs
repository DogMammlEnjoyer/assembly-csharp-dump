using System;

namespace Fusion
{
	public abstract class SerializableDictionary
	{
		public static SerializableDictionary<TKey, TValue> Create<TKey, TValue>()
		{
			return new SerializableDictionary<TKey, TValue>();
		}
	}
}
