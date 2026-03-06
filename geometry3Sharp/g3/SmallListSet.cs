using System;
using System.Collections.Generic;

namespace g3
{
	public class SmallListSet
	{
		public SmallListSet()
		{
			this.list_heads = new DVector<int>();
			this.linked_store = new DVector<int>();
			this.free_head_ptr = -1;
			this.block_store = new DVector<int>();
			this.free_blocks = new DVector<int>();
		}

		public SmallListSet(SmallListSet copy)
		{
			this.linked_store = new DVector<int>(copy.linked_store);
			this.free_head_ptr = copy.free_head_ptr;
			this.list_heads = new DVector<int>(copy.list_heads);
			this.block_store = new DVector<int>(copy.block_store);
			this.free_blocks = new DVector<int>(copy.free_blocks);
		}

		public int Size
		{
			get
			{
				return this.list_heads.size;
			}
		}

		public void Resize(int new_size)
		{
			int size = this.list_heads.size;
			if (new_size > size)
			{
				this.list_heads.resize(new_size);
				for (int i = size; i < new_size; i++)
				{
					this.list_heads[i] = -1;
				}
			}
		}

		public void AllocateAt(int list_index)
		{
			if (list_index >= this.list_heads.size)
			{
				int i = this.list_heads.size;
				this.list_heads.insert(-1, list_index);
				while (i < list_index)
				{
					this.list_heads[i] = -1;
					i++;
				}
				return;
			}
			if (this.list_heads[list_index] != -1)
			{
				throw new Exception("SmallListSet: list at " + list_index.ToString() + " is not empty!");
			}
		}

		public void Insert(int list_index, int val)
		{
			int num = this.list_heads[list_index];
			if (num == -1)
			{
				num = this.allocate_block();
				this.block_store[num] = 0;
				this.list_heads[list_index] = num;
			}
			int num2 = this.block_store[num];
			if (num2 < 8)
			{
				this.block_store[num + num2 + 1] = val;
			}
			else
			{
				int value = this.block_store[num + 9];
				if (this.free_head_ptr == -1)
				{
					int size = this.linked_store.size;
					this.linked_store.Add(val);
					this.linked_store.Add(value);
					this.block_store[num + 9] = size;
				}
				else
				{
					int num3 = this.free_head_ptr;
					this.free_head_ptr = this.linked_store[num3 + 1];
					this.linked_store[num3] = val;
					this.linked_store[num3 + 1] = value;
					this.block_store[num + 9] = num3;
				}
			}
			DVector<int> dvector = this.block_store;
			int i = num;
			dvector[i]++;
		}

		public bool Remove(int list_index, int val)
		{
			int num = this.list_heads[list_index];
			int num2 = this.block_store[num];
			int num3 = num + Math.Min(num2, 8);
			for (int i = num + 1; i <= num3; i++)
			{
				if (this.block_store[i] == val)
				{
					for (int j = i + 1; j <= num3; j++)
					{
						this.block_store[j - 1] = this.block_store[j];
					}
					if (num2 > 8)
					{
						int num4 = this.block_store[num + 9];
						this.block_store[num + 9] = this.linked_store[num4 + 1];
						this.block_store[num3] = this.linked_store[num4];
						this.add_free_link(num4);
					}
					DVector<int> dvector = this.block_store;
					int i2 = num;
					dvector[i2]--;
					return true;
				}
			}
			if (num2 > 8 && this.remove_from_linked_list(num, val))
			{
				DVector<int> dvector = this.block_store;
				int i2 = num;
				dvector[i2]--;
				return true;
			}
			return false;
		}

		public void Move(int from_index, int to_index)
		{
			if (this.list_heads[to_index] != -1)
			{
				throw new Exception("SmallListSet.MoveTo: list at " + to_index.ToString() + " is not empty!");
			}
			if (this.list_heads[from_index] == -1)
			{
				throw new Exception("SmallListSet.MoveTo: list at " + from_index.ToString() + " is empty!");
			}
			this.list_heads[to_index] = this.list_heads[from_index];
			this.list_heads[from_index] = -1;
		}

		public void Clear(int list_index)
		{
			int num = this.list_heads[list_index];
			if (num != -1)
			{
				if (this.block_store[num] > 8)
				{
					int num2 = this.block_store[num + 9];
					while (num2 != -1)
					{
						int ptr = num2;
						num2 = this.linked_store[num2 + 1];
						this.add_free_link(ptr);
					}
					this.block_store[num + 9] = -1;
				}
				this.block_store[num] = 0;
				this.free_blocks.push_back(num);
				this.list_heads[list_index] = -1;
			}
		}

		public int Count(int list_index)
		{
			int num = this.list_heads[list_index];
			if (num != -1)
			{
				return this.block_store[num];
			}
			return 0;
		}

