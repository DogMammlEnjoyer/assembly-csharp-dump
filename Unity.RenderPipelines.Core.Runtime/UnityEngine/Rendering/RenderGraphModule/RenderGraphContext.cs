using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public struct RenderGraphContext : IDerivedRendergraphContext
	{
		public void FromInternalContext(InternalRenderGraphContext context)
		{
			this.wrappedContext = context;
		}

		public ScriptableRenderContext renderContext
		{
			get
			{
				return this.wrappedContext.renderContext;
			}
		}

		public CommandBuffer cmd
		{
			get
			{
				return this.wrappedContext.cmd;
			}
		}

		public RenderGraphObjectPool renderGraphPool
		{
			get
			{
				return this.wrappedContext.renderGraphPool;
			}
		}

		public RenderGraphDefaultResources defaultResources
		{
			get
			{
				return this.wrappedContext.defaultResources;
			}
		}

		private InternalRenderGraphContext wrappedContext;
	}
}
