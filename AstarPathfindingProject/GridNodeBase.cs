using System;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding
{
	public abstract class GridNodeBase : GraphNode
	{
		protected GridNodeBase(AstarPath astar) : base(astar)
		{
		}

		public int NodeInGridIndex
		{
			get
			{
				return this.nodeInGridIndex & 16777215;
			}
			set
			{
				this.nodeInGridIndex = ((this.nodeInGridIndex & -16777216) | value);
			}
		}

		public int XCoordinateInGrid
		{
			get
			{
				return this.NodeInGridIndex % GridNode.GetGridGraph(base.GraphIndex).width;
			}
		}

		public int ZCoordinateInGrid
		{
			get
			{
				return this.NodeInGridIndex / GridNode.GetGridGraph(base.GraphIndex).width;
			}
		}

		public bool WalkableErosion
		{
			get
			{
				return (this.gridFlags & 256) > 0;
			}
			set
			{
				this.gridFlags = (ushort)(((int)this.gridFlags & -257) | (value ? 256 : 0));
			}
		}

		public bool TmpWalkable
		{
			get
			{
				return (this.gridFlags & 512) > 0;
			}
			set
			{
				this.gridFlags = (ushort)(((int)this.gridFlags & -513) | (value ? 512 : 0));
			}
		}

		public abstract bool HasConnectionsToAllEightNeighbours { get; }

		public override float SurfaceArea()
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			return gridGraph.nodeSize * gridGraph.nodeSize;
		}

		public override Vector3 RandomPointOnSurface()
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			Vector3 a = gridGraph.transform.InverseTransform((Vector3)this.position);
			return gridGraph.transform.Transform(a + new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f));
		}

		public Vector2 NormalizePoint(Vector3 worldPoint)
		{
			Vector3 vector = GridNode.GetGridGraph(base.GraphIndex).transform.InverseTransform(worldPoint);
			return new Vector2(vector.x - (float)this.XCoordinateInGrid, vector.z - (float)this.ZCoordinateInGrid);
		}

		public Vector3 UnNormalizePoint(Vector2 normalizedPointOnSurface)
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			return (Vector3)this.position + gridGraph.transform.TransformVector(new Vector3(normalizedPointOnSurface.x - 0.5f, 0f, normalizedPointOnSurface.y - 0.5f));
		}

		public override int GetGizmoHashCode()
		{
			int num = base.GetGizmoHashCode();
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					num ^= 17 * this.connections[i].GetHashCode();
				}
			}
			return num ^ (int)(109 * this.gridFlags);
		}

		public abstract GridNodeBase GetNeighbourAlongDirection(int direction);

		public virtual bool HasConnectionInDirection(int direction)
		{
			return this.GetNeighbourAlongDirection(direction) != null;
		}

		public override bool ContainsConnection(GraphNode node)
		{
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					if (this.connections[i].node == node)
					{
						return true;
					}
				}
			}
			for (int j = 0; j < 8; j++)
			{
				if (node == this.GetNeighbourAlongDirection(j))
				{
					return true;
				}
			}
			return false;
		}

		public void ClearCustomConnections(bool alsoReverse)
		{
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					this.connections[i].node.RemoveConnection(this);
				}
			}
			this.connections = null;
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		public override void ClearConnections(bool alsoReverse)
		{
			this.ClearCustomConnections(alsoReverse);
		}

		public override void GetConnections(Action<GraphNode> action)
		{
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					action(this.connections[i].node);
				}
			}
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			ushort pathID = handler.PathID;
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					GraphNode node = this.connections[i].node;
					PathNode pathNode2 = handler.GetPathNode(node);
					if (pathNode2.parent == pathNode && pathNode2.pathID == pathID)
					{
						node.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			ushort pathID = handler.PathID;
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					GraphNode node = this.connections[i].node;
					if (path.CanTraverse(node))
					{
						PathNode pathNode2 = handler.GetPathNode(node);
						uint cost = this.connections[i].cost;
						if (pathNode2.pathID != pathID)
						{
							pathNode2.parent = pathNode;
							pathNode2.pathID = pathID;
							pathNode2.cost = cost;
							pathNode2.H = path.CalculateHScore(node);
							pathNode2.UpdateG(path);
							handler.heap.Add(pathNode2);
						}
						else if (pathNode.G + cost + path.GetTraversalCost(node) < pathNode2.G)
						{
							pathNode2.cost = cost;
							pathNode2.parent = pathNode;
							node.UpdateRecursiveG(path, pathNode2, handler);
						}
					}
				}
			}
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			if (node == null)
			{
				throw new ArgumentNullException();
			}
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					if (this.connections[i].node == node)
					{
						this.connections[i].cost = cost;
						return;
					}
				}
			}
			int num = (this.connections != null) ? this.connections.Length : 0;
			Connection[] array = new Connection[num + 1];
			for (int j = 0; j < num; j++)
			{
				array[j] = this.connections[j];
			}
			array[num] = new Connection(node, cost, byte.MaxValue);
			this.connections = array;
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		public override void RemoveConnection(GraphNode node)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i].node == node)
				{
					int num = this.connections.Length;
					Connection[] array = new Connection[num - 1];
					for (int j = 0; j < i; j++)
					{
						array[j] = this.connections[j];
					}
					for (int k = i + 1; k < num; k++)
					{
						array[k - 1] = this.connections[k];
					}
					this.connections = array;
					AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
					return;
				}
			}
		}

		public override void SerializeReferences(GraphSerializationContext ctx)
		{
			if (this.connections == null)
			{
				ctx.writer.Write(-1);
				return;
			}
			ctx.writer.Write(this.connections.Length);
			for (int i = 0; i < this.connections.Length; i++)
			{
				ctx.SerializeNodeReference(this.connections[i].node);
				ctx.writer.Write(this.connections[i].cost);
			}
		}

		public override void DeserializeReferences(GraphSerializationContext ctx)
		{
			if (ctx.meta.version < AstarSerializer.V3_8_3)
			{
				return;
			}
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.connections = null;
				return;
			}
			this.connections = new Connection[num];
			for (int i = 0; i < num; i++)
			{
				this.connections[i] = new Connection(ctx.DeserializeNodeReference(), ctx.reader.ReadUInt32(), byte.MaxValue);
			}
		}

		private const int GridFlagsWalkableErosionOffset = 8;

		private const int GridFlagsWalkableErosionMask = 256;

		private const int GridFlagsWalkableTmpOffset = 9;

		private const int GridFlagsWalkableTmpMask = 512;

		protected const int NodeInGridIndexLayerOffset = 24;

		protected const int NodeInGridIndexMask = 16777215;

		protected int nodeInGridIndex;

		protected ushort gridFlags;

		public Connection[] connections;
	}
}
