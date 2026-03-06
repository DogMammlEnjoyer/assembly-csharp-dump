using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	[SupportedOnRenderer(typeof(UniversalRendererData))]
	[DisallowMultipleRendererFeature("Screen Space Shadows")]
	[Tooltip("Screen Space Shadows")]
	internal class ScreenSpaceShadows : ScriptableRendererFeature
	{
		public override void Create()
		{
			if (this.m_SSShadowsPass == null)
			{
				this.m_SSShadowsPass = new ScreenSpaceShadows.ScreenSpaceShadowsPass();
			}
			if (this.m_SSShadowsPostPass == null)
			{
				this.m_SSShadowsPostPass = new ScreenSpaceShadows.ScreenSpaceShadowsPostPass();
			}
			this.LoadMaterial();
			this.m_SSShadowsPass.renderPassEvent = RenderPassEvent.BeforeRenderingGbuffer;
			this.m_SSShadowsPostPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
		}

		public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
			{
				return;
			}
			if (!this.LoadMaterial())
			{
				Debug.LogErrorFormat("{0}.AddRenderPasses(): Missing material. {1} render pass will not be added. Check for missing reference in the renderer resources.", new object[]
				{
					base.GetType().Name,
					base.name
				});
				return;
			}
			if (*renderingData.shadowData.supportsMainLightShadows && *renderingData.lightData.mainLightIndex != -1 && this.m_SSShadowsPass.Setup(this.m_Settings, this.m_Material))
			{
				UniversalRenderer universalRenderer = renderer as UniversalRenderer;
				bool flag = universalRenderer != null && universalRenderer.usesDeferredLighting;
				this.m_SSShadowsPass.renderPassEvent = (flag ? RenderPassEvent.BeforeRenderingGbuffer : ((RenderPassEvent)201));
				renderer.EnqueuePass(this.m_SSShadowsPass);
				renderer.EnqueuePass(this.m_SSShadowsPostPass);
			}
		}

		protected override void Dispose(bool disposing)
		{
			ScreenSpaceShadows.ScreenSpaceShadowsPass ssshadowsPass = this.m_SSShadowsPass;
			if (ssshadowsPass != null)
			{
				ssshadowsPass.Dispose();
			}
			this.m_SSShadowsPass = null;
			CoreUtils.Destroy(this.m_Material);
		}

		private bool LoadMaterial()
		{
			if (this.m_Material != null)
			{
				return true;
			}
			if (this.m_Shader == null)
			{
				this.m_Shader = Shader.Find("Hidden/Universal Render Pipeline/ScreenSpaceShadows");
				if (this.m_Shader == null)
				{
					return false;
				}
			}
			this.m_Material = CoreUtils.CreateEngineMaterial(this.m_Shader);
			return this.m_Material != null;
		}

		[SerializeField]
		[HideInInspector]
		private Shader m_Shader;

		[SerializeField]
		private ScreenSpaceShadowsSettings m_Settings = new ScreenSpaceShadowsSettings();

		private Material m_Material;

		private ScreenSpaceShadows.ScreenSpaceShadowsPass m_SSShadowsPass;

		private ScreenSpaceShadows.ScreenSpaceShadowsPostPass m_SSShadowsPostPass;

		private const string k_ShaderName = "Hidden/Universal Render Pipeline/ScreenSpaceShadows";

		private class ScreenSpaceShadowsPass : ScriptableRenderPass
		{
			internal ScreenSpaceShadowsPass()
			{
				base.profilingSampler = new ProfilingSampler("Blit Screen Space Shadows");
				this.m_CurrentSettings = new ScreenSpaceShadowsSettings();
				this.m_ScreenSpaceShadowmapTextureID = Shader.PropertyToID("_ScreenSpaceShadowmapTexture");
				this.m_PassData = new ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData();
			}

			public void Dispose()
			{
				RTHandle renderTarget = this.m_RenderTarget;
				if (renderTarget == null)
				{
					return;
				}
				renderTarget.Release();
			}

			internal bool Setup(ScreenSpaceShadowsSettings featureSettings, Material material)
			{
				this.m_CurrentSettings = featureSettings;
				this.m_Material = material;
				base.ConfigureInput(ScriptableRenderPassInput.Depth);
				return this.m_Material != null;
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				RenderTextureDescriptor renderTextureDescriptor = *renderingData.cameraData.cameraTargetDescriptor;
				renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
				renderTextureDescriptor.msaaSamples = 1;
				renderTextureDescriptor.graphicsFormat = (SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, GraphicsFormatUsage.Blend) ? GraphicsFormat.R8_UNorm : GraphicsFormat.B8G8R8A8_UNorm);
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_RenderTarget, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Clamp, 1, 0f, "_ScreenSpaceShadowmapTexture");
				cmd.SetGlobalTexture(this.m_RenderTarget.name, this.m_RenderTarget.nameID);
				base.ConfigureTarget(this.m_RenderTarget);
				base.ConfigureClear(ClearFlag.None, Color.white);
			}

			private void InitPassData(ref ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData passData)
			{
				passData.material = this.m_Material;
				passData.shadowmapID = this.m_ScreenSpaceShadowmapTextureID;
			}

			public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
			{
				if (this.m_Material == null)
				{
					Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceShadows pass will not execute. Check for missing reference in the renderer resources.", new object[]
					{
						base.GetType().Name
					});
					return;
				}
				RenderTextureDescriptor cameraTargetDescriptor = frameData.Get<UniversalCameraData>().cameraTargetDescriptor;
				cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
				cameraTargetDescriptor.msaaSamples = 1;
				cameraTargetDescriptor.graphicsFormat = (SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, GraphicsFormatUsage.Blend) ? GraphicsFormat.R8_UNorm : GraphicsFormat.B8G8R8A8_UNorm);
				TextureHandle target = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_ScreenSpaceShadowmapTexture", true, FilterMode.Point, TextureWrapMode.Clamp);
				ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData passData;
				using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\RendererFeatures\\ScreenSpaceShadows.cs", 203))
				{
					passData.target = target;
					unsafeRenderGraphBuilder.UseTexture(target, AccessFlags.WriteAll);
					this.InitPassData(ref passData);
					unsafeRenderGraphBuilder.AllowGlobalStateModification(true);
					if (target.IsValid())
					{
						unsafeRenderGraphBuilder.SetGlobalTextureAfterPass(target, this.m_ScreenSpaceShadowmapTextureID);
					}
					unsafeRenderGraphBuilder.SetRenderFunc<ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData>(delegate(ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData data, UnsafeGraphContext rgContext)
					{
						ScreenSpaceShadows.ScreenSpaceShadowsPass.ExecutePass(rgContext.cmd, data, data.target);
					});
				}
			}

			private static void ExecutePass(RasterCommandBuffer cmd, ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData data, RTHandle target)
			{
				Blitter.BlitTexture(cmd, target, Vector2.one, data.material, 0);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadows, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowCascades, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowScreen, true);
			}

			private static void ExecutePass(UnsafeCommandBuffer cmd, ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData data, RTHandle target)
			{
				Blitter.BlitTexture(cmd, target, Vector2.one, data.material, 0);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadows, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowCascades, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowScreen, true);
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (this.m_Material == null)
				{
					Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceShadows pass will not execute. Check for missing reference in the renderer resources.", new object[]
					{
						base.GetType().Name
					});
					return;
				}
				this.InitPassData(ref this.m_PassData);
				CommandBuffer cmd = *renderingData.commandBuffer;
				using (new ProfilingScope(cmd, base.profilingSampler))
				{
					ScreenSpaceShadows.ScreenSpaceShadowsPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, this.m_RenderTarget);
				}
			}

			private Material m_Material;

			private ScreenSpaceShadowsSettings m_CurrentSettings;

			private RTHandle m_RenderTarget;

			private int m_ScreenSpaceShadowmapTextureID;

			private ScreenSpaceShadows.ScreenSpaceShadowsPass.PassData m_PassData;

			private class PassData
			{
				internal TextureHandle target;

				internal Material material;

				internal int shadowmapID;
			}
		}

		private class ScreenSpaceShadowsPostPass : ScriptableRenderPass
		{
			internal ScreenSpaceShadowsPostPass()
			{
				base.profilingSampler = new ProfilingSampler("Set Screen Space Shadow Keywords");
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
				base.ConfigureTarget(ScreenSpaceShadows.ScreenSpaceShadowsPostPass.k_CurrentActive);
			}

			private static void ExecutePass(RasterCommandBuffer cmd, UniversalShadowData shadowData)
			{
				int mainLightShadowCascadesCount = shadowData.mainLightShadowCascadesCount;
				bool supportsMainLightShadows = shadowData.supportsMainLightShadows;
				bool value = supportsMainLightShadows && mainLightShadowCascadesCount == 1;
				bool value2 = supportsMainLightShadows && mainLightShadowCascadesCount > 1;
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowScreen, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadows, value);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowCascades, value2);
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				CommandBuffer cmd = *renderingData.commandBuffer;
				UniversalShadowData shadowData = renderingData.frameData.Get<UniversalShadowData>();
				using (new ProfilingScope(cmd, base.profilingSampler))
				{
					ScreenSpaceShadows.ScreenSpaceShadowsPostPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), shadowData);
				}
			}

			public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
			{
				ScreenSpaceShadows.ScreenSpaceShadowsPostPass.PassData passData;
				using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<ScreenSpaceShadows.ScreenSpaceShadowsPostPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\RendererFeatures\\ScreenSpaceShadows.cs", 308))
				{
					TextureHandle activeColorTexture = frameData.Get<UniversalResourceData>().activeColorTexture;
					rasterRenderGraphBuilder.SetRenderAttachment(activeColorTexture, 0, AccessFlags.Write);
					passData.shadowData = frameData.Get<UniversalShadowData>();
					passData.pass = this;
					rasterRenderGraphBuilder.AllowGlobalStateModification(true);
					rasterRenderGraphBuilder.SetRenderFunc<ScreenSpaceShadows.ScreenSpaceShadowsPostPass.PassData>(delegate(ScreenSpaceShadows.ScreenSpaceShadowsPostPass.PassData data, RasterGraphContext rgContext)
					{
						ScreenSpaceShadows.ScreenSpaceShadowsPostPass.ExecutePass(rgContext.cmd, data.shadowData);
					});
				}
			}

			private static readonly RTHandle k_CurrentActive = RTHandles.Alloc(BuiltinRenderTextureType.CurrentActive);

			internal class PassData
			{
				internal ScreenSpaceShadows.ScreenSpaceShadowsPostPass pass;

				internal UniversalShadowData shadowData;
			}
		}
	}
}
