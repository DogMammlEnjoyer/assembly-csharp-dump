using System;

namespace g3
{
	public struct ShiftGridIndexer2 : IGridWorldIndexer2
	{
		public ShiftGridIndexer2(Vector2d origin, double cellSize)
		{
			this.Origin = origin;
			this.CellSize = cellSize;
		}

		public Vector2i ToGrid(Vector2d point)
		{
			return new Vector2i((int)((point.x - this.Origin.x) / this.CellSize), (int)((point.y - this.Origin.y) / this.CellSize));
		}

		public Vector2d FromGrid(Vector2i gridpoint)
		{
			return new Vector2d((double)gridpoint.x * this.CellSize + this.Origin.x, (double)gridpoint.y * this.CellSize + this.Origin.y);
		}

		public Vector2d FromGrid(Vector2d gridpointf)
		{
			return new Vector2d(gridpointf.x * this.CellSize + this.Origin.x, gridpointf.y * this.CellSize + this.Origin.y);
		}

		public Vector2d Origin;

		public double CellSize;
	}
}
