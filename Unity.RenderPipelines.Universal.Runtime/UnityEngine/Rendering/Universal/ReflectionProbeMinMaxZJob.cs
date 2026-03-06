using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile]
	internal struct ReflectionProbeMinMaxZJob : IJobFor
	{
		public void Execute(int index)
		{
			float2 @float = math.float2(float.MaxValue, float.MinValue);
			int index2 = index % this.reflectionProbes.Length;
			VisibleReflectionProbe visibleReflectionProbe = this.reflectionProbes[index2];
			int index3 = index / this.reflectionProbes.Length;
			float4x4 a = this.worldToViews[index3];
			float3 lhs = visibleReflectionProbe.bounds.center;
			float3 lhs2 = visibleReflectionProbe.bounds.extents;
			for (int i = 0; i < 8; i++)
			{
				int num = (i << 1 & 2) - 1;
				int num2 = (i & 2) - 1;
				int num3 = (i >> 1 & 2) - 1;
				float4 float2 = math.mul(a, math.float4(lhs + lhs2 * math.float3((float)num, (float)num2, (float)num3), 1f));
				float2.z *= -1f;
				@float.x = math.min(@float.x, float2.z);
				@float.y = math.max(@float.y, float2.z);
			}
			this.minMaxZs[index] = @float;
		}

		public Fixed2<float4x4> worldToViews;

		[ReadOnly]
		public NativeArray<VisibleReflectionProbe> reflectionProbes;

		public NativeArray<float2> minMaxZs;
	}
}
