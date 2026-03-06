using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DrawLight2DPass : ScriptableRenderPass
	{
		internal void Setup(RenderGraph renderGraph, ref Renderer2DData rendererData)
		{
			foreach (Light2D light2D in rendererData.lightCullResult.visibleLights)
			{
				if (light2D.useCookieSprite && light2D.m_CookieSpriteTexture != null)
				{
					light2D.m_CookieSpriteTextureHandle = renderGraph.ImportTexture(light2D.m_CookieSpriteTexture);
				}
			}
		}

		private static bool TryGetShadowIndex(ref LayerBatch layerBatch, int lightIndex, out int shadowIndex)
		{
			shadowIndex = 0;
			for (int i = 0; i < layerBatch.shadowIndices.Count; i++)
			{
				if (layerBatch.shadowIndices[i] == lightIndex)
				{
					shadowIndex = i;
					return true;
				}
			}
			return false;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			throw new NotImplementedException();
		}

		private static void Execute(RasterCommandBuffer cmd, DrawLight2DPass.PassData passData, ref LayerBatch layerBatch, int lightTextureIndex)
		{
			cmd.SetGlobalFloat(DrawLight2DPass.k_InverseHDREmulationScaleID, 1f / passData.rendererData.hdrEmulationScale);
			int num = layerBatch.activeBlendStylesIndices[lightTextureIndex];
			string name = passData.rendererData.lightBlendStyles[num].name;
			cmd.BeginSample(name);
			int blendStyleIndex = Renderer2D.supportsMRT ? lightTextureIndex : 0;
			if (!passData.isVolumetric)
			{
				RendererLighting.EnableBlendStyle(cmd, blendStyleIndex, true);
			}
			List<Light2D> lights = passData.layerBatch.lights;
			for (int i = 0; i < lights.Count; i++)
			{
				Light2D light2D = lights[i];
				if (!(light2D == null) && light2D.lightType != Light2D.LightType.Global && light2D.blendStyleIndex == num && (!passData.isVolumetric || (light2D.volumeIntensity > 0f && light2D.volumetricEnabled && layerBatch.endLayerValue == light2D.GetTopMostLitLayer())))
				{
					bool flag = passData.layerBatch.lightStats.useShadows && layerBatch.shadowIndices.Contains(i);
					Material lightMaterial = passData.rendererData.GetLightMaterial(light2D, passData.isVolumetric, flag);
					Mesh lightMesh = light2D.lightMesh;
					int batchSlotIndex = light2D.batchSlotIndex;
					int slot = RendererLighting.lightBatch.SlotIndex(batchSlotIndex);
					int lightHash;
					if (!RendererLighting.lightBatch.CanBatch(light2D, lightMaterial, batchSlotIndex, out lightHash) && LightBatch.isBatchingSupported)
					{
						RendererLighting.lightBatch.Flush(cmd);
					}
					if (passData.layerBatch.lightStats.useNormalMap)
					{
						DrawLight2DPass.s_PropertyBlock.SetTexture(DrawLight2DPass.k_NormalMapID, passData.normalMap);
					}
					int num2;
					if (flag && DrawLight2DPass.TryGetShadowIndex(ref layerBatch, i, out num2))
					{
						DrawLight2DPass.s_PropertyBlock.SetTexture(DrawLight2DPass.k_ShadowMapID, passData.shadowTextures[num2]);
					}
					if (!passData.isVolumetric || (passData.isVolumetric && light2D.volumetricEnabled))
					{
						RendererLighting.SetCookieShaderProperties(light2D, DrawLight2DPass.s_PropertyBlock);
					}
					RendererLighting.SetPerLightShaderGlobals(cmd, light2D, slot, passData.isVolumetric, flag, LightBatch.isBatchingSupported);
					if (light2D.normalMapQuality != Light2D.NormalMapQuality.Disabled || light2D.lightType == Light2D.LightType.Point)
					{
						RendererLighting.SetPerPointLightShaderGlobals(cmd, light2D, slot, LightBatch.isBatchingSupported);
					}
					if (LightBatch.isBatchingSupported)
					{
						RendererLighting.lightBatch.AddBatch(light2D, lightMaterial, light2D.GetMatrix(), lightMesh, 0, lightHash, batchSlotIndex);
						RendererLighting.lightBatch.Flush(cmd);
					}
					else
					{
						cmd.DrawMesh(lightMesh, light2D.GetMatrix(), lightMaterial, 0, 0, DrawLight2DPass.s_PropertyBlock);
					}
				}
			}
			RendererLighting.EnableBlendStyle(cmd, blendStyleIndex, false);
			cmd.EndSample(name);
		}

		private void InitializeRenderPass(IRasterRenderGraphBuilder builder, ContextContainer frameData, DrawLight2DPass.PassData passData, Renderer2DData rendererData, ref LayerBatch layerBatch, int batchIndex, bool isVolumetric = false)
		{
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			this.intermediateTexture[0] = universalResourceData.activeColorTexture;
			if (layerBatch.lightStats.useNormalMap)
			{
				builder.UseTexture(universal2DResourceData.normalsTexture[batchIndex], AccessFlags.Read);
			}
			if (layerBatch.lightStats.useShadows)
			{
				passData.shadowTextures = universal2DResourceData.shadowTextures[batchIndex];
				for (int i = 0; i < passData.shadowTextures.Length; i++)
				{
					builder.UseTexture(passData.shadowTextures[i], AccessFlags.Read);
				}
			}
			foreach (Light2D light2D in layerBatch.lights)
			{
				if (!(light2D == null) && light2D.m_CookieSpriteTextureHandle.IsValid() && (!isVolumetric || (isVolumetric && light2D.volumetricEnabled)))
				{
					builder.UseTexture(light2D.m_CookieSpriteTextureHandle, AccessFlags.Read);
				}
			}
			passData.layerBatch = layerBatch;
			passData.rendererData = rendererData;
			passData.isVolumetric = isVolumetric;
			passData.normalMap = (layerBatch.lightStats.useNormalMap ? universal2DResourceData.normalsTexture[batchIndex] : TextureHandle.nullHandle);
			builder.AllowGlobalStateModification(true);
		}

		internal void Render(RenderGraph graph, ContextContainer frameData, Renderer2DData rendererData, ref LayerBatch layerBatch, int batchIndex, bool isVolumetric = false)
		{
			Universal2DResourceData universal2DResourceData = frameData.Get<Universal2DResourceData>();
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(frameData.Get<UniversalCameraData>());
			bool flag = activeDebugHandler == null || activeDebugHandler.IsLightingActive;
			if (!layerBatch.lightStats.useLights || (isVolumetric && !layerBatch.lightStats.useVolumetricLights) || !flag)
			{
				return;
			}
			if (!isVolumetric && !Renderer2D.supportsMRT)
			{
				for (int i = 0; i < layerBatch.activeBlendStylesIndices.Length; i++)
				{
					DrawLight2DPass.PassData passData;
					using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<DrawLight2DPass.PassData>(DrawLight2DPass.k_LightSRTPass, out passData, DrawLight2DPass.m_ProfilingSampleSRT, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\DrawLight2DPass.cs", 202))
					{
						this.InitializeRenderPass(rasterRenderGraphBuilder, frameData, passData, rendererData, ref layerBatch, batchIndex, isVolumetric);
						TextureHandle[] array = universal2DResourceData.lightTextures[batchIndex];
						rasterRenderGraphBuilder.SetRenderAttachment(array[i], 0, AccessFlags.Write);
						passData.lightTextureIndex = i;
						rasterRenderGraphBuilder.SetRenderFunc<DrawLight2DPass.PassData>(delegate(DrawLight2DPass.PassData data, RasterGraphContext context)
						{
							DrawLight2DPass.Execute(context.cmd, data, ref data.layerBatch, data.lightTextureIndex);
						});
					}
				}
				return;
			}
			DrawLight2DPass.PassData passData2;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder2 = graph.AddRasterRenderPass<DrawLight2DPass.PassData>((!isVolumetric) ? DrawLight2DPass.k_LightPass : DrawLight2DPass.k_LightVolumetricPass, out passData2, (!isVolumetric) ? DrawLight2DPass.m_ProfilingSampler : DrawLight2DPass.m_ProfilingSamplerVolume, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\2D\\Rendergraph\\DrawLight2DPass.cs", 222))
			{
				this.InitializeRenderPass(rasterRenderGraphBuilder2, frameData, passData2, rendererData, ref layerBatch, batchIndex, isVolumetric);
				TextureHandle[] array2 = (!isVolumetric) ? universal2DResourceData.lightTextures[batchIndex] : this.intermediateTexture;
				for (int j = 0; j < array2.Length; j++)
				{
					rasterRenderGraphBuilder2.SetRenderAttachment(array2[j], j, AccessFlags.Write);
				}
				rasterRenderGraphBuilder2.SetRenderFunc<DrawLight2DPass.PassData>(delegate(DrawLight2DPass.PassData data, RasterGraphContext context)
				{
					for (int k = 0; k < data.layerBatch.activeBlendStylesIndices.Length; k++)
					{
						DrawLight2DPass.Execute(context.cmd, data, ref data.layerBatch, k);
					}
				});
			}
		}

		private static readonly string k_LightPass = "Light2D Pass";

		private static readonly string k_LightSRTPass = "Light2D SRT Pass";

		private static readonly string k_LightVolumetricPass = "Light2D Volumetric Pass";

		private static readonly ProfilingSampler m_ProfilingSampler = new ProfilingSampler(DrawLight2DPass.k_LightPass);

		private static readonly ProfilingSampler m_ProfilingSampleSRT = new ProfilingSampler(DrawLight2DPass.k_LightSRTPass);

		private static readonly ProfilingSampler m_ProfilingSamplerVolume = new ProfilingSampler(DrawLight2DPass.k_LightVolumetricPass);

		internal static readonly int k_InverseHDREmulationScaleID = Shader.PropertyToID("_InverseHDREmulationScale");

		internal static readonly string k_NormalMapID = "_NormalMap";

		internal static readonly string k_ShadowMapID = "_ShadowTex";

		private TextureHandle[] intermediateTexture = new TextureHandle[1];

		internal static MaterialPropertyBlock s_PropertyBlock = new MaterialPropertyBlock();

		internal class PassData
		{
			internal LayerBatch layerBatch;

			internal Renderer2DData rendererData;

			internal bool isVolumetric;

			internal TextureHandle normalMap;

			internal TextureHandle[] shadowTextures;

			internal int lightTextureIndex;
		}
	}
}
