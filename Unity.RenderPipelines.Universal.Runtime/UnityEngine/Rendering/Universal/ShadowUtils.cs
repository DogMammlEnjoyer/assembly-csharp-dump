using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal.Internal;

namespace UnityEngine.Rendering.Universal
{
	public static class ShadowUtils
	{
		public static bool ExtractDirectionalLightMatrix(ref CullingResults cullResults, ref ShadowData shadowData, int shadowLightIndex, int cascadeIndex, int shadowmapWidth, int shadowmapHeight, int shadowResolution, float shadowNearPlane, out Vector4 cascadeSplitDistance, out ShadowSliceData shadowSliceData, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix)
		{
			bool result = ShadowUtils.ExtractDirectionalLightMatrix(ref cullResults, ref shadowData, shadowLightIndex, cascadeIndex, shadowmapWidth, shadowmapHeight, shadowResolution, shadowNearPlane, out cascadeSplitDistance, out shadowSliceData);
			viewMatrix = shadowSliceData.viewMatrix;
			projMatrix = shadowSliceData.projectionMatrix;
			return result;
		}

		public static bool ExtractDirectionalLightMatrix(ref CullingResults cullResults, ref ShadowData shadowData, int shadowLightIndex, int cascadeIndex, int shadowmapWidth, int shadowmapHeight, int shadowResolution, float shadowNearPlane, out Vector4 cascadeSplitDistance, out ShadowSliceData shadowSliceData)
		{
			return ShadowUtils.ExtractDirectionalLightMatrix(ref cullResults, shadowData.universalShadowData, shadowLightIndex, cascadeIndex, shadowmapWidth, shadowmapHeight, shadowResolution, shadowNearPlane, out cascadeSplitDistance, out shadowSliceData);
		}

		public static bool ExtractDirectionalLightMatrix(ref CullingResults cullResults, UniversalShadowData shadowData, int shadowLightIndex, int cascadeIndex, int shadowmapWidth, int shadowmapHeight, int shadowResolution, float shadowNearPlane, out Vector4 cascadeSplitDistance, out ShadowSliceData shadowSliceData)
		{
			bool result = cullResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(shadowLightIndex, cascadeIndex, shadowData.mainLightShadowCascadesCount, shadowData.mainLightShadowCascadesSplit, shadowResolution, shadowNearPlane, out shadowSliceData.viewMatrix, out shadowSliceData.projectionMatrix, out shadowSliceData.splitData);
			cascadeSplitDistance = shadowSliceData.splitData.cullingSphere;
			shadowSliceData.offsetX = cascadeIndex % 2 * shadowResolution;
			shadowSliceData.offsetY = cascadeIndex / 2 * shadowResolution;
			shadowSliceData.resolution = shadowResolution;
			shadowSliceData.shadowTransform = ShadowUtils.GetShadowTransform(shadowSliceData.projectionMatrix, shadowSliceData.viewMatrix);
			shadowSliceData.splitData.shadowCascadeBlendCullingFactor = 1f;
			if (shadowData.mainLightShadowCascadesCount > 1)
			{
				ShadowUtils.ApplySliceTransform(ref shadowSliceData, shadowmapWidth, shadowmapHeight);
			}
			return result;
		}

		public static bool ExtractSpotLightMatrix(ref CullingResults cullResults, ref ShadowData shadowData, int shadowLightIndex, out Matrix4x4 shadowMatrix, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData)
		{
			return ShadowUtils.ExtractSpotLightMatrix(ref cullResults, shadowData.universalShadowData, shadowLightIndex, out shadowMatrix, out viewMatrix, out projMatrix, out splitData);
		}

		public static bool ExtractSpotLightMatrix(ref CullingResults cullResults, UniversalShadowData shadowData, int shadowLightIndex, out Matrix4x4 shadowMatrix, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData)
		{
			bool result = cullResults.ComputeSpotShadowMatricesAndCullingPrimitives(shadowLightIndex, out viewMatrix, out projMatrix, out splitData);
			shadowMatrix = ShadowUtils.GetShadowTransform(projMatrix, viewMatrix);
			return result;
		}

		public static bool ExtractPointLightMatrix(ref CullingResults cullResults, ref ShadowData shadowData, int shadowLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 shadowMatrix, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData)
		{
			return ShadowUtils.ExtractPointLightMatrix(ref cullResults, shadowData.universalShadowData, shadowLightIndex, cubemapFace, fovBias, out shadowMatrix, out viewMatrix, out projMatrix, out splitData);
		}

