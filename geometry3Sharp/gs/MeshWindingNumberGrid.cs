using System;
using System.Collections.Generic;
using System.Threading;
using g3;

namespace gs
{
	public class MeshWindingNumberGrid
	{
		public MeshWindingNumberGrid(DMesh3 mesh, DMeshAABBTree3 spatial, double cellSize)
		{
			this.Mesh = mesh;
			this.MeshSpatial = spatial;
			this.CellSize = cellSize;
		}

		public void Compute()
		{
			AxisAlignedBox3d cachedBounds = this.Mesh.CachedBounds;
			float f = (float)(2 * this.BufferCells) * (float)this.CellSize;
			this.grid_origin = (Vector3f)cachedBounds.Min - f * Vector3f.One;
			Vector3f vector3f = (Vector3f)cachedBounds.Max + f * Vector3f.One;
			int ni = (int)((vector3f.x - this.grid_origin.x) / (float)this.CellSize) + 1;
			int nj = (int)((vector3f.y - this.grid_origin.y) / (float)this.CellSize) + 1;
			int nk = (int)((vector3f.z - this.grid_origin.z) / (float)this.CellSize) + 1;
			this.winding_grid = new DenseGrid3f();
			if (this.ComputeMode == MeshWindingNumberGrid.ComputeModes.FullGrid)
			{
				this.make_grid_dense(this.grid_origin, (float)this.CellSize, ni, nj, nk, this.winding_grid);
				return;
			}
			this.make_grid(this.grid_origin, (float)this.CellSize, ni, nj, nk, this.winding_grid);
		}

		public Vector3i Dimensions
		{
			get
			{
				return new Vector3i(this.winding_grid.ni, this.winding_grid.nj, this.winding_grid.nk);
			}
		}

		public DenseGrid3f Grid
		{
			get
			{
				return this.winding_grid;
			}
		}

		public Vector3f GridOrigin
		{
			get
			{
				return this.grid_origin;
			}
		}

		public MeshSignedDistanceGrid SDFGrid
		{
			get
			{
				return this.mesh_sdf;
			}
		}

		public float this[int i, int j, int k]
		{
			get
			{
				return this.winding_grid[i, j, k];
			}
		}

		public Vector3f CellCenter(int i, int j, int k)
		{
			return new Vector3f((double)((float)i) * this.CellSize + (double)this.grid_origin.x, (double)((float)j) * this.CellSize + (double)this.grid_origin.y, (double)((float)k) * this.CellSize + (double)this.grid_origin.z);
		}

