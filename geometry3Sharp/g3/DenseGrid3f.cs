using System;
using System.Collections.Generic;

namespace g3
{
	public class DenseGrid3f
	{
		public DenseGrid3f()
		{
			this.ni = (this.nj = (this.nk = 0));
		}

		public DenseGrid3f(int ni, int nj, int nk, float initialValue)
		{
			this.resize(ni, nj, nk);
			this.assign(initialValue);
		}

		public DenseGrid3f(DenseGrid3f copy)
		{
			this.Buffer = new float[copy.Buffer.Length];
			Array.Copy(copy.Buffer, this.Buffer, this.Buffer.Length);
			this.ni = copy.ni;
			this.nj = copy.nj;
			this.nk = copy.nk;
		}

		public void swap(DenseGrid3f g2)
		{
			float[] buffer = g2.Buffer;
			g2.Buffer = this.Buffer;
			this.Buffer = buffer;
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
			this.Buffer = new float[ni * nj * nk];
			this.ni = ni;
			this.nj = nj;
			this.nk = nk;
		}

		public void assign(float value)
		{
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				this.Buffer[i] = value;
			}
		}

		public void set_min(ref Vector3i ijk, float f)
		{
			int num = ijk.x + this.ni * (ijk.y + this.nj * ijk.z);
			if (f < this.Buffer[num])
			{
				this.Buffer[num] = f;
			}
		}

		public void set_max(ref Vector3i ijk, float f)
		{
			int num = ijk.x + this.ni * (ijk.y + this.nj * ijk.z);
			if (f > this.Buffer[num])
			{
				this.Buffer[num] = f;
			}
		}

		public float this[int i]
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

		public float this[int i, int j, int k]
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

		public float this[Vector3i ijk]
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

		public void get_x_pair(int i0, int j, int k, out float a, out float b)
		{
			int num = this.ni * (j + this.nj * k);
			a = this.Buffer[num + i0];
			b = this.Buffer[num + i0 + 1];
		}

		public void get_x_pair(int i0, int j, int k, out double a, out double b)
		{
			int num = this.ni * (j + this.nj * k);
			a = (double)this.Buffer[num + i0];
			b = (double)this.Buffer[num + i0 + 1];
		}

		public void apply(Func<float, float> f)
		{
			for (int i = 0; i < this.nk; i++)
			{
				for (int j = 0; j < this.nj; j++)
				{
					for (int k = 0; k < this.ni; k++)
					{
						int num = k + this.ni * (j + this.nj * i);
						this.Buffer[num] = f(this.Buffer[num]);
					}
				}
			}
		}

		public DenseGrid2f get_slice(int slice_i, int dimension)
		{
			DenseGrid2f denseGrid2f;
			if (dimension == 0)
			{
				denseGrid2f = new DenseGrid2f(this.nj, this.nk, 0f);
				for (int i = 0; i < this.nk; i++)
				{
					for (int j = 0; j < this.nj; j++)
					{
						denseGrid2f[j, i] = this.Buffer[slice_i + this.ni * (j + this.nj * i)];
					}
				}
			}
			else if (dimension == 1)
			{
				denseGrid2f = new DenseGrid2f(this.ni, this.nk, 0f);
				for (int k = 0; k < this.nk; k++)
				{
					for (int l = 0; l < this.ni; l++)
					{
						denseGrid2f[l, k] = this.Buffer[l + this.ni * (slice_i + this.nj * k)];
					}
				}
			}
			else
			{
				denseGrid2f = new DenseGrid2f(this.ni, this.nj, 0f);
				for (int m = 0; m < this.nj; m++)
				{
					for (int n = 0; n < this.ni; n++)
					{
						denseGrid2f[n, m] = this.Buffer[n + this.ni * (m + this.nj * slice_i)];
					}
				}
			}
			return denseGrid2f;
		}

		public void set_slice(DenseGrid2f slice, int slice_i, int dimension)
		{
			if (dimension == 0)
			{
				for (int i = 0; i < this.nk; i++)
				{
					for (int j = 0; j < this.nj; j++)
					{
						this.Buffer[slice_i + this.ni * (j + this.nj * i)] = slice[j, i];
					}
				}
				return;
			}
			if (dimension == 1)
			{
				for (int k = 0; k < this.nk; k++)
				{
					for (int l = 0; l < this.ni; l++)
					{
						this.Buffer[l + this.ni * (slice_i + this.nj * k)] = slice[l, k];
					}
				}
				return;
			}
			for (int m = 0; m < this.nj; m++)
			{
				for (int n = 0; n < this.ni; n++)
				{
					this.Buffer[n + this.ni * (m + this.nj * slice_i)] = slice[n, m];
				}
			}
		}

		public AxisAlignedBox3i Bounds
		{
			get
			{
				return new AxisAlignedBox3i(0, 0, 0, this.ni, this.nj, this.nk);
			}
		}

		public AxisAlignedBox3i BoundsInclusive
		{
			get
			{
				return new AxisAlignedBox3i(0, 0, 0, this.ni - 1, this.nj - 1, this.nk - 1);
			}
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

		public Vector3i to_index(int idx)
		{
			int x = idx % this.ni;
			int y = idx / this.ni % this.nj;
			int z = idx / (this.ni * this.nj);
			return new Vector3i(x, y, z);
		}

		public int to_linear(int i, int j, int k)
		{
			return i + this.ni * (j + this.nj * k);
		}

		public int to_linear(ref Vector3i ijk)
		{
			return ijk.x + this.ni * (ijk.y + this.nj * ijk.z);
		}

		public int to_linear(Vector3i ijk)
		{
			return ijk.x + this.ni * (ijk.y + this.nj * ijk.z);
		}

		public float[] Buffer;

		public int ni;

		public int nj;

		public int nk;
	}
}
