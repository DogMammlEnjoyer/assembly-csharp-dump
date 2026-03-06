using System;
using System.Collections.Generic;

namespace g3
{
	public class DenseGrid2f
	{
		public DenseGrid2f()
		{
			this.ni = (this.nj = 0);
		}

		public DenseGrid2f(int ni, int nj, float initialValue)
		{
			this.resize(ni, nj);
			this.assign(initialValue);
		}

		public DenseGrid2f(DenseGrid2f copy)
		{
			this.Buffer = new float[copy.Buffer.Length];
			Array.Copy(copy.Buffer, this.Buffer, this.Buffer.Length);
			this.ni = copy.ni;
			this.nj = copy.nj;
		}

		public void swap(DenseGrid2f g2)
		{
			float[] buffer = g2.Buffer;
			g2.Buffer = this.Buffer;
			this.Buffer = buffer;
		}

		public int size
		{
			get
			{
				return this.ni * this.nj;
			}
		}

		public void resize(int ni, int nj)
		{
			this.Buffer = new float[ni * nj];
			this.ni = ni;
			this.nj = nj;
		}

		public void assign(float value)
		{
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				this.Buffer[i] = value;
			}
		}

		public void assign_border(float value, int rings)
		{
			for (int i = 0; i < rings; i++)
			{
				int num = this.nj - 1 - i;
				for (int j = 0; j < this.ni; j++)
				{
					this.Buffer[j + this.ni * i] = value;
					this.Buffer[j + this.ni * num] = value;
				}
			}
			int num2 = this.nj - 1 - rings;
			for (int k = rings; k < num2; k++)
			{
				for (int l = 0; l < rings; l++)
				{
					this.Buffer[l + this.ni * k] = value;
					this.Buffer[this.ni - 1 - l + this.ni * k] = value;
				}
			}
		}

		public void clear()
		{
			Array.Clear(this.Buffer, 0, this.Buffer.Length);
		}

		public void copy(DenseGrid2f copy)
		{
			Array.Copy(copy.Buffer, this.Buffer, this.Buffer.Length);
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

		public float this[int i, int j]
		{
			get
			{
				return this.Buffer[i + this.ni * j];
			}
			set
			{
				this.Buffer[i + this.ni * j] = value;
			}
		}

		public float this[Vector2i ijk]
		{
			get
			{
				return this.Buffer[ijk.x + this.ni * ijk.y];
			}
			set
			{
				this.Buffer[ijk.x + this.ni * ijk.y] = value;
			}
		}

		public void get_x_pair(int i0, int j, out double a, out double b)
		{
			int num = this.ni * j;
			a = (double)this.Buffer[num + i0];
			b = (double)this.Buffer[num + i0 + 1];
		}

		public void apply(Func<float, float> f)
		{
			for (int i = 0; i < this.nj; i++)
			{
				for (int j = 0; j < this.ni; j++)
				{
					int num = j + this.ni * i;
					this.Buffer[num] = f(this.Buffer[num]);
				}
			}
		}

		public void set_min(DenseGrid2f grid2)
		{
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				this.Buffer[i] = Math.Min(this.Buffer[i], grid2.Buffer[i]);
			}
		}

		public void set_max(DenseGrid2f grid2)
		{
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				this.Buffer[i] = Math.Max(this.Buffer[i], grid2.Buffer[i]);
			}
		}

		public AxisAlignedBox2i Bounds
		{
			get
			{
				return new AxisAlignedBox2i(0, 0, this.ni, this.nj);
			}
		}

		public IEnumerable<Vector2i> Indices()
		{
			int num;
			for (int y = 0; y < this.nj; y = num)
			{
				for (int x = 0; x < this.ni; x = num)
				{
					yield return new Vector2i(x, y);
					num = x + 1;
				}
				num = y + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2i> InsetIndices(int border_width)
		{
			int stopy = this.nj - border_width;
			int stopx = this.ni - border_width;
			int num;
			for (int y = border_width; y < stopy; y = num)
			{
				for (int x = border_width; x < stopx; x = num)
				{
					yield return new Vector2i(x, y);
					num = x + 1;
				}
				num = y + 1;
			}
			yield break;
		}

		public float[] Buffer;

		public int ni;

		public int nj;
	}
}
