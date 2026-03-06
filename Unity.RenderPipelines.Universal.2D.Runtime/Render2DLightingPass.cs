using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal class Render2DLightingPass : ScriptableRenderPass, IRenderPass2D
	{
		public Render2DLightingPass(Renderer2DData rendererData, Material blitMaterial, Material samplingMaterial, Texture2D fallOffLookup)
		{
			this.m_Renderer2DData = rendererData;
			this.m_BlitMaterial = blitMaterial;
			this.m_SamplingMaterial = samplingMaterial;
			this.m_FallOffLookup = fallOffLookup;
			this.m_CameraSortingLayerBoundsIndex = Render2DLightingPass.GetCameraSortingLayerBoundsIndex(this.m_Renderer2DData);
		}

		internal void Setup(bool useDepth)
		{
			this.m_NeedsDepth = useDepth;
		}

		private unsafe void CopyCameraSortingLayerRenderTexture(ScriptableRenderContext context, RenderingData renderingData, RenderBufferStoreAction mainTargetStoreAction)
		{
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			this.CreateCameraSortingLayerRenderTexture(renderingData, commandBuffer, this.m_Renderer2DData.cameraSortingLayerDownsamplingMethod);
			Material material = this.m_SamplingMaterial;
			int pass = 0;
			if (this.m_Renderer2DData.cameraSortingLayerDownsamplingMethod != Downsampling._4xBox)
			{
				material = this.m_BlitMaterial;
				pass = ((base.colorAttachmentHandle.rt.filterMode == FilterMode.Bilinear) ? 1 : 0);
			}
			Blitter.BlitCameraTexture(commandBuffer, base.colorAttachmentHandle, this.m_Renderer2DData.cameraSortingLayerRenderTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, material, pass);
			CoreUtils.SetRenderTarget(commandBuffer, base.colorAttachmentHandle, RenderBufferLoadAction.Load, mainTargetStoreAction, base.depthAttachmentHandle, RenderBufferLoadAction.Load, mainTargetStoreAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
			commandBuffer.SetGlobalTexture(this.m_Renderer2DData.cameraSortingLayerRenderTarget.name, this.m_Renderer2DData.cameraSortingLayerRenderTarget.nameID);
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		public static short GetCameraSortingLayerBoundsIndex(Renderer2DData rendererData)
		{
			SortingLayer[] cachedSortingLayer = Light2DManager.GetCachedSortingLayer();
			short num = 0;
			while ((int)num < cachedSortingLayer.Length)
			{
				if (cachedSortingLayer[(int)num].id == rendererData.cameraSortingLayerTextureBound)
				{
					return (short)cachedSortingLayer[(int)num].value;
				}
				num += 1;
			}
			return short.MinValue;
		}

		private void DetermineWhenToResolve(int startIndex, int batchesDrawn, int batchCount, LayerBatch[] layerBatches, out int resolveDuringBatch, out bool resolveIsAfterCopy)
		{
			bool flag = false;
			List<Light2D> visibleLights = this.m_Renderer2DData.lightCullResult.visibleLights;
			for (int i = 0; i < visibleLights.Count; i++)
			{
				flag = visibleLights[i].renderVolumetricShadows;
				if (flag)
				{
					break;
				}
			}
			int num = -1;
			if (flag)
			{
				for (int j = startIndex + batchesDrawn - 1; j >= startIndex; j--)
				{
					if (layerBatches[j].lightStats.totalVolumetricUsage > 0)
					{
						num = j;
						break;
					}
				}
			}
			if (this.m_Renderer2DData.useCameraSortingLayerTexture)
			{
				short cameraSortingLayerBoundsIndex = Render2DLightingPass.GetCameraSortingLayerBoundsIndex(this.m_Renderer2DData);
				int num2 = -1;
				for (int k = startIndex; k < startIndex + batchesDrawn; k++)
				{
					LayerBatch layerBatch = layerBatches[k];
					if (cameraSortingLayerBoundsIndex >= layerBatch.layerRange.lowerBound && cameraSortingLayerBoundsIndex <= layerBatch.layerRange.upperBound)
					{
						num2 = k;
						break;
					}
				}
				resolveIsAfterCopy = (num2 > num);
				resolveDuringBatch = (resolveIsAfterCopy ? num2 : num);
				return;
			}
			resolveDuringBatch = num;
			resolveIsAfterCopy = false;
		}

		private unsafe void Render(ScriptableRenderContext context, CommandBuffer cmd, ref RenderingData renderingData, ref FilteringSettings filterSettings, DrawingSettings drawSettings)
		{
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(renderingData.frameData.Get<UniversalCameraData>());
			if (activeDebugHandler != null)
			{
				UniversalRenderingData universalRenderingData = renderingData.universalRenderingData;
				RenderStateBlock renderStateBlock = default(RenderStateBlock);
				activeDebugHandler.CreateRendererListsWithDebugRenderState(context, ref universalRenderingData.cullResults, ref drawSettings, ref filterSettings, ref renderStateBlock).DrawWithRendererList(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer));
				return;
			}
			RendererListParams rendererListParams = new RendererListParams(*renderingData.cullResults, drawSettings, filterSettings);
			RendererList rendererList = context.CreateRendererList(ref rendererListParams);
			cmd.DrawRendererList(rendererList);
		}

		private int DrawLayerBatches(LayerBatch[] layerBatches, int batchCount, int startIndex, CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData, ref DrawingSettings normalsDrawSettings, ref DrawingSettings drawSettings, ref RenderTextureDescriptor desc)
		{
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(renderingData.frameData.Get<UniversalCameraData>());
			bool flag = activeDebugHandler == null || activeDebugHandler.IsLightingActive;
			int num = 0;
			uint num2 = 0U;
			bool bFirstClear = true;
			using (new ProfilingScope(cmd, Render2DLightingPass.m_ProfilingDrawLights))
			{
				for (int i = startIndex; i < batchCount; i++)
				{
					ref LayerBatch ptr = ref layerBatches[i];
					uint num3 = ptr.lightStats.blendStylesUsed;
					uint num4 = 0U;
					while (num3 > 0U)
					{
						num4 += (num3 & 1U);
						num3 >>= 1;
					}
					num2 += num4;
					if (num2 > LayerUtility.maxTextureCount)
					{
						break;
					}
					num++;
					if (ptr.useNormals)
					{
						FilteringSettings filterSettings;
						LayerUtility.GetFilterSettings(this.m_Renderer2DData, ref ptr, out filterSettings);
						RTHandle depthTarget = this.m_NeedsDepth ? base.depthAttachmentHandle : null;
						this.RenderNormals(context, renderingData, normalsDrawSettings, filterSettings, depthTarget, bFirstClear);
						bFirstClear = false;
					}
					using (new ProfilingScope(cmd, Render2DLightingPass.m_ProfilingDrawLightTextures))
					{
						this.RenderLights(renderingData, cmd, ref ptr, ref desc);
					}
				}
			}
			bool flag2 = renderingData.cameraData.cameraTargetDescriptor.msaaSamples > 1;
			bool flag3 = startIndex + num >= batchCount;
			int num5 = -1;
			bool flag4 = false;
			if (flag2 && flag3)
			{
				this.DetermineWhenToResolve(startIndex, num, batchCount, layerBatches, out num5, out flag4);
			}
			int num6 = this.m_Renderer2DData.lightBlendStyles.Length;
			using (new ProfilingScope(cmd, Render2DLightingPass.m_ProfilingDrawRenderers))
			{
				RenderBufferStoreAction renderBufferStoreAction;
				if (flag2)
				{
					renderBufferStoreAction = ((num5 < startIndex) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.StoreAndResolve);
				}
				else
				{
					renderBufferStoreAction = RenderBufferStoreAction.Store;
				}
				CoreUtils.SetRenderTarget(cmd, base.colorAttachmentHandle, RenderBufferLoadAction.Load, renderBufferStoreAction, base.depthAttachmentHandle, RenderBufferLoadAction.Load, renderBufferStoreAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
				for (int j = startIndex; j < startIndex + num; j++)
				{
					using (new ProfilingScope(cmd, Render2DLightingPass.m_ProfilingDrawLayerBatch))
					{
						LayerBatch layerBatch = layerBatches[j];
						if (layerBatch.lightStats.useLights)
						{
							for (int k = 0; k < num6; k++)
							{
								uint num7 = 1U << k;
								bool flag5 = (layerBatch.lightStats.blendStylesUsed & num7) > 0U;
								if (flag5)
								{
									RenderTargetIdentifier rtid = layerBatch.GetRTId(cmd, desc, k);
									cmd.SetGlobalTexture(Render2DLightingPass.k_ShapeLightTextureIDs[k], rtid);
								}
								RendererLighting.EnableBlendStyle(CommandBufferHelpers.GetRasterCommandBuffer(cmd), k, flag5);
							}
						}
						else
						{
							for (int l = 0; l < Render2DLightingPass.k_ShapeLightTextureIDs.Length; l++)
							{
								cmd.SetGlobalTexture(Render2DLightingPass.k_ShapeLightTextureIDs[l], Texture2D.blackTexture);
								RendererLighting.EnableBlendStyle(CommandBufferHelpers.GetRasterCommandBuffer(cmd), l, l == 0);
							}
						}
						context.ExecuteCommandBuffer(cmd);
						cmd.Clear();
						short cameraSortingLayerBoundsIndex = Render2DLightingPass.GetCameraSortingLayerBoundsIndex(this.m_Renderer2DData);
						RenderBufferStoreAction mainTargetStoreAction;
						if (flag2)
						{
							mainTargetStoreAction = ((num5 == j && flag4) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.StoreAndResolve);
						}
						else
						{
							mainTargetStoreAction = RenderBufferStoreAction.Store;
						}
						FilteringSettings filteringSettings;
						LayerUtility.GetFilterSettings(this.m_Renderer2DData, ref layerBatch, out filteringSettings);
						this.Render(context, cmd, ref renderingData, ref filteringSettings, drawSettings);
						if (this.m_Renderer2DData.useCameraSortingLayerTexture && cameraSortingLayerBoundsIndex >= layerBatch.layerRange.lowerBound && cameraSortingLayerBoundsIndex <= layerBatch.layerRange.upperBound)
						{
							this.CopyCameraSortingLayerRenderTexture(context, renderingData, mainTargetStoreAction);
						}
						RendererLighting.DisableAllKeywords(CommandBufferHelpers.GetRasterCommandBuffer(cmd));
						if (flag && layerBatch.lightStats.totalVolumetricUsage > 0)
						{
							string name = "Render 2D Light Volumes";
							cmd.BeginSample(name);
							RenderBufferStoreAction finalStoreAction;
							if (flag2)
							{
								finalStoreAction = ((num5 == j && !flag4) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.StoreAndResolve);
							}
							else
							{
								finalStoreAction = RenderBufferStoreAction.Store;
							}
							this.RenderLightVolumes(renderingData, cmd, ref layerBatch, base.colorAttachmentHandle.nameID, base.depthAttachmentHandle.nameID, RenderBufferStoreAction.Store, finalStoreAction, false, this.m_Renderer2DData.lightCullResult.visibleLights);
							cmd.EndSample(name);
						}
					}
				}
			}
			for (int m = startIndex; m < startIndex + num; m++)
			{
				layerBatches[m].ReleaseRT(cmd);
			}
			return num;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			bool flag = true;
			Camera camera = *renderingData.cameraData.camera;
			FilteringSettings defaultValue = FilteringSettings.defaultValue;
			defaultValue.renderQueueRange = RenderQueueRange.all;
			defaultValue.layerMask = -1;
			defaultValue.renderingLayerMask = uint.MaxValue;
			defaultValue.sortingLayerRange = SortingLayerRange.all;
			LayerUtility.InitializeBudget(this.m_Renderer2DData.lightRenderTextureMemoryBudget);
			ShadowRendering.InitializeBudget(this.m_Renderer2DData.shadowRenderTextureMemoryBudget);
			RendererLighting.lightBatch.Reset();
			PixelPerfectCamera pixelPerfectCamera;
			camera.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
			if (pixelPerfectCamera != null && pixelPerfectCamera.enabled && pixelPerfectCamera.offscreenRTSize != Vector2Int.zero)
			{
				int x = pixelPerfectCamera.offscreenRTSize.x;
				int y = pixelPerfectCamera.offscreenRTSize.y;
				renderingData.commandBuffer->SetGlobalVector(ShaderPropertyId.screenParams, new Vector4((float)x, (float)y, 1f + 1f / (float)x, 1f + 1f / (float)y));
			}
			if (this.m_Renderer2DData.lightCullResult.IsSceneLit() && flag)
			{
				DrawingSettings drawingSettings = base.CreateDrawingSettings(Render2DLightingPass.k_ShaderTags, ref renderingData, SortingCriteria.CommonTransparent);
				DrawingSettings drawingSettings2 = base.CreateDrawingSettings(Render2DLightingPass.k_NormalsRenderingPassName, ref renderingData, SortingCriteria.CommonTransparent);
				SortingSettings sortingSettings = drawingSettings.sortingSettings;
				RendererLighting.GetTransparencySortingMode(this.m_Renderer2DData, camera, ref sortingSettings);
				drawingSettings.sortingSettings = sortingSettings;
				drawingSettings2.sortingSettings = sortingSettings;
				CommandBuffer commandBuffer = *renderingData.commandBuffer;
				commandBuffer.SetGlobalFloat(Render2DLightingPass.k_HDREmulationScaleID, this.m_Renderer2DData.hdrEmulationScale);
				commandBuffer.SetGlobalFloat(Render2DLightingPass.k_InverseHDREmulationScaleID, 1f / this.m_Renderer2DData.hdrEmulationScale);
				commandBuffer.SetGlobalColor(Render2DLightingPass.k_RendererColorID, Color.white);
				commandBuffer.SetGlobalTexture(Render2DLightingPass.k_FalloffLookupID, this.m_FallOffLookup);
				commandBuffer.SetGlobalTexture(Render2DLightingPass.k_LightLookupID, Light2DLookupTexture.GetLightLookupTexture());
				RendererLighting.SetLightShaderGlobals(this.m_Renderer2DData, CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer));
				RenderTextureDescriptor blendStyleRenderTextureDesc = this.GetBlendStyleRenderTextureDesc(renderingData);
				ShadowRendering.CallOnBeforeRender(*renderingData.cameraData.camera, this.m_Renderer2DData.lightCullResult);
				int num;
				LayerBatch[] layerBatches = LayerUtility.CalculateBatches(this.m_Renderer2DData, out num);
				int num2;
				for (int i = 0; i < num; i += num2)
				{
					num2 = this.DrawLayerBatches(layerBatches, num, i, commandBuffer, context, ref renderingData, ref drawingSettings2, ref drawingSettings, ref blendStyleRenderTextureDesc);
				}
				RendererLighting.DisableAllKeywords(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer));
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
			}
			else
			{
				DrawingSettings drawSettings = base.CreateDrawingSettings(Render2DLightingPass.k_ShaderTags, ref renderingData, SortingCriteria.CommonTransparent);
				RenderBufferStoreAction renderBufferStoreAction = (renderingData.cameraData.cameraTargetDescriptor.msaaSamples > 1) ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.Store;
				SortingSettings sortingSettings2 = drawSettings.sortingSettings;
				RendererLighting.GetTransparencySortingMode(this.m_Renderer2DData, camera, ref sortingSettings2);
				drawSettings.sortingSettings = sortingSettings2;
				CommandBuffer commandBuffer2 = *renderingData.commandBuffer;
				using (new ProfilingScope(commandBuffer2, Render2DLightingPass.m_ProfilingSamplerUnlit))
				{
					CoreUtils.SetRenderTarget(commandBuffer2, base.colorAttachmentHandle, RenderBufferLoadAction.Load, renderBufferStoreAction, base.depthAttachmentHandle, RenderBufferLoadAction.Load, renderBufferStoreAction, ClearFlag.None, Color.clear, 0, CubemapFace.Unknown, -1);
					commandBuffer2.SetGlobalColor(Render2DLightingPass.k_RendererColorID, Color.white);
					for (int j = 0; j < Render2DLightingPass.k_ShapeLightTextureIDs.Length; j++)
					{
						if (j == 0)
						{
							commandBuffer2.SetGlobalTexture(Render2DLightingPass.k_ShapeLightTextureIDs[j], Texture2D.blackTexture);
						}
						RendererLighting.EnableBlendStyle(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer2), j, j == 0);
					}
				}
				RendererLighting.DisableAllKeywords(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer2));
				context.ExecuteCommandBuffer(commandBuffer2);
				commandBuffer2.Clear();
				if (this.m_Renderer2DData.useCameraSortingLayerTexture)
				{
					defaultValue.sortingLayerRange = new SortingLayerRange(short.MinValue, this.m_CameraSortingLayerBoundsIndex);
					this.Render(context, commandBuffer2, ref renderingData, ref defaultValue, drawSettings);
					this.CopyCameraSortingLayerRenderTexture(context, renderingData, renderBufferStoreAction);
					defaultValue.sortingLayerRange = new SortingLayerRange(this.m_CameraSortingLayerBoundsIndex + 1, short.MaxValue);
					this.Render(context, commandBuffer2, ref renderingData, ref defaultValue, drawSettings);
				}
				else
				{
					this.Render(context, commandBuffer2, ref renderingData, ref defaultValue, drawSettings);
				}
			}
			defaultValue.sortingLayerRange = SortingLayerRange.all;
			RendererList nullRendererList = RendererList.nullRendererList;
		}

		Renderer2DData IRenderPass2D.rendererData
		{
			get
			{
				return this.m_Renderer2DData;
			}
		}

		public void Dispose()
		{
			RTHandle normalsRenderTarget = this.m_Renderer2DData.normalsRenderTarget;
			if (normalsRenderTarget != null)
			{
				normalsRenderTarget.Release();
			}
			this.m_Renderer2DData.normalsRenderTarget = null;
			RTHandle cameraSortingLayerRenderTarget = this.m_Renderer2DData.cameraSortingLayerRenderTarget;
			if (cameraSortingLayerRenderTarget != null)
			{
				cameraSortingLayerRenderTarget.Release();
			}
			this.m_Renderer2DData.cameraSortingLayerRenderTarget = null;
		}

		private static readonly int k_HDREmulationScaleID = Shader.PropertyToID("_HDREmulationScale");

		private static readonly int k_InverseHDREmulationScaleID = Shader.PropertyToID("_InverseHDREmulationScale");

		private static readonly int k_RendererColorID = Shader.PropertyToID("_RendererColor");

		private static readonly int k_LightLookupID = Shader.PropertyToID("_LightLookup");

		private static readonly int k_FalloffLookupID = Shader.PropertyToID("_FalloffLookup");

		private static readonly int[] k_ShapeLightTextureIDs = new int[]
		{
			Shader.PropertyToID("_ShapeLightTexture0"),
			Shader.PropertyToID("_ShapeLightTexture1"),
			Shader.PropertyToID("_ShapeLightTexture2"),
			Shader.PropertyToID("_ShapeLightTexture3")
		};

		private static readonly ShaderTagId k_CombinedRenderingPassName = new ShaderTagId("Universal2D");

		private static readonly ShaderTagId k_NormalsRenderingPassName = new ShaderTagId("NormalsRendering");

		private static readonly ShaderTagId k_LegacyPassName = new ShaderTagId("SRPDefaultUnlit");

		private static readonly List<ShaderTagId> k_ShaderTags = new List<ShaderTagId>
		{
			Render2DLightingPass.k_LegacyPassName,
			Render2DLightingPass.k_CombinedRenderingPassName
		};

		private static readonly ProfilingSampler m_ProfilingDrawLights = new ProfilingSampler("Draw 2D Lights");

		private static readonly ProfilingSampler m_ProfilingDrawLightTextures = new ProfilingSampler("Draw 2D Lights Textures");

		private static readonly ProfilingSampler m_ProfilingDrawRenderers = new ProfilingSampler("Draw All Renderers");

		private static readonly ProfilingSampler m_ProfilingDrawLayerBatch = new ProfilingSampler("Draw Layer Batch");

		private static readonly ProfilingSampler m_ProfilingSamplerUnlit = new ProfilingSampler("Render Unlit");

		private Material m_BlitMaterial;

		private Material m_SamplingMaterial;

		private readonly Renderer2DData m_Renderer2DData;

		private readonly Texture2D m_FallOffLookup;

		private bool m_NeedsDepth;

		private short m_CameraSortingLayerBoundsIndex;
	}
}
