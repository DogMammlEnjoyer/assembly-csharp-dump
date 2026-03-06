using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class MeshSignedDistanceGrid
	{
		public MeshSignedDistanceGrid(DMesh3 mesh, double cellSize, DMeshAABBTree3 spatial = null)
		{
			this.Mesh = mesh;
			this.CellSize = (float)cellSize;
			this.Spatial = spatial;
		}

		public void Compute()
		{
			AxisAlignedBox3d cachedBounds = this.Mesh.CachedBounds;
			float num = (float)(2 * this.ExactBandWidth) * this.CellSize;
			if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.NarrowBand_SpatialFloodFill)
			{
				num = (float)Math.Max((double)num, 2.0 * this.NarrowBandMaxDistance);
			}
			this.grid_origin = (Vector3f)cachedBounds.Min - num * Vector3f.One - (Vector3f)this.ExpandBounds;
			Vector3f vector3f = (Vector3f)cachedBounds.Max + num * Vector3f.One + (Vector3f)this.ExpandBounds;
			int ni = (int)((vector3f.x - this.grid_origin.x) / this.CellSize) + 1;
			int nj = (int)((vector3f.y - this.grid_origin.y) / this.CellSize) + 1;
			int nk = (int)((vector3f.z - this.grid_origin.z) / this.CellSize) + 1;
			this.grid = new DenseGrid3f();
			if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.NarrowBand_SpatialFloodFill)
			{
				if (this.Spatial == null || this.NarrowBandMaxDistance == 0.0 || !this.UseParallel)
				{
					throw new Exception("MeshSignedDistanceGrid.Compute: must set Spatial data structure and band max distance, and UseParallel=true");
				}
				this.make_level_set3_parallel_floodfill(this.grid_origin, this.CellSize, ni, nj, nk, this.grid, this.ExactBandWidth);
				return;
			}
			else
			{
				if (!this.UseParallel)
				{
					this.make_level_set3(this.grid_origin, this.CellSize, ni, nj, nk, this.grid, this.ExactBandWidth);
					return;
				}
				if (this.Spatial != null)
				{
					this.make_level_set3_parallel_spatial(this.grid_origin, this.CellSize, ni, nj, nk, this.grid, this.ExactBandWidth);
					return;
				}
				this.make_level_set3_parallel(this.grid_origin, this.CellSize, ni, nj, nk, this.grid, this.ExactBandWidth);
				return;
			}
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

		private float upper_bound(DenseGrid3f grid)
		{
			return (float)(grid.ni + grid.nj + grid.nk) * this.CellSize;
		}

		private float cell_tri_dist(Vector3i idx, int tid)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d vector3d = this.cell_center(idx);
			this.Mesh.GetTriVertices(tid, ref zero, ref zero2, ref zero3);
			return (float)MeshSignedDistanceGrid.point_triangle_distance(ref vector3d, ref zero, ref zero2, ref zero3);
		}

		private void make_level_set3(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f distances, int exact_band)
		{
			distances.resize(ni, nj, nk);
			distances.assign(this.upper_bound(distances));
			DenseGrid3i denseGrid3i = new DenseGrid3i(ni, nj, nk, -1);
			DenseGrid3i denseGrid3i2 = new DenseGrid3i(ni, nj, nk, 0);
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			double num = (double)dx;
			double num2 = (double)origin[0];
			double num3 = (double)origin[1];
			double num4 = (double)origin[2];
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			foreach (int num5 in this.Mesh.TriangleIndices())
			{
				if (num5 % 100 == 0 && this.CancelF())
				{
					break;
				}
				this.Mesh.GetTriVertices(num5, ref zero, ref zero2, ref zero3);
				double a = (zero[0] - num2) / num;
				double a2 = (zero[1] - num3) / num;
				double a3 = (zero[2] - num4) / num;
				double b = (zero2[0] - num2) / num;
				double b2 = (zero2[1] - num3) / num;
				double b3 = (zero2[2] - num4) / num;
				double c = (zero3[0] - num2) / num;
				double c2 = (zero3[1] - num3) / num;
				double c3 = (zero3[2] - num4) / num;
				int num6 = MathUtil.Clamp((int)MathUtil.Min(a, b, c) - exact_band, 0, ni - 1);
				int num7 = MathUtil.Clamp((int)MathUtil.Max(a, b, c) + exact_band + 1, 0, ni - 1);
				int num8 = MathUtil.Clamp((int)MathUtil.Min(a2, b2, c2) - exact_band, 0, nj - 1);
				int num9 = MathUtil.Clamp((int)MathUtil.Max(a2, b2, c2) + exact_band + 1, 0, nj - 1);
				int num10 = MathUtil.Clamp((int)MathUtil.Min(a3, b3, c3) - exact_band, 0, nk - 1);
				int num11 = MathUtil.Clamp((int)MathUtil.Max(a3, b3, c3) + exact_band + 1, 0, nk - 1);
				for (int i = num10; i <= num11; i++)
				{
					for (int j = num8; j <= num9; j++)
					{
						for (int k = num6; k <= num7; k++)
						{
							Vector3d vector3d = new Vector3d((double)((float)k * dx + origin[0]), (double)((float)j * dx + origin[1]), (double)((float)i * dx + origin[2]));
							float num12 = (float)MeshSignedDistanceGrid.point_triangle_distance(ref vector3d, ref zero, ref zero2, ref zero3);
							if (num12 < distances[k, j, i])
							{
								distances[k, j, i] = num12;
								denseGrid3i[k, j, i] = num5;
							}
						}
					}
				}
			}
			if (this.CancelF())
			{
				return;
			}
			if (this.ComputeSigns)
			{
				if (this.DebugPrint)
				{
					Console.WriteLine("done narrow-band");
				}
				this.compute_intersections(origin, dx, ni, nj, nk, denseGrid3i2);
				if (this.CancelF())
				{
					return;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done intersections");
				}
				if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.FullGrid)
				{
					for (int l = 0; l < 2; l++)
					{
						this.sweep_pass(origin, dx, distances, denseGrid3i);
						if (this.CancelF())
						{
							return;
						}
					}
					if (this.DebugPrint)
					{
						Console.WriteLine("done sweeping");
					}
				}
				else if (this.DebugPrint)
				{
					Console.WriteLine("skipped sweeping");
				}
				this.compute_signs(ni, nj, nk, distances, denseGrid3i2);
				if (this.CancelF())
				{
					return;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done signs");
				}
				if (this.WantIntersectionsGrid)
				{
					this.intersections_grid = denseGrid3i2;
				}
			}
			if (this.WantClosestTriGrid)
			{
				this.closest_tri_grid = denseGrid3i;
			}
		}

		private void make_level_set3_parallel(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f distances, int exact_band)
		{
			distances.resize(ni, nj, nk);
			distances.assign(this.upper_bound(this.grid));
			DenseGrid3i closest_tri = new DenseGrid3i(ni, nj, nk, -1);
			DenseGrid3i denseGrid3i = new DenseGrid3i(ni, nj, nk, 0);
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			double ox = (double)origin[0];
			double oy = (double)origin[1];
			double oz = (double)origin[2];
			double invdx = 1.0 / (double)dx;
			int wi = ni / 2;
			int wj = nj / 2;
			int wk = nk / 2;
			SpinLock[] grid_locks = new SpinLock[8];
			bool abort = false;
			gParallel.ForEach<int>(this.Mesh.TriangleIndices(), delegate(int tid)
			{
				if (tid % 100 == 0)
				{
					abort = this.CancelF();
				}
				if (abort)
				{
					return;
				}
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d zero3 = Vector3d.Zero;
				this.Mesh.GetTriVertices(tid, ref zero, ref zero2, ref zero3);
				double a = (zero[0] - ox) * invdx;
				double a2 = (zero[1] - oy) * invdx;
				double a3 = (zero[2] - oz) * invdx;
				double b = (zero2[0] - ox) * invdx;
				double b2 = (zero2[1] - oy) * invdx;
				double b3 = (zero2[2] - oz) * invdx;
				double c = (zero3[0] - ox) * invdx;
				double c2 = (zero3[1] - oy) * invdx;
				double c3 = (zero3[2] - oz) * invdx;
				int num = MathUtil.Clamp((int)MathUtil.Min(a, b, c) - exact_band, 0, ni - 1);
				int num2 = MathUtil.Clamp((int)MathUtil.Max(a, b, c) + exact_band + 1, 0, ni - 1);
				int num3 = MathUtil.Clamp((int)MathUtil.Min(a2, b2, c2) - exact_band, 0, nj - 1);
				int num4 = MathUtil.Clamp((int)MathUtil.Max(a2, b2, c2) + exact_band + 1, 0, nj - 1);
				int num5 = MathUtil.Clamp((int)MathUtil.Min(a3, b3, c3) - exact_band, 0, nk - 1);
				int num6 = MathUtil.Clamp((int)MathUtil.Max(a3, b3, c3) + exact_band + 1, 0, nk - 1);
				for (int j = num5; j <= num6; j++)
				{
					for (int k = num3; k <= num4; k++)
					{
						int num7 = ((k < wj) ? 0 : 1) | ((j < wk) ? 0 : 2);
						for (int l = num; l <= num2; l++)
						{
							Vector3d vector3d = new Vector3d((double)((float)l * dx + origin[0]), (double)((float)k * dx + origin[1]), (double)((float)j * dx + origin[2]));
							float num8 = (float)MeshSignedDistanceGrid.point_triangle_distance(ref vector3d, ref zero, ref zero2, ref zero3);
							if (num8 < distances[l, k, j])
							{
								int num9 = num7 | ((l < wi) ? 0 : 4);
								bool flag = false;
								grid_locks[num9].Enter(ref flag);
								if (num8 < distances[l, k, j])
								{
									distances[l, k, j] = num8;
									closest_tri[l, k, j] = tid;
								}
								grid_locks[num9].Exit();
							}
						}
					}
				}
			});
			if (this.DebugPrint)
			{
				Console.WriteLine("done narrow-band");
			}
			if (this.CancelF())
			{
				return;
			}
			if (this.ComputeSigns)
			{
				this.compute_intersections(origin, dx, ni, nj, nk, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done intersections");
				}
				if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.FullGrid)
				{
					for (int i = 0; i < 2; i++)
					{
						this.sweep_pass(origin, dx, distances, closest_tri);
						if (this.CancelF())
						{
							return;
						}
					}
					if (this.DebugPrint)
					{
						Console.WriteLine("done sweeping");
					}
				}
				else if (this.DebugPrint)
				{
					Console.WriteLine("skipped sweeping");
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done sweeping");
				}
				this.compute_signs(ni, nj, nk, distances, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.WantIntersectionsGrid)
				{
					this.intersections_grid = denseGrid3i;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done signs");
				}
			}
			if (this.WantClosestTriGrid)
			{
				this.closest_tri_grid = closest_tri;
			}
		}

		private void make_level_set3_parallel_spatial(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f distances, int exact_band)
		{
			distances.resize(ni, nj, nk);
			float upper_bound = this.upper_bound(distances);
			distances.assign(upper_bound);
			DenseGrid3i closest_tri = new DenseGrid3i(ni, nj, nk, -1);
			DenseGrid3i denseGrid3i = new DenseGrid3i(ni, nj, nk, 0);
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			double ox = (double)origin[0];
			double oy = (double)origin[1];
			double oz = (double)origin[2];
			double invdx = 1.0 / (double)dx;
			bool abort = false;
			gParallel.ForEach<int>(this.Mesh.TriangleIndices(), delegate(int tid)
			{
				if (tid % 100 == 0)
				{
					abort = this.CancelF();
				}
				if (abort)
				{
					return;
				}
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d zero3 = Vector3d.Zero;
				this.Mesh.GetTriVertices(tid, ref zero, ref zero2, ref zero3);
				double a = (zero[0] - ox) * invdx;
				double a2 = (zero[1] - oy) * invdx;
				double a3 = (zero[2] - oz) * invdx;
				double b = (zero2[0] - ox) * invdx;
				double b2 = (zero2[1] - oy) * invdx;
				double b3 = (zero2[2] - oz) * invdx;
				double c = (zero3[0] - ox) * invdx;
				double c2 = (zero3[1] - oy) * invdx;
				double c3 = (zero3[2] - oz) * invdx;
				int num = MathUtil.Clamp((int)MathUtil.Min(a, b, c) - exact_band, 0, ni - 1);
				int num2 = MathUtil.Clamp((int)MathUtil.Max(a, b, c) + exact_band + 1, 0, ni - 1);
				int num3 = MathUtil.Clamp((int)MathUtil.Min(a2, b2, c2) - exact_band, 0, nj - 1);
				int num4 = MathUtil.Clamp((int)MathUtil.Max(a2, b2, c2) + exact_band + 1, 0, nj - 1);
				int num5 = MathUtil.Clamp((int)MathUtil.Min(a3, b3, c3) - exact_band, 0, nk - 1);
				int num6 = MathUtil.Clamp((int)MathUtil.Max(a3, b3, c3) + exact_band + 1, 0, nk - 1);
				for (int j = num5; j <= num6; j++)
				{
					for (int k = num3; k <= num4; k++)
					{
						for (int l = num; l <= num2; l++)
						{
							distances[l, k, j] = 1f;
						}
					}
				}
			});
			if (this.DebugPrint)
			{
				Console.WriteLine("done narrow-band tagging");
			}
			double max_dist = (double)exact_band * ((double)dx * 1.4142135623730951);
			gParallel.ForEach<Vector3i>(this.grid.Indices(), delegate(Vector3i idx)
			{
				if (distances[idx] == 1f)
				{
					int x = idx.x;
					int y = idx.y;
					int z = idx.z;
					Vector3d p = new Vector3d((double)((float)x * dx + origin[0]), (double)((float)y * dx + origin[1]), (double)((float)z * dx + origin[2]));
					int num = this.Spatial.FindNearestTriangle(p, max_dist);
					if (num == -1)
					{
						distances[idx] = upper_bound;
						return;
					}
					Triangle3d triangle3d = default(Triangle3d);
					this.Mesh.GetTriVertices(num, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
					Vector3d vector3d = default(Vector3d);
					Vector3d vector3d2 = default(Vector3d);
					double d = DistPoint3Triangle3.DistanceSqr(ref p, ref triangle3d, out vector3d, out vector3d2);
					distances[idx] = (float)Math.Sqrt(d);
					closest_tri[idx] = num;
				}
			});
			if (this.DebugPrint)
			{
				Console.WriteLine("done distances");
			}
			if (this.CancelF())
			{
				return;
			}
			if (this.ComputeSigns)
			{
				if (this.DebugPrint)
				{
					Console.WriteLine("done narrow-band");
				}
				this.compute_intersections(origin, dx, ni, nj, nk, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done intersections");
				}
				if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.FullGrid)
				{
					for (int i = 0; i < 2; i++)
					{
						this.sweep_pass(origin, dx, distances, closest_tri);
						if (this.CancelF())
						{
							return;
						}
					}
					if (this.DebugPrint)
					{
						Console.WriteLine("done sweeping");
					}
				}
				else if (this.DebugPrint)
				{
					Console.WriteLine("skipped sweeping");
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done sweeping");
				}
				this.compute_signs(ni, nj, nk, distances, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.WantIntersectionsGrid)
				{
					this.intersections_grid = denseGrid3i;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done signs");
				}
			}
			if (this.WantClosestTriGrid)
			{
				this.closest_tri_grid = closest_tri;
			}
		}

		private void make_level_set3_parallel_floodfill(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f distances, int exact_band)
		{
			distances.resize(ni, nj, nk);
			float upper_bound = this.upper_bound(distances);
			distances.assign(upper_bound);
			DenseGrid3i closest_tri = new DenseGrid3i(ni, nj, nk, -1);
			DenseGrid3i denseGrid3i = new DenseGrid3i(ni, nj, nk, 0);
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			double ox = (double)origin[0];
			double oy = (double)origin[1];
			double oz = (double)origin[2];
			double invdx = 1.0 / (double)dx;
			SpinLock grid_lock = default(SpinLock);
			List<int> Q = new List<int>();
			bool[] done = new bool[distances.size];
			bool abort = false;
			gParallel.ForEach<int>(this.Mesh.VertexIndices(), delegate(int vid)
			{
				if (vid % 100 == 0)
				{
					abort = this.CancelF();
				}
				if (abort)
				{
					return;
				}
				Vector3d vertex = this.Mesh.GetVertex(vid);
				double num = (vertex.x - ox) * invdx;
				double num2 = (vertex.y - oy) * invdx;
				double num3 = (vertex.z - oz) * invdx;
				Vector3i ijk = new Vector3i(MathUtil.Clamp((int)num, 0, ni - 1), MathUtil.Clamp((int)num2, 0, nj - 1), MathUtil.Clamp((int)num3, 0, nk - 1));
				if (distances[ijk] < upper_bound)
				{
					return;
				}
				bool flag = false;
				grid_lock.Enter(ref flag);
				Vector3d p = this.cell_center(ijk);
				int num4 = this.Spatial.FindNearestTriangle(p, double.MaxValue);
				Triangle3d triangle3d = default(Triangle3d);
				this.Mesh.GetTriVertices(num4, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
				Vector3d vector3d = default(Vector3d);
				Vector3d vector3d2 = default(Vector3d);
				double d = DistPoint3Triangle3.DistanceSqr(ref p, ref triangle3d, out vector3d, out vector3d2);
				distances[ijk] = (float)Math.Sqrt(d);
				closest_tri[ijk] = num4;
				int num5 = distances.to_linear(ref ijk);
				Q.Add(num5);
				done[num5] = true;
				grid_lock.Exit();
			});
			if (this.DebugPrint)
			{
				Console.WriteLine("done vertices");
			}
			if (this.CancelF())
			{
				return;
			}
			List<int> next_Q = new List<int>();
			AxisAlignedBox3i bounds = distances.BoundsInclusive;
			double max_dist = this.NarrowBandMaxDistance;
			double max_query_dist = max_dist + (double)(2f * dx) * 1.4142135623730951;
			Action<int> <>9__1;
			for (int i = Q.Count; i > 0; i = Q.Count)
			{
				next_Q.Clear();
				IEnumerable<int> q = Q;
				Action<int> body;
				if ((body = <>9__1) == null)
				{
					body = (<>9__1 = delegate(int cur_linear_index)
					{
						Vector3i v = distances.to_index(cur_linear_index);
						foreach (Vector3i v2 in gIndices.GridOffsets26)
						{
							Vector3i vector3i = v + v2;
							if (bounds.Contains(vector3i))
							{
								int num = distances.to_linear(ref vector3i);
								if (!done[num])
								{
									Vector3d p = this.cell_center(vector3i);
									int num2 = this.Spatial.FindNearestTriangle(p, max_query_dist);
									if (num2 == -1)
									{
										done[num] = true;
									}
									else
									{
										Triangle3d triangle3d = default(Triangle3d);
										this.Mesh.GetTriVertices(num2, ref triangle3d.V0, ref triangle3d.V1, ref triangle3d.V2);
										Vector3d vector3d = default(Vector3d);
										Vector3d vector3d2 = default(Vector3d);
										double num3 = Math.Sqrt(DistPoint3Triangle3.DistanceSqr(ref p, ref triangle3d, out vector3d, out vector3d2));
										bool flag = false;
										grid_lock.Enter(ref flag);
										if (!done[num])
										{
											distances[num] = (float)num3;
											closest_tri[num] = num2;
											done[num] = true;
											if (num3 < max_dist)
											{
												next_Q.Add(num);
											}
										}
										grid_lock.Exit();
									}
								}
							}
						}
					});
				}
				gParallel.ForEach<int>(q, body);
				List<int> q2 = Q;
				Q = next_Q;
				next_Q = q2;
			}
			if (this.DebugPrint)
			{
				Console.WriteLine("done floodfill");
			}
			if (this.CancelF())
			{
				return;
			}
			if (this.ComputeSigns)
			{
				if (this.DebugPrint)
				{
					Console.WriteLine("done narrow-band");
				}
				this.compute_intersections(origin, dx, ni, nj, nk, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done intersections");
				}
				if (this.ComputeMode == MeshSignedDistanceGrid.ComputeModes.FullGrid)
				{
					for (int j = 0; j < 2; j++)
					{
						this.sweep_pass(origin, dx, distances, closest_tri);
						if (this.CancelF())
						{
							return;
						}
					}
					if (this.DebugPrint)
					{
						Console.WriteLine("done sweeping");
					}
				}
				else if (this.DebugPrint)
				{
					Console.WriteLine("skipped sweeping");
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done sweeping");
				}
				this.compute_signs(ni, nj, nk, distances, denseGrid3i);
				if (this.CancelF())
				{
					return;
				}
				if (this.WantIntersectionsGrid)
				{
					this.intersections_grid = denseGrid3i;
				}
				if (this.DebugPrint)
				{
					Console.WriteLine("done signs");
				}
			}
			if (this.WantClosestTriGrid)
			{
				this.closest_tri_grid = closest_tri;
			}
		}

		private void sweep_pass(Vector3f origin, float dx, DenseGrid3f distances, DenseGrid3i closest_tri)
		{
			this.sweep(distances, closest_tri, origin, dx, 1, 1, 1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, -1, -1, -1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, 1, 1, -1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, -1, -1, 1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, 1, -1, 1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, -1, 1, -1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, 1, -1, -1);
			if (this.CancelF())
			{
				return;
			}
			this.sweep(distances, closest_tri, origin, dx, -1, 1, 1);
		}

		private void sweep(DenseGrid3f phi, DenseGrid3i closest_tri, Vector3f origin, float dx, int di, int dj, int dk)
		{
			int num;
			int num2;
			if (di > 0)
			{
				num = 1;
				num2 = phi.ni;
			}
			else
			{
				num = phi.ni - 2;
				num2 = -1;
			}
			int num3;
			int num4;
			if (dj > 0)
			{
				num3 = 1;
				num4 = phi.nj;
			}
			else
			{
				num3 = phi.nj - 2;
				num4 = -1;
			}
			int num5;
			int num6;
			if (dk > 0)
			{
				num5 = 1;
				num6 = phi.nk;
			}
			else
			{
				num5 = phi.nk - 2;
				num6 = -1;
			}
			for (int num7 = num5; num7 != num6; num7 += dk)
			{
				if (this.CancelF())
				{
					return;
				}
				for (int num8 = num3; num8 != num4; num8 += dj)
				{
					for (int num9 = num; num9 != num2; num9 += di)
					{
						Vector3d vector3d = new Vector3d((double)((float)num9 * dx + origin[0]), (double)((float)num8 * dx + origin[1]), (double)((float)num7 * dx + origin[2]));
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9 - di, num8, num7);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9, num8 - dj, num7);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9 - di, num8 - dj, num7);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9, num8, num7 - dk);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9 - di, num8, num7 - dk);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9, num8 - dj, num7 - dk);
						this.check_neighbour(phi, closest_tri, ref vector3d, num9, num8, num7, num9 - di, num8 - dj, num7 - dk);
					}
				}
			}
		}

		private void check_neighbour(DenseGrid3f phi, DenseGrid3i closest_tri, ref Vector3d gx, int i0, int j0, int k0, int i1, int j1, int k1)
		{
			if (closest_tri[i1, j1, k1] >= 0)
			{
				Vector3d vector3d = Vector3f.Zero;
				Vector3d vector3d2 = Vector3f.Zero;
				Vector3d vector3d3 = Vector3f.Zero;
				this.Mesh.GetTriVertices(closest_tri[i1, j1, k1], ref vector3d, ref vector3d2, ref vector3d3);
				float num = (float)MeshSignedDistanceGrid.point_triangle_distance(ref gx, ref vector3d, ref vector3d2, ref vector3d3);
				if (num < phi[i0, j0, k0])
				{
					phi[i0, j0, k0] = num;
					closest_tri[i0, j0, k0] = closest_tri[i1, j1, k1];
				}
			}
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
				if (this.InsideMode == MeshSignedDistanceGrid.InsideModes.ParityCount)
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
						if (MeshSignedDistanceGrid.point_in_triangle_2d((double)j, (double)i, num2, num3, num5, num6, num8, num9, out num14, out num15, out num16))
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
			if (this.InsideMode == MeshSignedDistanceGrid.InsideModes.ParityCount)
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

		public static float point_segment_distance(ref Vector3f x0, ref Vector3f x1, ref Vector3f x2)
		{
			Vector3f vector3f = x2 - x1;
			float lengthSquared = vector3f.LengthSquared;
			float num = vector3f.Dot(x2 - x0) / lengthSquared;
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			return x0.Distance(num * x1 + (1f - num) * x2);
		}

		public static double point_segment_distance(ref Vector3d x0, ref Vector3d x1, ref Vector3d x2)
		{
			Vector3d vector3d = x2 - x1;
			double lengthSquared = vector3d.LengthSquared;
			double num = vector3d.Dot(x2 - x0) / lengthSquared;
			if (num < 0.0)
			{
				num = 0.0;
			}
			else if (num > 1.0)
			{
				num = 1.0;
			}
			return x0.Distance(num * x1 + (1.0 - num) * x2);
		}

		public static float point_triangle_distance(ref Vector3f x0, ref Vector3f x1, ref Vector3f x2, ref Vector3f x3)
		{
			Vector3f vector3f = x1 - x3;
			Vector3f v = x2 - x3;
			Vector3f v2 = x0 - x3;
			float lengthSquared = vector3f.LengthSquared;
			float lengthSquared2 = v.LengthSquared;
			float num = vector3f.Dot(v);
			float num2 = 1f / Math.Max(lengthSquared * lengthSquared2 - num * num, 1E-30f);
			float num3 = vector3f.Dot(v2);
			float num4 = v.Dot(v2);
			float num5 = num2 * (lengthSquared2 * num3 - num * num4);
			float num6 = num2 * (lengthSquared * num4 - num * num3);
			float num7 = 1f - num5 - num6;
			if (num5 >= 0f && num6 >= 0f && num7 >= 0f)
			{
				return x0.Distance(num5 * x1 + num6 * x2 + num7 * x3);
			}
			if (num5 > 0f)
			{
				return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x2), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x3));
			}
			if (num6 > 0f)
			{
				return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x2), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x2, ref x3));
			}
			return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x3), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x2, ref x3));
		}

		public static double point_triangle_distance(ref Vector3d x0, ref Vector3d x1, ref Vector3d x2, ref Vector3d x3)
		{
			Vector3d vector3d = x1 - x3;
			Vector3d vector3d2 = x2 - x3;
			Vector3d vector3d3 = x0 - x3;
			double lengthSquared = vector3d.LengthSquared;
			double lengthSquared2 = vector3d2.LengthSquared;
			double num = vector3d.Dot(ref vector3d2);
			double num2 = 1.0 / Math.Max(lengthSquared * lengthSquared2 - num * num, 1E-30);
			double num3 = vector3d.Dot(ref vector3d3);
			double num4 = vector3d2.Dot(ref vector3d3);
			double num5 = num2 * (lengthSquared2 * num3 - num * num4);
			double num6 = num2 * (lengthSquared * num4 - num * num3);
			double num7 = 1.0 - num5 - num6;
			if (num5 >= 0.0 && num6 >= 0.0 && num7 >= 0.0)
			{
				return x0.Distance(num5 * x1 + num6 * x2 + num7 * x3);
			}
			if (num5 > 0.0)
			{
				return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x2), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x3));
			}
			if (num6 > 0.0)
			{
				return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x2), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x2, ref x3));
			}
			return Math.Min(MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x1, ref x3), MeshSignedDistanceGrid.point_segment_distance(ref x0, ref x2, ref x3));
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
			int num = MeshSignedDistanceGrid.orientation(x2, y2, x3, y3, out a);
			if (num == 0)
			{
				return false;
			}
			if (MeshSignedDistanceGrid.orientation(x3, y3, x1, y1, out b) != num)
			{
				return false;
			}
			if (MeshSignedDistanceGrid.orientation(x1, y1, x2, y2, out c) != num)
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

		public int ExactBandWidth = 1;

		public Vector3d ExpandBounds = Vector3d.Zero;

		public bool UseParallel = true;

		public MeshSignedDistanceGrid.ComputeModes ComputeMode = MeshSignedDistanceGrid.ComputeModes.NarrowBandOnly;

		public double NarrowBandMaxDistance;

		public bool ComputeSigns = true;

		public MeshSignedDistanceGrid.InsideModes InsideMode = MeshSignedDistanceGrid.InsideModes.ParityCount;

		public bool WantClosestTriGrid;

		public bool WantIntersectionsGrid;

		public Func<bool> CancelF = () => false;

		public bool DebugPrint;

		private Vector3f grid_origin;

		private DenseGrid3f grid;

		private DenseGrid3i closest_tri_grid;

		private DenseGrid3i intersections_grid;

		public enum ComputeModes
		{
			FullGrid,
			NarrowBandOnly,
			NarrowBand_SpatialFloodFill
		}

		public enum InsideModes
		{
			CrossingCount,
			ParityCount
		}
	}
}
