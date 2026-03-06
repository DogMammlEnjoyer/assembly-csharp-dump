using System;
using System.Collections.Generic;

namespace g3
{
	public class ImplicitFieldSampler3d
	{
		public ImplicitFieldSampler3d(AxisAlignedBox3d fieldBounds, double cellSize)
		{
			this.CellSize = cellSize;
			this.GridOrigin = fieldBounds.Min;
			this.Indexer = new ShiftGridIndexer3(this.GridOrigin, this.CellSize);
			Vector3d vector3d = fieldBounds.Max + cellSize;
			int num = (int)((vector3d.x - this.GridOrigin.x) / this.CellSize) + 1;
			int num2 = (int)((vector3d.y - this.GridOrigin.y) / this.CellSize) + 1;
			int num3 = (int)((vector3d.z - this.GridOrigin.z) / this.CellSize) + 1;
			this.GridBounds = new AxisAlignedBox3i(0, 0, 0, num, num2, num3);
			this.BackgroundValue = (float)((double)(num + num2 + num3) * this.CellSize);
			this.Grid = new DenseGrid3f(num, num2, num3, this.BackgroundValue);
		}

		public DenseGridTrilinearImplicit ToImplicit()
		{
			return new DenseGridTrilinearImplicit(this.Grid, this.GridOrigin, this.CellSize);
		}

		public void Clear(float f)
		{
			this.BackgroundValue = f;
			this.Grid.assign(this.BackgroundValue);
		}

		public void Sample(BoundedImplicitFunction3d f, double expandRadius = 0.0)
		{
			AxisAlignedBox3d axisAlignedBox3d = f.Bounds();
			Vector3d v = expandRadius * Vector3d.One;
			Vector3i vector3i = this.Indexer.ToGrid(axisAlignedBox3d.Min - v);
			Vector3i vector3i2 = this.Indexer.ToGrid(axisAlignedBox3d.Max + v) + Vector3i.One;
			vector3i = this.GridBounds.ClampExclusive(vector3i);
			vector3i2 = this.GridBounds.ClampExclusive(vector3i2);
			AxisAlignedBox3i axisAlignedBox3i = new AxisAlignedBox3i(vector3i, vector3i2);
			if (this.CombineMode == ImplicitFieldSampler3d.CombineModes.DistanceMinUnion)
			{
				this.sample_min(f, axisAlignedBox3i.IndicesInclusive());
			}
		}

		private void sample_min(BoundedImplicitFunction3d f, IEnumerable<Vector3i> indices)
		{
			gParallel.ForEach<Vector3i>(indices, delegate(Vector3i idx)
			{
				Vector3d vector3d = this.Indexer.FromGrid(idx);
				double num = f.Value(ref vector3d);
				this.Grid.set_min(ref idx, (float)num);
			});
		}

		public DenseGrid3f Grid;

		public double CellSize;

		public Vector3d GridOrigin;

		public ShiftGridIndexer3 Indexer;

		public AxisAlignedBox3i GridBounds;

		public float BackgroundValue;

		public ImplicitFieldSampler3d.CombineModes CombineMode;

		public enum CombineModes
		{
			DistanceMinUnion
		}
	}
}
