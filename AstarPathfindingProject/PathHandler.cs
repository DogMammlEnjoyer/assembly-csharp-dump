using System;
using System.Text;

namespace Pathfinding
{
	public class PathHandler
	{
		public ushort PathID
		{
			get
			{
				return this.pathID;
			}
		}

		public PathHandler(int threadID, int totalThreadCount)
		{
			this.threadID = threadID;
			this.totalThreadCount = totalThreadCount;
		}

		public void InitializeForPath(Path p)
		{
			this.pathID = p.pathID;
			this.heap.Clear();
		}

		public void DestroyNode(GraphNode node)
		{
			PathNode pathNode = this.GetPathNode(node);
			pathNode.node = null;
			pathNode.parent = null;
			pathNode.pathID = 0;
			pathNode.G = 0U;
			pathNode.H = 0U;
		}

		public void InitializeNode(GraphNode node)
		{
			int nodeIndex = node.NodeIndex;
			if (nodeIndex >= this.nodes.Length)
			{
				PathNode[] array = new PathNode[Math.Max(128, this.nodes.Length * 2)];
				this.nodes.CopyTo(array, 0);
				for (int i = this.nodes.Length; i < array.Length; i++)
				{
					array[i] = new PathNode();
				}
				this.nodes = array;
			}
			this.nodes[nodeIndex].node = node;
		}

		public PathNode GetPathNode(int nodeIndex)
		{
			return this.nodes[nodeIndex];
		}

		public PathNode GetPathNode(GraphNode node)
		{
			return this.nodes[node.NodeIndex];
		}

		public void ClearPathIDs()
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] != null)
				{
					this.nodes[i].pathID = 0;
				}
			}
		}

		private ushort pathID;

		public readonly int threadID;

		public readonly int totalThreadCount;

		public readonly BinaryHeap heap = new BinaryHeap(128);

		public PathNode[] nodes = new PathNode[0];

		public readonly StringBuilder DebugStringBuilder = new StringBuilder();
	}
}
