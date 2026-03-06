using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	[DebuggerDisplay("PassRandomWriteData: Res({resource.index}):{index}:{preserveCounterValue}")]
	internal readonly struct PassRandomWriteData
	{
		public PassRandomWriteData(ResourceHandle resource, int index, bool preserveCounterValue)
		{
			this.resource = resource;
			this.index = index;
			this.preserveCounterValue = preserveCounterValue;
		}

		public override int GetHashCode()
		{
			return this.resource.GetHashCode() * 23 + this.index.GetHashCode();
		}

		public readonly ResourceHandle resource;

		public readonly int index;

		public readonly bool preserveCounterValue;
	}
}
