using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	internal struct ReceiverSphereCuller
	{
		internal static ReceiverSphereCuller CreateEmptyForTesting(Allocator allocator)
		{
			return new ReceiverSphereCuller
			{
				splitInfos = new NativeList<ReceiverSphereCuller.SplitInfo>(0, allocator),
				worldToLightSpaceRotation = float3x3.identity
			};
		}

		internal void Dispose(JobHandle job)
		{
			this.splitInfos.Dispose(job);
		}

		internal bool UseReceiverPlanes()
		{
			return this.splitInfos.Length == 0;
		}

		internal static ReceiverSphereCuller Create(in BatchCullingContext cc, Allocator allocator)
		{
			int num = cc.cullingSplits.Length;
			bool flag = num > 1;
			for (int i = 0; i < num; i++)
			{
				if (cc.cullingSplits[i].sphereRadius <= 0f)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				num = 0;
			}
			float3x3 v = (float3x3)cc.localToWorldMatrix;
			ReceiverSphereCuller receiverSphereCuller = new ReceiverSphereCuller
			{
				splitInfos = new NativeList<ReceiverSphereCuller.SplitInfo>(num, allocator),
				worldToLightSpaceRotation = math.transpose(v)
			};
			receiverSphereCuller.splitInfos.ResizeUninitialized(num);
			for (int j = 0; j < num; j++)
			{
				CullingSplit cullingSplit = cc.cullingSplits[j];
				float4 receiverSphereLightSpace = new float4(math.mul(receiverSphereCuller.worldToLightSpaceRotation, cullingSplit.sphereCenter), cullingSplit.sphereRadius);
				receiverSphereCuller.splitInfos[j] = new ReceiverSphereCuller.SplitInfo
				{
					receiverSphereLightSpace = receiverSphereLightSpace,
					cascadeBlendCullingFactor = cullingSplit.cascadeBlendCullingFactor
				};
			}
			return receiverSphereCuller;
		}

		internal static float DistanceUntilCylinderFullyCrossesPlane(float3 cylinderCenter, float3 cylinderDirection, float cylinderRadius, Plane plane)
		{
			float y = 0.001f;
			float num = math.max(math.abs(math.dot(plane.normal, cylinderDirection)), y);
			float num2 = (math.dot(plane.normal, cylinderCenter) + plane.distance) / num;
			float num3 = math.sqrt(math.max(1f - num * num, 0f));
			float num4 = cylinderRadius * num3 / num;
			return num2 + num4;
		}

		internal static uint ComputeSplitVisibilityMask(NativeArray<Plane> lightFacingFrustumPlanes, NativeArray<ReceiverSphereCuller.SplitInfo> splitInfos, float3x3 worldToLightSpaceRotation, in AABB bounds)
		{
			float3 center = bounds.center;
			float3 lhs = math.mul(worldToLightSpaceRotation, bounds.center);
			float num = math.length(bounds.extents);
			float3 c = math.transpose(worldToLightSpaceRotation).c2;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < lightFacingFrustumPlanes.Length; i++)
			{
				num2 = math.min(num2, ReceiverSphereCuller.DistanceUntilCylinderFullyCrossesPlane(center, c, num, lightFacingFrustumPlanes[i]));
			}
			num2 = math.max(num2, 0f);
			uint num3 = 0U;
			int length = splitInfos.Length;
			for (int j = 0; j < length; j++)
			{
				ReceiverSphereCuller.SplitInfo splitInfo = splitInfos[j];
				float3 xyz = splitInfo.receiverSphereLightSpace.xyz;
				float w = splitInfo.receiverSphereLightSpace.w;
				float3 @float = lhs - xyz;
				float num4 = math.lengthsq(num + w) - math.lengthsq(@float.xy);
				if (num4 >= 0f && (@float.z <= 0f || math.lengthsq(@float.z) <= num4))
				{
					num3 |= 1U << j;
					float num5 = w * splitInfo.cascadeBlendCullingFactor;
					float3 x = @float + new float3(0f, 0f, num2);
					float num6 = num5 - num;
					float num7 = math.max(math.lengthsq(@float), math.lengthsq(x));
					if (num6 > 0f && num7 < math.lengthsq(num6))
					{
						break;
					}
				}
			}
			return num3;
		}

		public NativeList<ReceiverSphereCuller.SplitInfo> splitInfos;

		public float3x3 worldToLightSpaceRotation;

		internal struct SplitInfo
		{
			public float4 receiverSphereLightSpace;

			public float cascadeBlendCullingFactor;
		}
	}
}
