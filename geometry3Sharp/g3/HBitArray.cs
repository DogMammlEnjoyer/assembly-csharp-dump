using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class HBitArray : IEnumerable<int>, IEnumerable
	{
		public HBitArray(int maxIndex)
		{
			this.max_index = maxIndex;
			int num = maxIndex / 32;
			if (maxIndex % 32 != 0)
			{
				num++;
			}
			this.bits = new HBitArray.MyBitVector32[num];
			this.count = 0;
			this.layerCount = 2;
			this.layers = new HBitArray.Layer[this.layerCount];
			int num2 = this.bits.Length;
			for (int i = 0; i < this.layerCount; i++)
			{
				int num3 = num2 / 32;
				if (num2 % 32 != 0)
				{
					num3++;
				}
				this.layers[i].layer_bits = new HBitArray.MyBitVector32[num3];
				num2 = num3;
			}
		}

		public bool this[int i]
		{
			get
			{
				return this.Get(i);
			}
			set
			{
				this.Set(i, value);
			}
		}

		public int Count
		{
			get
			{
				return this.max_index;
			}
		}

		public int TrueCount
		{
			get
			{
				return this.count;
			}
		}

		public bool Contains(int i)
		{
			return this.Get(i);
		}

		public void Add(int i)
		{
			this.Set(i, true);
		}

		public void Set(int i, bool value)
		{
			int num = i / 32;
			int i2 = i - 32 * num;
			if (value)
			{
				if (!this.bits[num][i2])
				{
					this.bits[num][i2] = true;
					this.count++;
					for (int j = 0; j < this.layerCount; j++)
					{
						int num2 = num / 32;
						int i3 = num - 32 * num2;
						this.layers[j].layer_bits[num2][i3] = true;
						num = num2;
					}
					return;
				}
			}
			else if (this.bits[num][i2])
			{
				this.bits[num][i2] = false;
				this.count--;
				for (int k = 0; k < this.layerCount; k++)
				{
					int num3 = num / 32;
					int i4 = num - 32 * num3;
					this.layers[k].layer_bits[num3][i4] = false;
					num = num3;
				}
			}
		}

		public bool Get(int i)
		{
			int num = i / 32;
			int i2 = i - 32 * num;
			return this.bits[num][i2];
		}

		public IEnumerator<int> GetEnumerator()
		{
			if (this.count > this.max_index / 3)
			{
				int num;
				for (int bi = 0; bi < this.bits.Length; bi = num)
				{
					int d = this.bits[bi].Data;
					int dmask = 1;
					int maxj = (bi == this.bits.Length - 1) ? (this.max_index % 32) : 32;
					for (int i = 0; i < maxj; i = num)
					{
						if ((d & dmask) != 0)
						{
							yield return bi * 32 + i;
						}
						dmask <<= 1;
						num = i + 1;
					}
					num = bi + 1;
				}
			}
			else
			{
				int num;
				for (int bi = 0; bi < this.layers[1].layer_bits.Length; bi = num)
				{
					if (this.layers[1].layer_bits[bi].Data != 0)
					{
						for (int maxj = 0; maxj < 32; maxj = num + 1)
						{
							if (this.layers[1].layer_bits[bi][maxj])
							{
								int dmask = bi * 32 + maxj;
								for (int d = 0; d < 32; d = num + 1)
								{
									if (this.layers[0].layer_bits[dmask][d])
									{
										int i = dmask * 32 + d;
										int d2 = this.bits[i].Data;
										int dmask2 = 1;
										for (int j = 0; j < 32; j = num)
										{
											if ((d2 & dmask2) != 0)
											{
												yield return i * 32 + j;
											}
											dmask2 <<= 1;
											num = j + 1;
										}
									}
									num = d;
								}
							}
							num = maxj;
						}
					}
					num = bi + 1;
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private HBitArray.MyBitVector32[] bits;

		private HBitArray.Layer[] layers;

		private int layerCount;

		private int max_index;

		private int count;

		private struct MyBitVector32
		{
			public bool this[int i]
			{
				get
				{
					return (this.bits & 1 << i) != 0;
				}
				set
				{
					if (value)
					{
						this.bits |= 1 << i;
						return;
					}
					this.bits &= ~(1 << i);
				}
			}

			public int Data
			{
				get
				{
					return this.bits;
				}
			}

			private int bits;
		}

		private struct Layer
		{
			public HBitArray.MyBitVector32[] layer_bits;
		}
	}
}
