using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.Universal
{
	internal struct Fixed2<[IsUnmanaged] T> where T : struct, ValueType
	{
		public Fixed2(T item1)
		{
			this = new Fixed2<T>(item1, item1);
		}

		public Fixed2(T item1, T item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}

		public unsafe T this[int index]
		{
			get
			{
				fixed (T* ptr = &this.item1)
				{
					return ptr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
			}
			set
			{
				fixed (T* ptr = &this.item1)
				{
					ptr[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
				}
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckRange(int index)
		{
		}

		public T item1;

		public T item2;
	}
}
