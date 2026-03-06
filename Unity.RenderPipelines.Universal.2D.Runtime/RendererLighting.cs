using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal static class RendererLighting
	{
		internal static GraphicsFormat GetRenderTextureFormat()
		{
			if (!RendererLighting.s_HasSetupRenderTextureFormatToUse)
			{
				if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, GraphicsFormatUsage.Blend))
				{
					RendererLighting.s_RenderTextureFormatToUse = GraphicsFormat.B10G11R11_UFloatPack32;
				}
				else if (SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Blend))
				{
					RendererLighting.s_RenderTextureFormatToUse = GraphicsFormat.R16G16B16A16_SFloat;
				}
				RendererLighting.s_HasSetupRenderTextureFormatToUse = true;
			}
			return RendererLighting.s_RenderTextureFormatToUse;
		}

		public static void CreateNormalMapRenderTexture(this IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmd, float renderScale)
		{
			RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor((int)((float)renderingData.cameraData.cameraTargetDescriptor.width * renderScale), (int)((float)renderingData.cameraData.cameraTargetDescriptor.height * renderScale));
			renderTextureDescriptor.graphicsFormat = RendererLighting.GetRenderTextureFormat();
			renderTextureDescriptor.useMipMap = false;
			renderTextureDescriptor.autoGenerateMips = false;
			renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
			renderTextureDescriptor.msaaSamples = renderingData.cameraData.cameraTargetDescriptor.msaaSamples;
			renderTextureDescriptor.dimension = TextureDimension.Tex2D;
			RenderingUtils.ReAllocateHandleIfNeeded(ref pass.rendererData.normalsRenderTarget, renderTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_NormalMap");
			cmd.SetGlobalTexture(pass.rendererData.normalsRenderTarget.name, pass.rendererData.normalsRenderTarget.nameID);
		}

		public static RenderTextureDescriptor GetBlendStyleRenderTextureDesc(this IRenderPass2D pass, RenderingData renderingData)
		{
			float num = Mathf.Clamp(pass.rendererData.lightRenderTextureScale, 0.01f, 1f);
			int width = (int)((float)renderingData.cameraData.cameraTargetDescriptor.width * num);
			int height = (int)((float)renderingData.cameraData.cameraTargetDescriptor.height * num);
			return new RenderTextureDescriptor(width, height)
			{
				graphicsFormat = RendererLighting.GetRenderTextureFormat(),
				useMipMap = false,
				autoGenerateMips = false,
				depthStencilFormat = GraphicsFormat.None,
				msaaSamples = 1,
				dimension = TextureDimension.Tex2D
			};
		}

		public static void CreateCameraSortingLayerRenderTexture(this IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmd, Downsampling downsamplingMethod)
		{
			float num = 1f;
			if (downsamplingMethod == Downsampling._2xBilinear)
			{
				num = 0.5f;
			}
			else if (downsamplingMethod == Downsampling._4xBox || downsamplingMethod == Downsampling._4xBilinear)
			{
				num = 0.25f;
			}
			int width = (int)((float)renderingData.cameraData.cameraTargetDescriptor.width * num);
			int height = (int)((float)renderingData.cameraData.cameraTargetDescriptor.height * num);
			RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height);
			renderTextureDescriptor.graphicsFormat = renderingData.cameraData.cameraTargetDescriptor.graphicsFormat;
			renderTextureDescriptor.useMipMap = false;
			renderTextureDescriptor.autoGenerateMips = false;
			renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
			renderTextureDescriptor.msaaSamples = 1;
			RenderingUtils.ReAllocateHandleIfNeeded(ref pass.rendererData.cameraSortingLayerRenderTarget, renderTextureDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_CameraSortingLayerTexture");
			cmd.SetGlobalTexture(pass.rendererData.cameraSortingLayerRenderTarget.name, pass.rendererData.cameraSortingLayerRenderTarget.nameID);
		}

		internal static void EnableBlendStyle(IRasterCommandBuffer cmd, int blendStyleIndex, bool enabled)
		{
			string keyword = RendererLighting.k_UseBlendStyleKeywords[blendStyleIndex];
			if (enabled)
			{
				cmd.EnableShaderKeyword(keyword);
				return;
			}
			cmd.DisableShaderKeyword(keyword);
		}

		internal static void DisableAllKeywords(RasterCommandBuffer cmd)
		{
			foreach (string keyword in RendererLighting.k_UseBlendStyleKeywords)
			{
				cmd.DisableShaderKeyword(keyword);
			}
		}

		internal static void GetTransparencySortingMode(Renderer2DData rendererData, Camera camera, ref SortingSettings sortingSettings)
		{
			TransparencySortMode transparencySortMode = rendererData.transparencySortMode;
			if (transparencySortMode == TransparencySortMode.Default)
			{
				transparencySortMode = (camera.orthographic ? TransparencySortMode.Orthographic : TransparencySortMode.Perspective);
			}
			if (transparencySortMode == TransparencySortMode.Perspective)
			{
				sortingSettings.distanceMetric = DistanceMetric.Perspective;
				return;
			}
			if (transparencySortMode != TransparencySortMode.Orthographic)
			{
				sortingSettings.distanceMetric = DistanceMetric.CustomAxis;
				sortingSettings.customAxis = rendererData.transparencySortAxis;
				return;
			}
			sortingSettings.distanceMetric = DistanceMetric.Orthographic;
		}

		private static bool CanRenderLight(IRenderPass2D pass, Light2D light, int blendStyleIndex, int layerToRender, bool isVolume, bool hasShadows, ref Mesh lightMesh, ref Material lightMaterial)
		{
			if (!(light != null) || light.lightType == Light2D.LightType.Global || light.blendStyleIndex != blendStyleIndex || !light.IsLitLayer(layerToRender))
			{
				return false;
			}
			lightMesh = light.lightMesh;
			if (lightMesh == null)
			{
				return false;
			}
			lightMaterial = pass.rendererData.GetLightMaterial(light, isVolume, hasShadows);
			return !(lightMaterial == null);
		}

		internal static bool CanCastShadows(Light2D light, int layerToRender)
		{
			return light.shadowsEnabled && light.shadowIntensity > 0f && light.IsLitLayer(layerToRender);
		}

		private static bool CanCastVolumetricShadows(Light2D light, int endLayerValue)
		{
			int topMostLitLayer = light.GetTopMostLitLayer();
			return light.volumetricShadowsEnabled && light.shadowVolumeIntensity > 0f && topMostLitLayer == endLayerValue;
		}

		internal static void RenderLight(IRenderPass2D pass, CommandBuffer cmd, Light2D light, bool isVolume, int blendStyleIndex, int layerToRender, bool hasShadows, bool batchingSupported, ref int shadowLightCount)
		{
			Mesh mesh = null;
			Material material = null;
			if (!RendererLighting.CanRenderLight(pass, light, blendStyleIndex, layerToRender, isVolume, hasShadows, ref mesh, ref material))
			{
				return;
			}
			int lightHash;
			bool flag = RendererLighting.lightBatch.CanBatch(light, material, light.batchSlotIndex, out lightHash);
			bool flag2 = RendererLighting.SetCookieShaderGlobals(cmd, light);
			if ((hasShadows || flag2 || !flag) && batchingSupported)
			{
				RendererLighting.lightBatch.Flush(CommandBufferHelpers.GetRasterCommandBuffer(cmd));
			}
			if (hasShadows)
			{
				int num = shadowLightCount;
				shadowLightCount = num + 1;
				ShadowRendering.SetGlobalShadowTexture(cmd, light, num);
			}
			int slot = RendererLighting.lightBatch.SlotIndex(light.batchSlotIndex);
			RendererLighting.SetPerLightShaderGlobals(CommandBufferHelpers.GetRasterCommandBuffer(cmd), light, slot, isVolume, hasShadows, batchingSupported);
			if (light.lightType == Light2D.LightType.Point)
			{
				RendererLighting.SetPerPointLightShaderGlobals(CommandBufferHelpers.GetRasterCommandBuffer(cmd), light, slot, batchingSupported);
			}
			if (batchingSupported)
			{
				RendererLighting.lightBatch.AddBatch(light, material, light.GetMatrix(), mesh, 0, lightHash, light.batchSlotIndex);
				return;
			}
			cmd.DrawMesh(mesh, light.GetMatrix(), material);
		}

		private static void RenderLightSet(IRenderPass2D pass, RenderingData renderingData, int blendStyleIndex, CommandBuffer cmd, ref LayerBatch layer, RenderTargetIdentifier renderTexture, List<Light2D> lights)
		{
			uint maxTextureCount = ShadowRendering.maxTextureCount;
			bool flag = true;
			if (maxTextureCount < 1U)
			{
				Debug.LogError("maxShadowTextureCount cannot be less than 1");
				return;
			}
			NativeArray<bool> nativeArray = new NativeArray<bool>(lights.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num2;
			for (int i = 0; i < lights.Count; i += num2)
			{
				long num = (long)((ulong)lights.Count - (ulong)((long)i));
				num2 = 0;
				int num3 = 0;
				while ((long)num2 < num && (long)num3 < (long)((ulong)maxTextureCount))
				{
					int index = i + num2;
					Light2D light2D = lights[index];
					if (RendererLighting.CanCastShadows(light2D, layer.startLayerID))
					{
						nativeArray[index] = false;
						if (pass.PrerenderShadows(renderingData, cmd, ref layer, light2D, num3, light2D.shadowIntensity))
						{
							nativeArray[index] = true;
							num3++;
						}
					}
					num2++;
				}
				if (num3 > 0 || flag)
				{
					cmd.SetRenderTarget(renderTexture, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
					flag = false;
				}
				num3 = 0;
				for (int j = 0; j < num2; j++)
				{
					int index2 = i + j;
					RendererLighting.RenderLight(pass, cmd, lights[index2], false, blendStyleIndex, layer.startLayerID, nativeArray[index2], LightBatch.isBatchingSupported, ref num3);
				}
				RendererLighting.lightBatch.Flush(CommandBufferHelpers.GetRasterCommandBuffer(cmd));
				for (int k = num3 - 1; k >= 0; k--)
				{
					ShadowRendering.ReleaseShadowRenderTexture(cmd, k);
				}
			}
			nativeArray.Dispose();
		}

		public static void RenderLightVolumes(this IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmd, ref LayerBatch layer, RenderTargetIdentifier renderTexture, RenderTargetIdentifier depthTexture, RenderBufferStoreAction intermediateStoreAction, RenderBufferStoreAction finalStoreAction, bool requiresRTInit, List<Light2D> lights)
		{
			uint maxTextureCount = ShadowRendering.maxTextureCount;
			NativeArray<bool> nativeArray = new NativeArray<bool>(lights.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			if (maxTextureCount < 1U)
			{
				Debug.LogError("maxShadowLightCount cannot be less than 1");
				return;
			}
			int num = lights.Count;
			if (intermediateStoreAction != finalStoreAction)
			{
				for (int i = lights.Count - 1; i >= 0; i--)
				{
					if (lights[i].renderVolumetricShadows)
					{
						num = i;
						break;
					}
				}
			}
			int num3;
			for (int j = 0; j < lights.Count; j += num3)
			{
				long num2 = (long)((ulong)lights.Count - (ulong)((long)j));
				num3 = 0;
				int num4 = 0;
				while ((long)num3 < num2 && (long)num4 < (long)((ulong)maxTextureCount))
				{
					int index = j + num3;
					Light2D light2D = lights[index];
					if (RendererLighting.CanCastVolumetricShadows(light2D, layer.endLayerValue))
					{
						nativeArray[index] = false;
						if (pass.PrerenderShadows(renderingData, cmd, ref layer, light2D, num4, light2D.shadowVolumeIntensity))
						{
							nativeArray[index] = true;
							num4++;
						}
					}
					num3++;
				}
				if (num4 > 0 || requiresRTInit)
				{
					RenderBufferStoreAction renderBufferStoreAction = (j + num3 >= num) ? finalStoreAction : intermediateStoreAction;
					cmd.SetRenderTarget(renderTexture, RenderBufferLoadAction.Load, renderBufferStoreAction, depthTexture, RenderBufferLoadAction.Load, renderBufferStoreAction);
					requiresRTInit = false;
				}
				num4 = 0;
				for (int k = 0; k < num3; k++)
				{
					int index2 = j + k;
					Light2D light2D2 = lights[index2];
					if (light2D2.volumeIntensity > 0f && light2D2.volumetricEnabled && layer.endLayerValue == light2D2.GetTopMostLitLayer())
					{
						RendererLighting.RenderLight(pass, cmd, light2D2, true, light2D2.blendStyleIndex, layer.startLayerID, nativeArray[index2], LightBatch.isBatchingSupported, ref num4);
					}
				}
				RendererLighting.lightBatch.Flush(CommandBufferHelpers.GetRasterCommandBuffer(cmd));
				for (int l = num4 - 1; l >= 0; l--)
				{
					ShadowRendering.ReleaseShadowRenderTexture(cmd, l);
				}
			}
			nativeArray.Dispose();
		}

		internal static void SetLightShaderGlobals(Renderer2DData rendererData, RasterCommandBuffer cmd)
		{
			for (int i = 0; i < rendererData.lightBlendStyles.Length; i++)
			{
				Light2DBlendStyle light2DBlendStyle = rendererData.lightBlendStyles[i];
				if (i >= RendererLighting.k_BlendFactorsPropIDs.Length)
				{
					break;
				}
				cmd.SetGlobalVector(RendererLighting.k_BlendFactorsPropIDs[i], light2DBlendStyle.blendFactors);
				cmd.SetGlobalVector(RendererLighting.k_MaskFilterPropIDs[i], light2DBlendStyle.maskTextureChannelFilter.mask);
				cmd.SetGlobalVector(RendererLighting.k_InvertedFilterPropIDs[i], light2DBlendStyle.maskTextureChannelFilter.inverted);
			}
		}

		internal static void SetLightShaderGlobals(RasterCommandBuffer cmd, Light2DBlendStyle[] lightBlendStyles, int[] blendStyleIndices)
		{
			foreach (int num in blendStyleIndices)
			{
				if (num >= RendererLighting.k_BlendFactorsPropIDs.Length)
				{
					break;
				}
				Light2DBlendStyle light2DBlendStyle = lightBlendStyles[num];
				cmd.SetGlobalVector(RendererLighting.k_BlendFactorsPropIDs[num], light2DBlendStyle.blendFactors);
				cmd.SetGlobalVector(RendererLighting.k_MaskFilterPropIDs[num], light2DBlendStyle.maskTextureChannelFilter.mask);
				cmd.SetGlobalVector(RendererLighting.k_InvertedFilterPropIDs[num], light2DBlendStyle.maskTextureChannelFilter.inverted);
			}
		}

		private static float GetNormalizedInnerRadius(Light2D light)
		{
			return light.pointLightInnerRadius / light.pointLightOuterRadius;
		}

		private static float GetNormalizedAngle(float angle)
		{
			return angle / 360f;
		}

		private static void GetScaledLightInvMatrix(Light2D light, out Matrix4x4 retMatrix)
		{
			float pointLightOuterRadius = light.pointLightOuterRadius;
			Vector3 one = Vector3.one;
			Vector3 s = new Vector3(one.x * pointLightOuterRadius, one.y * pointLightOuterRadius, one.z * pointLightOuterRadius);
			Transform transform = light.transform;
			Matrix4x4 m = Matrix4x4.TRS(transform.position, transform.rotation, s);
			retMatrix = Matrix4x4.Inverse(m);
		}

		internal static void SetPerLightShaderGlobals(IRasterCommandBuffer cmd, Light2D light, int slot, bool isVolumetric, bool hasShadows, bool batchingSupported)
		{
			Color color = light.intensity * light.color.a * light.color;
			color.a = 1f;
			float num = light.volumetricEnabled ? light.volumeIntensity : 1f;
			if (batchingSupported)
			{
				PerLight2D light2 = RendererLighting.lightBatch.GetLight(slot);
				light2.Position = new float4(light.transform.position, light.normalMapDistance);
				light2.FalloffIntensity = light.falloffIntensity;
				light2.FalloffDistance = light.shapeLightFalloffSize;
				light2.Color = new float4(color.r, color.g, color.b, color.a);
				light2.VolumeOpacity = num;
				light2.LightType = (int)light.lightType;
				light2.ShadowIntensity = 1f;
				if (hasShadows)
				{
					light2.ShadowIntensity = (isVolumetric ? (1f - light.shadowVolumeIntensity) : (1f - light.shadowIntensity));
				}
				RendererLighting.lightBatch.SetLight(slot, light2);
			}
			else
			{
				cmd.SetGlobalVector(RendererLighting.k_L2DPosition, new float4(light.transform.position, light.normalMapDistance));
				cmd.SetGlobalFloat(RendererLighting.k_L2DFalloffIntensity, light.falloffIntensity);
				cmd.SetGlobalFloat(RendererLighting.k_L2DFalloffDistance, light.shapeLightFalloffSize);
				cmd.SetGlobalColor(RendererLighting.k_L2DColor, color);
				cmd.SetGlobalFloat(RendererLighting.k_L2DVolumeOpacity, num);
				cmd.SetGlobalInt(RendererLighting.k_L2DLightType, (int)light.lightType);
				cmd.SetGlobalFloat(RendererLighting.k_L2DShadowIntensity, hasShadows ? (isVolumetric ? (1f - light.shadowVolumeIntensity) : (1f - light.shadowIntensity)) : 1f);
			}
			if (hasShadows)
			{
				ShadowRendering.SetGlobalShadowProp(cmd);
			}
		}

		internal static void SetPerPointLightShaderGlobals(IRasterCommandBuffer cmd, Light2D light, int slot, bool batchingSupported)
		{
			Matrix4x4 value;
			RendererLighting.GetScaledLightInvMatrix(light, out value);
			float normalizedInnerRadius = RendererLighting.GetNormalizedInnerRadius(light);
			float normalizedAngle = RendererLighting.GetNormalizedAngle(light.pointLightInnerAngle);
			float normalizedAngle2 = RendererLighting.GetNormalizedAngle(light.pointLightOuterAngle);
			float num = 1f / (1f - normalizedInnerRadius);
			if (batchingSupported)
			{
				PerLight2D light2 = RendererLighting.lightBatch.GetLight(slot);
				light2.InvMatrix = new float4x4(value.GetColumn(0), value.GetColumn(1), value.GetColumn(2), value.GetColumn(3));
				light2.InnerRadiusMult = num;
				light2.InnerAngle = normalizedAngle;
				light2.OuterAngle = normalizedAngle2;
				RendererLighting.lightBatch.SetLight(slot, light2);
				return;
			}
			cmd.SetGlobalMatrix(RendererLighting.k_L2DInvMatrix, value);
			cmd.SetGlobalFloat(RendererLighting.k_L2DInnerRadiusMult, num);
			cmd.SetGlobalFloat(RendererLighting.k_L2DInnerAngle, normalizedAngle);
			cmd.SetGlobalFloat(RendererLighting.k_L2DOuterAngle, normalizedAngle2);
		}

		internal static bool SetCookieShaderGlobals(CommandBuffer cmd, Light2D light)
		{
			if (light.useCookieSprite)
			{
				cmd.SetGlobalTexture((light.lightType == Light2D.LightType.Sprite) ? RendererLighting.k_CookieTexID : RendererLighting.k_PointLightCookieTexID, light.lightCookieSprite.texture);
			}
			return light.useCookieSprite;
		}

		internal static void SetCookieShaderProperties(Light2D light, MaterialPropertyBlock properties)
		{
			if (light.useCookieSprite && light.m_CookieSpriteTextureHandle.IsValid())
			{
				properties.SetTexture((light.lightType == Light2D.LightType.Sprite) ? RendererLighting.k_CookieTexID : RendererLighting.k_PointLightCookieTexID, light.m_CookieSpriteTextureHandle);
			}
		}

		public static void ClearDirtyLighting(this IRenderPass2D pass, CommandBuffer cmd, uint blendStylesUsed)
		{
			for (int i = 0; i < pass.rendererData.lightBlendStyles.Length; i++)
			{
				if ((blendStylesUsed & 1U << i) != 0U && pass.rendererData.lightBlendStyles[i].isDirty)
				{
					CoreUtils.SetRenderTarget(cmd, pass.rendererData.lightBlendStyles[i].renderTargetHandle, ClearFlag.Color, Color.black, 0, CubemapFace.Unknown, -1);
					pass.rendererData.lightBlendStyles[i].isDirty = false;
				}
			}
		}

		internal unsafe static void RenderNormals(this IRenderPass2D pass, ScriptableRenderContext context, RenderingData renderingData, DrawingSettings drawSettings, FilteringSettings filterSettings, RTHandle depthTarget, bool bFirstClear)
		{
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			using (new ProfilingScope(commandBuffer, RendererLighting.m_ProfilingSampler))
			{
				float renderScale;
				if (depthTarget != null)
				{
					renderScale = 1f;
				}
				else
				{
					renderScale = Mathf.Clamp(pass.rendererData.lightRenderTextureScale, 0.01f, 1f);
				}
				pass.CreateNormalMapRenderTexture(renderingData, commandBuffer, renderScale);
				RenderBufferStoreAction renderBufferStoreAction = (renderingData.cameraData.cameraTargetDescriptor.msaaSamples > 1) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.Store;
				ClearFlag clearFlag = (pass.rendererData.useDepthStencilBuffer && bFirstClear) ? ClearFlag.All : ClearFlag.Color;
				if (depthTarget != null)
				{
					CoreUtils.SetRenderTarget(commandBuffer, pass.rendererData.normalsRenderTarget, RenderBufferLoadAction.DontCare, renderBufferStoreAction, depthTarget, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, clearFlag, RendererLighting.k_NormalClearColor, 0, CubemapFace.Unknown, -1);
				}
				else
				{
					CoreUtils.SetRenderTarget(commandBuffer, pass.rendererData.normalsRenderTarget, RenderBufferLoadAction.DontCare, renderBufferStoreAction, clearFlag, RendererLighting.k_NormalClearColor, 0, CubemapFace.Unknown, -1);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				drawSettings.SetShaderPassName(0, RendererLighting.k_NormalsRenderingPassName);
				RendererListParams rendererListParams = new RendererListParams(*renderingData.cullResults, drawSettings, filterSettings);
				RendererList rendererList = context.CreateRendererList(ref rendererListParams);
				commandBuffer.DrawRendererList(rendererList);
			}
		}

		public static void RenderLights(this IRenderPass2D pass, RenderingData renderingData, CommandBuffer cmd, ref LayerBatch layerBatch, ref RenderTextureDescriptor rtDesc)
		{
			List<Light2D> visibleLights = pass.rendererData.lightCullResult.visibleLights;
			for (int i = 0; i < visibleLights.Count; i++)
			{
				visibleLights[i].CacheValues();
			}
			ShadowCasterGroup2DManager.CacheValues();
			Light2DBlendStyle[] lightBlendStyles = pass.rendererData.lightBlendStyles;
			for (int j = 0; j < lightBlendStyles.Length; j++)
			{
				if ((layerBatch.lightStats.blendStylesUsed & 1U << j) != 0U)
				{
					string name = lightBlendStyles[j].name;
					cmd.BeginSample(name);
					Color black;
					if (!Light2DManager.GetGlobalColor(layerBatch.startLayerID, j, out black))
					{
						black = Color.black;
					}
					bool flag = (layerBatch.lightStats.blendStylesWithLights & 1U << j) > 0U;
					RenderTextureDescriptor desc = rtDesc;
					if (!flag)
					{
						desc.width = (desc.height = 4);
					}
					RenderTargetIdentifier rtid = layerBatch.GetRTId(cmd, desc, j);
					cmd.SetRenderTarget(rtid, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
					cmd.ClearRenderTarget(false, true, black);
					if (flag)
					{
						RendererLighting.RenderLightSet(pass, renderingData, j, cmd, ref layerBatch, rtid, pass.rendererData.lightCullResult.visibleLights);
					}
					cmd.EndSample(name);
				}
			}
		}

		private static void SetBlendModes(Material material, BlendMode src, BlendMode dst)
		{
			material.SetFloat(RendererLighting.k_SrcBlendID, (float)src);
			material.SetFloat(RendererLighting.k_DstBlendID, (float)dst);
		}

		private static uint GetLightMaterialIndex(Light2D light, bool isVolume, bool useShadows)
		{
			bool isPointLight = light.isPointLight;
			int num = 0;
			uint num2 = isVolume ? (1U << num) : 0U;
			num++;
			uint num3 = (isVolume && !isPointLight) ? (1U << num) : 0U;
			num++;
			uint num4 = (light.overlapOperation == Light2D.OverlapOperation.AlphaBlend) ? 0U : (1U << num);
			num++;
			uint num5 = (isPointLight && light.lightCookieSprite != null && light.lightCookieSprite.texture != null) ? (1U << num) : 0U;
			num++;
			uint num6 = (light.normalMapQuality == Light2D.NormalMapQuality.Fast) ? (1U << num) : 0U;
			num++;
			uint num7 = (light.normalMapQuality != Light2D.NormalMapQuality.Disabled) ? (1U << num) : 0U;
			num++;
			uint num8 = useShadows ? (1U << num) : 0U;
			return num6 | num5 | num4 | num3 | num2 | num7 | num8;
		}

		private static Material CreateLightMaterial(Renderer2DData rendererData, Light2D light, bool isVolume, bool useShadows)
		{
			Renderer2DResources renderer2DResources;
			if (!GraphicsSettings.TryGetRenderPipelineSettings<Renderer2DResources>(out renderer2DResources))
			{
				return null;
			}
			bool isPointLight = light.isPointLight;
			Material material = CoreUtils.CreateEngineMaterial(renderer2DResources.lightShader);
			if (!isVolume)
			{
				if (light.overlapOperation == Light2D.OverlapOperation.Additive)
				{
					RendererLighting.SetBlendModes(material, BlendMode.One, BlendMode.One);
					material.EnableKeyword(RendererLighting.k_UseAdditiveBlendingKeyword);
				}
				else
				{
					RendererLighting.SetBlendModes(material, BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
				}
			}
			else
			{
				material.EnableKeyword(RendererLighting.k_UseVolumetric);
				if (light.lightType == Light2D.LightType.Point)
				{
					RendererLighting.SetBlendModes(material, BlendMode.One, BlendMode.One);
				}
				else
				{
					RendererLighting.SetBlendModes(material, BlendMode.SrcAlpha, BlendMode.One);
				}
			}
			if (isPointLight && light.lightCookieSprite != null && light.lightCookieSprite.texture != null)
			{
				material.EnableKeyword(RendererLighting.k_UsePointLightCookiesKeyword);
			}
			if (light.normalMapQuality == Light2D.NormalMapQuality.Fast)
			{
				material.EnableKeyword(RendererLighting.k_LightQualityFastKeyword);
			}
			if (light.normalMapQuality != Light2D.NormalMapQuality.Disabled)
			{
				material.EnableKeyword(RendererLighting.k_UseNormalMap);
			}
			if (useShadows)
			{
				material.EnableKeyword(RendererLighting.k_UseShadowMap);
			}
			return material;
		}

		internal static Material GetLightMaterial(this Renderer2DData rendererData, Light2D light, bool isVolume, bool useShadows)
		{
			uint lightMaterialIndex = RendererLighting.GetLightMaterialIndex(light, isVolume, useShadows);
			Material material;
			if (!rendererData.lightMaterials.TryGetValue(lightMaterialIndex, out material))
			{
				material = RendererLighting.CreateLightMaterial(rendererData, light, isVolume, useShadows);
				rendererData.lightMaterials[lightMaterialIndex] = material;
			}
			return material;
		}

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Draw Normals");

		private static readonly ShaderTagId k_NormalsRenderingPassName = new ShaderTagId("NormalsRendering");

		public static readonly Color k_NormalClearColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private static readonly string k_UsePointLightCookiesKeyword = "USE_POINT_LIGHT_COOKIES";

		private static readonly string k_LightQualityFastKeyword = "LIGHT_QUALITY_FAST";

		private static readonly string k_UseNormalMap = "USE_NORMAL_MAP";

		private static readonly string k_UseShadowMap = "USE_SHADOW_MAP";

		private static readonly string k_UseAdditiveBlendingKeyword = "USE_ADDITIVE_BLENDING";

		private static readonly string k_UseVolumetric = "USE_VOLUMETRIC";

		private static readonly string[] k_UseBlendStyleKeywords = new string[]
		{
			"USE_SHAPE_LIGHT_TYPE_0",
			"USE_SHAPE_LIGHT_TYPE_1",
			"USE_SHAPE_LIGHT_TYPE_2",
			"USE_SHAPE_LIGHT_TYPE_3"
		};

		private static readonly int[] k_BlendFactorsPropIDs = new int[]
		{
			Shader.PropertyToID("_ShapeLightBlendFactors0"),
			Shader.PropertyToID("_ShapeLightBlendFactors1"),
			Shader.PropertyToID("_ShapeLightBlendFactors2"),
			Shader.PropertyToID("_ShapeLightBlendFactors3")
		};

		private static readonly int[] k_MaskFilterPropIDs = new int[]
		{
			Shader.PropertyToID("_ShapeLightMaskFilter0"),
			Shader.PropertyToID("_ShapeLightMaskFilter1"),
			Shader.PropertyToID("_ShapeLightMaskFilter2"),
			Shader.PropertyToID("_ShapeLightMaskFilter3")
		};

		private static readonly int[] k_InvertedFilterPropIDs = new int[]
		{
			Shader.PropertyToID("_ShapeLightInvertedFilter0"),
			Shader.PropertyToID("_ShapeLightInvertedFilter1"),
			Shader.PropertyToID("_ShapeLightInvertedFilter2"),
			Shader.PropertyToID("_ShapeLightInvertedFilter3")
		};

		public static readonly string[] k_ShapeLightTextureIDs = new string[]
		{
			"_ShapeLightTexture0",
			"_ShapeLightTexture1",
			"_ShapeLightTexture2",
			"_ShapeLightTexture3"
		};

		private static GraphicsFormat s_RenderTextureFormatToUse = GraphicsFormat.R8G8B8A8_UNorm;

		private static bool s_HasSetupRenderTextureFormatToUse;

		private static readonly int k_SrcBlendID = Shader.PropertyToID("_SrcBlend");

		private static readonly int k_DstBlendID = Shader.PropertyToID("_DstBlend");

		private static readonly int k_CookieTexID = Shader.PropertyToID("_CookieTex");

		private static readonly int k_PointLightCookieTexID = Shader.PropertyToID("_PointLightCookieTex");

		private static readonly int k_L2DInvMatrix = Shader.PropertyToID("L2DInvMatrix");

		private static readonly int k_L2DColor = Shader.PropertyToID("L2DColor");

		private static readonly int k_L2DPosition = Shader.PropertyToID("L2DPosition");

		private static readonly int k_L2DFalloffIntensity = Shader.PropertyToID("L2DFalloffIntensity");

		private static readonly int k_L2DFalloffDistance = Shader.PropertyToID("L2DFalloffDistance");

		private static readonly int k_L2DOuterAngle = Shader.PropertyToID("L2DOuterAngle");

		private static readonly int k_L2DInnerAngle = Shader.PropertyToID("L2DInnerAngle");

		private static readonly int k_L2DInnerRadiusMult = Shader.PropertyToID("L2DInnerRadiusMult");

		private static readonly int k_L2DVolumeOpacity = Shader.PropertyToID("L2DVolumeOpacity");

		private static readonly int k_L2DShadowIntensity = Shader.PropertyToID("L2DShadowIntensity");

		private static readonly int k_L2DLightType = Shader.PropertyToID("L2DLightType");

		internal static LightBatch lightBatch = new LightBatch();
	}
}
