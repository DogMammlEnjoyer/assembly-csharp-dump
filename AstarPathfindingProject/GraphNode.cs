using System;
using System.Collections.Generic;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding
{
	public abstract class GraphNode
	{
		public NavGraph Graph
		{
			get
			{
				if (!this.Destroyed)
				{
					return AstarData.GetGraph(this);
				}
				return null;
			}
		}

		protected GraphNode(AstarPath astar)
		{
			if (astar != null)
			{
				this.nodeIndex = astar.GetNewNodeIndex();
				astar.InitializeNode(this);
				return;
			}
			throw new Exception("No active AstarPath object to bind to");
		}

		public void Destroy()
		{
			if (this.Destroyed)
			{
				return;
			}
			this.ClearConnections(true);
			if (AstarPath.active != null)
			{
				AstarPath.active.DestroyNode(this);
			}
			this.NodeIndex = 268435454;
		}

		public bool Destroyed
		{
			get
			{
				return this.NodeIndex == 268435454;
			}
		}

		public int NodeIndex
		{
			get
			{
				return this.nodeIndex & 268435455;
			}
			private set
			{
				this.nodeIndex = ((this.nodeIndex & -268435456) | value);
			}
		}

		internal bool TemporaryFlag1
		{
			get
			{
				return (this.nodeIndex & 268435456) != 0;
			}
			set
			{
				this.nodeIndex = ((this.nodeIndex & -268435457) | (value ? 268435456 : 0));
			}
		}

		internal bool TemporaryFlag2
		{
			get
			{
				return (this.nodeIndex & 536870912) != 0;
			}
			set
			{
				this.nodeIndex = ((this.nodeIndex & -536870913) | (value ? 536870912 : 0));
			}
		}

		public uint Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public uint Penalty
		{
			get
			{
				return this.penalty;
			}
			set
			{
				if (value > 16777215U)
				{
					Debug.LogWarning("Very high penalty applied. Are you sure negative values haven't underflowed?\nPenalty values this high could with long paths cause overflows and in some cases infinity loops because of that.\nPenalty value applied: " + value.ToString());
				}
				this.penalty = value;
			}
		}

		public bool Walkable
		{
			get
			{
				return (this.flags & 1U) > 0U;
			}
			set
			{
				this.flags = ((this.flags & 4294967294U) | (value ? 1U : 0U));
				AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
			}
		}

		internal int HierarchicalNodeIndex
		{
			get
			{
				return (int)((this.flags & 262142U) >> 1);
			}
			set
			{
				this.flags = ((this.flags & 4294705153U) | (uint)((uint)value << 1));
			}
		}

		internal bool IsHierarchicalNodeDirty
		{
			get
			{
				return (this.flags & 262144U) > 0U;
			}
			set
			{
				this.flags = ((this.flags & 4294705151U) | (value ? 1U : 0U) << 18);
			}
		}

		public uint Area
		{
			get
			{
				return AstarPath.active.hierarchicalGraph.GetConnectedComponent(this.HierarchicalNodeIndex);
			}
		}

		public uint GraphIndex
		{
			get
			{
				return (this.flags & 4278190080U) >> 24;
			}
			set
			{
				this.flags = ((this.flags & 16777215U) | value << 24);
			}
		}

		public uint Tag
		{
			get
			{
				return (this.flags & 16252928U) >> 19;
			}
			set
			{
				this.flags = ((this.flags & 4278714367U) | (value << 19 & 16252928U));
			}
		}

		public void SetConnectivityDirty()
		{
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		[Obsolete("This method is deprecated because it never did anything, you can safely remove any calls to this method")]
		public void RecalculateConnectionCosts()
		{
		}

		public virtual void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			pathNode.UpdateG(path);
			handler.heap.Add(pathNode);
			this.GetConnections(delegate(GraphNode other)
			{
				PathNode pathNode2 = handler.GetPathNode(other);
				if (pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
				{
					other.UpdateRecursiveG(path, pathNode2, handler);
				}
			});
		}

		public abstract void GetConnections(Action<GraphNode> action);

		public abstract void AddConnection(GraphNode node, uint cost);

		public abstract void RemoveConnection(GraphNode node);

		public abstract void ClearConnections(bool alsoReverse);

		public virtual bool ContainsConnection(GraphNode node)
		{
			bool contains = false;
			this.GetConnections(delegate(GraphNode neighbour)
			{
				contains |= (neighbour == node);
			});
			return contains;
		}

		public virtual bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			return false;
		}

		public abstract void Open(Path path, PathNode pathNode, PathHandler handler);

		public virtual float SurfaceArea()
		{
			return 0f;
		}

		public virtual Vector3 RandomPointOnSurface()
		{
			return (Vector3)this.position;
		}

		public abstract Vector3 ClosestPointOnNode(Vector3 p);

		public virtual int GetGizmoHashCode()
		{
			return this.position.GetHashCode() ^ (int)(19U * this.Penalty) ^ (int)(41U * (this.flags & 4294443009U));
		}

		public virtual void SerializeNode(GraphSerializationContext ctx)
		{
			ctx.writer.Write(this.Penalty);
			ctx.writer.Write(this.Flags & 4294443009U);
		}

		public virtual void DeserializeNode(GraphSerializationContext ctx)
		{
			this.Penalty = ctx.reader.ReadUInt32();
			this.Flags = ((ctx.reader.ReadUInt32() & 4294443009U) | (this.Flags & 524286U));
			this.GraphIndex = ctx.graphIndex;
		}

		public virtual void SerializeReferences(GraphSerializationContext ctx)
		{
		}

		public virtual void DeserializeReferences(GraphSerializationContext ctx)
		{
		}

		private int nodeIndex;

		protected uint flags;

		private uint penalty;

		private const int NodeIndexMask = 268435455;

		private const int DestroyedNodeIndex = 268435454;

		private const int TemporaryFlag1Mask = 268435456;

		private const int TemporaryFlag2Mask = 536870912;

		public Int3 position;

		private const int FlagsWalkableOffset = 0;

		private const uint FlagsWalkableMask = 1U;

		private const int FlagsHierarchicalIndexOffset = 1;

		private const uint HierarchicalIndexMask = 262142U;

		private const int HierarchicalDirtyOffset = 18;

		private const uint HierarchicalDirtyMask = 262144U;

		private const int FlagsGraphOffset = 24;

		private const uint FlagsGraphMask = 4278190080U;

		public const uint MaxHierarchicalNodeIndex = 131071U;

		public const uint MaxGraphIndex = 255U;

		private const int FlagsTagOffset = 19;

		private const uint FlagsTagMask = 16252928U;
	}
}
