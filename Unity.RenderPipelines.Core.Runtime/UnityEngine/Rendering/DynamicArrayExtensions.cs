using System;

namespace UnityEngine.Rendering
{
	public static class DynamicArrayExtensions
	{
		private unsafe static int Partition<T>(Span<T> data, int left, int right) where T : IComparable<T>, new()
		{
			T other = *data[left];
			left--;
			right++;
			for (;;)
			{
				T t = default(T);
				int num;
				do
				{
					left++;
					t = *data[left];
					num = t.CompareTo(other);
				}
				while (num < 0);
				T t2 = default(T);
				do
				{
					right--;
					t2 = *data[right];
					num = t2.CompareTo(other);
				}
				while (num > 0);
				if (left >= right)
				{
					break;
				}
				*data[right] = t;
				*data[left] = t2;
			}
			return right;
		}

		private static void QuickSort<T>(Span<T> data, int left, int right) where T : IComparable<T>, new()
		{
			if (left < right)
			{
				int num = DynamicArrayExtensions.Partition<T>(data, left, right);
				if (num >= 1)
				{
					DynamicArrayExtensions.QuickSort<T>(data, left, num);
				}
				if (num + 1 < right)
				{
					DynamicArrayExtensions.QuickSort<T>(data, num + 1, right);
				}
			}
		}

		private unsafe static int Partition<T>(Span<T> data, int left, int right, DynamicArray<T>.SortComparer comparer) where T : new()
		{
			T y = *data[left];
			left--;
			right++;
			for (;;)
			{
				T t = default(T);
				int num;
				do
				{
					left++;
					t = *data[left];
					num = comparer(t, y);
				}
				while (num < 0);
				T t2 = default(T);
				do
				{
					right--;
					t2 = *data[right];
					num = comparer(t2, y);
				}
				while (num > 0);
				if (left >= right)
				{
					break;
				}
				*data[right] = t;
				*data[left] = t2;
			}
			return right;
		}

		private static void QuickSort<T>(Span<T> data, int left, int right, DynamicArray<T>.SortComparer comparer) where T : new()
		{
			if (left < right)
			{
				int num = DynamicArrayExtensions.Partition<T>(data, left, right, comparer);
				if (num >= 1)
				{
					DynamicArrayExtensions.QuickSort<T>(data, left, num, comparer);
				}
				if (num + 1 < right)
				{
					DynamicArrayExtensions.QuickSort<T>(data, num + 1, right, comparer);
				}
			}
		}

		public static void QuickSort<T>(this DynamicArray<T> array) where T : IComparable<T>, new()
		{
			DynamicArrayExtensions.QuickSort<T>(array, 0, array.size - 1);
			array.BumpVersion();
		}

		public static void QuickSort<T>(this DynamicArray<T> array, DynamicArray<T>.SortComparer comparer) where T : new()
		{
			DynamicArrayExtensions.QuickSort<T>(array, 0, array.size - 1, comparer);
			array.BumpVersion();
		}
	}
}
