using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class ComponentUtils
	{
		public static T GetOrAddIf<T>(GameObject gameObject, bool add) where T : Component
		{
			T t = gameObject.GetComponent<T>();
			if (add && t == null)
			{
				t = gameObject.AddComponent<T>();
			}
			return t;
		}
	}
}
