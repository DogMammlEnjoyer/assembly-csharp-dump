using System;
using System.Collections.Generic;

namespace g3
{
	public class EdgeLoop
	{
		public EdgeLoop(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public EdgeLoop(DMesh3 mesh, int[] vertices, int[] edges, bool bCopyArrays)
		{
			this.Mesh = mesh;
			if (bCopyArrays)
			{
				this.Vertices = new int[vertices.Length];
				Array.Copy(vertices, this.Vertices, this.Vertices.Length);
				this.Edges = new int[edges.Length];
				Array.Copy(edges, this.Edges, this.Edges.Length);
				return;
			}
			this.Vertices = vertices;
			this.Edges = edges;
		}

		public EdgeLoop(EdgeLoop copy)
		{
			this.Mesh = copy.Mesh;
			this.Vertices = new int[copy.Vertices.Length];
			Array.Copy(copy.Vertices, this.Vertices, this.Vertices.Length);
			this.Edges = new int[copy.Edges.Length];
			Array.Copy(copy.Edges, this.Edges, this.Edges.Length);
			if (copy.BowtieVertices != null)
			{
				this.BowtieVertices = new int[copy.BowtieVertices.Length];
				Array.Copy(copy.BowtieVertices, this.BowtieVertices, this.BowtieVertices.Length);
			}
		}

		public static EdgeLoop FromEdges(DMesh3 mesh, IList<int> edges)
		{
			int[] array = new int[edges.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = edges[i];
			}
			int[] array2 = new int[array.Length];
			Index2i edgeV = mesh.GetEdgeV(array[0]);
			Index2i index2i = edgeV;
			for (int j = 1; j < array.Length; j++)
			{
				Index2i edgeV2 = mesh.GetEdgeV(array[j % array.Length]);
				array2[j] = IndexUtil.find_shared_edge_v(ref index2i, ref edgeV2);
				index2i = edgeV2;
			}
			array2[0] = IndexUtil.find_edge_other_v(ref edgeV, array2[1]);
			return new EdgeLoop(mesh, array2, array, false);
		}

		public static EdgeLoop FromVertices(DMesh3 mesh, IList<int> vertices)
		{
			int count = vertices.Count;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = vertices[i];
			}
			int num = count;
			int[] array2 = new int[num];
			for (int j = 0; j < num; j++)
			{
				array2[j] = mesh.FindEdge(array[j], array[(j + 1) % num]);
				if (array2[j] == -1)
				{
					throw new Exception("EdgeLoop.FromVertices: vertices are not connected by edge!");
				}
			}
			return new EdgeLoop(mesh, array, array2, false);
		}

		public static EdgeLoop FromVertices(DMesh3 mesh, IList<int> vertices, bool bAutoOrient = true)
		{
			int[] array = new int[vertices.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = vertices[i];
			}
			if (bAutoOrient)
			{
				int num = array[0];
				int num2 = array[1];
				int num3 = mesh.FindEdge(num, num2);
				if (mesh.IsBoundaryEdge(num3))
				{
					Index2i orientedBoundaryEdgeV = mesh.GetOrientedBoundaryEdgeV(num3);
					if (orientedBoundaryEdgeV.a == num2 && orientedBoundaryEdgeV.b == num)
					{
						Array.Reverse<int>(array);
					}
				}
			}
			int[] array2 = new int[array.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				int vA = array[j];
				int vB = array[(j + 1) % array.Length];
				array2[j] = mesh.FindEdge(vA, vB);
				if (array2[j] == -1)
				{
					throw new Exception(string.Concat(new string[]
					{
						"EdgeLoop.FromVertices: invalid edge [",
						vA.ToString(),
						",",
						vB.ToString(),
						"]"
					}));
				}
			}
			return new EdgeLoop(mesh, array, array2, false);
		}

		public int VertexCount
		{
			get
			{
				return this.Vertices.Length;
			}
		}

		public int EdgeCount
		{
			get
			{
				return this.Edges.Length;
			}
		}

		public Vector3d GetVertex(int i)
		{
			return this.Mesh.GetVertex(this.Vertices[i]);
		}

		public AxisAlignedBox3d GetBounds()
		{
			AxisAlignedBox3d empty = AxisAlignedBox3d.Empty;
			for (int i = 0; i < this.Vertices.Length; i++)
			{
				empty.Contain(this.Mesh.GetVertex(this.Vertices[i]));
			}
			return empty;
		}

		public DCurve3 ToCurve(DMesh3 sourceMesh = null)
		{
			if (sourceMesh == null)
			{
				sourceMesh = this.Mesh;
			}
			DCurve3 dcurve = MeshUtil.ExtractLoopV(sourceMesh, this.Vertices);
			dcurve.Closed = true;
			return dcurve;
		}

		public bool CorrectOrientation()
		{
			int num = this.Vertices[0];
			int num2 = this.Vertices[1];
			int num3 = this.Mesh.FindEdge(num, num2);
			if (this.Mesh.IsBoundaryEdge(num3))
			{
				Index2i orientedBoundaryEdgeV = this.Mesh.GetOrientedBoundaryEdgeV(num3);
				if (orientedBoundaryEdgeV.a == num2 && orientedBoundaryEdgeV.b == num)
				{
					this.Reverse();
					return true;
				}
			}
			return false;
		}

		public void Reverse()
		{
			Array.Reverse<int>(this.Vertices);
			Array.Reverse<int>(this.Edges);
		}

