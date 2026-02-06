using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class NetworkArrayExtensions
	{
		public static int IndexOf<[IsUnmanaged] T>(this NetworkArray<T> array, T elem) where T : struct, ValueType, IEquatable<T>
		{
			for (int i = 0; i < array.Length; i++)
			{
				T t = array[i];
				bool flag = t.Equals(elem);
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		public static ref T GetRef<[IsUnmanaged] T>(this NetworkArray<T> array, int index) where T : struct, ValueType
		{
			return array.GetRef(index);
		}
	}
}
