using System;
using System.Collections.Generic;

namespace g3
{
	public class LockingQueue<T>
	{
		public LockingQueue()
		{
			this.queue = new Queue<T>();
			this.queue_lock = new object();
		}

		public bool Remove(ref T val)
		{
			object obj = this.queue_lock;
			bool result;
			lock (obj)
			{
				if (this.queue.Count > 0)
				{
					val = this.queue.Dequeue();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void Add(T obj)
		{
			object obj2 = this.queue_lock;
			lock (obj2)
			{
				this.queue.Enqueue(obj);
			}
		}

		public int Count
		{
			get
			{
				object obj = this.queue_lock;
				int count;
				lock (obj)
				{
					count = this.queue.Count;
				}
				return count;
			}
		}

		private Queue<T> queue;

		private object queue_lock;
	}
}
