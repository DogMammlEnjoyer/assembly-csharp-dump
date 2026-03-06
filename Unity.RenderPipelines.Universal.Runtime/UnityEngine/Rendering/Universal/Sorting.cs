using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.Universal
{
	internal struct Sorting
	{
		public static void QuickSort<T>(T[] data, Func<T, T, int> compare)
		{
			using (new ProfilingScope(Sorting.s_QuickSortSampler))
			{
				Sorting.QuickSort<T>(data, 0, data.Length - 1, compare);
			}
		}

		public static void QuickSort<T>(T[] data, int start, int end, Func<T, T, int> compare)
		{
			int num = end - start;
			if (num < 1)
			{
				return;
			}
			if (num < 8)
			{
				Sorting.InsertionSort<T>(data, start, end, compare);
				return;
			}
			if (start < end)
			{
				int num2 = Sorting.Partition<T>(data, start, end, compare);
				if (num2 >= 1)
				{
					Sorting.QuickSort<T>(data, start, num2, compare);
				}
				if (num2 + 1 < end)
				{
					Sorting.QuickSort<T>(data, num2 + 1, end, compare);
				}
			}
		}

		private static T Median3Pivot<T>(T[] data, int start, int pivot, int end, Func<T, T, int> compare)
		{
			Sorting.<>c__DisplayClass4_0<T> CS$<>8__locals1;
			CS$<>8__locals1.data = data;
			if (compare(CS$<>8__locals1.data[end], CS$<>8__locals1.data[start]) < 0)
			{
				Sorting.<Median3Pivot>g__Swap|4_0<T>(start, end, ref CS$<>8__locals1);
			}
			if (compare(CS$<>8__locals1.data[pivot], CS$<>8__locals1.data[start]) < 0)
			{
				Sorting.<Median3Pivot>g__Swap|4_0<T>(start, pivot, ref CS$<>8__locals1);
			}
			if (compare(CS$<>8__locals1.data[end], CS$<>8__locals1.data[pivot]) < 0)
			{
				Sorting.<Median3Pivot>g__Swap|4_0<T>(pivot, end, ref CS$<>8__locals1);
			}
			return CS$<>8__locals1.data[pivot];
		}

		private static int Partition<T>(T[] data, int start, int end, Func<T, T, int> compare)
		{
			int num = end - start;
			int pivot = start + num / 2;
			T arg = Sorting.Median3Pivot<T>(data, start, pivot, end, compare);
			for (;;)
			{
				if (compare(data[start], arg) >= 0)
				{
					while (compare(data[end], arg) > 0)
					{
						end--;
					}
					if (start >= end)
					{
						break;
					}
					T t = data[start];
					data[start++] = data[end];
					data[end--] = t;
				}
				else
				{
					start++;
				}
			}
			return end;
		}

		public static void InsertionSort<T>(T[] data, Func<T, T, int> compare)
		{
			using (new ProfilingScope(Sorting.s_InsertionSortSampler))
			{
				Sorting.InsertionSort<T>(data, 0, data.Length - 1, compare);
			}
		}

		public static void InsertionSort<T>(T[] data, int start, int end, Func<T, T, int> compare)
		{
			for (int i = start + 1; i < end + 1; i++)
			{
				T t = data[i];
				int num = i - 1;
				while (num >= 0 && compare(t, data[num]) < 0)
				{
					data[num + 1] = data[num];
					num--;
				}
				data[num + 1] = t;
			}
		}

		[CompilerGenerated]
		internal static void <Median3Pivot>g__Swap|4_0<T>(int a, int b, ref Sorting.<>c__DisplayClass4_0<T> A_2)
		{
			T t = A_2.data[a];
			A_2.data[a] = A_2.data[b];
			A_2.data[b] = t;
		}

		public static ProfilingSampler s_QuickSortSampler = new ProfilingSampler("QuickSort");

		public static ProfilingSampler s_InsertionSortSampler = new ProfilingSampler("InsertionSort");
	}
}
