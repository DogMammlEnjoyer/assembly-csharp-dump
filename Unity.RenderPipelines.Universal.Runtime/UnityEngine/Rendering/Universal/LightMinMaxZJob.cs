using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	[BurstCompile]
	internal struct LightMinMaxZJob : IJobFor
	{
		public void Execute(int index)
		{
			int index2 = index % this.lights.Length;
			VisibleLight visibleLight = this.lights[index2];
			float4x4 float4x = visibleLight.localToWorldMatrix;
			float3 xyz = float4x.c3.xyz;
			int index3 = index / this.lights.Length;
			float4x4 a = this.worldToViews[index3];
			float3 xyz2 = math.mul(a, math.float4(xyz, 1f)).xyz;
			xyz2.z *= -1f;
			float2 @float = math.float2(xyz2.z - visibleLight.range, xyz2.z + visibleLight.range);
			if (visibleLight.lightType == LightType.Spot)
			{
				float num = math.radians(visibleLight.spotAngle) * 0.5f;
				float num2 = math.cos(num);
				float num3 = visibleLight.range * num2;
				float3 xyz3 = float4x.c2.xyz;
				float3 xyz4 = xyz + xyz3 * num3;
				float3 xyz5 = math.mul(a, math.float4(xyz4, 1f)).xyz;
				xyz5.z *= -1f;
				float x = 1.5707964f - num;
				float num4 = visibleLight.range * num2 * math.sin(num) / math.sin(x);
				float3 float2 = xyz5 - xyz2;
				float num5 = math.sqrt(1f - float2.z * float2.z / math.dot(float2, float2));
				if (-float2.z < num3 * num2)
				{
					@float.x = math.min(xyz2.z, xyz5.z - num5 * num4);
				}
				if (float2.z < num3 * num2)
				{
					@float.y = math.max(xyz2.z, xyz5.z + num5 * num4);
				}
			}
			else if (visibleLight.lightType != LightType.Point)
			{
				@float.x = float.MaxValue;
				@float.y = float.MinValue;
			}
			@float.x = math.max(@float.x, 0f);
			@float.y = math.max(@float.y, 0f);
			this.minMaxZs[index] = @float;
		}

		public Fixed2<float4x4> worldToViews;

		[ReadOnly]
		public NativeArray<VisibleLight> lights;

		public NativeArray<float2> minMaxZs;
	}
}
