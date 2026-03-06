using System;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public struct RenderGraphBuilder : IDisposable
	{
		public TextureHandle UseColorBuffer(in TextureHandle input, int index)
		{
			this.m_Resources.IncrementWriteCount(input.handle);
			this.m_RenderPass.SetColorBuffer(input, index);
			return input;
		}

		public TextureHandle UseDepthBuffer(in TextureHandle input, DepthAccess flags)
		{
			if ((flags & DepthAccess.Write) != (DepthAccess)0)
			{
				this.m_Resources.IncrementWriteCount(input.handle);
			}
			if ((flags & DepthAccess.Read) != (DepthAccess)0 && !this.m_Resources.IsRenderGraphResourceImported(input.handle) && this.m_Resources.TextureNeedsFallback(input))
			{
				this.WriteTexture(input);
			}
			this.m_RenderPass.SetDepthBuffer(input, flags);
			return input;
		}

		public TextureHandle ReadTexture(in TextureHandle input)
		{
			if (!this.m_Resources.IsRenderGraphResourceImported(input.handle) && this.m_Resources.TextureNeedsFallback(input))
			{
				TextureResource textureResource = this.m_Resources.GetTextureResource(input.handle);
				textureResource.desc.clearBuffer = true;
				textureResource.desc.clearColor = Color.black;
				TextureHandle result;
				if (this.m_RenderGraph.GetImportedFallback(textureResource.desc, out result))
				{
					return result;
				}
				this.WriteTexture(input);
			}
			this.m_RenderPass.AddResourceRead(input.handle);
			return input;
		}

		public TextureHandle WriteTexture(in TextureHandle input)
		{
			this.m_Resources.IncrementWriteCount(input.handle);
			this.m_RenderPass.AddResourceWrite(input.handle);
			return input;
		}

		public TextureHandle ReadWriteTexture(in TextureHandle input)
		{
			this.m_Resources.IncrementWriteCount(input.handle);
			this.m_RenderPass.AddResourceWrite(input.handle);
			this.m_RenderPass.AddResourceRead(input.handle);
			return input;
		}

		public TextureHandle CreateTransientTexture(in TextureDesc desc)
		{
			TextureHandle result = this.m_Resources.CreateTexture(desc, this.m_RenderPass.index);
			this.m_RenderPass.AddTransientResource(result.handle);
			return result;
		}

		public TextureHandle CreateTransientTexture(in TextureHandle texture)
		{
			TextureDesc textureResourceDesc = this.m_Resources.GetTextureResourceDesc(texture.handle, false);
			TextureHandle result = this.m_Resources.CreateTexture(textureResourceDesc, this.m_RenderPass.index);
			this.m_RenderPass.AddTransientResource(result.handle);
			return result;
		}

		public RayTracingAccelerationStructureHandle WriteRayTracingAccelerationStructure(in RayTracingAccelerationStructureHandle input)
		{
			this.m_Resources.IncrementWriteCount(input.handle);
			this.m_RenderPass.AddResourceWrite(input.handle);
			return input;
		}

		public RayTracingAccelerationStructureHandle ReadRayTracingAccelerationStructure(in RayTracingAccelerationStructureHandle input)
		{
			this.m_RenderPass.AddResourceRead(input.handle);
			return input;
		}

		public RendererListHandle UseRendererList(in RendererListHandle input)
		{
			RendererListHandle rendererListHandle = input;
			if (rendererListHandle.IsValid())
			{
				this.m_RenderPass.UseRendererList(input);
			}
			return input;
		}

		public BufferHandle ReadBuffer(in BufferHandle input)
		{
			this.m_RenderPass.AddResourceRead(input.handle);
			return input;
		}

		public BufferHandle WriteBuffer(in BufferHandle input)
		{
			this.m_RenderPass.AddResourceWrite(input.handle);
			this.m_Resources.IncrementWriteCount(input.handle);
			return input;
		}

		public BufferHandle CreateTransientBuffer(in BufferDesc desc)
		{
			BufferHandle result = this.m_Resources.CreateBuffer(desc, this.m_RenderPass.index);
			this.m_RenderPass.AddTransientResource(result.handle);
			return result;
		}

		public BufferHandle CreateTransientBuffer(in BufferHandle graphicsbuffer)
		{
			BufferDesc bufferResourceDesc = this.m_Resources.GetBufferResourceDesc(graphicsbuffer.handle, false);
			BufferHandle result = this.m_Resources.CreateBuffer(bufferResourceDesc, this.m_RenderPass.index);
			this.m_RenderPass.AddTransientResource(result.handle);
			return result;
		}

		public void SetRenderFunc<PassData>(BaseRenderFunc<PassData, RenderGraphContext> renderFunc) where PassData : class, new()
		{
			((RenderGraphPass<PassData>)this.m_RenderPass).renderFunc = renderFunc;
		}

		public void EnableAsyncCompute(bool value)
		{
			this.m_RenderPass.EnableAsyncCompute(value);
		}

		public void AllowPassCulling(bool value)
		{
			this.m_RenderPass.AllowPassCulling(value);
		}

		public void EnableFoveatedRasterization(bool value)
		{
			this.m_RenderPass.EnableFoveatedRasterization(value);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public void AllowRendererListCulling(bool value)
		{
			this.m_RenderPass.AllowRendererListCulling(value);
		}

		public RendererListHandle DependsOn(in RendererListHandle input)
		{
			this.m_RenderPass.UseRendererList(input);
			return input;
		}

		internal RenderGraphBuilder(RenderGraphPass renderPass, RenderGraphResourceRegistry resources, RenderGraph renderGraph)
		{
			this.m_RenderPass = renderPass;
			this.m_Resources = resources;
			this.m_RenderGraph = renderGraph;
			this.m_Disposed = false;
		}

		private void Dispose(bool disposing)
		{
			if (this.m_Disposed)
			{
				return;
			}
			this.m_RenderGraph.RenderGraphState = RenderGraphState.RecordingGraph;
			this.m_RenderGraph.OnPassAdded(this.m_RenderPass);
			this.m_Disposed = true;
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void CheckResource(in ResourceHandle res, bool checkTransientReadWrite = true)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (!res.IsValid())
				{
					throw new ArgumentException("Trying to use an invalid resource (pass " + this.m_RenderPass.name + ").");
				}
				int renderGraphResourceTransientIndex = this.m_Resources.GetRenderGraphResourceTransientIndex(res);
				if (renderGraphResourceTransientIndex == this.m_RenderPass.index && checkTransientReadWrite)
				{
					Debug.LogError("Trying to read or write a transient resource at pass " + this.m_RenderPass.name + ".Transient resource are always assumed to be both read and written.");
				}
				if (renderGraphResourceTransientIndex != -1 && renderGraphResourceTransientIndex != this.m_RenderPass.index)
				{
					throw new ArgumentException(string.Format("Trying to use a transient texture (pass index {0}) in a different pass (pass index {1}).", renderGraphResourceTransientIndex, this.m_RenderPass.index));
				}
			}
		}

		internal void GenerateDebugData(bool value)
		{
			this.m_RenderPass.GenerateDebugData(value);
		}

		private RenderGraphPass m_RenderPass;

		private RenderGraphResourceRegistry m_Resources;

		private RenderGraph m_RenderGraph;

		private bool m_Disposed;
	}
}
