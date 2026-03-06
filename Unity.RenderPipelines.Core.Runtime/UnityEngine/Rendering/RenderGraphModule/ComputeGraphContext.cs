using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public class ComputeGraphContext : IDerivedRendergraphContext
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
			ComputeGraphContext.computecmd.m_WrappedCommandBuffer = this.wrappedContext.cmd;
			ComputeGraphContext.computecmd.m_ExecutingPass = context.executingPass;
			this.cmd = ComputeGraphContext.computecmd;
		}

		private InternalRenderGraphContext wrappedContext;

		public ComputeCommandBuffer cmd;

		internal static ComputeCommandBuffer computecmd = new ComputeCommandBuffer(null, null, false);
	}
}
