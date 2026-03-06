using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	internal struct LODGroupCullingData
	{
		public float3 worldSpaceReferencePoint;

		public int lodCount;

		[FixedBuffer(typeof(float), 8)]
		public LODGroupCullingData.<sqrDistances>e__FixedBuffer sqrDistances;

		[FixedBuffer(typeof(float), 8)]
		public LODGroupCullingData.<transitionDistances>e__FixedBuffer transitionDistances;

		public float worldSpaceSize;

		[FixedBuffer(typeof(bool), 8)]
		public LODGroupCullingData.<percentageFlags>e__FixedBuffer percentageFlags;

		public byte forceLODMask;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <percentageFlags>e__FixedBuffer
		{
			public bool FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <sqrDistances>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <transitionDistances>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
