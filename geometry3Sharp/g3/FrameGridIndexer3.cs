using System;

namespace g3
{
	public struct FrameGridIndexer3 : IGridWorldIndexer3
	{
		public FrameGridIndexer3(Frame3f frame, Vector3f cellSize)
		{
			this.GridFrame = frame;
			this.CellSize = cellSize;
		}

		public Vector3i ToGrid(Vector3d point)
		{
			Vector3f a = (Vector3f)point;
			a = this.GridFrame.ToFrameP(ref a);
			return (Vector3i)(a / this.CellSize);
		}

		public Vector3d ToGridf(Vector3d point)
		{
			point = this.GridFrame.ToFrameP(ref point);
			point.x /= (double)this.CellSize.x;
			point.y /= (double)this.CellSize.y;
			point.z /= (double)this.CellSize.z;
			return point;
		}

		public Vector3d FromGrid(Vector3i gridpoint)
		{
			Vector3f vector3f = this.CellSize * (Vector3f)gridpoint;
			return this.GridFrame.FromFrameP(ref vector3f);
		}

		public Vector3d FromGrid(Vector3d gridpointf)
		{
			gridpointf *= this.CellSize;
			return this.GridFrame.FromFrameP(ref gridpointf);
		}

		public Frame3f GridFrame;

		public Vector3f CellSize;
	}
}
