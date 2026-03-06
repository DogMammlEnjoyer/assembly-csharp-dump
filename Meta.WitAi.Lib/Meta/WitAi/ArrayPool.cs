using System;

namespace Meta.WitAi
{
	public class ArrayPool<TElementType> : ObjectPool<TElementType[]>
	{
		public int Capacity { get; }

		public ArrayPool(int capacity, int preload = 0) : base(() => new TElementType[capacity], preload)
		{
			this.Capacity = capacity;
		}
	}
}
