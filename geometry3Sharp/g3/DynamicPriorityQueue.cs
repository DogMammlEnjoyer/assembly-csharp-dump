using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class DynamicPriorityQueue<T> : IEnumerable<T>, IEnumerable where T : DynamicPriorityQueueNode
	{
		public DynamicPriorityQueue()
		{
			this.num_nodes = 0;
			this.nodes = new DVector<T>();
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
				this.nodes = new DVector<T>();
			}
			this.num_nodes = 0;
		}

		public T First
		{
			get
			{
				return this.nodes[1];
			}
		}

		public float FirstPriority
		{
			get
			{
				return this.nodes[1].priority;
			}
		}

		public bool Contains(T node)
		{
			return this.nodes[node.index] == node;
		}

		public void Enqueue(T node, float priority)
		{
			if (this.EnableDebugChecks && this.Contains(node))
			{
				throw new Exception("DynamicPriorityQueue.Enqueue: tried to add node that is already in queue!");
			}
			node.priority = priority;
			this.num_nodes++;
			this.nodes.insert(node, this.num_nodes);
			node.index = this.num_nodes;
			this.move_up(this.nodes[this.num_nodes]);
		}

		public T Dequeue()
		{
			if (this.EnableDebugChecks && this.Count == 0)
			{
				throw new Exception("DynamicPriorityQueue.Dequeue: queue is empty!");
			}
			T t = this.nodes[1];
			this.Remove(t);
			return t;
		}

		public void Remove(T node)
		{
			if (this.EnableDebugChecks && !this.Contains(node))
			{
				throw new Exception("DynamicPriorityQueue.Remove: tried to remove node that does not exist in queue!");
			}
			if (node.index == this.num_nodes)
			{
				this.nodes[this.num_nodes] = default(T);
				this.num_nodes--;
				return;
			}
			T t = this.nodes[this.num_nodes];
			this.swap_nodes(node, t);
			this.nodes[this.num_nodes] = default(T);
			this.num_nodes--;
			this.on_node_updated(t);
		}

		public void Update(T node, float priority)
		{
			if (this.EnableDebugChecks && !this.Contains(node))
			{
				throw new Exception("DynamicPriorityQueue.Update: tried to update node that does not exist in queue!");
			}
			node.priority = priority;
			this.on_node_updated(node);
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 1; i <= this.num_nodes; i = num + 1)
			{
				yield return this.nodes[i];
				num = i;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void swap_nodes(T node1, T node2)
		{
			this.nodes[node1.index] = node2;
			this.nodes[node2.index] = node1;
			int index = node1.index;
			node1.index = node2.index;
			node2.index = index;
		}

		private void move_up(T node)
		{
			for (int i = node.index / 2; i >= 1; i = node.index / 2)
			{
				T t = this.nodes[i];
				if (this.has_higher_priority(t, node))
				{
					break;
				}
				this.swap_nodes(node, t);
			}
		}

		private void move_down(T node)
		{
			int num = node.index;
			for (;;)
			{
				T t = node;
				int num2 = 2 * num;
				if (num2 > this.num_nodes)
				{
					break;
				}
				T t2 = this.nodes[num2];
				if (this.has_higher_priority(t2, t))
				{
					t = t2;
				}
				int num3 = num2 + 1;
				if (num3 <= this.num_nodes)
				{
					T t3 = this.nodes[num3];
					if (this.has_higher_priority(t3, t))
					{
						t = t3;
					}
				}
				if (t == node)
				{
					goto IL_B2;
				}
				this.nodes[num] = t;
				int index = t.index;
				t.index = num;
				num = index;
			}
			node.index = num;
			this.nodes[num] = node;
			return;
			IL_B2:
			node.index = num;
			this.nodes[num] = node;
		}

		private void on_node_updated(T node)
		{
			int num = node.index / 2;
			T lower = this.nodes[num];
			if (num > 0 && this.has_higher_priority(node, lower))
			{
				this.move_up(node);
				return;
			}
			this.move_down(node);
		}

		private bool has_higher_priority(T higher, T lower)
		{
			return higher.priority < lower.priority;
		}

		public bool IsValidQueue()
		{
			for (int i = 1; i < this.num_nodes; i++)
			{
				if (this.nodes[i] != null)
				{
					int num = 2 * i;
					if (num < this.num_nodes && this.nodes[num] != null && this.has_higher_priority(this.nodes[num], this.nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < this.num_nodes && this.nodes[num2] != null && this.has_higher_priority(this.nodes[num2], this.nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void DebugPrint()
		{
			for (int i = 1; i <= this.num_nodes; i++)
			{
				Console.WriteLine("{0} : p {1}  idx {2}", i, this.nodes[i].priority, this.nodes[i].index);
			}
		}

		public bool EnableDebugChecks;

		private DVector<T> nodes;

		private int num_nodes;
	}
}
