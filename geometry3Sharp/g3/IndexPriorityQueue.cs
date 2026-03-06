using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class IndexPriorityQueue : IEnumerable<int>, IEnumerable
	{
		public IndexPriorityQueue(int maxID)
		{
			this.nodes = new DVector<IndexPriorityQueue.QueueNode>();
			this.id_to_index = new int[maxID];
			for (int i = 0; i < maxID; i++)
			{
				this.id_to_index[i] = -1;
			}
			this.num_nodes = 0;
		}

		public int Count
		{
			get
			{
				return this.num_nodes;
			}
		}

		public void Clear(bool bFreeMemory = true)
		{
			if (bFreeMemory)
			{
				this.nodes = new DVector<IndexPriorityQueue.QueueNode>();
			}
			Array.Clear(this.id_to_index, 0, this.id_to_index.Length);
			this.num_nodes = 0;
		}

		public int First
		{
			get
			{
				return this.nodes[1].id;
			}
		}

		public float FirstPriority
		{
			get
			{
				return this.nodes[1].priority;
			}
		}

		public bool Contains(int id)
		{
			int num = this.id_to_index[id];
			return num > 0 && num <= this.num_nodes && this.nodes[num].index > 0;
		}

		public void Insert(int id, float priority)
		{
			if (this.EnableDebugChecks && this.Contains(id))
			{
				throw new Exception("IndexPriorityQueue.Insert: tried to add node that is already in queue!");
			}
			IndexPriorityQueue.QueueNode queueNode = default(IndexPriorityQueue.QueueNode);
			queueNode.id = id;
			queueNode.priority = priority;
			this.num_nodes++;
			queueNode.index = this.num_nodes;
			this.id_to_index[id] = queueNode.index;
			this.nodes.insert(queueNode, this.num_nodes);
			this.move_up(this.nodes[this.num_nodes].index);
		}

		public void Enqueue(int id, float priority)
		{
			this.Insert(id, priority);
		}

		public int Dequeue()
		{
			if (this.EnableDebugChecks && this.Count == 0)
			{
				throw new Exception("IndexPriorityQueue.Dequeue: queue is empty!");
			}
			int id = this.nodes[1].id;
			this.remove_at_index(1);
			return id;
		}

		public void Remove(int id)
		{
			if (this.EnableDebugChecks && !this.Contains(id))
			{
				throw new Exception("IndexPriorityQueue.Remove: tried to remove node that does not exist in queue!");
			}
			int iNode = this.id_to_index[id];
			this.remove_at_index(iNode);
		}

		public void Update(int id, float priority)
		{
			if (this.EnableDebugChecks && !this.Contains(id))
			{
				throw new Exception("IndexPriorityQueue.Update: tried to update node that does not exist in queue!");
			}
			int num = this.id_to_index[id];
			IndexPriorityQueue.QueueNode value = this.nodes[num];
			value.priority = priority;
			this.nodes[num] = value;
			this.on_node_updated(num);
		}

		public float GetPriority(int id)
		{
			if (this.EnableDebugChecks && !this.Contains(id))
			{
				throw new Exception("IndexPriorityQueue.Update: tried to get priorty of node that does not exist in queue!");
			}
			int i = this.id_to_index[id];
			return this.nodes[i].priority;
		}

		public IEnumerator<int> GetEnumerator()
		{
			int num;
			for (int i = 1; i <= this.num_nodes; i = num + 1)
			{
				yield return this.nodes[i].id;
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void remove_at_index(int iNode)
		{
			if (iNode == this.num_nodes)
			{
				this.nodes[iNode] = default(IndexPriorityQueue.QueueNode);
				this.num_nodes--;
				return;
			}
			this.swap_nodes_at_indices(iNode, this.num_nodes);
			this.nodes[this.num_nodes] = default(IndexPriorityQueue.QueueNode);
			this.num_nodes--;
			this.on_node_updated(iNode);
		}

		private void swap_nodes_at_indices(int i1, int i2)
		{
			IndexPriorityQueue.QueueNode queueNode = this.nodes[i1];
			queueNode.index = i2;
			IndexPriorityQueue.QueueNode queueNode2 = this.nodes[i2];
			queueNode2.index = i1;
			this.nodes[i1] = queueNode2;
			this.nodes[i2] = queueNode;
			this.id_to_index[queueNode2.id] = i1;
			this.id_to_index[queueNode.id] = i2;
		}

		private void move(int iFrom, int iTo)
		{
			IndexPriorityQueue.QueueNode queueNode = this.nodes[iFrom];
			queueNode.index = iTo;
			this.nodes[iTo] = queueNode;
			this.id_to_index[queueNode.id] = iTo;
		}

		private void set(int iTo, ref IndexPriorityQueue.QueueNode n)
		{
			n.index = iTo;
			this.nodes[iTo] = n;
			this.id_to_index[n.id] = iTo;
		}

		private void move_up(int iNode)
		{
			int num = iNode;
			IndexPriorityQueue.QueueNode queueNode = this.nodes[num];
			int num2 = iNode / 2;
			while (num2 >= 1 && this.nodes[num2].priority >= queueNode.priority)
			{
				this.move(num2, iNode);
				iNode = num2;
				num2 = this.nodes[iNode].index / 2;
			}
			if (iNode != num)
			{
				this.set(iNode, ref queueNode);
			}
		}

		private void move_down(int iNode)
		{
			int num = iNode;
			IndexPriorityQueue.QueueNode queueNode = this.nodes[num];
			for (;;)
			{
				int num2 = iNode;
				int num3 = 2 * iNode;
				if (num3 > this.num_nodes)
				{
					break;
				}
				float num4 = queueNode.priority;
				float priority = this.nodes[num3].priority;
				if (priority < num4)
				{
					num2 = num3;
					num4 = priority;
				}
				int num5 = num3 + 1;
				if (num5 <= this.num_nodes && this.nodes[num5].priority < num4)
				{
					num2 = num5;
				}
				if (num2 == iNode)
				{
					break;
				}
				this.move(num2, iNode);
				iNode = num2;
			}
			if (iNode != num)
			{
				this.set(iNode, ref queueNode);
			}
		}

		private void on_node_updated(int iNode)
		{
			int num = iNode / 2;
			if (num > 0 && this.has_higher_priority(iNode, num))
			{
				this.move_up(iNode);
				return;
			}
			this.move_down(iNode);
		}

		private bool has_higher_priority(int iHigher, int iLower)
		{
			return this.nodes[iHigher].priority < this.nodes[iLower].priority;
		}

		public bool IsValidQueue()
		{
			for (int i = 1; i < this.num_nodes; i++)
			{
				int num = 2 * i;
				if (num < this.num_nodes && this.has_higher_priority(num, i))
				{
					return false;
				}
				int num2 = num + 1;
				if (num2 < this.num_nodes && this.has_higher_priority(num2, i))
				{
					return false;
				}
			}
			return true;
		}

		public void DebugPrint()
		{
			for (int i = 1; i <= this.num_nodes; i++)
			{
				Console.WriteLine("{0} : p {1}  index {2}  id {3}", new object[]
				{
					i,
					this.nodes[i].priority,
					this.nodes[i].index,
					this.nodes[i].id
				});
			}
		}

		public bool EnableDebugChecks;

		private DVector<IndexPriorityQueue.QueueNode> nodes;

		private int num_nodes;

		private int[] id_to_index;

		private struct QueueNode
		{
			public int id;

			public float priority;

			public int index;
		}
	}
}
