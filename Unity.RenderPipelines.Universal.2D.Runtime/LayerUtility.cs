using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal static class LayerUtility
	{
		public static uint maxTextureCount { get; private set; }

		public static void InitializeBudget(uint maxTextureCount)
		{
			LayerUtility.maxTextureCount = math.max(4U, maxTextureCount);
		}

		private static bool CanBatchLightsInLayer(int layerIndex1, int layerIndex2, SortingLayer[] sortingLayers, ILight2DCullResult lightCullResult)
		{
			int id = sortingLayers[layerIndex1].id;
			int id2 = sortingLayers[layerIndex2].id;
			foreach (Light2D light2D in lightCullResult.visibleLights)
			{
				if (light2D.IsLitLayer(id) != light2D.IsLitLayer(id2))
				{
					return false;
				}
			}
			foreach (ShadowCasterGroup2D shadowCasterGroup2D in lightCullResult.visibleShadows)
			{
				foreach (ShadowCaster2D shadowCaster2D in shadowCasterGroup2D.GetShadowCasters())
				{
					if (shadowCaster2D.IsShadowedLayer(id) != shadowCaster2D.IsShadowedLayer(id2))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool CanBatchCameraSortingLayer(int startLayerIndex, SortingLayer[] sortingLayers, Renderer2DData rendererData)
		{
			if (rendererData.useCameraSortingLayerTexture)
			{
				short cameraSortingLayerBoundsIndex = Render2DLightingPass.GetCameraSortingLayerBoundsIndex(rendererData);
				return sortingLayers[startLayerIndex].value == (int)cameraSortingLayerBoundsIndex;
			}
			return false;
		}

		private static int FindUpperBoundInBatch(int startLayerIndex, SortingLayer[] sortingLayers, Renderer2DData rendererData)
		{
			if (LayerUtility.CanBatchCameraSortingLayer(startLayerIndex, sortingLayers, rendererData))
			{
				return startLayerIndex;
			}
			for (int i = startLayerIndex + 1; i < sortingLayers.Length; i++)
			{
				if (!LayerUtility.CanBatchLightsInLayer(startLayerIndex, i, sortingLayers, rendererData.lightCullResult))
				{
					return i - 1;
				}
				if (LayerUtility.CanBatchCameraSortingLayer(i, sortingLayers, rendererData))
				{
					return i;
				}
			}
			return sortingLayers.Length - 1;
		}

		private static void InitializeBatchInfos(SortingLayer[] cachedSortingLayers)
		{
			int num = cachedSortingLayers.Length;
			bool flag = LayerUtility.s_LayerBatches == null;
			if (LayerUtility.s_LayerBatches == null)
			{
				LayerUtility.s_LayerBatches = new LayerBatch[num];
			}
			if (flag)
			{
				for (int i = 0; i < LayerUtility.s_LayerBatches.Length; i++)
				{
					LayerUtility.s_LayerBatches[i].InitRTIds(i);
				}
			}
		}

		public static LayerBatch[] CalculateBatches(Renderer2DData rendererData, out int batchCount)
		{
			SortingLayer[] cachedSortingLayer = Light2DManager.GetCachedSortingLayer();
			LayerUtility.InitializeBatchInfos(cachedSortingLayer);
			bool flag = false;
			batchCount = 0;
			int num2;
			for (int i = 0; i < cachedSortingLayer.Length; i = num2 + 1)
			{
				int id = cachedSortingLayer[i].id;
				LayerBatch[] array = LayerUtility.s_LayerBatches;
				int num = batchCount;
				batchCount = num + 1;
				ref LayerBatch ptr = ref array[num];
				LightStats lightStatsByLayer = rendererData.lightCullResult.GetLightStatsByLayer(id, ref ptr);
				num2 = LayerUtility.FindUpperBoundInBatch(i, cachedSortingLayer, rendererData);
				short num3 = (short)cachedSortingLayer[i].value;
				short lowerBound = (i == 0) ? short.MinValue : num3;
				short num4 = (short)cachedSortingLayer[num2].value;
				short upperBound = (num2 == cachedSortingLayer.Length - 1) ? short.MaxValue : num4;
				SortingLayerRange layerRange = new SortingLayerRange(lowerBound, upperBound);
				ptr.startLayerID = id;
				ptr.endLayerValue = (int)num4;
				ptr.layerRange = layerRange;
				ptr.lightStats = lightStatsByLayer;
				flag |= ptr.lightStats.useNormalMap;
			}
			for (int j = 0; j < batchCount; j++)
			{
				LayerBatch[] array2 = LayerUtility.s_LayerBatches;
				int num5 = j;
				bool flag2 = SpriteMaskUtility.HasSpriteMaskInLayerRange(array2[num5].layerRange);
				array2[num5].useNormals = (array2[num5].lightStats.useNormalMap || (flag && flag2));
			}
			LayerUtility.SetupActiveBlendStyles();
			return LayerUtility.s_LayerBatches;
		}

		public static void GetFilterSettings(Renderer2DData rendererData, ref LayerBatch layerBatch, out FilteringSettings filterSettings)
		{
			filterSettings = FilteringSettings.defaultValue;
			filterSettings.renderQueueRange = RenderQueueRange.all;
			filterSettings.layerMask = rendererData.layerMask;
			filterSettings.renderingLayerMask = uint.MaxValue;
			filterSettings.sortingLayerRange = layerBatch.layerRange;
		}

		private static void SetupActiveBlendStyles()
		{
			for (int i = 0; i < LayerUtility.s_LayerBatches.Length; i++)
			{
				ref LayerBatch ptr = ref LayerUtility.s_LayerBatches[i];
				int num = 0;
				for (int j = 0; j < RendererLighting.k_ShapeLightTextureIDs.Length; j++)
				{
					uint num2 = 1U << j;
					if ((ptr.lightStats.blendStylesUsed & num2) > 0U)
					{
						num++;
					}
				}
				if (ptr.activeBlendStylesIndices == null || ptr.activeBlendStylesIndices.Length != num)
				{
					ptr.activeBlendStylesIndices = new int[num];
				}
				int num3 = 0;
				for (int k = 0; k < RendererLighting.k_ShapeLightTextureIDs.Length; k++)
				{
					uint num4 = 1U << k;
					if ((ptr.lightStats.blendStylesUsed & num4) > 0U)
					{
						ptr.activeBlendStylesIndices[num3++] = k;
					}
				}
			}
		}

		private static LayerBatch[] s_LayerBatches;
	}
}
