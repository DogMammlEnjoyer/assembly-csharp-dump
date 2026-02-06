using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class FixedArray
	{
		public unsafe static FixedArray<T> CreateFromFieldSequence<[IsUnmanaged] T>(ref T firstField, ref T lastField) where T : struct, ValueType
		{
			fixed (T* ptr = &firstField)
			{
				T* ptr2 = ptr;
				fixed (T* ptr3 = &lastField)
				{
					T* ptr4 = ptr3;
					return new FixedArray<T>(ptr2, (int)((long)((ptr4 - ptr2) / (IntPtr)sizeof(T) * (IntPtr)sizeof(T))) + 1);
				}
			}
		}

		public unsafe static FixedArray<T> Create<[IsUnmanaged] T>(ref T firstField, int length) where T : struct, ValueType
		{
			fixed (T* ptr = &firstField)
			{
				T* array = ptr;
				return new FixedArray<T>(array, length);
			}
		}

		public unsafe static FixedArray<TAdapted> Create<[IsUnmanaged] TActual, [IsUnmanaged] TAdapted>(ref TActual firstField, int length) where TActual : struct, ValueType where TAdapted : struct, ValueType
		{
			fixed (TActual* ptr = &firstField)
			{
				TActual* array = ptr;
				return new FixedArray<TAdapted>((TAdapted*)array, length);
			}
		}

		public static int IndexOf<[IsUnmanaged] T>(this FixedArray<T> array, T elem) where T : struct, ValueType, IEquatable<T>
		{
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = array[i].Equals(elem);
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
