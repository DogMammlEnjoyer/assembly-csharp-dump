using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class Bitmap2
	{
		public Vector2i Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		public Bitmap2(Vector2i dims)
		{
			this.Resize(dims);
		}

		public Bitmap2(int Width, int Height)
		{
			this.Resize(new Vector2i(Width, Height));
		}

		public void Resize(Vector2i dims)
		{
			int length = dims.x * dims.y;
			this.Bits = new BitArray(length);
			this.dimensions = dims;
			this.row_size = dims.x;
		}

		public AxisAlignedBox2i GridBounds
		{
			get
			{
				return new AxisAlignedBox2i(Vector2i.Zero, this.Dimensions);
			}
		}

		public bool this[int i]
		{
			get
			{
				return this.Bits[i];
			}
			set
			{
				this.Bits[i] = value;
			}
		}

		public bool this[int r, int c]
		{
			get
			{
				return this.Bits[r * this.row_size + c];
			}
			set
			{
				this.Bits[r * this.row_size + c] = value;
			}
		}

		public bool this[Vector2i idx]
		{
			get
			{
				int index = idx.y * this.row_size + idx.x;
				return this.Bits[index];
			}
			set
			{
				int index = idx.y * this.row_size + idx.x;
				this.Bits[index] = value;
			}
		}

		public void Set(Vector2i idx, bool val)
		{
			int index = idx.y * this.row_size + idx.x;
			this.Bits[index] = val;
		}

		public bool Get(Vector2i idx)
		{
			int index = idx.y * this.row_size + idx.x;
			return this.Bits[index];
		}

		public Vector2i ToIndex(int i)
		{
			int num = i / this.row_size;
			i -= num * this.row_size;
			return new Vector2i(i, num);
		}

		public int ToLinear(Vector2i idx)
		{
			return idx.y * this.row_size + idx.x;
		}

		public IEnumerable<Vector2i> Indices()
		{
			int num;
			for (int y = 0; y < this.Dimensions.y; y = num)
			{
				for (int x = 0; x < this.Dimensions.x; x = num)
				{
					yield return new Vector2i(x, y);
					num = x + 1;
				}
				num = y + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2i> NonZeros()
		{
			int num;
			for (int i = 0; i < this.Bits.Count; i = num)
			{
				if (this.Bits[i])
				{
					yield return this.ToIndex(i);
				}
				num = i + 1;
			}
			yield break;
		}

		public BitArray Bits;

		private Vector2i dimensions;

		private int row_size;
	}
}
