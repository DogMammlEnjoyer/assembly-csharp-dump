using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal struct IndirectBufferContextHandles
	{
		public void UseForOcclusionTest(IBaseRenderGraphBuilder builder)
		{
			this.instanceBuffer = builder.UseBuffer(this.instanceBuffer, AccessFlags.ReadWrite);
			this.instanceInfoBuffer = builder.UseBuffer(this.instanceInfoBuffer, AccessFlags.Read);
			this.argsBuffer = builder.UseBuffer(this.argsBuffer, AccessFlags.ReadWrite);
			this.drawInfoBuffer = builder.UseBuffer(this.drawInfoBuffer, AccessFlags.Read);
		}

		public BufferHandle instanceBuffer;

		public BufferHandle instanceInfoBuffer;

		public BufferHandle argsBuffer;

		public BufferHandle drawInfoBuffer;
	}
}
