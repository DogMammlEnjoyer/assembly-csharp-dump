using System;

namespace g3
{
	public struct ShiftGridIndexer3 : IGridWorldIndexer3
	{
		public ShiftGridIndexer3(Vector3d origin, double cellSize)
		{
			this.Origin = origin;
			this.CellSize = cellSize;
		}

		public Vector3i ToGrid(Vector3d point)
		{
			return new Vector3i((int)((point.x - this.Origin.x) / this.CellSize), (int)((point.y - this.Origin.y) / this.CellSize), (int)((point.z - this.Origin.z) / this.CellSize));
		}

		public Vector3d ToGridf(Vector3d point)
		{
			return new Vector3d((point.x - this.Origin.x) / this.CellSize, (point.y - this.Origin.y) / this.CellSize, (point.z - this.Origin.z) / this.CellSize);
		}

		public Vector3d FromGrid(Vector3i gridpoint)
		{
			return new Vector3d((double)gridpoint.x * this.CellSize + this.Origin.x, (double)gridpoint.y * this.CellSize + this.Origin.y, (double)gridpoint.z * this.CellSize + this.Origin.z);
		}

		public Vector3d FromGrid(Vector3d gridpointf)
		{
			return new Vector3d(gridpointf.x * this.CellSize + this.Origin.x, gridpointf.y * this.CellSize + this.Origin.y, gridpointf.z * this.CellSize + this.Origin.z);
		}

		public Vector3d Origin;

		public double CellSize;
	}
}
