using System;
using System.Collections;

namespace g3
{
	public class RefCountVector : IEnumerable
	{
		public RefCountVector()
		{
			this.ref_counts = new DVector<short>();
			this.free_indices = new DVector<int>();
			this.used_count = 0;
		}

		public RefCountVector(RefCountVector copy)
		{
			this.ref_counts = new DVector<short>(copy.ref_counts);
			this.free_indices = new DVector<int>(copy.free_indices);
			this.used_count = copy.used_count;
		}

		public RefCountVector(short[] raw_ref_counts, bool build_free_list = false)
		{
			this.ref_counts = new DVector<short>(raw_ref_counts);
			this.free_indices = new DVector<int>();
			this.used_count = 0;
			if (build_free_list)
			{
				this.rebuild_free_list();
			}
		}

		public DVector<short> RawRefCounts
		{
			get
			{
				return this.ref_counts;
			}
		}

		public bool empty
		{
			get
			{
				return this.used_count == 0;
			}
		}

		public int count
		{
			get
			{
				return this.used_count;
			}
		}

		public int max_index
		{
			get
			{
				return this.ref_counts.size;
			}
		}

		public bool is_dense
		{
			get
			{
				return this.free_indices.Length == 0;
			}
		}

		public bool isValid(int index)
		{
			return index >= 0 && index < this.ref_counts.size && this.ref_counts[index] > 0;
		}

		public bool isValidUnsafe(int index)
		{
			return this.ref_counts[index] > 0;
		}

		public int refCount(int index)
		{
			int num = (int)this.ref_counts[index];
			if (num != (int)RefCountVector.invalid)
			{
				return num;
			}
			return 0;
		}

		public int rawRefCount(int index)
		{
			return (int)this.ref_counts[index];
		}

		public int allocate()
		{
			this.used_count++;
			if (this.free_indices.empty)
			{
				this.ref_counts.push_back(1);
				return this.ref_counts.size - 1;
			}
			int back = (int)RefCountVector.invalid;
			while (back == (int)RefCountVector.invalid && !this.free_indices.empty)
			{
				back = this.free_indices.back;
				this.free_indices.pop_back();
			}
			if (back != (int)RefCountVector.invalid)
			{
				this.ref_counts[back] = 1;
				return back;
			}
			this.ref_counts.push_back(1);
			return this.ref_counts.size - 1;
		}

		public int increment(int index, short increment = 1)
		{
			DVector<short> dvector = this.ref_counts;
			dvector[index] += increment;
			return (int)this.ref_counts[index];
		}

		public void decrement(int index, short decrement = 1)
		{
			DVector<short> dvector = this.ref_counts;
			dvector[index] -= decrement;
			if (this.ref_counts[index] == 0)
			{
				this.free_indices.push_back(index);
				this.ref_counts[index] = RefCountVector.invalid;
				this.used_count--;
			}
		}

		public bool allocate_at(int index)
		{
			if (index >= this.ref_counts.size)
			{
				for (int i = this.ref_counts.size; i < index; i++)
				{
					this.ref_counts.push_back(RefCountVector.invalid);
					this.free_indices.push_back(i);
				}
				this.ref_counts.push_back(1);
				this.used_count++;
				return true;
			}
			if (this.ref_counts[index] > 0)
			{
				return false;
			}
			int size = this.free_indices.size;
			for (int j = 0; j < size; j++)
			{
				if (this.free_indices[j] == index)
				{
					this.free_indices[j] = (int)RefCountVector.invalid;
					this.ref_counts[index] = 1;
					this.used_count++;
					return true;
				}
			}
			return false;
		}

		public bool allocate_at_unsafe(int index)
		{
			if (index >= this.ref_counts.size)
			{
				for (int i = this.ref_counts.size; i < index; i++)
				{
					this.ref_counts.push_back(RefCountVector.invalid);
				}
				this.ref_counts.push_back(1);
				this.used_count++;
				return true;
			}
			if (this.ref_counts[index] > 0)
			{
				return false;
			}
			this.ref_counts[index] = 1;
			this.used_count++;
			return true;
		}

		public void set_Unsafe(int index, short count)
		{
			this.ref_counts[index] = count;
		}

		public void rebuild_free_list()
		{
			this.free_indices = new DVector<int>();
			this.used_count = 0;
			int length = this.ref_counts.Length;
			for (int i = 0; i < length; i++)
			{
				if (this.ref_counts[i] > 0)
				{
					this.used_count++;
				}
				else
				{
					this.free_indices.Add(i);
				}
			}
		}

		public void trim(int maxIndex)
		{
			this.free_indices = new DVector<int>();
			this.ref_counts.resize(maxIndex);
			this.used_count = maxIndex;
		}

		public IEnumerator GetEnumerator()
		{
			int nIndex = 0;
			int nLast = this.max_index;
			while (nIndex != nLast)
			{
				if (this.ref_counts[nIndex] > 0)
				{
					break;
				}
				int num = nIndex;
				nIndex = num + 1;
			}
			while (nIndex != nLast)
			{
				yield return nIndex;
				if (nIndex != nLast)
				{
					int num = nIndex;
					nIndex = num + 1;
				}
				while (nIndex != nLast && this.ref_counts[nIndex] <= 0)
				{
					int num = nIndex;
					nIndex = num + 1;
				}
			}
			yield break;
		}

		public string UsageStats
		{
			get
			{
				return string.Format("RefCountSize {0}  FreeSize {1} FreeMem {2}kb", this.ref_counts.size, this.free_indices.size, this.free_indices.MemoryUsageBytes / 1024);
			}
		}

		public string debug_print()
		{
			string text = string.Format("size {0} used {1} free_size {2}\n", this.ref_counts.size, this.used_count, this.free_indices.size);
			for (int i = 0; i < this.ref_counts.size; i++)
			{
				text += string.Format("{0}:{1} ", i, this.ref_counts[i]);
			}
			text += "\nfree:\n";
			for (int j = 0; j < this.free_indices.size; j++)
			{
				text = text + this.free_indices[j].ToString() + " ";
			}
			return text;
		}

		public static readonly short invalid = -1;

		private DVector<short> ref_counts;

		private DVector<int> free_indices;

		private int used_count;
	}
}
