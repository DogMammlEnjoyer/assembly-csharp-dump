using System;
using System.Collections.Generic;

namespace UnityEngine.Splines
{
	internal static class ArrayUtility
	{
		public static void RemoveAt<T>(ref T[] array, int index)
		{
			if (index < 0 || index >= array.Length)
			{
				throw new IndexOutOfRangeException();
			}
			Array.Copy(array, index + 1, array, index, array.Length - index - 1);
			Array.Resize<T>(ref array, array.Length - 1);
		}

		public static void RemoveAt<T>(ref T[] array, IEnumerable<int> indices)
		{
			List<int> list = new List<int>(indices);
			list.Sort();
			ArrayUtility.SortedRemoveAt<T>(ref array, list);
		}

		public static void SortedRemoveAt<T>(ref T[] array, IList<int> sorted)
		{
			int count = sorted.Count;
			int num = array.Length;
			T[] array2 = new T[num - count];
			int i = 0;
			for (int j = 0; j < num; j++)
			{
				if (i < count && sorted[i] == j)
				{
					while (i < count)
					{
						if (sorted[i] != j)
						{
							break;
						}
						i++;
					}
				}
				else
				{
					array2[j - i] = array[j];
				}
			}
			array = array2;
		}

		public static void Remove<T>(ref T[] array, T element)
		{
			int num = Array.IndexOf<T>(array, element);
			if (num >= 0)
			{
				ArrayUtility.RemoveAt<T>(ref array, num);
			}
		}

		public static void Add<T>(ref T[] array, T element)
		{
			Array.Resize<T>(ref array, array.Length + 1);
			array[array.Length - 1] = element;
		}
	}
}
