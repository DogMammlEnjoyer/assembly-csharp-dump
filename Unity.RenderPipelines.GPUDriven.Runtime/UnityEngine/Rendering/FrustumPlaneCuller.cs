using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	internal struct FrustumPlaneCuller
	{
		internal void Dispose(JobHandle job)
		{
			this.planePackets.Dispose(job);
			this.splitInfos.Dispose(job);
		}

		internal static FrustumPlaneCuller Create(in BatchCullingContext cc, NativeArray<Plane> receiverPlanes, in ReceiverSphereCuller receiverSphereCuller, Allocator allocator)
		{
			int length = cc.cullingSplits.Length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				int num2 = receiverPlanes.Length + cc.cullingSplits[i].cullingPlaneCount;
				num += (num2 + 3) / 4;
			}
			FrustumPlaneCuller result = new FrustumPlaneCuller
			{
				planePackets = new NativeList<FrustumPlaneCuller.PlanePacket4>(num, allocator),
				splitInfos = new NativeList<FrustumPlaneCuller.SplitInfo>(length, allocator)
			};
			result.planePackets.ResizeUninitialized(num);
			result.splitInfos.ResizeUninitialized(length);
			NativeList<Plane> nativeList = new NativeList<Plane>(Allocator.Temp);
			int num3 = 0;
			for (int j = 0; j < length; j++)
			{
				CullingSplit cullingSplit = cc.cullingSplits[j];
				nativeList.Clear();
				for (int k = 0; k < cullingSplit.cullingPlaneCount; k++)
				{
					Plane plane = cc.cullingPlanes[cullingSplit.cullingPlaneOffset + k];
					nativeList.Add(plane);
				}
				ReceiverSphereCuller receiverSphereCuller2 = receiverSphereCuller;
				if (receiverSphereCuller2.UseReceiverPlanes())
				{
					nativeList.AddRange(receiverPlanes);
				}
				int num4 = (nativeList.Length + 3) / 4;
				result.splitInfos[j] = new FrustumPlaneCuller.SplitInfo
				{
					packetCount = num4
				};
				for (int l = 0; l < num4; l++)
				{
					result.planePackets[num3 + l] = new FrustumPlaneCuller.PlanePacket4(nativeList.AsArray(), 4 * l, nativeList.Length - 1);
				}
				num3 += num4;
			}
			nativeList.Dispose();
			return result;
		}

		internal static uint ComputeSplitVisibilityMask(NativeArray<FrustumPlaneCuller.PlanePacket4> planePackets, NativeArray<FrustumPlaneCuller.SplitInfo> splitInfos, in AABB bounds)
		{
			float3 @float = bounds.center;
			float4 xxxx = @float.xxxx;
			@float = bounds.center;
			float4 yyyy = @float.yyyy;
			@float = bounds.center;
			float4 zzzz = @float.zzzz;
			@float = bounds.extents;
			float4 xxxx2 = @float.xxxx;
			@float = bounds.extents;
			float4 yyyy2 = @float.yyyy;
			@float = bounds.extents;
			float4 zzzz2 = @float.zzzz;
			uint num = 0U;
			int num2 = 0;
			int length = splitInfos.Length;
			for (int i = 0; i < length; i++)
			{
				FrustumPlaneCuller.SplitInfo splitInfo = splitInfos[i];
				bool4 @bool = new bool4(false);
				for (int j = 0; j < splitInfo.packetCount; j++)
				{
					FrustumPlaneCuller.PlanePacket4 planePacket = planePackets[num2 + j];
					float4 lhs = planePacket.nx * xxxx + planePacket.ny * yyyy + planePacket.nz * zzzz + planePacket.d;
					float4 rhs = planePacket.nxAbs * xxxx2 + planePacket.nyAbs * yyyy2 + planePacket.nzAbs * zzzz2;
					@bool |= (lhs + rhs < float4.zero);
				}
				if (!math.any(@bool))
				{
					num |= 1U << i;
				}
				num2 += splitInfo.packetCount;
			}
			return num;
		}

		public NativeList<FrustumPlaneCuller.PlanePacket4> planePackets;

		public NativeList<FrustumPlaneCuller.SplitInfo> splitInfos;

		internal struct PlanePacket4
		{
			public PlanePacket4(NativeArray<Plane> planes, int offset, int limit)
			{
				Plane plane = planes[Mathf.Min(offset, limit)];
				Plane plane2 = planes[Mathf.Min(offset + 1, limit)];
				Plane plane3 = planes[Mathf.Min(offset + 2, limit)];
				Plane plane4 = planes[Mathf.Min(offset + 3, limit)];
				this.nx = new float4(plane.normal.x, plane2.normal.x, plane3.normal.x, plane4.normal.x);
				this.ny = new float4(plane.normal.y, plane2.normal.y, plane3.normal.y, plane4.normal.y);
				this.nz = new float4(plane.normal.z, plane2.normal.z, plane3.normal.z, plane4.normal.z);
				this.d = new float4(plane.distance, plane2.distance, plane3.distance, plane4.distance);
				this.nxAbs = math.abs(this.nx);
				this.nyAbs = math.abs(this.ny);
				this.nzAbs = math.abs(this.nz);
			}

			public float4 nx;

			public float4 ny;

			public float4 nz;

			public float4 d;

			public float4 nxAbs;

			public float4 nyAbs;

			public float4 nzAbs;
		}

		internal struct SplitInfo
		{
			public int packetCount;
		}
	}
}
