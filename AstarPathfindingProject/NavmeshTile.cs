using System;
using Pathfinding.Util;

namespace Pathfinding
{
	public class NavmeshTile : INavmeshHolder, ITransformedGraph, INavmesh
	{
		public void GetTileCoordinates(int tileIndex, out int x, out int z)
		{
			x = this.x;
			z = this.z;
		}

		public int GetVertexArrayIndex(int index)
		{
			return index & 4095;
		}

		public Int3 GetVertex(int index)
		{
			int num = index & 4095;
			return this.verts[num];
		}

		public Int3 GetVertexInGraphSpace(int index)
		{
			return this.vertsInGraphSpace[index & 4095];
		}

		public GraphTransform transform
		{
			get
			{
				return this.graph.transform;
			}
		}

		public void GetNodes(Action<GraphNode> action)
		{
			if (this.nodes == null)
			{
				return;
			}
			for (int i = 0; i < this.nodes.Length; i++)
			{
				action(this.nodes[i]);
			}
		}

		public int[] tris;

		public Int3[] verts;

		public Int3[] vertsInGraphSpace;

		public int x;

		public int z;

		public int w;

		public int d;

		public TriangleMeshNode[] nodes;

		public BBTree bbTree;

		public bool flag;

		public NavmeshBase graph;
	}
}
