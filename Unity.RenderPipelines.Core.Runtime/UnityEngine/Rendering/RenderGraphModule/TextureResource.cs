using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("TextureResource ({desc.name})")]
	internal class TextureResource : RenderGraphResource<TextureDesc, RTHandle>
	{
		public override string GetName()
		{
			if (!this.imported || this.shared)
			{
				return this.desc.name;
			}
			if (this.graphicsResource == null)
			{
				return "null resource";
			}
			return this.graphicsResource.name;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetDescHashCode()
		{
			return this.desc.GetHashCode();
		}

		public override void CreateGraphicsResource()
		{
			string text = this.GetName();
			if (text == "")
			{
				text = string.Format("RenderGraphTexture_{0}", TextureResource.m_TextureCreationIndex++);
			}
			RTHandleAllocInfo info = new RTHandleAllocInfo(text)
			{
				slices = this.desc.slices,
				format = this.desc.format,
				filterMode = this.desc.filterMode,
				wrapModeU = this.desc.wrapMode,
				wrapModeV = this.desc.wrapMode,
				wrapModeW = this.desc.wrapMode,
				dimension = this.desc.dimension,
				enableRandomWrite = this.desc.enableRandomWrite,
				useMipMap = this.desc.useMipMap,
				autoGenerateMips = this.desc.autoGenerateMips,
				anisoLevel = this.desc.anisoLevel,
				mipMapBias = this.desc.mipMapBias,
				isShadowMap = this.desc.isShadowMap,
				msaaSamples = this.desc.msaaSamples,
				bindTextureMS = this.desc.bindTextureMS,
				useDynamicScale = this.desc.useDynamicScale,
				useDynamicScaleExplicit = this.desc.useDynamicScaleExplicit,
				memoryless = this.desc.memoryless,
				vrUsage = this.desc.vrUsage,
				enableShadingRate = this.desc.enableShadingRate
			};
			switch (this.desc.sizeMode)
			{
			case TextureSizeMode.Explicit:
				this.graphicsResource = RTHandles.Alloc(this.desc.width, this.desc.height, info);
				return;
			case TextureSizeMode.Scale:
				this.graphicsResource = RTHandles.Alloc(this.desc.scale, info);
				return;
			case TextureSizeMode.Functor:
				this.graphicsResource = RTHandles.Alloc(this.desc.func, info);
				return;
			default:
				return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void UpdateGraphicsResource()
		{
			if (this.graphicsResource != null)
			{
				this.graphicsResource.m_Name = this.GetName();
			}
		}

		public override void ReleaseGraphicsResource()
		{
			if (this.graphicsResource != null)
			{
				this.graphicsResource.Release();
			}
			base.ReleaseGraphicsResource();
		}

		public override void LogCreation(RenderGraphLogger logger)
		{
			logger.LogLine(string.Format("Created Texture: {0} (Cleared: {1})", this.desc.name, this.desc.clearBuffer), Array.Empty<object>());
		}

		public override void LogRelease(RenderGraphLogger logger)
		{
			logger.LogLine("Released Texture: " + this.desc.name, Array.Empty<object>());
		}

		private static int m_TextureCreationIndex;
	}
}
