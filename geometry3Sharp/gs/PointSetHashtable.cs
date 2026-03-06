using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class PointSetHashtable
	{
		public PointSetHashtable(IPointSet points)
		{
			this.Points = points;
		}

		public void Build(int maxAxisSubdivs = 64)
		{
			AxisAlignedBox3d axisAlignedBox3d = BoundsUtil.Bounds(this.Points);
			double cellSize = axisAlignedBox3d.MaxDim / (double)maxAxisSubdivs;
			this.Build(cellSize, axisAlignedBox3d.Min);
		}

		public void Build(double cellSize, Vector3d origin)
		{
			this.Origin = origin;
			this.CellSize = cellSize;
			this.indexF = new ShiftGridIndexer3(this.Origin, this.CellSize);
			this.Grid = new DSparseGrid3<PointSetHashtable.PointList>(new PointSetHashtable.PointList());
			foreach (int num in this.Points.VertexIndices())
			{
				Vector3d vertex = this.Points.GetVertex(num);
				Vector3i index = this.indexF.ToGrid(vertex);
				this.Grid.Get(index, true).Add(num);
			}
		}

		public bool FindInBall(Vector3d pt, double r, int[] buffer, out int buffer_count)
		{
			buffer_count = 0;
			double num = this.CellSize * 0.5;
			Vector3i vector3i = this.indexF.ToGrid(pt);
			Vector3d vector3d = this.indexF.FromGrid(vector3i) + num * Vector3d.One;
			if (r > this.CellSize)
			{
				throw new ArgumentException("PointSetHashtable.FindInBall: large radius unsupported");
			}
			double num2 = r * r;
			PointSetHashtable.PointList pointList = this.Grid.Get(vector3i, false);
			if (pointList != null)
			{
				foreach (int num3 in pointList)
				{
					if (pt.DistanceSquared(this.Points.GetVertex(num3)) < num2)
					{
						if (buffer_count == buffer.Length)
						{
							return false;
						}
						int num4 = buffer_count;
						buffer_count = num4 + 1;
						buffer[num4] = num3;
					}
				}
			}
			if ((pt - vector3d).MaxAbs + r > num)
			{
				for (int i = 0; i < 26; i++)
				{
					Vector3i vector3i2 = gIndices.GridOffsets26[i];
					Vector3d vector3d2 = new Vector3d(vector3d.x + num * (double)vector3i2.x - pt.x, vector3d.y + num * (double)vector3i2.y - pt.y, vector3d.z + num * (double)vector3i2.z - pt.z);
					if (vector3d2.MinAbs <= r)
					{
						PointSetHashtable.PointList pointList2 = this.Grid.Get(vector3i + vector3i2, false);
						if (pointList2 != null)
						{
							foreach (int num5 in pointList2)
							{
								if (pt.DistanceSquared(this.Points.GetVertex(num5)) < num2)
								{
									if (buffer_count == buffer.Length)
									{
										return false;
									}
									int num4 = buffer_count;
									buffer_count = num4 + 1;
									buffer[num4] = num5;
								}
							}
						}
					}
				}
			}
			return true;
		}

		private IPointSet Points;

		private DSparseGrid3<PointSetHashtable.PointList> Grid;

		private ShiftGridIndexer3 indexF;

		private Vector3d Origin;

		private double CellSize;

		public class PointList : List<int>, IGridElement3
		{
			public IGridElement3 CreateNewGridElement(bool bCopy)
			{
				return new PointSetHashtable.PointList();
			}
		}
	}
}
