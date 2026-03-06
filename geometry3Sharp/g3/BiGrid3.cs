using System;
using System.Collections.Generic;

namespace g3
{
	public class BiGrid3<BlockType> where BlockType : class, IGridElement3, IFixedGrid3
	{
		public Vector3i BlockSize
		{
			get
			{
				return this.block_size;
			}
		}

		public MultigridIndexer3 Indexer
		{
			get
			{
				return this.indexer;
			}
		}

		public DSparseGrid3<BlockType> BlockGrid
		{
			get
			{
				return this.sparse_grid;
			}
		}

		public BiGrid3(BlockType exemplar)
		{
			this.block_size = exemplar.Dimensions;
			this.indexer = new MultigridIndexer3(this.block_size);
			this.sparse_grid = new DSparseGrid3<BlockType>(exemplar);
		}

		public void Update(Index3i index, Action<BlockType, Vector3i> UpdateF)
		{
			GridLevelIndex gridLevelIndex = this.Indexer.ToBlock(index);
			BlockType arg = this.sparse_grid.Get(gridLevelIndex.block_index, true);
			UpdateF(arg, gridLevelIndex.local_index);
		}

		public IEnumerable<KeyValuePair<Vector3i, BlockType>> AllocatedBlocks()
		{
			return this.sparse_grid.Allocated();
		}

		private Vector3i block_size;

		private MultigridIndexer3 indexer;

		private DSparseGrid3<BlockType> sparse_grid;
	}
}
