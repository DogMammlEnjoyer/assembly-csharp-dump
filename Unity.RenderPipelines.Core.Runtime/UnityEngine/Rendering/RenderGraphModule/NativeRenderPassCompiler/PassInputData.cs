using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	[DebuggerDisplay("PassInputData: Res({resource.index})")]
	internal readonly struct PassInputData
	{
		public PassInputData(ResourceHandle resource)
		{
			this.resource = resource;
		}

		public readonly ResourceHandle resource;
	}
}
