using System;
using System.Collections.Generic;
using System.Linq;
using g3;

namespace gs
{
	public class MinimalHoleFill
	{
		public MinimalHoleFill(DMesh3 mesh, EdgeLoop fillLoop)
		{
			this.Mesh = mesh;
			this.FillLoop = fillLoop;
		}

		public bool Apply()
		{
			SimpleHoleFiller simpleHoleFiller = new SimpleHoleFiller(this.Mesh, this.FillLoop);
			int group_id = this.Mesh.AllocateTriangleGroup();
			if (!simpleHoleFiller.Fill(group_id))
			{
				return false;
			}
			if (this.FillLoop.Vertices.Length <= 3)
			{
				this.FillTriangles = simpleHoleFiller.NewTriangles;
				this.FillVertices = new int[0];
				return true;
			}
			HashSet<int> hashSet = new HashSet<int>(simpleHoleFiller.NewTriangles);
			this.regionop = new RegionOperator(this.Mesh, simpleHoleFiller.NewTriangles, delegate(DSubmesh3 submesh)
			{
				submesh.ComputeTriMaps = true;
			});
			this.fillmesh = this.regionop.Region.SubMesh;
			this.boundaryv = new HashSet<int>(MeshIterators.BoundaryEdgeVertices(this.fillmesh));
			this.exterior_angle_sums = new Dictionary<int, double>();
			if (!this.IgnoreBoundaryTriangles)
			{
				foreach (int num in this.boundaryv)
				{
					double num2 = 0.0;
					int num3 = this.regionop.Region.MapVertexToBaseMesh(num);
					foreach (int num4 in this.regionop.BaseMesh.VtxTrianglesItr(num3))
					{
						if (!hashSet.Contains(num4))
						{
							Index3i triangle = this.regionop.BaseMesh.GetTriangle(num4);
							int i = IndexUtil.find_tri_index(num3, ref triangle);
							num2 += this.regionop.BaseMesh.GetTriInternalAngleR(num4, i);
						}
					}
					this.exterior_angle_sums[num] = num2;
				}
			}
			double num5;
			double num6;
			double num7;
			MeshQueries.EdgeLengthStatsFromEdges(this.Mesh, this.FillLoop.Edges, out num5, out num6, out num7, 0);
			double num8;
			double num9;
			double num10;
			MeshQueries.EdgeLengthStats(this.fillmesh, out num8, out num9, out num10, 0);
			double num11 = num7;
			if (num9 / num11 > 10.0)
			{
				num11 = num9 / 10.0;
			}
			RemesherPro remesherPro = new RemesherPro(this.fillmesh);
			remesherPro.SmoothSpeedT = 1.0;
			MeshConstraintUtil.FixAllBoundaryEdges(remesherPro);
			remesherPro.SetTargetEdgeLength(num11);
			remesherPro.FastestRemesh(25, true);
			int num12 = 0;
			int num13 = 0;
			while (num13++ < 20 && num12 < 2)
			{
				int maxEdgeID = this.fillmesh.MaxEdgeID;
				int num14 = 0;
				for (int j = 0; j < maxEdgeID; j++)
				{
					if (this.fillmesh.IsEdge(j) && !this.fillmesh.IsBoundaryEdge(j))
					{
						Index2i edgeV = this.fillmesh.GetEdgeV(j);
						bool flag = this.boundaryv.Contains(edgeV.a);
						bool flag2 = this.boundaryv.Contains(edgeV.b);
						if (!flag || !flag2)
						{
							int num15 = flag ? edgeV.a : edgeV.b;
							int vRemove = (num15 == edgeV.a) ? edgeV.b : edgeV.a;
							Vector3d vertex = this.fillmesh.GetVertex(num15);
							DMesh3.EdgeCollapseInfo edgeCollapseInfo;
							if (!MeshUtil.CheckIfCollapseCreatesFlip(this.fillmesh, j, vertex) && this.fillmesh.CollapseEdge(num15, vRemove, out edgeCollapseInfo) == MeshResult.Ok)
							{
								num14++;
							}
						}
					}
				}
				if (num14 == 0)
				{
					num12++;
				}
				else
				{
					num12 = 0;
				}
				maxEdgeID = this.fillmesh.MaxEdgeID;
				for (int k = 0; k < maxEdgeID; k++)
				{
					if (this.fillmesh.IsEdge(k) && !this.fillmesh.IsBoundaryEdge(k))
					{
						bool flag3 = false;
						Index2i edgeV2 = this.fillmesh.GetEdgeV(k);
						Vector3d vector3d;
						Vector3d v;
						Vector3d vector3d2;
						Vector3d v2;
						MeshUtil.GetEdgeFlipNormals(this.fillmesh, k, out vector3d, out v, out vector3d2, out v2);
						double num16 = vector3d.Dot(v);
						double num17 = vector3d2.Dot(v2);
						if (vector3d.Dot(v) < 0.1 || num17 > num16 + 1.1920928955078125E-07)
						{
							flag3 = true;
						}
						if (!flag3)
						{
							Index2i edgeOpposingV = this.fillmesh.GetEdgeOpposingV(k);
							double num18 = this.fillmesh.GetVertex(edgeV2.a).Distance(this.fillmesh.GetVertex(edgeV2.b));
							if (this.fillmesh.GetVertex(edgeOpposingV.a).Distance(this.fillmesh.GetVertex(edgeOpposingV.b)) < num18 && !MeshUtil.CheckIfEdgeFlipCreatesFlip(this.fillmesh, k, 0.0))
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							DMesh3.EdgeFlipInfo edgeFlipInfo;
							this.fillmesh.FlipEdge(k, out edgeFlipInfo);
						}
					}
				}
			}
			this.remove_remaining_interior_verts();
			bool flag4 = true;
			bool optimizeDevelopability = this.OptimizeDevelopability;
			bool flag5 = this.OptimizeDevelopability && this.OptimizeTriangles;
			HashSet<int> hashSet2 = new HashSet<int>(this.fillmesh.EdgeIndices());
			HashSet<int> hashSet3 = new HashSet<int>();
			int num19 = 0;
			int num20 = 0;
			while (num19++ < 40 && num20 < 2 && hashSet2.Count<int>() > 0 && flag4)
			{
				num20++;
				foreach (int num21 in hashSet2)
				{
					if (!this.fillmesh.IsBoundaryEdge(num21))
					{
						bool flag6 = false;
						this.fillmesh.GetEdgeV(num21);
						Vector3d vector3d3;
						Vector3d v3;
						Vector3d vector3d4;
						Vector3d v4;
						MeshUtil.GetEdgeFlipNormals(this.fillmesh, num21, out vector3d3, out v3, out vector3d4, out v4);
						double num22 = vector3d3.Dot(v3);
						double num23 = vector3d4.Dot(v4);
						if (num19 < 20 && num22 < 0.1)
						{
							flag6 = true;
						}
						if (num23 > num22 + 1.1920928955078125E-07)
						{
							flag6 = true;
						}
						DMesh3.EdgeFlipInfo edgeFlipInfo2;
						if (flag6 && this.fillmesh.FlipEdge(num21, out edgeFlipInfo2) == MeshResult.Ok)
						{
							num20 = 0;
							this.add_all_edges(num21, hashSet3);
						}
					}
				}
				HashSet<int> hashSet4 = hashSet2;
				hashSet2 = hashSet3;
				hashSet3 = hashSet4;
				hashSet3.Clear();
			}
			int num24 = 0;
			if (optimizeDevelopability)
			{
				this.curvatures = new double[this.fillmesh.MaxVertexID];
				foreach (int vid in this.fillmesh.VertexIndices())
				{
					this.update_curvature(vid);
				}
				hashSet2 = new HashSet<int>(this.fillmesh.EdgeIndices());
				hashSet3 = new HashSet<int>();
				while (num24++ < 40 && hashSet2.Count<int>() > 0 && optimizeDevelopability)
				{
					foreach (int num25 in hashSet2)
					{
						if (!this.fillmesh.IsBoundaryEdge(num25))
						{
							Index2i edgeV3 = this.fillmesh.GetEdgeV(num25);
							Index2i edgeOpposingV2 = this.fillmesh.GetEdgeOpposingV(num25);
							if (this.fillmesh.FindEdge(edgeOpposingV2.a, edgeOpposingV2.b) == -1)
							{
								double num26 = this.curvature_metric_cached(edgeV3.a, edgeV3.b, edgeOpposingV2.a, edgeOpposingV2.b);
								DMesh3.EdgeFlipInfo edgeFlipInfo3;
								if (num26 >= 9.999999974752427E-07 && this.fillmesh.FlipEdge(num25, out edgeFlipInfo3) == MeshResult.Ok)
								{
									if (this.curvature_metric_eval(edgeV3.a, edgeV3.b, edgeOpposingV2.a, edgeOpposingV2.b) >= num26 - 9.999999974752427E-07)
									{
										this.fillmesh.FlipEdge(num25, out edgeFlipInfo3);
									}
									else
									{
										this.update_curvature(edgeV3.a);
										this.update_curvature(edgeV3.b);
										this.update_curvature(edgeOpposingV2.a);
										this.update_curvature(edgeOpposingV2.b);
										this.add_all_edges(num25, hashSet3);
									}
								}
							}
						}
					}
					HashSet<int> hashSet5 = hashSet2;
					hashSet2 = hashSet3;
					hashSet3 = hashSet5;
					hashSet3.Clear();
				}
			}
			if (flag5)
			{
				hashSet2 = new HashSet<int>(this.fillmesh.EdgeIndices());
				hashSet3 = new HashSet<int>();
				int num27 = 0;
				while (hashSet2.Count<int>() > 0 && num27 < 20)
				{
					num27++;
					foreach (int num28 in hashSet2)
					{
						if (!this.fillmesh.IsBoundaryEdge(num28))
						{
							Index2i edgeV4 = this.fillmesh.GetEdgeV(num28);
							Index2i edgeOpposingV3 = this.fillmesh.GetEdgeOpposingV(num28);
							if (this.fillmesh.FindEdge(edgeOpposingV3.a, edgeOpposingV3.b) == -1)
							{
								double num29 = this.curvature_metric_cached(edgeV4.a, edgeV4.b, edgeOpposingV3.a, edgeOpposingV3.b);
								DMesh3.EdgeFlipInfo edgeFlipInfo4;
								if (this.aspect_metric(num28) <= 1.0 && this.fillmesh.FlipEdge(num28, out edgeFlipInfo4) == MeshResult.Ok)
								{
									double num30 = this.curvature_metric_eval(edgeV4.a, edgeV4.b, edgeOpposingV3.a, edgeOpposingV3.b);
									if (Math.Abs(num29 - num30) >= this.DevelopabilityTolerance)
									{
										this.fillmesh.FlipEdge(num28, out edgeFlipInfo4);
									}
									else
									{
										this.update_curvature(edgeV4.a);
										this.update_curvature(edgeV4.b);
										this.update_curvature(edgeOpposingV3.a);
										this.update_curvature(edgeOpposingV3.b);
										this.add_all_edges(num28, hashSet3);
									}
								}
							}
						}
					}
					HashSet<int> hashSet6 = hashSet2;
					hashSet2 = hashSet3;
					hashSet3 = hashSet6;
					hashSet3.Clear();
				}
			}
			this.regionop.BackPropropagate(true);
			this.FillTriangles = this.regionop.CurrentBaseTriangles;
			this.FillVertices = this.regionop.CurrentBaseInteriorVertices().ToArray<int>();
			return true;
		}

