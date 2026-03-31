using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	public struct AssembleVertexDataJob : IJob
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int3 Sort3(int a, int b, int c)
		{
			if (a > b)
			{
				int num = a;
				a = b;
				b = num;
			}
			if (b > c)
			{
				int num2 = b;
				b = c;
				c = num2;
			}
			if (a > b)
			{
				int num3 = a;
				a = b;
				b = num3;
			}
			return new int3(a, b, c);
		}

		private static int4 MakeMatSet(byte m0, byte m1, byte m2)
		{
			return new int4((int)m0, (int)m1, (int)m2, 255);
		}

		public void Execute()
		{
			NativeParallelHashMap<AssembleVertexDataJob.Key, int> nativeParallelHashMap = new NativeParallelHashMap<AssembleVertexDataJob.Key, int>(this.srcVerts.Length * 2, Allocator.Temp);
			NativeList<MeshVertexData> nativeList = new NativeList<MeshVertexData>(this.srcVerts.Length * 2, Allocator.Temp);
			NativeList<ushort> nativeList2 = new NativeList<ushort>(this.srcTris.Length, Allocator.Temp);
			int num = this.srcTris.Length / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = this.srcTris[i * 3];
				int num3 = this.srcTris[i * 3 + 1];
				int num4 = this.srcTris[i * 3 + 2];
				int4 mats = AssembleVertexDataJob.MakeMatSet(this.srcMats[num2], this.srcMats[num3], this.srcMats[num4]);
				ushort orCreate = this.GetOrCreate(num2, mats, new float4(1f, 0f, 0f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(orCreate);
				orCreate = this.GetOrCreate(num3, mats, new float4(0f, 1f, 0f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(orCreate);
				orCreate = this.GetOrCreate(num4, mats, new float4(0f, 0f, 1f, 0f), ref nativeParallelHashMap, ref nativeList);
				nativeList2.Add(orCreate);
			}
			this.triangleCounter.Count = num;
			int length = nativeList.Length;
			int length2 = nativeList2.Length;
			NativeArray<MeshVertexData>.Copy(nativeList.AsArray(), this.vertexData, length);
			NativeArray<ushort>.Copy(nativeList2.AsArray(), this.triangleData, length2);
		}

		private ushort GetOrCreate(int srcIdx, int4 mats, float4 blend, ref NativeParallelHashMap<AssembleVertexDataJob.Key, int> map, ref NativeList<MeshVertexData> vertsOut)
		{
			AssembleVertexDataJob.Key key = new AssembleVertexDataJob.Key
			{
				srcIdx = srcIdx,
				mats = mats
			};
			int length;
			if (!map.TryGetValue(key, out length))
			{
				length = vertsOut.Length;
				map.Add(key, length);
				float3 @float = this.srcNorm[srcIdx];
				float3 xyz = math.normalize(math.cross((math.abs(@float.y) < 0.999f) ? new float3(0f, 1f, 0f) : new float3(1f, 0f, 0f), @float));
				MeshVertexData meshVertexData = new MeshVertexData(this.srcVerts[srcIdx], @float, new float4(xyz, 1f), mats, blend);
				vertsOut.Add(meshVertexData);
			}
			return (ushort)length;
		}

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<MeshVertexData> vertexData;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<ushort> triangleData;

		[ReadOnly]
		public NativeArray<float3> srcVerts;

		[ReadOnly]
		public NativeArray<byte> srcMats;

		[ReadOnly]
		public NativeArray<float3> srcNorm;

		[ReadOnly]
		public NativeArray<int> srcTris;

		public NativeCounter triangleCounter;

		private struct Key : IEquatable<AssembleVertexDataJob.Key>
		{
			public bool Equals(AssembleVertexDataJob.Key other)
			{
				return this.srcIdx == other.srcIdx && this.mats.Equals(other.mats);
			}

			public override int GetHashCode()
			{
				return (int)math.hash(new int4(this.srcIdx, this.mats.xyz));
			}

			public int srcIdx;

			public int4 mats;
		}
	}
}
