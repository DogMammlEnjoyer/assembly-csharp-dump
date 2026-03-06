using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
	internal sealed class ComputeRenderGraphPass<PassData> : BaseRenderGraphPass<PassData, ComputeGraphContext> where PassData : class, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Execute(InternalRenderGraphContext renderGraphContext)
		{
			ComputeRenderGraphPass<PassData>.c.FromInternalContext(renderGraphContext);
			this.renderFunc(this.data, ComputeRenderGraphPass<PassData>.c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Release(RenderGraphObjectPool pool)
		{
			base.Release(pool);
			pool.Release<ComputeRenderGraphPass<PassData>>(this);
		}

		internal static ComputeGraphContext c = new ComputeGraphContext();
	}
}
