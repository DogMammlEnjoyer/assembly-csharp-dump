using System;
using System.Collections.Generic;

namespace g3
{
	public class EdgeSpan
	{
		public EdgeSpan(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public EdgeSpan(DMesh3 mesh, int[] vertices, int[] edges, bool bCopyArrays)
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

		public static EdgeSpan FromEdges(DMesh3 mesh, IList<int> edges)
		{
			int[] array = new int[edges.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = edges[i];
			}
			int[] array2 = new int[array.Length + 1];
			Index2i edgeV = mesh.GetEdgeV(array[0]);
			Index2i ev = edgeV;
			if (array.Length > 1)
			{
				for (int j = 1; j < array.Length; j++)
				{
					Index2i edgeV2 = mesh.GetEdgeV(array[j]);
					array2[j] = IndexUtil.find_shared_edge_v(ref ev, ref edgeV2);
					ev = edgeV2;
				}
				array2[0] = IndexUtil.find_edge_other_v(ref edgeV, array2[1]);
				array2[array2.Length - 1] = IndexUtil.find_edge_other_v(ev, array2[array2.Length - 2]);
			}
			else
			{
				array2[0] = edgeV[0];
				array2[1] = edgeV[1];
			}
			return new EdgeSpan(mesh, array2, array, false);
		}

		public static EdgeSpan FromVertices(DMesh3 mesh, IList<int> vertices)
		{
			int count = vertices.Count;
			int[] array = new int[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = vertices[i];
			}
			int num = count - 1;
			int[] array2 = new int[num];
			for (int j = 0; j < num; j++)
			{
				array2[j] = mesh.FindEdge(array[j], array[j + 1]);
				if (array2[j] == -1)
				{
					throw new Exception("EdgeSpan.FromVertices: vertices are not connected by edge!");
				}
			}
			return new EdgeSpan(mesh, array, array2, false);
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
			dcurve.Closed = false;
			return dcurve;
		}

		public bool IsInternalSpan()
		{
			int num = this.Vertices.Length;
			for (int i = 0; i < num - 1; i++)
			{
				int eid = this.Mesh.FindEdge(this.Vertices[i], this.Vertices[i + 1]);
				if (this.Mesh.IsBoundaryEdge(eid))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsBoundarySpan(DMesh3 testMesh = null)
		{
			DMesh3 dmesh = (testMesh != null) ? testMesh : this.Mesh;
			int num = this.Vertices.Length;
			for (int i = 0; i < num - 1; i++)
			{
				int eid = dmesh.FindEdge(this.Vertices[i], this.Vertices[i + 1]);
				if (!dmesh.IsBoundaryEdge(eid))
				{
					return false;
				}
			}
			return true;
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

		public bool IsSameSpan(EdgeSpan Spanw, bool bReverse2 = false, double tolerance = 1E-08)
		{
			throw new NotImplementedException("todo!");
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
						throw new Exception("EdgeSpan.CheckValidity: check failed");
					}
				};
			}
			action(this.Vertices.Length == this.Edges.Length + 1);
			for (int i = 0; i < this.Edges.Length; i++)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(this.Edges[i]);
				action(this.Mesh.IsVertex(edgeV.a));
				action(this.Mesh.IsVertex(edgeV.b));
				action(this.Mesh.FindEdge(edgeV.a, edgeV.b) != -1);
				action(this.Vertices[i] == edgeV.a || this.Vertices[i] == edgeV.b);
				action(this.Vertices[i + 1] == edgeV.a || this.Vertices[i + 1] == edgeV.b);
			}
			for (int j = 0; j < this.Vertices.Length - 1; j++)
			{
				int num = this.Vertices[j];
				int num2 = this.Vertices[j + 1];
				action(this.Mesh.IsVertex(num));
				action(this.Mesh.IsVertex(num2));
				action(this.Mesh.FindEdge(num, num2) != -1);
				if (j < this.Vertices.Length - 2)
				{
					int num3 = 0;
					int num4 = this.Edges[j];
					int num5 = this.Edges[j + 1];
					foreach (int num6 in this.Mesh.VtxEdgesItr(num2))
					{
						if (num6 == num4 || num6 == num5)
						{
							num3++;
						}
					}
					action(num3 == 2);
				}
			}
			return true;
		}

		public static int[] VerticesToEdges(DMesh3 mesh, int[] vertex_span)
		{
			int num = vertex_span.Length;
			int[] array = new int[num - 1];
			for (int i = 0; i < num - 1; i++)
			{
				int vA = vertex_span[i];
				int vB = vertex_span[i + 1];
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
