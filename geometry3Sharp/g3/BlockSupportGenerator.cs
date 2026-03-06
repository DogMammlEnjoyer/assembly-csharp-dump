using System;

namespace g3
{
	public class BlockSupportGenerator
	{
		public BlockSupportGenerator(DMesh3 mesh, double cellSize)
		{
			this.Mesh = mesh;
			this.CellSize = cellSize;
		}

		public BlockSupportGenerator(DMesh3 mesh, int grid_resolution)
		{
			this.Mesh = mesh;
			double num = Math.Max(this.Mesh.CachedBounds.Width, this.Mesh.CachedBounds.Height);
			this.CellSize = num / (double)grid_resolution;
		}

		public void Generate()
		{
			this.grid_bounds = this.Mesh.CachedBounds;
			if (this.ForceMinY != 3.4028235E+38f)
			{
				this.grid_bounds.Min.y = (double)this.ForceMinY;
			}
			float num = 2f * (float)this.CellSize;
			Vector3f v = new Vector3f(num, 0f, num);
			this.grid_origin = (Vector3f)this.grid_bounds.Min - v;
			this.grid_origin.y = this.grid_origin.y + (float)this.CellSize * 0.5f;
			Vector3f vector3f = (Vector3f)this.grid_bounds.Max + v;
			int ni = (int)((vector3f.x - this.grid_origin.x) / (float)this.CellSize) + 1;
			int nj = (int)((vector3f.y - this.grid_origin.y) / (float)this.CellSize) + 1;
			int nk = (int)((vector3f.z - this.grid_origin.z) / (float)this.CellSize) + 1;
			this.volume_grid = new DenseGrid3f();
			this.generate_support(this.grid_origin, (float)this.CellSize, ni, nj, nk, this.volume_grid);
		}

		public Vector3i Dimensions
		{
			get
			{
				return new Vector3i(this.volume_grid.ni, this.volume_grid.nj, this.volume_grid.nk);
			}
		}

		public DenseGrid3f Grid
		{
			get
			{
				return this.volume_grid;
			}
		}

		public Vector3f GridOrigin
		{
			get
			{
				return this.grid_origin;
			}
		}

		public float this[int i, int j, int k]
		{
			get
			{
				return this.volume_grid[i, j, k];
			}
		}

		public Vector3f CellCenter(int i, int j, int k)
		{
			return new Vector3f((double)((float)i) * this.CellSize + (double)this.grid_origin.x, (double)((float)j) * this.CellSize + (double)this.grid_origin.y, (double)((float)k) * this.CellSize + (double)this.grid_origin.z);
		}

		private void generate_support(Vector3f origin, float dx, int ni, int nj, int nk, DenseGrid3f supportGrid)
		{
			supportGrid.resize(ni, nj, nk);
			supportGrid.assign(1f);
			bool flag = false;
			int num = 1;
			if (this.SubtractMesh && this.SubtractMeshOffset > 0.0)
			{
				int val = (int)(this.SubtractMeshOffset / this.CellSize) + 1;
				num = Math.Max(num, val);
			}
			this.sdf = new MeshSignedDistanceGrid(this.Mesh, this.CellSize, null)
			{
				ComputeSigns = true,
				ExactBandWidth = num
			};
			this.sdf.CancelF = this.CancelF;
			this.sdf.Compute();
			if (this.CancelF())
			{
				return;
			}
			DenseGridTrilinearImplicit distanceField = new DenseGridTrilinearImplicit(this.sdf.Grid, this.sdf.GridOrigin, (double)this.sdf.CellSize);
			double num2 = Math.Cos(MathUtil.Clamp(this.OverhangAngleDeg, 0.01, 89.99) * 0.017453292519943295);
			double num3 = (double)dx;
			double num4 = (double)origin[0];
			double num5 = (double)origin[1];
			double num6 = (double)origin[2];
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			foreach (int num7 in this.Mesh.TriangleIndices())
			{
				if (num7 % 100 == 0 && this.CancelF())
				{
					break;
				}
				this.Mesh.GetTriVertices(num7, ref zero, ref zero2, ref zero3);
				if (MathUtil.Normal(ref zero, ref zero2, ref zero3).Dot(-Vector3d.AxisY) >= num2)
				{
					double a = (zero[0] - num4) / num3;
					double a2 = (zero[1] - num5) / num3;
					double a3 = (zero[2] - num6) / num3;
					double b = (zero2[0] - num4) / num3;
					double b2 = (zero2[1] - num5) / num3;
					double b3 = (zero2[2] - num6) / num3;
					double c = (zero3[0] - num4) / num3;
					double c2 = (zero3[1] - num5) / num3;
					double c3 = (zero3[2] - num6) / num3;
					int num8 = 0;
					int num9 = MathUtil.Clamp((int)MathUtil.Min(a, b, c) - num8, 0, ni - 1);
					int num10 = MathUtil.Clamp((int)MathUtil.Max(a, b, c) + num8 + 1, 0, ni - 1);
					int num11 = MathUtil.Clamp((int)MathUtil.Min(a2, b2, c2) - num8, 0, nj - 1);
					int num12 = MathUtil.Clamp((int)MathUtil.Max(a2, b2, c2) + num8 + 1, 0, nj - 1);
					int num13 = MathUtil.Clamp((int)MathUtil.Min(a3, b3, c3) - num8, 0, nk - 1);
					int num14 = MathUtil.Clamp((int)MathUtil.Max(a3, b3, c3) + num8 + 1, 0, nk - 1);
					for (int i = num13; i <= num14; i++)
					{
						for (int j = num11; j <= num12; j++)
						{
							int k = num9;
							while (k <= num10)
							{
								Vector3d vector3d = new Vector3d((double)((float)k * dx + origin[0]), (double)((float)j * dx + origin[1]), (double)((float)i * dx + origin[2]));
								float num15 = (float)MeshSignedDistanceGrid.point_triangle_distance(ref vector3d, ref zero, ref zero2, ref zero3);
								if (!flag)
								{
									goto IL_34B;
								}
								int num16 = (i % 2 == 0) ? 1 : 0;
								if (k % 2 != num16)
								{
									goto IL_34B;
								}
								IL_368:
								k++;
								continue;
								IL_34B:
								if (num15 < dx / 2f)
								{
									supportGrid[k, j, i] = -1f;
									goto IL_368;
								}
								goto IL_368;
							}
						}
					}
				}
			}
			if (this.CancelF())
			{
				return;
			}
			this.fill_vertical_spans(supportGrid, distanceField);
			this.generate_mesh(supportGrid, distanceField);
		}

