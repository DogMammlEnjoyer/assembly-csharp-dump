using System;

namespace UnityEngine.ProBuilder.KdTree
{
	internal class NearestNeighbourList<TItem, TDistance> : INearestNeighbourList<TItem, TDistance>
	{
		public NearestNeighbourList(int maxCapacity, ITypeMath<TDistance> distanceMath)
		{
			this.maxCapacity = maxCapacity;
			this.distanceMath = distanceMath;
			this.queue = new PriorityQueue<TItem, TDistance>(maxCapacity, distanceMath);
		}

		public int MaxCapacity
		{
			get
			{
				return this.maxCapacity;
			}
		}

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public bool Add(TItem item, TDistance distance)
		{
			if (this.queue.Count < this.maxCapacity)
			{
				this.queue.Enqueue(item, distance);
				return true;
			}
			if (this.distanceMath.Compare(distance, this.queue.GetHighestPriority()) < 0)
			{
				this.queue.Dequeue();
				this.queue.Enqueue(item, distance);
				return true;
			}
			return false;
		}

		public TItem GetFurtherest()
		{
			if (this.Count == 0)
			{
				throw new Exception("List is empty");
			}
			return this.queue.GetHighest();
		}

		public TDistance GetFurtherestDistance()
		{
			if (this.Count == 0)
			{
				throw new Exception("List is empty");
			}
			return this.queue.GetHighestPriority();
		}

		public TItem RemoveFurtherest()
		{
			return this.queue.Dequeue();
		}

		public bool IsCapacityReached
		{
			get
			{
				return this.Count == this.MaxCapacity;
			}
		}

		private PriorityQueue<TItem, TDistance> queue;

		private ITypeMath<TDistance> distanceMath;

		private int maxCapacity;
	}
}
