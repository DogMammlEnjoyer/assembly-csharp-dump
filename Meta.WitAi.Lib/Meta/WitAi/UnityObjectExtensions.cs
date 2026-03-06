using System;
using UnityEngine;

namespace Meta.WitAi
{
	public static class UnityObjectExtensions
	{
		public static void DestroySafely(this Object unityObject)
		{
			if (!unityObject)
			{
				return;
			}
			Object.Destroy(unityObject);
		}

		public static T GetOrAddComponent<T>(this GameObject unityObject) where T : Component
		{
			if (!unityObject)
			{
				return default(T);
			}
			T component = unityObject.GetComponent<T>();
			if (component)
			{
				return component;
			}
			return unityObject.AddComponent<T>();
		}
	}
}
