using System;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal abstract class AsyncEnumerableSorter<TElement>
	{
		internal abstract UniTask ComputeKeysAsync(TElement[] elements, int count);

		internal abstract int CompareKeys(int index1, int index2);

		internal UniTask<int[]> SortAsync(TElement[] elements, int count)
		{
			AsyncEnumerableSorter<TElement>.<SortAsync>d__2 <SortAsync>d__;
			<SortAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<int[]>.Create();
			<SortAsync>d__.<>4__this = this;
			<SortAsync>d__.elements = elements;
			<SortAsync>d__.count = count;
			<SortAsync>d__.<>1__state = -1;
			<SortAsync>d__.<>t__builder.Start<AsyncEnumerableSorter<TElement>.<SortAsync>d__2>(ref <SortAsync>d__);
			return <SortAsync>d__.<>t__builder.Task;
		}

		private void QuickSort(int[] map, int left, int right)
		{
			do
			{
				int num = left;
				int num2 = right;
				int index = map[num + (num2 - num >> 1)];
				do
				{
					if (num < map.Length)
					{
						if (this.CompareKeys(index, map[num]) > 0)
						{
							num++;
							continue;
						}
					}
					while (num2 >= 0 && this.CompareKeys(index, map[num2]) < 0)
					{
						num2--;
					}
					if (num > num2)
					{
						break;
					}
					if (num < num2)
					{
						int num3 = map[num];
						map[num] = map[num2];
						map[num2] = num3;
					}
					num++;
					num2--;
				}
				while (num <= num2);
				if (num2 - left <= right - num)
				{
					if (left < num2)
					{
						this.QuickSort(map, left, num2);
					}
					left = num;
				}
				else
				{
					if (num < right)
					{
						this.QuickSort(map, num, right);
					}
					right = num2;
				}
			}
			while (left < right);
		}
	}
}
