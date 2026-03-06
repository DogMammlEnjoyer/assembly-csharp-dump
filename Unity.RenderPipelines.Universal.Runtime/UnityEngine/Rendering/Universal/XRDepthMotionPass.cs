using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	public class XRDepthMotionPass : ScriptableRenderPass
	{
		public XRDepthMotionPass(RenderPassEvent evt, Shader xrMotionVector)
		{
			base.profilingSampler = new ProfilingSampler("XRDepthMotionPass");
			this.m_PassData = new XRDepthMotionPass.PassData();
			base.renderPassEvent = evt;
			this.ResetMotionData();
			this.m_XRMotionVectorMaterial = CoreUtils.CreateEngineMaterial(xrMotionVector);
			this.xrMotionVectorColor = TextureHandle.nullHandle;
			this.m_XRMotionVectorColor = null;
			this.xrMotionVectorDepth = TextureHandle.nullHandle;
			this.m_XRMotionVectorDepth = null;
		}

		private static DrawingSettings GetObjectMotionDrawingSettings(Camera camera)
		{
			SortingSettings sortingSettings = new SortingSettings(camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings result = new DrawingSettings(XRDepthMotionPass.k_MotionOnlyShaderTagId, sortingSettings)
			{
				perObjectData = PerObjectData.MotionVectors,
				enableDynamicBatching = false,
				enableInstancing = true
			};
			result.SetShaderPassName(0, XRDepthMotionPass.k_MotionOnlyShaderTagId);
			return result;
		}

		private void InitObjectMotionRendererLists(ref XRDepthMotionPass.PassData passData, ref CullingResults cullResults, RenderGraph renderGraph, Camera camera)
		{
			DrawingSettings objectMotionDrawingSettings = XRDepthMotionPass.GetObjectMotionDrawingSettings(camera);
			FilteringSettings fs = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), camera.cullingMask, uint.MaxValue, 0);
			fs.forceAllMotionVectorObjects = true;
			RenderStateBlock rsb = new RenderStateBlock(RenderStateMask.Nothing);
			RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref cullResults, objectMotionDrawingSettings, fs, rsb, ref passData.objMotionRendererList);
		}

		private void InitPassData(ref XRDepthMotionPass.PassData passData, UniversalCameraData cameraData)
		{
			passData.previousViewProjectionStereo = this.m_PreviousViewProjection;
			passData.viewProjectionStereo = this.m_ViewProjection;
			passData.xrMotionVector = this.m_XRMotionVectorMaterial;
		}

		private void ImportXRMotionColorAndDepth(RenderGraph renderGraph, UniversalCameraData cameraData)
		{
			RenderTargetIdentifier motionVectorRenderTarget = cameraData.xr.motionVectorRenderTarget;
			if (this.m_XRMotionVectorColor == null)
			{
				this.m_XRMotionVectorColor = RTHandles.Alloc(motionVectorRenderTarget);
			}
			else if (this.m_XRMotionVectorColor.nameID != motionVectorRenderTarget)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_XRMotionVectorColor, motionVectorRenderTarget);
			}
			RenderTargetIdentifier motionVectorRenderTarget2 = cameraData.xr.motionVectorRenderTarget;
			if (this.m_XRMotionVectorDepth == null)
			{
				this.m_XRMotionVectorDepth = RTHandles.Alloc(motionVectorRenderTarget2);
			}
			else if (this.m_XRMotionVectorDepth.nameID != motionVectorRenderTarget2)
			{
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref this.m_XRMotionVectorDepth, motionVectorRenderTarget2);
			}
			RenderTargetInfo renderTargetInfo = default(RenderTargetInfo);
			renderTargetInfo.width = cameraData.xr.motionVectorRenderTargetDesc.width;
			renderTargetInfo.height = cameraData.xr.motionVectorRenderTargetDesc.height;
			renderTargetInfo.volumeDepth = cameraData.xr.motionVectorRenderTargetDesc.volumeDepth;
			renderTargetInfo.msaaSamples = cameraData.xr.motionVectorRenderTargetDesc.msaaSamples;
			renderTargetInfo.format = cameraData.xr.motionVectorRenderTargetDesc.graphicsFormat;
			RenderTargetInfo info = default(RenderTargetInfo);
			info = renderTargetInfo;
			info.format = cameraData.xr.motionVectorRenderTargetDesc.depthStencilFormat;
			ImportResourceParams importParams = default(ImportResourceParams);
			importParams.clearOnFirstUse = true;
			importParams.clearColor = Color.black;
			importParams.discardOnLastUse = false;
			ImportResourceParams importParams2 = default(ImportResourceParams);
			importParams2.clearOnFirstUse = true;
			importParams2.clearColor = Color.black;
			importParams2.discardOnLastUse = false;
			this.xrMotionVectorColor = renderGraph.ImportTexture(this.m_XRMotionVectorColor, renderTargetInfo, importParams);
			this.xrMotionVectorDepth = renderGraph.ImportTexture(this.m_XRMotionVectorDepth, info, importParams2);
			this.m_XRSpaceWarpRightHandedNDC = cameraData.xr.spaceWarpRightHandedNDC;
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			if (!universalCameraData.xr.enabled || !universalCameraData.xr.singlePassEnabled)
			{
				Debug.LogWarning("XRDepthMotionPass::Render is skipped because either XR is not enabled or singlepass rendering is not enabled.");
				return;
			}
			if (!universalCameraData.xr.hasMotionVectorPass)
			{
				Debug.LogWarning("XRDepthMotionPass::Render is skipped because XR motion vector is not enabled for the current XRPass.");
				return;
			}
			this.ImportXRMotionColorAndDepth(renderGraph, universalCameraData);
			universalCameraData.camera.depthTextureMode |= (DepthTextureMode.Depth | DepthTextureMode.MotionVectors);
			XRDepthMotionPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<XRDepthMotionPass.PassData>("XR Motion Pass", out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\XRDepthMotionPass.cs", 194))
			{
				rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering);
				rasterRenderGraphBuilder.SetRenderAttachment(this.xrMotionVectorColor, 0, AccessFlags.Write);
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(this.xrMotionVectorDepth, AccessFlags.Write);
				this.InitObjectMotionRendererLists(ref passData, ref universalRenderingData.cullResults, renderGraph, universalCameraData.camera);
				rasterRenderGraphBuilder.UseRendererList(passData.objMotionRendererList);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				this.InitPassData(ref passData, universalCameraData);
				rasterRenderGraphBuilder.SetRenderFunc<XRDepthMotionPass.PassData>(delegate(XRDepthMotionPass.PassData data, RasterGraphContext context)
				{
					context.cmd.SetGlobalMatrixArray(ShaderPropertyId.previousViewProjectionNoJitterStereo, data.previousViewProjectionStereo);
					context.cmd.SetGlobalMatrixArray(ShaderPropertyId.viewProjectionNoJitterStereo, data.viewProjectionStereo);
					context.cmd.SetGlobalFloat(XRDepthMotionPass.k_SpaceWarpNDCModifier, this.m_XRSpaceWarpRightHandedNDC ? -1f : 1f);
					context.cmd.DrawRendererList(passData.objMotionRendererList);
					context.cmd.DrawProcedural(Matrix4x4.identity, data.xrMotionVector, 0, MeshTopology.Triangles, 3, 1);
				});
			}
		}

		private void ResetMotionData()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_ViewProjection[i] = Matrix4x4.identity;
				this.m_PreviousViewProjection[i] = Matrix4x4.identity;
			}
			this.m_LastFrameIndex = -1;
		}

		public void Update(ref UniversalCameraData cameraData)
		{
			if (!cameraData.xr.enabled || !cameraData.xr.singlePassEnabled)
			{
				Debug.LogWarning("XRDepthMotionPass::Update is skipped because either XR is not enabled or singlepass rendering is not enabled.");
				return;
			}
			if (this.m_LastFrameIndex != Time.frameCount)
			{
				Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(0), false) * cameraData.GetViewMatrix(0);
				Matrix4x4 matrix4x2 = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(1), false) * cameraData.GetViewMatrix(1);
				this.m_PreviousViewProjection[0] = this.m_ViewProjection[0];
				this.m_PreviousViewProjection[1] = this.m_ViewProjection[1];
				this.m_ViewProjection[0] = matrix4x;
				this.m_ViewProjection[1] = matrix4x2;
				this.m_LastFrameIndex = Time.frameCount;
			}
		}

		public void Dispose()
		{
			RTHandle xrmotionVectorColor = this.m_XRMotionVectorColor;
			if (xrmotionVectorColor != null)
			{
				xrmotionVectorColor.Release();
			}
			RTHandle xrmotionVectorDepth = this.m_XRMotionVectorDepth;
			if (xrmotionVectorDepth != null)
			{
				xrmotionVectorDepth.Release();
			}
			CoreUtils.Destroy(this.m_XRMotionVectorMaterial);
		}

		public const string k_MotionOnlyShaderTagIdName = "XRMotionVectors";

		private static readonly ShaderTagId k_MotionOnlyShaderTagId = new ShaderTagId("XRMotionVectors");

		private static readonly int k_SpaceWarpNDCModifier = Shader.PropertyToID("_SpaceWarpNDCModifier");

		private XRDepthMotionPass.PassData m_PassData;

		private RTHandle m_XRMotionVectorColor;

		private TextureHandle xrMotionVectorColor;

		private RTHandle m_XRMotionVectorDepth;

		private TextureHandle xrMotionVectorDepth;

		private bool m_XRSpaceWarpRightHandedNDC;

		private const int k_XRViewCount = 2;

		private Matrix4x4[] m_ViewProjection = new Matrix4x4[2];

		private Matrix4x4[] m_PreviousViewProjection = new Matrix4x4[2];

		private int m_LastFrameIndex;

		private Material m_XRMotionVectorMaterial;

		private class PassData
		{
			internal RendererListHandle objMotionRendererList;

			internal Matrix4x4[] previousViewProjectionStereo = new Matrix4x4[2];

			internal Matrix4x4[] viewProjectionStereo = new Matrix4x4[2];

			internal Material xrMotionVector;
		}
	}
}