		private void remove_remaining_interior_verts()
		{
			HashSet<int> hashSet = new HashSet<int>(MeshIterators.InteriorVertices(this.fillmesh));
			int num = 0;
			while (hashSet.Count > 0 && hashSet.Count != num)
			{
				num = hashSet.Count;
				foreach (int num2 in hashSet.ToArray<int>())
				{
					foreach (int eID in this.fillmesh.VtxEdgesItr(num2))
					{
						Index2i edgeV = this.fillmesh.GetEdgeV(eID);
						int vKeep = (edgeV.a == num2) ? edgeV.b : edgeV.a;
						DMesh3.EdgeCollapseInfo edgeCollapseInfo;
						if (this.fillmesh.CollapseEdge(vKeep, num2, out edgeCollapseInfo) == MeshResult.Ok)
						{
							break;
						}
					}
					if (!this.fillmesh.IsVertex(num2))
					{
						hashSet.Remove(num2);
					}
				}
			}
			if (hashSet.Count > 0)
			{
				Util.gBreakToDebugger();
			}
		}

		private void add_all_edges(int ei, HashSet<int> edge_set)
		{
			Index2i edgeT = this.fillmesh.GetEdgeT(ei);
			Index3i triEdges = this.fillmesh.GetTriEdges(edgeT.a);
			edge_set.Add(triEdges.a);
			edge_set.Add(triEdges.b);
			edge_set.Add(triEdges.c);
			triEdges = this.fillmesh.GetTriEdges(edgeT.b);
			edge_set.Add(triEdges.a);
			edge_set.Add(triEdges.b);
			edge_set.Add(triEdges.c);
		}

