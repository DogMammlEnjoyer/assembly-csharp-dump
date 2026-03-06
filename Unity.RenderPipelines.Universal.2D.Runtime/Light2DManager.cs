using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal static class Light2DManager
	{
		public static List<Light2D> lights { get; } = new List<Light2D>();

		internal static void Initialize()
		{
		}

		internal static void Dispose()
		{
		}

		public static void RegisterLight(Light2D light)
		{
			Light2DManager.lights.Add(light);
			Light2DManager.ErrorIfDuplicateGlobalLight(light);
		}

		public static void DeregisterLight(Light2D light)
		{
			Light2DManager.lights.Remove(light);
		}

		public static void ErrorIfDuplicateGlobalLight(Light2D light)
		{
			if (light.lightType != Light2D.LightType.Global)
			{
				return;
			}
			foreach (int num in light.targetSortingLayers)
			{
				if (Light2DManager.ContainsDuplicateGlobalLight(num, light.blendStyleIndex))
				{
					Debug.LogError("More than one global light on layer " + SortingLayer.IDToName(num) + " for light blend style index " + light.blendStyleIndex.ToString());
				}
			}
		}

		public static bool GetGlobalColor(int sortingLayerIndex, int blendStyleIndex, out Color color)
		{
			bool flag = false;
			color = Color.black;
			foreach (Light2D light2D in Light2DManager.lights)
			{
				if (light2D.lightType == Light2D.LightType.Global && light2D.blendStyleIndex == blendStyleIndex && light2D.IsLitLayer(sortingLayerIndex))
				{
					if (true)
					{
						color = light2D.color * light2D.intensity;
						return true;
					}
					if (!flag)
					{
						color = light2D.color * light2D.intensity;
						flag = true;
					}
				}
			}
			return flag;
		}

		private static bool ContainsDuplicateGlobalLight(int sortingLayerIndex, int blendStyleIndex)
		{
			int num = 0;
			foreach (Light2D light2D in Light2DManager.lights)
			{
				if (light2D.lightType == Light2D.LightType.Global && light2D.blendStyleIndex == blendStyleIndex && light2D.IsLitLayer(sortingLayerIndex))
				{
					if (num > 0)
					{
						return true;
					}
					num++;
				}
			}
			return false;
		}

		public static SortingLayer[] GetCachedSortingLayer()
		{
			if (Light2DManager.s_SortingLayers == null)
			{
				Light2DManager.s_SortingLayers = SortingLayer.layers;
			}
			return Light2DManager.s_SortingLayers;
		}

		private static SortingLayer[] s_SortingLayers;
	}
}
