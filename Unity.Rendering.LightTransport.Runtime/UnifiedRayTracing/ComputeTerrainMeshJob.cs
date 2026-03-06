using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	[BurstCompile]
	internal struct ComputeTerrainMeshJob : IJobParallelFor
	{
		public void DisposeArrays()
		{
			this.heightmap.Dispose();
			this.holes.Dispose();
			this.positions.Dispose();
			this.uvs.Dispose();
			this.normals.Dispose();
			this.indices.Dispose();
		}

		public void Execute(int index)
		{
			int num = index % this.width;
			int num2 = index / this.height;
			float3 lhs = new float3((float)num, this.heightmap[num2 * this.width + num], (float)num2);
			this.positions[index] = lhs * this.heightmapScale;
			this.uvs[index] = lhs.xz / new float2((float)this.width, (float)this.height);
			this.normals[index] = ComputeTerrainMeshJob.CalculateTerrainNormal(this.heightmap, num, num2, this.width, this.height, this.heightmapScale);
			if (num < this.width - 1 && num2 < this.height - 1)
			{
				int num3 = num2 * this.width + num;
				int value = num3 + 1;
				int num4 = num3 + this.width;
				int value2 = num4 + 1;
				int num5 = num + num2 * (this.width - 1);
				if (!this.holes[num5])
				{
					value = (num3 = (num4 = (value2 = 0)));
				}
				this.indices[6 * num5] = num3;
				this.indices[6 * num5 + 1] = value2;
				this.indices[6 * num5 + 2] = value;
				this.indices[6 * num5 + 3] = num3;
				this.indices[6 * num5 + 4] = num4;
				this.indices[6 * num5 + 5] = value2;
			}
		}

		private static float3 CalculateTerrainNormal(NativeArray<float> heightmap, int x, int y, int width, int height, float3 scale)
		{
			float num = (ComputeTerrainMeshJob.SampleHeight(x - 1, y - 1, width, height, heightmap, scale.y) * -1f + ComputeTerrainMeshJob.SampleHeight(x - 1, y, width, height, heightmap, scale.y) * -2f + ComputeTerrainMeshJob.SampleHeight(x - 1, y + 1, width, height, heightmap, scale.y) * -1f + ComputeTerrainMeshJob.SampleHeight(x + 1, y - 1, width, height, heightmap, scale.y) * 1f + ComputeTerrainMeshJob.SampleHeight(x + 1, y, width, height, heightmap, scale.y) * 2f + ComputeTerrainMeshJob.SampleHeight(x + 1, y + 1, width, height, heightmap, scale.y) * 1f) / scale.x;
			float num2 = ComputeTerrainMeshJob.SampleHeight(x - 1, y - 1, width, height, heightmap, scale.y) * -1f;
			num2 += ComputeTerrainMeshJob.SampleHeight(x, y - 1, width, height, heightmap, scale.y) * -2f;
			num2 += ComputeTerrainMeshJob.SampleHeight(x + 1, y - 1, width, height, heightmap, scale.y) * -1f;
			num2 += ComputeTerrainMeshJob.SampleHeight(x - 1, y + 1, width, height, heightmap, scale.y) * 1f;
			num2 += ComputeTerrainMeshJob.SampleHeight(x, y + 1, width, height, heightmap, scale.y) * 2f;
			num2 += ComputeTerrainMeshJob.SampleHeight(x + 1, y + 1, width, height, heightmap, scale.y) * 1f;
			num2 /= scale.z;
			return math.normalize(new float3(-num, 8f, -num2));
		}

		private static float SampleHeight(int x, int y, int width, int height, NativeArray<float> heightmap, float scale)
		{
			x = math.clamp(x, 0, width - 1);
			y = math.clamp(y, 0, height - 1);
			return heightmap[x + y * width] * scale;
		}

		[ReadOnly]
		public NativeArray<float> heightmap;

		[ReadOnly]
		public NativeArray<bool> holes;

		public int width;

		public int height;

		public float3 heightmapScale;

		public NativeArray<float3> positions;

		public NativeArray<float2> uvs;

		public NativeArray<float3> normals;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> indices;
	}
}
