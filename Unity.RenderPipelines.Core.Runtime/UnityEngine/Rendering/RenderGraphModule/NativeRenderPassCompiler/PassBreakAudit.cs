using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	[DebuggerDisplay("{reason} : {breakPass}")]
	internal readonly struct PassBreakAudit
	{
		public PassBreakAudit(PassBreakReason reason, int breakPass)
		{
			this.reason = reason;
			this.breakPass = breakPass;
		}

		public readonly PassBreakReason reason;

		public readonly int breakPass;

		public static readonly string[] BreakReasonMessages = new string[]
		{
			"The native render pass optimizer never ran on this pass. Pass is standalone and not merged.",
			"The render target sizes of the next pass do not match.",
			"The next pass reads data output by this pass as a regular texture.",
			"The next pass uses a texture sampled in this pass as a render target.",
			"The next pass is not a raster render pass.",
			"The next pass uses a different depth buffer. All passes in the native render pass need to use the same depth buffer.",
			string.Format("The limit of {0} native pass attachments would be exceeded when merging with the next pass.", 8),
			string.Format("The limit of {0} native subpasses would be exceeded when merging with the next pass.", 8),
			"This is the last pass in the graph, there are no other passes to merge.",
			"The next pass uses a different foveated rendering state",
			"The next pass uses a different shading rate image",
			"The next pass uses a different shading rate rendering state",
			"Pass merging is disabled so this pass was not merged",
			"The next pass got merged into this pass."
		};
	}
}
