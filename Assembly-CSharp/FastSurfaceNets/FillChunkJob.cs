using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Voxels;

namespace FastSurfaceNets
{
	internal struct FillChunkJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			int z = index / this.strideZ;
			int y = index % this.strideZ / this.strideY;
			int x = index % this.strideY;
			float3 @float = (this.chunkPosition + new int3(x, y, z)).ToFloat3();
			float value = noise.snoise(@float * this.noiseScale) - @float.y / this.heightScale;
			this.sdf[index] = value.ToByte();
		}

		[WriteOnly]
		public NativeArray<byte> sdf;

		[ReadOnly]
		public int3 shape;

		[ReadOnly]
		public int3 chunkPosition;

		[ReadOnly]
		public int3 shapeMin;

		[ReadOnly]
		public int3 shapeMax;

		[ReadOnly]
		public float noiseScale;

		[ReadOnly]
		public float heightScale;

		[ReadOnly]
		public int3 min;

		[ReadOnly]
		public int3 max;

		[ReadOnly]
		public int strideY;

		[ReadOnly]
		public int strideZ;
	}
}
