using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.Layout
{
	internal class ManagedObjectStore<T>
	{
		public ManagedObjectStore(int chunkSize = 2048)
		{
			this.m_ChunkSize = chunkSize;
			this.m_Chunks = new List<T[]>
			{
				new T[this.m_ChunkSize]
			};
			this.m_Length = 1;
			this.m_Free = new Queue<int>();
		}

		public T GetValue(int index)
		{
			bool flag = index == 0;
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				int index2 = index / this.m_ChunkSize;
				int num = index % this.m_ChunkSize;
				result = this.m_Chunks[index2][num];
			}
			return result;
		}

		public void UpdateValue(ref int index, T value)
		{
			bool flag = index != 0;
			if (flag)
			{
				bool flag2 = value != null;
				if (flag2)
				{
					int index2 = index / this.m_ChunkSize;
					int num = index % this.m_ChunkSize;
					this.m_Chunks[index2][num] = value;
				}
				else
				{
					this.m_Free.Enqueue(index);
					int index3 = index / this.m_ChunkSize;
					int num2 = index % this.m_ChunkSize;
					this.m_Chunks[index3][num2] = default(T);
					index = 0;
				}
			}
			else
			{
				bool flag3 = value != null;
				if (flag3)
				{
					bool flag4 = this.m_Free.Count > 0;
					if (flag4)
					{
						index = this.m_Free.Dequeue();
						int index4 = index / this.m_ChunkSize;
						int num3 = index % this.m_ChunkSize;
						this.m_Chunks[index4][num3] = value;
					}
					else
					{
						int length = this.m_Length;
						this.m_Length = length + 1;
						index = length;
						bool flag5 = index >= this.m_Chunks.Count * this.m_ChunkSize;
						if (flag5)
						{
							this.m_Chunks.Add(new T[this.m_ChunkSize]);
						}
						int index5 = index / this.m_ChunkSize;
						int num4 = index % this.m_ChunkSize;
						this.m_Chunks[index5][num4] = value;
					}
				}
			}
		}

		private const int k_ChunkSize = 2048;

		private readonly int m_ChunkSize;

		private int m_Length;

		private readonly List<T[]> m_Chunks;

		private readonly Queue<int> m_Free;
	}
}
