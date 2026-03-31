using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class ArrayPoolUtil
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void EnsureCapacity<T>(ref T[] array, int index, ArrayPool<T> pool)
		{
			if (array.Length <= index)
			{
				ArrayPoolUtil.EnsureCapacityCore<T>(ref array, index, pool);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void EnsureCapacityCore<T>(ref T[] array, int index, ArrayPool<T> pool)
		{
			if (array.Length <= index)
			{
				int num = array.Length * 2;
				T[] array2 = pool.Rent((index < num) ? num : (index * 2));
				Array.Copy(array, 0, array2, 0, array.Length);
				pool.Return(array, !RuntimeHelpersAbstraction.IsWellKnownNoReferenceContainsType<T>());
				array = array2;
			}
		}

		public static ArrayPoolUtil.RentArray<T> Materialize<T>(IEnumerable<T> source)
		{
			T[] array = source as T[];
			if (array != null)
			{
				return new ArrayPoolUtil.RentArray<T>(array, array.Length, null);
			}
			int num = 32;
			ICollection<T> collection = source as ICollection<T>;
			if (collection != null)
			{
				if (collection.Count == 0)
				{
					return new ArrayPoolUtil.RentArray<T>(Array.Empty<T>(), 0, null);
				}
				num = collection.Count;
				ArrayPool<T> shared = ArrayPool<T>.Shared;
				T[] array2 = shared.Rent(num);
				collection.CopyTo(array2, 0);
				return new ArrayPoolUtil.RentArray<T>(array2, collection.Count, shared);
			}
			else
			{
				IReadOnlyCollection<T> readOnlyCollection = source as IReadOnlyCollection<T>;
				if (readOnlyCollection != null)
				{
					num = readOnlyCollection.Count;
				}
				if (num == 0)
				{
					return new ArrayPoolUtil.RentArray<T>(Array.Empty<T>(), 0, null);
				}
				ArrayPool<T> shared2 = ArrayPool<T>.Shared;
				int num2 = 0;
				T[] array3 = shared2.Rent(num);
				foreach (T t in source)
				{
					ArrayPoolUtil.EnsureCapacity<T>(ref array3, num2, shared2);
					array3[num2++] = t;
				}
				return new ArrayPoolUtil.RentArray<T>(array3, num2, shared2);
			}
		}

		public struct RentArray<T> : IDisposable
		{
			public RentArray(T[] array, int length, ArrayPool<T> pool)
			{
				this.Array = array;
				this.Length = length;
				this.pool = pool;
			}

			public void Dispose()
			{
				this.DisposeManually(!RuntimeHelpersAbstraction.IsWellKnownNoReferenceContainsType<T>());
			}

			public void DisposeManually(bool clearArray)
			{
				if (this.pool != null)
				{
					if (clearArray)
					{
						System.Array.Clear(this.Array, 0, this.Length);
					}
					this.pool.Return(this.Array, false);
					this.pool = null;
				}
			}

			public readonly T[] Array;

			public readonly int Length;

			private ArrayPool<T> pool;
		}
	}
}
