using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace g3
{
	public class DVector<T> : IEnumerable<T>, IEnumerable
	{
		public DVector()
		{
			this.iCurBlock = 0;
			this.iCurBlockUsed = 0;
			this.Blocks = new List<T[]>();
			this.Blocks.Add(new T[this.nBlockSize]);
		}

		public DVector(DVector<T> copy)
		{
			this.nBlockSize = copy.nBlockSize;
			this.iCurBlock = copy.iCurBlock;
			this.iCurBlockUsed = copy.iCurBlockUsed;
			this.Blocks = new List<T[]>();
			for (int i = 0; i < copy.Blocks.Count; i++)
			{
				this.Blocks.Add(new T[this.nBlockSize]);
				Array.Copy(copy.Blocks[i], this.Blocks[i], copy.Blocks[i].Length);
			}
		}

		public DVector(T[] data)
		{
			this.Initialize(data);
		}

		public DVector(IEnumerable<T> init)
		{
			this.iCurBlock = 0;
			this.iCurBlockUsed = 0;
			this.Blocks = new List<T[]>();
			this.Blocks.Add(new T[this.nBlockSize]);
			foreach (T value in init)
			{
				this.Add(value);
			}
		}

		public int Length
		{
			get
			{
				return this.iCurBlock * this.nBlockSize + this.iCurBlockUsed;
			}
		}

		public int BlockCount
		{
			get
			{
				return this.nBlockSize;
			}
		}

		public int size
		{
			get
			{
				return this.Length;
			}
		}

		public bool empty
		{
			get
			{
				return this.iCurBlock == 0 && this.iCurBlockUsed == 0;
			}
		}

		public int MemoryUsageBytes
		{
			get
			{
				if (this.Blocks.Count != 0)
				{
					return this.Blocks.Count * this.nBlockSize * Marshal.SizeOf<T>(this.Blocks[0][0]);
				}
				return 0;
			}
		}

		public void Add(T value)
		{
			if (this.iCurBlockUsed == this.nBlockSize)
			{
				if (this.iCurBlock == this.Blocks.Count - 1)
				{
					this.Blocks.Add(new T[this.nBlockSize]);
				}
				this.iCurBlock++;
				this.iCurBlockUsed = 0;
			}
			this.Blocks[this.iCurBlock][this.iCurBlockUsed] = value;
			this.iCurBlockUsed++;
		}

		public void Add(T value, int nRepeat)
		{
			for (int i = 0; i < nRepeat; i++)
			{
				this.Add(value);
			}
		}

		public void Add(T[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				this.Add(values[i]);
			}
		}

		public void Add(T[] values, int nRepeat)
		{
			for (int i = 0; i < nRepeat; i++)
			{
				for (int j = 0; j < values.Length; j++)
				{
					this.Add(values[j]);
				}
			}
		}

		public void push_back(T value)
		{
			this.Add(value);
		}

		public void pop_back()
		{
			if (this.iCurBlockUsed > 0)
			{
				this.iCurBlockUsed--;
			}
			if (this.iCurBlockUsed == 0 && this.iCurBlock > 0)
			{
				this.iCurBlock--;
				this.iCurBlockUsed = this.nBlockSize;
			}
		}

		public void insert(T value, int index)
		{
			this.insertAt(value, index);
		}

		public void insertAt(T value, int index)
		{
			int size = this.size;
			if (index == size)
			{
				this.push_back(value);
				return;
			}
			if (index > size)
			{
				this.resize(index);
				this.push_back(value);
				return;
			}
			this[index] = value;
		}

		public void resize(int count)
		{
			if (this.Length == count)
			{
				return;
			}
			int num = 1 + count / this.nBlockSize;
			int count2 = this.Blocks.Count;
			for (int i = num; i < count2; i++)
			{
				this.Blocks[i] = null;
			}
			if (num >= this.Blocks.Count)
			{
				this.Blocks.Capacity = num;
			}
			else
			{
				this.Blocks.RemoveRange(num, this.Blocks.Count - num);
			}
			for (int j = count2; j < num; j++)
			{
				this.Blocks.Add(new T[this.nBlockSize]);
			}
			this.iCurBlockUsed = count - (num - 1) * this.nBlockSize;
			this.iCurBlock = num - 1;
		}

		public void copy(DVector<T> copyIn)
		{
			if (this.Blocks != null && copyIn.Blocks.Count == this.Blocks.Count)
			{
				int count = copyIn.Blocks.Count;
				for (int i = 0; i < count; i++)
				{
					Array.Copy(copyIn.Blocks[i], this.Blocks[i], copyIn.Blocks[i].Length);
				}
				this.iCurBlock = copyIn.iCurBlock;
				this.iCurBlockUsed = copyIn.iCurBlockUsed;
				return;
			}
			this.resize(copyIn.size);
			int count2 = copyIn.Blocks.Count;
			for (int j = 0; j < count2; j++)
			{
				Array.Copy(copyIn.Blocks[j], this.Blocks[j], copyIn.Blocks[j].Length);
			}
			this.iCurBlock = copyIn.iCurBlock;
			this.iCurBlockUsed = copyIn.iCurBlockUsed;
		}

		public T this[int i]
		{
			get
			{
				return this.Blocks[i >> 11][i & 2047];
			}
			set
			{
				this.Blocks[i >> 11][i & 2047] = value;
			}
		}

		public T back
		{
			get
			{
				return this.Blocks[this.iCurBlock][this.iCurBlockUsed - 1];
			}
			set
			{
				this.Blocks[this.iCurBlock][this.iCurBlockUsed - 1] = value;
			}
		}

		public T front
		{
			get
			{
				return this.Blocks[0][0];
			}
			set
			{
				this.Blocks[0][0] = value;
			}
		}

		public void GetBuffer(T[] data)
		{
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				data[i] = this[i];
			}
		}

		public T[] GetBuffer()
		{
			T[] array = new T[this.Length];
			for (int i = 0; i < this.Length; i++)
			{
				array[i] = this[i];
			}
			return array;
		}

		public T[] ToArray()
		{
			return this.GetBuffer();
		}

		public T2[] GetBufferCast<T2>()
		{
			T2[] array = new T2[this.Length];
			for (int i = 0; i < this.Length; i++)
			{
				array[i] = (T2)((object)Convert.ChangeType(this[i], typeof(T2)));
			}
			return array;
		}

		public byte[] GetBytes()
		{
			int num = Marshal.SizeOf(typeof(T));
			byte[] array = new byte[this.Length * num];
			int num2 = 0;
			int count = this.Blocks.Count;
			for (int i = 0; i < count - 1; i++)
			{
				Buffer.BlockCopy(this.Blocks[i], 0, array, num2, this.nBlockSize * num);
				num2 += this.nBlockSize * num;
			}
			Buffer.BlockCopy(this.Blocks[count - 1], 0, array, num2, this.iCurBlockUsed * num);
			return array;
		}

		public void Initialize(T[] data)
		{
			int num = data.Length / this.nBlockSize;
			this.Blocks = new List<T[]>();
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				T[] array = new T[this.nBlockSize];
				Array.Copy(data, num2, array, 0, this.nBlockSize);
				this.Blocks.Add(array);
				num2 += this.nBlockSize;
			}
			this.iCurBlockUsed = data.Length - num2;
			if (this.iCurBlockUsed != 0)
			{
				T[] array2 = new T[this.nBlockSize];
				Array.Copy(data, num2, array2, 0, this.iCurBlockUsed);
				this.Blocks.Add(array2);
			}
			else
			{
				this.iCurBlockUsed = this.nBlockSize;
			}
			this.iCurBlock = this.Blocks.Count - 1;
		}

		public void Clear()
		{
			foreach (T[] array in this.Blocks)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		public void Apply(Action<T, int> applyF)
		{
			for (int i = 0; i < this.iCurBlock; i++)
			{
				T[] array = this.Blocks[i];
				for (int j = 0; j < this.nBlockSize; j++)
				{
					applyF(array[j], j);
				}
			}
			T[] array2 = this.Blocks[this.iCurBlock];
			for (int k = 0; k < this.iCurBlockUsed; k++)
			{
				applyF(array2[k], k);
			}
		}

		public void ApplyReplace(Func<T, int, T> applyF)
		{
			for (int i = 0; i < this.iCurBlock; i++)
			{
				T[] array = this.Blocks[i];
				for (int j = 0; j < this.nBlockSize; j++)
				{
					array[j] = applyF(array[j], j);
				}
			}
			T[] array2 = this.Blocks[this.iCurBlock];
			for (int k = 0; k < this.iCurBlockUsed; k++)
			{
				array2[k] = applyF(array2[k], k);
			}
		}

		public unsafe static void FastGetBuffer(DVector<double> v, double* pBuffer)
		{
			IntPtr destination = new IntPtr((void*)pBuffer);
			int count = v.Blocks.Count;
			for (int i = 0; i < count - 1; i++)
			{
				Marshal.Copy(v.Blocks[i], 0, destination, v.nBlockSize);
				destination = new IntPtr(destination.ToInt64() + (long)(v.nBlockSize * 8));
			}
			Marshal.Copy(v.Blocks[count - 1], 0, destination, v.iCurBlockUsed);
		}

		public unsafe static void FastGetBuffer(DVector<float> v, float* pBuffer)
		{
			IntPtr destination = new IntPtr((void*)pBuffer);
			int count = v.Blocks.Count;
			for (int i = 0; i < count - 1; i++)
			{
				Marshal.Copy(v.Blocks[i], 0, destination, v.nBlockSize);
				destination = new IntPtr(destination.ToInt64() + (long)(v.nBlockSize * 4));
			}
			Marshal.Copy(v.Blocks[count - 1], 0, destination, v.iCurBlockUsed);
		}

		public unsafe static void FastGetBuffer(DVector<int> v, int* pBuffer)
		{
			IntPtr destination = new IntPtr((void*)pBuffer);
			int count = v.Blocks.Count;
			for (int i = 0; i < count - 1; i++)
			{
				Marshal.Copy(v.Blocks[i], 0, destination, v.nBlockSize);
				destination = new IntPtr(destination.ToInt64() + (long)(v.nBlockSize * 4));
			}
			Marshal.Copy(v.Blocks[count - 1], 0, destination, v.iCurBlockUsed);
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int bi = 0; bi < this.iCurBlock; bi = num)
			{
				T[] block = this.Blocks[bi];
				for (int i = 0; i < this.nBlockSize; i = num)
				{
					yield return block[i];
					num = i + 1;
				}
				block = null;
				num = bi + 1;
			}
			T[] lastblock = this.Blocks[this.iCurBlock];
			for (int bi = 0; bi < this.iCurBlockUsed; bi = num)
			{
				yield return lastblock[bi];
				num = bi + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerable<DVector<T>.DBlock> BlockIterator()
		{
			int num;
			for (int i = 0; i < this.iCurBlock; i = num)
			{
				yield return new DVector<T>.DBlock
				{
					data = this.Blocks[i],
					usedCount = this.nBlockSize
				};
				num = i + 1;
			}
			yield return new DVector<T>.DBlock
			{
				data = this.Blocks[this.iCurBlock],
				usedCount = this.iCurBlockUsed
			};
			yield break;
		}

		private List<T[]> Blocks;

		private int iCurBlock;

		private int iCurBlockUsed;

		private int nBlockSize = 2048;

		private const int nShiftBits = 11;

		private const int nBlockIndexBitmask = 2047;

		public struct DBlock
		{
			public T[] data;

			public int usedCount;
		}
	}
}
