using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class DenseGrid3i
	{
		public DenseGrid3i()
		{
			this.ni = (this.nj = (this.nk = 0));
		}

		public DenseGrid3i(int ni, int nj, int nk, int initialValue)
		{
			this.resize(ni, nj, nk);
			this.assign(initialValue);
		}

		public int size
		{
			get
			{
				return this.ni * this.nj * this.nk;
			}
		}

		public void resize(int ni, int nj, int nk)
		{
			this.Buffer = new int[ni * nj * nk];
			this.ni = ni;
			this.nj = nj;
			this.nk = nk;
		}

		public void assign(int value)
		{
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				this.Buffer[i] = value;
			}
		}

		public int this[int i]
		{
			get
			{
				return this.Buffer[i];
			}
			set
			{
				this.Buffer[i] = value;
			}
		}

		public int this[int i, int j, int k]
		{
			get
			{
				return this.Buffer[i + this.ni * (j + this.nj * k)];
			}
			set
			{
				this.Buffer[i + this.ni * (j + this.nj * k)] = value;
			}
		}

		public int this[Vector3i ijk]
		{
			get
			{
				return this.Buffer[ijk.x + this.ni * (ijk.y + this.nj * ijk.z)];
			}
			set
			{
				this.Buffer[ijk.x + this.ni * (ijk.y + this.nj * ijk.z)] = value;
			}
		}

		public void increment(int i, int j, int k)
		{
			this.Buffer[i + this.ni * (j + this.nj * k)]++;
		}

		public void decrement(int i, int j, int k)
		{
			this.Buffer[i + this.ni * (j + this.nj * k)]--;
		}

		public void atomic_increment(int i, int j, int k)
		{
			Interlocked.Increment(ref this.Buffer[i + this.ni * (j + this.nj * k)]);
		}

		public void atomic_decrement(int i, int j, int k)
		{
			Interlocked.Decrement(ref this.Buffer[i + this.ni * (j + this.nj * k)]);
		}

		public void atomic_incdec(int i, int j, int k, bool decrement = false)
		{
			if (decrement)
			{
				Interlocked.Decrement(ref this.Buffer[i + this.ni * (j + this.nj * k)]);
				return;
			}
			Interlocked.Increment(ref this.Buffer[i + this.ni * (j + this.nj * k)]);
		}

		public DenseGrid2i get_slice(int slice_i, int dimension)
		{
			DenseGrid2i denseGrid2i;
			if (dimension == 0)
			{
				denseGrid2i = new DenseGrid2i(this.nj, this.nk, 0);
				for (int i = 0; i < this.nk; i++)
				{
					for (int j = 0; j < this.nj; j++)
					{
						denseGrid2i[j, i] = this.Buffer[slice_i + this.ni * (j + this.nj * i)];
					}
				}
			}
			else if (dimension == 1)
			{
				denseGrid2i = new DenseGrid2i(this.ni, this.nk, 0);
				for (int k = 0; k < this.nk; k++)
				{
					for (int l = 0; l < this.ni; l++)
					{
						denseGrid2i[l, k] = this.Buffer[l + this.ni * (slice_i + this.nj * k)];
					}
				}
			}
			else
			{
				denseGrid2i = new DenseGrid2i(this.ni, this.nj, 0);
				for (int m = 0; m < this.nj; m++)
				{
					for (int n = 0; n < this.ni; n++)
					{
						denseGrid2i[n, m] = this.Buffer[n + this.ni * (m + this.nj * slice_i)];
					}
				}
			}
			return denseGrid2i;
		}

		public Bitmap3 get_bitmap(int thresh = 0)
		{
			Bitmap3 bitmap = new Bitmap3(new Vector3i(this.ni, this.nj, this.nk));
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				bitmap[i] = (this.Buffer[i] > thresh);
			}
			return bitmap;
		}

		public IEnumerable<Vector3i> Indices()
		{
			int num;
			for (int z = 0; z < this.nk; z = num)
			{
				for (int y = 0; y < this.nj; y = num)
				{
					for (int x = 0; x < this.ni; x = num)
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

		public IEnumerable<Vector3i> InsetIndices(int border_width)
		{
			int stopy = this.nj - border_width;
			int stopx = this.ni - border_width;
			int num;
			for (int z = border_width; z < this.nk - border_width; z = num)
			{
				for (int y = border_width; y < stopy; y = num)
				{
					for (int x = border_width; x < stopx; x = num)
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

		public int[] Buffer;

		public int ni;

		public int nj;

		public int nk;
	}
}
