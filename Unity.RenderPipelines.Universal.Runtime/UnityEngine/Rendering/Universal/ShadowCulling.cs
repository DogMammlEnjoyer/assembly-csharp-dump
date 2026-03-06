using System;
using Unity.Collections;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	internal static class ShadowCulling
	{
		public static NativeArray<URPLightShadowCullingInfos> CullShadowCasters(ref ScriptableRenderContext context, UniversalShadowData shadowData, ref AdditionalLightsShadowAtlasLayout shadowAtlasLayout, ref CullingResults cullResults)
		{
			ShadowCastersCullingInfos infos;
			NativeArray<URPLightShadowCullingInfos> result;
			ShadowCulling.ComputeShadowCasterCullingInfos(shadowData, ref shadowAtlasLayout, ref cullResults, out infos, out result);
			context.CullShadowCasters(cullResults, infos);
			return result;
		}

		private static void ComputeShadowCasterCullingInfos(UniversalShadowData shadowData, ref AdditionalLightsShadowAtlasLayout shadowAtlasLayout, ref CullingResults cullingResults, out ShadowCastersCullingInfos shadowCullingInfos, out NativeArray<URPLightShadowCullingInfos> urpVisibleLightsShadowCullingInfos)
		{
			using (new ProfilingScope(ShadowCulling.computeShadowCasterCullingInfosMarker))
			{
				NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
				NativeArray<ShadowSplitData> nativeArray = new NativeArray<ShadowSplitData>(visibleLights.Length * 6, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<LightShadowCasterCullingInfo> perLightInfos = new NativeArray<LightShadowCasterCullingInfo>(visibleLights.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				urpVisibleLightsShadowCullingInfos = new NativeArray<URPLightShadowCullingInfos>(visibleLights.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				int num = 0;
				int num2 = 0;
				int i = 0;
				while (i < visibleLights.Length)
				{
					ref VisibleLight ptr = ref cullingResults.visibleLights.UnsafeElementAt(i);
					LightType lightType = ptr.lightType;
					NativeArray<ShadowSliceData> slices = default(NativeArray<ShadowSliceData>);
					uint num3 = 0U;
					if (lightType == LightType.Directional)
					{
						if (shadowData.supportsMainLightShadows)
						{
							int mainLightShadowCascadesCount = shadowData.mainLightShadowCascadesCount;
							int mainLightRenderTargetWidth = shadowData.mainLightRenderTargetWidth;
							int mainLightRenderTargetHeight = shadowData.mainLightRenderTargetHeight;
							int mainLightShadowResolution = shadowData.mainLightShadowResolution;
							slices = new NativeArray<ShadowSliceData>(mainLightShadowCascadesCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
							num3 = 0U;
							for (int j = 0; j < mainLightShadowCascadesCount; j++)
							{
								ShadowSliceData shadowSliceData = default(ShadowSliceData);
								Vector4 vector;
								if (ShadowUtils.ExtractDirectionalLightMatrix(ref cullingResults, shadowData, i, j, mainLightRenderTargetWidth, mainLightRenderTargetHeight, mainLightShadowResolution, ptr.light.shadowNearPlane, out vector, out shadowSliceData))
								{
									num3 |= 1U << j;
								}
								slices[j] = shadowSliceData;
								nativeArray[num2 + j] = shadowSliceData.splitData;
							}
							goto IL_26C;
						}
					}
					else if (lightType == LightType.Point)
					{
						if (shadowData.supportsAdditionalLightShadows && shadowAtlasLayout.HasSpaceForLight(i))
						{
							int punctualLightShadowSlicesCount = ShadowUtils.GetPunctualLightShadowSlicesCount(lightType);
							int allocatedResolution = (int)shadowAtlasLayout.GetSliceShadowResolutionRequest(i, 0).allocatedResolution;
							bool shadowFiltering = ptr.light.shadows == LightShadows.Soft;
							float pointLightShadowFrustumFovBiasInDegrees = AdditionalLightsShadowCasterPass.GetPointLightShadowFrustumFovBiasInDegrees(allocatedResolution, shadowFiltering);
							slices = new NativeArray<ShadowSliceData>(punctualLightShadowSlicesCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
							num3 = 0U;
							for (int k = 0; k < punctualLightShadowSlicesCount; k++)
							{
								ShadowSliceData shadowSliceData2 = default(ShadowSliceData);
								if (ShadowUtils.ExtractPointLightMatrix(ref cullingResults, shadowData, i, (CubemapFace)k, pointLightShadowFrustumFovBiasInDegrees, out shadowSliceData2.shadowTransform, out shadowSliceData2.viewMatrix, out shadowSliceData2.projectionMatrix, out shadowSliceData2.splitData))
								{
									num3 |= 1U << k;
								}
								slices[k] = shadowSliceData2;
								nativeArray[num2 + k] = shadowSliceData2.splitData;
							}
							goto IL_26C;
						}
					}
					else
					{
						if (lightType != LightType.Spot)
						{
							goto IL_26C;
						}
						if (shadowData.supportsAdditionalLightShadows && shadowAtlasLayout.HasSpaceForLight(i))
						{
							slices = new NativeArray<ShadowSliceData>(1, Allocator.Temp, NativeArrayOptions.ClearMemory);
							num3 = 0U;
							ShadowSliceData shadowSliceData3 = default(ShadowSliceData);
							if (ShadowUtils.ExtractSpotLightMatrix(ref cullingResults, shadowData, i, out shadowSliceData3.shadowTransform, out shadowSliceData3.viewMatrix, out shadowSliceData3.projectionMatrix, out shadowSliceData3.splitData))
							{
								num3 |= 1U;
							}
							slices[0] = shadowSliceData3;
							nativeArray[num2] = shadowSliceData3.splitData;
							goto IL_26C;
						}
					}
					IL_2DF:
					i++;
					continue;
					IL_26C:
					urpVisibleLightsShadowCullingInfos[i] = new URPLightShadowCullingInfos
					{
						slices = slices,
						slicesValidMask = num3
					};
					perLightInfos[i] = new LightShadowCasterCullingInfo
					{
						splitRange = new RangeInt(num2, slices.Length),
						projectionType = ShadowCulling.GetCullingProjectionType(lightType)
					};
					num2 += slices.Length;
					num += slices.Length;
					goto IL_2DF;
				}
				shadowCullingInfos = default(ShadowCastersCullingInfos);
				shadowCullingInfos.splitBuffer = nativeArray.GetSubArray(0, num);
				shadowCullingInfos.perLightInfos = perLightInfos;
			}
		}

		private static BatchCullingProjectionType GetCullingProjectionType(LightType type)
		{
			switch (type)
			{
			case LightType.Spot:
				return BatchCullingProjectionType.Perspective;
			case LightType.Directional:
				return BatchCullingProjectionType.Orthographic;
			case LightType.Point:
				return BatchCullingProjectionType.Perspective;
			default:
				return BatchCullingProjectionType.Unknown;
			}
		}

		private static readonly ProfilingSampler computeShadowCasterCullingInfosMarker = new ProfilingSampler("UniversalRenderPipeline.ComputeShadowCasterCullingInfos");
	}
}
