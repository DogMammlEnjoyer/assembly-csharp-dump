using System;
using Pathfinding.Serialization;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public abstract class MeshNode : GraphNode
	{
		protected MeshNode(AstarPath astar) : base(astar)
		{
		}

		public abstract Int3 GetVertex(int i);

		public abstract int GetVertexCount();

		public abstract Vector3 ClosestPointOnNodeXZ(Vector3 p);

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse && this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					if (this.connections[i].node != null)
					{
						this.connections[i].node.RemoveConnection(this);
					}
				}
			}
			ArrayPool<Connection>.Release(ref this.connections, true);
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		public override void GetConnections(Action<GraphNode> action)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				action(this.connections[i].node);
			}
		}

		public override bool ContainsConnection(GraphNode node)
		{
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i].node == node)
				{
					return true;
				}
			}
			return false;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			pathNode.UpdateG(path);
			handler.heap.Add(pathNode);
			for (int i = 0; i < this.connections.Length; i++)
			{
				GraphNode node = this.connections[i].node;
				PathNode pathNode2 = handler.GetPathNode(node);
				if (pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
				{
					node.UpdateRecursiveG(path, pathNode2, handler);
				}
			}
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			this.AddConnection(node, cost, byte.MaxValue);
		}

		public void AddConnection(GraphNode node, uint cost, byte shapeEdge)
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
						this.connections[i].shapeEdge = ((shapeEdge != byte.MaxValue) ? shapeEdge : this.connections[i].shapeEdge);
						return;
					}
				}
			}
			int num = (this.connections != null) ? this.connections.Length : 0;
			Connection[] array = ArrayPool<Connection>.ClaimWithExactLength(num + 1);
			for (int j = 0; j < num; j++)
			{
				array[j] = this.connections[j];
			}
			array[num] = new Connection(node, cost, shapeEdge);
			if (this.connections != null)
			{
				ArrayPool<Connection>.Release(ref this.connections, true);
			}
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
					Connection[] array = ArrayPool<Connection>.ClaimWithExactLength(num - 1);
					for (int j = 0; j < i; j++)
					{
						array[j] = this.connections[j];
					}
					for (int k = i + 1; k < num; k++)
					{
						array[k - 1] = this.connections[k];
					}
					if (this.connections != null)
					{
						ArrayPool<Connection>.Release(ref this.connections, true);
					}
					this.connections = array;
					AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
					return;
				}
			}
		}

		public virtual bool ContainsPoint(Int3 point)
		{
			return this.ContainsPoint((Vector3)point);
		}

		public abstract bool ContainsPoint(Vector3 point);

		public abstract bool ContainsPointInGraphSpace(Int3 point);

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
			return num;
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
				ctx.writer.Write(this.connections[i].shapeEdge);
			}
		}

		public override void DeserializeReferences(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.connections = null;
				return;
			}
			this.connections = ArrayPool<Connection>.ClaimWithExactLength(num);
			for (int i = 0; i < num; i++)
			{
				this.connections[i] = new Connection(ctx.DeserializeNodeReference(), ctx.reader.ReadUInt32(), (ctx.meta.version < AstarSerializer.V4_1_0) ? byte.MaxValue : ctx.reader.ReadByte());
			}
		}

		public Connection[] connections;
	}
}
