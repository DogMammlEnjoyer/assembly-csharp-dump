using System;

namespace UnityEngine.ProBuilder.KdTree
{
	internal class PriorityQueue<TItem, TPriority> : IPriorityQueue<TItem, TPriority>
	{
		public PriorityQueue(int capacity, ITypeMath<TPriority> priorityMath)
		{
			if (capacity <= 0)
			{
				throw new ArgumentException("Capacity must be greater than zero");
			}
			this.capacity = capacity;
			this.queue = new ItemPriority<TItem, TPriority>[capacity];
			this.priorityMath = priorityMath;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private void ExpandCapacity()
		{
			this.capacity *= 2;
			ItemPriority<TItem, TPriority>[] destinationArray = new ItemPriority<TItem, TPriority>[this.capacity];
			Array.Copy(this.queue, destinationArray, this.queue.Length);
			this.queue = destinationArray;
		}

		public void Enqueue(TItem item, TPriority priority)
		{
			int num = this.count + 1;
			this.count = num;
			if (num > this.capacity)
			{
				this.ExpandCapacity();
			}
			int num2 = this.count - 1;
			this.queue[num2] = new ItemPriority<TItem, TPriority>
			{
				Item = item,
				Priority = priority
			};
			this.ReorderItem(num2, -1);
		}

		public TItem Dequeue()
		{
			TItem item = this.queue[0].Item;
			this.queue[0].Item = default(TItem);
			this.queue[0].Priority = this.priorityMath.MinValue;
			this.ReorderItem(0, 1);
			this.count--;
			return item;
		}

		private void ReorderItem(int index, int direction)
		{
			if (direction != -1 && direction != 1)
			{
				throw new ArgumentException("Invalid Direction");
			}
			ItemPriority<TItem, TPriority> itemPriority = this.queue[index];
			int num = index + direction;
			while (num >= 0 && num < this.count)
			{
				ItemPriority<TItem, TPriority> itemPriority2 = this.queue[num];
				int num2 = this.priorityMath.Compare(itemPriority.Priority, itemPriority2.Priority);
				if ((direction != -1 || num2 <= 0) && (direction != 1 || num2 >= 0))
				{
					break;
				}
				this.queue[index] = itemPriority2;
				this.queue[num] = itemPriority;
				index += direction;
				num += direction;
			}
		}

		public TItem GetHighest()
		{
			if (this.count == 0)
			{
				throw new Exception("Queue is empty");
			}
			return this.queue[0].Item;
		}

		public TPriority GetHighestPriority()
		{
			if (this.count == 0)
			{
				throw new Exception("Queue is empty");
			}
			return this.queue[0].Priority;
		}

		private ITypeMath<TPriority> priorityMath;

		private ItemPriority<TItem, TPriority>[] queue;

		private int capacity;

		private int count;
	}
}
