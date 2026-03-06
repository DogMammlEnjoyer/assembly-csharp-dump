using System;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class GameObjectExtensions
	{
		public static void SetHideFlagsRecursively(this GameObject gameObject, HideFlags hideFlags)
		{
			gameObject.hideFlags = hideFlags;
			foreach (object obj in gameObject.transform)
			{
				((Transform)obj).gameObject.SetHideFlagsRecursively(hideFlags);
			}
		}

		public static void AddToHideFlagsRecursively(this GameObject gameObject, HideFlags hideFlags)
		{
			gameObject.hideFlags |= hideFlags;
			foreach (object obj in gameObject.transform)
			{
				((Transform)obj).gameObject.AddToHideFlagsRecursively(hideFlags);
			}
		}

		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			foreach (object obj in gameObject.transform)
			{
				((Transform)obj).gameObject.SetLayerRecursively(layer);
			}
		}

		public static void SetLayerAndAddToHideFlagsRecursively(this GameObject gameObject, int layer, HideFlags hideFlags)
		{
			gameObject.layer = layer;
			gameObject.hideFlags |= hideFlags;
			foreach (object obj in gameObject.transform)
			{
				((Transform)obj).gameObject.SetLayerAndAddToHideFlagsRecursively(layer, hideFlags);
			}
		}

		public static void SetLayerAndHideFlagsRecursively(this GameObject gameObject, int layer, HideFlags hideFlags)
		{
			gameObject.layer = layer;
			gameObject.hideFlags = hideFlags;
			foreach (object obj in gameObject.transform)
			{
				((Transform)obj).gameObject.SetLayerAndHideFlagsRecursively(layer, hideFlags);
			}
		}

		public static void SetRunInEditModeRecursively(this GameObject gameObject, bool enabled)
		{
		}
	}
}
