using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class DenseGrid2i
	{
		public DenseGrid2i()
		{
			this.ni = (this.nj = 0);
		}

		public DenseGrid2i(int ni, int nj, int initialValue)
		{
			this.resize(ni, nj);
			this.assign(initialValue);
		}

		public DenseGrid2i(DenseGrid2i copy)
		{
			this.resize(copy.ni, copy.nj);
			Array.Copy(copy.Buffer, this.Buffer, this.Buffer.Length);
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
			this.Buffer = new int[ni * nj];
			this.ni = ni;
			this.nj = nj;
		}

		public void clear()
		{
			Array.Clear(this.Buffer, 0, this.Buffer.Length);
		}

		public void copy(DenseGrid2i copy)
		{
			Array.Copy(copy.Buffer, this.Buffer, this.Buffer.Length);
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

		public int this[int i, int j]
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

		public int this[Vector2i ijk]
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

		public void increment(int i, int j)
		{
			this.Buffer[i + this.ni * j]++;
		}

		public void decrement(int i, int j)
		{
			this.Buffer[i + this.ni * j]--;
		}

		public void atomic_increment(int i, int j)
		{
			Interlocked.Increment(ref this.Buffer[i + this.ni * j]);
		}

		public void atomic_decrement(int i, int j)
		{
			Interlocked.Decrement(ref this.Buffer[i + this.ni * j]);
		}

		public void atomic_incdec(int i, int j, bool decrement = false)
		{
			if (decrement)
			{
				Interlocked.Decrement(ref this.Buffer[i + this.ni * j]);
				return;
			}
			Interlocked.Increment(ref this.Buffer[i + this.ni * j]);
		}

		public int sum()
		{
			int num = 0;
			for (int i = 0; i < this.Buffer.Length; i++)
			{
				num += this.Buffer[i];
			}
			return num;
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

		public int[] Buffer;

		public int ni;

		public int nj;
	}
}