		private double area_metric(int eid)
		{
			Index3i index3i;
			Index3i index3i2;
			Index3i index3i3;
			Index3i index3i4;
			MeshUtil.GetEdgeFlipTris(this.fillmesh, eid, out index3i, out index3i2, out index3i3, out index3i4);
			double num = this.get_tri_area(this.fillmesh, ref index3i);
			double num2 = this.get_tri_area(this.fillmesh, ref index3i2);
			double num3 = this.get_tri_area(this.fillmesh, ref index3i3);
			double num4 = this.get_tri_area(this.fillmesh, ref index3i4);
			double num5 = (num + num2) * 0.5;
			double num6 = (num3 + num4) * 0.5;
			double num7 = Math.Abs(num - num5) + Math.Abs(num2 - num5);
			return (Math.Abs(num3 - num6) + Math.Abs(num4 - num6)) / num7;
		}

		private double aspect_metric(int eid)
		{
			Index3i index3i;
			Index3i index3i2;
			Index3i index3i3;
			Index3i index3i4;
			MeshUtil.GetEdgeFlipTris(this.fillmesh, eid, out index3i, out index3i2, out index3i3, out index3i4);
			double num = this.get_tri_aspect(this.fillmesh, ref index3i);
			double num2 = this.get_tri_aspect(this.fillmesh, ref index3i2);
			double num3 = this.get_tri_aspect(this.fillmesh, ref index3i3);
			double num4 = this.get_tri_aspect(this.fillmesh, ref index3i4);
			double num5 = Math.Abs(num - 1.0) + Math.Abs(num2 - 1.0);
			return (Math.Abs(num3 - 1.0) + Math.Abs(num4 - 1.0)) / num5;
		}

