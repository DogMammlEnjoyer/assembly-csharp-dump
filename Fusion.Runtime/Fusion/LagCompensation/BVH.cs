using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion.LagCompensation
{
	internal class BVH : ILagCompensationBroadphase
	{
		internal ref BVHNode rootBVH
		{
			get
			{
				return ref this._nodes[1];
			}
		}

		public void CopyFrom(ILagCompensationBroadphase other)
		{
			BVH bvh = (BVH)other;
			bool flag = bvh._nodesCount > this._nodes.Length;
			if (flag)
			{
				this.ResizeNodesArray(bvh._nodesCount - this._nodes.Length);
			}
			Array.Clear(this._nodes, 0, this._nodes.Length);
			Array.Copy(bvh._nodes, 0, this._nodes, 0, bvh._nodesCount);
			this.maxDepth = bvh.maxDepth;
			this._nodesCount = bvh._nodesCount;
			this._usedNodesCount = bvh._usedNodesCount;
			this._freeNodesHead = bvh._freeNodesHead;
		}

		internal int UsedNodesCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._usedNodesCount;
			}
		}

		internal ref BVHNode GetNextNode(out int index)
		{
			bool flag = this._freeNodesHead == 0;
			if (flag)
			{
				int nodesCount = this._nodesCount;
				this._nodesCount = nodesCount + 1;
				index = nodesCount;
			}
			else
			{
				index = this._freeNodesHead;
				this._freeNodesHead = this._nodes[this._freeNodesHead].Next;
			}
			ref BVHNode ptr = ref this._nodes[index];
			Assert.Check<int>(!ptr.Used, "Retrieving a node that is already marked as used {0}", index);
			ptr = default(BVHNode);
			ptr.Used = true;
			this._usedNodesCount++;
			return ref ptr;
		}

		internal void DisposeNode(int index)
		{
			Assert.Check<int>(this._nodes[index].Used, "Disposing a node that is not marked as Used. {0}", index);
			ref BVHNode ptr = ref this._nodes[index];
			ptr.Used = false;
			ptr.Next = this._freeNodesHead;
			this._freeNodesHead = index;
			this._usedNodesCount--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref BVHNode GetNode(int index)
		{
			return ref this._nodes[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(HitboxRoot changed, int tick)
		{
			int index;
			bool flag = this.Mapper.TryGetLeafIndex(changed, out index);
			if (flag)
			{
				ref BVHNode node = ref this.GetNode(index);
				Assert.Check(BehaviourUtils.IsSame(node._root, changed));
				node.RefitObjectChanged(this);
			}
		}

		public void Traverse(IBoundsTraversalTest hitTest, HashSet<HitboxRoot> candidateRoots, int layerMask)
		{
			this.TraverseInternal(this.rootBVH, hitTest, candidateRoots, layerMask);
		}

		private void TraverseInternal(ref BVHNode curNode, IBoundsTraversalTest hitTest, HashSet<HitboxRoot> candidateRoots, int layermask)
		{
			bool flag = !curNode.IsValid || !hitTest.Check(ref curNode._cachedBounds);
			if (!flag)
			{
				bool flag2 = curNode.IsLeaf && curNode.Active && curNode.HasValidRoot;
				if (flag2)
				{
					candidateRoots.Add(curNode._root);
				}
				this.TraverseInternal(curNode.GetLeft(this), hitTest, candidateRoots, layermask);
				this.TraverseInternal(curNode.GetRight(this), hitTest, candidateRoots, layermask);
			}
		}

		public void PosUpdateRefit()
		{
			foreach (int index in this.refitNodes)
			{
				this.GetNode(index).ChildRefit(this, true);
			}
			this.refitNodes.Clear();
		}

		public void Add(HitboxRoot root)
		{
			Bounds bounds = root.GetBounds();
			float newObSah = BVHNode.SA(ref bounds);
			bool flag = this._nodesCount >= this._nodes.Length;
			if (flag)
			{
				this.ResizeNodesArray(this._nodes.Length);
			}
			BVHNode.Add(this, this.rootBVH, root, ref bounds, newObSah);
		}

		internal static Bounds BoundsFromSphere(Vector3 pos, float radius)
		{
			return new Bounds
			{
				min = new Vector3(pos.x - radius, pos.y - radius, pos.z - radius),
				max = new Vector3(pos.x + radius, pos.y + radius, pos.z + radius)
			};
		}

		public bool Remove(HitboxRoot root)
		{
			int index;
			bool flag = this.Mapper.TryGetLeafIndex(root, out index);
			bool result;
			if (flag)
			{
				this.GetNode(index).Remove(this, root);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal BVH(Mapper mapper, int nodesCapacity, List<HitboxRoot> initialEntries = null, float expansionFactor = 0.15f, int parentsToExpand = 3)
		{
			this._nodes = new BVHNode[Mathf.Max(32, nodesCapacity)];
			this.Mapper = mapper;
			this.ExpansionFactor = expansionFactor;
			this.ParentsToExpand = parentsToExpand;
			int num;
			ref BVHNode nextNode = ref this.GetNextNode(out num);
			Assert.Check(num == 1);
			BVHNode.InitNode(ref nextNode, this, num, 0, 0, initialEntries);
			Assert.Check(nextNode.IsRootNode);
		}

		internal void BuildNodesLog(StringBuilder builder)
		{
			builder.AppendLine(string.Format("Nodes count: {0}, Used nodes: {1}", this._nodesCount, this.UsedNodesCount));
			for (int i = 0; i < this._nodesCount; i++)
			{
				builder.Append(string.Format("[{0}]: ", i));
				this._nodes[i].BuildLog(builder);
				builder.AppendLine();
			}
		}

		private void ResizeNodesArray(int minimumIncrease)
		{
			int newSize = this._nodes.Length * Math.Max(2, Mathf.FloorToInt((float)minimumIncrease / (float)this._nodes.Length + 1f));
			Array.Resize<BVHNode>(ref this._nodes, newSize);
		}

		internal BVHNode[] _nodes;

		internal Mapper Mapper;

		internal int maxDepth = 0;

		internal HashSet<int> refitNodes = new HashSet<int>();

		internal readonly List<HitboxRoot> ReusableList = new List<HitboxRoot>(2);

		private int _nodesCount = 1;

		private int _usedNodesCount = 0;

		private int _freeNodesHead = 0;

		private const float DEFAULT_EXPANSION_FACTOR = 0.15f;

		private const int DEFAULT_PARENTS_TO_EXPAND = 3;

		internal float ExpansionFactor;

		internal int ParentsToExpand;
	}
}
