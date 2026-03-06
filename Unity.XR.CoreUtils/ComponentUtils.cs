using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class ComponentUtils<T>
	{
		public static T GetComponent(GameObject gameObject)
		{
			T result = default(T);
			gameObject.GetComponents<T>(ComponentUtils<T>.k_RetrievalList);
			if (ComponentUtils<T>.k_RetrievalList.Count > 0)
			{
				result = ComponentUtils<T>.k_RetrievalList[0];
			}
			return result;
		}

		public static T GetComponentInChildren(GameObject gameObject)
		{
			T result = default(T);
			gameObject.GetComponentsInChildren<T>(ComponentUtils<T>.k_RetrievalList);
			if (ComponentUtils<T>.k_RetrievalList.Count > 0)
			{
				result = ComponentUtils<T>.k_RetrievalList[0];
			}
			return result;
		}

		private static readonly List<T> k_RetrievalList = new List<T>();
	}
}
