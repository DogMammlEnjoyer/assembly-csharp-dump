using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal sealed class MotionVectorRenderPass : ScriptableRenderPass
	{
		internal MotionVectorRenderPass(RenderPassEvent evt, Material cameraMaterial, LayerMask opaqueLayerMask)
		{
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.DrawMotionVectors);
			base.renderPassEvent = evt;
			this.m_CameraMaterial = cameraMaterial;
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), opaqueLayerMask, uint.MaxValue, 0);
			this.m_PassData = new MotionVectorRenderPass.PassData();
			base.ConfigureInput(ScriptableRenderPassInput.Depth);
		}

		internal void Setup(RTHandle color, RTHandle depth)
		{
			this.m_Color = color;
			this.m_Depth = depth;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			cmd.SetGlobalTexture(this.m_Color.name, this.m_Color.nameID);
			cmd.SetGlobalTexture(this.m_Depth.name, this.m_Depth.nameID);
			base.ConfigureTarget(this.m_Color, this.m_Depth);
			base.ConfigureClear(ClearFlag.Color | ClearFlag.Depth, Color.black);
			base.ConfigureDepthStoreAction(RenderBufferStoreAction.DontCare);
		}

		private static void ExecutePass(RasterCommandBuffer cmd, MotionVectorRenderPass.PassData passData, RendererList rendererList)
		{
			Material cameraMaterial = passData.cameraMaterial;
			if (cameraMaterial == null)
			{
				return;
			}
			Camera camera = passData.camera;
			if (camera.cameraType == CameraType.Preview)
			{
				return;
			}
			camera.depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.MotionVectors);
			MotionVectorRenderPass.DrawCameraMotionVectors(cmd, passData.xr, cameraMaterial);
			MotionVectorRenderPass.DrawObjectMotionVectors(cmd, passData.xr, ref rendererList);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer);
			using (new ProfilingScope(rasterCommandBuffer, base.profilingSampler))
			{
				this.InitPassData(ref this.m_PassData, cameraData);
				this.InitRendererLists(ref this.m_PassData, ref universalRenderingData.cullResults, universalRenderingData.supportsDynamicBatching, context, null, false);
				MotionVectorRenderPass.ExecutePass(rasterCommandBuffer, this.m_PassData, this.m_PassData.rendererList);
			}
		}

		private static DrawingSettings GetDrawingSettings(Camera camera, bool supportsDynamicBatching)
		{
			SortingSettings sortingSettings = new SortingSettings(camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings result = new DrawingSettings(ShaderTagId.none, sortingSettings)
			{
				perObjectData = PerObjectData.MotionVectors,
				enableDynamicBatching = supportsDynamicBatching,
				enableInstancing = true,
				lodCrossFadeStencilMask = 0
			};
			for (int i = 0; i < MotionVectorRenderPass.s_ShaderTags.Length; i++)
			{
				result.SetShaderPassName(i, new ShaderTagId(MotionVectorRenderPass.s_ShaderTags[i]));
			}
			return result;
		}

		private static void DrawCameraMotionVectors(RasterCommandBuffer cmd, XRPass xr, Material cameraMaterial)
		{
			bool supportsFoveatedRendering = xr.supportsFoveatedRendering;
			bool flag = supportsFoveatedRendering && XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster);
			if (supportsFoveatedRendering)
			{
				if (flag)
				{
					cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
				}
				else
				{
					cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
				}
			}
			cmd.DrawProcedural(Matrix4x4.identity, cameraMaterial, 0, MeshTopology.Triangles, 3, 1);
			if (supportsFoveatedRendering && !flag)
			{
				cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
		}

		private static void DrawObjectMotionVectors(RasterCommandBuffer cmd, XRPass xr, ref RendererList rendererList)
		{
			bool supportsFoveatedRendering = xr.supportsFoveatedRendering;
			if (supportsFoveatedRendering)
			{
				cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
			}
			cmd.DrawRendererList(rendererList);
			if (supportsFoveatedRendering)
			{
				cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
			}
		}

		private void InitPassData(ref MotionVectorRenderPass.PassData passData, UniversalCameraData cameraData)
		{
			passData.camera = cameraData.camera;
			passData.xr = cameraData.xr;
			passData.cameraMaterial = this.m_CameraMaterial;
		}

		private void InitRendererLists(ref MotionVectorRenderPass.PassData passData, ref CullingResults cullResults, bool supportsDynamicBatching, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
		{
			DrawingSettings drawingSettings = MotionVectorRenderPass.GetDrawingSettings(passData.camera, supportsDynamicBatching);
			RenderStateBlock rsb = new RenderStateBlock(RenderStateMask.Nothing);
			if (useRenderGraph)
			{
				RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref cullResults, drawingSettings, this.m_FilteringSettings, rsb, ref passData.rendererListHdl);
				return;
			}
			RenderingUtils.CreateRendererListWithRenderStateBlock(context, ref cullResults, drawingSettings, this.m_FilteringSettings, rsb, ref passData.rendererList);
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle cameraDepthTexture, TextureHandle motionVectorColor, TextureHandle motionVectorDepth)
		{
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			MotionVectorRenderPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<MotionVectorRenderPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\MotionVectorRenderPass.cs", 221))
			{
				rasterRenderGraphBuilder.UseAllGlobalTextures(true);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && universalCameraData.xrUniversal.canFoveateIntermediatePasses);
				}
				passData.motionVectorColor = motionVectorColor;
				rasterRenderGraphBuilder.SetRenderAttachment(motionVectorColor, 0, AccessFlags.Write);
				passData.motionVectorDepth = motionVectorDepth;
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(motionVectorDepth, AccessFlags.Write);
				this.InitPassData(ref passData, universalCameraData);
				passData.cameraDepth = cameraDepthTexture;
				rasterRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				this.InitRendererLists(ref passData, ref universalRenderingData.cullResults, universalRenderingData.supportsDynamicBatching, default(ScriptableRenderContext), renderGraph, true);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererListHdl);
				if (motionVectorColor.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(motionVectorColor, Shader.PropertyToID("_MotionVectorTexture"));
				}
				if (motionVectorDepth.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(motionVectorDepth, Shader.PropertyToID("_MotionVectorDepthTexture"));
				}
				rasterRenderGraphBuilder.SetRenderFunc<MotionVectorRenderPass.PassData>(delegate(MotionVectorRenderPass.PassData data, RasterGraphContext context)
				{
					if (data.cameraMaterial != null)
					{
						data.cameraMaterial.SetTexture(MotionVectorRenderPass.s_CameraDepthTextureID, data.cameraDepth);
					}
					MotionVectorRenderPass.ExecutePass(context.cmd, data, data.rendererListHdl);
				});
			}
		}

		internal static void SetMotionVectorGlobalMatrices(CommandBuffer cmd, UniversalCameraData cameraData)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData;
			if (cameraData.camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
			{
				MotionVectorsPersistentData motionVectorsPersistentData = universalAdditionalCameraData.motionVectorsPersistentData;
				if (motionVectorsPersistentData == null)
				{
					return;
				}
				motionVectorsPersistentData.SetGlobalMotionMatrices(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData.xr);
			}
		}

		internal static void SetRenderGraphMotionVectorGlobalMatrices(RenderGraph renderGraph, UniversalCameraData cameraData)
		{
			UniversalAdditionalCameraData universalAdditionalCameraData;
			if (cameraData.camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
			{
				MotionVectorRenderPass.MotionMatrixPassData motionMatrixPassData;
				using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<MotionVectorRenderPass.MotionMatrixPassData>(MotionVectorRenderPass.s_SetMotionMatrixProfilingSampler.name, out motionMatrixPassData, MotionVectorRenderPass.s_SetMotionMatrixProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\MotionVectorRenderPass.cs", 276))
				{
					motionMatrixPassData.motionData = universalAdditionalCameraData.motionVectorsPersistentData;
					motionMatrixPassData.xr = cameraData.xr;
					rasterRenderGraphBuilder.AllowGlobalStateModification(true);
					rasterRenderGraphBuilder.SetRenderFunc<MotionVectorRenderPass.MotionMatrixPassData>(delegate(MotionVectorRenderPass.MotionMatrixPassData data, RasterGraphContext context)
					{
						data.motionData.SetGlobalMotionMatrices(context.cmd, data.xr);
					});
				}
			}
		}

		internal const string k_MotionVectorTextureName = "_MotionVectorTexture";

		internal const string k_MotionVectorDepthTextureName = "_MotionVectorDepthTexture";

		internal const GraphicsFormat k_TargetFormat = GraphicsFormat.R16G16_SFloat;

		public const string k_MotionVectorsLightModeTag = "MotionVectors";

		private static readonly string[] s_ShaderTags = new string[]
		{
			"MotionVectors"
		};

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private static readonly ProfilingSampler s_SetMotionMatrixProfilingSampler = new ProfilingSampler("Set Motion Vector Global Matrices");

		private RTHandle m_Color;

		private RTHandle m_Depth;

		private readonly Material m_CameraMaterial;

		private readonly FilteringSettings m_FilteringSettings;

		private MotionVectorRenderPass.PassData m_PassData;

		private class PassData
		{
			internal Camera camera;

			internal XRPass xr;

			internal TextureHandle motionVectorColor;

			internal TextureHandle motionVectorDepth;

			internal TextureHandle cameraDepth;

			internal Material cameraMaterial;

			internal RendererListHandle rendererListHdl;

			internal RendererList rendererList;
		}

		public class MotionMatrixPassData
		{
			public MotionVectorsPersistentData motionData;

			public XRPass xr;
		}
	}
}
