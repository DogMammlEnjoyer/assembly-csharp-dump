using System;

namespace g3
{
	public struct MultigridIndexer3 : IMultigridIndexer3
	{
		public MultigridIndexer3(Vector3i blockSize)
		{
			this.BlockSize = blockSize;
			this.OuterShift = (this.BlockShift = Vector3i.Zero);
		}

		public Vector3i ToBlockIndex(Vector3i outer_index)
		{
			Vector3i vector3i = outer_index - this.OuterShift;
			vector3i.x = ((vector3i.x >= 0) ? (vector3i.x / this.BlockSize.x) : (vector3i.x / this.BlockSize.x - 1));
			vector3i.y = ((vector3i.y >= 0) ? (vector3i.y / this.BlockSize.y) : (vector3i.y / this.BlockSize.y - 1));
			vector3i.z = ((vector3i.z >= 0) ? (vector3i.z / this.BlockSize.z) : (vector3i.z / this.BlockSize.z - 1));
			return vector3i - this.BlockShift;
		}

		public Vector3i ToBlockLocal(Vector3i outer_index)
		{
			Vector3i a = this.ToBlockIndex(outer_index);
			return outer_index - a * this.BlockSize;
		}

		public GridLevelIndex ToBlock(Vector3i outer_index)
		{
			Vector3i vector3i = outer_index - this.OuterShift;
			vector3i.x = ((vector3i.x >= 0) ? (vector3i.x / this.BlockSize.x) : (vector3i.x / this.BlockSize.x - 1));
			vector3i.y = ((vector3i.y >= 0) ? (vector3i.y / this.BlockSize.y) : (vector3i.y / this.BlockSize.y - 1));
			vector3i.z = ((vector3i.z >= 0) ? (vector3i.z / this.BlockSize.z) : (vector3i.z / this.BlockSize.z - 1));
			vector3i -= this.BlockShift;
			return new GridLevelIndex
			{
				block_index = vector3i,
				local_index = outer_index - vector3i * this.BlockSize
			};
		}

		public Vector3i FromBlock(Vector3i block_idx)
		{
			return (block_idx + this.BlockShift) * this.BlockSize + this.OuterShift;
		}

		public Vector3i OuterShift;

		public Vector3i BlockSize;

		public Vector3i BlockShift;
	}
}
