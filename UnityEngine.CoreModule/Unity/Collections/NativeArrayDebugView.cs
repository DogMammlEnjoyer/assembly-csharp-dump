using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	internal sealed class NativeArrayDebugView<T> where T : struct
	{
		public NativeArrayDebugView(NativeArray<T> array)
		{
			this.m_Array = array;
		}

		public unsafe T[] Items
		{
			get
			{
				bool flag = !this.m_Array.IsCreated;
				T[] result;
				if (flag)
				{
					result = null;
				}
				else
				{
					int length = this.m_Array.m_Length;
					T[] array = new T[length];
					GCHandle gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
					IntPtr value = gchandle.AddrOfPinnedObject();
					UnsafeUtility.MemCpy((void*)value, this.m_Array.m_Buffer, (long)(length * UnsafeUtility.SizeOf<T>()));
					gchandle.Free();
					result = array;
				}
				return result;
			}
		}

		private NativeArray<T> m_Array;
	}
}
