using System;
using System.Collections.Generic;

namespace Unity.XR.CoreUtils
{
	public static class CollectionPool<TCollection, TValue> where TCollection : ICollection<TValue>, new()
	{
		public static TCollection GetCollection()
		{
			if (CollectionPool<TCollection, TValue>.k_CollectionQueue.Count <= 0)
			{
				return Activator.CreateInstance<TCollection>();
			}
			return CollectionPool<TCollection, TValue>.k_CollectionQueue.Dequeue();
		}

		public static void RecycleCollection(TCollection collection)
		{
			collection.Clear();
			CollectionPool<TCollection, TValue>.k_CollectionQueue.Enqueue(collection);
		}

		private static readonly Queue<TCollection> k_CollectionQueue = new Queue<TCollection>();
	}
}
