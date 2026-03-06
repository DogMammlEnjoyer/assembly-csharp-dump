using System;

namespace g3
{
	public struct MultigridIndexer2 : IMultigridIndexer2
	{
		public MultigridIndexer2(Vector2i blockSize)
		{
			this.BlockSize = blockSize;
			this.OuterShift = (this.BlockShift = Vector2i.Zero);
		}

		public Vector2i ToBlockIndex(Vector2i outer_index)
		{
			Vector2i vector2i = outer_index - this.OuterShift;
			vector2i.x = ((vector2i.x >= 0) ? (vector2i.x / this.BlockSize.x) : (vector2i.x / this.BlockSize.x - 1));
			vector2i.y = ((vector2i.y >= 0) ? (vector2i.y / this.BlockSize.y) : (vector2i.y / this.BlockSize.y - 1));
			return vector2i - this.BlockShift;
		}

		public Vector2i ToBlockLocal(Vector2i outer_index)
		{
			Vector2i a = this.ToBlockIndex(outer_index);
			return outer_index - a * this.BlockSize;
		}

		public GridLevelIndex2 ToBlock(Vector2i outer_index)
		{
			Vector2i vector2i = outer_index - this.OuterShift;
			vector2i.x = ((vector2i.x >= 0) ? (vector2i.x / this.BlockSize.x) : (vector2i.x / this.BlockSize.x - 1));
			vector2i.y = ((vector2i.y >= 0) ? (vector2i.y / this.BlockSize.y) : (vector2i.y / this.BlockSize.y - 1));
			vector2i -= this.BlockShift;
			return new GridLevelIndex2
			{
				block_index = vector2i,
				local_index = outer_index - vector2i * this.BlockSize
			};
		}

		public Vector2i FromBlock(Vector2i block_idx)
		{
			return (block_idx + this.BlockShift) * this.BlockSize + this.OuterShift;
		}

		public Vector2i OuterShift;

		public Vector2i BlockSize;

		public Vector2i BlockShift;
	}
}