		public static bool ExtractPointLightMatrix(ref CullingResults cullResults, UniversalShadowData shadowData, int shadowLightIndex, CubemapFace cubemapFace, float fovBias, out Matrix4x4 shadowMatrix, out Matrix4x4 viewMatrix, out Matrix4x4 projMatrix, out ShadowSplitData splitData)
		{
			bool result = cullResults.ComputePointShadowMatricesAndCullingPrimitives(shadowLightIndex, cubemapFace, fovBias, out viewMatrix, out projMatrix, out splitData);
			viewMatrix.m10 = -viewMatrix.m10;
			viewMatrix.m11 = -viewMatrix.m11;
			viewMatrix.m12 = -viewMatrix.m12;
			viewMatrix.m13 = -viewMatrix.m13;
			shadowMatrix = ShadowUtils.GetShadowTransform(projMatrix, viewMatrix);
			return result;
		}

		public static void RenderShadowSlice(CommandBuffer cmd, ref ScriptableRenderContext context, ref ShadowSliceData shadowSliceData, ref ShadowDrawingSettings settings, Matrix4x4 proj, Matrix4x4 view)
		{
			cmd.SetGlobalDepthBias(1f, 2.5f);
			cmd.SetViewport(new Rect((float)shadowSliceData.offsetX, (float)shadowSliceData.offsetY, (float)shadowSliceData.resolution, (float)shadowSliceData.resolution));
			cmd.SetViewProjectionMatrices(view, proj);
			RendererList rendererList = context.CreateShadowRendererList(ref settings);
			cmd.DrawRendererList(rendererList);
			cmd.DisableScissorRect();
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
			cmd.SetGlobalDepthBias(0f, 0f);
		}

		internal static void RenderShadowSlice(RasterCommandBuffer cmd, ref ShadowSliceData shadowSliceData, ref RendererList shadowRendererList, Matrix4x4 proj, Matrix4x4 view)
		{
			cmd.SetGlobalDepthBias(1f, 2.5f);
			cmd.SetViewport(new Rect((float)shadowSliceData.offsetX, (float)shadowSliceData.offsetY, (float)shadowSliceData.resolution, (float)shadowSliceData.resolution));
			cmd.SetViewProjectionMatrices(view, proj);
			if (shadowRendererList.isValid)
			{
				cmd.DrawRendererList(shadowRendererList);
			}
			cmd.DisableScissorRect();
			cmd.SetGlobalDepthBias(0f, 0f);
		}

		public static void RenderShadowSlice(CommandBuffer cmd, ref ScriptableRenderContext context, ref ShadowSliceData shadowSliceData, ref ShadowDrawingSettings settings)
		{
			ShadowUtils.RenderShadowSlice(cmd, ref context, ref shadowSliceData, ref settings, shadowSliceData.projectionMatrix, shadowSliceData.viewMatrix);
		}

		public static int GetMaxTileResolutionInAtlas(int atlasWidth, int atlasHeight, int tileCount)
		{
			int num = Mathf.Min(atlasWidth, atlasHeight);
			for (int i = atlasWidth / num * atlasHeight / num; i < tileCount; i = atlasWidth / num * atlasHeight / num)
			{
				num >>= 1;
			}
			return num;
		}

		public static void ApplySliceTransform(ref ShadowSliceData shadowSliceData, int atlasWidth, int atlasHeight)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			float num = 1f / (float)atlasWidth;
			float num2 = 1f / (float)atlasHeight;
			identity.m00 = (float)shadowSliceData.resolution * num;
			identity.m11 = (float)shadowSliceData.resolution * num2;
			identity.m03 = (float)shadowSliceData.offsetX * num;
			identity.m13 = (float)shadowSliceData.offsetY * num2;
			shadowSliceData.shadowTransform = identity * shadowSliceData.shadowTransform;
		}

		public unsafe static Vector4 GetShadowBias(ref VisibleLight shadowLight, int shadowLightIndex, ref ShadowData shadowData, Matrix4x4 lightProjectionMatrix, float shadowResolution)
		{
			return ShadowUtils.GetShadowBias(ref shadowLight, shadowLightIndex, *shadowData.bias, *shadowData.supportsSoftShadows, lightProjectionMatrix, shadowResolution);
		}

		public static Vector4 GetShadowBias(ref VisibleLight shadowLight, int shadowLightIndex, UniversalShadowData shadowData, Matrix4x4 lightProjectionMatrix, float shadowResolution)
		{
			return ShadowUtils.GetShadowBias(ref shadowLight, shadowLightIndex, shadowData.bias, shadowData.supportsSoftShadows, lightProjectionMatrix, shadowResolution);
		}

