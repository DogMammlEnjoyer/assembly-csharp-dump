using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class MeshInsertUVPolyCurve
	{
		public MeshInsertUVPolyCurve(DMesh3 mesh, PolyLine2d curve, bool isLoop = false)
		{
			this.Mesh = mesh;
			this.Curve = curve;
			this.IsLoop = isLoop;
			this.PointF = ((int vid) => this.Mesh.GetVertex(vid).xy);
			this.SetPointF = delegate(int vid, Vector2d pos)
			{
				this.Mesh.SetVertex(vid, new Vector3d(pos.x, pos.y, 0.0));
			};
		}

		public MeshInsertUVPolyCurve(DMesh3 mesh, Polygon2d loop)
		{
			this.Mesh = mesh;
			this.Curve = new PolyLine2d(loop.Vertices);
			this.IsLoop = true;
			this.PointF = ((int vid) => this.Mesh.GetVertex(vid).xy);
			this.SetPointF = delegate(int vid, Vector2d pos)
			{
				this.Mesh.SetVertex(vid, new Vector3d(pos.x, pos.y, 0.0));
			};
		}

		public MeshInsertUVPolyCurve(DMesh3 mesh, PolyLine2d path)
		{
			this.Mesh = mesh;
			this.Curve = new PolyLine2d(path.Vertices);
			this.IsLoop = false;
			this.PointF = ((int vid) => this.Mesh.GetVertex(vid).xy);
			this.SetPointF = delegate(int vid, Vector2d pos)
			{
				this.Mesh.SetVertex(vid, new Vector3d(pos.x, pos.y, 0.0));
			};
		}

		public virtual ValidationStatus Validate(double fDegenerateTol = 9.999999974752427E-07)
		{
			double num = fDegenerateTol * fDegenerateTol;
			int num2 = this.IsLoop ? (this.Curve.VertexCount - 1) : this.Curve.VertexCount;
			for (int i = 0; i < num2; i++)
			{
				Vector2d vector2d = this.Curve[i];
				Vector2d v = this.Curve[(i + 1) % this.Curve.VertexCount];
				if (vector2d.DistanceSquared(v) < num)
				{
					return ValidationStatus.NearDenegerateInputGeometry;
				}
			}
			foreach (int eID in this.Mesh.EdgeIndices())
			{
				Index2i edgeV = this.Mesh.GetEdgeV(eID);
				if (this.PointF(edgeV.a).DistanceSquared(this.PointF(edgeV.b)) < num)
				{
					return ValidationStatus.NearDegenerateMeshEdges;
				}
			}
			return ValidationStatus.Ok;
		}

		private void spatial_add_triangle(int tid)
		{
			if (this.triSpatial == null)
			{
				return;
			}
			Index3i triangle = this.Mesh.GetTriangle(tid);
			Vector2d vector2d = this.PointF(triangle.a);
			Vector2d vector2d2 = this.PointF(triangle.b);
			Vector2d vector2d3 = this.PointF(triangle.c);
			this.triSpatial.InsertTriangleUnsafe(tid, ref vector2d, ref vector2d2, ref vector2d3);
		}

		private void spatial_add_triangles(int t0, int t1)
		{
			if (this.triSpatial == null)
			{
				return;
			}
			this.spatial_add_triangle(t0);
			if (t1 != -1)
			{
				this.spatial_add_triangle(t1);
			}
		}

		private void spatial_remove_triangle(int tid)
		{
			if (this.triSpatial == null)
			{
				return;
			}
			Index3i triangle = this.Mesh.GetTriangle(tid);
			Vector2d vector2d = this.PointF(triangle.a);
			Vector2d vector2d2 = this.PointF(triangle.b);
			Vector2d vector2d3 = this.PointF(triangle.c);
			this.triSpatial.RemoveTriangleUnsafe(tid, ref vector2d, ref vector2d2, ref vector2d3);
		}

		private void spatial_remove_triangles(int t0, int t1)
		{
			if (this.triSpatial == null)
			{
				return;
			}
			this.spatial_remove_triangle(t0);
			if (t1 != -1)
			{
				this.spatial_remove_triangle(t1);
			}
		}

		private void insert_corners(HashSet<int> MeshVertsOnCurve)
		{
			PrimalQuery2d query = new PrimalQuery2d(this.PointF);
			if (this.UseTriSpatial)
			{
				int num = this.Mesh.TriangleCount + this.Curve.VertexCount;
				int numCells = 32;
				if (num < 25)
				{
					numCells = 8;
				}
				else if (num < 100)
				{
					numCells = 16;
				}
				AxisAlignedBox3d cachedBounds = this.Mesh.CachedBounds;
				AxisAlignedBox2d bounds = new AxisAlignedBox2d(cachedBounds.Min.xy, cachedBounds.Max.xy);
				this.triSpatial = new TriangleBinsGrid2d(bounds, numCells);
				foreach (int tid2 in this.Mesh.TriangleIndices())
				{
					this.spatial_add_triangle(tid2);
				}
			}
			Func<int, Vector2d, bool> containsF = delegate(int tid, Vector2d pos)
			{
				Index3i triangle3 = this.Mesh.GetTriangle(tid);
				int num7 = query.ToTriangleUnsigned(pos, triangle3.a, triangle3.b, triangle3.c);
				return num7 == -1 || num7 == 0;
			};
			this.CurveVertices = new int[this.Curve.VertexCount];
			for (int i = 0; i < this.Curve.VertexCount; i++)
			{
				Vector2d vector2d = this.Curve[i];
				bool flag = false;
				int num2 = -1;
				if (this.triSpatial != null)
				{
					num2 = this.triSpatial.FindContainingTriangle(vector2d, containsF, null);
				}
				else
				{
					foreach (int num3 in this.Mesh.TriangleIndices())
					{
						Index3i triangle = this.Mesh.GetTriangle(num3);
						int num4 = query.ToTriangleUnsigned(vector2d, triangle.a, triangle.b, triangle.c);
						if (num4 == -1 || num4 == 0)
						{
							num2 = num3;
							break;
						}
					}
				}
				if (num2 != -1)
				{
					Index3i triangle2 = this.Mesh.GetTriangle(num2);
					Vector3d bary_coords = MathUtil.BarycentricCoords(vector2d, this.PointF(triangle2.a), this.PointF(triangle2.b), this.PointF(triangle2.c));
					bool flag2;
					int num5 = this.insert_corner_from_bary(i, num2, bary_coords, 0.01, 100.0 * this.SpatialEpsilon, out flag2);
					if (num5 > 0)
					{
						this.CurveVertices[i] = num5;
						if (flag2)
						{
							MeshVertsOnCurve.Add(num5);
						}
						flag = true;
					}
				}
				if (!flag)
				{
					foreach (int num6 in this.Mesh.VertexIndices())
					{
						Vector2d v = this.PointF(num6);
						if (vector2d.Distance(v) < this.SpatialEpsilon)
						{
							this.CurveVertices[i] = num6;
							MeshVertsOnCurve.Add(num6);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					throw new Exception("MeshInsertUVPolyCurve.insert_corners: curve vertex " + i.ToString() + " is not inside or on any mesh triangle!");
				}
			}
		}

		private int insert_corner_from_bary(int iCorner, int tid, Vector3d bary_coords, double bary_tol, double spatial_tol, out bool is_existing_v)
		{
			is_existing_v = false;
			Vector2d vector2d = this.Curve[iCorner];
			Index3i triangle = this.Mesh.GetTriangle(tid);
			int num = -1;
			if (bary_coords.x > 1.0 - bary_tol)
			{
				num = triangle.a;
			}
			else if (bary_coords.y > 1.0 - bary_tol)
			{
				num = triangle.b;
			}
			else if (bary_coords.z > 1.0 - bary_tol)
			{
				num = triangle.c;
			}
			if (num != -1 && this.PointF(num).Distance(vector2d) < spatial_tol)
			{
				is_existing_v = true;
				return num;
			}
			int num2 = -1;
			if (bary_coords.x < bary_tol)
			{
				num2 = 1;
			}
			else if (bary_coords.y < bary_tol)
			{
				num2 = 2;
			}
			else if (bary_coords.z < bary_tol)
			{
				num2 = 0;
			}
			if (num2 >= 0)
			{
				int triEdge = this.Mesh.GetTriEdge(tid, num2);
				Index2i edgeV = this.Mesh.GetEdgeV(triEdge);
				Segment2d segment2d = new Segment2d(this.PointF(edgeV.a), this.PointF(edgeV.b));
				if (segment2d.DistanceSquared(vector2d) < spatial_tol * spatial_tol)
				{
					Index2i edgeT = this.Mesh.GetEdgeT(triEdge);
					this.spatial_remove_triangles(edgeT.a, edgeT.b);
					DMesh3.EdgeSplitInfo edgeSplitInfo;
					MeshResult meshResult = this.Mesh.SplitEdge(triEdge, out edgeSplitInfo, 0.5);
					if (meshResult != MeshResult.Ok)
					{
						throw new Exception("MeshInsertUVPolyCurve.insert_corner_from_bary: edge split failed in case sum==2 - " + meshResult.ToString());
					}
					this.SetPointF(edgeSplitInfo.vNew, vector2d);
					this.spatial_add_triangles(edgeT.a, edgeT.b);
					this.spatial_add_triangles(edgeSplitInfo.eNewT2, edgeSplitInfo.eNewT3);
					return edgeSplitInfo.vNew;
				}
			}
			this.spatial_remove_triangle(tid);
			DMesh3.PokeTriangleInfo pokeTriangleInfo;
			MeshResult meshResult2 = this.Mesh.PokeTriangle(tid, bary_coords, out pokeTriangleInfo);
			if (meshResult2 != MeshResult.Ok)
			{
				throw new Exception("MeshInsertUVPolyCurve.insert_corner_from_bary: face poke failed - " + meshResult2.ToString());
			}
			this.SetPointF(pokeTriangleInfo.new_vid, vector2d);
			this.spatial_add_triangle(tid);
			this.spatial_add_triangle(pokeTriangleInfo.new_t1);
			this.spatial_add_triangle(pokeTriangleInfo.new_t2);
			return pokeTriangleInfo.new_vid;
		}

		public virtual bool Apply()
		{
			HashSet<int> hashSet = new HashSet<int>();
			this.insert_corners(hashSet);
			HashSet<int> hashSet2 = new HashSet<int>();
			HashSet<int> hashSet3 = new HashSet<int>();
			this.OnCutEdges = new HashSet<int>();
			HashSet<int> hashSet4 = new HashSet<int>();
			HashSet<int> hashSet5 = new HashSet<int>();
			sbyte[] signs = new sbyte[2 * this.Mesh.MaxVertexID + 2 * this.Curve.VertexCount];
			HashSet<int> hashSet6 = new HashSet<int>();
			HashSet<int> hashSet7 = new HashSet<int>();
			HashSet<int> hashSet8 = new HashSet<int>();
			int num = this.IsLoop ? this.Curve.VertexCount : (this.Curve.VertexCount - 1);
			for (int i = 0; i < num; i++)
			{
				int num2 = i;
				int num3 = (i + 1) % this.Curve.VertexCount;
				Segment2d seg = new Segment2d(this.Curve[num2], this.Curve[num3]);
				int i0_vid = this.CurveVertices[num2];
				int i1_vid = this.CurveVertices[num3];
				int num4 = this.Mesh.FindEdge(i0_vid, i1_vid);
				if (num4 != -1)
				{
					this.add_cut_edge(num4);
				}
				else
				{
					if (this.triSpatial != null)
					{
						hashSet6.Clear();
						hashSet7.Clear();
						hashSet8.Clear();
						AxisAlignedBox2d range = new AxisAlignedBox2d(seg.P0);
						range.Contain(seg.P1);
						range.Expand(9.999999747378752E-06);
						this.triSpatial.FindTrianglesInRange(range, hashSet6);
						IndexUtil.TrianglesToVertices(this.Mesh, hashSet6, hashSet7);
						IndexUtil.TrianglesToEdges(this.Mesh, hashSet6, hashSet8);
					}
					int maxVertexID = this.Mesh.MaxVertexID;
					IEnumerable<int> source = Interval1i.Range(maxVertexID);
					if (this.triSpatial != null)
					{
						source = hashSet7;
					}
					if (signs.Length < maxVertexID)
					{
						signs = new sbyte[2 * maxVertexID];
					}
					gParallel.ForEach<int>(source, delegate(int vid)
					{
						if (!this.Mesh.IsVertex(vid))
						{
							signs[vid] = sbyte.MaxValue;
							return;
						}
						if (vid == i0_vid || vid == i1_vid)
						{
							signs[vid] = 0;
							return;
						}
						Vector2d test = this.PointF(vid);
						signs[vid] = (sbyte)seg.WhichSide(test, this.SpatialEpsilon);
					});
					int maxEdgeID = this.Mesh.MaxEdgeID;
					hashSet4.Clear();
					hashSet5.Clear();
					hashSet5.Add(i0_vid);
					hashSet5.Add(i1_vid);
					IEnumerable<int> enumerable = Interval1i.Range(maxEdgeID);
					if (this.triSpatial != null)
					{
						enumerable = hashSet8;
					}
					foreach (int num5 in enumerable)
					{
						if (this.Mesh.IsEdge(num5) && num5 < maxEdgeID && !hashSet4.Contains(num5) && !this.Mesh.IsBoundaryEdge(num5))
						{
							Index2i edgeV = this.Mesh.GetEdgeV(num5);
							int num6 = (int)signs[edgeV.a];
							int num7 = (int)signs[edgeV.b];
							bool flag = false;
							if (num6 == 0)
							{
								flag = (hashSet.Contains(edgeV.a) || Math.Abs(seg.Project(this.PointF(edgeV.a))) < seg.Extent + this.SpatialEpsilon);
							}
							bool flag2 = false;
							if (num7 == 0)
							{
								flag2 = (hashSet.Contains(edgeV.b) || Math.Abs(seg.Project(this.PointF(edgeV.b))) < seg.Extent + this.SpatialEpsilon);
							}
							if (flag || flag2)
							{
								if (flag && flag2)
								{
									hashSet2.Add(num5);
									this.add_cut_edge(num5);
									hashSet5.Add(edgeV.a);
									hashSet5.Add(edgeV.b);
								}
								else
								{
									int item = flag ? edgeV.a : edgeV.b;
									hashSet3.Add(item);
									hashSet5.Add(item);
								}
							}
							else if (num6 * num7 <= 0)
							{
								Vector2d vector2d = this.PointF(edgeV.a);
								Vector2d vector2d2 = this.PointF(edgeV.b);
								Segment2d segment2d = new Segment2d(vector2d, vector2d2);
								IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(seg, segment2d);
								intrSegment2Segment.Compute();
								if (intrSegment2Segment.Type == IntersectionType.Segment)
								{
									hashSet2.Add(num5);
									hashSet5.Add(edgeV.a);
									hashSet5.Add(edgeV.b);
									this.add_cut_edge(num5);
								}
								else if (intrSegment2Segment.Type == IntersectionType.Point)
								{
									Vector2d point = intrSegment2Segment.Point0;
									double split_t = Math.Sqrt(point.DistanceSquared(vector2d) / vector2d.DistanceSquared(vector2d2));
									if (Math.Abs(segment2d.Project(point)) < segment2d.Extent - this.SpatialEpsilon)
									{
										Index2i edgeT = this.Mesh.GetEdgeT(num5);
										this.spatial_remove_triangles(edgeT.a, edgeT.b);
										DMesh3.EdgeSplitInfo edgeSplitInfo;
										MeshResult meshResult = this.Mesh.SplitEdge(num5, out edgeSplitInfo, split_t);
										if (meshResult != MeshResult.Ok)
										{
											throw new Exception("MeshInsertUVSegment.Apply: SplitEdge failed - " + meshResult.ToString());
										}
										this.SetPointF(edgeSplitInfo.vNew, point);
										hashSet5.Add(edgeSplitInfo.vNew);
										hashSet4.Add(edgeSplitInfo.eNewBN);
										hashSet4.Add(edgeSplitInfo.eNewCN);
										this.spatial_add_triangles(edgeT.a, edgeT.b);
										this.spatial_add_triangles(edgeSplitInfo.eNewT2, edgeSplitInfo.eNewT3);
										Index2i edgeV2 = this.Mesh.GetEdgeV(edgeSplitInfo.eNewCN);
										if (hashSet5.Contains(edgeV2.a) && hashSet5.Contains(edgeV2.b))
										{
											this.add_cut_edge(edgeSplitInfo.eNewCN);
										}
										if (edgeSplitInfo.eNewDN != -1)
										{
											hashSet4.Add(edgeSplitInfo.eNewDN);
											Index2i edgeV3 = this.Mesh.GetEdgeV(edgeSplitInfo.eNewDN);
											if (hashSet5.Contains(edgeV3.a) && hashSet5.Contains(edgeV3.b))
											{
												this.add_cut_edge(edgeSplitInfo.eNewDN);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.EnableCutSpansAndLoops)
			{
				this.find_cut_paths(this.OnCutEdges);
			}
			return true;
		}

		private void add_cut_edge(int eid)
		{
			this.OnCutEdges.Add(eid);
		}

		public void Simplify()
		{
			for (int i = 0; i < this.Loops.Count; i++)
			{
				EdgeLoop value = this.simplify(this.Loops[i]);
				this.Loops[i] = value;
			}
		}

		private EdgeLoop simplify(EdgeLoop loop)
		{
			HashSet<int> hashSet = new HashSet<int>(this.CurveVertices);
			List<int> list = new List<int>();
			for (int i = 0; i < loop.EdgeCount; i++)
			{
				int num = loop.Edges[i];
				Index2i edgeV = this.Mesh.GetEdgeV(num);
				if (hashSet.Contains(edgeV.a) && hashSet.Contains(edgeV.b))
				{
					list.Add(num);
				}
				else
				{
					int num2 = edgeV.a;
					int num3 = edgeV.b;
					Vector3d vector3d = Vector3d.Zero;
					if (hashSet.Contains(edgeV.b))
					{
						num2 = edgeV.b;
						num3 = edgeV.a;
						vector3d = this.Mesh.GetVertex(edgeV.b);
					}
					else if (hashSet.Contains(edgeV.a))
					{
						vector3d = this.Mesh.GetVertex(edgeV.a);
					}
					else
					{
						vector3d = 0.5 * (this.Mesh.GetVertex(edgeV.a) + this.Mesh.GetVertex(edgeV.b));
					}
					if (MeshUtil.CheckIfCollapseCreatesFlip(this.Mesh, num, vector3d))
					{
						list.Add(num);
					}
					else
					{
						Index4i edge = this.Mesh.GetEdge(num);
						int vB = IndexUtil.find_tri_other_vtx(num2, num3, this.Mesh.GetTriangle(edge.c));
						int vB2 = IndexUtil.find_tri_other_vtx(num2, num3, this.Mesh.GetTriangle(edge.d));
						int item = this.Mesh.FindEdge(num3, vB);
						int item2 = this.Mesh.FindEdge(num3, vB2);
						DMesh3.EdgeCollapseInfo edgeCollapseInfo;
						if (this.OnCutEdges.Contains(item) || this.OnCutEdges.Contains(item2))
						{
							list.Add(num);
						}
						else if (this.Mesh.CollapseEdge(num2, num3, out edgeCollapseInfo) == MeshResult.Ok)
						{
							this.Mesh.SetVertex(edgeCollapseInfo.vKept, vector3d);
							this.OnCutEdges.Remove(edgeCollapseInfo.eCollapsed);
						}
						else
						{
							list.Add(num);
						}
					}
				}
			}
			return EdgeLoop.FromEdges(this.Mesh, list);
		}

		private void find_cut_paths(HashSet<int> CutEdges)
		{
			this.Spans = new List<EdgeSpan>();
			this.Loops = new List<EdgeLoop>();
			HashSet<int> hashSet = new HashSet<int>(CutEdges);
			while (hashSet.Count > 0)
			{
				int num = hashSet.First<int>();
				hashSet.Remove(num);
				Index2i edgeV = this.Mesh.GetEdgeV(num);
				bool flag;
				List<int> list = MeshInsertUVPolyCurve.walk_edge_span_forward(this.Mesh, num, edgeV.a, hashSet, out flag);
				if (!flag)
				{
					List<int> list2 = MeshInsertUVPolyCurve.walk_edge_span_forward(this.Mesh, num, edgeV.b, hashSet, out flag);
					if (flag)
					{
						throw new Exception("find_cut_paths: how did this possibly happen?!?");
					}
					if (list2.Count > 1)
					{
						list2.Reverse();
						list2.RemoveAt(list2.Count - 1);
						list2.AddRange(list);
						Index2i edgeV2 = this.Mesh.GetEdgeV(list2[0]);
						Index2i edgeV3 = this.Mesh.GetEdgeV(list2[list2.Count - 1]);
						flag = (list2.Count > 2 && IndexUtil.find_shared_edge_v(ref edgeV2, ref edgeV3) != -1);
						list = list2;
					}
				}
				if (flag)
				{
					EdgeLoop item = EdgeLoop.FromEdges(this.Mesh, list);
					this.Loops.Add(item);
				}
				else
				{
					EdgeSpan item2 = EdgeSpan.FromEdges(this.Mesh, list);
					this.Spans.Add(item2);
				}
			}
		}

		private static List<int> walk_edge_span_forward(DMesh3 mesh, int start_edge, int start_pivot_v, HashSet<int> EdgeSet, out bool bClosedLoop)
		{
			bClosedLoop = false;
			List<int> list = new List<int>();
			list.Add(start_edge);
			int num = start_pivot_v;
			int num2 = IndexUtil.find_edge_other_v(mesh.GetEdgeV(start_edge), start_pivot_v);
			bool flag = false;
			while (!flag)
			{
				int num3 = -1;
				foreach (int num4 in mesh.VtxEdgesItr(num))
				{
					if (EdgeSet.Contains(num4))
					{
						num3 = num4;
						break;
					}
				}
				if (num3 == -1)
				{
					flag = true;
					break;
				}
				Index2i edgeV = mesh.GetEdgeV(num3);
				if (edgeV.a == num)
				{
					num = edgeV.b;
				}
				else
				{
					if (edgeV.b != num)
					{
						throw new Exception("walk_edge_span_forward: found valid next edge but not connected to previous vertex??");
					}
					num = edgeV.a;
				}
				list.Add(num3);
				EdgeSet.Remove(num3);
				if (num == num2)
				{
					flag = true;
					bClosedLoop = true;
				}
			}
			return list;
		}

		public DMesh3 Mesh;

		public PolyLine2d Curve;

		public bool IsLoop;

		public Func<int, Vector2d> PointF;

		public Action<int, Vector2d> SetPointF;

		public bool EnableCutSpansAndLoops = true;

		public bool UseTriSpatial = true;

		public double SpatialEpsilon = 1E-08;

		public int[] CurveVertices;

		public HashSet<int> OnCutEdges;

		public List<EdgeSpan> Spans;

		public List<EdgeLoop> Loops;

		private TriangleBinsGrid2d triSpatial;
	}
}
