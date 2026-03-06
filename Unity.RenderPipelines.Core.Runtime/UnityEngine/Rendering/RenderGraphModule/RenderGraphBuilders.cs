using System;
using System.Diagnostics;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal class RenderGraphBuilders : IBaseRenderGraphBuilder, IDisposable, IComputeRenderGraphBuilder, IRasterRenderGraphBuilder, IUnsafeRenderGraphBuilder
	{
		public RenderGraphBuilders()
		{
			this.m_RenderPass = null;
			this.m_Resources = null;
			this.m_RenderGraph = null;
			this.m_Disposed = true;
		}

		public void Setup(RenderGraphPass renderPass, RenderGraphResourceRegistry resources, RenderGraph renderGraph)
		{
			this.m_RenderPass = renderPass;
			this.m_Resources = resources;
			this.m_RenderGraph = renderGraph;
			this.m_Disposed = false;
			renderPass.useAllGlobalTextures = false;
			if (renderPass.type == RenderGraphPassType.Raster)
			{
				CommandBuffer.ThrowOnSetRenderTarget = true;
			}
		}

		public void EnableAsyncCompute(bool value)
		{
			this.m_RenderPass.EnableAsyncCompute(value);
		}

		public void AllowPassCulling(bool value)
		{
			if (value && this.m_RenderPass.allowGlobalState)
			{
				return;
			}
			this.m_RenderPass.AllowPassCulling(value);
		}

		public void AllowGlobalStateModification(bool value)
		{
			this.m_RenderPass.AllowGlobalState(value);
			if (value)
			{
				this.AllowPassCulling(false);
			}
		}

		public void EnableFoveatedRasterization(bool value)
		{
			this.m_RenderPass.EnableFoveatedRasterization(value);
		}

		public BufferHandle CreateTransientBuffer(in BufferDesc desc)
		{
			BufferHandle result = this.m_Resources.CreateBuffer(desc, this.m_RenderPass.index);
			this.UseResource(result.handle, AccessFlags.ReadWrite, true);
			return result;
		}

		public BufferHandle CreateTransientBuffer(in BufferHandle computebuffer)
		{
			BufferDesc bufferResourceDesc = this.m_Resources.GetBufferResourceDesc(computebuffer.handle, false);
			return this.CreateTransientBuffer(bufferResourceDesc);
		}

		public TextureHandle CreateTransientTexture(in TextureDesc desc)
		{
			TextureHandle result = this.m_Resources.CreateTexture(desc, this.m_RenderPass.index);
			this.UseResource(result.handle, AccessFlags.ReadWrite, true);
			return result;
		}

		public TextureHandle CreateTransientTexture(in TextureHandle texture)
		{
			TextureDesc textureResourceDesc = this.m_Resources.GetTextureResourceDesc(texture.handle, false);
			return this.CreateTransientTexture(textureResourceDesc);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_Disposed)
			{
				return;
			}
			try
			{
				if (disposing)
				{
					this.m_RenderGraph.RenderGraphState = RenderGraphState.RecordingGraph;
					if (this.m_RenderPass.useAllGlobalTextures)
					{
						foreach (TextureHandle textureHandle in this.m_RenderGraph.AllGlobals())
						{
							if (textureHandle.IsValid())
							{
								this.UseTexture(textureHandle, AccessFlags.Read);
							}
						}
					}
					foreach (ValueTuple<TextureHandle, int> valueTuple in this.m_RenderPass.setGlobalsList)
					{
						this.m_RenderGraph.SetGlobal(valueTuple.Item1, valueTuple.Item2);
					}
					this.m_RenderGraph.OnPassAdded(this.m_RenderPass);
				}
			}
			finally
			{
				if (this.m_RenderPass.type == RenderGraphPassType.Raster)
				{
					CommandBuffer.ThrowOnSetRenderTarget = false;
				}
				this.m_RenderPass = null;
				this.m_Resources = null;
				this.m_RenderGraph = null;
				this.m_Disposed = true;
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateWriteTo(in ResourceHandle handle)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (handle.IsVersioned)
				{
					string renderGraphResourceName = this.m_Resources.GetRenderGraphResourceName(handle);
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Trying to write to a versioned resource handle. You can only write to unversioned resource handles to avoid branches in the resource history. (pass ",
						this.m_RenderPass.name,
						" resource",
						renderGraphResourceName,
						")."
					}));
				}
				if (this.m_RenderPass.IsWritten(handle))
				{
					string renderGraphResourceName2 = this.m_Resources.GetRenderGraphResourceName(handle);
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Trying to write a resource twice in a pass. You can only write the same resource once within a pass (pass ",
						this.m_RenderPass.name,
						" resource",
						renderGraphResourceName2,
						")."
					}));
				}
			}
		}

		private ResourceHandle UseResource(in ResourceHandle handle, AccessFlags flags, bool isTransient = false)
		{
			if ((flags & AccessFlags.Discard) == AccessFlags.None)
			{
				ResourceHandle item;
				if (!handle.IsVersioned)
				{
					item = this.m_Resources.GetLatestVersionHandle(handle);
				}
				else
				{
					item = handle;
				}
				if (isTransient)
				{
					this.m_RenderPass.AddTransientResource(item);
					return this.GetLatestVersionHandle(handle);
				}
				this.m_RenderPass.AddResourceRead(item);
				this.m_Resources.IncrementReadCount(handle);
				if ((flags & AccessFlags.Read) == AccessFlags.None)
				{
					this.m_RenderPass.implicitReadsList.Add(item);
				}
			}
			else if ((flags & AccessFlags.Read) != AccessFlags.None)
			{
				RenderGraphPass renderPass = this.m_RenderPass;
				ResourceHandle resourceHandle = this.m_Resources.GetZeroVersionedHandle(handle);
				renderPass.AddResourceRead(resourceHandle);
				this.m_Resources.IncrementReadCount(handle);
			}
			if ((flags & AccessFlags.Write) != AccessFlags.None)
			{
				RenderGraphPass renderPass2 = this.m_RenderPass;
				ResourceHandle resourceHandle = this.m_Resources.GetNewVersionedHandle(handle);
				renderPass2.AddResourceWrite(resourceHandle);
				this.m_Resources.IncrementWriteCount(handle);
			}
			return this.GetLatestVersionHandle(handle);
		}

		public BufferHandle UseBuffer(in BufferHandle input, AccessFlags flags)
		{
			this.UseResource(input.handle, flags, false);
			return input;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckNotUseFragment(TextureHandle tex)
		{
			if (RenderGraph.enableValidityChecks)
			{
				bool flag = this.m_RenderPass.depthAccess.textureHandle.IsValid() && this.m_RenderPass.depthAccess.textureHandle.handle.index == tex.handle.index;
				if (!flag)
				{
					for (int i = 0; i <= this.m_RenderPass.colorBufferMaxIndex; i++)
					{
						if (this.m_RenderPass.colorBufferAccess[i].textureHandle.IsValid() && this.m_RenderPass.colorBufferAccess[i].textureHandle.handle.index == tex.handle.index)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					string renderGraphResourceName = this.m_Resources.GetRenderGraphResourceName(tex.handle);
					throw new ArgumentException(string.Concat(new string[]
					{
						"Trying to UseTexture on a texture that is already used through SetRenderAttachment. Consider updating your code. (pass ",
						this.m_RenderPass.name,
						" resource",
						renderGraphResourceName,
						")."
					}));
				}
			}
		}

		public void UseTexture(in TextureHandle input, AccessFlags flags)
		{
			this.UseResource(input.handle, flags, false);
		}

		public void UseGlobalTexture(int propertyId, AccessFlags flags)
		{
			TextureHandle global = this.m_RenderGraph.GetGlobal(propertyId);
			if (global.IsValid())
			{
				this.UseTexture(global, flags);
				return;
			}
			throw new ArgumentException(string.Format("Trying to read global texture property {0} but no previous pass in the graph assigned a value to this global.", propertyId));
		}

		public void UseAllGlobalTextures(bool enable)
		{
			this.m_RenderPass.useAllGlobalTextures = enable;
		}

		public void SetGlobalTextureAfterPass(in TextureHandle input, int propertyId)
		{
			this.m_RenderPass.setGlobalsList.Add(ValueTuple.Create<TextureHandle, int>(input, propertyId));
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckUseFragment(TextureHandle tex, bool isDepth)
		{
			if (RenderGraph.enableValidityChecks)
			{
				bool flag = false;
				for (int i = 0; i < this.m_RenderPass.resourceReadLists[tex.handle.iType].Count; i++)
				{
					if (this.m_RenderPass.resourceReadLists[tex.handle.iType][i].index == tex.handle.index)
					{
						flag = true;
						break;
					}
				}
				for (int j = 0; j < this.m_RenderPass.resourceWriteLists[tex.handle.iType].Count; j++)
				{
					if (this.m_RenderPass.resourceWriteLists[tex.handle.iType][j].index == tex.handle.index)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					string renderGraphResourceName = this.m_Resources.GetRenderGraphResourceName(tex.handle);
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Trying to SetRenderAttachment on a texture that is already used through UseTexture/SetRenderAttachment. Consider updating your code. (pass '",
						this.m_RenderPass.name,
						"' resource '",
						renderGraphResourceName,
						"')."
					}));
				}
				RenderTargetInfo renderTargetInfo;
				this.m_Resources.GetRenderTargetInfo(tex.handle, out renderTargetInfo);
				if (this.m_RenderGraph.nativeRenderPassesEnabled)
				{
					if (isDepth)
					{
						if (!GraphicsFormatUtility.IsDepthFormat(renderTargetInfo.format))
						{
							string renderGraphResourceName2 = this.m_Resources.GetRenderGraphResourceName(tex.handle);
							throw new InvalidOperationException(string.Format("Trying to SetRenderAttachmentDepth on a texture that has a color format {0}. Use a texture with a depth format instead. (pass '{1}' resource '{2}').", renderTargetInfo.format, this.m_RenderPass.name, renderGraphResourceName2));
						}
					}
					else if (GraphicsFormatUtility.IsDepthFormat(renderTargetInfo.format))
					{
						string renderGraphResourceName3 = this.m_Resources.GetRenderGraphResourceName(tex.handle);
						throw new InvalidOperationException(string.Concat(new string[]
						{
							"Trying to SetRenderAttachment on a texture that has a depth format. Use a texture with a color format instead. (pass '",
							this.m_RenderPass.name,
							"' resource '",
							renderGraphResourceName3,
							"')."
						}));
					}
				}
				foreach (ValueTuple<TextureHandle, int> valueTuple in this.m_RenderPass.setGlobalsList)
				{
					if (valueTuple.Item1.handle.index == tex.handle.index)
					{
						throw new InvalidOperationException("Trying to SetRenderAttachment on a texture that is currently set on a global texture slot. Shaders might be using the texture using samplers. You should ensure textures are not set as globals when using them as fragment attachments.");
					}
				}
			}
		}

		public void SetRenderAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice)
		{
			ResourceHandle handle = this.UseResource(tex.handle, flags, false);
			TextureHandle textureHandle = default(TextureHandle);
			textureHandle.handle = handle;
			this.m_RenderPass.SetColorBufferRaw(textureHandle, index, flags, mipLevel, depthSlice);
		}

		public void SetInputAttachment(TextureHandle tex, int index, AccessFlags flags, int mipLevel, int depthSlice)
		{
			ResourceHandle handle = this.UseResource(tex.handle, flags, false);
			TextureHandle textureHandle = default(TextureHandle);
			textureHandle.handle = handle;
			this.m_RenderPass.SetFragmentInputRaw(textureHandle, index, flags, mipLevel, depthSlice);
		}

		public void SetRenderAttachmentDepth(TextureHandle tex, AccessFlags flags, int mipLevel, int depthSlice)
		{
			ResourceHandle handle = this.UseResource(tex.handle, flags, false);
			TextureHandle textureHandle = default(TextureHandle);
			textureHandle.handle = handle;
			this.m_RenderPass.SetDepthBufferRaw(textureHandle, flags, mipLevel, depthSlice);
		}

		public TextureHandle SetRandomAccessAttachment(TextureHandle input, int index, AccessFlags flags = AccessFlags.Read)
		{
			ResourceHandle handle = this.UseResource(input.handle, flags, false);
			TextureHandle textureHandle = default(TextureHandle);
			textureHandle.handle = handle;
			this.m_RenderPass.SetRandomWriteResourceRaw(textureHandle.handle, index, false, flags);
			return input;
		}

		public BufferHandle UseBufferRandomAccess(BufferHandle input, int index, AccessFlags flags = AccessFlags.Read)
		{
			BufferHandle bufferHandle = this.UseBuffer(input, flags);
			this.m_RenderPass.SetRandomWriteResourceRaw(bufferHandle.handle, index, true, flags);
			return input;
		}

		public BufferHandle UseBufferRandomAccess(BufferHandle input, int index, bool preserveCounterValue, AccessFlags flags = AccessFlags.Read)
		{
			BufferHandle bufferHandle = this.UseBuffer(input, flags);
			this.m_RenderPass.SetRandomWriteResourceRaw(bufferHandle.handle, index, preserveCounterValue, flags);
			return input;
		}

		public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, ComputeGraphContext> renderFunc) where PassData : class, new()
		{
			((ComputeRenderGraphPass<PassData>)this.m_RenderPass).renderFunc = renderFunc;
		}

		public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, RasterGraphContext> renderFunc) where PassData : class, new()
		{
			((RasterRenderGraphPass<PassData>)this.m_RenderPass).renderFunc = renderFunc;
		}

		public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, UnsafeGraphContext> renderFunc) where PassData : class, new()
		{
			((UnsafeRenderGraphPass<PassData>)this.m_RenderPass).renderFunc = renderFunc;
		}

		public void UseRendererList(in RendererListHandle input)
		{
			this.m_RenderPass.UseRendererList(input);
		}

		private ResourceHandle GetLatestVersionHandle(in ResourceHandle handle)
		{
			if (this.m_Resources.GetRenderGraphResourceTransientIndex(handle) >= 0)
			{
				return handle;
			}
			return this.m_Resources.GetLatestVersionHandle(handle);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckResource(in ResourceHandle res, bool checkTransientReadWrite = false)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (!res.IsValid())
				{
					throw new Exception("Trying to use an invalid resource (pass " + this.m_RenderPass.name + ").");
				}
				int renderGraphResourceTransientIndex = this.m_Resources.GetRenderGraphResourceTransientIndex(res);
				if (renderGraphResourceTransientIndex == this.m_RenderPass.index && checkTransientReadWrite)
				{
					Debug.LogError("Trying to read or write a transient resource at pass " + this.m_RenderPass.name + ".Transient resource are always assumed to be both read and written.");
				}
				if (renderGraphResourceTransientIndex != -1 && renderGraphResourceTransientIndex != this.m_RenderPass.index)
				{
					throw new ArgumentException(string.Format("Trying to use a transient {0} (pass index {1}) in a different pass (pass index {2}).", res.type, renderGraphResourceTransientIndex, this.m_RenderPass.index));
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckFrameBufferFetchEmulationIsSupported(in TextureHandle tex)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (!RenderGraphUtils.IsFramebufferFetchEmulationSupportedOnCurrentPlatform())
				{
					throw new InvalidOperationException(string.Format("This API is not supported on the current platform: {0}", SystemInfo.graphicsDeviceType));
				}
				if (!RenderGraphUtils.IsFramebufferFetchEmulationMSAASupportedOnCurrentPlatform() && this.m_RenderGraph.GetRenderTargetInfo(tex).bindMS)
				{
					throw new InvalidOperationException(string.Format("This API is not supported with MSAA attachments on the current platform: {0}", SystemInfo.graphicsDeviceType));
				}
			}
		}

		public void SetShadingRateImageAttachment(in TextureHandle sriTextureHandle)
		{
			TextureHandle textureHandle = default(TextureHandle);
			textureHandle.handle = this.UseResource(sriTextureHandle.handle, AccessFlags.Read, false);
			this.m_RenderPass.SetShadingRateImage(textureHandle, AccessFlags.Read, 0, 0);
		}

		public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
		{
			this.m_RenderPass.SetShadingRateFragmentSize(shadingRateFragmentSize);
		}

		public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			this.m_RenderPass.SetShadingRateCombiner(stage, combiner);
		}

		void IRasterRenderGraphBuilder.SetShadingRateImageAttachment(in TextureHandle tex)
		{
			this.SetShadingRateImageAttachment(tex);
		}

		void IBaseRenderGraphBuilder.UseTexture(in TextureHandle input, AccessFlags flags)
		{
			this.UseTexture(input, flags);
		}

		void IBaseRenderGraphBuilder.SetGlobalTextureAfterPass(in TextureHandle input, int propertyId)
		{
			this.SetGlobalTextureAfterPass(input, propertyId);
		}

		BufferHandle IBaseRenderGraphBuilder.UseBuffer(in BufferHandle input, AccessFlags flags)
		{
			return this.UseBuffer(input, flags);
		}

		TextureHandle IBaseRenderGraphBuilder.CreateTransientTexture(in TextureDesc desc)
		{
			return this.CreateTransientTexture(desc);
		}

		TextureHandle IBaseRenderGraphBuilder.CreateTransientTexture(in TextureHandle texture)
		{
			return this.CreateTransientTexture(texture);
		}

		BufferHandle IBaseRenderGraphBuilder.CreateTransientBuffer(in BufferDesc desc)
		{
			return this.CreateTransientBuffer(desc);
		}

		BufferHandle IBaseRenderGraphBuilder.CreateTransientBuffer(in BufferHandle computebuffer)
		{
			return this.CreateTransientBuffer(computebuffer);
		}

		void IBaseRenderGraphBuilder.UseRendererList(in RendererListHandle input)
		{
			this.UseRendererList(input);
		}

		private RenderGraphPass m_RenderPass;

		private RenderGraphResourceRegistry m_Resources;

		private RenderGraph m_RenderGraph;

		private bool m_Disposed;
	}
}
