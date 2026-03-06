using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class GraphSupportGenerator
	{
		protected virtual bool Cancelled()
		{
			return this.Progress != null && this.Progress.Cancelled();
		}

		public GraphSupportGenerator(DMesh3 mesh, DMeshAABBTree3 spatial, double cellSize)
		{
			this.Mesh = mesh;
			this.MeshSpatial = spatial;
			this.CellSize = cellSize;
		}

		public GraphSupportGenerator(DMesh3 mesh, DMeshAABBTree3 spatial, int grid_resolution)
		{
			this.Mesh = mesh;
			this.MeshSpatial = spatial;
			double num = Math.Max(this.Mesh.CachedBounds.Width, this.Mesh.CachedBounds.Height);
			this.CellSize = num / (double)grid_resolution;
		}

		public void Generate()
		{
			AxisAlignedBox3d cachedBounds = this.Mesh.CachedBounds;
			if (this.ForceMinY != 3.4028235E+38f)
			{
				cachedBounds.Min.y = (double)this.ForceMinY;
			}
			float num = 2f * (float)this.CellSize;
			Vector3f v = new Vector3f(num, 0f, num);
			this.grid_origin = (Vector3f)cachedBounds.Min - v;
			this.grid_origin.y = this.grid_origin.y + (float)this.CellSize * 0.5f;
			Vector3f vector3f = (Vector3f)cachedBounds.Max + v;
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
			if (this.DebugPrint)
			{
				Console.WriteLine("start");
			}
			bool flag = false;
			Console.WriteLine("Computing SDF");
			MeshSignedDistanceGrid meshSignedDistanceGrid = new MeshSignedDistanceGrid(this.Mesh, this.CellSize, null)
			{
				ComputeSigns = true,
				ExactBandWidth = 3
			};
			meshSignedDistanceGrid.CancelF = new Func<bool>(this.Cancelled);
			meshSignedDistanceGrid.Compute();
			if (this.Cancelled())
			{
				return;
			}
			DenseGridTrilinearImplicit distanceField = new DenseGridTrilinearImplicit(meshSignedDistanceGrid.Grid, meshSignedDistanceGrid.GridOrigin, (double)meshSignedDistanceGrid.CellSize);
			double num = Math.Cos(MathUtil.Clamp(this.OverhangAngleDeg, 0.01, 89.99) * 0.017453292519943295);
			Console.WriteLine("Marking overhangs");
			double num2 = (double)dx;
			double num3 = (double)origin[0];
			double num4 = (double)origin[1];
			double num5 = (double)origin[2];
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			foreach (int num6 in this.Mesh.TriangleIndices())
			{
				if (num6 % 100 == 0 && this.Cancelled())
				{
					break;
				}
				this.Mesh.GetTriVertices(num6, ref zero, ref zero2, ref zero3);
				if (MathUtil.Normal(ref zero, ref zero2, ref zero3).Dot(-Vector3d.AxisY) >= num)
				{
					double a = (zero[0] - num3) / num2;
					double a2 = (zero[1] - num4) / num2;
					double a3 = (zero[2] - num5) / num2;
					double b = (zero2[0] - num3) / num2;
					double b2 = (zero2[1] - num4) / num2;
					double b3 = (zero2[2] - num5) / num2;
					double c = (zero3[0] - num3) / num2;
					double c2 = (zero3[1] - num4) / num2;
					double c3 = (zero3[2] - num5) / num2;
					int num7 = 0;
					int num8 = MathUtil.Clamp((int)MathUtil.Min(a, b, c) - num7, 0, ni - 1);
					int num9 = MathUtil.Clamp((int)MathUtil.Max(a, b, c) + num7 + 1, 0, ni - 1);
					int num10 = MathUtil.Clamp((int)MathUtil.Min(a2, b2, c2) - num7, 0, nj - 1);
					int num11 = MathUtil.Clamp((int)MathUtil.Max(a2, b2, c2) + num7 + 1, 0, nj - 1);
					int num12 = MathUtil.Clamp((int)MathUtil.Min(a3, b3, c3) - num7, 0, nk - 1);
					int num13 = MathUtil.Clamp((int)MathUtil.Max(a3, b3, c3) + num7 + 1, 0, nk - 1);
					if (num10 == 0)
					{
						num10 = 1;
					}
					for (int i = num12; i <= num13; i++)
					{
						for (int j = num10; j <= num11; j++)
						{
							int k = num8;
							while (k <= num9)
							{
								Vector3d vector3d = new Vector3d((double)((float)k * dx + origin[0]), (double)((float)j * dx + origin[1]), (double)((float)i * dx + origin[2]));
								float num14 = (float)MeshSignedDistanceGrid.point_triangle_distance(ref vector3d, ref zero, ref zero2, ref zero3);
								if (!flag)
								{
									goto IL_324;
								}
								int num15 = (i % 2 == 0) ? 1 : 0;
								if (k % 2 != num15)
								{
									goto IL_324;
								}
								IL_36E:
								k++;
								continue;
								IL_324:
								if (num14 >= dx / 2f)
								{
									goto IL_36E;
								}
								if (j > 1)
								{
									supportGrid[k, j, i] = -2f;
									supportGrid[k, j - 1, i] = -3f;
									goto IL_36E;
								}
								supportGrid[k, j, i] = -3f;
								goto IL_36E;
							}
						}
					}
				}
			}
			if (this.Cancelled())
			{
				return;
			}
			this.generate_graph(supportGrid, distanceField);
			this.postprocess_graph();
		}

		private Vector3d get_cell_center(Vector3i ijk)
		{
			return new Vector3d((double)ijk.x * this.CellSize, (double)ijk.y * this.CellSize, (double)ijk.z * this.CellSize) + this.GridOrigin;
		}

		private void generate_graph(DenseGrid3f supportGrid, DenseGridTrilinearImplicit distanceField)
		{
			int ni = supportGrid.ni;
			int nj = supportGrid.nj;
			int nk = supportGrid.nk;
			float num = (float)this.CellSize;
			Vector3f gridOrigin = this.GridOrigin;
			float num2 = 0.01f;
			float num3 = 99999f;
			bool flag = true;
			float num4 = 10f * (float)this.CellSize;
			Vector3i center_idx = new Vector3i(ni / 2, 0, nk / 2);
			bool flag2 = true;
			DenseGrid3f costGrid = new DenseGrid3f(supportGrid);
			foreach (Vector3i vector3i in costGrid.Indices())
			{
				Vector3d vector3d = new Vector3f((float)vector3i.x * num, (float)vector3i.y * num, (float)vector3i.z * num) + gridOrigin;
				float num5 = (float)distanceField.Value(ref vector3d);
				if (num5 <= num2)
				{
					num5 = num3;
				}
				else if (flag)
				{
					num5 = 1f;
				}
				else if (num5 > num4)
				{
					num5 = num4;
				}
				costGrid[vector3i] = num5;
			}
			List<Vector3i> list = new List<Vector3i>();
			List<Vector3i> list2 = new List<Vector3i>();
			Comparison<Vector3i> <>9__5;
			for (int i = 0; i < nj; i++)
			{
				list2.Clear();
				for (int j = 0; j < nk; j++)
				{
					for (int k = 0; k < ni; k++)
					{
						if (supportGrid[k, i, j] == -3f)
						{
							list2.Add(new Vector3i(k, i, j));
						}
					}
				}
				List<Vector3i> list3 = list2;
				Comparison<Vector3i> comparison;
				if ((comparison = <>9__5) == null)
				{
					comparison = (<>9__5 = delegate(Vector3i a, Vector3i b)
					{
						Vector3i v5 = a;
						v5.y = 0;
						Vector3i v6 = b;
						v6.y = 0;
						int lengthSquared = (v5 - center_idx).LengthSquared;
						int lengthSquared2 = (v6 - center_idx).LengthSquared;
						return lengthSquared.CompareTo(lengthSquared2);
					});
				}
				list3.Sort(comparison);
				if (flag2)
				{
					list2.Reverse();
				}
				list.AddRange(list2);
			}
			HashSet<Vector3i> seed_indices = new HashSet<Vector3i>(list);
			if (!this.ProcessBottomUp)
			{
				list.Reverse();
			}
			Func<int, bool> nodeFilterF = delegate(int a)
			{
				Vector3i vector3i6 = costGrid.to_index(a);
				return vector3i6.x > 0 && vector3i6.z > 0 && vector3i6.x != ni - 1 && vector3i6.y != nj - 1 && vector3i6.z != nk - 1;
			};
			Func<int, int, float> nodeDistanceF = delegate(int a, int b)
			{
				Vector3i vector3i6 = costGrid.to_index(a);
				Vector3i vector3i7 = costGrid.to_index(b);
				if (vector3i7.y >= vector3i6.y)
				{
					return float.MaxValue;
				}
				float num10 = supportGrid[vector3i7];
				if (num10 == -2f)
				{
					return float.MaxValue;
				}
				if (num10 < 0f)
				{
					return -999999f;
				}
				float num11 = costGrid[b];
				float num12 = (float)(Math.Sqrt((double)(vector3i7 - vector3i6).LengthSquared) * this.CellSize);
				return num11 + num12;
			};
			Func<int, IEnumerable<int>> neighboursF = delegate(int a)
			{
				Vector3i idx = costGrid.to_index(a);
				return this.down_neighbours(idx, costGrid);
			};
			Func<int, bool> terminatingNodeF = delegate(int a)
			{
				Vector3i vector3i6 = costGrid.to_index(a);
				return (!seed_indices.Contains(vector3i6) && supportGrid[vector3i6] < 0f) || vector3i6.y == 0;
			};
			DijkstraGraphDistance dijkstraGraphDistance = new DijkstraGraphDistance(ni * nj * nk, false, nodeFilterF, nodeDistanceF, neighboursF, null);
			dijkstraGraphDistance.TrackOrder = true;
			List<int> list4 = new List<int>();
			this.Graph = new DGraph3();
			Dictionary<Vector3i, int> dictionary = new Dictionary<Vector3i, int>();
			this.TipVertices = new HashSet<int>();
			this.TipBaseVertices = new HashSet<int>();
			this.GroundVertices = new HashSet<int>();
			for (int l = 0; l < list.Count; l++)
			{
				int id = costGrid.to_linear(list[l]);
				dijkstraGraphDistance.Reset();
				dijkstraGraphDistance.AddSeed(id, 0f);
				int num6 = dijkstraGraphDistance.ComputeToNode(terminatingNodeF, float.MaxValue);
				if (num6 < 0)
				{
					num6 = dijkstraGraphDistance.GetOrder().Last<int>();
				}
				list4.Clear();
				dijkstraGraphDistance.GetPathToSeed(num6, list4);
				int count = list4.Count;
				Vector3i vector3i2 = supportGrid.to_index(list4[0]);
				int num7;
				if (!dictionary.TryGetValue(vector3i2, out num7))
				{
					Vector3d v = this.get_cell_center(vector3i2);
					if (vector3i2.y == 0)
					{
						v.y = 0.0;
					}
					num7 = this.Graph.AppendVertex(v);
					if (vector3i2.y == 0)
					{
						this.GroundVertices.Add(num7);
					}
					dictionary[vector3i2] = num7;
				}
				int v2 = num7;
				for (int m = 0; m < count; m++)
				{
					int i2 = list4[m];
					if (supportGrid[i2] >= 0f)
					{
						supportGrid[i2] = -1f;
					}
					if (m > 0)
					{
						Vector3i vector3i3 = supportGrid.to_index(list4[m]);
						int num8;
						if (!dictionary.TryGetValue(vector3i3, out num8))
						{
							Vector3d v3 = this.get_cell_center(vector3i3);
							num8 = this.Graph.AppendVertex(v3);
							dictionary[vector3i3] = num8;
						}
						this.Graph.AppendEdge(v2, num8, -1);
						v2 = num8;
					}
				}
				if (supportGrid[list4[count - 1]] == -3f)
				{
					Vector3i vector3i4 = supportGrid.to_index(list4[count - 1]);
					this.TipBaseVertices.Add(dictionary[vector3i4]);
					Vector3i vector3i5 = vector3i4 + Vector3i.AxisY;
					int num9;
					if (!dictionary.TryGetValue(vector3i5, out num9))
					{
						Vector3d v4 = this.get_cell_center(vector3i5);
						num9 = this.Graph.AppendVertex(v4);
						dictionary[vector3i5] = num9;
						this.Graph.AppendEdge(v2, num9, -1);
						this.TipVertices.Add(num9);
					}
				}
			}
			gParallel.ForEach<int>(this.TipVertices, delegate(int tip_vid)
			{
				bool flag3 = false;
				Vector3d vector3d2 = this.Graph.GetVertex(tip_vid);
				Frame3f frame3f;
				if (MeshQueries.RayHitPointFrame(this.Mesh, this.MeshSpatial, new Ray3d(vector3d2, Vector3d.AxisY, false), out frame3f, false) && vector3d2.Distance(frame3f.Origin) < 2.0 * this.CellSize)
				{
					vector3d2 = frame3f.Origin;
					flag3 = true;
				}
				if (!flag3 && MeshQueries.RayHitPointFrame(this.Mesh, this.MeshSpatial, new Ray3d(vector3d2, -Vector3d.AxisY, false), out frame3f, false) && vector3d2.Distance(frame3f.Origin) < this.CellSize)
				{
					vector3d2 = frame3f.Origin;
					flag3 = true;
				}
				if (!flag3)
				{
					frame3f = MeshQueries.NearestPointFrame(this.Mesh, this.MeshSpatial, vector3d2, false);
					if (vector3d2.Distance(frame3f.Origin) < 2.0 * this.CellSize)
					{
						vector3d2 = frame3f.Origin;
						flag3 = true;
					}
				}
				if (flag3)
				{
					this.Graph.SetVertex(tip_vid, vector3d2);
				}
			});
		}

		protected DMesh3 MakeDebugGraphMesh()
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			dmesh.EnableVertexColors(Vector3f.One);
			foreach (int num in this.Graph.VertexIndices())
			{
				if (this.TipVertices.Contains(num))
				{
					MeshEditor.AppendBox(dmesh, this.Graph.GetVertex(num), 0.3f, Colorf.Green);
				}
				else if (this.TipBaseVertices.Contains(num))
				{
					MeshEditor.AppendBox(dmesh, this.Graph.GetVertex(num), 0.225f, Colorf.Magenta);
				}
				else if (this.GroundVertices.Contains(num))
				{
					MeshEditor.AppendBox(dmesh, this.Graph.GetVertex(num), 0.35f, Colorf.Blue);
				}
				else
				{
					MeshEditor.AppendBox(dmesh, this.Graph.GetVertex(num), 0.15f, Colorf.White);
				}
			}
			foreach (int eID in this.Graph.EdgeIndices())
			{
				Segment3d edgeSegment = this.Graph.GetEdgeSegment(eID);
				MeshEditor.AppendLine(dmesh, edgeSegment, 0.1f);
			}
			return dmesh;
		}

		protected virtual void postprocess_graph()
		{
			double num = MathUtil.Clamp(this.OptimizationAlpha, 0.0, 1.0);
			if (num == 0.0 || this.OptimizationRounds == 0)
			{
				return;
			}
			this.constrained_smooth(this.Graph, this.GraphSurfaceDistanceOffset, Math.Cos((90.0 - this.OverhangAngleOptimizeDeg) * 0.017453292519943295), num, this.OptimizationRounds);
		}

		private IEnumerable<int> down_neighbours(Vector3i idx, DenseGrid3f grid)
		{
			yield return grid.to_linear(idx.x, idx.y - 1, idx.z);
			yield return grid.to_linear(idx.x - 1, idx.y - 1, idx.z);
			yield return grid.to_linear(idx.x + 1, idx.y - 1, idx.z);
			yield return grid.to_linear(idx.x, idx.y - 1, idx.z - 1);
			yield return grid.to_linear(idx.x, idx.y - 1, idx.z + 1);
			yield return grid.to_linear(idx.x - 1, idx.y - 1, idx.z - 1);
			yield return grid.to_linear(idx.x + 1, idx.y - 1, idx.z - 1);
			yield return grid.to_linear(idx.x - 1, idx.y - 1, idx.z + 1);
			yield return grid.to_linear(idx.x + 1, idx.y - 1, idx.z + 1);
			yield break;
		}

		private void constrained_smooth(DGraph3 graph, double surfDist, double dotThresh, double alpha, int rounds)
		{
			int maxVertexID = graph.MaxVertexID;
			Vector3d[] pos = new Vector3d[maxVertexID];
			Action<int> <>9__0;
			for (int i = 0; i < rounds; i++)
			{
				IEnumerable<int> source = graph.VertexIndices();
				Action<int> body;
				if ((body = <>9__0) == null)
				{
					body = (<>9__0 = delegate(int vid)
					{
						Vector3d vertex = graph.GetVertex(vid);
						if (this.GroundVertices.Contains(vid) || this.TipVertices.Contains(vid))
						{
							pos[vid] = vertex;
							return;
						}
						if (this.TipBaseVertices.Contains(vid))
						{
							pos[vid] = vertex;
							return;
						}
						Vector3d vector3d = Vector3d.Zero;
						int num2 = 0;
						foreach (int vID in graph.VtxVerticesItr(vid))
						{
							vector3d += graph.GetVertex(vID);
							num2++;
						}
						if (num2 == 1)
						{
							pos[vid] = vertex;
							return;
						}
						vector3d /= (double)num2;
						Vector3d vector3d2 = (1.0 - alpha) * vertex + alpha * vector3d;
						int num3 = 0;
						for (;;)
						{
							IL_FD:
							foreach (int vID2 in graph.VtxVerticesItr(vid))
							{
								Vector3d vector3d3 = graph.GetVertex(vID2) - vector3d2;
								vector3d3.Normalize(2.220446049250313E-16);
								if (Math.Abs(vector3d3.Dot(Vector3d.AxisY)) < dotThresh)
								{
									if (num3++ < 3)
									{
										vector3d2 = Vector3d.Lerp(vertex, vector3d2, 0.66);
										goto IL_FD;
									}
									pos[vid] = vertex;
									return;
								}
							}
							break;
						}
						Frame3f frame3f = MeshQueries.NearestPointFrame(this.Mesh, this.MeshSpatial, vector3d2, true);
						Vector3d v = frame3f.Origin;
						double num4 = vector3d2.Distance(v);
						if (this.MeshSpatial.IsInside(vector3d2) || num4 < surfDist)
						{
							Vector3d v2 = frame3f.Z;
							if (v2.Dot(Vector3d.AxisY) < 0.0)
							{
								v2.y = 0.0;
								v2.Normalize(2.220446049250313E-16);
							}
							vector3d2 = frame3f.Origin + surfDist * v2;
						}
						pos[vid] = vector3d2;
					});
				}
				gParallel.ForEach<int>(source, body);
				foreach (int num in graph.VertexIndices())
				{
					graph.SetVertex(num, pos[num]);
				}
			}
		}

		private void process_version2(DenseGrid3f supportGrid, DenseGridTrilinearImplicit distanceField)
		{
			int ni = supportGrid.ni;
			int nj = supportGrid.nj;
			int nk = supportGrid.nk;
			float num = (float)this.CellSize;
			Vector3f gridOrigin = this.GridOrigin;
			DenseGrid2f denseGrid2f = supportGrid.get_slice(nj - 1, 1);
			DenseGrid2f tmp = new DenseGrid2f(denseGrid2f);
			Bitmap3 bitmap = new Bitmap3(new Vector3i(ni, nj, nk));
			for (int i = nj - 2; i >= 0; i--)
			{
				DenseGrid2i denseGrid2i = GraphSupportGenerator.binarize(denseGrid2f, 0f);
				GraphSupportGenerator.skeletonize(denseGrid2i, null, 2);
				if (i == 0)
				{
					GraphSupportGenerator.dilate(denseGrid2i, null, true);
					GraphSupportGenerator.dilate(denseGrid2i, null, true);
				}
				for (int j = 1; j < nk - 1; j++)
				{
					for (int k = 1; k < ni - 1; k++)
					{
						bitmap[new Vector3i(k, i, j)] = (denseGrid2i[k, j] == 1);
					}
				}
				GraphSupportGenerator.smooth(denseGrid2f, tmp, 0.5f, 5);
				DenseGrid2f denseGrid2f2 = supportGrid.get_slice(i, 1);
				denseGrid2f2.set_min(denseGrid2f);
				for (int l = 1; l < nk - 1; l++)
				{
					for (int m = 1; m < ni - 1; m++)
					{
						float val = (denseGrid2i[m, l] > 0) ? -1f : 2.1474836E+09f;
						denseGrid2f2[m, l] = Math.Min(denseGrid2f2[m, l], val);
						if (denseGrid2f2[m, l] < 0f)
						{
							Vector3d vector3d = new Vector3f((float)m * num, (float)i * num, (float)l * num) + gridOrigin;
							if (distanceField.Value(ref vector3d) < -this.CellSize)
							{
								denseGrid2f2[m, l] = 1f;
							}
						}
					}
				}
				for (int n = 1; n < nk - 1; n++)
				{
					for (int num2 = 1; num2 < ni - 1; num2++)
					{
						if (this.is_loner(denseGrid2i, num2, n))
						{
							foreach (Vector2i vector2i in gIndices.GridOffsets8)
							{
								float num4 = 1f / (float)Math.Sqrt((double)(vector2i.x * vector2i.x + vector2i.y * vector2i.y));
								DenseGrid2f denseGrid2f3 = denseGrid2f2;
								int i2 = num2 + vector2i.x;
								int j2 = n + vector2i.y;
								denseGrid2f3[i2, j2] += -0.25f * num4;
							}
						}
					}
				}
				for (int num5 = 1; num5 < nk - 1; num5++)
				{
					for (int num6 = 1; num6 < ni - 1; num6++)
					{
						supportGrid[num6, i, num5] = denseGrid2f2[num6, num5];
					}
				}
				denseGrid2f.swap(denseGrid2f2);
			}
			VoxelSurfaceGenerator voxelSurfaceGenerator = new VoxelSurfaceGenerator();
			voxelSurfaceGenerator.Voxels = bitmap;
			voxelSurfaceGenerator.Generate();
			Util.WriteDebugMesh(voxelSurfaceGenerator.Meshes[0], "c:\\scratch\\binary.obj");
		}

		private static DenseGrid2i binarize(DenseGrid2f grid, float thresh = 0f)
		{
			DenseGrid2i denseGrid2i = new DenseGrid2i();
			denseGrid2i.resize(grid.ni, grid.nj);
			int size = denseGrid2i.size;
			for (int i = 0; i < size; i++)
			{
				denseGrid2i[i] = ((grid[i] < thresh) ? 1 : 0);
			}
			return denseGrid2i;
		}

		private static DenseGrid3i binarize(DenseGrid3f grid, float thresh = 0f)
		{
			DenseGrid3i denseGrid3i = new DenseGrid3i();
			denseGrid3i.resize(grid.ni, grid.nj, grid.nk);
			int size = denseGrid3i.size;
			for (int i = 0; i < size; i++)
			{
				denseGrid3i[i] = ((grid[i] < thresh) ? 1 : 0);
			}
			return denseGrid3i;
		}

		private static void smooth(DenseGrid2f grid, DenseGrid2f tmp, float alpha, int rounds)
		{
			if (tmp == null)
			{
				tmp = new DenseGrid2f(grid.ni, grid.nj, 0f);
			}
			int ni = grid.ni;
			int nj = grid.nj;
			for (int i = 0; i < rounds; i++)
			{
				tmp.assign_border(1f, 1);
				for (int j = 1; j < nj - 1; j++)
				{
					for (int k = 1; k < ni - 1; k++)
					{
						float num = grid[k - 1, j] + grid[k - 1, j + 1] + grid[k, j + 1] + grid[k + 1, j + 1] + grid[k + 1, j] + grid[k + 1, j - 1] + grid[k, j - 1] + grid[k - 1, j - 1];
						num /= 8f;
						tmp[k, j] = (1f - alpha) * grid[k, j] + alpha * num;
					}
				}
				grid.copy(tmp);
			}
		}

		private void process_version1(DenseGrid3f supportGrid, DenseGridTrilinearImplicit distanceField)
		{
			int ni = supportGrid.ni;
			int nj = supportGrid.nj;
			int nk = supportGrid.nk;
			float num = (float)this.CellSize;
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
							Vector3d vector3d = new Vector3f((float)j * num, (float)k * num, (float)i * num) + gridOrigin;
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
			DenseGrid3i denseGrid3i = new DenseGrid3i(ni, nj, nk, 0);
			foreach (Vector3i ijk in denseGrid3i.Indices())
			{
				denseGrid3i[ijk] = ((supportGrid[ijk] < 0f) ? 1 : 0);
			}
			for (int l = 0; l < nj; l++)
			{
				GraphSupportGenerator.skeletonize_layer(denseGrid3i, l, 1);
			}
			for (int m = 0; m < nj; m++)
			{
				for (int n = 1; n < nk - 1; n++)
				{
					for (int num2 = 1; num2 < ni - 1; num2++)
					{
						if (denseGrid3i[num2, m, n] > 0)
						{
							supportGrid[num2, m, n] = -3f;
						}
					}
				}
			}
			for (int num3 = 0; num3 < nk; num3++)
			{
				for (int num4 = 0; num4 < ni; num4++)
				{
					if (supportGrid[num4, 0, num3] < 0f)
					{
						supportGrid[num4, 0, num3] = -5f;
					}
				}
			}
			DenseGrid3f denseGrid3f = new DenseGrid3f(supportGrid);
			float num5 = 0.5f;
			for (int num6 = 0; num6 < 15; num6++)
			{
				for (int num7 = 0; num7 < nj; num7++)
				{
					for (int num8 = 1; num8 < nk - 1; num8++)
					{
						for (int num9 = 1; num9 < ni - 1; num9++)
						{
							if (denseGrid3i[num9, num7, num8] > 0)
							{
								supportGrid[num9, num7, num8] -= num5 / 25f;
							}
						}
					}
				}
				for (int num10 = 0; num10 < nj; num10++)
				{
					for (int num11 = 1; num11 < nk - 1; num11++)
					{
						for (int num12 = 1; num12 < ni - 1; num12++)
						{
							int num13 = 0;
							float num14 = 0f;
							float num15 = 0f;
							for (int num16 = 0; num16 < 8; num16++)
							{
								int i2 = num12 + gIndices.GridOffsets8[num16].x;
								int k2 = num11 + gIndices.GridOffsets8[num16].y;
								float num17 = supportGrid[i2, num10, k2];
								if (num17 < 0f)
								{
									num13++;
								}
								num14 += num5 * num17;
								num15 += num5;
							}
							if (num13 > -1)
							{
								num14 += supportGrid[num12, num10, num11];
								num15 += 1f;
								denseGrid3f[num12, num10, num11] = num14 / num15;
							}
							else
							{
								denseGrid3f[num12, num10, num11] = supportGrid[num12, num10, num11];
							}
						}
					}
				}
				supportGrid.swap(denseGrid3f);
			}
		}

		private static void skeletonize_pass(DenseGrid2i grid, DenseGrid2i tmp, int iter)
		{
			int ni = grid.ni;
			int nj = grid.nj;
			for (int i = 1; i < ni - 1; i++)
			{
				for (int j = 1; j < nj - 1; j++)
				{
					int num = grid[i - 1, j];
					int num2 = grid[i - 1, j + 1];
					int num3 = grid[i, j + 1];
					int num4 = grid[i + 1, j + 1];
					int num5 = grid[i + 1, j];
					int num6 = grid[i + 1, j - 1];
					int num7 = grid[i, j - 1];
					int num8 = grid[i - 1, j - 1];
					int num9 = ((num == 0 && num2 == 1) ? 1 : 0) + ((num2 == 0 && num3 == 1) ? 1 : 0) + ((num3 == 0 && num4 == 1) ? 1 : 0) + ((num4 == 0 && num5 == 1) ? 1 : 0) + ((num5 == 0 && num6 == 1) ? 1 : 0) + ((num6 == 0 && num7 == 1) ? 1 : 0) + ((num7 == 0 && num8 == 1) ? 1 : 0) + ((num8 == 0 && num == 1) ? 1 : 0);
					int num10 = num + num2 + num3 + num4 + num5 + num6 + num7 + num8;
					int num11 = (iter == 0) ? (num * num3 * num5) : (num * num3 * num7);
					int num12 = (iter == 0) ? (num3 * num5 * num7) : (num * num5 * num7);
					if (num9 == 1 && num10 >= 2 && num10 <= 6 && num11 == 0 && num12 == 0)
					{
						tmp[i, j] = 1;
					}
				}
			}
			for (int k = 0; k < ni; k++)
			{
				for (int l = 0; l < nj; l++)
				{
					grid[k, l] &= ~tmp[k, l];
				}
			}
		}

		private static void dilate(DenseGrid2i grid, DenseGrid2i tmp, bool corners = true)
		{
			if (tmp == null)
			{
				tmp = new DenseGrid2i(grid.ni, grid.nj, 0);
			}
			int ni = grid.ni;
			int nj = grid.nj;
			for (int i = 1; i < ni - 1; i++)
			{
				for (int j = 1; j < nj - 1; j++)
				{
					if (grid[i, j] == 1)
					{
						tmp[i, j] = 1;
						tmp[i - 1, j] = 1;
						tmp[i, j + 1] = 1;
						tmp[i + 1, j] = 1;
						tmp[i, j - 1] = 1;
						if (corners)
						{
							tmp[i - 1, j + 1] = 1;
							tmp[i + 1, j + 1] = 1;
							tmp[i + 1, j - 1] = 1;
							tmp[i - 1, j - 1] = 1;
						}
					}
				}
			}
			grid.copy(tmp);
		}

		private static void dilate_loners(DenseGrid2i grid, DenseGrid2i tmp, int mode)
		{
			if (tmp == null)
			{
				tmp = new DenseGrid2i(grid.ni, grid.nj, 0);
			}
			int ni = grid.ni;
			int nj = grid.nj;
			for (int i = 1; i < ni - 1; i++)
			{
				for (int j = 1; j < nj - 1; j++)
				{
					if (grid[i, j] == 1)
					{
						tmp[i, j] = 1;
						if (grid[i - 1, j] + grid[i - 1, j + 1] + grid[i, j + 1] + grid[i + 1, j + 1] + grid[i + 1, j] + grid[i + 1, j - 1] + grid[i, j - 1] + grid[i - 1, j - 1] == 0)
						{
							if (mode != 3)
							{
								tmp[i - 1, j] = 1;
								tmp[i + 1, j] = 1;
								tmp[i, j + 1] = 1;
								tmp[i, j - 1] = 1;
							}
							if (mode == 2 || mode == 3)
							{
								tmp[i - 1, j + 1] = 1;
								tmp[i + 1, j + 1] = 1;
								tmp[i + 1, j - 1] = 1;
								tmp[i - 1, j - 1] = 1;
							}
						}
					}
				}
			}
			grid.copy(tmp);
		}

		private bool is_loner(DenseGrid2i grid, int i, int j)
		{
			return grid[i, j] != 0 && grid[i - 1, j] + grid[i - 1, j + 1] + grid[i, j + 1] + grid[i + 1, j + 1] + grid[i + 1, j] + grid[i + 1, j - 1] + grid[i, j - 1] + grid[i - 1, j - 1] == 0;
		}

		private static void skeletonize(DenseGrid2i grid, DenseGrid2i tmp, int dilation_rounds = 1)
		{
			if (tmp == null)
			{
				tmp = new DenseGrid2i(grid.ni, grid.nj, 0);
			}
			for (int i = 0; i < dilation_rounds; i++)
			{
				tmp.clear();
				GraphSupportGenerator.dilate(grid, tmp, true);
			}
			bool flag = false;
			while (!flag)
			{
				int num = grid.sum();
				tmp.clear();
				GraphSupportGenerator.skeletonize_pass(grid, tmp, 0);
				tmp.clear();
				GraphSupportGenerator.skeletonize_pass(grid, tmp, 1);
				int num2 = grid.sum();
				if (num == num2)
				{
					break;
				}
			}
		}

		private static void diffuse(DenseGrid2f grid, float t, Func<int, int, bool> skipF)
		{
			int ni = grid.ni;
			int nj = grid.nj;
			DenseGrid2f denseGrid2f = new DenseGrid2f(grid);
			for (int i = 1; i < nj - 1; i++)
			{
				for (int j = 1; j < ni - 1; j++)
				{
					if ((skipF == null || !skipF(j, i)) && grid[j, i] < 0f)
					{
						foreach (Vector2i vector2i in gIndices.GridOffsets8)
						{
							float num = (vector2i.LengthSquared > 1) ? -1f : -0.707f;
							num *= t;
							denseGrid2f[j + vector2i.x, i + vector2i.y] = Math.Min(denseGrid2f[j + vector2i.x, i + vector2i.y], num);
						}
					}
				}
			}
			grid.swap(denseGrid2f);
		}

		private static void skeletonize_layer(DenseGrid3i grid, int j, int dilation_rounds = 1)
		{
			DenseGrid2i denseGrid2i = grid.get_slice(j, 1);
			DenseGrid2i denseGrid2i2 = new DenseGrid2i(denseGrid2i.ni, denseGrid2i.nj, 0);
			for (int i = 0; i < dilation_rounds; i++)
			{
				denseGrid2i2.assign(0);
				GraphSupportGenerator.dilate(denseGrid2i, denseGrid2i2, true);
			}
			bool flag = false;
			while (!flag)
			{
				int num = denseGrid2i.sum();
				denseGrid2i2.assign(0);
				GraphSupportGenerator.skeletonize_pass(denseGrid2i, denseGrid2i2, 0);
				denseGrid2i2.assign(0);
				GraphSupportGenerator.skeletonize_pass(denseGrid2i, denseGrid2i2, 1);
				int num2 = denseGrid2i.sum();
				if (num == num2)
				{
					break;
				}
			}
			for (int k = 0; k < grid.ni; k++)
			{
				for (int l = 0; l < grid.nk; l++)
				{
					grid[k, j, l] = denseGrid2i[k, l];
				}
			}
		}

		private static void smooth(DenseGrid3f grid, DenseGrid3f tmp, float alpha, int iters, int min_j = 1)
		{
			if (tmp == null)
			{
				tmp = new DenseGrid3f(grid);
			}
			int ni = grid.ni;
			int nj = grid.nj;
			int nk = grid.nk;
			for (int i = 0; i < iters; i++)
			{
				for (int j = min_j; j < nj - 1; j++)
				{
					for (int k = 1; k < nk - 1; k++)
					{
						for (int l = 1; l < ni - 1; l++)
						{
							float num = 0f;
							foreach (Vector3i vector3i in gIndices.GridOffsets26)
							{
								int i2 = l + vector3i.x;
								int j2 = j + vector3i.y;
								int k2 = k + vector3i.z;
								float num2 = grid[i2, j2, k2];
								num += num2;
							}
							num /= 26f;
							tmp[l, j, k] = (1f - alpha) * grid[l, j, k] + alpha * num;
						}
					}
				}
				grid.swap(tmp);
			}
		}

		public DMesh3 Mesh;

		public DMeshAABBTree3 MeshSpatial;

		public double CellSize;

		public double OverhangAngleDeg = 30.0;

		public float ForceMinY = float.MaxValue;

		public bool ProcessBottomUp;

		public double GraphSurfaceDistanceOffset = 1.5;

		public double OverhangAngleOptimizeDeg = 25.0;

		public double OptimizationAlpha = 1.0;

		public int OptimizationRounds = 20;

		public ProgressCancel Progress;

		public bool DebugPrint;

		private Vector3f grid_origin;

		private DenseGrid3f volume_grid;

		public DGraph3 Graph;

		public HashSet<int> TipVertices;

		public HashSet<int> TipBaseVertices;

		public HashSet<int> GroundVertices;

		private const float SUPPORT_GRID_USED = -1f;

		private const float SUPPORT_TIP_TOP = -2f;

		private const float SUPPORT_TIP_BASE = -3f;

		public class ImplicitCurve3d : BoundedImplicitFunction3d, ImplicitFunction3d
		{
			public ImplicitCurve3d(DCurve3 curve, double radius)
			{
				this.Curve = curve;
				this.Radius = radius;
				this.Box = curve.GetBoundingBox();
				this.Box.Expand(this.Radius);
				this.spatial = new DCurve3BoxTree(curve);
			}

			public double Value(ref Vector3d pt)
			{
				return this.spatial.Distance(pt) - this.Radius;
			}

			public AxisAlignedBox3d Bounds()
			{
				return this.Box;
			}

			public DCurve3 Curve;

			public double Radius;

			public AxisAlignedBox3d Box;

			private DCurve3BoxTree spatial;
		}
	}
}
