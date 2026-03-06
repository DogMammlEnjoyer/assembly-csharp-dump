using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingDebugShaderVariables.cs", needAccessors = false, generateCBuffer = true)]
	internal struct OcclusionCullingDebugShaderVariables
	{
		public Vector4 _DepthSizeInOccluderPixels;

		[FixedBuffer(typeof(uint), 32)]
		[HLSLArray(8, typeof(ShaderGenUInt4))]
		public OcclusionCullingDebugShaderVariables.<_OccluderMipBounds>e__FixedBuffer _OccluderMipBounds;

		public uint _OccluderMipLayoutSizeX;

		public uint _OccluderMipLayoutSizeY;

		public uint _OcclusionCullingDebugPad0;

		public uint _OcclusionCullingDebugPad1;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <_OccluderMipBounds>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