		public bool Contains(int list_index, int val)
		{
			int num = this.list_heads[list_index];
			if (num != -1)
			{
				int num2 = this.block_store[num];
				if (num2 < 8)
				{
					int num3 = num + num2;
					for (int i = num + 1; i <= num3; i++)
					{
						if (this.block_store[i] == val)
						{
							return true;
						}
					}
				}
				else
				{
					int num4 = num + 8;
					for (int j = num + 1; j <= num4; j++)
					{
						if (this.block_store[j] == val)
						{
							return true;
						}
					}
					for (int num5 = this.block_store[num + 9]; num5 != -1; num5 = this.linked_store[num5 + 1])
					{
						if (this.linked_store[num5] == val)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public int First(int list_index)
		{
			int num = this.list_heads[list_index];
			return this.block_store[num + 1];
		}

		public IEnumerable<int> ValueItr(int list_index)
		{
			int block_ptr = this.list_heads[list_index];
			if (block_ptr != -1)
			{
				int num = this.block_store[block_ptr];
				if (num < 8)
				{
					int iEnd = block_ptr + num;
					int num2;
					for (int i = block_ptr + 1; i <= iEnd; i = num2)
					{
						yield return this.block_store[i];
						num2 = i + 1;
					}
				}
				else
				{
					int iEnd = block_ptr + 8;
					int num2;
					for (int j = block_ptr + 1; j <= iEnd; j = num2)
					{
						yield return this.block_store[j];
						num2 = j + 1;
					}
					for (int i = this.block_store[block_ptr + 9]; i != -1; i = this.linked_store[i + 1])
					{
						yield return this.linked_store[i];
					}
				}
			}
			yield break;
		}

		public int Find(int list_index, Func<int, bool> findF, int invalidValue = -1)
		{
			int num = this.list_heads[list_index];
			if (num != -1)
			{
				int num2 = this.block_store[num];
				if (num2 < 8)
				{
					int num3 = num + num2;
					for (int i = num + 1; i <= num3; i++)
					{
						int num4 = this.block_store[i];
						if (findF(num4))
						{
							return num4;
						}
					}
				}
				else
				{
					int num5 = num + 8;
					for (int j = num + 1; j <= num5; j++)
					{
						int num6 = this.block_store[j];
						if (findF(num6))
						{
							return num6;
						}
					}
					for (int num7 = this.block_store[num + 9]; num7 != -1; num7 = this.linked_store[num7 + 1])
					{
						int num8 = this.linked_store[num7];
						if (findF(num8))
						{
							return num8;
						}
					}
				}
			}
			return invalidValue;
		}

		public bool Replace(int list_index, Func<int, bool> findF, int new_value)
		{
			int num = this.list_heads[list_index];
			if (num != -1)
			{
				int num2 = this.block_store[num];
				if (num2 < 8)
				{
					int num3 = num + num2;
					for (int i = num + 1; i <= num3; i++)
					{
						int arg = this.block_store[i];
						if (findF(arg))
						{
							this.block_store[i] = new_value;
							return true;
						}
					}
				}
				else
				{
					int num4 = num + 8;
					for (int j = num + 1; j <= num4; j++)
					{
						int arg2 = this.block_store[j];
						if (findF(arg2))
						{
							this.block_store[j] = new_value;
							return true;
						}
					}
					for (int num5 = this.block_store[num + 9]; num5 != -1; num5 = this.linked_store[num5 + 1])
					{
						int arg3 = this.linked_store[num5];
						if (findF(arg3))
						{
							this.linked_store[num5] = new_value;
							return true;
						}
					}
				}
			}
			return false;
		}

		protected int allocate_block()
		{
			int size = this.free_blocks.size;
			if (size > 0)
			{
				int result = this.free_blocks[size - 1];
				this.free_blocks.pop_back();
				return result;
			}
			int size2 = this.block_store.size;
			this.block_store.insert(-1, size2 + 9);
			this.block_store[size2] = 0;
			this.allocated_count++;
			return size2;
		}

		private void add_free_link(int ptr)
		{
			this.linked_store[ptr + 1] = this.free_head_ptr;
			this.free_head_ptr = ptr;
		}

		private bool remove_from_linked_list(int block_ptr, int val)
		{
			int num = this.block_store[block_ptr + 9];
			int num2 = -1;
			while (num != -1)
			{
				if (this.linked_store[num] == val)
				{
					int value = this.linked_store[num + 1];
					if (num2 == -1)
					{
						this.block_store[block_ptr + 9] = value;
					}
					else
					{
						this.linked_store[num2 + 1] = value;
					}
					this.add_free_link(num);
					return true;
				}
				num2 = num;
				num = this.linked_store[num + 1];
			}
			return false;
		}

		public string MemoryUsage
		{
			get
			{
				return string.Format("ListSize {0}  Blocks Count {1} Free {2} Mem {3}kb  Linked Mem {4}kb", new object[]
				{
					this.list_heads.size,
					this.allocated_count,
					this.free_blocks.size * 4 / 1024,
					this.block_store.size,
					this.linked_store.size * 4 / 1024
				});
			}
		}

		private const int Null = -1;

		private const int BLOCKSIZE = 8;

		private const int BLOCK_LIST_OFFSET = 9;

		private DVector<int> list_heads;

		private DVector<int> block_store;

		private DVector<int> free_blocks;

		private int allocated_count;

		private DVector<int> linked_store;

		private int free_head_ptr;
	}
}
