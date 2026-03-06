using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public class GraphUpdateObject
	{
		[Obsolete("Not necessary anymore")]
		public bool requiresFloodFill
		{
			set
			{
			}
		}

		public GraphUpdateStage stage
		{
			get
			{
				switch (this.internalStage)
				{
				case -3:
					return GraphUpdateStage.Aborted;
				case -1:
					return GraphUpdateStage.Created;
				case 0:
					return GraphUpdateStage.Applied;
				}
				return GraphUpdateStage.Pending;
			}
		}

		public virtual void WillUpdateNode(GraphNode node)
		{
			if (this.trackChangedNodes && node != null)
			{
				if (this.changedNodes == null)
				{
					this.changedNodes = ListPool<GraphNode>.Claim();
					this.backupData = ListPool<uint>.Claim();
					this.backupPositionData = ListPool<Int3>.Claim();
				}
				this.changedNodes.Add(node);
				this.backupPositionData.Add(node.position);
				this.backupData.Add(node.Penalty);
				this.backupData.Add(node.Flags);
				GridNode gridNode = node as GridNode;
				if (gridNode != null)
				{
					this.backupData.Add((uint)gridNode.InternalGridFlags);
				}
			}
		}

		public virtual void RevertFromBackup()
		{
			if (!this.trackChangedNodes)
			{
				throw new InvalidOperationException("Changed nodes have not been tracked, cannot revert from backup. Please set trackChangedNodes to true before applying the update.");
			}
			if (this.changedNodes == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < this.changedNodes.Count; i++)
			{
				this.changedNodes[i].Penalty = this.backupData[num];
				num++;
				int hierarchicalNodeIndex = this.changedNodes[i].HierarchicalNodeIndex;
				this.changedNodes[i].Flags = this.backupData[num];
				this.changedNodes[i].HierarchicalNodeIndex = hierarchicalNodeIndex;
				num++;
				GridNode gridNode = this.changedNodes[i] as GridNode;
				if (gridNode != null)
				{
					gridNode.InternalGridFlags = (ushort)this.backupData[num];
					num++;
				}
				this.changedNodes[i].position = this.backupPositionData[i];
				this.changedNodes[i].SetConnectivityDirty();
			}
			ListPool<GraphNode>.Release(ref this.changedNodes);
			ListPool<uint>.Release(ref this.backupData);
			ListPool<Int3>.Release(ref this.backupPositionData);
		}

		public virtual void Apply(GraphNode node)
		{
			if (this.shape == null || this.shape.Contains(node))
			{
				node.Penalty = (uint)((ulong)node.Penalty + (ulong)((long)this.addPenalty));
				if (this.modifyWalkability)
				{
					node.Walkable = this.setWalkability;
				}
				if (this.modifyTag)
				{
					node.Tag = (uint)this.setTag;
				}
			}
		}

		public GraphUpdateObject()
		{
		}

		public GraphUpdateObject(Bounds b)
		{
			this.bounds = b;
		}

		public Bounds bounds;

		public bool updatePhysics = true;

		public bool resetPenaltyOnPhysics = true;

		public bool updateErosion = true;

		public NNConstraint nnConstraint = NNConstraint.None;

		public int addPenalty;

		public bool modifyWalkability;

		public bool setWalkability;

		public bool modifyTag;

		public int setTag;

		public bool trackChangedNodes;

		public List<GraphNode> changedNodes;

		private List<uint> backupData;

		private List<Int3> backupPositionData;

		public GraphUpdateShape shape;

		internal int internalStage = -1;

		internal const int STAGE_CREATED = -1;

		internal const int STAGE_PENDING = -2;

		internal const int STAGE_ABORTED = -3;

		internal const int STAGE_APPLIED = 0;
	}
}
