using System;

namespace g3
{
	public class CachingMeshSDF
	{
		public CachingMeshSDF(DMesh3 mesh, double cellSize, DMeshAABBTree3 spatial)
		{
			this.Mesh = mesh;
			this.CellSize = (float)cellSize;
			this.Spatial = spatial;
		}

		public void Initialize()
		{
			AxisAlignedBox3d cachedBounds = this.Mesh.CachedBounds;
			float f = Math.Max(4f * this.CellSize, 2f * this.MaxOffsetDistance + 2f * this.CellSize);
			this.grid_origin = (Vector3f)cachedBounds.Min - f * Vector3f.One - (Vector3f)this.ExpandBounds;
			Vector3f vector3f = (Vector3f)cachedBounds.Max + f * Vector3f.One + (Vector3f)this.ExpandBounds;
			int num = (int)((vector3f.x - this.grid_origin.x) / this.CellSize) + 1;
			int num2 = (int)((vector3f.y - this.grid_origin.y) / this.CellSize) + 1;
			int num3 = (int)((vector3f.z - this.grid_origin.z) / this.CellSize) + 1;
			this.UpperBoundDistance = (float)(num + num2 + num3) * this.CellSize;
			this.grid = new DenseGrid3f(num, num2, num3, this.UpperBoundDistance);
			this.MaxDistQueryDist = (double)this.MaxOffsetDistance + (double)(2f * this.CellSize) * 1.4142135623730951;
			if (this.WantClosestTriGrid)
			{
				this.closest_tri_grid = new DenseGrid3i(num, num2, num3, -1);
			}
			DenseGrid3i denseGrid3i = new DenseGrid3i(num, num2, num3, 0);
			if (this.ComputeSigns)
			{
				this.compute_intersections(this.grid_origin, this.CellSize, num, num2, num3, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				this.compute_signs(num, num2, num3, this.grid, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.WantIntersectionsGrid)
				{
					this.intersections_grid = denseGrid3i;
				}
			}
		}

		public float GetValue(Vector3i idx)
		{
			float num = this.grid[idx];
			if (num == this.UpperBoundDistance || num == -this.UpperBoundDistance)
			{
				Vector3d p = this.cell_center(idx);
				float num2 = (float)Math.Sign(num);
				double d;
				int num3 = this.Spatial.FindNearestTriangle(p, out d, this.MaxDistQueryDist);
				if (num3 == -1)
				{
					num += 0.0001f;
				}
				else
				{
					num = num2 * (float)Math.Sqrt(d);
				}
				this.grid[idx] = num;
				if (this.closest_tri_grid != null)
				{
					this.closest_tri_grid[idx] = num3;
				}
			}
			return num;
		}

		public Vector3i Dimensions
		{
			get
			{
				return new Vector3i(this.grid.ni, this.grid.nj, this.grid.nk);
			}
		}

		public DenseGrid3f Grid
		{
			get
			{
				return this.grid;
			}
		}

		public Vector3f GridOrigin
		{
			get
			{
				return this.grid_origin;
			}
		}

		public DenseGrid3i ClosestTriGrid
		{
			get
			{
				if (!this.WantClosestTriGrid)
				{
					throw new Exception("Set WantClosestTriGrid=true to return this value");
				}
				return this.closest_tri_grid;
			}
		}

		public DenseGrid3i IntersectionsGrid
		{
			get
			{
				if (!this.WantIntersectionsGrid)
				{
					throw new Exception("Set WantIntersectionsGrid=true to return this value");
				}
				return this.intersections_grid;
			}
		}

		public float this[int i, int j, int k]
		{
			get
			{
				return this.grid[i, j, k];
			}
		}

		public float this[Vector3i idx]
		{
			get
			{
				return this.grid[idx.x, idx.y, idx.z];
			}
		}

		public Vector3f CellCenter(int i, int j, int k)
		{
			return this.cell_center(new Vector3i(i, j, k));
		}

		private Vector3f cell_center(Vector3i ijk)
		{
			return new Vector3f((float)ijk.x * this.CellSize + this.grid_origin[0], (float)ijk.y * this.CellSize + this.grid_origin[1], (float)ijk.z * this.CellSize + this.grid_origin[2]);
		}

		private void compute_intersections(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3i intersection_count)
		{
			double ox = (double)origin[0];
			double oy = (double)origin[1];
			double oz = (double)origin[2];
			double invdx = 1.0 / (double)dx;
			bool cancelled = false;
			Action<int> action = delegate(int tid)
			{
				if (tid % 100 == 0 && this.CancelF())
				{
					cancelled = true;
				}
				if (cancelled)
				{
					return;
				}
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d zero3 = Vector3d.Zero;
				this.Mesh.GetTriVertices(tid, ref zero, ref zero2, ref zero3);
				bool decrement = false;
				if (this.InsideMode == CachingMeshSDF.InsideModes.ParityCount)
				{
					decrement = (MathUtil.FastNormalDirection(ref zero, ref zero2, ref zero3).x > 0.0);
				}
				double num = (zero[0] - ox) * invdx;
				double num2 = (zero[1] - oy) * invdx;
				double num3 = (zero[2] - oz) * invdx;
				double num4 = (zero2[0] - ox) * invdx;
				double num5 = (zero2[1] - oy) * invdx;
				double num6 = (zero2[2] - oz) * invdx;
				double num7 = (zero3[0] - ox) * invdx;
				double num8 = (zero3[1] - oy) * invdx;
				double num9 = (zero3[2] - oz) * invdx;
				int num10 = MathUtil.Clamp((int)Math.Ceiling(MathUtil.Min(num2, num5, num8)), 0, nj - 1);
				int num11 = MathUtil.Clamp((int)Math.Floor(MathUtil.Max(num2, num5, num8)), 0, nj - 1);
				int num12 = MathUtil.Clamp((int)Math.Ceiling(MathUtil.Min(num3, num6, num9)), 0, nk - 1);
				int num13 = MathUtil.Clamp((int)Math.Floor(MathUtil.Max(num3, num6, num9)), 0, nk - 1);
				for (int i = num12; i <= num13; i++)
				{
					for (int j = num10; j <= num11; j++)
					{
						double num14;
						double num15;
						double num16;
						if (CachingMeshSDF.point_in_triangle_2d((double)j, (double)i, num2, num3, num5, num6, num8, num9, out num14, out num15, out num16))
						{
							int num17 = (int)Math.Ceiling(num14 * num + num15 * num4 + num16 * num7);
							if (num17 < 0)
							{
								intersection_count.atomic_incdec(0, j, i, decrement);
							}
							else if (num17 < ni)
							{
								intersection_count.atomic_incdec(num17, j, i, decrement);
							}
						}
					}
				}
			};
			if (this.UseParallel)
			{
				gParallel.ForEach<int>(this.Mesh.TriangleIndices(), action);
				return;
			}
			foreach (int obj in this.Mesh.TriangleIndices())
			{
				action(obj);
			}
		}

		private void compute_signs(int ni, int nj, int nk, DenseGrid3f distances, DenseGrid3i intersection_counts)
		{
			Func<int, bool> isInsideF = (int count) => count % 2 == 1;
			if (this.InsideMode == CachingMeshSDF.InsideModes.ParityCount)
			{
				isInsideF = ((int count) => count > 0);
			}
			if (this.UseParallel)
			{
				AxisAlignedBox2i axisAlignedBox2i = new AxisAlignedBox2i(0, 0, nj, nk);
				gParallel.ForEach<Vector2i>(axisAlignedBox2i.IndicesExclusive(), delegate(Vector2i vi)
				{
					if (this.CancelF())
					{
						return;
					}
					int x = vi.x;
					int y = vi.y;
					int num2 = 0;
					for (int l = 0; l < ni; l++)
					{
						num2 += intersection_counts[l, x, y];
						if (isInsideF(num2))
						{
							distances[l, x, y] = -distances[l, x, y];
						}
					}
				});
				return;
			}
			for (int i = 0; i < nk; i++)
			{
				if (this.CancelF())
				{
					return;
				}
				for (int j = 0; j < nj; j++)
				{
					int num = 0;
					for (int k = 0; k < ni; k++)
					{
						num += intersection_counts[k, j, i];
						if (isInsideF(num))
						{
							distances[k, j, i] = -distances[k, j, i];
						}
					}
				}
			}
		}

		public static int orientation(double x1, double y1, double x2, double y2, out double twice_signed_area)
		{
			twice_signed_area = y1 * x2 - x1 * y2;
			if (twice_signed_area > 0.0)
			{
				return 1;
			}
			if (twice_signed_area < 0.0)
			{
				return -1;
			}
			if (y2 > y1)
			{
				return 1;
			}
			if (y2 < y1)
			{
				return -1;
			}
			if (x1 > x2)
			{
				return 1;
			}
			if (x1 < x2)
			{
				return -1;
			}
			return 0;
		}

		public static bool point_in_triangle_2d(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3, out double a, out double b, out double c)
		{
			a = (b = (c = 0.0));
			x1 -= x0;
			x2 -= x0;
			x3 -= x0;
			y1 -= y0;
			y2 -= y0;
			y3 -= y0;
			int num = CachingMeshSDF.orientation(x2, y2, x3, y3, out a);
			if (num == 0)
			{
				return false;
			}
			if (CachingMeshSDF.orientation(x3, y3, x1, y1, out b) != num)
			{
				return false;
			}
			if (CachingMeshSDF.orientation(x1, y1, x2, y2, out c) != num)
			{
				return false;
			}
			double num2 = a + b + c;
			if (num2 == 0.0)
			{
				throw new Exception("MakeNarrowBandLevelSet.point_in_triangle_2d: badness!");
			}
			a /= num2;
			b /= num2;
			c /= num2;
			return true;
		}

		public DMesh3 Mesh;

		public DMeshAABBTree3 Spatial;

		public float CellSize;

		public Vector3d ExpandBounds = Vector3d.Zero;

		public float MaxOffsetDistance;

		public bool UseParallel = true;

		public bool ComputeSigns = true;

		public CachingMeshSDF.InsideModes InsideMode = CachingMeshSDF.InsideModes.ParityCount;

		public bool WantClosestTriGrid;

		public bool WantIntersectionsGrid;

		public Func<bool> CancelF = () => false;

		public bool DebugPrint;

		private Vector3f grid_origin;

		private DenseGrid3f grid;

		private DenseGrid3i closest_tri_grid;

		private DenseGrid3i intersections_grid;

		private float UpperBoundDistance;

		private double MaxDistQueryDist;

		public enum InsideModes
		{
			CrossingCount,
			ParityCount
		}
	}
}
