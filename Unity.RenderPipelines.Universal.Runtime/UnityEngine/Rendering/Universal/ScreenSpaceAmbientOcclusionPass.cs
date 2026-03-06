using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class ScreenSpaceAmbientOcclusionPass : ScriptableRenderPass
	{
		internal ScreenSpaceAmbientOcclusionPass()
		{
			this.m_CurrentSettings = new ScreenSpaceAmbientOcclusionSettings();
			this.m_PassData = new ScreenSpaceAmbientOcclusionPass.SSAOPassData();
		}

		internal bool Setup(ref ScreenSpaceAmbientOcclusionSettings featureSettings, ref ScriptableRenderer renderer, ref Material material, ref Texture2D[] blueNoiseTextures)
		{
			this.m_BlueNoiseTextures = blueNoiseTextures;
			this.m_Material = material;
			this.m_Renderer = renderer;
			this.m_CurrentSettings = featureSettings;
			UniversalRenderer universalRenderer = renderer as UniversalRenderer;
			if (universalRenderer != null && universalRenderer.usesDeferredLighting)
			{
				base.renderPassEvent = (this.m_CurrentSettings.AfterOpaque ? RenderPassEvent.AfterRenderingOpaques : RenderPassEvent.AfterRenderingGbuffer);
				this.m_CurrentSettings.Source = ScreenSpaceAmbientOcclusionSettings.DepthSource.DepthNormals;
			}
			else
			{
				base.renderPassEvent = (this.m_CurrentSettings.AfterOpaque ? RenderPassEvent.BeforeRenderingTransparents : ((RenderPassEvent)201));
			}
			ScreenSpaceAmbientOcclusionSettings.DepthSource source = this.m_CurrentSettings.Source;
			if (source != ScreenSpaceAmbientOcclusionSettings.DepthSource.Depth)
			{
				if (source != ScreenSpaceAmbientOcclusionSettings.DepthSource.DepthNormals)
				{
					throw new ArgumentOutOfRangeException();
				}
				base.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
			}
			else
			{
				base.ConfigureInput(ScriptableRenderPassInput.Depth);
			}
			switch (this.m_CurrentSettings.BlurQuality)
			{
			case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.High:
				this.m_BlurType = ScreenSpaceAmbientOcclusionPass.BlurTypes.Bilateral;
				break;
			case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Medium:
				this.m_BlurType = ScreenSpaceAmbientOcclusionPass.BlurTypes.Gaussian;
				break;
			case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Low:
				this.m_BlurType = ScreenSpaceAmbientOcclusionPass.BlurTypes.Kawase;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return this.m_Material != null && this.m_CurrentSettings.Intensity > 0f && this.m_CurrentSettings.Radius > 0f && this.m_CurrentSettings.Falloff > 0f;
		}

		private static bool IsAfterOpaquePass(ref ScreenSpaceAmbientOcclusionPass.ShaderPasses pass)
		{
			return pass == ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralAfterOpaque || pass == ScreenSpaceAmbientOcclusionPass.ShaderPasses.GaussianAfterOpaque || pass == ScreenSpaceAmbientOcclusionPass.ShaderPasses.KawaseAfterOpaque;
		}

		private void SetupKeywordsAndParameters(ref ScreenSpaceAmbientOcclusionSettings settings, ref UniversalCameraData cameraData)
		{
			int num = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled) ? 2 : 1;
			for (int i = 0; i < num; i++)
			{
				Matrix4x4 viewMatrix = cameraData.GetViewMatrix(i);
				Matrix4x4 projectionMatrix = cameraData.GetProjectionMatrix(i);
				this.m_CameraViewProjections[i] = projectionMatrix * viewMatrix;
				Matrix4x4 rhs = viewMatrix;
				rhs.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
				Matrix4x4 inverse = (projectionMatrix * rhs).inverse;
				Vector4 vector = inverse.MultiplyPoint(new Vector4(-1f, 1f, -1f, 1f));
				Vector4 a = inverse.MultiplyPoint(new Vector4(1f, 1f, -1f, 1f));
				Vector4 a2 = inverse.MultiplyPoint(new Vector4(-1f, -1f, -1f, 1f));
				Vector4 vector2 = inverse.MultiplyPoint(new Vector4(0f, 0f, 1f, 1f));
				this.m_CameraTopLeftCorner[i] = vector;
				this.m_CameraXExtent[i] = a - vector;
				this.m_CameraYExtent[i] = a2 - vector;
				this.m_CameraZExtent[i] = vector2;
			}
			this.m_Material.SetVector(ScreenSpaceAmbientOcclusionPass.s_ProjectionParams2ID, new Vector4(1f / cameraData.camera.nearClipPlane, 0f, 0f, 0f));
			this.m_Material.SetMatrixArray(ScreenSpaceAmbientOcclusionPass.s_CameraViewProjectionsID, this.m_CameraViewProjections);
			this.m_Material.SetVectorArray(ScreenSpaceAmbientOcclusionPass.s_CameraViewTopLeftCornerID, this.m_CameraTopLeftCorner);
			this.m_Material.SetVectorArray(ScreenSpaceAmbientOcclusionPass.s_CameraViewXExtentID, this.m_CameraXExtent);
			this.m_Material.SetVectorArray(ScreenSpaceAmbientOcclusionPass.s_CameraViewYExtentID, this.m_CameraYExtent);
			this.m_Material.SetVectorArray(ScreenSpaceAmbientOcclusionPass.s_CameraViewZExtentID, this.m_CameraZExtent);
			if (settings.AOMethod == ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.BlueNoise)
			{
				this.m_BlueNoiseTextureIndex = (this.m_BlueNoiseTextureIndex + 1) % this.m_BlueNoiseTextures.Length;
				Texture2D value = this.m_BlueNoiseTextures[this.m_BlueNoiseTextureIndex];
				Vector4 value2 = new Vector4((float)cameraData.pixelWidth / (float)this.m_BlueNoiseTextures[this.m_BlueNoiseTextureIndex].width, (float)cameraData.pixelHeight / (float)this.m_BlueNoiseTextures[this.m_BlueNoiseTextureIndex].height, Random.value, Random.value);
				this.m_Material.SetTexture(ScreenSpaceAmbientOcclusionPass.s_BlueNoiseTextureID, value);
				this.m_Material.SetVector(ScreenSpaceAmbientOcclusionPass.s_SSAOBlueNoiseParamsID, value2);
			}
			ScreenSpaceAmbientOcclusionPass.SSAOMaterialParams ssaomaterialParams = new ScreenSpaceAmbientOcclusionPass.SSAOMaterialParams(ref settings, cameraData.camera.orthographic);
			int num2 = (!this.m_SSAOParamsPrev.Equals(ref ssaomaterialParams)) ? 1 : 0;
			bool flag = this.m_Material.HasProperty(ScreenSpaceAmbientOcclusionPass.s_SSAOParamsID);
			if (num2 == 0 && flag)
			{
				return;
			}
			this.m_SSAOParamsPrev = ssaomaterialParams;
			CoreUtils.SetKeyword(this.m_Material, "_ORTHOGRAPHIC", ssaomaterialParams.orthographicCamera);
			CoreUtils.SetKeyword(this.m_Material, "_BLUE_NOISE", ssaomaterialParams.aoBlueNoise);
			CoreUtils.SetKeyword(this.m_Material, "_INTERLEAVED_GRADIENT", ssaomaterialParams.aoInterleavedGradient);
			CoreUtils.SetKeyword(this.m_Material, "_SAMPLE_COUNT_HIGH", ssaomaterialParams.sampleCountHigh);
			CoreUtils.SetKeyword(this.m_Material, "_SAMPLE_COUNT_MEDIUM", ssaomaterialParams.sampleCountMedium);
			CoreUtils.SetKeyword(this.m_Material, "_SAMPLE_COUNT_LOW", ssaomaterialParams.sampleCountLow);
			CoreUtils.SetKeyword(this.m_Material, "_SOURCE_DEPTH_NORMALS", ssaomaterialParams.sourceDepthNormals);
			CoreUtils.SetKeyword(this.m_Material, "_SOURCE_DEPTH_HIGH", ssaomaterialParams.sourceDepthHigh);
			CoreUtils.SetKeyword(this.m_Material, "_SOURCE_DEPTH_MEDIUM", ssaomaterialParams.sourceDepthMedium);
			CoreUtils.SetKeyword(this.m_Material, "_SOURCE_DEPTH_LOW", ssaomaterialParams.sourceDepthLow);
			this.m_Material.SetVector(ScreenSpaceAmbientOcclusionPass.s_SSAOParamsID, ssaomaterialParams.ssaoParams);
		}

		private void InitSSAOPassData(ref ScreenSpaceAmbientOcclusionPass.SSAOPassData data)
		{
			data.material = this.m_Material;
			data.BlurQuality = this.m_CurrentSettings.BlurQuality;
			data.afterOpaque = this.m_CurrentSettings.AfterOpaque;
			data.directLightingStrength = this.m_CurrentSettings.DirectLightingStrength;
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			TextureHandle aotexture;
			TextureHandle blurTexture;
			TextureHandle finalTexture;
			this.CreateRenderTextureHandles(renderGraph, universalResourceData, cameraData, out aotexture, out blurTexture, out finalTexture);
			TextureHandle cameraDepthTexture = universalResourceData.cameraDepthTexture;
			TextureHandle cameraNormalsTexture = universalResourceData.cameraNormalsTexture;
			this.SetupKeywordsAndParameters(ref this.m_CurrentSettings, ref cameraData);
			ScreenSpaceAmbientOcclusionPass.SSAOPassData ssaopassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ScreenSpaceAmbientOcclusionPass.SSAOPassData>("Blit SSAO", out ssaopassData, this.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\ScreenSpaceAmbientOcclusionPass.cs", 335))
			{
				unsafeRenderGraphBuilder.AllowGlobalStateModification(true);
				this.InitSSAOPassData(ref ssaopassData);
				ssaopassData.cameraColor = universalResourceData.cameraColor;
				ssaopassData.AOTexture = aotexture;
				ssaopassData.finalTexture = finalTexture;
				ssaopassData.blurTexture = blurTexture;
				unsafeRenderGraphBuilder.UseTexture(ssaopassData.AOTexture, AccessFlags.ReadWrite);
				TextureHandle cameraColor = universalResourceData.cameraColor;
				if (cameraColor.IsValid())
				{
					IBaseRenderGraphBuilder baseRenderGraphBuilder = unsafeRenderGraphBuilder;
					cameraColor = universalResourceData.cameraColor;
					baseRenderGraphBuilder.UseTexture(cameraColor, AccessFlags.Read);
				}
				if (ssaopassData.BlurQuality != ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Low)
				{
					unsafeRenderGraphBuilder.UseTexture(ssaopassData.blurTexture, AccessFlags.ReadWrite);
				}
				if (cameraDepthTexture.IsValid())
				{
					unsafeRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				}
				if (this.m_CurrentSettings.Source == ScreenSpaceAmbientOcclusionSettings.DepthSource.DepthNormals && cameraNormalsTexture.IsValid())
				{
					unsafeRenderGraphBuilder.UseTexture(cameraNormalsTexture, AccessFlags.Read);
					ssaopassData.cameraNormalsTexture = cameraNormalsTexture;
				}
				if (!ssaopassData.afterOpaque && finalTexture.IsValid())
				{
					unsafeRenderGraphBuilder.UseTexture(ssaopassData.finalTexture, AccessFlags.ReadWrite);
					unsafeRenderGraphBuilder.SetGlobalTextureAfterPass(finalTexture, ScreenSpaceAmbientOcclusionPass.s_SSAOFinalTextureID);
				}
				unsafeRenderGraphBuilder.SetRenderFunc<ScreenSpaceAmbientOcclusionPass.SSAOPassData>(delegate(ScreenSpaceAmbientOcclusionPass.SSAOPassData data, UnsafeGraphContext rgContext)
				{
					CommandBuffer nativeCommandBuffer = CommandBufferHelpers.GetNativeCommandBuffer(rgContext.cmd);
					RenderBufferLoadAction loadAction = data.afterOpaque ? RenderBufferLoadAction.Load : RenderBufferLoadAction.DontCare;
					if (data.cameraColor.IsValid())
					{
						PostProcessUtils.SetSourceSize(nativeCommandBuffer, data.cameraColor);
					}
					if (data.cameraNormalsTexture.IsValid())
					{
						data.material.SetTexture(ScreenSpaceAmbientOcclusionPass.s_CameraNormalsTextureID, data.cameraNormalsTexture);
					}
					Blitter.BlitCameraTexture(nativeCommandBuffer, data.AOTexture, data.AOTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, data.material, 0);
					switch (data.BlurQuality)
					{
					case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.High:
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.AOTexture, data.blurTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, data.material, 1);
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.blurTexture, data.AOTexture, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, data.material, 2);
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.AOTexture, data.finalTexture, loadAction, RenderBufferStoreAction.Store, data.material, data.afterOpaque ? 4 : 3);
						break;
					case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Medium:
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.AOTexture, data.blurTexture, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, data.material, 5);
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.blurTexture, data.finalTexture, loadAction, RenderBufferStoreAction.Store, data.material, data.afterOpaque ? 7 : 6);
						break;
					case ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Low:
						Blitter.BlitCameraTexture(nativeCommandBuffer, data.AOTexture, data.finalTexture, loadAction, RenderBufferStoreAction.Store, data.material, data.afterOpaque ? 9 : 8);
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!data.afterOpaque)
					{
						rgContext.cmd.SetKeyword(ShaderGlobalKeywords.ScreenSpaceOcclusion, true);
						rgContext.cmd.SetGlobalVector(ScreenSpaceAmbientOcclusionPass.s_AmbientOcclusionParamID, new Vector4(1f, 0f, 0f, data.directLightingStrength));
					}
				});
			}
		}

		private void CreateRenderTextureHandles(RenderGraph renderGraph, UniversalResourceData resourceData, UniversalCameraData cameraData, out TextureHandle aoTexture, out TextureHandle blurTexture, out TextureHandle finalTexture)
		{
			RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
			cameraTargetDescriptor.colorFormat = (this.m_SupportsR8RenderTextureFormat ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32);
			cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
			cameraTargetDescriptor.msaaSamples = 1;
			int num = this.m_CurrentSettings.Downsample ? 2 : 1;
			bool flag = this.m_SupportsR8RenderTextureFormat && this.m_BlurType > ScreenSpaceAmbientOcclusionPass.BlurTypes.Bilateral;
			RenderTextureDescriptor desc = cameraTargetDescriptor;
			desc.colorFormat = (flag ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32);
			desc.width /= num;
			desc.height /= num;
			aoTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_SSAO_OcclusionTexture0", false, FilterMode.Bilinear, TextureWrapMode.Clamp);
			finalTexture = (this.m_CurrentSettings.AfterOpaque ? resourceData.activeColorTexture : UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "_ScreenSpaceOcclusionTexture", false, FilterMode.Bilinear, TextureWrapMode.Clamp));
			if (this.m_CurrentSettings.BlurQuality != ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions.Low)
			{
				blurTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_SSAO_OcclusionTexture1", false, FilterMode.Bilinear, TextureWrapMode.Clamp);
			}
			else
			{
				blurTexture = TextureHandle.nullHandle;
			}
			if (!this.m_CurrentSettings.AfterOpaque)
			{
				resourceData.ssaoTexture = finalTexture;
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			this.InitSSAOPassData(ref this.m_PassData);
			this.SetupKeywordsAndParameters(ref this.m_CurrentSettings, ref universalCameraData);
			int num = this.m_CurrentSettings.Downsample ? 2 : 1;
			RenderTextureDescriptor aopassDescriptor = *renderingData.cameraData.cameraTargetDescriptor;
			aopassDescriptor.msaaSamples = 1;
			aopassDescriptor.depthStencilFormat = GraphicsFormat.None;
			this.m_AOPassDescriptor = aopassDescriptor;
			this.m_AOPassDescriptor.width = this.m_AOPassDescriptor.width / num;
			this.m_AOPassDescriptor.height = this.m_AOPassDescriptor.height / num;
			this.m_AOPassDescriptor.colorFormat = ((this.m_SupportsR8RenderTextureFormat && this.m_BlurType > ScreenSpaceAmbientOcclusionPass.BlurTypes.Bilateral) ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_SSAOTextures[0], this.m_AOPassDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_SSAO_OcclusionTexture0");
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_SSAOTextures[1], this.m_AOPassDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_SSAO_OcclusionTexture1");
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_SSAOTextures[2], this.m_AOPassDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_SSAO_OcclusionTexture2");
			this.m_AOPassDescriptor.width = this.m_AOPassDescriptor.width * num;
			this.m_AOPassDescriptor.height = this.m_AOPassDescriptor.height * num;
			this.m_AOPassDescriptor.colorFormat = (this.m_SupportsR8RenderTextureFormat ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32);
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_SSAOTextures[3], this.m_AOPassDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, 1, 0f, "_SSAO_OcclusionTexture");
			PostProcessUtils.SetSourceSize(cmd, this.m_SSAOTextures[3]);
			base.ConfigureTarget(this.m_CurrentSettings.AfterOpaque ? this.m_Renderer.cameraColorTargetHandle : this.m_SSAOTextures[3]);
			base.ConfigureClear(ClearFlag.None, Color.white);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (this.m_Material == null)
			{
				Debug.LogErrorFormat("{0}.Execute(): Missing material. ScreenSpaceAmbientOcclusion pass will not execute. Check for missing reference in the renderer resources.", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			using (new ProfilingScope(commandBuffer, ProfilingSampler.Get<URPProfileId>(URPProfileId.SSAO)))
			{
				if (!this.m_CurrentSettings.AfterOpaque)
				{
					commandBuffer.SetKeyword(ShaderGlobalKeywords.ScreenSpaceOcclusion, true);
				}
				commandBuffer.SetGlobalTexture("_ScreenSpaceOcclusionTexture", this.m_SSAOTextures[3]);
				bool flag = false;
				if (renderingData.cameraData.xr.supportsFoveatedRendering)
				{
					if (this.m_CurrentSettings.Downsample || SystemInfo.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster) || (SystemInfo.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.FoveationImage) && this.m_CurrentSettings.Source == ScreenSpaceAmbientOcclusionSettings.DepthSource.Depth))
					{
						commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
					}
					else if (SystemInfo.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.FoveationImage))
					{
						commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
						flag = true;
					}
				}
				int[] array;
				ScreenSpaceAmbientOcclusionPass.ShaderPasses[] array2;
				ScreenSpaceAmbientOcclusionPass.GetPassOrder(this.m_BlurType, this.m_CurrentSettings.AfterOpaque, out array, out array2);
				RTHandle cameraDepthTargetHandle = renderingData.cameraData.renderer->cameraDepthTargetHandle;
				ScreenSpaceAmbientOcclusionPass.RenderAndSetBaseMap(ref commandBuffer, ref renderingData, renderingData.cameraData.renderer, ref this.m_Material, ref cameraDepthTargetHandle, ref this.m_SSAOTextures[0], ScreenSpaceAmbientOcclusionPass.ShaderPasses.AmbientOcclusion);
				for (int i = 0; i < array2.Length; i++)
				{
					int num = array[i];
					int num2 = array[i + 1];
					ScreenSpaceAmbientOcclusionPass.RenderAndSetBaseMap(ref commandBuffer, ref renderingData, renderingData.cameraData.renderer, ref this.m_Material, ref this.m_SSAOTextures[num], ref this.m_SSAOTextures[num2], array2[i]);
				}
				commandBuffer.SetGlobalVector(ScreenSpaceAmbientOcclusionPass.s_AmbientOcclusionParamID, new Vector4(1f, 0f, 0f, this.m_CurrentSettings.DirectLightingStrength));
				if (flag)
				{
					commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
				}
			}
		}

		private static void RenderAndSetBaseMap(ref CommandBuffer cmd, ref RenderingData renderingData, ref ScriptableRenderer renderer, ref Material mat, ref RTHandle baseMap, ref RTHandle target, ScreenSpaceAmbientOcclusionPass.ShaderPasses pass)
		{
			if (ScreenSpaceAmbientOcclusionPass.IsAfterOpaquePass(ref pass))
			{
				Blitter.BlitCameraTexture(cmd, baseMap, renderer.cameraColorTargetHandle, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, mat, (int)pass);
				return;
			}
			if (baseMap.rt == null)
			{
				Vector2 v = baseMap.useScaling ? new Vector2(baseMap.rtHandleProperties.rtHandleScale.x, baseMap.rtHandleProperties.rtHandleScale.y) : Vector2.one;
				CoreUtils.SetRenderTarget(cmd, target, ClearFlag.None, 0, CubemapFace.Unknown, -1);
				Blitter.BlitTexture(cmd, baseMap.nameID, v, mat, (int)pass);
				return;
			}
			Blitter.BlitCameraTexture(cmd, baseMap, target, mat, (int)pass);
		}

		private static void GetPassOrder(ScreenSpaceAmbientOcclusionPass.BlurTypes blurType, bool isAfterOpaque, out int[] textureIndices, out ScreenSpaceAmbientOcclusionPass.ShaderPasses[] shaderPasses)
		{
			switch (blurType)
			{
			case ScreenSpaceAmbientOcclusionPass.BlurTypes.Bilateral:
				textureIndices = ScreenSpaceAmbientOcclusionPass.m_BilateralTexturesIndices;
				shaderPasses = (isAfterOpaque ? ScreenSpaceAmbientOcclusionPass.m_BilateralAfterOpaquePasses : ScreenSpaceAmbientOcclusionPass.m_BilateralPasses);
				return;
			case ScreenSpaceAmbientOcclusionPass.BlurTypes.Gaussian:
				textureIndices = ScreenSpaceAmbientOcclusionPass.m_GaussianTexturesIndices;
				shaderPasses = (isAfterOpaque ? ScreenSpaceAmbientOcclusionPass.m_GaussianAfterOpaquePasses : ScreenSpaceAmbientOcclusionPass.m_GaussianPasses);
				return;
			case ScreenSpaceAmbientOcclusionPass.BlurTypes.Kawase:
				textureIndices = ScreenSpaceAmbientOcclusionPass.m_KawaseTexturesIndices;
				shaderPasses = (isAfterOpaque ? ScreenSpaceAmbientOcclusionPass.m_KawaseAfterOpaquePasses : ScreenSpaceAmbientOcclusionPass.m_KawasePasses);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			if (!this.m_CurrentSettings.AfterOpaque)
			{
				cmd.SetKeyword(ShaderGlobalKeywords.ScreenSpaceOcclusion, false);
			}
		}

		public void Dispose()
		{
			RTHandle rthandle = this.m_SSAOTextures[0];
			if (rthandle != null)
			{
				rthandle.Release();
			}
			RTHandle rthandle2 = this.m_SSAOTextures[1];
			if (rthandle2 != null)
			{
				rthandle2.Release();
			}
			RTHandle rthandle3 = this.m_SSAOTextures[2];
			if (rthandle3 != null)
			{
				rthandle3.Release();
			}
			RTHandle rthandle4 = this.m_SSAOTextures[3];
			if (rthandle4 != null)
			{
				rthandle4.Release();
			}
			this.m_SSAOParamsPrev = default(ScreenSpaceAmbientOcclusionPass.SSAOMaterialParams);
		}

		private readonly bool m_SupportsR8RenderTextureFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8);

		private int m_BlueNoiseTextureIndex;

		private Material m_Material;

		private ScreenSpaceAmbientOcclusionPass.SSAOPassData m_PassData;

		private Texture2D[] m_BlueNoiseTextures;

		private Vector4[] m_CameraTopLeftCorner = new Vector4[2];

		private Vector4[] m_CameraXExtent = new Vector4[2];

		private Vector4[] m_CameraYExtent = new Vector4[2];

		private Vector4[] m_CameraZExtent = new Vector4[2];

		private RTHandle[] m_SSAOTextures = new RTHandle[4];

		private ScreenSpaceAmbientOcclusionPass.BlurTypes m_BlurType;

		private Matrix4x4[] m_CameraViewProjections = new Matrix4x4[2];

		private ProfilingSampler m_ProfilingSampler = ProfilingSampler.Get<URPProfileId>(URPProfileId.SSAO);

		private ScriptableRenderer m_Renderer;

		private RenderTextureDescriptor m_AOPassDescriptor;

		private ScreenSpaceAmbientOcclusionSettings m_CurrentSettings;

		private const string k_SSAOTextureName = "_ScreenSpaceOcclusionTexture";

		private const string k_AmbientOcclusionParamName = "_AmbientOcclusionParam";

		internal static readonly int s_AmbientOcclusionParamID = Shader.PropertyToID("_AmbientOcclusionParam");

		private static readonly int s_SSAOParamsID = Shader.PropertyToID("_SSAOParams");

		private static readonly int s_SSAOBlueNoiseParamsID = Shader.PropertyToID("_SSAOBlueNoiseParams");

		private static readonly int s_BlueNoiseTextureID = Shader.PropertyToID("_BlueNoiseTexture");

		private static readonly int s_SSAOFinalTextureID = Shader.PropertyToID("_ScreenSpaceOcclusionTexture");

		private static readonly int s_CameraViewXExtentID = Shader.PropertyToID("_CameraViewXExtent");

		private static readonly int s_CameraViewYExtentID = Shader.PropertyToID("_CameraViewYExtent");

		private static readonly int s_CameraViewZExtentID = Shader.PropertyToID("_CameraViewZExtent");

		private static readonly int s_ProjectionParams2ID = Shader.PropertyToID("_ProjectionParams2");

		private static readonly int s_CameraViewProjectionsID = Shader.PropertyToID("_CameraViewProjections");

		private static readonly int s_CameraViewTopLeftCornerID = Shader.PropertyToID("_CameraViewTopLeftCorner");

		private static readonly int s_CameraDepthTextureID = Shader.PropertyToID("_CameraDepthTexture");

		private static readonly int s_CameraNormalsTextureID = Shader.PropertyToID("_CameraNormalsTexture");

		private static readonly int[] m_BilateralTexturesIndices = new int[]
		{
			0,
			1,
			2,
			3
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_BilateralPasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralBlurHorizontal,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralBlurVertical,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralBlurFinal
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_BilateralAfterOpaquePasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralBlurHorizontal,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralBlurVertical,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.BilateralAfterOpaque
		};

		private static readonly int[] m_GaussianTexturesIndices = new int[]
		{
			0,
			1,
			3,
			3
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_GaussianPasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.GaussianBlurHorizontal,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.GaussianBlurVertical
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_GaussianAfterOpaquePasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.GaussianBlurHorizontal,
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.GaussianAfterOpaque
		};

		private static readonly int[] m_KawaseTexturesIndices = new int[]
		{
			0,
			3
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_KawasePasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.KawaseBlur
		};

		private static readonly ScreenSpaceAmbientOcclusionPass.ShaderPasses[] m_KawaseAfterOpaquePasses = new ScreenSpaceAmbientOcclusionPass.ShaderPasses[]
		{
			ScreenSpaceAmbientOcclusionPass.ShaderPasses.KawaseAfterOpaque
		};

		private ScreenSpaceAmbientOcclusionPass.SSAOMaterialParams m_SSAOParamsPrev;

		private enum BlurTypes
		{
			Bilateral,
			Gaussian,
			Kawase
		}

		private enum ShaderPasses
		{
			AmbientOcclusion,
			BilateralBlurHorizontal,
			BilateralBlurVertical,
			BilateralBlurFinal,
			BilateralAfterOpaque,
			GaussianBlurHorizontal,
			GaussianBlurVertical,
			GaussianAfterOpaque,
			KawaseBlur,
			KawaseAfterOpaque
		}

		private struct SSAOMaterialParams
		{
			internal SSAOMaterialParams(ref ScreenSpaceAmbientOcclusionSettings settings, bool isOrthographic)
			{
				bool flag = settings.Source == ScreenSpaceAmbientOcclusionSettings.DepthSource.DepthNormals;
				float num = (settings.AOMethod == ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.BlueNoise) ? 1.5f : 1f;
				this.orthographicCamera = isOrthographic;
				this.aoBlueNoise = (settings.AOMethod == ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.BlueNoise);
				this.aoInterleavedGradient = (settings.AOMethod == ScreenSpaceAmbientOcclusionSettings.AOMethodOptions.InterleavedGradient);
				this.sampleCountHigh = (settings.Samples == ScreenSpaceAmbientOcclusionSettings.AOSampleOption.High);
				this.sampleCountMedium = (settings.Samples == ScreenSpaceAmbientOcclusionSettings.AOSampleOption.Medium);
				this.sampleCountLow = (settings.Samples == ScreenSpaceAmbientOcclusionSettings.AOSampleOption.Low);
				this.sourceDepthNormals = (settings.Source == ScreenSpaceAmbientOcclusionSettings.DepthSource.DepthNormals);
				this.sourceDepthHigh = (!flag && settings.NormalSamples == ScreenSpaceAmbientOcclusionSettings.NormalQuality.High);
				this.sourceDepthMedium = (!flag && settings.NormalSamples == ScreenSpaceAmbientOcclusionSettings.NormalQuality.Medium);
				this.sourceDepthLow = (!flag && settings.NormalSamples == ScreenSpaceAmbientOcclusionSettings.NormalQuality.Low);
				this.ssaoParams = new Vector4(settings.Intensity, settings.Radius * num, 1f / (float)(settings.Downsample ? 2 : 1), settings.Falloff);
			}

			internal bool Equals(ref ScreenSpaceAmbientOcclusionPass.SSAOMaterialParams other)
			{
				return this.orthographicCamera == other.orthographicCamera && this.aoBlueNoise == other.aoBlueNoise && this.aoInterleavedGradient == other.aoInterleavedGradient && this.sampleCountHigh == other.sampleCountHigh && this.sampleCountMedium == other.sampleCountMedium && this.sampleCountLow == other.sampleCountLow && this.sourceDepthNormals == other.sourceDepthNormals && this.sourceDepthHigh == other.sourceDepthHigh && this.sourceDepthMedium == other.sourceDepthMedium && this.sourceDepthLow == other.sourceDepthLow && this.ssaoParams == other.ssaoParams;
			}

			internal bool orthographicCamera;

			internal bool aoBlueNoise;

			internal bool aoInterleavedGradient;

			internal bool sampleCountHigh;

			internal bool sampleCountMedium;

			internal bool sampleCountLow;

			internal bool sourceDepthNormals;

			internal bool sourceDepthHigh;

			internal bool sourceDepthMedium;

			internal bool sourceDepthLow;

			internal Vector4 ssaoParams;
		}

		private class SSAOPassData
		{
			internal bool afterOpaque;

			internal ScreenSpaceAmbientOcclusionSettings.BlurQualityOptions BlurQuality;

			internal Material material;

			internal float directLightingStrength;

			internal TextureHandle cameraColor;

			internal TextureHandle AOTexture;

			internal TextureHandle finalTexture;

			internal TextureHandle blurTexture;

			internal TextureHandle cameraNormalsTexture;
		}
	}
}
