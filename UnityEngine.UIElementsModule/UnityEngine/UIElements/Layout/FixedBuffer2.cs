using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout
{
	[Serializable]
	internal struct FixedBuffer2<[IsUnmanaged] T> where T : struct, ValueType
	{
		public unsafe T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= 2;
				if (flag)
				{
					throw new IndexOutOfRangeException("index");
				}
				fixed (FixedBuffer2<T>* ptr = (FixedBuffer2<T>*)(&this))
				{
					void* ptr2 = (void*)ptr;
					T* ptr3 = (T*)ptr2;
					return ref ptr3[(IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
				}
			}
		}

		[SerializeField]
		private T __0;

		[SerializeField]
		private T __1;

		public const int Length = 2;
	}
}
