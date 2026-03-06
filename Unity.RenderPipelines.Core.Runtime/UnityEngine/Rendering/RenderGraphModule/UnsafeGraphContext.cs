using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public class UnsafeGraphContext : IDerivedRendergraphContext
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
			UnsafeGraphContext.unsCmd.m_WrappedCommandBuffer = this.wrappedContext.cmd;
			UnsafeGraphContext.unsCmd.m_ExecutingPass = context.executingPass;
			this.cmd = UnsafeGraphContext.unsCmd;
		}

		private InternalRenderGraphContext wrappedContext;

		public UnsafeCommandBuffer cmd;

		internal static UnsafeCommandBuffer unsCmd = new UnsafeCommandBuffer(null, null, false);
	}
}
