using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
	internal sealed class UnsafeRenderGraphPass<PassData> : BaseRenderGraphPass<PassData, UnsafeGraphContext> where PassData : class, new()
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Execute(InternalRenderGraphContext renderGraphContext)
		{
			UnsafeRenderGraphPass<PassData>.c.FromInternalContext(renderGraphContext);
			this.renderFunc(this.data, UnsafeRenderGraphPass<PassData>.c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Release(RenderGraphObjectPool pool)
		{
			base.Release(pool);
			pool.Release<UnsafeRenderGraphPass<PassData>>(this);
		}

		internal static UnsafeGraphContext c = new UnsafeGraphContext();
	}
}