		private Vector3d get_cell_center(Vector3i ijk)
		{
			return new Vector3d((double)ijk.x * this.CellSize, (double)ijk.y * this.CellSize, (double)ijk.z * this.CellSize) + this.GridOrigin;
		}

		private Vector3d get_cell_center(int i, int j, int k)
		{
			return new Vector3d((double)i * this.CellSize, (double)j * this.CellSize, (double)k * this.CellSize) + this.GridOrigin;
		}

		private void fill_vertical_spans(DenseGrid3f supportGrid, DenseGridTrilinearImplicit distanceField)
		{
			int ni = supportGrid.ni;
			int nj = supportGrid.nj;
			int nk = supportGrid.nk;
			double cellSize = this.CellSize;
			Vector3f gridOrigin = this.GridOrigin;
			for (int i = 0; i < nk; i++)
			{
				for (int j = 0; j < ni; j++)
				{
					bool flag = false;
					for (int k = nj - 1; k >= 0; k--)
					{
						if (supportGrid[j, k, i] >= 0f)
						{
							Vector3d vector3d = this.get_cell_center(j, k, i);
							if (flag)
							{
								if (distanceField.Value(ref vector3d) < 0.0)
								{
									supportGrid[j, k, i] = -3f;
									flag = false;
								}
								else
								{
									supportGrid[j, k, i] = -1f;
								}
							}
						}
						else
						{
							flag = true;
						}
					}
				}
			}
		}

		private void generate_mesh(DenseGrid3f supportGrid, DenseGridTrilinearImplicit distanceField)
		{
			DenseGridTrilinearImplicit denseGridTrilinearImplicit = new DenseGridTrilinearImplicit(supportGrid, this.GridOrigin, this.CellSize);
			BoundedImplicitFunction3d a = denseGridTrilinearImplicit;
			if (this.SubtractMesh)
			{
				BoundedImplicitFunction3d b = distanceField;
				if (this.SubtractMeshOffset > 0.0)
				{
					b = new ImplicitOffset3d
					{
						A = distanceField,
						Offset = this.SubtractMeshOffset
					};
				}
				a = new ImplicitDifference3d
				{
					A = denseGridTrilinearImplicit,
					B = b
				};
			}
			ImplicitHalfSpace3d b2 = new ImplicitHalfSpace3d
			{
				Origin = Vector3d.Zero,
				Normal = Vector3d.AxisY
			};
			ImplicitDifference3d @implicit = new ImplicitDifference3d
			{
				A = a,
				B = b2
			};
			MarchingCubes marchingCubes = new MarchingCubes
			{
				Implicit = @implicit,
				Bounds = this.grid_bounds,
				CubeSize = this.CellSize
			};
			marchingCubes.Bounds.Min.y = -2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes2 = marchingCubes;
			marchingCubes2.Bounds.Min.x = marchingCubes2.Bounds.Min.x - 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes3 = marchingCubes;
			marchingCubes3.Bounds.Min.z = marchingCubes3.Bounds.Min.z - 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes4 = marchingCubes;
			marchingCubes4.Bounds.Max.x = marchingCubes4.Bounds.Max.x + 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes5 = marchingCubes;
			marchingCubes5.Bounds.Max.z = marchingCubes5.Bounds.Max.z + 2.0 * marchingCubes.CubeSize;
			marchingCubes.Generate();
			this.SupportMesh = marchingCubes.Mesh;
		}

		public DMesh3 Mesh;

		public double CellSize;

		public double OverhangAngleDeg = 30.0;

		public float ForceMinY = float.MaxValue;

		public bool SubtractMesh;

		public double SubtractMeshOffset = 0.05;

		public Func<bool> CancelF = () => false;

		public bool DebugPrint;

		private AxisAlignedBox3d grid_bounds;

		private Vector3f grid_origin;

		private DenseGrid3f volume_grid;

		private MeshSignedDistanceGrid sdf;

		public DMesh3 SupportMesh;

		private const float SUPPORT_TIP_TOP = -1f;
	}
}
