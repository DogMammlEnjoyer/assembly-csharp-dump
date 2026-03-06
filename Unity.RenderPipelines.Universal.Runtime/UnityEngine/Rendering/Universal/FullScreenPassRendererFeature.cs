using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[MovedFrom("")]
	public class FullScreenPassRendererFeature : ScriptableRendererFeature, ISerializationCallbackReceiver
	{
		public override void Create()
		{
			this.m_FullScreenPass = new FullScreenPassRendererFeature.FullScreenRenderPass(base.name);
		}

		internal override bool RequireRenderingLayers(bool isDeferred, bool needsGBufferAccurateNormals, out RenderingLayerUtils.Event atEvent, out RenderingLayerUtils.MaskSize maskSize)
		{
			atEvent = RenderingLayerUtils.Event.Opaque;
			maskSize = RenderingLayerUtils.MaskSize.Bits8;
			return false;
		}

		public unsafe override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			if (*renderingData.cameraData.cameraType == CameraType.Preview || *renderingData.cameraData.cameraType == CameraType.Reflection || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
			{
				return;
			}
			if (this.passMaterial == null)
			{
				Debug.LogWarningFormat("The full screen feature \"{0}\" will not execute - no material is assigned. Please make sure a material is assigned for this feature on the renderer asset.", new object[]
				{
					base.name
				});
				return;
			}
			if (this.passIndex < 0 || this.passIndex >= this.passMaterial.passCount)
			{
				Debug.LogWarningFormat("The full screen feature \"{0}\" will not execute - the pass index is out of bounds for the material.", new object[]
				{
					base.name
				});
				return;
			}
			this.m_FullScreenPass.renderPassEvent = (RenderPassEvent)this.injectionPoint;
			this.m_FullScreenPass.ConfigureInput(this.requirements);
			this.m_FullScreenPass.SetupMembers(this.passMaterial, this.passIndex, this.fetchColorBuffer, this.bindDepthStencilAttachment);
			this.m_FullScreenPass.requiresIntermediateTexture = this.fetchColorBuffer;
			renderer.EnqueuePass(this.m_FullScreenPass);
		}

		protected override void Dispose(bool disposing)
		{
			this.m_FullScreenPass.Dispose();
		}

		private void UpgradeIfNeeded()
		{
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (this.m_Version == FullScreenPassRendererFeature.Version.Uninitialised)
			{
				this.m_Version = FullScreenPassRendererFeature.Version.AddFetchColorBufferCheckbox;
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.m_Version == FullScreenPassRendererFeature.Version.Uninitialised)
			{
				this.m_Version = FullScreenPassRendererFeature.Version.Initial;
			}
			this.UpgradeIfNeeded();
		}

		public FullScreenPassRendererFeature.InjectionPoint injectionPoint = FullScreenPassRendererFeature.InjectionPoint.AfterRenderingPostProcessing;

		public bool fetchColorBuffer = true;

		public ScriptableRenderPassInput requirements;

		public Material passMaterial;

		public int passIndex;

		public bool bindDepthStencilAttachment;

		private FullScreenPassRendererFeature.FullScreenRenderPass m_FullScreenPass;

		[SerializeField]
		[HideInInspector]
		private FullScreenPassRendererFeature.Version m_Version = FullScreenPassRendererFeature.Version.Uninitialised;

		public enum InjectionPoint
		{
			BeforeRenderingTransparents = 450,
			BeforeRenderingPostProcessing = 550,
			AfterRenderingPostProcessing = 600
		}

		internal class FullScreenRenderPass : ScriptableRenderPass
		{
			public FullScreenRenderPass(string passName)
			{
				base.profilingSampler = new ProfilingSampler(passName);
			}

			public void SetupMembers(Material material, int passIndex, bool fetchActiveColor, bool bindDepthStencilAttachment)
			{
				this.m_Material = material;
				this.m_PassIndex = passIndex;
				this.m_FetchActiveColor = fetchActiveColor;
				this.m_BindDepthStencilAttachment = bindDepthStencilAttachment;
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
			{
				base.ResetTarget();
				if (this.m_FetchActiveColor)
				{
					this.ReAllocate(*renderingData.cameraData.cameraTargetDescriptor);
				}
			}

			internal void ReAllocate(RenderTextureDescriptor desc)
			{
				desc.msaaSamples = 1;
				desc.depthStencilFormat = GraphicsFormat.None;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_CopiedColor, desc, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, "_FullscreenPassColorCopy");
			}

			public void Dispose()
			{
				RTHandle copiedColor = this.m_CopiedColor;
				if (copiedColor == null)
				{
					return;
				}
				copiedColor.Release();
			}

			private static void ExecuteCopyColorPass(RasterCommandBuffer cmd, RTHandle sourceTexture)
			{
				Blitter.BlitTexture(cmd, sourceTexture, new Vector4(1f, 1f, 0f, 0f), 0f, false);
			}

			private static void ExecuteMainPass(RasterCommandBuffer cmd, RTHandle sourceTexture, Material material, int passIndex)
			{
				FullScreenPassRendererFeature.FullScreenRenderPass.s_SharedPropertyBlock.Clear();
				if (sourceTexture != null)
				{
					FullScreenPassRendererFeature.FullScreenRenderPass.s_SharedPropertyBlock.SetTexture(ShaderPropertyId.blitTexture, sourceTexture);
				}
				FullScreenPassRendererFeature.FullScreenRenderPass.s_SharedPropertyBlock.SetVector(ShaderPropertyId.blitScaleBias, new Vector4(1f, 1f, 0f, 0f));
				cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Triangles, 3, 1, FullScreenPassRendererFeature.FullScreenRenderPass.s_SharedPropertyBlock);
			}

			[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
			public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				ref CameraData ptr = ref renderingData.cameraData;
				CommandBuffer commandBuffer = *renderingData.commandBuffer;
				using (new ProfilingScope(commandBuffer, base.profilingSampler))
				{
					RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer);
					if (this.m_FetchActiveColor)
					{
						CoreUtils.SetRenderTarget(commandBuffer, this.m_CopiedColor, ClearFlag.None, 0, CubemapFace.Unknown, -1);
						FullScreenPassRendererFeature.FullScreenRenderPass.ExecuteCopyColorPass(rasterCommandBuffer, ptr.renderer->cameraColorTargetHandle);
					}
					if (this.m_BindDepthStencilAttachment)
					{
						CoreUtils.SetRenderTarget(commandBuffer, ptr.renderer->cameraColorTargetHandle, ptr.renderer->cameraDepthTargetHandle, 0, CubemapFace.Unknown, -1);
					}
					else
					{
						CoreUtils.SetRenderTarget(commandBuffer, ptr.renderer->cameraColorTargetHandle, ClearFlag.None, 0, CubemapFace.Unknown, -1);
					}
					FullScreenPassRendererFeature.FullScreenRenderPass.ExecuteMainPass(rasterCommandBuffer, this.m_FetchActiveColor ? this.m_CopiedColor : null, this.m_Material, this.m_PassIndex);
				}
			}

			public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
			{
				UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
				UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
				TextureHandle inputTexture;
				TextureHandle textureHandle2;
				if (this.m_FetchActiveColor)
				{
					TextureHandle textureHandle = universalResourceData.cameraColor;
					TextureDesc textureDesc = renderGraph.GetTextureDesc(textureHandle);
					textureDesc.name = "_CameraColorFullScreenPass";
					textureDesc.clearBuffer = false;
					inputTexture = universalResourceData.activeColorTexture;
					textureHandle2 = renderGraph.CreateTexture(textureDesc);
					FullScreenPassRendererFeature.FullScreenRenderPass.CopyPassData copyPassData;
					using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<FullScreenPassRendererFeature.FullScreenRenderPass.CopyPassData>("Copy Color Full Screen", out copyPassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\RendererFeatures\\FullScreenPassRendererFeature.cs", 226))
					{
						copyPassData.inputTexture = inputTexture;
						rasterRenderGraphBuilder.UseTexture(copyPassData.inputTexture, AccessFlags.Read);
						rasterRenderGraphBuilder.SetRenderAttachment(textureHandle2, 0, AccessFlags.Write);
						rasterRenderGraphBuilder.SetRenderFunc<FullScreenPassRendererFeature.FullScreenRenderPass.CopyPassData>(delegate(FullScreenPassRendererFeature.FullScreenRenderPass.CopyPassData data, RasterGraphContext rgContext)
						{
							FullScreenPassRendererFeature.FullScreenRenderPass.ExecuteCopyColorPass(rgContext.cmd, data.inputTexture);
						});
					}
					inputTexture = textureHandle2;
				}
				else
				{
					inputTexture = TextureHandle.nullHandle;
				}
				textureHandle2 = universalResourceData.activeColorTexture;
				FullScreenPassRendererFeature.FullScreenRenderPass.MainPassData mainPassData;
				using (IRasterRenderGraphBuilder rasterRenderGraphBuilder2 = renderGraph.AddRasterRenderPass<FullScreenPassRendererFeature.FullScreenRenderPass.MainPassData>(base.passName, out mainPassData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\RendererFeatures\\FullScreenPassRendererFeature.cs", 250))
				{
					mainPassData.material = this.m_Material;
					mainPassData.passIndex = this.m_PassIndex;
					mainPassData.inputTexture = inputTexture;
					if (mainPassData.inputTexture.IsValid())
					{
						rasterRenderGraphBuilder2.UseTexture(mainPassData.inputTexture, AccessFlags.Read);
					}
					bool flag = (base.input & ScriptableRenderPassInput.Color) > ScriptableRenderPassInput.None;
					bool flag2 = (base.input & ScriptableRenderPassInput.Depth) > ScriptableRenderPassInput.None;
					bool flag3 = (base.input & ScriptableRenderPassInput.Motion) > ScriptableRenderPassInput.None;
					bool flag4 = (base.input & ScriptableRenderPassInput.Normal) > ScriptableRenderPassInput.None;
					if (flag && universalCameraData.renderer.SupportsCameraOpaque())
					{
						IBaseRenderGraphBuilder baseRenderGraphBuilder = rasterRenderGraphBuilder2;
						TextureHandle textureHandle = universalResourceData.cameraOpaqueTexture;
						baseRenderGraphBuilder.UseTexture(textureHandle, AccessFlags.Read);
					}
					if (flag2)
					{
						IBaseRenderGraphBuilder baseRenderGraphBuilder2 = rasterRenderGraphBuilder2;
						TextureHandle textureHandle = universalResourceData.cameraDepthTexture;
						baseRenderGraphBuilder2.UseTexture(textureHandle, AccessFlags.Read);
					}
					if (flag3 && universalCameraData.renderer.SupportsMotionVectors())
					{
						IBaseRenderGraphBuilder baseRenderGraphBuilder3 = rasterRenderGraphBuilder2;
						TextureHandle textureHandle = universalResourceData.motionVectorColor;
						baseRenderGraphBuilder3.UseTexture(textureHandle, AccessFlags.Read);
						IBaseRenderGraphBuilder baseRenderGraphBuilder4 = rasterRenderGraphBuilder2;
						textureHandle = universalResourceData.motionVectorDepth;
						baseRenderGraphBuilder4.UseTexture(textureHandle, AccessFlags.Read);
					}
					if (flag4 && universalCameraData.renderer.SupportsCameraNormals())
					{
						IBaseRenderGraphBuilder baseRenderGraphBuilder5 = rasterRenderGraphBuilder2;
						TextureHandle textureHandle = universalResourceData.cameraNormalsTexture;
						baseRenderGraphBuilder5.UseTexture(textureHandle, AccessFlags.Read);
					}
					rasterRenderGraphBuilder2.SetRenderAttachment(textureHandle2, 0, AccessFlags.Write);
					if (this.m_BindDepthStencilAttachment)
					{
						rasterRenderGraphBuilder2.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Write);
					}
					rasterRenderGraphBuilder2.SetRenderFunc<FullScreenPassRendererFeature.FullScreenRenderPass.MainPassData>(delegate(FullScreenPassRendererFeature.FullScreenRenderPass.MainPassData data, RasterGraphContext rgContext)
					{
						FullScreenPassRendererFeature.FullScreenRenderPass.ExecuteMainPass(rgContext.cmd, data.inputTexture, data.material, data.passIndex);
					});
				}
			}

			private Material m_Material;

			private int m_PassIndex;

			private bool m_FetchActiveColor;

			private bool m_BindDepthStencilAttachment;

			private RTHandle m_CopiedColor;

			private static MaterialPropertyBlock s_SharedPropertyBlock = new MaterialPropertyBlock();

			private class CopyPassData
			{
				internal TextureHandle inputTexture;
			}

			private class MainPassData
			{
				internal Material material;

				internal int passIndex;

				internal TextureHandle inputTexture;
			}
		}

		private enum Version
		{
			Uninitialised = -1,
			Initial,
			AddFetchColorBufferCheckbox,
			Count,
			Latest = 1
		}
	}
}
