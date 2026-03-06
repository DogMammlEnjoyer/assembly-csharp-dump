using System;
using System.Collections.Generic;
using System.Threading;
using g3;

namespace gs
{
	public class MeshScalarSamplingGrid
	{
		public MeshScalarSamplingGrid(DMesh3 mesh, double cellSize, Func<Vector3d, double> scalarF)
		{
			this.Mesh = mesh;
			this.ScalarF = scalarF;
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
			this.scalar_grid = new DenseGrid3f();
			if (this.ComputeMode == MeshScalarSamplingGrid.ComputeModes.FullGrid)
			{
				this.make_grid_dense(this.grid_origin, (float)this.CellSize, ni, nj, nk, this.scalar_grid);
				return;
			}
			this.make_grid(this.grid_origin, (float)this.CellSize, ni, nj, nk, this.scalar_grid);
		}

		public Vector3i Dimensions
		{
			get
			{
				return new Vector3i(this.scalar_grid.ni, this.scalar_grid.nj, this.scalar_grid.nk);
			}
		}

		public DenseGrid3f Grid
		{
			get
			{
				return this.scalar_grid;
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
				return this.scalar_grid[i, j, k];
			}
		}

		public Vector3f CellCenter(int i, int j, int k)
		{
			return new Vector3f((double)((float)i) * this.CellSize + (double)this.grid_origin.x, (double)((float)j) * this.CellSize + (double)this.grid_origin.y, (double)((float)k) * this.CellSize + (double)this.grid_origin.z);
		}

		private void make_grid(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f scalars)
		{
			MeshScalarSamplingGrid.<>c__DisplayClass26_0 CS$<>8__locals1 = new MeshScalarSamplingGrid.<>c__DisplayClass26_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.dx = dx;
			CS$<>8__locals1.origin = origin;
			CS$<>8__locals1.scalars = scalars;
			CS$<>8__locals1.ni = ni;
			CS$<>8__locals1.scalars.resize(CS$<>8__locals1.ni, nj, nk);
			CS$<>8__locals1.scalars.assign(float.MaxValue);
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
						Vector3d arg = new Vector3d((double)((float)vector3i2.x * CS$<>8__locals1.dx + CS$<>8__locals1.origin[0]), (double)((float)vector3i2.y * CS$<>8__locals1.dx + CS$<>8__locals1.origin[1]), (double)((float)vector3i2.z * CS$<>8__locals1.dx + CS$<>8__locals1.origin[2]));
						CS$<>8__locals1.scalars[vector3i2] = (float)CS$<>8__locals1.<>4__this.ScalarF(arg);
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
			CS$<>8__locals1.bounds = CS$<>8__locals1.scalars.Bounds;
			MeshScalarSamplingGrid.<>c__DisplayClass26_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.bounds.Max = CS$<>8__locals2.bounds.Max - Vector3i.One;
			CS$<>8__locals1.bits = new Bitmap3(new Vector3i(CS$<>8__locals1.ni, nj, nk));
			List<Vector3i> list = new List<Vector3i>();
			foreach (Vector3i vector3i in CS$<>8__locals1.scalars.Indices())
			{
				if (CS$<>8__locals1.scalars[vector3i] != 3.4028235E+38f)
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
					float num5 = CS$<>8__locals1.scalars[ijk];
					for (int i = 0; i < 26; i++)
					{
						Vector3i vector3i2 = ijk + gIndices.GridOffsets26[i];
						if (CS$<>8__locals1.bounds.Contains(vector3i2))
						{
							float num6 = CS$<>8__locals1.scalars[vector3i2];
							if (num6 == 3.4028235E+38f)
							{
								Vector3d arg = new Vector3d((double)((float)vector3i2.x * CS$<>8__locals1.dx + CS$<>8__locals1.origin[0]), (double)((float)vector3i2.y * CS$<>8__locals1.dx + CS$<>8__locals1.origin[1]), (double)((float)vector3i2.z * CS$<>8__locals1.dx + CS$<>8__locals1.origin[2]));
								num6 = (float)CS$<>8__locals1.<>4__this.ScalarF(arg);
								CS$<>8__locals1.scalars[vector3i2] = num6;
							}
							if (!CS$<>8__locals1.bits[vector3i2] && ((num5 < CS$<>8__locals1.<>4__this.IsoValue && num6 > CS$<>8__locals1.<>4__this.IsoValue) || (num5 > CS$<>8__locals1.<>4__this.IsoValue && num6 < CS$<>8__locals1.<>4__this.IsoValue)))
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
					goto IL_2EC;
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
			IL_2EC:
			if (this.DebugPrint)
			{
				Console.WriteLine("done front-prop");
			}
			if (this.DebugPrint)
			{
				int num4 = 0;
				foreach (Vector3i ijk2 in CS$<>8__locals1.scalars.Indices())
				{
					if (CS$<>8__locals1.scalars[ijk2] != 3.4028235E+38f)
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
			this.fill_spans(CS$<>8__locals1.ni, nj, nk, CS$<>8__locals1.scalars);
			if (this.DebugPrint)
			{
				Console.WriteLine("done sweep");
			}
		}

		private void make_grid_dense(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f scalars)
		{
			scalars.resize(ni, nj, nk);
			bool abort = false;
			int count = 0;
			gParallel.ForEach<Vector3i>(scalars.Indices(), delegate(Vector3i ijk)
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
				Vector3d arg = new Vector3d((double)((float)ijk.x * dx + origin[0]), (double)((float)ijk.y * dx + origin[1]), (double)((float)ijk.z * dx + origin[2]));
				scalars[ijk] = (float)this.ScalarF(arg);
			});
		}

		private void fill_spans(int ni, int nj, int nk, DenseGrid3f scalars)
		{
			gParallel.ForEach<Vector3i>(gIndices.Grid3IndicesYZ(nj, nk), delegate(Vector3i idx)
			{
				int y = idx.y;
				int z = idx.z;
				float num = scalars[0, y, z];
				if (num == 3.4028235E+38f)
				{
					num = 0f;
				}
				for (int i = 0; i < ni; i++)
				{
					if (scalars[i, y, z] == 3.4028235E+38f)
					{
						scalars[i, y, z] = num;
					}
					else
					{
						num = scalars[i, y, z];
						if (num < this.IsoValue)
						{
							num = 0f;
						}
					}
				}
			});
		}

		public DMesh3 Mesh;

		public Func<Vector3d, double> ScalarF;

		public double CellSize;

		public int BufferCells = 1;

		public MeshScalarSamplingGrid.ComputeModes ComputeMode = MeshScalarSamplingGrid.ComputeModes.NarrowBand;

		public float IsoValue = 0.5f;

		public bool WantMeshSDFGrid = true;

		public Func<bool> CancelF = () => false;

		public bool DebugPrint;

		private Vector3f grid_origin;

		private DenseGrid3f scalar_grid;

		private MeshSignedDistanceGrid mesh_sdf;

		public enum ComputeModes
		{
			FullGrid,
			NarrowBand
		}
	}
}