		private static Vector4 GetShadowBias(ref VisibleLight shadowLight, int shadowLightIndex, List<Vector4> bias, bool supportsSoftShadows, Matrix4x4 lightProjectionMatrix, float shadowResolution)
		{
			if (shadowLightIndex < 0 || shadowLightIndex >= bias.Count)
			{
				Debug.LogWarning(string.Format("{0} is not a valid light index.", shadowLightIndex));
				return Vector4.zero;
			}
			float num;
			if (shadowLight.lightType == LightType.Directional)
			{
				num = 2f / lightProjectionMatrix.m00;
			}
			else if (shadowLight.lightType == LightType.Spot)
			{
				num = Mathf.Tan(shadowLight.spotAngle * 0.5f * 0.017453292f) * shadowLight.range;
			}
			else if (shadowLight.lightType == LightType.Point)
			{
				float pointLightShadowFrustumFovBiasInDegrees = AdditionalLightsShadowCasterPass.GetPointLightShadowFrustumFovBiasInDegrees((int)shadowResolution, shadowLight.light.shadows == LightShadows.Soft);
				num = Mathf.Tan((90f + pointLightShadowFrustumFovBiasInDegrees) * 0.5f * 0.017453292f) * shadowLight.range;
			}
			else
			{
				Debug.LogWarning("Only point, spot and directional shadow casters are supported in universal pipeline");
				num = 0f;
			}
			float num2 = num / shadowResolution;
			float num3 = -bias[shadowLightIndex].x * num2;
			float num4 = -bias[shadowLightIndex].y * num2;
			if (shadowLight.lightType == LightType.Point)
			{
				num4 = 0f;
			}
			if (supportsSoftShadows && shadowLight.light.shadows == LightShadows.Soft)
			{
				SoftShadowQuality softShadowQuality = SoftShadowQuality.Medium;
				UniversalAdditionalLightData universalAdditionalLightData;
				if (shadowLight.light.TryGetComponent<UniversalAdditionalLightData>(out universalAdditionalLightData))
				{
					softShadowQuality = universalAdditionalLightData.softShadowQuality;
				}
				float num5 = 2.5f;
				switch (softShadowQuality)
				{
				case SoftShadowQuality.Low:
					num5 = 1.5f;
					break;
				case SoftShadowQuality.Medium:
					num5 = 2.5f;
					break;
				case SoftShadowQuality.High:
					num5 = 3.5f;
					break;
				}
				num3 *= num5;
				num4 *= num5;
			}
			return new Vector4(num3, num4, (float)shadowLight.lightType, 0f);
		}

		internal static void GetScaleAndBiasForLinearDistanceFade(float fadeDistance, float border, out float scale, out float bias)
		{
			if (border < 0.0001f)
			{
				float num = 1000f;
				scale = num;
				bias = -fadeDistance * num;
				return;
			}
			border = 1f - border;
			border *= border;
			float num2 = border * fadeDistance;
			scale = 1f / (fadeDistance - num2);
			bias = -num2 / (fadeDistance - num2);
		}

		public static void SetupShadowCasterConstantBuffer(CommandBuffer cmd, ref VisibleLight shadowLight, Vector4 shadowBias)
		{
			ShadowUtils.SetupShadowCasterConstantBuffer(CommandBufferHelpers.GetRasterCommandBuffer(cmd), ref shadowLight, shadowBias);
		}

		internal static void SetupShadowCasterConstantBuffer(RasterCommandBuffer cmd, ref VisibleLight shadowLight, Vector4 shadowBias)
		{
			ShadowUtils.SetShadowBias(cmd, shadowBias);
			Vector3 lightDirection = -shadowLight.localToWorldMatrix.GetColumn(2);
			ShadowUtils.SetLightDirection(cmd, lightDirection);
			Vector3 lightPosition = shadowLight.localToWorldMatrix.GetColumn(3);
			ShadowUtils.SetLightPosition(cmd, lightPosition);
		}

		internal static void SetShadowBias(RasterCommandBuffer cmd, Vector4 shadowBias)
		{
			cmd.SetGlobalVector(ShaderPropertyId.shadowBias, shadowBias);
		}

		internal static void SetLightDirection(RasterCommandBuffer cmd, Vector3 lightDirection)
		{
			cmd.SetGlobalVector(ShaderPropertyId.lightDirection, new Vector4(lightDirection.x, lightDirection.y, lightDirection.z, 0f));
		}

		internal static void SetLightPosition(RasterCommandBuffer cmd, Vector3 lightPosition)
		{
			cmd.SetGlobalVector(ShaderPropertyId.lightPosition, new Vector4(lightPosition.x, lightPosition.y, lightPosition.z, 1f));
		}

