using System;
using System.Collections.Generic;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding
{
	public class LevelGridNode : GridNodeBase
	{
		public LevelGridNode(AstarPath astar) : base(astar)
		{
		}

		public static LayerGridGraph GetGridGraph(uint graphIndex)
		{
			return LevelGridNode._gridGraphs[(int)graphIndex];
		}

		public static void SetGridGraph(int graphIndex, LayerGridGraph graph)
		{
			GridNode.SetGridGraph(graphIndex, graph);
			if (LevelGridNode._gridGraphs.Length <= graphIndex)
			{
				LayerGridGraph[] array = new LayerGridGraph[graphIndex + 1];
				for (int i = 0; i < LevelGridNode._gridGraphs.Length; i++)
				{
					array[i] = LevelGridNode._gridGraphs[i];
				}
				LevelGridNode._gridGraphs = array;
			}
			LevelGridNode._gridGraphs[graphIndex] = graph;
		}

		public void ResetAllGridConnections()
		{
			this.gridConnections = ulong.MaxValue;
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		public bool HasAnyGridConnections()
		{
			return this.gridConnections != ulong.MaxValue;
		}

		public override bool HasConnectionsToAllEightNeighbours
		{
			get
			{
				return false;
			}
		}

		public int LayerCoordinateInGrid
		{
			get
			{
				return this.nodeInGridIndex >> 24;
			}
			set
			{
				this.nodeInGridIndex = ((this.nodeInGridIndex & 16777215) | value << 24);
			}
		}

		public void SetPosition(Int3 position)
		{
			this.position = position;
		}

		public override int GetGizmoHashCode()
		{
			return base.GetGizmoHashCode() ^ (int)(805306457UL * this.gridConnections);
		}

		public override GridNodeBase GetNeighbourAlongDirection(int direction)
		{
			int connectionValue = this.GetConnectionValue(direction);
			if (connectionValue != 255)
			{
				LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
				return gridGraph.nodes[base.NodeInGridIndex + gridGraph.neighbourOffsets[direction] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
			}
			return null;
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse)
			{
				LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
				int[] neighbourOffsets = gridGraph.neighbourOffsets;
				GridNodeBase[] nodes = gridGraph.nodes;
				for (int i = 0; i < 4; i++)
				{
					int connectionValue = this.GetConnectionValue(i);
					if (connectionValue != 255)
					{
						LevelGridNode levelGridNode = nodes[base.NodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue] as LevelGridNode;
						if (levelGridNode != null)
						{
							levelGridNode.SetConnectionValue((i + 2) % 4, 255);
						}
					}
				}
			}
			this.ResetAllGridConnections();
			base.ClearConnections(alsoReverse);
		}

		public override void GetConnections(Action<GraphNode> action)
		{
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNodeBase[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					GraphNode graphNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					if (graphNode != null)
					{
						action(graphNode);
					}
				}
			}
			base.GetConnections(action);
		}

		[Obsolete("Use HasConnectionInDirection instead")]
		public bool GetConnection(int i)
		{
			return (this.gridConnections >> i * 8 & 255UL) != 255UL;
		}

		public override bool HasConnectionInDirection(int direction)
		{
			return (this.gridConnections >> direction * 8 & 255UL) != 255UL;
		}

		public void SetConnectionValue(int dir, int value)
		{
			this.gridConnections = ((this.gridConnections & ~(255UL << dir * 8)) | (ulong)((ulong)((long)value) << dir * 8));
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}

		public int GetConnectionValue(int dir)
		{
			return (int)(this.gridConnections >> dir * 8 & 255UL);
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			LevelGridNode levelGridNode = node as LevelGridNode;
			if (levelGridNode != null && levelGridNode.GraphIndex == base.GraphIndex)
			{
				this.RemoveGridConnection(levelGridNode);
			}
			base.AddConnection(node, cost);
		}

		public override void RemoveConnection(GraphNode node)
		{
			base.RemoveConnection(node);
			LevelGridNode levelGridNode = node as LevelGridNode;
			if (levelGridNode != null && levelGridNode.GraphIndex == base.GraphIndex)
			{
				this.RemoveGridConnection(levelGridNode);
			}
		}

		protected void RemoveGridConnection(LevelGridNode node)
		{
			int nodeInGridIndex = base.NodeInGridIndex;
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			for (int i = 0; i < 8; i++)
			{
				if (nodeInGridIndex + gridGraph.neighbourOffsets[i] == node.NodeInGridIndex && this.GetNeighbourAlongDirection(i) == node)
				{
					this.SetConnectionValue(i, 255);
					return;
				}
			}
		}

		public override bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			if (backwards)
			{
				return true;
			}
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNodeBase[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255 && other == nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue])
				{
					Vector3 a = (Vector3)(this.position + other.position) * 0.5f;
					Vector3 vector = Vector3.Cross(gridGraph.collision.up, (Vector3)(other.position - this.position));
					vector.Normalize();
					vector *= gridGraph.nodeSize * 0.5f;
					left.Add(a - vector);
					right.Add(a + vector);
					return true;
				}
			}
			return false;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			handler.heap.Add(pathNode);
			pathNode.UpdateG(path);
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNodeBase[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					GridNodeBase gridNodeBase = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					PathNode pathNode2 = handler.GetPathNode(gridNodeBase);
					if (pathNode2 != null && pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
					{
						gridNodeBase.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
			base.UpdateRecursiveG(path, pathNode, handler);
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			uint[] neighbourCosts = gridGraph.neighbourCosts;
			GridNodeBase[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					GraphNode graphNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					if (path.CanTraverse(graphNode))
					{
						PathNode pathNode2 = handler.GetPathNode(graphNode);
						if (pathNode2.pathID != handler.PathID)
						{
							pathNode2.parent = pathNode;
							pathNode2.pathID = handler.PathID;
							pathNode2.cost = neighbourCosts[i];
							pathNode2.H = path.CalculateHScore(graphNode);
							pathNode2.UpdateG(path);
							handler.heap.Add(pathNode2);
						}
						else
						{
							uint num = neighbourCosts[i];
							if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
							{
								pathNode2.cost = num;
								pathNode2.parent = pathNode;
								graphNode.UpdateRecursiveG(path, pathNode2, handler);
							}
						}
					}
				}
			}
			base.Open(path, pathNode, handler);
		}

		public override Vector3 ClosestPointOnNode(Vector3 p)
		{
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			p = gridGraph.transform.InverseTransform(p);
			int xcoordinateInGrid = base.XCoordinateInGrid;
			int zcoordinateInGrid = base.ZCoordinateInGrid;
			float y = gridGraph.transform.InverseTransform((Vector3)this.position).y;
			Vector3 point = new Vector3(Mathf.Clamp(p.x, (float)xcoordinateInGrid, (float)xcoordinateInGrid + 1f), y, Mathf.Clamp(p.z, (float)zcoordinateInGrid, (float)zcoordinateInGrid + 1f));
			return gridGraph.transform.Transform(point);
		}

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.SerializeInt3(this.position);
			ctx.writer.Write(this.gridFlags);
			ctx.writer.Write(this.gridConnections);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.position = ctx.DeserializeInt3();
			this.gridFlags = ctx.reader.ReadUInt16();
			if (ctx.meta.version < AstarSerializer.V3_9_0)
			{
				this.gridConnections = ((ulong)ctx.reader.ReadUInt32() | 18446744069414584320UL);
				return;
			}
			this.gridConnections = ctx.reader.ReadUInt64();
		}

		private static LayerGridGraph[] _gridGraphs = new LayerGridGraph[0];

		public ulong gridConnections;

		protected static LayerGridGraph[] gridGraphs;

		public const int NoConnection = 255;

		public const int ConnectionMask = 255;

		private const int ConnectionStride = 8;

		public const int MaxLayerCount = 255;
	}
}
