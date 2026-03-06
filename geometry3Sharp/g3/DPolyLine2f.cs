using System;
using System.Collections.Generic;

namespace g3
{
	public class DPolyLine2f
	{
		public DPolyLine2f()
		{
			this.m_vertices = new List<DPolyLine2f.Vertex>();
			this.m_edges = new List<DPolyLine2f.Edge>();
		}

		public DPolyLine2f(DPolyLine2f copy)
		{
			this.m_vertices = new List<DPolyLine2f.Vertex>(copy.m_vertices);
			this.m_edges = new List<DPolyLine2f.Edge>(copy.m_edges);
		}

		public List<DPolyLine2f.Edge> Edges
		{
			get
			{
				return this.m_edges;
			}
		}

		public List<DPolyLine2f.Vertex> Vertices
		{
			get
			{
				return this.m_vertices;
			}
		}

		public int VertexCount
		{
			get
			{
				return this.m_vertices.Count;
			}
		}

		public int EdgeCount
		{
			get
			{
				return this.m_edges.Count;
			}
		}

		public void Clear()
		{
			this.m_vertices.Clear();
			this.m_edges.Clear();
		}

		public DPolyLine2f.Vertex GetVertex(int i)
		{
			return this.m_vertices[i];
		}

		public int AddVertex(float fX, float fY)
		{
			int count = this.m_vertices.Count;
			this.m_vertices.Add(new DPolyLine2f.Vertex(fX, fY, count));
			return count;
		}

		public int AddEdge(int v1, int v2)
		{
			int count = this.m_edges.Count;
			this.m_edges.Add(new DPolyLine2f.Edge(v1, v2));
			return count;
		}

		public bool OrderVertices()
		{
			List<DPolyLine2f.Vertex> list = new List<DPolyLine2f.Vertex>(this.m_vertices.Count);
			List<DPolyLine2f.Edge> list2 = new List<DPolyLine2f.Edge>(this.m_edges.Count);
			int[] array = new int[2];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			list[num2++] = this.m_vertices[num];
			int num4 = -1;
			while (num2 != this.m_vertices.Count)
			{
				int num5 = 0;
				for (int i = 0; i < this.m_edges.Count; i++)
				{
					if (this.m_edges[i].v1 == num || this.m_edges[i].v2 == num)
					{
						if (num5 > 1)
						{
							return false;
						}
						array[num5++] = i;
					}
				}
				if (num5 != 2)
				{
					return false;
				}
				int num6;
				if (num4 == -1)
				{
					num6 = 0;
				}
				else if (array[0] == num4)
				{
					num6 = array[1];
				}
				else
				{
					if (array[1] != num4)
					{
						return false;
					}
					num6 = array[0];
				}
				int num7 = (this.m_edges[num6].v1 == num) ? this.m_edges[num6].v2 : this.m_edges[num6].v1;
				list[num2++] = this.m_vertices[num7];
				list2[num3++] = new DPolyLine2f.Edge(num, num7);
				num = num7;
				num4 = num6;
			}
			list2[num3++] = new DPolyLine2f.Edge(num, 0);
			this.m_edges = list2;
			this.m_vertices = list;
			return true;
		}

		private List<DPolyLine2f.Vertex> m_vertices;

		private List<DPolyLine2f.Edge> m_edges;

		public struct Edge
		{
			public Edge(int vertex1, int vertex2)
			{
				this.v1 = vertex1;
				this.v2 = vertex2;
			}

			public int v1;

			public int v2;
		}

		public struct Vertex
		{
			public Vertex(float fX, float fY, int nIndex)
			{
				this.x = fX;
				this.y = fY;
				this.index = nIndex;
			}

			public int index;

			public float x;

			public float y;
		}
	}
}
