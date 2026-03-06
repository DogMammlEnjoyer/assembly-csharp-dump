using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	public class FastAction<A, B, C>
	{
		public void Add(Action<A, B, C> rhs)
		{
			bool flag = this.lookup.ContainsKey(rhs);
			if (!flag)
			{
				this.lookup[rhs] = this.delegates.AddLast(rhs);
			}
		}

		public void Remove(Action<A, B, C> rhs)
		{
			LinkedListNode<Action<A, B, C>> node;
			bool flag = this.lookup.TryGetValue(rhs, out node);
			if (flag)
			{
				this.lookup.Remove(rhs);
				this.delegates.Remove(node);
			}
		}

		public void Call(A a, B b, C c)
		{
			for (LinkedListNode<Action<A, B, C>> linkedListNode = this.delegates.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				linkedListNode.Value(a, b, c);
			}
		}

		private LinkedList<Action<A, B, C>> delegates = new LinkedList<Action<A, B, C>>();

		private Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>> lookup = new Dictionary<Action<A, B, C>, LinkedListNode<Action<A, B, C>>>();
	}
}
