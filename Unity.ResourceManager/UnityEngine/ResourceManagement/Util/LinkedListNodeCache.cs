using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.Util
{
	public class LinkedListNodeCache<T>
	{
		public LinkedListNodeCache()
		{
			this.InitCache(int.MaxValue, 10, 0);
		}

		public LinkedListNodeCache(int maxNodesAllowed, int initialCapacity, int initialPreallocateCount)
		{
			this.InitCache(maxNodesAllowed, initialCapacity, initialPreallocateCount);
		}

		private void InitCache(int maxNodesAllowed = 2147483647, int initialCapacity = 10, int initialPreallocateCount = 0)
		{
			this.m_maxNodesAllowed = maxNodesAllowed;
			this.m_NodeCache = new Stack<LinkedListNode<T>>(initialCapacity);
			for (int i = 0; i < initialPreallocateCount; i++)
			{
				this.m_NodeCache.Push(new LinkedListNode<T>(default(T)));
				this.m_NodesCreated++;
			}
		}

		public LinkedListNode<T> Acquire(T val)
		{
			LinkedListNode<T> linkedListNode;
			if (this.m_NodeCache.TryPop(out linkedListNode))
			{
				linkedListNode.Value = val;
				return linkedListNode;
			}
			this.m_NodesCreated++;
			return new LinkedListNode<T>(val);
		}

		public void Release(LinkedListNode<T> node)
		{
			if (this.m_NodeCache.Count < this.m_maxNodesAllowed)
			{
				node.Value = default(T);
				this.m_NodeCache.Push(node);
			}
		}

		internal int CreatedNodeCount
		{
			get
			{
				return this.m_NodesCreated;
			}
		}

		internal int CachedNodeCount
		{
			get
			{
				if (this.m_NodeCache != null)
				{
					return this.m_NodeCache.Count;
				}
				return 0;
			}
			set
			{
				while (value < this.m_NodeCache.Count)
				{
					LinkedListNode<T> linkedListNode;
					this.m_NodeCache.TryPop(out linkedListNode);
				}
				while (value > this.m_NodeCache.Count)
				{
					this.m_NodeCache.Push(new LinkedListNode<T>(default(T)));
				}
			}
		}

		private int m_maxNodesAllowed = int.MaxValue;

		private int m_NodesCreated;

		private Stack<LinkedListNode<T>> m_NodeCache;
	}
}