		public bool IsInternalLoop()
		{
			int num = this.Vertices.Length;
			for (int i = 0; i < num; i++)
			{
				int eid = this.Mesh.FindEdge(this.Vertices[i], this.Vertices[(i + 1) % num]);
				if (this.Mesh.IsBoundaryEdge(eid))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsBoundaryLoop(DMesh3 testMesh = null)
		{
			DMesh3 dmesh = (testMesh != null) ? testMesh : this.Mesh;
			int num = this.Vertices.Length;
			for (int i = 0; i < num; i++)
			{
				int eid = dmesh.FindEdge(this.Vertices[i], this.Vertices[(i + 1) % num]);
				if (!dmesh.IsBoundaryEdge(eid))
				{
					return false;
				}
			}
			return true;
		}

		public int FindVertexIndex(int vID)
		{
			int num = this.Vertices.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.Vertices[i] == vID)
				{
					return i;
				}
			}
			return -1;
		}

		public int FindNearestVertex(Vector3d v)
		{
			int result = -1;
			double num = double.MaxValue;
			int num2 = this.Vertices.Length;
			for (int i = 0; i < num2; i++)
			{
				Vector3d vertex = this.Mesh.GetVertex(this.Vertices[i]);
				double num3 = v.DistanceSquared(vertex);
				if (num3 < num)
				{
					num = num3;
					result = i;
				}
			}
			return result;
		}

		public int CountWithinTolerance(Vector3d v, double tol, out int last_in_tol)
		{
			last_in_tol = -1;
			int num = 0;
			int num2 = this.Vertices.Length;
			for (int i = 0; i < num2; i++)
			{
				Vector3d vertex = this.Mesh.GetVertex(this.Vertices[i]);
				if (v.Distance(vertex) < tol)
				{
					num++;
					last_in_tol = i;
				}
			}
			return num;
		}

		public bool IsSameLoop(EdgeLoop Loop2, bool bReverse2 = false, double tolerance = 1E-08)
		{
			int num = this.Vertices.Length;
			int num2 = Loop2.Vertices.Length;
			if (num != num2)
			{
				return false;
			}
			DMesh3 mesh = Loop2.Mesh;
			int num3 = 0;
			int num4 = -1;
			bool flag = false;
			while (!flag && num3 < num)
			{
				Vector3d vertex = this.Mesh.GetVertex(num3);
				if (Loop2.CountWithinTolerance(vertex, tolerance, out num4) == 1)
				{
					flag = true;
				}
				else
				{
					num3++;
				}
			}
			if (!flag)
			{
				return false;
			}
			for (int i = 0; i < num; i++)
			{
				int num5 = (num3 + i) % num;
				int num6 = bReverse2 ? MathUtil.WrapSignedIndex(num4 - i, num2) : ((num4 + i) % num2);
				Vector3d vertex2 = this.Mesh.GetVertex(this.Vertices[num5]);
				Vector3d vertex3 = mesh.GetVertex(Loop2.Vertices[num6]);
				if (vertex2.Distance(vertex3) > tolerance)
				{
					return false;
				}
			}
			return true;
		}

		public int[] GetVertexSpan(int starti, int count, int[] span, bool reverse = false)
		{
			int num = this.Vertices.Length;
			if (starti < 0 || starti >= num || count > num - 1)
			{
				return null;
			}
			if (reverse)
			{
				for (int i = 0; i < count; i++)
				{
					span[count - i - 1] = this.Vertices[(starti + i) % num];
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					span[j] = this.Vertices[(starti + j) % num];
				}
			}
			return span;
		}

		public bool CheckValidity(FailMode eFailMode = FailMode.Throw)
		{
			bool is_ok = true;
			Action<bool> action = delegate(bool b)
			{
				is_ok = (is_ok && b);
			};
			if (eFailMode == FailMode.DebugAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == FailMode.gDevAssert)
			{
				action = delegate(bool b)
				{
					is_ok = (is_ok && b);
				};
			}
			else if (eFailMode == FailMode.Throw)
			{
				action = delegate(bool b)
				{
					if (!b)
					{
						throw new Exception("EdgeLoop.CheckValidity: check failed");
					}
				};
			}
			action(this.Vertices.Length == this.Edges.Length);
			for (int i = 0; i < this.Edges.Length; i++)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(this.Edges[i]);
				action(this.Mesh.IsVertex(edgeV.a));
				action(this.Mesh.IsVertex(edgeV.b));
				action(this.Mesh.FindEdge(edgeV.a, edgeV.b) != -1);
				action(this.Vertices[i] == edgeV.a || this.Vertices[i] == edgeV.b);
				action(this.Vertices[(i + 1) % this.Edges.Length] == edgeV.a || this.Vertices[(i + 1) % this.Edges.Length] == edgeV.b);
			}
			for (int j = 0; j < this.Vertices.Length; j++)
			{
				int num = this.Vertices[j];
				int num2 = this.Vertices[(j + 1) % this.Vertices.Length];
				action(this.Mesh.IsVertex(num));
				action(this.Mesh.IsVertex(num2));
				action(this.Mesh.FindEdge(num, num2) != -1);
				int num3 = 0;
				int num4 = this.Edges[j];
				int num5 = this.Edges[(j + 1) % this.Vertices.Length];
				foreach (int num6 in this.Mesh.VtxEdgesItr(num2))
				{
					if (num6 == num4 || num6 == num5)
					{
						num3++;
					}
				}
				action(num3 == 2);
			}
			return is_ok;
		}

		public static int[] VertexLoopToEdgeLoop(DMesh3 mesh, int[] vertex_loop)
		{
			int num = vertex_loop.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				int vA = vertex_loop[i];
				int vB = vertex_loop[(i + 1) % num];
				array[i] = mesh.FindEdge(vA, vB);
			}
			return array;
		}

		public DMesh3 Mesh;

		public int[] Vertices;

		public int[] Edges;

		public int[] BowtieVertices;
	}
}
