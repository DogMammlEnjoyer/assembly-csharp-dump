using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion.LagCompensation
{
	internal struct BVHNode
	{
		internal int Index
		{
			get
			{
				return this._nodeIndex;
			}
		}

		internal bool IsValid
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._nodeIndex > 0;
			}
		}

		internal bool IsRootNode
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._nodeIndex == 1;
			}
		}

		internal bool HasParent
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._parentIndex > 0;
			}
		}

		internal bool HasLeft
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._leftIndex > 0;
			}
		}

		internal bool HasRight
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._rightIndex > 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref BVHNode GetParent(BVH bvh)
		{
			return ref bvh._nodes[this._parentIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref BVHNode GetRight(BVH bvh)
		{
			return ref bvh._nodes[this._rightIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref BVHNode GetLeft(BVH bvh)
		{
			return ref bvh._nodes[this._leftIndex];
		}

		public override string ToString()
		{
			return string.Format("BVHNode: {0}", this._nodeIndex);
		}

		internal bool IsLeaf
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._isLeaf;
			}
		}

		internal bool HasValidRoot
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return BehaviourUtils.IsAlive(this._root) && BehaviourUtils.IsAlive(this._root.Object);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RefitObjectChanged(BVH bvh)
		{
			Assert.Check(BehaviourUtils.IsAlive(this._root));
			this.Active = this._root.HitboxRootActive;
			bool flag = this.Active && this.RefitVolume(bvh);
			if (flag)
			{
				bool hasParent = this.HasParent;
				if (hasParent)
				{
					bvh.refitNodes.Add(this._parentIndex);
				}
			}
		}

		private void ExpandVolume(BVH bvh, Vector3 objectpos, float radius, ref Bounds bounds, bool expandParent)
		{
			bool flag = false;
			bool flag2 = objectpos.x - radius < bounds.min.x;
			if (flag2)
			{
				bounds.min = new Vector3(objectpos.x - radius, bounds.min.y, bounds.min.z);
				flag = true;
			}
			bool flag3 = objectpos.x + radius > bounds.max.x;
			if (flag3)
			{
				bounds.max = new Vector3(objectpos.x + radius, bounds.max.y, bounds.max.z);
				flag = true;
			}
			bool flag4 = objectpos.y - radius < bounds.min.y;
			if (flag4)
			{
				bounds.min = new Vector3(bounds.min.x, objectpos.y - radius, bounds.min.z);
				flag = true;
			}
			bool flag5 = objectpos.y + radius > bounds.max.y;
			if (flag5)
			{
				bounds.max = new Vector3(bounds.max.x, objectpos.y + radius, bounds.max.z);
				flag = true;
			}
			bool flag6 = objectpos.z - radius < bounds.min.z;
			if (flag6)
			{
				bounds.min = new Vector3(bounds.min.x, bounds.min.y, objectpos.z - radius);
				flag = true;
			}
			bool flag7 = objectpos.z + radius > bounds.max.z;
			if (flag7)
			{
				bounds.max = new Vector3(bounds.max.x, bounds.max.y, objectpos.z + radius);
				flag = true;
			}
			bool flag8 = expandParent && flag && this.HasParent;
			if (flag8)
			{
				this.GetParent(bvh).ChildExpanded(bvh, ref this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AssignVolume(Vector3 pos, float radius, ref Bounds bounds)
		{
			bounds.min = new Vector3(pos.x - radius, pos.y - radius, pos.z - radius);
			bounds.max = new Vector3(pos.x + radius, pos.y + radius, pos.z + radius);
		}

		private void ComputeVolume(BVH bvh)
		{
			bool flag = BehaviourUtils.IsAlive(this._root);
			if (flag)
			{
				this.AssignVolume(this._root.CachedTransform.TransformPoint(this._root.Offset), this._root.BroadRadius, ref this.Box);
			}
			else
			{
				BVHNode.ChildRefit(bvh, this.Index, false);
			}
		}

		private Bounds ComputeMinVolume(BVH bvh)
		{
			bool flag = BehaviourUtils.IsAlive(this._root);
			Bounds result;
			if (flag)
			{
				Bounds bounds = default(Bounds);
				this.AssignVolume(this._root.CachedTransform.TransformPoint(this._root.Offset), this._root.BroadRadius, ref bounds);
				result = bounds;
			}
			else
			{
				Assert.Fail();
				BVHNode.ChildRefit(bvh, this.Index, false);
				result = default(Bounds);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RefitVolume(BVH bvh)
		{
			Assert.Check(BehaviourUtils.IsAlive(this._root));
			Bounds bounds = this.ComputeMinVolume(bvh);
			bool flag = !this.Box.ContainBounds(bounds);
			bool result;
			if (flag)
			{
				this.Box = bounds;
				this.Box.Expand(this.Box.size * bvh.ExpansionFactor);
				this.UpdateBoundsCache();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		internal static float SA(Bounds box)
		{
			float num = box.max.x - box.min.x;
			float num2 = box.max.y - box.min.y;
			float num3 = box.max.z - box.min.z;
			return 2f * (num * num2 + num * num3 + num2 * num3);
		}

		internal static float SA(ref Bounds box)
		{
			float num = box.max.x - box.min.x;
			float num2 = box.max.y - box.min.y;
			float num3 = box.max.z - box.min.z;
			return 2f * (num * num2 + num * num3 + num2 * num3);
		}

		internal static float SA(ref BVHNode node)
		{
			float num = node.Box.max.x - node.Box.min.x;
			float num2 = node.Box.max.y - node.Box.min.y;
			float num3 = node.Box.max.z - node.Box.min.z;
			return 2f * (num * num2 + num * num3 + num2 * num3);
		}

		internal static Bounds AABBofPair(ref BVHNode nodea, ref BVHNode nodeb)
		{
			Bounds box = nodea.Box;
			box.Encapsulate(nodeb.Box);
			return box;
		}

		private static Bounds GetEntryBounds(HitboxRoot entry)
		{
			float broadRadius = entry.BroadRadius;
			return new Bounds
			{
				min = new Vector3(-broadRadius, -broadRadius, -broadRadius),
				max = new Vector3(broadRadius, broadRadius, broadRadius)
			};
		}

		private static float SAofList(List<HitboxRoot> entries)
		{
			Assert.Check(entries.Count > 0);
			Bounds entryBounds = BVHNode.GetEntryBounds(entries[0]);
			foreach (HitboxRoot entry in entries)
			{
				Bounds entryBounds2 = BVHNode.GetEntryBounds(entry);
				entryBounds.Encapsulate(entryBounds2);
			}
			return BVHNode.SA(ref entryBounds);
		}

		internal void SplitNode(BVH bvh, List<HitboxRoot> entries)
		{
			Assert.Check(entries != null);
			Assert.Check(entries.Count > 1);
			int num = entries.Count / 2;
			entries.Sort(BVHNode.ComparerX);
			List<HitboxRoot> range = entries.GetRange(0, num);
			List<HitboxRoot> range2 = entries.GetRange(num, entries.Count - num);
			float num2 = BVHNode.SAofList(range) * (float)range.Count + BVHNode.SAofList(range2) * (float)range2.Count;
			float num3 = num2;
			List<HitboxRoot> entries2 = range;
			List<HitboxRoot> entries3 = range2;
			entries.Sort(BVHNode.ComparerY);
			range = entries.GetRange(0, num);
			range2 = entries.GetRange(num, entries.Count - num);
			num2 = BVHNode.SAofList(range) * (float)range.Count + BVHNode.SAofList(range2) * (float)range2.Count;
			bool flag = num2 < num3 || (num2 == num3 && Random.value > 0.67f);
			if (flag)
			{
				num3 = num2;
				entries2 = range;
				entries3 = range2;
			}
			entries.Sort(BVHNode.ComparerZ);
			range = entries.GetRange(0, num);
			range2 = entries.GetRange(num, entries.Count - num);
			num2 = BVHNode.SAofList(range) * (float)range.Count + BVHNode.SAofList(range2) * (float)range2.Count;
			bool flag2 = num2 < num3 || (num2 == num3 && Random.value > 0.67f);
			if (flag2)
			{
				entries2 = range;
				entries3 = range2;
			}
			ref BVHNode nextNode = ref bvh.GetNextNode(out this._leftIndex);
			ref BVHNode nextNode2 = ref bvh.GetNextNode(out this._rightIndex);
			BVHNode.InitNode(ref nextNode, bvh, this._leftIndex, this._nodeIndex, this.Depth + 1, entries2);
			BVHNode.InitNode(ref nextNode2, bvh, this._rightIndex, this._nodeIndex, this.Depth + 1, entries3);
			this._isLeaf = false;
			this.Active = true;
		}

		private static void AddObjectPushdown(BVH bvh, ref BVHNode curNode, HitboxRoot entry)
		{
			ref BVHNode parent = ref curNode.GetParent(bvh);
			bool flag = parent._rightIndex == curNode.Index;
			bvh.ReusableList.Clear();
			bvh.ReusableList.Add(entry);
			int num;
			ref BVHNode nextNode = ref bvh.GetNextNode(out num);
			BVHNode.InitNode(ref nextNode, bvh, num, parent.Index, curNode.Depth, null);
			int index;
			ref BVHNode nextNode2 = ref bvh.GetNextNode(out index);
			BVHNode.InitNode(ref nextNode2, bvh, index, num, nextNode.Depth + 1, bvh.ReusableList);
			bool flag2 = flag;
			if (flag2)
			{
				parent._rightIndex = num;
			}
			else
			{
				parent._leftIndex = num;
			}
			curNode._parentIndex = num;
			curNode.Depth = nextNode.Depth + 1;
			nextNode._leftIndex = curNode.Index;
			nextNode._rightIndex = nextNode2.Index;
			BVHNode.ChildRefit(bvh, num, true);
		}

		internal static void Add(BVH bvh, ref BVHNode startNode, HitboxRoot entry, ref Bounds newObBox, float newObSah)
		{
			Assert.Check(BehaviourUtils.IsAlive(entry));
			bool flag = (!startNode.HasLeft || !startNode.HasRight) && startNode.IsRootNode;
			if (flag)
			{
				Assert.Check(!startNode.HasLeft && !startNode.HasRight);
				bvh.ReusableList.Clear();
				bvh.ReusableList.Add(entry);
				Assert.Check(startNode._isLeaf == BehaviourUtils.IsAlive(startNode._root));
				bool isLeaf = startNode._isLeaf;
				if (isLeaf)
				{
					bvh.ReusableList.Add(startNode._root);
				}
				BVHNode.InitNode(ref startNode, bvh, startNode.Index, 0, 0, bvh.ReusableList);
			}
			else
			{
				ref BVHNode ptr = ref startNode;
				while (!ptr.IsLeaf)
				{
					bool flag2 = !ptr.HasLeft || !ptr.HasRight;
					if (flag2)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(string.Format("Trying to add '{0}' to a {1}({2}) that does not have a Left({3}) and/or Right({4}) children and is not a Leaf. Interrupting operation to avoid an infinite loop.", new object[]
						{
							entry,
							"BVHNode",
							ptr.Index,
							ptr._leftIndex,
							ptr._rightIndex
						}));
						bvh.BuildNodesLog(stringBuilder);
						throw new InvalidOperationException(stringBuilder.ToString());
					}
					ref BVHNode left = ref ptr.GetLeft(bvh);
					ref BVHNode right = ref ptr.GetRight(bvh);
					float num = BVHNode.SA(ref left);
					float num2 = BVHNode.SA(ref right);
					Bounds box = new Bounds
					{
						min = left.Box.min,
						max = left.Box.max
					};
					Bounds box2 = new Bounds
					{
						min = right.Box.min,
						max = right.Box.max
					};
					box.Encapsulate(newObBox);
					box2.Encapsulate(newObBox);
					float num3 = num2 + BVHNode.SA(box);
					float num4 = num + BVHNode.SA(box2);
					float num5 = BVHNode.SA(BVHNode.AABBofPair(ref left, ref right)) + newObSah;
					bool flag3 = num3 < num4;
					if (flag3)
					{
						ptr = ref left;
					}
					else
					{
						ptr = ref right;
					}
				}
				BVHNode.AddObjectPushdown(bvh, ref ptr, entry);
			}
		}

		internal int NodesCount(BVH bvh)
		{
			bool flag = BehaviourUtils.IsAlive(this._root);
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				result = this.GetLeft(bvh).NodesCount(bvh) + this.GetRight(bvh).NodesCount(bvh);
			}
			return result;
		}

		internal void Remove(BVH bvh, HitboxRoot entry)
		{
			bool flag = !this._root;
			if (flag)
			{
				throw new Exception("removeObject() called on nonLeaf!");
			}
			Assert.Check(BehaviourUtils.IsSame(this._root, entry));
			bvh.Mapper.DeRegister(entry);
			bool isRootNode = this.IsRootNode;
			if (isRootNode)
			{
				ref BVHNode node = ref bvh.GetNode(this._nodeIndex);
				BVHNode.InitNode(ref node, bvh, this._nodeIndex, 0, 0, null);
			}
			else
			{
				Assert.Check(this.HasParent);
				this.GetParent(bvh).RemoveLeaf(bvh, this.Index);
			}
		}

		private void SetDepth(BVH bvh, int newdepth)
		{
			this.Depth = newdepth;
			bool flag = newdepth > bvh.maxDepth;
			if (flag)
			{
				bvh.maxDepth = newdepth;
			}
			bool hasLeft = this.HasLeft;
			if (hasLeft)
			{
				this.GetLeft(bvh).SetDepth(bvh, newdepth + 1);
			}
			bool hasRight = this.HasRight;
			if (hasRight)
			{
				this.GetRight(bvh).SetDepth(bvh, newdepth + 1);
			}
		}

		private void RemoveLeaf(BVH bvh, int removeIndex)
		{
			Assert.Check(!this.IsLeaf);
			bool flag = removeIndex != this._leftIndex && removeIndex != this._rightIndex;
			if (flag)
			{
				throw new Exception("removeLeaf doesn't match any leaf!");
			}
			ref BVHNode ptr = ref this;
			ref BVHNode ptr2 = ref this;
			bool flag2 = removeIndex == this._leftIndex;
			if (flag2)
			{
				ptr = this.GetLeft(bvh);
				ptr2 = this.GetRight(bvh);
				this._leftIndex = 0;
			}
			else
			{
				bool flag3 = removeIndex == this._rightIndex;
				if (flag3)
				{
					ptr = this.GetRight(bvh);
					ptr2 = this.GetLeft(bvh);
					this._rightIndex = 0;
				}
			}
			bvh.DisposeNode(removeIndex);
			Assert.Check(ptr.IsLeaf);
			bool isRootNode = this.IsRootNode;
			if (isRootNode)
			{
				this = ptr2;
				this._nodeIndex = 1;
				this._parentIndex = 0;
				this.Depth = 0;
				bool hasLeft = this.HasLeft;
				if (hasLeft)
				{
					ref BVHNode left = ref this.GetLeft(bvh);
					left._parentIndex = this._nodeIndex;
					left.SetDepth(bvh, this.Depth + 1);
				}
				bool hasRight = this.HasRight;
				if (hasRight)
				{
					ref BVHNode right = ref this.GetRight(bvh);
					right._parentIndex = this._nodeIndex;
					right.SetDepth(bvh, this.Depth + 1);
				}
				bool isLeaf = this._isLeaf;
				if (isLeaf)
				{
					bvh.Mapper.DeRegister(this._root);
					bvh.Mapper.RegisterMapping(this._root, this._nodeIndex);
				}
				bvh.DisposeNode(ptr2.Index);
			}
			else
			{
				Assert.Check(this.HasParent);
				ref BVHNode parent = ref this.GetParent(bvh);
				Assert.Check(this.Index == parent._leftIndex || this.Index == parent._rightIndex);
				bool flag4 = this.Index == parent._leftIndex;
				if (flag4)
				{
					parent._leftIndex = ptr2.Index;
				}
				else
				{
					parent._rightIndex = ptr2.Index;
				}
				Assert.Check(ptr2.IsValid);
				ptr2._parentIndex = this._parentIndex;
				ptr2.SetDepth(bvh, ptr2.Depth - 1);
				Assert.Check(ptr2.HasParent);
				ptr2.GetParent(bvh).ChildRefit(bvh, true);
				bvh.DisposeNode(this.Index);
			}
		}

		internal void FindOverlappingLeaves(BVH bvh, Vector3 origin, float radius, List<BVHNode> overlapList)
		{
			bool flag = this.BoundsIntersectsSphere(this.ToBounds(), origin, radius);
			if (flag)
			{
				bool flag2 = BehaviourUtils.IsAlive(this._root);
				if (flag2)
				{
					overlapList.Add(this);
				}
				else
				{
					this.GetLeft(bvh).FindOverlappingLeaves(bvh, origin, radius, overlapList);
					this.GetRight(bvh).FindOverlappingLeaves(bvh, origin, radius, overlapList);
				}
			}
		}

		private bool BoundsIntersectsSphere(Bounds bounds, Vector3 origin, float radius)
		{
			bool flag = origin.x + radius < bounds.min.x || origin.y + radius < bounds.min.y || origin.z + radius < bounds.min.z || origin.x - radius > bounds.max.x || origin.y - radius > bounds.max.y || origin.z - radius > bounds.max.z;
			return !flag;
		}

		internal void FindOverlappingLeaves(BVH bvh, Bounds aabb, List<BVHNode> overlapList)
		{
			bool flag = this.ToBounds().Intersects(aabb);
			if (flag)
			{
				bool flag2 = BehaviourUtils.IsAlive(this._root);
				if (flag2)
				{
					overlapList.Add(this);
				}
				else
				{
					this.GetLeft(bvh).FindOverlappingLeaves(bvh, aabb, overlapList);
					this.GetRight(bvh).FindOverlappingLeaves(bvh, aabb, overlapList);
				}
			}
		}

		internal Bounds ToBounds()
		{
			return new Bounds
			{
				min = new Vector3(this.Box.min.x, this.Box.min.y, this.Box.min.z),
				max = new Vector3(this.Box.max.x, this.Box.max.y, this.Box.max.z)
			};
		}

		internal void ChildExpanded(BVH bvh, ref BVHNode child)
		{
			bool flag = false;
			bool flag2 = child.Box.min.x < this.Box.min.x;
			if (flag2)
			{
				this.Box.min = new Vector3(child.Box.min.x, this.Box.min.y, this.Box.min.z);
				flag = true;
			}
			bool flag3 = child.Box.max.x > this.Box.max.x;
			if (flag3)
			{
				this.Box.max = new Vector3(child.Box.max.x, this.Box.max.y, this.Box.max.z);
				flag = true;
			}
			bool flag4 = child.Box.min.y < this.Box.min.y;
			if (flag4)
			{
				this.Box.min = new Vector3(this.Box.min.x, child.Box.min.y, this.Box.min.z);
				flag = true;
			}
			bool flag5 = child.Box.max.y > this.Box.max.y;
			if (flag5)
			{
				this.Box.max = new Vector3(this.Box.max.x, child.Box.max.y, this.Box.max.z);
				flag = true;
			}
			bool flag6 = child.Box.min.z < this.Box.min.z;
			if (flag6)
			{
				this.Box.min = new Vector3(this.Box.min.x, this.Box.min.y, child.Box.min.z);
				flag = true;
			}
			bool flag7 = child.Box.max.z > this.Box.max.z;
			if (flag7)
			{
				this.Box.max = new Vector3(this.Box.max.x, this.Box.max.y, child.Box.max.z);
				flag = true;
			}
			bool flag8 = flag;
			if (flag8)
			{
				this.UpdateBoundsCache();
			}
			bool flag9 = flag && this.HasParent;
			if (flag9)
			{
				this.GetParent(bvh).ChildExpanded(bvh, ref this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateBoundsCache()
		{
			this._cachedBounds = new AABB(this.Box);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ChildRefit(BVH bvh, bool propagate = true)
		{
			BVHNode.ChildRefit(bvh, this.Index, propagate);
		}

		internal static void ChildRefit(BVH bvh, int nodeIndex, bool propagate = true)
		{
			int num = 0;
			do
			{
				ref BVHNode node = ref bvh.GetNode(nodeIndex);
				ref BVHNode left = ref node.GetLeft(bvh);
				ref BVHNode right = ref node.GetRight(bvh);
				bool flag = !node.Box.ContainBounds(left.Box) || !node.Box.ContainBounds(right.Box);
				if (flag)
				{
					Bounds box = left.Box;
					box.Encapsulate(right.Box);
					node.Box = box;
					bool flag2 = num < bvh.ParentsToExpand;
					if (flag2)
					{
						node.Box.Expand(node.Box.size * bvh.ExpansionFactor);
						num++;
					}
					node.UpdateBoundsCache();
					nodeIndex = node._parentIndex;
				}
				else
				{
					nodeIndex = 0;
				}
			}
			while (propagate && nodeIndex != 0 && !bvh.refitNodes.Contains(nodeIndex));
		}

		internal static void InitNode(ref BVHNode node, BVH bvh, int index, int parentIndex, int curDepth, List<HitboxRoot> entries = null)
		{
			node._nodeIndex = index;
			node._parentIndex = parentIndex;
			node._leftIndex = 0;
			node._rightIndex = 0;
			node._root = null;
			node._isLeaf = false;
			node.Active = true;
			node.Depth = curDepth;
			bool flag = bvh.maxDepth < node.Depth;
			if (flag)
			{
				bvh.maxDepth = node.Depth;
			}
			node.Box = default(Bounds);
			bool flag2 = entries == null || entries.Count < 1;
			if (!flag2)
			{
				bool flag3 = entries.Count <= 1;
				if (flag3)
				{
					Assert.Check(entries.Count == 1);
					node._root = entries[0];
					node.Active = node._root.HitboxRootActive;
					node._isLeaf = true;
					bvh.Mapper.RegisterMapping(node._root, node.Index);
					node.Box = node.ComputeMinVolume(bvh);
					node.Box.Expand(node.Box.size * bvh.ExpansionFactor);
					node.UpdateBoundsCache();
				}
				else
				{
					node.SplitNode(bvh, entries);
					node.ChildRefit(bvh, false);
				}
			}
		}

		public void BuildLog(StringBuilder builder)
		{
			builder.Append(string.Format("Index: {0}", this._nodeIndex));
			builder.Append(string.Format(", Active: {0}", this.Active));
			builder.Append(string.Format(", Used: {0}", this.Used));
			builder.Append(string.Format(", Next: {0}", this.Next));
			builder.Append(string.Format(", Depth: {0}", this.Depth));
			builder.Append(", Root: '" + (BehaviourUtils.IsAlive(this._root) ? this._root.name : "NULL") + "'");
			builder.Append(string.Format(", Parent: {0}", this._parentIndex));
			builder.Append(string.Format(", IsLeaf: {0}", this._isLeaf));
			builder.Append(string.Format(", Left: {0}", this._leftIndex));
			builder.Append(string.Format(", Right {0}", this._rightIndex));
		}

		internal const int MaxEntriesPerNode = 1;

		internal const int RootNodeIndex = 1;

		private static readonly HitboxRoot.HitboxComparerX ComparerX = new HitboxRoot.HitboxComparerX();

		private static readonly HitboxRoot.HitboxComparerY ComparerY = new HitboxRoot.HitboxComparerY();

		private static readonly HitboxRoot.HitboxComparerZ ComparerZ = new HitboxRoot.HitboxComparerZ();

		public Bounds Box;

		internal AABB _cachedBounds;

		private int _nodeIndex;

		private int _parentIndex;

		private int _leftIndex;

		private int _rightIndex;

		internal bool Active;

		internal int Depth;

		internal bool Used;

		internal int Next;

		internal HitboxRoot _root;

		internal bool _isLeaf;

		internal enum Rot
		{
			NONE,
			L_RL,
			L_RR,
			R_LL,
			R_LR,
			LL_RR,
			LL_RL
		}
	}
}
