using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal static class ArraySpecialized
	{
		private static int FloorLog2(int n)
		{
			int num = 0;
			while (n >= 1)
			{
				num++;
				n /= 2;
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Swap<T>(T[] a, int i, int j)
		{
			Assert.Check(i != j);
			T t = a[i];
			a[i] = a[j];
			a[j] = t;
		}

		public static void Sort(int[] array, int index, int length)
		{
			bool flag = length >= 2;
			if (flag)
			{
				ArraySpecialized.IntroSort(array, index, length + index - 1, 2 * ArraySpecialized.FloorLog2(array.Length));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Compare(int x, int y)
		{
			return x - y;
		}

		private static void SwapIfGreater(int[] array, int a, int b)
		{
			bool flag = a != b && ArraySpecialized.Compare(array[a], array[b]) > 0;
			if (flag)
			{
				int num = array[a];
				array[a] = array[b];
				array[b] = num;
			}
		}

		private static void IntroSort(int[] array, int lo, int hi, int depthLimit)
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				bool flag = num <= 16;
				if (flag)
				{
					switch (num)
					{
					case 1:
						break;
					case 2:
						ArraySpecialized.SwapIfGreater(array, lo, hi);
						break;
					case 3:
						ArraySpecialized.SwapIfGreater(array, lo, hi - 1);
						ArraySpecialized.SwapIfGreater(array, lo, hi);
						ArraySpecialized.SwapIfGreater(array, hi - 1, hi);
						break;
					default:
						ArraySpecialized.InsertionSort(array, lo, hi);
						break;
					}
					break;
				}
				bool flag2 = depthLimit == 0;
				if (flag2)
				{
					ArraySpecialized.Heapsort(array, lo, hi);
					break;
				}
				depthLimit--;
				int num2 = ArraySpecialized.PickPivotAndPartition(array, lo, hi);
				ArraySpecialized.IntroSort(array, num2 + 1, hi, depthLimit);
				hi = num2 - 1;
			}
		}

		private static int PickPivotAndPartition(int[] array, int lo, int hi)
		{
			int num = lo + (hi - lo) / 2;
			ArraySpecialized.SwapIfGreater(array, lo, num);
			ArraySpecialized.SwapIfGreater(array, lo, hi);
			ArraySpecialized.SwapIfGreater(array, num, hi);
			int num2 = array[num];
			ArraySpecialized.Swap<int>(array, num, hi - 1);
			int i = lo;
			int num3 = hi - 1;
			while (i < num3)
			{
				while (ArraySpecialized.Compare(array[++i], num2) < 0)
				{
				}
				while (ArraySpecialized.Compare(num2, array[--num3]) < 0)
				{
				}
				bool flag = i >= num3;
				if (flag)
				{
					break;
				}
				ArraySpecialized.Swap<int>(array, i, num3);
			}
			bool flag2 = i != hi - 1;
			if (flag2)
			{
				ArraySpecialized.Swap<int>(array, i, hi - 1);
			}
			return i;
		}

		private static void Heapsort(int[] array, int lo, int hi)
		{
			int num = hi - lo + 1;
			for (int i = num / 2; i >= 1; i--)
			{
				ArraySpecialized.DownHeap(array, i, num, lo);
			}
			for (int j = num; j > 1; j--)
			{
				ArraySpecialized.Swap<int>(array, lo, lo + j - 1);
				ArraySpecialized.DownHeap(array, 1, j - 1, lo);
			}
		}

		private static void DownHeap(int[] array, int i, int n, int lo)
		{
			int num = array[lo + i - 1];
			while (i <= n / 2)
			{
				int num2 = 2 * i;
				bool flag = num2 < n && ArraySpecialized.Compare(array[lo + num2 - 1], array[lo + num2]) < 0;
				if (flag)
				{
					num2++;
				}
				bool flag2 = ArraySpecialized.Compare(num, array[lo + num2 - 1]) >= 0;
				if (flag2)
				{
					break;
				}
				array[lo + i - 1] = array[lo + num2 - 1];
				i = num2;
			}
			array[lo + i - 1] = num;
		}

		private static void InsertionSort(int[] array, int lo, int hi)
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				int num2 = array[i + 1];
				while (num >= lo && ArraySpecialized.Compare(num2, array[num]) < 0)
				{
					array[num + 1] = array[num];
					num--;
				}
				array[num + 1] = num2;
			}
		}

		public static void Sort(SimulationInput[] array, int index, int length)
		{
			bool flag = length >= 2;
			if (flag)
			{
				ArraySpecialized.IntroSort(array, index, length + index - 1, 2 * ArraySpecialized.FloorLog2(array.Length));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static int Compare(SimulationInput x, SimulationInput y)
		{
			return x.Header->Tick.CompareTo(y.Header->Tick);
		}

		private static void SwapIfGreater(SimulationInput[] array, int a, int b)
		{
			bool flag = a != b && ArraySpecialized.Compare(array[a], array[b]) > 0;
			if (flag)
			{
				SimulationInput simulationInput = array[a];
				array[a] = array[b];
				array[b] = simulationInput;
			}
		}

		private static void IntroSort(SimulationInput[] array, int lo, int hi, int depthLimit)
		{
			while (hi > lo)
			{
				int num = hi - lo + 1;
				bool flag = num <= 16;
				if (flag)
				{
					switch (num)
					{
					case 1:
						break;
					case 2:
						ArraySpecialized.SwapIfGreater(array, lo, hi);
						break;
					case 3:
						ArraySpecialized.SwapIfGreater(array, lo, hi - 1);
						ArraySpecialized.SwapIfGreater(array, lo, hi);
						ArraySpecialized.SwapIfGreater(array, hi - 1, hi);
						break;
					default:
						ArraySpecialized.InsertionSort(array, lo, hi);
						break;
					}
					break;
				}
				bool flag2 = depthLimit == 0;
				if (flag2)
				{
					ArraySpecialized.Heapsort(array, lo, hi);
					break;
				}
				depthLimit--;
				int num2 = ArraySpecialized.PickPivotAndPartition(array, lo, hi);
				ArraySpecialized.IntroSort(array, num2 + 1, hi, depthLimit);
				hi = num2 - 1;
			}
		}

		private static int PickPivotAndPartition(SimulationInput[] array, int lo, int hi)
		{
			int num = lo + (hi - lo) / 2;
			ArraySpecialized.SwapIfGreater(array, lo, num);
			ArraySpecialized.SwapIfGreater(array, lo, hi);
			ArraySpecialized.SwapIfGreater(array, num, hi);
			SimulationInput simulationInput = array[num];
			ArraySpecialized.Swap<SimulationInput>(array, num, hi - 1);
			int i = lo;
			int num2 = hi - 1;
			while (i < num2)
			{
				while (ArraySpecialized.Compare(array[++i], simulationInput) < 0)
				{
				}
				while (ArraySpecialized.Compare(simulationInput, array[--num2]) < 0)
				{
				}
				bool flag = i >= num2;
				if (flag)
				{
					break;
				}
				ArraySpecialized.Swap<SimulationInput>(array, i, num2);
			}
			bool flag2 = i != hi - 1;
			if (flag2)
			{
				ArraySpecialized.Swap<SimulationInput>(array, i, hi - 1);
			}
			return i;
		}

		private static void Heapsort(SimulationInput[] array, int lo, int hi)
		{
			int num = hi - lo + 1;
			for (int i = num / 2; i >= 1; i--)
			{
				ArraySpecialized.DownHeap(array, i, num, lo);
			}
			for (int j = num; j > 1; j--)
			{
				ArraySpecialized.Swap<SimulationInput>(array, lo, lo + j - 1);
				ArraySpecialized.DownHeap(array, 1, j - 1, lo);
			}
		}

		private static void DownHeap(SimulationInput[] array, int i, int n, int lo)
		{
			SimulationInput simulationInput = array[lo + i - 1];
			while (i <= n / 2)
			{
				int num = 2 * i;
				bool flag = num < n && ArraySpecialized.Compare(array[lo + num - 1], array[lo + num]) < 0;
				if (flag)
				{
					num++;
				}
				bool flag2 = ArraySpecialized.Compare(simulationInput, array[lo + num - 1]) >= 0;
				if (flag2)
				{
					break;
				}
				array[lo + i - 1] = array[lo + num - 1];
				i = num;
			}
			array[lo + i - 1] = simulationInput;
		}

		private static void InsertionSort(SimulationInput[] array, int lo, int hi)
		{
			for (int i = lo; i < hi; i++)
			{
				int num = i;
				SimulationInput simulationInput = array[i + 1];
				while (num >= lo && ArraySpecialized.Compare(simulationInput, array[num]) < 0)
				{
					array[num + 1] = array[num];
					num--;
				}
				array[num + 1] = simulationInput;
			}
		}

		private const int IntrosortSizeThreshold = 16;
	}
}
