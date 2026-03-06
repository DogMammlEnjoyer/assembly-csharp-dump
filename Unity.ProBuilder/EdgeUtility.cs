using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	internal static class EdgeUtility
	{
		public static IEnumerable<Edge> GetSharedVertexHandleEdges(this ProBuilderMesh mesh, IEnumerable<Edge> edges)
		{
			return from x in edges
			select mesh.GetSharedVertexHandleEdge(x);
		}

		public static Edge GetSharedVertexHandleEdge(this ProBuilderMesh mesh, Edge edge)
		{
			return new Edge(mesh.sharedVertexLookup[edge.a], mesh.sharedVertexLookup[edge.b]);
		}

		internal static Edge GetEdgeWithSharedVertexHandles(this ProBuilderMesh mesh, Edge edge)
		{
			return new Edge(mesh.sharedVerticesInternal[edge.a][0], mesh.sharedVerticesInternal[edge.b][0]);
		}

		public static bool ValidateEdge(ProBuilderMesh mesh, Edge edge, out SimpleTuple<Face, Edge> validEdge)
		{
			Face[] facesInternal = mesh.facesInternal;
			SharedVertex[] sharedVerticesInternal = mesh.sharedVerticesInternal;
			Edge sharedVertexHandleEdge = mesh.GetSharedVertexHandleEdge(edge);
			for (int i = 0; i < facesInternal.Length; i++)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				if (facesInternal[i].distinctIndexesInternal.ContainsMatch(sharedVerticesInternal[sharedVertexHandleEdge.a].arrayInternal, out num, out num3) && facesInternal[i].distinctIndexesInternal.ContainsMatch(sharedVerticesInternal[sharedVertexHandleEdge.b].arrayInternal, out num2, out num4))
				{
					int a = facesInternal[i].distinctIndexesInternal[num];
					int b = facesInternal[i].distinctIndexesInternal[num2];
					validEdge = new SimpleTuple<Face, Edge>(facesInternal[i], new Edge(a, b));
					return true;
				}
			}
			validEdge = default(SimpleTuple<Face, Edge>);
			return false;
		}

		internal static bool Contains(this Edge[] edges, Edge edge)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				if (edges[i].Equals(edge))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool Contains(this Edge[] edges, int x, int y)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				if ((x == edges[i].a && y == edges[i].b) || (x == edges[i].b && y == edges[i].a))
				{
					return true;
				}
			}
			return false;
		}

		internal static int IndexOf(this ProBuilderMesh mesh, IList<Edge> edges, Edge edge)
		{
			for (int i = 0; i < edges.Count; i++)
			{
				if (edges[i].Equals(edge, mesh.sharedVertexLookup))
				{
					return i;
				}
			}
			return -1;
		}

		internal static int[] AllTriangles(this Edge[] edges)
		{
			int[] array = new int[edges.Length * 2];
			int num = 0;
			for (int i = 0; i < edges.Length; i++)
			{
				array[num++] = edges[i].a;
				array[num++] = edges[i].b;
			}
			return array;
		}

		internal static Face GetFace(this ProBuilderMesh mesh, Edge edge)
		{
			Face result = null;
			foreach (Face face in mesh.facesInternal)
			{
				Edge[] edgesInternal = face.edgesInternal;
				int j = 0;
				int num = edgesInternal.Length;
				while (j < num)
				{
					if (edge.Equals(edgesInternal[j]))
					{
						return face;
					}
					if (edgesInternal.Contains(edgesInternal[j]))
					{
						result = face;
					}
					j++;
				}
			}
			return result;
		}
	}
}
