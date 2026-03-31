using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	[BurstCompile]
	public struct SurfaceNetsJob : IJob
	{
		public void Execute()
		{
			this.buffer.Reset(this.shape.x * this.shape.y * this.shape.z);
			int x = this.shape.x;
			int num = this.shape.x * this.shape.y;
			for (int i = this.min.z; i < this.max.z; i++)
			{
				for (int j = this.min.y; j < this.max.y; j++)
				{
					int num2 = i * num + j * x + this.min.x;
					int k = this.min.x;
					while (k < this.max.x)
					{
						float3 rhs;
						if (this.EstimateSurfaceInCube(new int3(k, j, i), num2, 1, x, num, out rhs))
						{
							int length = this.buffer.Vertices.Length;
							this.buffer.StrideToIndex[num2] = length;
							int3 @int = new int3(k, j, i);
							this.buffer.SurfacePoints.Add(@int);
							this.buffer.SurfaceStrides.Add(num2);
							float3 @float = new float3((float)k, (float)j, (float)i) + rhs;
							this.buffer.Vertices.Add(@float);
							this.buffer.Normals.Add(float3.zero);
							byte b = this.material[num2];
							this.buffer.Materials.Add(b);
						}
						k++;
						num2++;
					}
				}
			}
			this.MakeAllQuads(1, x, num);
			this.AccumulateNormals();
		}

		private bool EstimateSurfaceInCube(int3 voxel, int cubeMin, int sx, int sy, int sz, out float3 centroid)
		{
			int num = 0;
			NativeArray<float> nativeArray = new NativeArray<float>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < 8; i++)
			{
				int3 @int = SurfaceNetsJob.cubeCorners[i];
				float num2 = this.sdf[cubeMin + @int.x * sx + @int.y * sy + @int.z * sz].ToFloat();
				nativeArray[i] = num2;
				if (num2 < 0f)
				{
					num++;
				}
			}
			if (num == 0 || num == 8)
			{
				centroid = default(float3);
				nativeArray.Dispose();
				return false;
			}
			float3 lhs = float3.zero;
			int num3 = 0;
			for (int j = 0; j < 12; j++)
			{
				int2 int2 = SurfaceNetsJob.cubeEdges[j];
				float num4 = nativeArray[int2.x];
				float num5 = nativeArray[int2.y];
				if (num4 < 0f ^ num5 < 0f)
				{
					num3++;
					lhs += SurfaceNetsJob.EdgeIntersection(int2.x, int2.y, num4, num5);
				}
			}
			centroid = lhs / (float)num3;
			nativeArray.Dispose();
			return true;
		}

		private static float3 EdgeIntersection(int c1, int c2, float v1, float v2)
		{
			float num = v1 / (v1 - v2);
			return SurfaceNetsJob.cornerVecs[c1] * (1f - num) + SurfaceNetsJob.cornerVecs[c2] * num;
		}

		private void MakeAllQuads(int sx, int sy, int sz)
		{
			int3 @int = this.min;
			int3 int2 = this.max;
			for (int i = 0; i < this.buffer.SurfacePoints.Length; i++)
			{
				int3 int3 = this.buffer.SurfacePoints[i];
				int num = this.buffer.SurfaceStrides[i];
				if (int3.y != @int.y && int3.z != @int.z && int3.x != int2.x - 1)
				{
					this.TryQuad(num, num + sx, sy, sz);
				}
				if (int3.x != @int.x && int3.z != @int.z && int3.y != int2.y - 1)
				{
					this.TryQuad(num, num + sy, sz, sx);
				}
				if (int3.x != @int.x && int3.y != @int.y && int3.z != int2.z - 1)
				{
					this.TryQuad(num, num + sz, sx, sy);
				}
			}
		}

		private void TryQuad(int p1, int p2, int strideB, int strideC)
		{
			float num = this.sdf[p1].ToFloat();
			float num2 = this.sdf[p2].ToFloat();
			bool flag = num < 0f && num2 >= 0f;
			if (!flag && (num2 >= 0f || num < 0f))
			{
				return;
			}
			int num3 = this.buffer.StrideToIndex[p1];
			int num4 = this.buffer.StrideToIndex[p1 - strideB];
			int num5 = this.buffer.StrideToIndex[p1 - strideC];
			int num6 = this.buffer.StrideToIndex[p1 - strideB - strideC];
			if ((num3 | num4 | num5 | num6) == 2147483647)
			{
				return;
			}
			float3 lhs = this.buffer.Vertices[num3];
			float3 lhs2 = this.buffer.Vertices[num4];
			float3 rhs = this.buffer.Vertices[num5];
			float3 rhs2 = this.buffer.Vertices[num6];
			if (math.lengthsq(lhs - rhs2) < math.lengthsq(lhs2 - rhs))
			{
				if (flag)
				{
					this.buffer.Triangles.Add(num3);
					this.buffer.Triangles.Add(num6);
					this.buffer.Triangles.Add(num4);
					this.buffer.Triangles.Add(num3);
					this.buffer.Triangles.Add(num5);
					this.buffer.Triangles.Add(num6);
					return;
				}
				this.buffer.Triangles.Add(num3);
				this.buffer.Triangles.Add(num4);
				this.buffer.Triangles.Add(num6);
				this.buffer.Triangles.Add(num3);
				this.buffer.Triangles.Add(num6);
				this.buffer.Triangles.Add(num5);
				return;
			}
			else
			{
				if (flag)
				{
					this.buffer.Triangles.Add(num4);
					this.buffer.Triangles.Add(num5);
					this.buffer.Triangles.Add(num6);
					this.buffer.Triangles.Add(num4);
					this.buffer.Triangles.Add(num3);
					this.buffer.Triangles.Add(num5);
					return;
				}
				this.buffer.Triangles.Add(num4);
				this.buffer.Triangles.Add(num6);
				this.buffer.Triangles.Add(num5);
				this.buffer.Triangles.Add(num4);
				this.buffer.Triangles.Add(num5);
				this.buffer.Triangles.Add(num3);
				return;
			}
		}

		private void AccumulateNormals()
		{
			for (int i = 0; i < this.buffer.Triangles.Length; i += 3)
			{
				int num = this.buffer.Triangles[i];
				int num2 = this.buffer.Triangles[i + 1];
				int num3 = this.buffer.Triangles[i + 2];
				float3 rhs = this.buffer.Vertices[num];
				float3 lhs = this.buffer.Vertices[num2];
				float3 lhs2 = this.buffer.Vertices[num3];
				float3 rhs2 = math.cross(lhs - rhs, lhs2 - rhs);
				ref NativeList<float3> ptr = ref this.buffer.Normals;
				int index = num;
				ptr[index] += rhs2;
				ptr = ref this.buffer.Normals;
				index = num2;
				ptr[index] += rhs2;
				ptr = ref this.buffer.Normals;
				index = num3;
				ptr[index] += rhs2;
			}
		}

		[ReadOnly]
		public NativeArray<byte> sdf;

		[ReadOnly]
		public NativeArray<byte> material;

		public int3 shape;

		public int3 min;

		public int3 max;

		public byte isoLevel;

		public SurfaceNetsBuffer buffer;

		private static readonly int3[] cubeCorners = new int3[]
		{
			new int3(0, 0, 0),
			new int3(1, 0, 0),
			new int3(0, 1, 0),
			new int3(1, 1, 0),
			new int3(0, 0, 1),
			new int3(1, 0, 1),
			new int3(0, 1, 1),
			new int3(1, 1, 1)
		};

		private static readonly float3[] cornerVecs = new float3[]
		{
			new float3(0f, 0f, 0f),
			new float3(1f, 0f, 0f),
			new float3(0f, 1f, 0f),
			new float3(1f, 1f, 0f),
			new float3(0f, 0f, 1f),
			new float3(1f, 0f, 1f),
			new float3(0f, 1f, 1f),
			new float3(1f, 1f, 1f)
		};

		private static readonly int2[] cubeEdges = new int2[]
		{
			new int2(0, 1),
			new int2(0, 2),
			new int2(0, 4),
			new int2(1, 3),
			new int2(1, 5),
			new int2(2, 3),
			new int2(2, 6),
			new int2(3, 7),
			new int2(4, 5),
			new int2(4, 6),
			new int2(5, 7),
			new int2(6, 7)
		};
	}
}
