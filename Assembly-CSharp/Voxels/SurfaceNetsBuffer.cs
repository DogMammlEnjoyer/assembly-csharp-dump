using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	public struct SurfaceNetsBuffer : IDisposable
	{
		public SurfaceNetsBuffer(int vertexCap, int indexCap, int strideCount, Allocator alloc = Allocator.TempJob)
		{
			this.Vertices = new NativeList<float3>(vertexCap, alloc);
			this.Normals = new NativeList<float3>(vertexCap, alloc);
			this.Materials = new NativeList<byte>(vertexCap, alloc);
			this.Triangles = new NativeList<int>(indexCap, alloc);
			this.SurfacePoints = new NativeList<int3>(vertexCap, alloc);
			this.SurfaceStrides = new NativeList<int>(vertexCap, alloc);
			this.StrideToIndex = new NativeArray<int>(strideCount, alloc, NativeArrayOptions.UninitializedMemory);
			this.Reset(strideCount);
		}

		public void Reset(int strideCount)
		{
			this.Vertices.Clear();
			this.Normals.Clear();
			this.Triangles.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < strideCount)
			{
				this.StrideToIndex.Dispose();
			}
			if (!this.StrideToIndex.IsCreated || this.StrideToIndex.Length != strideCount)
			{
				this.StrideToIndex = new NativeArray<int>(strideCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			}
			for (int i = 0; i < strideCount; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		public void Dispose()
		{
			this.Vertices.Dispose();
			this.Normals.Dispose();
			this.Materials.Dispose();
			this.Triangles.Dispose();
			this.SurfacePoints.Dispose();
			this.SurfaceStrides.Dispose();
			this.StrideToIndex.Dispose();
		}

		public NativeList<float3> Vertices;

		public NativeList<float3> Normals;

		public NativeList<byte> Materials;

		public NativeList<int> Triangles;

		public NativeList<int3> SurfacePoints;

		public NativeList<int> SurfaceStrides;

		public NativeArray<int> StrideToIndex;

		public const int NullVertex = 2147483647;
	}
}
