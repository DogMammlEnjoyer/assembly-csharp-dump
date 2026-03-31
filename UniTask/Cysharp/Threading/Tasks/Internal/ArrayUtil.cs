using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class ArrayUtil
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EnsureCapacity<T>(ref T[] array, int index)
		{
			if (array.Length <= index)
			{
				ArrayUtil.EnsureCore<T>(ref array, index);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void EnsureCore<T>(ref T[] array, int index)
		{
			int num = array.Length * 2;
			T[] array2 = new T[(index < num) ? num : (index * 2)];
			Array.Copy(array, 0, array2, 0, array.Length);
			array = array2;
		}

		[return: TupleElementNames(new string[]
		{
			"array",
			"length"
		})]
		public static ValueTuple<T[], int> Materialize<T>(IEnumerable<T> source)
		{
			T[] array = source as T[];
			if (array != null)
			{
				return new ValueTuple<T[], int>(array, array.Length);
			}
			int num = 4;
			ICollection<T> collection = source as ICollection<T>;
			if (collection != null)
			{
				num = collection.Count;
				T[] array2 = new T[num];
				collection.CopyTo(array2, 0);
				return new ValueTuple<T[], int>(array2, num);
			}
			IReadOnlyCollection<T> readOnlyCollection = source as IReadOnlyCollection<T>;
			if (readOnlyCollection != null)
			{
				num = readOnlyCollection.Count;
			}
			if (num == 0)
			{
				return new ValueTuple<T[], int>(Array.Empty<T>(), 0);
			}
			int num2 = 0;
			T[] array3 = new T[num];
			foreach (T t in source)
			{
				ArrayUtil.EnsureCapacity<T>(ref array3, num2);
				array3[num2++] = t;
			}
			return new ValueTuple<T[], int>(array3, num2);
		}
	}
}
