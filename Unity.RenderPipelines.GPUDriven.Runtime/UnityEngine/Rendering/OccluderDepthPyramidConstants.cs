using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OccluderDepthPyramidConstants.cs", needAccessors = false, generateCBuffer = true)]
	internal struct OccluderDepthPyramidConstants
	{
		[FixedBuffer(typeof(float), 96)]
		[HLSLArray(6, typeof(Matrix4x4))]
		public OccluderDepthPyramidConstants.<_InvViewProjMatrix>e__FixedBuffer _InvViewProjMatrix;

		[FixedBuffer(typeof(float), 24)]
		[HLSLArray(6, typeof(Vector4))]
		public OccluderDepthPyramidConstants.<_SilhouettePlanes>e__FixedBuffer _SilhouettePlanes;

		[FixedBuffer(typeof(uint), 24)]
		[HLSLArray(6, typeof(ShaderGenUInt4))]
		public OccluderDepthPyramidConstants.<_SrcOffset>e__FixedBuffer _SrcOffset;

		[FixedBuffer(typeof(uint), 20)]
		[HLSLArray(5, typeof(ShaderGenUInt4))]
		public OccluderDepthPyramidConstants.<_MipOffsetAndSize>e__FixedBuffer _MipOffsetAndSize;

		public uint _OccluderMipLayoutSizeX;

		public uint _OccluderMipLayoutSizeY;

		public uint _OccluderDepthPyramidPad0;

		public uint _OccluderDepthPyramidPad1;

		public uint _SrcSliceIndices;

		public uint _DstSubviewIndices;

		public uint _MipCount;

		public uint _SilhouettePlaneCount;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 384)]
		public struct <_InvViewProjMatrix>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 80)]
		public struct <_MipOffsetAndSize>e__FixedBuffer
		{
			public uint FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 96)]
		public struct <_SilhouettePlanes>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 96)]
		public struct <_SrcOffset>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
