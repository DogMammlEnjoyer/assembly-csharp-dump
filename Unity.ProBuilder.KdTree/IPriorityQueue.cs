using System;

namespace UnityEngine.ProBuilder.KdTree
{
	internal interface IPriorityQueue<TItem, TPriority>
	{
		void Enqueue(TItem item, TPriority priority);

		TItem Dequeue();

		int Count { get; }
	}
}