		private void make_grid(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f winding)
		{
			MeshWindingNumberGrid.<>c__DisplayClass26_0 CS$<>8__locals1 = new MeshWindingNumberGrid.<>c__DisplayClass26_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dx = dx;
			CS$<>8__locals1.origin = origin;
			CS$<>8__locals1.winding = winding;
			CS$<>8__locals1.ni = ni;
			CS$<>8__locals1.winding.resize(CS$<>8__locals1.ni, nj, nk);
			CS$<>8__locals1.winding.assign(float.MaxValue);
			this.MeshSpatial.WindingNumber(Vector3d.Zero);
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			MeshSignedDistanceGrid meshSignedDistanceGrid = new MeshSignedDistanceGrid(this.Mesh, this.CellSize, null)
			{
				ComputeSigns = false
			};
			meshSignedDistanceGrid.CancelF = this.CancelF;
			meshSignedDistanceGrid.Compute();
			if (this.CancelF())
			{
				return;
			}
			CS$<>8__locals1.distances = meshSignedDistanceGrid.Grid;
			if (this.WantMeshSDFGrid)
			{
				this.mesh_sdf = meshSignedDistanceGrid;
			}
			if (this.DebugPrint)
			{
				Console.WriteLine("done initial sdf");
			}
			float num = CS$<>8__locals1.origin[0];
			float num2 = CS$<>8__locals1.origin[1];
			float num3 = CS$<>8__locals1.origin[2];
			gParallel.ForEach<Vector3i>(gIndices.Grid3IndicesYZ(nj, nk), delegate(Vector3i jk)
			{
				if (CS$<>8__locals1.<>4__this.CancelF())
				{
					return;
				}
				for (int i = 0; i < CS$<>8__locals1.ni; i++)
				{
					Vector3i vector3i2 = new Vector3i(i, jk.y, jk.z);
					if ((double)CS$<>8__locals1.distances[vector3i2] < CS$<>8__locals1.<>4__this.CellSize)
					{
						Vector3d p = new Vector3d((double)((float)vector3i2.x * CS$<>8__locals1.dx + CS$<>8__locals1.origin[0]), (double)((float)vector3i2.y * CS$<>8__locals1.dx + CS$<>8__locals1.origin[1]), (double)((float)vector3i2.z * CS$<>8__locals1.dx + CS$<>8__locals1.origin[2]));
						CS$<>8__locals1.winding[vector3i2] = (float)CS$<>8__locals1.<>4__this.MeshSpatial.WindingNumber(p);
					}
				}
			});
			if (this.CancelF())
			{
				return;
			}
			if (this.DebugPrint)
			{
				Console.WriteLine("done narrow-band");
			}
			CS$<>8__locals1.bounds = CS$<>8__locals1.winding.Bounds;
			MeshWindingNumberGrid.<>c__DisplayClass26_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.bounds.Max = CS$<>8__locals2.bounds.Max - Vector3i.One;
			CS$<>8__locals1.bits = new Bitmap3(new Vector3i(CS$<>8__locals1.ni, nj, nk));
			List<Vector3i> list = new List<Vector3i>();
			foreach (Vector3i vector3i in CS$<>8__locals1.winding.Indices())
			{
				if (CS$<>8__locals1.winding[vector3i] != 3.4028235E+38f)
				{
					list.Add(vector3i);
					CS$<>8__locals1.bits[vector3i] = true;
				}
			}
			if (this.CancelF())
			{
				return;
			}
			CS$<>8__locals1.queue = new HashSet<Vector3i>();
			CS$<>8__locals1.queue_lock = default(SpinLock);
			for (;;)
			{
				if (this.CancelF())
				{
					break;
				}
				bool abort = false;
				int iter_count = 0;
				gParallel.ForEach<Vector3i>(list, delegate(Vector3i ijk)
				{
					Interlocked.Increment(ref iter_count);
					if (iter_count % 100 == 0)
					{
						abort = CS$<>8__locals1.<>4__this.CancelF();
					}
					if (abort)
					{
						return;
					}
					float num5 = CS$<>8__locals1.winding[ijk];
					for (int i = 0; i < 26; i++)
					{
						Vector3i vector3i2 = ijk + gIndices.GridOffsets26[i];
						if (CS$<>8__locals1.bounds.Contains(vector3i2))
						{
							float num6 = CS$<>8__locals1.winding[vector3i2];
							if (num6 == 3.4028235E+38f)
							{
								Vector3d p = new Vector3d((double)((float)vector3i2.x * CS$<>8__locals1.dx + CS$<>8__locals1.origin[0]), (double)((float)vector3i2.y * CS$<>8__locals1.dx + CS$<>8__locals1.origin[1]), (double)((float)vector3i2.z * CS$<>8__locals1.dx + CS$<>8__locals1.origin[2]));
								num6 = (float)CS$<>8__locals1.<>4__this.MeshSpatial.WindingNumber(p);
								CS$<>8__locals1.winding[vector3i2] = num6;
							}
							if (!CS$<>8__locals1.bits[vector3i2] && ((num5 < CS$<>8__locals1.<>4__this.WindingIsoValue && num6 > CS$<>8__locals1.<>4__this.WindingIsoValue) || (num5 > CS$<>8__locals1.<>4__this.WindingIsoValue && num6 < CS$<>8__locals1.<>4__this.WindingIsoValue)))
							{
								bool flag = false;
								CS$<>8__locals1.queue_lock.Enter(ref flag);
								CS$<>8__locals1.queue.Add(vector3i2);
								CS$<>8__locals1.queue_lock.Exit();
							}
						}
					}
				});
				if (this.DebugPrint)
				{
					Console.WriteLine("front has {0} voxels", CS$<>8__locals1.queue.Count);
				}
				if (CS$<>8__locals1.queue.Count == 0)
				{
					goto IL_2FD;
				}
				foreach (Vector3i idx in CS$<>8__locals1.queue)
				{
					CS$<>8__locals1.bits[idx] = true;
				}
				list.Clear();
				list.AddRange(CS$<>8__locals1.queue);
				CS$<>8__locals1.queue.Clear();
			}
			return;
			IL_2FD:
			if (this.DebugPrint)
			{
				Console.WriteLine("done front-prop");
			}
			if (this.DebugPrint)
			{
				int num4 = 0;
				foreach (Vector3i ijk2 in CS$<>8__locals1.winding.Indices())
				{
					if (CS$<>8__locals1.winding[ijk2] != 3.4028235E+38f)
					{
						num4++;
					}
				}
				Console.WriteLine("filled: {0} / {1}  -  {2}%", num4, CS$<>8__locals1.ni * nj * nk, (double)num4 / (double)(CS$<>8__locals1.ni * nj * nk) * 100.0);
			}
			if (this.CancelF())
			{
				return;
			}
			this.fill_spans(CS$<>8__locals1.ni, nj, nk, CS$<>8__locals1.winding);
			if (this.DebugPrint)
			{
				Console.WriteLine("done sweep");
			}
		}

