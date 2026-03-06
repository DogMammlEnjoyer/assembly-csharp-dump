using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	internal struct URPLightShadowCullingInfos
	{
		public readonly bool IsSliceValid(int i)
		{
			return ((ulong)this.slicesValidMask & (ulong)(1L << (i & 31))) > 0UL;
		}

		public NativeArray<ShadowSliceData> slices;

		public uint slicesValidMask;
	}
}
