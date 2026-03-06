using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace UnityEngine.ProBuilder.KdTree
{
	[Serializable]
	internal class KdTree<TKey, TValue> : IKdTree<TKey, TValue>, IEnumerable<KdTreeNode<TKey, TValue>>, IEnumerable
	{
		public KdTree(int dimensions, ITypeMath<TKey> typeMath)
		{
			this.dimensions = dimensions;
			this.typeMath = typeMath;
			this.Count = 0;
		}

		public KdTree(int dimensions, ITypeMath<TKey> typeMath, AddDuplicateBehavior addDuplicateBehavior) : this(dimensions, typeMath)
		{
			this.AddDuplicateBehavior = addDuplicateBehavior;
		}

		public AddDuplicateBehavior AddDuplicateBehavior { get; private set; }

		public bool Add(TKey[] point, TValue value)
		{
			KdTreeNode<TKey, TValue> value2 = new KdTreeNode<TKey, TValue>(point, value);
			if (this.root == null)
			{
				this.root = new KdTreeNode<TKey, TValue>(point, value);
			}
			else
			{
				int num = -1;
				KdTreeNode<TKey, TValue> kdTreeNode = this.root;
				int compare;
				for (;;)
				{
					num = (num + 1) % this.dimensions;
					if (this.typeMath.AreEqual(point, kdTreeNode.Point))
					{
						switch (this.AddDuplicateBehavior)
						{
						case AddDuplicateBehavior.Skip:
							return false;
						case AddDuplicateBehavior.Error:
							goto IL_6D;
						case AddDuplicateBehavior.Update:
							kdTreeNode.Value = value;
							goto IL_90;
						case AddDuplicateBehavior.Collect:
							goto IL_7C;
						}
						break;
					}
					IL_90:
					compare = this.typeMath.Compare(point[num], kdTreeNode.Point[num]);
					if (kdTreeNode[compare] == null)
					{
						goto Block_4;
					}
					kdTreeNode = kdTreeNode[compare];
				}
				throw new Exception("Unexpected AddDuplicateBehavior");
				IL_6D:
				throw new DuplicateNodeError();
				IL_7C:
				kdTreeNode.AddDuplicate(value);
				return false;
				Block_4:
				kdTreeNode[compare] = value2;
			}
			int count = this.Count;
			this.Count = count + 1;
			return true;
		}

		private void ReadChildNodes(KdTreeNode<TKey, TValue> removedNode)
		{
			if (removedNode.IsLeaf)
			{
				return;
			}
			Queue<KdTreeNode<TKey, TValue>> queue = new Queue<KdTreeNode<TKey, TValue>>();
			Queue<KdTreeNode<TKey, TValue>> queue2 = new Queue<KdTreeNode<TKey, TValue>>();
			if (removedNode.LeftChild != null)
			{
				queue2.Enqueue(removedNode.LeftChild);
			}
			if (removedNode.RightChild != null)
			{
				queue2.Enqueue(removedNode.RightChild);
			}
			while (queue2.Count > 0)
			{
				KdTreeNode<TKey, TValue> kdTreeNode = queue2.Dequeue();
				queue.Enqueue(kdTreeNode);
				for (int i = -1; i <= 1; i += 2)
				{
					if (kdTreeNode[i] != null)
					{
						queue2.Enqueue(kdTreeNode[i]);
						kdTreeNode[i] = null;
					}
				}
			}
			while (queue.Count > 0)
			{
				KdTreeNode<TKey, TValue> kdTreeNode2 = queue.Dequeue();
				int count = this.Count;
				this.Count = count - 1;
				this.Add(kdTreeNode2.Point, kdTreeNode2.Value);
			}
		}

		public void RemoveAt(TKey[] point)
		{
			if (this.root == null)
			{
				return;
			}
			KdTreeNode<TKey, TValue> kdTreeNode;
			if (this.typeMath.AreEqual(point, this.root.Point))
			{
				kdTreeNode = this.root;
				this.root = null;
				int count = this.Count;
				this.Count = count - 1;
				this.ReadChildNodes(kdTreeNode);
				return;
			}
			kdTreeNode = this.root;
			int num = -1;
			for (;;)
			{
				num = (num + 1) % this.dimensions;
				int compare = this.typeMath.Compare(point[num], kdTreeNode.Point[num]);
				if (kdTreeNode[compare] == null)
				{
					break;
				}
				if (this.typeMath.AreEqual(point, kdTreeNode[compare].Point))
				{
					KdTreeNode<TKey, TValue> removedNode = kdTreeNode[compare];
					kdTreeNode[compare] = null;
					int count = this.Count;
					this.Count = count - 1;
					this.ReadChildNodes(removedNode);
				}
				else
				{
					kdTreeNode = kdTreeNode[compare];
				}
				if (kdTreeNode == null)
				{
					return;
				}
			}
		}

		public KdTreeNode<TKey, TValue>[] GetNearestNeighbours(TKey[] point, int count)
		{
			if (count > this.Count)
			{
				count = this.Count;
			}
			if (count < 0)
			{
				throw new ArgumentException("Number of neighbors cannot be negative");
			}
			if (count == 0)
			{
				return new KdTreeNode<TKey, TValue>[0];
			}
			NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighbourList = new NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey>(count, this.typeMath);
			HyperRect<TKey> rect = HyperRect<TKey>.Infinite(this.dimensions, this.typeMath);
			this.AddNearestNeighbours(this.root, point, rect, 0, nearestNeighbourList, this.typeMath.MaxValue);
			count = nearestNeighbourList.Count;
			KdTreeNode<TKey, TValue>[] array = new KdTreeNode<TKey, TValue>[count];
			for (int i = 0; i < count; i++)
			{
				array[count - i - 1] = nearestNeighbourList.RemoveFurtherest();
			}
			return array;
		}

		private void AddNearestNeighbours(KdTreeNode<TKey, TValue> node, TKey[] target, HyperRect<TKey> rect, int depth, NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighbours, TKey maxSearchRadiusSquared)
		{
			if (node == null)
			{
				return;
			}
			int num = depth % this.dimensions;
			HyperRect<TKey> hyperRect = rect.Clone();
			hyperRect.MaxPoint[num] = node.Point[num];
			HyperRect<TKey> hyperRect2 = rect.Clone();
			hyperRect2.MinPoint[num] = node.Point[num];
			int num2 = this.typeMath.Compare(target[num], node.Point[num]);
			HyperRect<TKey> rect2 = (num2 <= 0) ? hyperRect : hyperRect2;
			HyperRect<TKey> rect3 = (num2 <= 0) ? hyperRect2 : hyperRect;
			KdTreeNode<TKey, TValue> kdTreeNode = (num2 <= 0) ? node.LeftChild : node.RightChild;
			KdTreeNode<TKey, TValue> node2 = (num2 <= 0) ? node.RightChild : node.LeftChild;
			if (kdTreeNode != null)
			{
				this.AddNearestNeighbours(kdTreeNode, target, rect2, depth + 1, nearestNeighbours, maxSearchRadiusSquared);
			}
			TKey[] closestPoint = rect3.GetClosestPoint(target, this.typeMath);
			TKey tkey = this.typeMath.DistanceSquaredBetweenPoints(closestPoint, target);
			if (this.typeMath.Compare(tkey, maxSearchRadiusSquared) <= 0)
			{
				if (nearestNeighbours.IsCapacityReached)
				{
					if (this.typeMath.Compare(tkey, nearestNeighbours.GetFurtherestDistance()) < 0)
					{
						this.AddNearestNeighbours(node2, target, rect3, depth + 1, nearestNeighbours, maxSearchRadiusSquared);
					}
				}
				else
				{
					this.AddNearestNeighbours(node2, target, rect3, depth + 1, nearestNeighbours, maxSearchRadiusSquared);
				}
			}
			tkey = this.typeMath.DistanceSquaredBetweenPoints(node.Point, target);
			if (this.typeMath.Compare(tkey, maxSearchRadiusSquared) <= 0)
			{
				nearestNeighbours.Add(node, tkey);
			}
		}

		public KdTreeNode<TKey, TValue>[] RadialSearch(TKey[] center, TKey radius, int count)
		{
			NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey> nearestNeighbourList = new NearestNeighbourList<KdTreeNode<TKey, TValue>, TKey>(count, this.typeMath);
			this.AddNearestNeighbours(this.root, center, HyperRect<TKey>.Infinite(this.dimensions, this.typeMath), 0, nearestNeighbourList, this.typeMath.Multiply(radius, radius));
			count = nearestNeighbourList.Count;
			KdTreeNode<TKey, TValue>[] array = new KdTreeNode<TKey, TValue>[count];
			for (int i = 0; i < count; i++)
			{
				array[count - i - 1] = nearestNeighbourList.RemoveFurtherest();
			}
			return array;
		}

		public int Count { get; private set; }

		public bool TryFindValueAt(TKey[] point, out TValue value)
		{
			KdTreeNode<TKey, TValue> kdTreeNode = this.root;
			int num = -1;
			while (kdTreeNode != null)
			{
				if (this.typeMath.AreEqual(point, kdTreeNode.Point))
				{
					value = kdTreeNode.Value;
					return true;
				}
				num = (num + 1) % this.dimensions;
				int compare = this.typeMath.Compare(point[num], kdTreeNode.Point[num]);
				kdTreeNode = kdTreeNode[compare];
			}
			value = default(TValue);
			return false;
		}

		public TValue FindValueAt(TKey[] point)
		{
			TValue result;
			if (this.TryFindValueAt(point, out result))
			{
				return result;
			}
			return default(TValue);
		}

		public bool TryFindValue(TValue value, out TKey[] point)
		{
			if (this.root == null)
			{
				point = null;
				return false;
			}
			Queue<KdTreeNode<TKey, TValue>> queue = new Queue<KdTreeNode<TKey, TValue>>();
			queue.Enqueue(this.root);
			while (queue.Count > 0)
			{
				KdTreeNode<TKey, TValue> kdTreeNode = queue.Dequeue();
				if (kdTreeNode.Value.Equals(value))
				{
					point = kdTreeNode.Point;
					return true;
				}
				for (int i = -1; i <= 1; i += 2)
				{
					KdTreeNode<TKey, TValue> kdTreeNode2 = kdTreeNode[i];
					if (kdTreeNode2 != null)
					{
						queue.Enqueue(kdTreeNode2);
					}
				}
			}
			point = null;
			return false;
		}

		public TKey[] FindValue(TValue value)
		{
			TKey[] result;
			if (this.TryFindValue(value, out result))
			{
				return result;
			}
			return null;
		}

		private void AddNodeToStringBuilder(KdTreeNode<TKey, TValue> node, StringBuilder sb, int depth)
		{
			sb.AppendLine(node.ToString());
			for (int i = -1; i <= 1; i += 2)
			{
				for (int j = 0; j <= depth; j++)
				{
					sb.Append("\t");
				}
				sb.Append((i == -1) ? "L " : "R ");
				if (node[i] == null)
				{
					sb.AppendLine("");
				}
				else
				{
					this.AddNodeToStringBuilder(node[i], sb, depth + 1);
				}
			}
		}

		public override string ToString()
		{
			if (this.root == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			this.AddNodeToStringBuilder(this.root, stringBuilder, 0);
			return stringBuilder.ToString();
		}

		private void AddNodesToList(KdTreeNode<TKey, TValue> node, List<KdTreeNode<TKey, TValue>> nodes)
		{
			if (node == null)
			{
				return;
			}
			nodes.Add(node);
			for (int i = -1; i <= 1; i += 2)
			{
				if (node[i] != null)
				{
					this.AddNodesToList(node[i], nodes);
					node[i] = null;
				}
			}
		}

		private void SortNodesArray(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
		{
			for (int i = fromIndex + 1; i <= toIndex; i++)
			{
				int num = i;
				for (;;)
				{
					KdTreeNode<TKey, TValue> kdTreeNode = nodes[num - 1];
					KdTreeNode<TKey, TValue> kdTreeNode2 = nodes[num];
					if (this.typeMath.Compare(kdTreeNode2.Point[byDimension], kdTreeNode.Point[byDimension]) >= 0)
					{
						break;
					}
					nodes[num - 1] = kdTreeNode2;
					nodes[num] = kdTreeNode;
				}
			}
		}

		private void AddNodesBalanced(KdTreeNode<TKey, TValue>[] nodes, int byDimension, int fromIndex, int toIndex)
		{
			if (fromIndex == toIndex)
			{
				this.Add(nodes[fromIndex].Point, nodes[fromIndex].Value);
				nodes[fromIndex] = null;
				return;
			}
			this.SortNodesArray(nodes, byDimension, fromIndex, toIndex);
			int num = fromIndex + (int)Math.Round((double)((float)(toIndex + 1 - fromIndex) / 2f)) - 1;
			this.Add(nodes[num].Point, nodes[num].Value);
			nodes[num] = null;
			int byDimension2 = (byDimension + 1) % this.dimensions;
			if (fromIndex < num)
			{
				this.AddNodesBalanced(nodes, byDimension2, fromIndex, num - 1);
			}
			if (toIndex > num)
			{
				this.AddNodesBalanced(nodes, byDimension2, num + 1, toIndex);
			}
		}

		public void Balance()
		{
			List<KdTreeNode<TKey, TValue>> list = new List<KdTreeNode<TKey, TValue>>();
			this.AddNodesToList(this.root, list);
			this.Clear();
			this.AddNodesBalanced(list.ToArray(), 0, 0, list.Count - 1);
		}

		private void RemoveChildNodes(KdTreeNode<TKey, TValue> node)
		{
			for (int i = -1; i <= 1; i += 2)
			{
				if (node[i] != null)
				{
					this.RemoveChildNodes(node[i]);
					node[i] = null;
				}
			}
		}

		public void Clear()
		{
			if (this.root != null)
			{
				this.RemoveChildNodes(this.root);
			}
		}

		public void SaveToFile(string filename)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (FileStream fileStream = File.Create(filename))
			{
				binaryFormatter.Serialize(fileStream, this);
				fileStream.Flush();
			}
		}

		public static KdTree<TKey, TValue> LoadFromFile(string filename)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			KdTree<TKey, TValue> result;
			using (FileStream fileStream = File.Open(filename, FileMode.Open))
			{
				result = (KdTree<TKey, TValue>)binaryFormatter.Deserialize(fileStream);
			}
			return result;
		}

		public IEnumerator<KdTreeNode<TKey, TValue>> GetEnumerator()
		{
			Stack<KdTreeNode<TKey, TValue>> left = new Stack<KdTreeNode<TKey, TValue>>();
			Stack<KdTreeNode<TKey, TValue>> right = new Stack<KdTreeNode<TKey, TValue>>();
			Action<KdTreeNode<TKey, TValue>> addLeft = delegate(KdTreeNode<TKey, TValue> node)
			{
				if (node.LeftChild != null)
				{
					left.Push(node.LeftChild);
				}
			};
			Action<KdTreeNode<TKey, TValue>> addRight = delegate(KdTreeNode<TKey, TValue> node)
			{
				if (node.RightChild != null)
				{
					right.Push(node.RightChild);
				}
			};
			if (this.root != null)
			{
				yield return this.root;
				addLeft(this.root);
				addRight(this.root);
				for (;;)
				{
					if (left.Any<KdTreeNode<TKey, TValue>>())
					{
						KdTreeNode<TKey, TValue> kdTreeNode = left.Pop();
						addLeft(kdTreeNode);
						addRight(kdTreeNode);
						yield return kdTreeNode;
					}
					else
					{
						if (!right.Any<KdTreeNode<TKey, TValue>>())
						{
							break;
						}
						KdTreeNode<TKey, TValue> kdTreeNode2 = right.Pop();
						addLeft(kdTreeNode2);
						addRight(kdTreeNode2);
						yield return kdTreeNode2;
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int dimensions;

		private ITypeMath<TKey> typeMath;

		private KdTreeNode<TKey, TValue> root;
	}
}
