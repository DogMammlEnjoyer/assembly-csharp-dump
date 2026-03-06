using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering
{
	internal struct LODGroupData
	{
		public const int k_MaxLODLevelsCount = 8;

		public bool valid;

		public int lodCount;

		public int rendererCount;

		[FixedBuffer(typeof(float), 8)]
		public LODGroupData.<screenRelativeTransitionHeights>e__FixedBuffer screenRelativeTransitionHeights;

		[FixedBuffer(typeof(float), 8)]
		public LODGroupData.<fadeTransitionWidth>e__FixedBuffer fadeTransitionWidth;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <fadeTransitionWidth>e__FixedBuffer
		{
			public float FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 32)]
		public struct <screenRelativeTransitionHeights>e__FixedBuffer
		{
			public float FixedElementField;
		}
	}
}