		private void make_grid_dense(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f winding)
		{
			winding.resize(ni, nj, nk);
			this.MeshSpatial.WindingNumber(Vector3d.Zero);
			bool abort = false;
			int count = 0;
			gParallel.ForEach<Vector3i>(winding.Indices(), delegate(Vector3i ijk)
			{
				Interlocked.Increment(ref count);
				if (count % 100 == 0)
				{
					abort = this.CancelF();
				}
				if (abort)
				{
					return;
				}
				Vector3d p = new Vector3d((double)((float)ijk.x * dx + origin[0]), (double)((float)ijk.y * dx + origin[1]), (double)((float)ijk.z * dx + origin[2]));
				winding[ijk] = (float)this.MeshSpatial.WindingNumber(p);
			});
		}

		private void fill_spans(int ni, int nj, int nk, DenseGrid3f winding)
		{
			gParallel.ForEach<Vector3i>(gIndices.Grid3IndicesYZ(nj, nk), delegate(Vector3i idx)
			{
				int y = idx.y;
				int z = idx.z;
				float num = winding[0, y, z];
				if (num == 3.4028235E+38f)
				{
					num = 0f;
				}
				for (int i = 0; i < ni; i++)
				{
					if (winding[i, y, z] == 3.4028235E+38f)
					{
						winding[i, y, z] = num;
					}
					else
					{
						num = winding[i, y, z];
						if (num < this.WindingIsoValue)
						{
							num = 0f;
						}
					}
				}
			});
		}

		public DMesh3 Mesh;

		public DMeshAABBTree3 MeshSpatial;

		public double CellSize;

		public int BufferCells = 1;

		public MeshWindingNumberGrid.ComputeModes ComputeMode = MeshWindingNumberGrid.ComputeModes.NarrowBand;

		public float WindingIsoValue = 0.5f;

		public bool WantMeshSDFGrid = true;

		public Func<bool> CancelF = () => false;

		public bool DebugPrint;

		private Vector3f grid_origin;

		private DenseGrid3f winding_grid;

		private MeshSignedDistanceGrid mesh_sdf;

		public enum ComputeModes
		{
			FullGrid,
			NarrowBand
		}
	}
}
