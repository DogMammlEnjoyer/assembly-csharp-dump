using System;
using System.Collections.Generic;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	public class PointKDTree
	{
		public PointKDTree()
		{
			this.tree[1] = new PointKDTree.Node
			{
				data = this.GetOrCreateList()
			};
		}

		public void Add(GraphNode node)
		{
			this.numNodes++;
			this.Add(node, 1, 0);
		}

		public void Rebuild(GraphNode[] nodes, int start, int end)
		{
			if (start < 0 || end < start || end > nodes.Length)
			{
				throw new ArgumentException();
			}
			for (int i = 0; i < this.tree.Length; i++)
			{
				GraphNode[] data = this.tree[i].data;
				if (data != null)
				{
					for (int j = 0; j < 21; j++)
					{
						data[j] = null;
					}
					this.arrayCache.Push(data);
					this.tree[i].data = null;
				}
			}
			this.numNodes = end - start;
			this.Build(1, new List<GraphNode>(nodes), start, end);
		}

		private GraphNode[] GetOrCreateList()
		{
			if (this.arrayCache.Count <= 0)
			{
				return new GraphNode[21];
			}
			return this.arrayCache.Pop();
		}

		private int Size(int index)
		{
			if (this.tree[index].data == null)
			{
				return this.Size(2 * index) + this.Size(2 * index + 1);
			}
			return (int)this.tree[index].count;
		}

		private void CollectAndClear(int index, List<GraphNode> buffer)
		{
			GraphNode[] data = this.tree[index].data;
			ushort count = this.tree[index].count;
			if (data != null)
			{
				this.tree[index] = default(PointKDTree.Node);
				for (int i = 0; i < (int)count; i++)
				{
					buffer.Add(data[i]);
					data[i] = null;
				}
				this.arrayCache.Push(data);
				return;
			}
			this.CollectAndClear(index * 2, buffer);
			this.CollectAndClear(index * 2 + 1, buffer);
		}

		private static int MaxAllowedSize(int numNodes, int depth)
		{
			return Math.Min(5 * numNodes / 2 >> depth, 3 * numNodes / 4);
		}

		private void Rebalance(int index)
		{
			this.CollectAndClear(index, this.largeList);
			this.Build(index, this.largeList, 0, this.largeList.Count);
			this.largeList.ClearFast<GraphNode>();
		}

		private void EnsureSize(int index)
		{
			if (index >= this.tree.Length)
			{
				PointKDTree.Node[] array = new PointKDTree.Node[Math.Max(index + 1, this.tree.Length * 2)];
				this.tree.CopyTo(array, 0);
				this.tree = array;
			}
		}

		private void Build(int index, List<GraphNode> nodes, int start, int end)
		{
			this.EnsureSize(index);
			if (end - start <= 10)
			{
				GraphNode[] array = this.tree[index].data = this.GetOrCreateList();
				this.tree[index].count = (ushort)(end - start);
				for (int i = start; i < end; i++)
				{
					array[i - start] = nodes[i];
				}
				return;
			}
			Int3 position;
			Int3 @int = position = nodes[start].position;
			for (int j = start; j < end; j++)
			{
				Int3 position2 = nodes[j].position;
				position = new Int3(Math.Min(position.x, position2.x), Math.Min(position.y, position2.y), Math.Min(position.z, position2.z));
				@int = new Int3(Math.Max(@int.x, position2.x), Math.Max(@int.y, position2.y), Math.Max(@int.z, position2.z));
			}
			Int3 int2 = @int - position;
			int num = (int2.x > int2.y) ? ((int2.x > int2.z) ? 0 : 2) : ((int2.y > int2.z) ? 1 : 2);
			nodes.Sort(start, end - start, PointKDTree.comparers[num]);
			int num2 = (start + end) / 2;
			this.tree[index].split = (nodes[num2 - 1].position[num] + nodes[num2].position[num] + 1) / 2;
			this.tree[index].splitAxis = (byte)num;
			this.Build(index * 2, nodes, start, num2);
			this.Build(index * 2 + 1, nodes, num2, end);
		}

		private void Add(GraphNode point, int index, int depth = 0)
		{
			while (this.tree[index].data == null)
			{
				index = 2 * index + ((point.position[(int)this.tree[index].splitAxis] < this.tree[index].split) ? 0 : 1);
				depth++;
			}
			GraphNode[] data = this.tree[index].data;
			PointKDTree.Node[] array = this.tree;
			int num = index;
			ushort count = array[num].count;
			array[num].count = count + 1;
			data[(int)count] = point;
			if (this.tree[index].count >= 21)
			{
				int num2 = 0;
				while (depth - num2 > 0 && this.Size(index >> num2) > PointKDTree.MaxAllowedSize(this.numNodes, depth - num2))
				{
					num2++;
				}
				this.Rebalance(index >> num2);
			}
		}

		public GraphNode GetNearest(Int3 point, NNConstraint constraint)
		{
			GraphNode result = null;
			long maxValue = long.MaxValue;
			this.GetNearestInternal(1, point, constraint, ref result, ref maxValue);
			return result;
		}

		private void GetNearestInternal(int index, Int3 point, NNConstraint constraint, ref GraphNode best, ref long bestSqrDist)
		{
			GraphNode[] data = this.tree[index].data;
			if (data != null)
			{
				for (int i = (int)(this.tree[index].count - 1); i >= 0; i--)
				{
					long sqrMagnitudeLong = (data[i].position - point).sqrMagnitudeLong;
					if (sqrMagnitudeLong < bestSqrDist && (constraint == null || constraint.Suitable(data[i])))
					{
						bestSqrDist = sqrMagnitudeLong;
						best = data[i];
					}
				}
				return;
			}
			long num = (long)(point[(int)this.tree[index].splitAxis] - this.tree[index].split);
			int num2 = 2 * index + ((num < 0L) ? 0 : 1);
			this.GetNearestInternal(num2, point, constraint, ref best, ref bestSqrDist);
			if (num * num < bestSqrDist)
			{
				this.GetNearestInternal(num2 ^ 1, point, constraint, ref best, ref bestSqrDist);
			}
		}

		public GraphNode GetNearestConnection(Int3 point, NNConstraint constraint, long maximumSqrConnectionLength)
		{
			GraphNode result = null;
			long maxValue = long.MaxValue;
			long distanceThresholdOffset = (maximumSqrConnectionLength + 3L) / 4L;
			this.GetNearestConnectionInternal(1, point, constraint, ref result, ref maxValue, distanceThresholdOffset);
			return result;
		}

		private void GetNearestConnectionInternal(int index, Int3 point, NNConstraint constraint, ref GraphNode best, ref long bestSqrDist, long distanceThresholdOffset)
		{
			GraphNode[] data = this.tree[index].data;
			if (data != null)
			{
				Vector3 p = (Vector3)point;
				for (int i = (int)(this.tree[index].count - 1); i >= 0; i--)
				{
					long sqrMagnitudeLong = (data[i].position - point).sqrMagnitudeLong;
					if (sqrMagnitudeLong - distanceThresholdOffset < bestSqrDist && (constraint == null || constraint.Suitable(data[i])))
					{
						Connection[] connections = (data[i] as PointNode).connections;
						if (connections != null)
						{
							Vector3 vector = (Vector3)data[i].position;
							for (int j = 0; j < connections.Length; j++)
							{
								Vector3 b = ((Vector3)connections[j].node.position + vector) * 0.5f;
								long num = (long)(VectorMath.SqrDistancePointSegment(vector, b, p) * 1000f * 1000f);
								if (num < bestSqrDist)
								{
									bestSqrDist = num;
									best = data[i];
								}
							}
						}
						if (sqrMagnitudeLong < bestSqrDist)
						{
							bestSqrDist = sqrMagnitudeLong;
							best = data[i];
						}
					}
				}
				return;
			}
			long num2 = (long)(point[(int)this.tree[index].splitAxis] - this.tree[index].split);
			int num3 = 2 * index + ((num2 < 0L) ? 0 : 1);
			this.GetNearestConnectionInternal(num3, point, constraint, ref best, ref bestSqrDist, distanceThresholdOffset);
			if (num2 * num2 - distanceThresholdOffset < bestSqrDist)
			{
				this.GetNearestConnectionInternal(num3 ^ 1, point, constraint, ref best, ref bestSqrDist, distanceThresholdOffset);
			}
		}

		public void GetInRange(Int3 point, long sqrRadius, List<GraphNode> buffer)
		{
			this.GetInRangeInternal(1, point, sqrRadius, buffer);
		}

		private void GetInRangeInternal(int index, Int3 point, long sqrRadius, List<GraphNode> buffer)
		{
			GraphNode[] data = this.tree[index].data;
			if (data != null)
			{
				for (int i = (int)(this.tree[index].count - 1); i >= 0; i--)
				{
					if ((data[i].position - point).sqrMagnitudeLong < sqrRadius)
					{
						buffer.Add(data[i]);
					}
				}
				return;
			}
			long num = (long)(point[(int)this.tree[index].splitAxis] - this.tree[index].split);
			int num2 = 2 * index + ((num < 0L) ? 0 : 1);
			this.GetInRangeInternal(num2, point, sqrRadius, buffer);
			if (num * num < sqrRadius)
			{
				this.GetInRangeInternal(num2 ^ 1, point, sqrRadius, buffer);
			}
		}

		public const int LeafSize = 10;

		public const int LeafArraySize = 21;

		private PointKDTree.Node[] tree = new PointKDTree.Node[16];

		private int numNodes;

		private readonly List<GraphNode> largeList = new List<GraphNode>();

		private readonly Stack<GraphNode[]> arrayCache = new Stack<GraphNode[]>();

		private static readonly IComparer<GraphNode>[] comparers = new IComparer<GraphNode>[]
		{
			new PointKDTree.CompareX(),
			new PointKDTree.CompareY(),
			new PointKDTree.CompareZ()
		};

		private struct Node
		{
			public GraphNode[] data;

			public int split;

			public ushort count;

			public byte splitAxis;
		}

		private class CompareX : IComparer<GraphNode>
		{
			public int Compare(GraphNode lhs, GraphNode rhs)
			{
				return lhs.position.x.CompareTo(rhs.position.x);
			}
		}

		private class CompareY : IComparer<GraphNode>
		{
			public int Compare(GraphNode lhs, GraphNode rhs)
			{
				return lhs.position.y.CompareTo(rhs.position.y);
			}
		}

		private class CompareZ : IComparer<GraphNode>
		{
			public int Compare(GraphNode lhs, GraphNode rhs)
			{
				return lhs.position.z.CompareTo(rhs.position.z);
			}
		}
	}
}
