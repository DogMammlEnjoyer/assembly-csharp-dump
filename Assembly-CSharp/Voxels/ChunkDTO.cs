using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	public struct ChunkDTO
	{
		public bool IsValid
		{
			get
			{
				return !this.Size.Equals(int3.zero) && this.Density.IsCreated && this.Material.IsCreated;
			}
		}

		public ChunkDTO(Chunk chunk)
		{
			this.Id = chunk.Id;
			this.Size = chunk.Size;
			this.Dimensions = chunk.Dimensions;
			this.Density = chunk.Density;
			this.Material = chunk.Material;
		}

		public int3 Id;

		public int3 Size;

		public int3 Dimensions;

		public NativeArray<byte> Density;

		public NativeArray<byte> Material;
	}
}
