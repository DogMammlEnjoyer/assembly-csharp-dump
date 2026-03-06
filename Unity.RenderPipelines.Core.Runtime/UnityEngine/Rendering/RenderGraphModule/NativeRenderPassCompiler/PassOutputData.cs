using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	[DebuggerDisplay("PassOutputData: Res({resource.index})")]
	internal readonly struct PassOutputData
	{
		public PassOutputData(ResourceHandle resource)
		{
			this.resource = resource;
		}

		public readonly ResourceHandle resource;
	}
}
