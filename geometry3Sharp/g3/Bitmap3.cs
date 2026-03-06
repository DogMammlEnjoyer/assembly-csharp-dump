using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class Bitmap3 : IBinaryVoxelGrid, IGridElement3, IFixedGrid3
	{
		public Vector3i Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		public Bitmap3(Vector3i dims)
		{
			int length = dims.x * dims.y * dims.z;
			this.Bits = new BitArray(length);
			this.dimensions = dims;
			this.row_size = dims.x;
			this.slab_size = dims.x * dims.y;
		}

		public AxisAlignedBox3i GridBounds
		{
			get
			{
				return new AxisAlignedBox3i(Vector3i.Zero, this.Dimensions);
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

		public bool this[Vector3i idx]
		{
			get
			{
				int index = idx.z * this.slab_size + idx.y * this.row_size + idx.x;
				return this.Bits[index];
			}
			set
			{
				int index = idx.z * this.slab_size + idx.y * this.row_size + idx.x;
				this.Bits[index] = value;
			}
		}

		public void Set(Vector3i idx, bool val)
		{
			int index = idx.z * this.slab_size + idx.y * this.row_size + idx.x;
			this.Bits[index] = val;
		}

		public void SafeSet(Vector3i idx, bool val)
		{
			bool flag = false;
			this.bit_lock.Enter(ref flag);
			int index = idx.z * this.slab_size + idx.y * this.row_size + idx.x;
			this.Bits[index] = val;
			this.bit_lock.Exit();
		}

		public bool Get(Vector3i idx)
		{
			int index = idx.z * this.slab_size + idx.y * this.row_size + idx.x;
			return this.Bits[index];
		}

		public Vector3i ToIndex(int i)
		{
			int num = i / this.slab_size;
			i -= num * this.slab_size;
			int num2 = i / this.row_size;
			i -= num2 * this.row_size;
			return new Vector3i(i, num2, num);
		}

		public int ToLinear(Vector3i idx)
		{
			return idx.z * this.slab_size + idx.y * this.row_size + idx.x;
		}

		public IEnumerable<Vector3i> Indices()
		{
			int num;
			for (int z = 0; z < this.Dimensions.z; z = num)
			{
				for (int y = 0; y < this.Dimensions.y; y = num)
				{
					for (int x = 0; x < this.Dimensions.x; x = num)
					{
						yield return new Vector3i(x, y, z);
						num = x + 1;
					}
					num = y + 1;
				}
				num = z + 1;
			}
			yield break;
		}

		public IEnumerable<Vector3i> NonZeros()
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

		public void Filter(int nMinNbrs)
		{
			AxisAlignedBox3i gridBounds = this.GridBounds;
			gridBounds.Max -= Vector3i.One;
			for (int i = 0; i < this.Bits.Length; i++)
			{
				if (this.Bits[i])
				{
					Vector3i v = this.ToIndex(i);
					int num = 0;
					int num2 = 0;
					while (num2 < 6 && num <= nMinNbrs)
					{
						Vector3i vector3i = v + gIndices.GridOffsets6[num2];
						if (gridBounds.Contains(vector3i) && this.Get(vector3i))
						{
							num++;
						}
						num2++;
					}
					if (num <= nMinNbrs)
					{
						this.Bits[i] = false;
					}
				}
			}
		}

		public virtual IGridElement3 CreateNewGridElement(bool bCopy)
		{
			IGridElement3 result = new Bitmap3(this.Dimensions);
			if (bCopy)
			{
				throw new NotImplementedException();
			}
			return result;
		}

		public BitArray Bits;

		private Vector3i dimensions;

		private int row_size;

		private int slab_size;

		private SpinLock bit_lock;
	}
}
