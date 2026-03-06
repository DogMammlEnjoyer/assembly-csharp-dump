using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("BufferResource ({desc.name})")]
	internal class BufferResource : RenderGraphResource<BufferDesc, GraphicsBuffer>
	{
		public override string GetName()
		{
			if (this.imported)
			{
				return "ImportedGraphicsBuffer";
			}
			return this.desc.name;
		}

		public override int GetDescHashCode()
		{
			return this.desc.GetHashCode();
		}

		public override void CreateGraphicsResource()
		{
			this.GetName();
			this.graphicsResource = new GraphicsBuffer(this.desc.target, this.desc.usageFlags, this.desc.count, this.desc.stride);
		}

		public override void UpdateGraphicsResource()
		{
			if (this.graphicsResource != null)
			{
				this.graphicsResource.name = this.GetName();
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
			logger.LogLine("Created GraphicsBuffer: " + this.desc.name, Array.Empty<object>());
		}

		public override void LogRelease(RenderGraphLogger logger)
		{
			logger.LogLine("Released GraphicsBuffer: " + this.desc.name, Array.Empty<object>());
		}
	}
}
