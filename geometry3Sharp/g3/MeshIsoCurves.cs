using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshIsoCurves
	{
		public MeshIsoCurves(DMesh3 mesh, Func<Vector3d, double> valueF)
		{
			this.Mesh = mesh;
			this.ValueF = valueF;
		}

		public void Compute()
		{
			this.compute_full(this.Mesh.TriangleIndices(), true);
		}

		public void Compute(IEnumerable<int> Triangles)
		{
			this.compute_full(Triangles, false);
		}

		protected void compute_full(IEnumerable<int> Triangles, bool bIsFullMeshHint = false)
		{
			this.Graph = new DGraph3();
			if (this.WantGraphEdgeInfo)
			{
				this.GraphEdges = new DVector<MeshIsoCurves.GraphEdgeInfo>();
			}
			this.Vertices = new Dictionary<Vector3d, int>();
			double[] vertex_values = null;
			if (this.PrecomputeVertexValues)
			{
				vertex_values = new double[this.Mesh.MaxVertexID];
				IEnumerable<int> source = this.Mesh.VertexIndices();
				if (!bIsFullMeshHint)
				{
					MeshVertexSelection meshVertexSelection = new MeshVertexSelection(this.Mesh);
					meshVertexSelection.SelectTriangleVertices(Triangles);
					source = meshVertexSelection;
				}
				gParallel.ForEach<int>(source, delegate(int vid)
				{
					vertex_values[vid] = this.ValueF(this.Mesh.GetVertex(vid));
				});
				this.VertexValueF = ((int vid) => vertex_values[vid]);
			}
			foreach (int num in Triangles)
			{
				Vector3dTuple3 vector3dTuple = default(Vector3dTuple3);
				this.Mesh.GetTriVertices(num, ref vector3dTuple.V0, ref vector3dTuple.V1, ref vector3dTuple.V2);
				Index3i triangle = this.Mesh.GetTriangle(num);
				Vector3d vector3d = (this.VertexValueF != null) ? new Vector3d(this.VertexValueF(triangle.a), this.VertexValueF(triangle.b), this.VertexValueF(triangle.c)) : new Vector3d(this.ValueF(vector3dTuple.V0), this.ValueF(vector3dTuple.V1), this.ValueF(vector3dTuple.V2));
				if ((vector3d.x >= 0.0 || vector3d.y >= 0.0 || vector3d.z >= 0.0) && (vector3d.x <= 0.0 || vector3d.y <= 0.0 || vector3d.z <= 0.0))
				{
					Index3i triEdges = this.Mesh.GetTriEdges(num);
					if (vector3d.x * vector3d.y * vector3d.z == 0.0)
					{
						int num2 = (vector3d.x == 0.0) ? 0 : ((vector3d.y == 0.0) ? 1 : 2);
						int num3 = (num2 + 1) % 3;
						int num4 = (num2 + 2) % 3;
						if (vector3d[num3] * vector3d[num4] <= 0.0)
						{
							if (vector3d[num3] == 0.0 || vector3d[num4] == 0.0)
							{
								int num5 = (vector3d[num3] == 0.0) ? num3 : num4;
								if ((num2 + 1) % 3 != num5)
								{
									int num6 = num2;
									num2 = num5;
									num5 = num6;
								}
								int num7 = this.add_or_append_vertex(this.Mesh.GetVertex(triangle[num2]));
								int num8 = this.add_or_append_vertex(this.Mesh.GetVertex(triangle[num5]));
								int num9 = this.Graph.AppendEdge(num7, num8, 3);
								if (num9 >= 0 && this.WantGraphEdgeInfo)
								{
									this.add_on_edge(num9, num, triEdges[num2], new Index2i(num7, num8));
								}
							}
							else
							{
								int num10 = this.add_or_append_vertex(this.Mesh.GetVertex(triangle[num2]));
								int num11 = num3;
								int num12 = num4;
								if (triangle[num12] < triangle[num11])
								{
									int num13 = num11;
									num11 = num12;
									num12 = num13;
								}
								Vector3d vector3d2 = this.find_crossing(vector3dTuple[num11], vector3dTuple[num12], vector3d[num11], vector3d[num12]);
								int num14 = this.add_or_append_vertex(vector3d2);
								this.add_edge_pos(triangle[num11], triangle[num12], vector3d2);
								if (num10 != num14)
								{
									int num15 = this.Graph.AppendEdge(num10, num14, 2);
									if (num15 >= 0 && this.WantGraphEdgeInfo)
									{
										this.add_edge_vert(num15, num, triEdges[(num2 + 1) % 3], triangle[num2], new Index2i(num10, num14));
									}
								}
							}
						}
					}
					else
					{
						Index3i min = Index3i.Min;
						int num16 = 0;
						for (int i = 0; i < 3; i++)
						{
							int num17 = i;
							int num18 = (i + 1) % 3;
							if (vector3d[num17] < 0.0)
							{
								num16++;
							}
							if (vector3d[num17] * vector3d[num18] <= 0.0)
							{
								if (triangle[num18] < triangle[num17])
								{
									int num19 = num17;
									num17 = num18;
									num18 = num19;
								}
								Vector3d vector3d3 = this.find_crossing(vector3dTuple[num17], vector3dTuple[num18], vector3d[num17], vector3d[num18]);
								min[i] = this.add_or_append_vertex(vector3d3);
								this.add_edge_pos(triangle[num17], triangle[num18], vector3d3);
							}
						}
						int num20 = (min.a == int.MinValue) ? 1 : 0;
						int num21 = (min.c == int.MinValue) ? 1 : 2;
						if (num20 == 0 && num21 == 2)
						{
							num20 = 2;
							num21 = 0;
						}
						if (num16 == 1)
						{
							int num22 = num20;
							num20 = num21;
							num21 = num22;
						}
						int num23 = min[num20];
						int num24 = min[num21];
						if (num23 != num24)
						{
							int num25 = this.Graph.AppendEdge(num23, num24, 1);
							if (num25 >= 0 && this.WantGraphEdgeInfo)
							{
								this.add_edge_edge(num25, num, new Index2i(triEdges[num20], triEdges[num21]), new Index2i(num23, num24));
							}
						}
					}
				}
			}
			this.Vertices = null;
		}

		private int add_or_append_vertex(Vector3d pos)
		{
			int num;
			if (!this.Vertices.TryGetValue(pos, out num))
			{
				num = this.Graph.AppendVertex(pos);
				this.Vertices.Add(pos, num);
			}
			return num;
		}

		private void add_edge_edge(int graph_eid, int mesh_tri, Index2i mesh_edges, Index2i order)
		{
			MeshIsoCurves.GraphEdgeInfo value = new MeshIsoCurves.GraphEdgeInfo
			{
				caseType = MeshIsoCurves.TriangleCase.EdgeEdge,
				mesh_edges = mesh_edges,
				mesh_tri = mesh_tri,
				order = order
			};
			this.GraphEdges.insertAt(value, graph_eid);
		}

		private void add_edge_vert(int graph_eid, int mesh_tri, int mesh_edge, int mesh_vert, Index2i order)
		{
			MeshIsoCurves.GraphEdgeInfo value = new MeshIsoCurves.GraphEdgeInfo
			{
				caseType = MeshIsoCurves.TriangleCase.EdgeVertex,
				mesh_edges = new Index2i(mesh_edge, mesh_vert),
				mesh_tri = mesh_tri,
				order = order
			};
			this.GraphEdges.insertAt(value, graph_eid);
		}

		private void add_on_edge(int graph_eid, int mesh_tri, int mesh_edge, Index2i order)
		{
			MeshIsoCurves.GraphEdgeInfo value = new MeshIsoCurves.GraphEdgeInfo
			{
				caseType = MeshIsoCurves.TriangleCase.OnEdge,
				mesh_edges = new Index2i(mesh_edge, -1),
				mesh_tri = mesh_tri,
				order = order
			};
			this.GraphEdges.insertAt(value, graph_eid);
		}

		private Vector3d find_crossing(Vector3d a, Vector3d b, double fA, double fB)
		{
			if (fB < fA)
			{
				Vector3d vector3d = a;
				a = b;
				b = vector3d;
				double num = fA;
				fA = fB;
				fB = num;
			}
			if (this.RootMode == MeshIsoCurves.RootfindingModes.Bisection)
			{
				for (int i = 0; i < this.RootModeSteps; i++)
				{
					Vector3d vector3d2 = Vector3d.Lerp(a, b, 0.5);
					double num2 = this.ValueF(vector3d2);
					if (num2 < 0.0)
					{
						fA = num2;
						a = vector3d2;
					}
					else
					{
						fB = num2;
						b = vector3d2;
					}
				}
				return Vector3d.Lerp(a, b, 0.5);
			}
			if (Math.Abs(fB - fA) < 1E-08)
			{
				return a;
			}
			double num3;
			if (this.RootMode == MeshIsoCurves.RootfindingModes.LerpSteps)
			{
				for (int j = 0; j < this.RootModeSteps; j++)
				{
					num3 = MathUtil.Clamp((0.0 - fA) / (fB - fA), 0.0, 1.0);
					Vector3d vector3d3 = (1.0 - num3) * a + num3 * b;
					double num4 = this.ValueF(vector3d3);
					if (num4 < 0.0)
					{
						fA = num4;
						a = vector3d3;
					}
					else
					{
						fB = num4;
						b = vector3d3;
					}
				}
			}
			num3 = MathUtil.Clamp((0.0 - fA) / (fB - fA), 0.0, 1.0);
			return (1.0 - num3) * a + num3 * b;
		}

		private void add_edge_pos(int a, int b, Vector3d crossing_pos)
		{
			int num = this.Mesh.FindEdge(a, b);
			if (num == -1)
			{
				throw new Exception("MeshIsoCurves.add_edge_split: invalid edge?");
			}
			if (this.EdgeLocations.ContainsKey(num))
			{
				return;
			}
			this.EdgeLocations[num] = crossing_pos;
		}

		public void SplitAtIsoCrossings(double min_len = 0.0)
		{
			foreach (KeyValuePair<int, Vector3d> keyValuePair in this.EdgeLocations)
			{
				int key = keyValuePair.Key;
				Vector3d value = keyValuePair.Value;
				if (this.Mesh.IsEdge(key))
				{
					Index2i edgeV = this.Mesh.GetEdgeV(key);
					Vector3d vertex = this.Mesh.GetVertex(edgeV.a);
					Vector3d vertex2 = this.Mesh.GetVertex(edgeV.b);
					if (vertex.Distance(vertex2) >= min_len)
					{
						Vector3d v = (vertex + vertex2) * 0.5;
						DMesh3.EdgeSplitInfo edgeSplitInfo;
						if (vertex.Distance(v) >= min_len && vertex2.Distance(v) >= min_len && this.Mesh.SplitEdge(key, out edgeSplitInfo, 0.5) == MeshResult.Ok)
						{
							this.Mesh.SetVertex(edgeSplitInfo.vNew, value);
						}
					}
				}
			}
		}

		public bool ShouldReverseGraphEdge(int graph_eid)
		{
			if (this.GraphEdges == null)
			{
				throw new Exception("MeshIsoCurves.OrientEdge: must track edge graph info to orient edge");
			}
			Index2i edgeV = this.Graph.GetEdgeV(graph_eid);
			MeshIsoCurves.GraphEdgeInfo graphEdgeInfo = this.GraphEdges[graph_eid];
			return edgeV.b == graphEdgeInfo.order.a && edgeV.a == graphEdgeInfo.order.b;
		}

		public DMesh3 Mesh;

		public Func<Vector3d, double> ValueF;

		public Func<int, double> VertexValueF;

		public bool PrecomputeVertexValues;

		public MeshIsoCurves.RootfindingModes RootMode;

		public int RootModeSteps = 5;

		public DGraph3 Graph;

		public bool WantGraphEdgeInfo;

		public DVector<MeshIsoCurves.GraphEdgeInfo> GraphEdges;

		private Dictionary<int, Vector3d> EdgeLocations = new Dictionary<int, Vector3d>();

		private Dictionary<Vector3d, int> Vertices;

		public enum RootfindingModes
		{
			SingleLerp,
			LerpSteps,
			Bisection
		}

		public enum TriangleCase
		{
			EdgeEdge = 1,
			EdgeVertex,
			OnEdge
		}

		public struct GraphEdgeInfo
		{
			public MeshIsoCurves.TriangleCase caseType;

			public int mesh_tri;

			public Index2i mesh_edges;

			public Index2i order;
		}
	}
}
