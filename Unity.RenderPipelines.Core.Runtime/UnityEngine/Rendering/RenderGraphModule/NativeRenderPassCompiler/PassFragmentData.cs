using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	[DebuggerDisplay("PassFragmentData: Res({resource.index}):{accessFlags}")]
	internal readonly struct PassFragmentData
	{
		public PassFragmentData(ResourceHandle handle, AccessFlags flags, int mipLevel, int depthSlice)
		{
			this.resource = handle;
			this.accessFlags = flags;
			this.mipLevel = mipLevel;
			this.depthSlice = depthSlice;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			return ((this.resource.GetHashCode() * 23 + this.accessFlags.GetHashCode()) * 23 + this.mipLevel.GetHashCode()) * 23 + this.depthSlice.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SameSubResource(in PassFragmentData x, in PassFragmentData y)
		{
			return x.resource.index == y.resource.index && x.mipLevel == y.mipLevel && x.depthSlice == y.depthSlice;
		}

		public readonly ResourceHandle resource;

		public readonly AccessFlags accessFlags;

		public readonly int mipLevel;

		public readonly int depthSlice;
	}
}