		private void update_curvature(int vid)
		{
			double num = 0.0;
			this.exterior_angle_sums.TryGetValue(vid, out num);
			foreach (int tID in this.fillmesh.VtxTrianglesItr(vid))
			{
				Index3i triangle = this.fillmesh.GetTriangle(tID);
				int i = IndexUtil.find_tri_index(vid, ref triangle);
				num += this.fillmesh.GetTriInternalAngleR(tID, i);
			}
			this.curvatures[vid] = num - 6.283185307179586;
		}

		private double curvature_metric_cached(int a, int b, int c, int d)
		{
			double value = this.curvatures[a];
			double value2 = this.curvatures[b];
			double value3 = this.curvatures[c];
			double value4 = this.curvatures[d];
			return Math.Abs(value) + Math.Abs(value2) + Math.Abs(value3) + Math.Abs(value4);
		}

		private double curvature_metric_eval(int a, int b, int c, int d)
		{
			double value = this.compute_gauss_curvature(a);
			double value2 = this.compute_gauss_curvature(b);
			double value3 = this.compute_gauss_curvature(c);
			double value4 = this.compute_gauss_curvature(d);
			return Math.Abs(value) + Math.Abs(value2) + Math.Abs(value3) + Math.Abs(value4);
		}

		private double compute_gauss_curvature(int vid)
		{
			double num = 0.0;
			this.exterior_angle_sums.TryGetValue(vid, out num);
			foreach (int tID in this.fillmesh.VtxTrianglesItr(vid))
			{
				Index3i triangle = this.fillmesh.GetTriangle(tID);
				int i = IndexUtil.find_tri_index(vid, ref triangle);
				num += this.fillmesh.GetTriInternalAngleR(tID, i);
			}
			return num - 6.283185307179586;
		}

		private Vector3d get_tri_normal(DMesh3 mesh, Index3i tri)
		{
			return MathUtil.Normal(mesh.GetVertex(tri.a), mesh.GetVertex(tri.b), mesh.GetVertex(tri.c));
		}

		private double get_tri_area(DMesh3 mesh, ref Index3i tri)
		{
			return MathUtil.Area(mesh.GetVertex(tri.a), mesh.GetVertex(tri.b), mesh.GetVertex(tri.c));
		}

		private double get_tri_aspect(DMesh3 mesh, ref Index3i tri)
		{
			return MathUtil.AspectRatio(mesh.GetVertex(tri.a), mesh.GetVertex(tri.b), mesh.GetVertex(tri.c));
		}

		public DMesh3 Mesh;

		public EdgeLoop FillLoop;

		public bool IgnoreBoundaryTriangles;

		public bool OptimizeDevelopability = true;

		public bool OptimizeTriangles = true;

		public double DevelopabilityTolerance = 0.0001;

		public int[] FillVertices;

		public int[] FillTriangles;

		private RegionOperator regionop;

		private DMesh3 fillmesh;

		private HashSet<int> boundaryv;

		private Dictionary<int, double> exterior_angle_sums;

		private double[] curvatures;
	}
}
