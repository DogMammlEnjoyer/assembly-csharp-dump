using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class UnityObjectUtils
	{
		public static void Destroy(Object obj, bool withUndo = false)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(obj);
			}
		}

		public static T ConvertUnityObjectToType<T>(Object objectIn) where T : class
		{
			T t = objectIn as T;
			if (t == null && objectIn != null)
			{
				GameObject gameObject = objectIn as GameObject;
				if (gameObject != null)
				{
					return gameObject.GetComponent<T>();
				}
				Component component = objectIn as Component;
				if (component != null)
				{
					t = component.GetComponent<T>();
				}
			}
			return t;
		}

		public static void RemoveDestroyedObjects<T>(List<T> list) where T : Object
		{
			List<T> collection = CollectionPool<List<T>, T>.GetCollection();
			foreach (T t in list)
			{
				if (t == null)
				{
					collection.Add(t);
				}
			}
			foreach (T item in collection)
			{
				list.Remove(item);
			}
			CollectionPool<List<T>, T>.RecycleCollection(collection);
		}

		public static void RemoveDestroyedKeys<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : Object
		{
			List<TKey> collection = CollectionPool<List<TKey>, TKey>.GetCollection();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
			{
				TKey key = keyValuePair.Key;
				if (key == null)
				{
					collection.Add(key);
				}
			}
			foreach (TKey key2 in collection)
			{
				dictionary.Remove(key2);
			}
			CollectionPool<List<TKey>, TKey>.RecycleCollection(collection);
		}
	}
}
