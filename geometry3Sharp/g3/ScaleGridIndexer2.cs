using System;

namespace g3
{
	public struct ScaleGridIndexer2 : IGridWorldIndexer2
	{
		public ScaleGridIndexer2(double cellSize)
		{
			this.CellSize = cellSize;
		}

		public Vector2i ToGrid(Vector2d point)
		{
			return new Vector2i((int)(point.x / this.CellSize), (int)(point.y / this.CellSize));
		}

		public Vector2d FromGrid(Vector2i gridpoint)
		{
			return new Vector2d((double)gridpoint.x * this.CellSize, (double)gridpoint.y * this.CellSize);
		}

		public Vector2d FromGrid(Vector2d gridpointf)
		{
			return new Vector2d(gridpointf.x * this.CellSize, gridpointf.y * this.CellSize);
		}

		public double CellSize;
	}
}
