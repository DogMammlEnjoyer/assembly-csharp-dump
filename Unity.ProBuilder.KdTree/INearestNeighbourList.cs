using System;

namespace UnityEngine.ProBuilder.KdTree
{
	internal interface INearestNeighbourList<TItem, TDistance>
	{
		bool Add(TItem item, TDistance distance);

		TItem GetFurtherest();

		TItem RemoveFurtherest();

		int MaxCapacity { get; }

		int Count { get; }
	}
}
