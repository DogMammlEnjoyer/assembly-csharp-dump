using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RendererUtils;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal class RenderGraphResourceRegistry
	{
		internal static RenderGraphResourceRegistry current
		{
			get
			{
				return RenderGraphResourceRegistry.m_CurrentRegistry;
			}
			set
			{
				RenderGraphResourceRegistry.m_CurrentRegistry = value;
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckTextureResource(TextureResource texResource)
		{
			if (texResource.graphicsResource == null && !texResource.imported)
			{
				throw new InvalidOperationException("Trying to use a texture (" + texResource.GetName() + ") that was already released or not yet created. Make sure you declare it for reading in your pass or you don't read it before it's been written to at least once.");
			}
		}

		internal RTHandle GetTexture(in TextureHandle handle)
		{
			TextureHandle textureHandle = handle;
			if (!textureHandle.IsValid())
			{
				return null;
			}
			return this.GetTextureResource(handle.handle).graphicsResource;
		}

		internal RTHandle GetTexture(int index)
		{
			return this.GetTextureResource(index).graphicsResource;
		}

		internal bool TextureNeedsFallback(in TextureHandle handle)
		{
			TextureHandle textureHandle = handle;
			return textureHandle.IsValid() && this.GetTextureResource(handle.handle).NeedsFallBack();
		}

		internal RendererList GetRendererList(in RendererListHandle handle)
		{
			RendererListHandle rendererListHandle = handle;
			if (!rendererListHandle.IsValid())
			{
				return RendererList.nullRendererList;
			}
			RendererListHandleType type = handle.type;
			if (type != RendererListHandleType.Renderers)
			{
				if (type != RendererListHandleType.Legacy)
				{
					return RendererList.nullRendererList;
				}
				if (handle >= this.m_RendererListLegacyResources.size)
				{
					return RendererList.nullRendererList;
				}
				if (!this.m_RendererListLegacyResources[handle].isActive)
				{
					return RendererList.nullRendererList;
				}
				return this.m_RendererListLegacyResources[handle].rendererList;
			}
			else
			{
				if (handle >= this.m_RendererListResources.size)
				{
					return RendererList.nullRendererList;
				}
				return this.m_RendererListResources[handle].rendererList;
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckBufferResource(BufferResource bufferResource)
		{
			if (bufferResource.graphicsResource == null)
			{
				throw new InvalidOperationException("Trying to use a graphics buffer (" + bufferResource.GetName() + ") that was already released or not yet created. Make sure you declare it for reading in your pass or you don't read it before it's been written to at least once.");
			}
		}

		internal GraphicsBuffer GetBuffer(in BufferHandle handle)
		{
			BufferHandle bufferHandle = handle;
			if (!bufferHandle.IsValid())
			{
				return null;
			}
			return this.GetBufferResource(handle.handle).graphicsResource;
		}

		internal GraphicsBuffer GetBuffer(int index)
		{
			return this.GetBufferResource(index).graphicsResource;
		}

		internal RayTracingAccelerationStructure GetRayTracingAccelerationStructure(in RayTracingAccelerationStructureHandle handle)
		{
			RayTracingAccelerationStructureHandle rayTracingAccelerationStructureHandle = handle;
			if (!rayTracingAccelerationStructureHandle.IsValid())
			{
				return null;
			}
			return this.GetRayTracingAccelerationStructureResource(handle.handle).graphicsResource;
		}

		internal int GetSharedResourceCount(RenderGraphResourceType type)
		{
			return this.m_RenderGraphResources[(int)type].sharedResourcesCount;
		}

		private RenderGraphResourceRegistry()
		{
		}

		internal RenderGraphResourceRegistry(RenderGraphDebugParams renderGraphDebug, RenderGraphLogger frameInformationLogger)
		{
			this.m_RenderGraphDebug = renderGraphDebug;
			this.m_FrameInformationLogger = frameInformationLogger;
			for (int i = 0; i < 3; i++)
			{
				this.m_RenderGraphResources[i] = new RenderGraphResourceRegistry.RenderGraphResourcesData();
			}
			this.m_RenderGraphResources[0].createResourceCallback = new RenderGraphResourceRegistry.ResourceCreateCallback(this.CreateTextureCallback);
			this.m_RenderGraphResources[0].releaseResourceCallback = new RenderGraphResourceRegistry.ResourceCallback(this.ReleaseTextureCallback);
			this.m_RenderGraphResources[0].pool = new TexturePool();
			this.m_RenderGraphResources[1].pool = new BufferPool();
			this.m_RenderGraphResources[2].pool = null;
		}

		internal void BeginRenderGraph(int executionCount)
		{
			this.m_ExecutionCount = executionCount;
			ResourceHandle.NewFrame(executionCount);
			if (this.m_RenderGraphDebug.enableLogging)
			{
				this.m_ResourceLogger.Initialize("RenderGraph Resources");
			}
		}

		internal void BeginExecute(int currentFrameIndex)
		{
			this.m_CurrentFrameIndex = currentFrameIndex;
			this.ManageSharedRenderGraphResources();
			RenderGraphResourceRegistry.current = this;
		}

		internal void EndExecute()
		{
			RenderGraphResourceRegistry.current = null;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckHandleValidity(in ResourceHandle res)
		{
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckHandleValidity(RenderGraphResourceType type, int index)
		{
			if (RenderGraph.enableValidityChecks)
			{
				DynamicArray<IRenderGraphResource> resourceArray = this.m_RenderGraphResources[(int)type].resourceArray;
				if (index == 0)
				{
					throw new ArgumentException(string.Format("Trying to access resource of type {0} with an null resource index.", type));
				}
				if (index >= resourceArray.size)
				{
					throw new ArgumentException(string.Format("Trying to access resource of type {0} with an invalid resource index {1}", type, index));
				}
			}
		}

		internal unsafe void IncrementWriteCount(in ResourceHandle res)
		{
			this.m_RenderGraphResources[res.iType].resourceArray[res.index]->IncrementWriteCount();
		}

		internal unsafe void IncrementReadCount(in ResourceHandle res)
		{
			this.m_RenderGraphResources[res.iType].resourceArray[res.index]->IncrementReadCount();
		}

		internal unsafe void NewVersion(in ResourceHandle res)
		{
			this.m_RenderGraphResources[res.iType].resourceArray[res.index]->NewVersion();
		}

		internal unsafe ResourceHandle GetLatestVersionHandle(in ResourceHandle res)
		{
			int num = this.m_RenderGraphResources[res.iType].resourceArray[res.index]->version;
			if (this.IsRenderGraphResourceShared(res))
			{
				num -= this.m_ExecutionCount;
			}
			return new ResourceHandle(ref res, num);
		}

		internal unsafe int GetLatestVersionNumber(in ResourceHandle res)
		{
			int num = this.m_RenderGraphResources[res.iType].resourceArray[res.index]->version;
			if (this.IsRenderGraphResourceShared(res))
			{
				num -= this.m_ExecutionCount;
			}
			return num;
		}

		internal ResourceHandle GetZeroVersionedHandle(in ResourceHandle res)
		{
			return new ResourceHandle(ref res, 0);
		}

		internal unsafe ResourceHandle GetNewVersionedHandle(in ResourceHandle res)
		{
			int num = this.m_RenderGraphResources[res.iType].resourceArray[res.index]->NewVersion();
			if (this.IsRenderGraphResourceShared(res))
			{
				num -= this.m_ExecutionCount;
			}
			return new ResourceHandle(ref res, num);
		}

		internal unsafe IRenderGraphResource GetResourceLowLevel(in ResourceHandle res)
		{
			return *this.m_RenderGraphResources[res.iType].resourceArray[res.index];
		}

		internal unsafe string GetRenderGraphResourceName(in ResourceHandle res)
		{
			return this.m_RenderGraphResources[res.iType].resourceArray[res.index]->GetName();
		}

		internal unsafe string GetRenderGraphResourceName(RenderGraphResourceType type, int index)
		{
			return this.m_RenderGraphResources[(int)type].resourceArray[index]->GetName();
		}

		internal unsafe bool IsRenderGraphResourceImported(in ResourceHandle res)
		{
			return this.m_RenderGraphResources[res.iType].resourceArray[res.index]->imported;
		}

		internal unsafe bool IsRenderGraphResourceForceReleased(RenderGraphResourceType type, int index)
		{
			return this.m_RenderGraphResources[(int)type].resourceArray[index]->forceRelease;
		}

		internal bool IsRenderGraphResourceShared(RenderGraphResourceType type, int index)
		{
			return index <= this.m_RenderGraphResources[(int)type].sharedResourcesCount;
		}

		internal bool IsRenderGraphResourceShared(in ResourceHandle res)
		{
			return this.IsRenderGraphResourceShared(res.type, res.index);
		}

		internal unsafe bool IsGraphicsResourceCreated(in ResourceHandle res)
		{
			return this.m_RenderGraphResources[res.iType].resourceArray[res.index]->IsCreated();
		}

		internal bool IsRendererListCreated(in RendererListHandle res)
		{
			RendererListHandleType type = res.type;
			if (type != RendererListHandleType.Renderers)
			{
				return type == RendererListHandleType.Legacy && this.m_RendererListLegacyResources[res].isActive && this.m_RendererListLegacyResources[res].rendererList.isValid;
			}
			return this.m_RendererListResources[res].rendererList.isValid;
		}

		internal unsafe bool IsRenderGraphResourceImported(RenderGraphResourceType type, int index)
		{
			return this.m_RenderGraphResources[(int)type].resourceArray[index]->imported;
		}

		internal unsafe int GetRenderGraphResourceTransientIndex(in ResourceHandle res)
		{
			return this.m_RenderGraphResources[res.iType].resourceArray[res.index]->transientPassIndex;
		}

		internal TextureHandle ImportTexture(in RTHandle rt, bool isBuiltin = false)
		{
			ImportResourceParams importResourceParams = default(ImportResourceParams);
			importResourceParams.clearOnFirstUse = false;
			importResourceParams.discardOnLastUse = false;
			return this.ImportTexture(rt, importResourceParams, isBuiltin);
		}

		internal TextureHandle ImportTexture(in RTHandle rt, in ImportResourceParams importParams, bool isBuiltin = false)
		{
			if (rt != null && !(rt.m_RT != null))
			{
				rt.m_ExternalTexture != null;
			}
			TextureResource textureResource;
			int handle = this.m_RenderGraphResources[0].AddNewRenderGraphResource<TextureResource>(out textureResource, true);
			textureResource.graphicsResource = rt;
			textureResource.imported = true;
			RenderTexture renderTexture = (rt != null) ? ((rt.m_RT != null) ? rt.m_RT : (rt.m_ExternalTexture as RenderTexture)) : null;
			if (renderTexture)
			{
				textureResource.desc = new TextureDesc(renderTexture);
				textureResource.validDesc = true;
			}
			textureResource.desc.clearBuffer = importParams.clearOnFirstUse;
			textureResource.desc.clearColor = importParams.clearColor;
			textureResource.desc.discardBuffer = importParams.discardOnLastUse;
			TextureHandle result = new TextureHandle(handle, false, isBuiltin);
			RTHandle rthandle = rt;
			return result;
		}

		internal TextureHandle ImportTexture(in RTHandle rt, RenderTargetInfo info, in ImportResourceParams importParams)
		{
			TextureResource textureResource;
			int handle = this.m_RenderGraphResources[0].AddNewRenderGraphResource<TextureResource>(out textureResource, true);
			textureResource.graphicsResource = rt;
			textureResource.imported = true;
			textureResource.desc = default(TextureDesc);
			if (rt != null && rt.m_NameID != RenderGraphResourceRegistry.emptyId)
			{
				textureResource.desc.format = info.format;
				textureResource.desc.width = info.width;
				textureResource.desc.height = info.height;
				textureResource.desc.slices = info.volumeDepth;
				textureResource.desc.msaaSamples = (MSAASamples)info.msaaSamples;
				textureResource.desc.bindTextureMS = info.bindMS;
				textureResource.desc.clearBuffer = importParams.clearOnFirstUse;
				textureResource.desc.clearColor = importParams.clearColor;
				textureResource.desc.discardBuffer = importParams.discardOnLastUse;
				textureResource.validDesc = false;
			}
			return new TextureHandle(handle, false, false);
		}

		internal unsafe TextureHandle CreateSharedTexture(in TextureDesc desc, bool explicitRelease)
		{
			RenderGraphResourceRegistry.RenderGraphResourcesData renderGraphResourcesData = this.m_RenderGraphResources[0];
			int sharedResourcesCount = renderGraphResourcesData.sharedResourcesCount;
			TextureResource textureResource = null;
			int handle = -1;
			for (int i = 1; i < sharedResourcesCount + 1; i++)
			{
				if (!renderGraphResourcesData.resourceArray[i]->shared)
				{
					textureResource = (TextureResource)(*renderGraphResourcesData.resourceArray[i]);
					handle = i;
					break;
				}
			}
			if (textureResource == null)
			{
				handle = this.m_RenderGraphResources[0].AddNewRenderGraphResource<TextureResource>(out textureResource, false);
				renderGraphResourcesData.sharedResourcesCount++;
			}
			textureResource.imported = true;
			textureResource.shared = true;
			textureResource.sharedExplicitRelease = explicitRelease;
			textureResource.desc = desc;
			textureResource.validDesc = true;
			return new TextureHandle(handle, true, false);
		}

		internal void RefreshSharedTextureDesc(in TextureHandle texture, in TextureDesc desc)
		{
			TextureResource textureResource = this.GetTextureResource(texture.handle);
			textureResource.ReleaseGraphicsResource();
			textureResource.desc = desc;
		}

		internal void ReleaseSharedTexture(in TextureHandle texture)
		{
			RenderGraphResourceRegistry.RenderGraphResourcesData renderGraphResourcesData = this.m_RenderGraphResources[0];
			if (texture.handle.index == renderGraphResourcesData.sharedResourcesCount)
			{
				renderGraphResourcesData.sharedResourcesCount--;
			}
			TextureResource textureResource = this.GetTextureResource(texture.handle);
			textureResource.ReleaseGraphicsResource();
			textureResource.Reset(null);
		}

		internal TextureHandle ImportBackbuffer(RenderTargetIdentifier rt, in RenderTargetInfo info, in ImportResourceParams importParams)
		{
			if (this.m_CurrentBackbuffer != null)
			{
				this.m_CurrentBackbuffer.SetTexture(rt);
			}
			else
			{
				this.m_CurrentBackbuffer = RTHandles.Alloc(rt, "Backbuffer");
			}
			TextureResource textureResource;
			int handle = this.m_RenderGraphResources[0].AddNewRenderGraphResource<TextureResource>(out textureResource, true);
			textureResource.graphicsResource = this.m_CurrentBackbuffer;
			textureResource.imported = true;
			textureResource.desc = default(TextureDesc);
			textureResource.desc.width = info.width;
			textureResource.desc.height = info.height;
			textureResource.desc.slices = info.volumeDepth;
			textureResource.desc.msaaSamples = (MSAASamples)info.msaaSamples;
			textureResource.desc.bindTextureMS = info.bindMS;
			textureResource.desc.format = info.format;
			textureResource.desc.clearBuffer = importParams.clearOnFirstUse;
			textureResource.desc.clearColor = importParams.clearColor;
			textureResource.desc.discardBuffer = importParams.discardOnLastUse;
			textureResource.validDesc = false;
			return new TextureHandle(handle, false, false);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateRenderTarget(in ResourceHandle res)
		{
			if (RenderGraph.enableValidityChecks)
			{
				RenderTargetInfo renderTargetInfo;
				this.GetRenderTargetInfo(res, out renderTargetInfo);
			}
		}

		internal void GetRenderTargetInfo(in ResourceHandle res, out RenderTargetInfo outInfo)
		{
			TextureResource textureResource = this.GetTextureResource(res);
			if (!textureResource.imported)
			{
				TextureDesc textureResourceDesc = this.GetTextureResourceDesc(res, false);
				Vector2Int vector2Int = textureResourceDesc.CalculateFinalDimensions();
				outInfo = default(RenderTargetInfo);
				outInfo.width = vector2Int.x;
				outInfo.height = vector2Int.y;
				outInfo.volumeDepth = textureResourceDesc.slices;
				outInfo.msaaSamples = (int)textureResourceDesc.msaaSamples;
				outInfo.bindMS = textureResourceDesc.bindTextureMS;
				outInfo.format = textureResourceDesc.format;
				return;
			}
			RTHandle graphicsResource = textureResource.graphicsResource;
			if (graphicsResource == null)
			{
				outInfo = default(RenderTargetInfo);
				return;
			}
			if (graphicsResource.m_RT != null)
			{
				outInfo = default(RenderTargetInfo);
				outInfo.width = graphicsResource.m_RT.width;
				outInfo.height = graphicsResource.m_RT.height;
				outInfo.volumeDepth = graphicsResource.m_RT.volumeDepth;
				outInfo.format = this.GetFormat(graphicsResource.m_RT.graphicsFormat, graphicsResource.m_RT.depthStencilFormat);
				outInfo.msaaSamples = graphicsResource.m_RT.antiAliasing;
				outInfo.bindMS = graphicsResource.m_RT.bindTextureMS;
				return;
			}
			if (graphicsResource.m_ExternalTexture != null)
			{
				outInfo = default(RenderTargetInfo);
				outInfo.width = graphicsResource.m_ExternalTexture.width;
				outInfo.height = graphicsResource.m_ExternalTexture.height;
				outInfo.volumeDepth = 1;
				if (graphicsResource.m_ExternalTexture is RenderTexture)
				{
					RenderTexture renderTexture = (RenderTexture)graphicsResource.m_ExternalTexture;
					outInfo.format = this.GetFormat(renderTexture.graphicsFormat, renderTexture.depthStencilFormat);
					outInfo.msaaSamples = renderTexture.antiAliasing;
				}
				else
				{
					outInfo.format = graphicsResource.m_ExternalTexture.graphicsFormat;
					outInfo.msaaSamples = 1;
				}
				outInfo.bindMS = false;
				return;
			}
			if (graphicsResource.m_NameID != RenderGraphResourceRegistry.emptyId)
			{
				TextureDesc textureResourceDesc2 = this.GetTextureResourceDesc(res, true);
				outInfo.width = textureResourceDesc2.width;
				outInfo.height = textureResourceDesc2.height;
				outInfo.volumeDepth = textureResourceDesc2.slices;
				outInfo.msaaSamples = (int)textureResourceDesc2.msaaSamples;
				outInfo.format = textureResourceDesc2.format;
				outInfo.bindMS = textureResourceDesc2.bindTextureMS;
				return;
			}
			throw new Exception("Invalid imported texture. The RTHandle provided is invalid.");
		}

		internal GraphicsFormat GetFormat(GraphicsFormat color, GraphicsFormat depthStencil)
		{
			if (depthStencil == GraphicsFormat.None)
			{
				return color;
			}
			return depthStencil;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		internal void ValidateFormat(GraphicsFormat color, GraphicsFormat depthStencil)
		{
			if (RenderGraph.enableValidityChecks && color != GraphicsFormat.None && depthStencil != GraphicsFormat.None)
			{
				throw new Exception("Invalid imported texture. Both a color and a depthStencil format are provided. The texture needs to either have a color format or a depth stencil format.");
			}
		}

		internal TextureHandle CreateTexture(in TextureDesc desc, int transientPassIndex = -1)
		{
			TextureResource textureResource;
			int handle = this.m_RenderGraphResources[0].AddNewRenderGraphResource<TextureResource>(out textureResource, true);
			textureResource.desc = desc;
			textureResource.validDesc = true;
			textureResource.transientPassIndex = transientPassIndex;
			textureResource.requestFallBack = desc.fallBackToBlackTexture;
			return new TextureHandle(handle, false, false);
		}

		internal int GetResourceCount(RenderGraphResourceType type)
		{
			return this.m_RenderGraphResources[(int)type].resourceArray.size;
		}

		internal int GetTextureResourceCount()
		{
			return this.GetResourceCount(RenderGraphResourceType.Texture);
		}

		internal unsafe TextureResource GetTextureResource(in ResourceHandle handle)
		{
			return (*this.m_RenderGraphResources[0].resourceArray[handle.index]) as TextureResource;
		}

		internal unsafe TextureResource GetTextureResource(int index)
		{
			return (*this.m_RenderGraphResources[0].resourceArray[index]) as TextureResource;
		}

		internal unsafe TextureDesc GetTextureResourceDesc(in ResourceHandle handle, bool noThrowOnInvalidDesc = false)
		{
			TextureResource textureResource = (*this.m_RenderGraphResources[0].resourceArray[handle.index]) as TextureResource;
			if (!textureResource.validDesc && !noThrowOnInvalidDesc)
			{
				throw new ArgumentException("The passed in texture handle does not have a valid descriptor. (This is most commonly cause by the handle referencing a built-in texture such as the system back buffer.)", "handle");
			}
			return textureResource.desc;
		}

		internal RendererListHandle CreateRendererList(in RendererListDesc desc)
		{
			DynamicArray<RendererListResource> rendererListResources = this.m_RendererListResources;
			RendererListParams rendererListParams = RendererListDesc.ConvertToParameters(desc);
			RendererListResource rendererListResource = new RendererListResource(ref rendererListParams);
			return new RendererListHandle(rendererListResources.Add(rendererListResource), RendererListHandleType.Renderers);
		}

		internal RendererListHandle CreateRendererList(in RendererListParams desc)
		{
			DynamicArray<RendererListResource> rendererListResources = this.m_RendererListResources;
			RendererListResource rendererListResource = new RendererListResource(ref desc);
			return new RendererListHandle(rendererListResources.Add(rendererListResource), RendererListHandleType.Renderers);
		}

		internal RendererListHandle CreateShadowRendererList(ScriptableRenderContext context, ref ShadowDrawingSettings shadowDrawinSettings)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateShadowRendererList(ref shadowDrawinSettings);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateGizmoRendererList(ScriptableRenderContext context, in Camera camera, in GizmoSubset gizmoSubset)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateGizmoRendererList(camera, gizmoSubset);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateUIOverlayRendererList(ScriptableRenderContext context, in Camera camera, in UISubset uiSubset)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateUIOverlayRendererList(camera, uiSubset);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateWireOverlayRendererList(ScriptableRenderContext context, in Camera camera)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateWireOverlayRendererList(camera);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateSkyboxRendererList(ScriptableRenderContext context, in Camera camera)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateSkyboxRendererList(camera);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateSkyboxRendererList(ScriptableRenderContext context, in Camera camera, Matrix4x4 projectionMatrix, Matrix4x4 viewMatrix)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateSkyboxRendererList(camera, projectionMatrix, viewMatrix);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal RendererListHandle CreateSkyboxRendererList(ScriptableRenderContext context, in Camera camera, Matrix4x4 projectionMatrixL, Matrix4x4 viewMatrixL, Matrix4x4 projectionMatrixR, Matrix4x4 viewMatrixR)
		{
			RendererListLegacyResource rendererListLegacyResource = default(RendererListLegacyResource);
			rendererListLegacyResource.rendererList = context.CreateSkyboxRendererList(camera, projectionMatrixL, viewMatrixL, projectionMatrixR, viewMatrixR);
			return new RendererListHandle(this.m_RendererListLegacyResources.Add(rendererListLegacyResource), RendererListHandleType.Legacy);
		}

		internal BufferHandle ImportBuffer(GraphicsBuffer graphicsBuffer, bool forceRelease = false)
		{
			BufferResource bufferResource;
			int handle = this.m_RenderGraphResources[1].AddNewRenderGraphResource<BufferResource>(out bufferResource, true);
			bufferResource.graphicsResource = graphicsBuffer;
			bufferResource.imported = true;
			bufferResource.forceRelease = forceRelease;
			bufferResource.validDesc = false;
			return new BufferHandle(handle, false);
		}

		internal BufferHandle CreateBuffer(in BufferDesc desc, int transientPassIndex = -1)
		{
			BufferResource bufferResource;
			int handle = this.m_RenderGraphResources[1].AddNewRenderGraphResource<BufferResource>(out bufferResource, true);
			bufferResource.desc = desc;
			bufferResource.validDesc = true;
			bufferResource.transientPassIndex = transientPassIndex;
			return new BufferHandle(handle, false);
		}

		internal unsafe BufferDesc GetBufferResourceDesc(in ResourceHandle handle, bool noThrowOnInvalidDesc = false)
		{
			BufferResource bufferResource = (*this.m_RenderGraphResources[1].resourceArray[handle.index]) as BufferResource;
			if (!bufferResource.validDesc && !noThrowOnInvalidDesc)
			{
				throw new ArgumentException("The passed in buffer handle does not have a valid descriptor. (This is most commonly cause by importing the buffer.)", "handle");
			}
			return bufferResource.desc;
		}

		internal int GetBufferResourceCount()
		{
			return this.GetResourceCount(RenderGraphResourceType.Buffer);
		}

		private unsafe BufferResource GetBufferResource(in ResourceHandle handle)
		{
			return (*this.m_RenderGraphResources[1].resourceArray[handle.index]) as BufferResource;
		}

		private unsafe BufferResource GetBufferResource(int index)
		{
			return (*this.m_RenderGraphResources[1].resourceArray[index]) as BufferResource;
		}

		private unsafe RayTracingAccelerationStructureResource GetRayTracingAccelerationStructureResource(in ResourceHandle handle)
		{
			return (*this.m_RenderGraphResources[2].resourceArray[handle.index]) as RayTracingAccelerationStructureResource;
		}

		internal int GetRayTracingAccelerationStructureResourceCount()
		{
			return this.GetResourceCount(RenderGraphResourceType.AccelerationStructure);
		}

		internal RayTracingAccelerationStructureHandle ImportRayTracingAccelerationStructure(in RayTracingAccelerationStructure accelStruct, string name)
		{
			RayTracingAccelerationStructureResource rayTracingAccelerationStructureResource;
			int handle = this.m_RenderGraphResources[2].AddNewRenderGraphResource<RayTracingAccelerationStructureResource>(out rayTracingAccelerationStructureResource, false);
			rayTracingAccelerationStructureResource.graphicsResource = accelStruct;
			rayTracingAccelerationStructureResource.imported = true;
			rayTracingAccelerationStructureResource.forceRelease = false;
			rayTracingAccelerationStructureResource.desc.name = name;
			return new RayTracingAccelerationStructureHandle(handle);
		}

		internal unsafe void UpdateSharedResourceLastFrameIndex(int type, int index)
		{
			this.m_RenderGraphResources[type].resourceArray[index]->sharedResourceLastFrameUsed = this.m_ExecutionCount;
		}

		internal void UpdateSharedResourceLastFrameIndex(in ResourceHandle handle)
		{
			this.UpdateSharedResourceLastFrameIndex((int)handle.type, handle.index);
		}

		private unsafe void ManageSharedRenderGraphResources()
		{
			for (int i = 0; i < 3; i++)
			{
				RenderGraphResourceRegistry.RenderGraphResourcesData renderGraphResourcesData = this.m_RenderGraphResources[i];
				for (int j = 1; j < renderGraphResourcesData.sharedResourcesCount + 1; j++)
				{
					IRenderGraphResource renderGraphResource = *this.m_RenderGraphResources[i].resourceArray[j];
					bool flag = renderGraphResource.IsCreated();
					if (renderGraphResource.sharedResourceLastFrameUsed == this.m_ExecutionCount && !flag)
					{
						renderGraphResource.CreateGraphicsResource();
					}
					else if (flag && !renderGraphResource.sharedExplicitRelease && renderGraphResource.sharedResourceLastFrameUsed + 30 < this.m_ExecutionCount)
					{
						renderGraphResource.ReleaseGraphicsResource();
					}
				}
			}
		}

		internal unsafe bool CreatePooledResource(InternalRenderGraphContext rgContext, int type, int index)
		{
			bool? flag = new bool?(false);
			IRenderGraphResource renderGraphResource = *this.m_RenderGraphResources[type].resourceArray[index];
			if (!renderGraphResource.imported)
			{
				renderGraphResource.CreatePooledGraphicsResource();
				if (this.m_RenderGraphDebug.enableLogging)
				{
					renderGraphResource.LogCreation(this.m_FrameInformationLogger);
				}
				RenderGraphResourceRegistry.ResourceCreateCallback createResourceCallback = this.m_RenderGraphResources[type].createResourceCallback;
				flag = ((createResourceCallback != null) ? new bool?(createResourceCallback(rgContext, renderGraphResource)) : null);
			}
			return flag.GetValueOrDefault();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool CreatePooledResource(InternalRenderGraphContext rgContext, in ResourceHandle handle)
		{
			return this.CreatePooledResource(rgContext, handle.iType, handle.index);
		}

		private bool CreateTextureCallback(InternalRenderGraphContext rgContext, IRenderGraphResource res)
		{
			TextureResource textureResource = res as TextureResource;
			FastMemoryDesc fastMemoryDesc = textureResource.desc.fastMemoryDesc;
			if (fastMemoryDesc.inFastMemory)
			{
				textureResource.graphicsResource.SwitchToFastMemory(rgContext.cmd, fastMemoryDesc.residencyFraction, fastMemoryDesc.flags, false);
			}
			bool result = false;
			if ((this.forceManualClearOfResource && textureResource.desc.clearBuffer) || this.m_RenderGraphDebug.clearRenderTargetsAtCreation)
			{
				this.ClearTexture(rgContext, textureResource);
				result = true;
			}
			return result;
		}

		internal unsafe void ClearResource(InternalRenderGraphContext rgContext, int type, int index)
		{
			TextureResource textureResource = (*this.m_RenderGraphResources[type].resourceArray[index]) as TextureResource;
			if (textureResource != null)
			{
				this.ClearTexture(rgContext, textureResource);
			}
		}

		private void ClearTexture(InternalRenderGraphContext rgContext, TextureResource resource)
		{
			if (resource == null)
			{
				return;
			}
			bool flag = this.m_RenderGraphDebug.clearRenderTargetsAtCreation && !resource.desc.clearBuffer;
			ClearFlag clearFlag = GraphicsFormatUtility.IsDepthStencilFormat(resource.desc.format) ? ClearFlag.DepthStencil : ClearFlag.Color;
			Color clearColor = flag ? Color.magenta : resource.desc.clearColor;
			CoreUtils.SetRenderTarget(rgContext.cmd, resource.graphicsResource, clearFlag, clearColor, 0, CubemapFace.Unknown, -1);
		}

		internal unsafe void ReleasePooledResource(InternalRenderGraphContext rgContext, int type, int index)
		{
			IRenderGraphResource renderGraphResource = *this.m_RenderGraphResources[type].resourceArray[index];
			if (!renderGraphResource.imported || renderGraphResource.forceRelease)
			{
				RenderGraphResourceRegistry.ResourceCallback releaseResourceCallback = this.m_RenderGraphResources[type].releaseResourceCallback;
				if (releaseResourceCallback != null)
				{
					releaseResourceCallback(rgContext, renderGraphResource);
				}
				if (this.m_RenderGraphDebug.enableLogging)
				{
					renderGraphResource.LogRelease(this.m_FrameInformationLogger);
				}
				renderGraphResource.ReleasePooledGraphicsResource(this.m_CurrentFrameIndex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ReleasePooledResource(InternalRenderGraphContext rgContext, in ResourceHandle handle)
		{
			this.ReleasePooledResource(rgContext, handle.iType, handle.index);
		}

		private void ReleaseTextureCallback(InternalRenderGraphContext rgContext, IRenderGraphResource res)
		{
			TextureResource textureResource = res as TextureResource;
			if (this.m_RenderGraphDebug.clearRenderTargetsAtRelease)
			{
				ClearFlag clearFlag = GraphicsFormatUtility.IsDepthStencilFormat(textureResource.desc.format) ? ClearFlag.DepthStencil : ClearFlag.Color;
				CoreUtils.SetRenderTarget(rgContext.cmd, textureResource.graphicsResource, clearFlag, Color.magenta, 0, CubemapFace.Unknown, -1);
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateTextureDesc(in TextureDesc desc)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (desc.format == GraphicsFormat.None)
				{
					throw new ArgumentException("Texture was created with with no format. The texture needs to either have a color format or a depth stencil format.");
				}
				if (desc.dimension == TextureDimension.None || desc.dimension == TextureDimension.Any)
				{
					throw new ArgumentException("Texture was created with an invalid texture dimension.");
				}
				if (desc.slices == 0)
				{
					throw new ArgumentException("Texture was created with a slices parameter value of zero.");
				}
				if (desc.slices > 1 && (desc.dimension == TextureDimension.Tex2D || desc.dimension == TextureDimension.Cube) && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
				{
					throw new ArgumentException("Non-array texture was created with a slices parameter larger than one.");
				}
				if (desc.msaaSamples <= MSAASamples.None && desc.bindTextureMS)
				{
					throw new ArgumentException("A single sample texture was created with bindTextureMS.");
				}
				if (desc.sizeMode == TextureSizeMode.Explicit && (desc.width == 0 || desc.height == 0))
				{
					throw new ArgumentException("Texture using Explicit size mode was create with either width or height at zero.");
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateRendererListDesc(in RendererListDesc desc)
		{
			if (RenderGraph.enableValidityChecks)
			{
				RendererListDesc rendererListDesc = desc;
				if (!rendererListDesc.IsValid())
				{
					throw new ArgumentException("Renderer List descriptor is not valid.");
				}
				RenderQueueRange renderQueueRange = desc.renderQueueRange;
				if (renderQueueRange.lowerBound == 0)
				{
					renderQueueRange = desc.renderQueueRange;
					if (renderQueueRange.upperBound == 0)
					{
						throw new ArgumentException("Renderer List creation descriptor must have a valid RenderQueueRange.");
					}
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateBufferDesc(in BufferDesc desc)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (desc.stride % 4 != 0)
				{
					throw new ArgumentException("Invalid Graphics Buffer creation descriptor: Graphics Buffer stride must be at least 4.");
				}
				if (desc.count == 0)
				{
					throw new ArgumentException("Invalid Graphics Buffer creation descriptor: Graphics Buffer count  must be non zero.");
				}
			}
		}

		internal void CreateRendererLists(List<RendererListHandle> rendererLists, ScriptableRenderContext context, bool manualDispatch = false)
		{
			this.m_ActiveRendererLists.Clear();
			foreach (RendererListHandle rendererListHandle in rendererLists)
			{
				RendererListHandleType type = rendererListHandle.type;
				if (type != RendererListHandleType.Renderers)
				{
					if (type == RendererListHandleType.Legacy)
					{
						this.m_RendererListLegacyResources[rendererListHandle].isActive = true;
					}
				}
				else
				{
					ref RendererListResource ptr = ref this.m_RendererListResources[rendererListHandle];
					ref RendererListParams param = ref ptr.desc;
					ptr.rendererList = context.CreateRendererList(ref param);
					this.m_ActiveRendererLists.Add(ptr.rendererList);
				}
			}
			if (manualDispatch)
			{
				context.PrepareRendererListsAsync(this.m_ActiveRendererLists);
			}
		}

		internal void Clear(bool onException)
		{
			this.LogResources();
			for (int i = 0; i < 3; i++)
			{
				this.m_RenderGraphResources[i].Clear(onException, this.m_CurrentFrameIndex);
			}
			this.m_RendererListResources.Clear();
			this.m_RendererListLegacyResources.Clear();
			this.m_ActiveRendererLists.Clear();
		}

		internal void PurgeUnusedGraphicsResources()
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_RenderGraphResources[i].PurgeUnusedGraphicsResources(this.m_CurrentFrameIndex);
			}
		}

		internal void Cleanup()
		{
			for (int i = 0; i < 3; i++)
			{
				this.m_RenderGraphResources[i].Cleanup();
			}
			RTHandles.Release(this.m_CurrentBackbuffer);
		}

		private void LogResources()
		{
			if (this.m_RenderGraphDebug.enableLogging)
			{
				this.m_ResourceLogger.LogLine("==== Render Graph Resource Log ====\n", Array.Empty<object>());
				for (int i = 0; i < 3; i++)
				{
					if (this.m_RenderGraphResources[i].pool != null)
					{
						this.m_RenderGraphResources[i].pool.LogResources(this.m_ResourceLogger);
						this.m_ResourceLogger.LogLine("", Array.Empty<object>());
					}
				}
			}
		}

		internal void FlushLogs()
		{
			this.m_ResourceLogger.FlushLogs();
		}

		private const int kSharedResourceLifetime = 30;

		private static RenderGraphResourceRegistry m_CurrentRegistry;

		private RenderGraphResourceRegistry.RenderGraphResourcesData[] m_RenderGraphResources = new RenderGraphResourceRegistry.RenderGraphResourcesData[3];

		private DynamicArray<RendererListResource> m_RendererListResources = new DynamicArray<RendererListResource>();

		private DynamicArray<RendererListLegacyResource> m_RendererListLegacyResources = new DynamicArray<RendererListLegacyResource>();

		private RenderGraphDebugParams m_RenderGraphDebug;

		private RenderGraphLogger m_ResourceLogger = new RenderGraphLogger();

		private RenderGraphLogger m_FrameInformationLogger;

		private int m_CurrentFrameIndex;

		private int m_ExecutionCount;

		private RTHandle m_CurrentBackbuffer;

		private const int kInitialRendererListCount = 256;

		private List<RendererList> m_ActiveRendererLists = new List<RendererList>(256);

		private static RenderTargetIdentifier emptyId = RenderTargetIdentifier.Invalid;

		private static RenderTargetIdentifier builtinCameraRenderTarget = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);

		internal bool forceManualClearOfResource = true;

		private delegate bool ResourceCreateCallback(InternalRenderGraphContext rgContext, IRenderGraphResource res);

		private delegate void ResourceCallback(InternalRenderGraphContext rgContext, IRenderGraphResource res);

		private class RenderGraphResourcesData
		{
			public RenderGraphResourcesData()
			{
				this.resourceArray.Resize(1, false);
			}

			public void Clear(bool onException, int frameIndex)
			{
				this.resourceArray.Resize(this.sharedResourcesCount + 1, false);
				if (this.pool != null)
				{
					this.pool.CheckFrameAllocation(onException, frameIndex);
				}
			}

			public unsafe void Cleanup()
			{
				for (int i = 1; i < this.sharedResourcesCount + 1; i++)
				{
					IRenderGraphResource renderGraphResource = *this.resourceArray[i];
					if (renderGraphResource != null)
					{
						renderGraphResource.ReleaseGraphicsResource();
					}
				}
				if (this.pool != null)
				{
					this.pool.Cleanup();
				}
			}

			public void PurgeUnusedGraphicsResources(int frameIndex)
			{
				if (this.pool != null)
				{
					this.pool.PurgeUnusedResources(frameIndex);
				}
			}

			public unsafe int AddNewRenderGraphResource<ResType>(out ResType outRes, bool pooledResource = true) where ResType : IRenderGraphResource, new()
			{
				int size = this.resourceArray.size;
				this.resourceArray.Resize(this.resourceArray.size + 1, true);
				if (*this.resourceArray[size] == null)
				{
					*this.resourceArray[size] = Activator.CreateInstance<ResType>();
				}
				outRes = ((*this.resourceArray[size]) as ResType);
				outRes.Reset(pooledResource ? this.pool : null);
				return size;
			}

			public DynamicArray<IRenderGraphResource> resourceArray = new DynamicArray<IRenderGraphResource>();

			public int sharedResourcesCount;

			public IRenderGraphResourcePool pool;

			public RenderGraphResourceRegistry.ResourceCreateCallback createResourceCallback;

			public RenderGraphResourceRegistry.ResourceCallback releaseResourceCallback;
		}
	}
}