		internal static void SetCameraPosition(RasterCommandBuffer cmd, Vector3 worldSpaceCameraPos)
		{
			cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, worldSpaceCameraPos);
		}

		internal static void SetWorldToCameraAndCameraToWorldMatrices(RasterCommandBuffer cmd, Matrix4x4 viewMatrix)
		{
			Matrix4x4 value = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * viewMatrix;
			Matrix4x4 inverse = value.inverse;
			cmd.SetGlobalMatrix(ShaderPropertyId.worldToCameraMatrix, value);
			cmd.SetGlobalMatrix(ShaderPropertyId.cameraToWorldMatrix, inverse);
		}

		private static RenderTextureDescriptor GetTemporaryShadowTextureDescriptor(int width, int height, int bits)
		{
			GraphicsFormat depthStencilFormat = GraphicsFormatUtility.GetDepthStencilFormat(bits, 0);
			return new RenderTextureDescriptor(width, height, GraphicsFormat.None, depthStencilFormat)
			{
				shadowSamplingMode = (RenderingUtils.SupportsRenderTextureFormat(RenderTextureFormat.Shadowmap) ? ShadowSamplingMode.CompareDepths : ShadowSamplingMode.None)
			};
		}

		[Obsolete("Use AllocShadowRT or ShadowRTReAllocateIfNeeded", true)]
		public static RenderTexture GetTemporaryShadowTexture(int width, int height, int bits)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(ShadowUtils.GetTemporaryShadowTextureDescriptor(width, height, bits));
			temporary.filterMode = (ShadowUtils.m_ForceShadowPointSampling ? FilterMode.Point : FilterMode.Bilinear);
			temporary.wrapMode = TextureWrapMode.Clamp;
			return temporary;
		}

		public static bool ShadowRTNeedsReAlloc(RTHandle handle, int width, int height, int bits, int anisoLevel, float mipMapBias, string name)
		{
			if (handle == null || handle.rt == null)
			{
				return true;
			}
			RenderTextureDescriptor temporaryShadowTextureDescriptor = ShadowUtils.GetTemporaryShadowTextureDescriptor(width, height, bits);
			if (ShadowUtils.m_ForceShadowPointSampling)
			{
				if (handle.rt.filterMode != FilterMode.Point)
				{
					return true;
				}
			}
			else if (handle.rt.filterMode != FilterMode.Bilinear)
			{
				return true;
			}
			TextureDesc textureDesc = RTHandleResourcePool.CreateTextureDesc(temporaryShadowTextureDescriptor, TextureSizeMode.Explicit, anisoLevel, mipMapBias, ShadowUtils.m_ForceShadowPointSampling ? FilterMode.Point : FilterMode.Bilinear, TextureWrapMode.Clamp, name);
			return RenderingUtils.RTHandleNeedsReAlloc(handle, textureDesc, false);
		}

		public static RTHandle AllocShadowRT(int width, int height, int bits, int anisoLevel, float mipMapBias, string name)
		{
			RenderTextureDescriptor temporaryShadowTextureDescriptor = ShadowUtils.GetTemporaryShadowTextureDescriptor(width, height, bits);
			return RTHandles.Alloc(temporaryShadowTextureDescriptor, ShadowUtils.m_ForceShadowPointSampling ? FilterMode.Point : FilterMode.Bilinear, TextureWrapMode.Clamp, true, 1, 0f, name);
		}

		public static bool ShadowRTReAllocateIfNeeded(ref RTHandle handle, int width, int height, int bits, int anisoLevel = 1, float mipMapBias = 0f, string name = "")
		{
			if (ShadowUtils.ShadowRTNeedsReAlloc(handle, width, height, bits, anisoLevel, mipMapBias, name))
			{
				RTHandle rthandle = handle;
				if (rthandle != null)
				{
					rthandle.Release();
				}
				handle = ShadowUtils.AllocShadowRT(width, height, bits, anisoLevel, mipMapBias, name);
				return true;
			}
			return false;
		}

		private static Matrix4x4 GetShadowTransform(Matrix4x4 proj, Matrix4x4 view)
		{
			if (SystemInfo.usesReversedZBuffer)
			{
				proj.m20 = -proj.m20;
				proj.m21 = -proj.m21;
				proj.m22 = -proj.m22;
				proj.m23 = -proj.m23;
			}
			Matrix4x4 rhs = proj * view;
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = 0.5f;
			identity.m11 = 0.5f;
			identity.m22 = 0.5f;
			identity.m03 = 0.5f;
			identity.m23 = 0.5f;
			identity.m13 = 0.5f;
			return identity * rhs;
		}

		internal static float SoftShadowQualityToShaderProperty(Light light, bool softShadowsEnabled)
		{
			float num = softShadowsEnabled ? 1f : 0f;
			UniversalAdditionalLightData universalAdditionalLightData;
			if (light.TryGetComponent<UniversalAdditionalLightData>(out universalAdditionalLightData))
			{
				SoftShadowQuality? softShadowQuality;
				if (universalAdditionalLightData.softShadowQuality != SoftShadowQuality.UsePipelineSettings)
				{
					softShadowQuality = new SoftShadowQuality?(universalAdditionalLightData.softShadowQuality);
				}
				else
				{
					UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
					softShadowQuality = ((asset != null) ? new SoftShadowQuality?(asset.softShadowQuality) : null);
				}
				SoftShadowQuality? softShadowQuality2 = softShadowQuality;
				num *= (float)Math.Max((int)softShadowQuality2.Value, 1);
			}
			return num;
		}

		internal static bool SupportsPerLightSoftShadowQuality()
		{
			return true;
		}

		internal static void SetPerLightSoftShadowKeyword(RasterCommandBuffer cmd, bool hasSoftShadows)
		{
			if (ShadowUtils.SupportsPerLightSoftShadowQuality())
			{
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, hasSoftShadows);
			}
		}

		internal static void SetSoftShadowQualityShaderKeywords(RasterCommandBuffer cmd, UniversalShadowData shadowData)
		{
			cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, shadowData.isKeywordSoftShadowsEnabled);
			if (ShadowUtils.SupportsPerLightSoftShadowQuality())
			{
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsLow, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsMedium, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsHigh, false);
				return;
			}
			if (shadowData.isKeywordSoftShadowsEnabled)
			{
				UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
				if (asset != null && asset.softShadowQuality == SoftShadowQuality.Low)
				{
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsLow, true);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsMedium, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsHigh, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, false);
					return;
				}
			}
			if (shadowData.isKeywordSoftShadowsEnabled)
			{
				UniversalRenderPipelineAsset asset2 = UniversalRenderPipeline.asset;
				if (asset2 != null && asset2.softShadowQuality == SoftShadowQuality.Medium)
				{
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsLow, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsMedium, true);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsHigh, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, false);
					return;
				}
			}
			if (shadowData.isKeywordSoftShadowsEnabled)
			{
				UniversalRenderPipelineAsset asset3 = UniversalRenderPipeline.asset;
				if (asset3 != null && asset3.softShadowQuality == SoftShadowQuality.High)
				{
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsLow, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsMedium, false);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsHigh, true);
					cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, false);
				}
			}
		}

		internal static bool IsValidShadowCastingLight(UniversalLightData lightData, int i)
		{
			ref VisibleLight ptr = ref lightData.visibleLights.UnsafeElementAt(i);
			Light light = ptr.light;
			return !(light == null) && ShadowUtils.IsValidShadowCastingLight(lightData, i, ptr.lightType, light.shadows, light.shadowStrength);
		}

		internal static bool IsValidShadowCastingLight(UniversalLightData lightData, int i, LightType lightType, LightShadows lightShadows, float shadowStrength)
		{
			return i != lightData.mainLightIndex && lightType != LightType.Directional && lightShadows != LightShadows.None && shadowStrength > 0f;
		}

		internal static int GetPunctualLightShadowSlicesCount(in LightType lightType)
		{
			LightType lightType2 = lightType;
			if (lightType2 == LightType.Spot)
			{
				return 1;
			}
			if (lightType2 != LightType.Point)
			{
				return 0;
			}
			return 6;
		}

		internal static bool FastApproximately(float a, float b)
		{
			return Mathf.Abs(a - b) < 1E-06f;
		}

		internal static bool FastApproximately(Vector4 a, Vector4 b)
		{
			return ShadowUtils.FastApproximately(a.x, b.x) && ShadowUtils.FastApproximately(a.y, b.y) && ShadowUtils.FastApproximately(a.z, b.z) && ShadowUtils.FastApproximately(a.w, b.w);
		}

		internal static int MinimalPunctualLightShadowResolution(bool softShadow)
		{
			if (!softShadow)
			{
				return 8;
			}
			return 16;
		}

		internal static readonly bool m_ForceShadowPointSampling = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && GraphicsSettings.HasShaderDefine(Graphics.activeTier, BuiltinShaderDefine.UNITY_METAL_SHADOWS_USE_POINT_FILTERING);

		internal const int kMinimumPunctualLightHardShadowResolution = 8;

		internal const int kMinimumPunctualLightSoftShadowResolution = 16;
	}
}
