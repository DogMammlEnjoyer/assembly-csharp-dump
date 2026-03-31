using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	[BurstCompile]
	public struct GenerateVoxelDataJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			int num = index % this.dimension;
			int num2 = index / this.dimension % this.dimension;
			int num3 = index / (this.dimension * this.dimension);
			float3 @float = new float3((float)(this.chunkPosition.x * this.chunkSize + num), (float)(this.chunkPosition.y * this.chunkSize + num2), (float)(this.chunkPosition.z * this.chunkSize + num3));
			float3 rhs = new float3((float)this.seed * 1.7f, (float)this.seed * 2.3f, (float)this.seed * 3.1f);
			float3 lhs = @float + rhs;
			float num4 = noise.snoise((new float3(@float.x, 0f, @float.z) + rhs) * this.noiseScale) + (this.groundLevel - @float.y) / this.heightScale;
			num4 = math.clamp(num4 * this.heightCompensation, -1f, 1f);
			float num5 = this.noiseScale;
			float num6 = 1f;
			for (int i = 0; i < this.octaves; i++)
			{
				num5 *= 2f;
				num6 *= this.persistence;
				num4 += noise.snoise(lhs * num5) * num6;
			}
			if (noise.snoise(lhs * 0.05f) > 0.6f && num4 >= 0f)
			{
				this.materials[index] = 1;
			}
			this.voxels[index] = num4.ToByte();
		}

		public int3 chunkPosition;

		public int chunkSize;

		public int dimension;

		public float noiseScale;

		public float groundLevel;

		public float heightScale;

		public float heightCompensation;

		public int octaves;

		public float persistence;

		public int seed;

		[WriteOnly]
		public NativeArray<byte> voxels;

		[WriteOnly]
		public NativeArray<byte> materials;
	}
}
