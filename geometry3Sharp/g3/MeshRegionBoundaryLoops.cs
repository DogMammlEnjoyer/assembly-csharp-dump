using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class MeshRegionBoundaryLoops : IEnumerable<EdgeLoop>, IEnumerable
	{
		public MeshRegionBoundaryLoops(DMesh3 mesh, int[] RegionTris, bool bAutoCompute = true)
		{
			this.Mesh = mesh;
			this.triangles = new IndexFlagSet(mesh.MaxTriangleID, RegionTris.Length);
			for (int i = 0; i < RegionTris.Length; i++)
			{
				this.triangles[RegionTris[i]] = true;
			}
			this.edges = new IndexFlagSet(mesh.MaxEdgeID, RegionTris.Length);
			foreach (int tID in RegionTris)
			{
				Index3i triEdges = this.Mesh.GetTriEdges(tID);
				for (int k = 0; k < 3; k++)
				{
					int num = triEdges[k];
					if (!this.edges.Contains(num))
					{
						Index2i edgeT = mesh.GetEdgeT(num);
						if (edgeT.b == -1 || this.triangles[edgeT.a] != this.triangles[edgeT.b])
						{
							this.edges.Add(num);
						}
					}
				}
			}
			if (bAutoCompute)
			{
				this.Compute();
			}
		}

		public int Count
		{
			get
			{
				return this.Loops.Count;
			}
		}

		public EdgeLoop this[int index]
		{
			get
			{
				return this.Loops[index];
			}
		}

		public IEnumerator<EdgeLoop> GetEnumerator()
		{
			return this.Loops.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Loops.GetEnumerator();
		}

		public int MaxVerticesLoopIndex
		{
			get
			{
				int num = 0;
				for (int i = 1; i < this.Loops.Count; i++)
				{
					if (this.Loops[i].Vertices.Length > this.Loops[num].Vertices.Length)
					{
						num = i;
					}
				}
				return num;
			}
		}

		private bool edge_is_boundary(int eid)
		{
			return this.edges.Contains(eid);
		}

		private bool edge_is_boundary(int eid, ref int tid_in, ref int tid_out)
		{
			if (!this.edges.Contains(eid))
			{
				return false;
			}
			tid_in = (tid_out = -1);
			Index2i edgeT = this.Mesh.GetEdgeT(eid);
			if (edgeT.b == -1)
			{
				tid_in = edgeT.a;
				tid_out = edgeT.b;
				return true;
			}
			bool flag = this.triangles[edgeT.a];
			bool flag2 = this.triangles[edgeT.b];
			if (flag != flag2)
			{
				tid_in = (flag ? edgeT.a : edgeT.b);
				tid_out = (flag ? edgeT.b : edgeT.a);
				return true;
			}
			return false;
		}

		private Index2i get_oriented_edgev(int eID, int tid_in, int tid_out)
		{
			Index2i edgeV = this.Mesh.GetEdgeV(eID);
			int a = edgeV.a;
			int b = edgeV.b;
			Index3i triangle = this.Mesh.GetTriangle(tid_in);
			int num = IndexUtil.find_edge_index_in_tri(a, b, ref triangle);
			return new Index2i(triangle[num], triangle[(num + 1) % 3]);
		}

		public int vertex_boundary_edges(int vID, ref int e0, ref int e1)
		{
			int num = 0;
			foreach (int num2 in this.Mesh.VtxEdgesItr(vID))
			{
				if (this.edge_is_boundary(num2))
				{
					if (num == 0)
					{
						e0 = num2;
					}
					else if (num == 1)
					{
						e1 = num2;
					}
					num++;
				}
			}
			return num;
		}

		public int all_vertex_boundary_edges(int vID, int[] e)
		{
			int result = 0;
			foreach (int num in this.Mesh.VtxEdgesItr(vID))
			{
				if (this.edge_is_boundary(num))
				{
					e[result++] = num;
				}
			}
			return result;
		}

		public bool Compute()
		{
			this.Loops = new List<EdgeLoop>();
			IndexFlagSet indexFlagSet = new IndexFlagSet(this.Mesh.MaxEdgeID, this.edges.Count);
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			int[] array = new int[16];
			foreach (int num in this.edges)
			{
				if (!indexFlagSet[num] && this.edge_is_boundary(num))
				{
					int num2 = num;
					indexFlagSet[num2] = true;
					list.Add(num2);
					int num3 = num;
					bool flag = false;
					while (!flag)
					{
						int tid_in = -1;
						int tid_out = -1;
						this.edge_is_boundary(num3, ref tid_in, ref tid_out);
						Index2i index2i = this.get_oriented_edgev(num3, tid_in, tid_out);
						int a = index2i.a;
						int b = index2i.b;
						list2.Add(a);
						int num4 = -1;
						int num5 = 1;
						int num6 = this.vertex_boundary_edges(b, ref num4, ref num5);
						if (num6 < 2)
						{
							throw new MeshBoundaryLoopsException("MeshRegionBoundaryLoops.Compute: found broken neighbourhood at vertex " + b.ToString())
							{
								UnclosedLoop = true
							};
						}
						int num7;
						if (num6 > 2)
						{
							if (b == list2[0])
							{
								num7 = -2;
							}
							else
							{
								if (num6 >= array.Length)
								{
									array = new int[num6];
								}
								int bdry_edges_count = this.all_vertex_boundary_edges(b, array);
								num7 = this.find_left_turn_edge(num3, b, array, bdry_edges_count, indexFlagSet);
								if (num7 == -1)
								{
									throw new MeshBoundaryLoopsException("MeshRegionBoundaryLoops.Compute: cannot find valid outgoing edge at bowtie vertex " + b.ToString())
									{
										BowtieFailure = true
									};
								}
							}
							if (!list3.Contains(b))
							{
								list3.Add(b);
							}
						}
						else
						{
							num7 = ((num4 == num3) ? num5 : num4);
						}
						if (num7 == -2)
						{
							flag = true;
						}
						else if (num7 == num2)
						{
							flag = true;
						}
						else
						{
							list.Add(num7);
							num3 = num7;
							indexFlagSet[num3] = true;
						}
					}
					if (list3.Count > 0)
					{
						List<EdgeLoop> list4 = this.extract_subloops(list2, list, list3);
						for (int i = 0; i < list4.Count; i++)
						{
							this.Loops.Add(list4[i]);
						}
					}
					else
					{
						EdgeLoop edgeLoop = new EdgeLoop(this.Mesh);
						edgeLoop.Vertices = list2.ToArray();
						edgeLoop.Edges = list.ToArray();
						this.Loops.Add(edgeLoop);
					}
					list.Clear();
					list2.Clear();
					list3.Clear();
				}
			}
			return true;
		}

		private Vector3d get_vtx_normal(int vid)
		{
			Vector3d vector3d = Vector3d.Zero;
			foreach (int tID in this.Mesh.VtxTrianglesItr(vid))
			{
				vector3d += this.Mesh.GetTriNormal(tID);
			}
			vector3d.Normalize(2.220446049250313E-16);
			return vector3d;
		}

		private int find_left_turn_edge(int incoming_e, int bowtie_v, int[] bdry_edges, int bdry_edges_count, IndexFlagSet used_edges)
		{
			Vector3d v = this.get_vtx_normal(bowtie_v);
			int vID = this.Mesh.edge_other_v(incoming_e, bowtie_v);
			Vector3d v2 = this.Mesh.GetVertex(bowtie_v) - this.Mesh.GetVertex(vID);
			int result = -1;
			double num = double.MaxValue;
			for (int i = 0; i < bdry_edges_count; i++)
			{
				int num2 = bdry_edges[i];
				if (!used_edges[num2])
				{
					int tid_in = -1;
					int tid_out = -1;
					this.edge_is_boundary(num2, ref tid_in, ref tid_out);
					Index2i index2i = this.get_oriented_edgev(num2, tid_in, tid_out);
					if (index2i.a == bowtie_v)
					{
						Vector3d v3 = this.Mesh.GetVertex(index2i.b) - this.Mesh.GetVertex(bowtie_v);
						float num3 = MathUtil.PlaneAngleSignedD((Vector3f)v2, (Vector3f)v3, (Vector3f)v);
						if (num == 1.7976931348623157E+308 || (double)num3 < num)
						{
							num = (double)num3;
							result = num2;
						}
					}
				}
			}
			return result;
		}

		private List<EdgeLoop> extract_subloops(List<int> loopV, List<int> loopE, List<int> bowties)
		{
			List<EdgeLoop> list = new List<EdgeLoop>();
			List<int> list2 = new List<int>();
			foreach (int item in bowties)
			{
				if (this.count_in_list(loopV, item) > 1)
				{
					list2.Add(item);
				}
			}
			if (list2.Count == 0)
			{
				list.Add(new EdgeLoop(this.Mesh)
				{
					Vertices = loopV.ToArray(),
					Edges = loopE.ToArray(),
					BowtieVertices = bowties.ToArray()
				});
				return list;
			}
			while (list2.Count > 0)
			{
				int i = 0;
				int num = 0;
				int i2 = -1;
				int i3 = -1;
				int num2 = -1;
				int num3 = int.MaxValue;
				while (i < list2.Count)
				{
					num = list2[i];
					if (this.is_simple_bowtie_loop(loopV, list2, num, out i2, out i3))
					{
						int num4 = this.count_span(loopV, i2, i3);
						if (num4 < num3)
						{
							num2 = num;
							num3 = num4;
						}
					}
					i++;
				}
				if (num2 == -1)
				{
					throw new MeshBoundaryLoopsException("MeshRegionBoundaryLoops.Compute: Cannot find a valid simple loop");
				}
				if (num != num2)
				{
					num = num2;
					this.is_simple_bowtie_loop(loopV, list2, num, out i2, out i3);
				}
				EdgeLoop edgeLoop = new EdgeLoop(this.Mesh);
				edgeLoop.Vertices = this.extract_span(loopV, i2, i3, true);
				edgeLoop.Edges = EdgeLoop.VertexLoopToEdgeLoop(this.Mesh, edgeLoop.Vertices);
				edgeLoop.BowtieVertices = bowties.ToArray();
				list.Add(edgeLoop);
				if (this.count_in_list(loopV, num) < 2)
				{
					list2.Remove(num);
				}
			}
			int num5 = 0;
			for (int j = 0; j < loopV.Count; j++)
			{
				if (loopV[j] != -1)
				{
					num5++;
				}
			}
			if (num5 > 0)
			{
				EdgeLoop edgeLoop2 = new EdgeLoop(this.Mesh);
				edgeLoop2.Vertices = new int[num5];
				int num6 = 0;
				for (int k = 0; k < loopV.Count; k++)
				{
					if (loopV[k] != -1)
					{
						edgeLoop2.Vertices[num6++] = loopV[k];
					}
				}
				edgeLoop2.Edges = EdgeLoop.VertexLoopToEdgeLoop(this.Mesh, edgeLoop2.Vertices);
				edgeLoop2.BowtieVertices = bowties.ToArray();
				list.Add(edgeLoop2);
			}
			return list;
		}

		private bool is_simple_bowtie_loop(List<int> loopV, List<int> bowties, int bowtieV, out int start_i, out int end_i)
		{
			start_i = this.find_index(loopV, 0, bowtieV);
			end_i = this.find_index(loopV, start_i + 1, bowtieV);
			if (this.is_simple_path(loopV, bowties, bowtieV, start_i, end_i))
			{
				return true;
			}
			if (this.is_simple_path(loopV, bowties, bowtieV, end_i, start_i))
			{
				int num = start_i;
				start_i = end_i;
				end_i = num;
				return true;
			}
			return false;
		}

		private bool is_simple_path(List<int> loopV, List<int> bowties, int bowtieV, int i1, int i2)
		{
			int count = loopV.Count;
			for (int num = i1; num != i2; num = (num + 1) % count)
			{
				int num2 = loopV[num];
				if (num2 != -1 && num2 != bowtieV && bowties.Contains(num2))
				{
					return false;
				}
			}
			return true;
		}

		private int[] extract_span(List<int> loop, int i0, int i1, bool bMarkInvalid)
		{
			int[] array = new int[this.count_span(loop, i0, i1)];
			int num = 0;
			int count = loop.Count;
			for (int num2 = i0; num2 != i1; num2 = (num2 + 1) % count)
			{
				if (loop[num2] != -1)
				{
					array[num++] = loop[num2];
					if (bMarkInvalid)
					{
						loop[num2] = -1;
					}
				}
			}
			return array;
		}

		private int count_span(List<int> l, int i0, int i1)
		{
			int num = 0;
			int count = l.Count;
			for (int num2 = i0; num2 != i1; num2 = (num2 + 1) % count)
			{
				if (l[num2] != -1)
				{
					num++;
				}
			}
			return num;
		}

		private int find_index(List<int> loop, int start, int item)
		{
			for (int i = start; i < loop.Count; i++)
			{
				if (loop[i] == item)
				{
					return i;
				}
			}
			return -1;
		}

		private int count_in_list(List<int> loop, int item)
		{
			int num = 0;
			for (int i = 0; i < loop.Count; i++)
			{
				if (loop[i] == item)
				{
					num++;
				}
			}
			return num;
		}

		public DMesh3 Mesh;

		public List<EdgeLoop> Loops;

		private IndexFlagSet triangles;

		private IndexFlagSet edges;
	}
}
