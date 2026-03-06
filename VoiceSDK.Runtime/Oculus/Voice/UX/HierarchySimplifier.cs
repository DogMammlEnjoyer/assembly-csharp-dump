using System;
using UnityEngine;

namespace Oculus.Voice.UX
{
	public class HierarchySimplifier : MonoBehaviour
	{
		public static void HideSubObjects(GameObject obj, bool hideObjects)
		{
			HierarchySimplifier[] componentsInChildren = obj.GetComponentsInChildren<HierarchySimplifier>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				HierarchySimplifier.ToggleShowInHierarchyFlag(componentsInChildren[i].gameObject, hideObjects);
			}
		}

		private void OnValidate()
		{
			HierarchySimplifier.ToggleShowInHierarchyFlag(base.gameObject, this.hideByDefault);
		}

		public static void ToggleShowInHierarchyFlag(GameObject obj, bool hideObject)
		{
			obj.hideFlags = (hideObject ? (obj.hideFlags | HideFlags.HideInHierarchy) : (obj.hideFlags & ~HideFlags.HideInHierarchy));
		}

		[Tooltip("Whether to hide the object on startup, by default.")]
		[SerializeField]
		public bool hideByDefault = true;
	}
}
