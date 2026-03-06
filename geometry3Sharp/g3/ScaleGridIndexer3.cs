using System;

namespace g3
{
	public struct ScaleGridIndexer3 : IGridWorldIndexer3
	{
		public ScaleGridIndexer3(double cellSize)
		{
			this.CellSize = cellSize;
		}

		public Vector3i ToGrid(Vector3d point)
		{
			return new Vector3i((int)(point.x / this.CellSize), (int)(point.y / this.CellSize), (int)(point.z / this.CellSize));
		}

		public Vector3d ToGridf(Vector3d point)
		{
			return new Vector3d(point.x / this.CellSize, point.y / this.CellSize, point.z / this.CellSize);
		}

		public Vector3d FromGrid(Vector3i gridpoint)
		{
			return new Vector3d((double)gridpoint.x * this.CellSize, (double)gridpoint.y * this.CellSize, (double)gridpoint.z * this.CellSize);
		}

		public Vector3d FromGrid(Vector3d gridpointf)
		{
			return new Vector3d(gridpointf.x * this.CellSize, gridpointf.y * this.CellSize, gridpointf.z * this.CellSize);
		}

		public double CellSize;
	}
}
