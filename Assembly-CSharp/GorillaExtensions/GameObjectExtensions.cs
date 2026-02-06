using System;
using UnityEngine;

namespace GorillaExtensions
{
	public static class GameObjectExtensions
	{
		public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : MonoBehaviour
		{
			while (!obj.TryGetComponent<T>(out component))
			{
				obj = ((obj.transform.parent != null) ? obj.transform.parent.gameObject : null);
				if (!(obj != null))
				{
					return false;
				}
			}
			return true;
		}
	}
}
