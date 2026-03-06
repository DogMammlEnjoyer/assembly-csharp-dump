using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\OcclusionCullingCommonShaderVariables.cs", needAccessors = false, generateCBuffer = true)]
	internal struct OcclusionCullingCommonShaderVariables
	{
		internal unsafe OcclusionCullingCommonShaderVariables(in OccluderContext occluderCtx, in InstanceOcclusionTestSubviewSettings subviewSettings, bool occlusionOverlayCountVisible, bool overrideOcclusionTestToAlwaysPass)
		{
			int num = 0;
			OccluderContext occluderContext;
			for (;;)
			{
				int num2 = num;
				occluderContext = occluderCtx;
				if (num2 >= occluderContext.subviewCount)
				{
					break;
				}
				occluderContext = occluderCtx;
				if (occluderContext.IsSubviewValid(num))
				{
					for (int i = 0; i < 16; i++)
					{
						ref float ptr = ref this._ViewProjMatrix.FixedElementField + (IntPtr)(16 * num + i) * 4;
						NativeArray<OccluderDerivedData> subviewData = occluderCtx.subviewData;
						OccluderDerivedData occluderDerivedData = subviewData[num];
						ptr = occluderDerivedData.viewProjMatrix[i];
					}
					for (int j = 0; j < 4; j++)
					{
						ref float ptr2 = ref this._ViewOriginWorldSpace.FixedElementField + (IntPtr)(4 * num + j) * 4;
						NativeArray<OccluderDerivedData> subviewData = occluderCtx.subviewData;
						OccluderDerivedData occluderDerivedData = subviewData[num];
						ptr2 = occluderDerivedData.viewOriginWorldSpace[j];
						ref float ptr3 = ref this._FacingDirWorldSpace.FixedElementField + (IntPtr)(4 * num + j) * 4;
						subviewData = occluderCtx.subviewData;
						occluderDerivedData = subviewData[num];
						ptr3 = occluderDerivedData.facingDirWorldSpace[j];
						ref float ptr4 = ref this._RadialDirWorldSpace.FixedElementField + (IntPtr)(4 * num + j) * 4;
						subviewData = occluderCtx.subviewData;
						occluderDerivedData = subviewData[num];
						ptr4 = occluderDerivedData.radialDirWorldSpace[j];
					}
				}
				num++;
			}
			Vector2Int occluderMipLayoutSize = occluderCtx.occluderMipLayoutSize;
			this._OccluderMipLayoutSizeX = (uint)occluderMipLayoutSize.x;
			occluderMipLayoutSize = occluderCtx.occluderMipLayoutSize;
			this._OccluderMipLayoutSizeY = (uint)occluderMipLayoutSize.y;
			this._OcclusionTestDebugFlags = ((overrideOcclusionTestToAlwaysPass ? 1U : 0U) | (occlusionOverlayCountVisible ? 2U : 0U));
			this._OcclusionCullingCommonPad0 = 0U;
			this._OcclusionTestCount = subviewSettings.testCount;
			this._OccluderSubviewIndices = subviewSettings.occluderSubviewIndices;
			this._CullingSplitIndices = subviewSettings.cullingSplitIndices;
			this._CullingSplitMask = subviewSettings.cullingSplitMask;
			occluderContext = occluderCtx;
			this._DepthSizeInOccluderPixels = occluderContext.depthBufferSizeInOccluderPixels;
			Vector2Int occluderDepthPyramidSize = occluderCtx.occluderDepthPyramidSize;
			this._OccluderDepthPyramidSize = new Vector4((float)occluderDepthPyramidSize.x, (float)occluderDepthPyramidSize.y, 1f / (float)occluderDepthPyramidSize.x, 1f / (float)occluderDepthPyramidSize.y);
			int num3 = 0;
			for (;;)
			{
				int num4 = num3;
				NativeArray<OccluderMipBounds> occluderMipBounds = occluderCtx.occluderMipBounds;
				if (num4 >= occluderMipBounds.Length)
				{
					break;
				}
				occluderMipBounds = occluderCtx.occluderMipBounds;
				OccluderMipBounds occluderMipBounds2 = occluderMipBounds[num3];
				*(ref this._OccluderMipBounds.FixedElementField + (IntPtr)(4 * num3) * 4) = (uint)occluderMipBounds2.offset.x;
				*(ref this._OccluderMipBounds.FixedElementField + (IntPtr)(4 * num3 + 1) * 4) = (uint)occluderMipBounds2.offset.y;
				*(ref this._OccluderMipBounds.FixedElementField + (IntPtr)(4 * num3 + 2) * 4) = (uint)occluderMipBounds2.size.x;
				*(ref this._OccluderMipBounds.FixedElementField + (IntPtr)(4 * num3 + 3) * 4) = (uint)occluderMipBounds2.size.y;
				num3++;
			}
		}

		[FixedBuffer(typeof(uint), 32)]
		[HLSLArray(8, typeof(ShaderGenUInt4))]
		public OcclusionCullingCommonShaderVariables.<_OccluderMipBounds>e__FixedBuffer _OccluderMipBounds;

		[FixedBuffer(typeof(float), 96)]
		[HLSLArray(6, typeof(Matrix4x4))]
		public OcclusionCullingCommonShaderVariables.<_ViewProjMatrix>e__FixedBuffer _ViewProjMatrix;

		[FixedBuffer(typeof(float), 24)]
		[HLSLArray(6, typeof(Vector4))]
		public OcclusionCullingCommonShaderVariables.<_ViewOriginWorldSpace>e__FixedBuffer _ViewOriginWorldSpace;

		[FixedBuffer(typeof(float), 24)]
		[HLSLArray(6, typeof(Vector4))]
		public OcclusionCullingCommonShaderVariables.<_FacingDirWorldSpace>e__FixedBuffer _FacingDirWorldSpace;

		[FixedBuffer(typeof(float), 24)]
		[HLSLArray(6, typeof(Vector4))]
		public OcclusionCullingCommonShaderVariables.<_RadialDirWorldSpace>e__FixedBuffer _RadialDirWorldSpace;

		public Vector4 _DepthSizeInOccluderPixels;

		public Vector4 _OccluderDepthPyramidSize;

		public uint _OccluderMipLayoutSizeX;

		public uint _OccluderMipLayoutSizeY;

		public uint _OcclusionTestDebugFlags;

		public uint _OcclusionCullingCommonPad0;

		public int _OcclusionTestCount;

		public int _OccluderSubviewIndices;

		public int _CullingSplitIndices;

		public int _CullingSplitMask;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 96)]
		public struct <_FacingDirWorldSpace>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 128)]
		public struct <_OccluderMipBounds>e__FixedBuffer
		{
			public uint FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 96)]
		public struct <_RadialDirWorldSpace>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 96)]
		public struct <_ViewOriginWorldSpace>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 384)]
		public struct <_ViewProjMatrix>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
