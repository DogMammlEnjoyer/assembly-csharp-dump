using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public struct RasterGraphContext : IDerivedRendergraphContext
	{
		public RenderGraphDefaultResources defaultResources
		{
			get
			{
				return this.wrappedContext.defaultResources;
			}
		}

		public RenderGraphObjectPool renderGraphPool
		{
			get
			{
				return this.wrappedContext.renderGraphPool;
			}
		}

		public void FromInternalContext(InternalRenderGraphContext context)
		{
			this.wrappedContext = context;
			RasterGraphContext.rastercmd.m_WrappedCommandBuffer = this.wrappedContext.cmd;
			RasterGraphContext.rastercmd.m_ExecutingPass = context.executingPass;
			this.cmd = RasterGraphContext.rastercmd;
		}

		private InternalRenderGraphContext wrappedContext;

		public RasterCommandBuffer cmd;

		internal static RasterCommandBuffer rastercmd = new RasterCommandBuffer(null, null, false);
	}
}
