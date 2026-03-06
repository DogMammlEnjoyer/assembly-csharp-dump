using System;

namespace Pathfinding
{
	public class BinaryHeap
	{
		public bool isEmpty
		{
			get
			{
				return this.numberOfItems <= 0;
			}
		}

		private static int RoundUpToNextMultipleMod1(int v)
		{
			return v + (4 - (v - 1) % 4) % 4;
		}

		public BinaryHeap(int capacity)
		{
			capacity = BinaryHeap.RoundUpToNextMultipleMod1(capacity);
			this.heap = new BinaryHeap.Tuple[capacity];
			this.numberOfItems = 0;
		}

		public void Clear()
		{
			for (int i = 0; i < this.numberOfItems; i++)
			{
				this.heap[i].node.heapIndex = ushort.MaxValue;
			}
			this.numberOfItems = 0;
		}

		internal PathNode GetNode(int i)
		{
			return this.heap[i].node;
		}

		internal void SetF(int i, uint f)
		{
			this.heap[i].F = f;
		}

		private void Expand()
		{
			int num = BinaryHeap.RoundUpToNextMultipleMod1(Math.Max(this.heap.Length + 4, Math.Min(65533, (int)Math.Round((double)((float)this.heap.Length * this.growthFactor)))));
			if (num > 65534)
			{
				throw new Exception("Binary Heap Size really large (>65534). A heap size this large is probably the cause of pathfinding running in an infinite loop. ");
			}
			BinaryHeap.Tuple[] array = new BinaryHeap.Tuple[num];
			this.heap.CopyTo(array, 0);
			this.heap = array;
		}

		public void Add(PathNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.heapIndex != 65535)
			{
				this.DecreaseKey(this.heap[(int)node.heapIndex], node.heapIndex);
				return;
			}
			if (this.numberOfItems == this.heap.Length)
			{
				this.Expand();
			}
			this.DecreaseKey(new BinaryHeap.Tuple(0U, node), (ushort)this.numberOfItems);
			this.numberOfItems++;
		}

		private void DecreaseKey(BinaryHeap.Tuple node, ushort index)
		{
			int num = (int)index;
			uint num2 = node.F = node.node.F;
			uint g = node.node.G;
			while (num != 0)
			{
				int num3 = (num - 1) / 4;
				if (num2 >= this.heap[num3].F && (num2 != this.heap[num3].F || g <= this.heap[num3].node.G))
				{
					break;
				}
				this.heap[num] = this.heap[num3];
				this.heap[num].node.heapIndex = (ushort)num;
				num = num3;
			}
			this.heap[num] = node;
			node.node.heapIndex = (ushort)num;
		}

		public PathNode Remove()
		{
			PathNode node = this.heap[0].node;
			node.heapIndex = ushort.MaxValue;
			this.numberOfItems--;
			if (this.numberOfItems == 0)
			{
				return node;
			}
			BinaryHeap.Tuple tuple = this.heap[this.numberOfItems];
			uint g = tuple.node.G;
			int num = 0;
			for (;;)
			{
				int num2 = num;
				uint num3 = tuple.F;
				int num4 = num2 * 4 + 1;
				if (num4 <= this.numberOfItems)
				{
					uint f = this.heap[num4].F;
					uint f2 = this.heap[num4 + 1].F;
					uint f3 = this.heap[num4 + 2].F;
					uint f4 = this.heap[num4 + 3].F;
					if (num4 < this.numberOfItems && (f < num3 || (f == num3 && this.heap[num4].node.G < g)))
					{
						num3 = f;
						num = num4;
					}
					if (num4 + 1 < this.numberOfItems && (f2 < num3 || (f2 == num3 && this.heap[num4 + 1].node.G < ((num == num2) ? g : this.heap[num].node.G))))
					{
						num3 = f2;
						num = num4 + 1;
					}
					if (num4 + 2 < this.numberOfItems && (f3 < num3 || (f3 == num3 && this.heap[num4 + 2].node.G < ((num == num2) ? g : this.heap[num].node.G))))
					{
						num3 = f3;
						num = num4 + 2;
					}
					if (num4 + 3 < this.numberOfItems && (f4 < num3 || (f4 == num3 && this.heap[num4 + 3].node.G < ((num == num2) ? g : this.heap[num].node.G))))
					{
						num = num4 + 3;
					}
				}
				if (num2 == num)
				{
					break;
				}
				this.heap[num2] = this.heap[num];
				this.heap[num2].node.heapIndex = (ushort)num2;
			}
			this.heap[num] = tuple;
			tuple.node.heapIndex = (ushort)num;
			return node;
		}

		private void Validate()
		{
			for (int i = 1; i < this.numberOfItems; i++)
			{
				int num = (i - 1) / 4;
				if (this.heap[num].F > this.heap[i].F)
				{
					throw new Exception(string.Concat(new string[]
					{
						"Invalid state at ",
						i.ToString(),
						":",
						num.ToString(),
						" ( ",
						this.heap[num].F.ToString(),
						" > ",
						this.heap[i].F.ToString(),
						" ) "
					}));
				}
				if ((int)this.heap[i].node.heapIndex != i)
				{
					throw new Exception("Invalid heap index");
				}
			}
		}

		public void Rebuild()
		{
			for (int i = 2; i < this.numberOfItems; i++)
			{
				int num = i;
				BinaryHeap.Tuple tuple = this.heap[i];
				uint f = tuple.F;
				while (num != 1)
				{
					int num2 = num / 4;
					if (f >= this.heap[num2].F)
					{
						break;
					}
					this.heap[num] = this.heap[num2];
					this.heap[num].node.heapIndex = (ushort)num;
					this.heap[num2] = tuple;
					this.heap[num2].node.heapIndex = (ushort)num2;
					num = num2;
				}
			}
		}

		public int numberOfItems;

		public float growthFactor = 2f;

		private const int D = 4;

		private const bool SortGScores = true;

		public const ushort NotInHeap = 65535;

		private BinaryHeap.Tuple[] heap;

		private struct Tuple
		{
			public Tuple(uint f, PathNode node)
			{
				this.F = f;
				this.node = node;
			}

			public PathNode node;

			public uint F;
		}
	}
}
