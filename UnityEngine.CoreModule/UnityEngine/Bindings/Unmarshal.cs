using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal struct Unmarshal
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T UnmarshalUnityObject<T>(IntPtr gcHandlePtr) where T : Object
		{
			bool flag = gcHandlePtr == IntPtr.Zero;
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				T t = (T)((object)Unmarshal.FromIntPtrUnsafe(gcHandlePtr).Target);
				result = t;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static GCHandle FromIntPtrUnsafe(IntPtr gcHandle)
		{
			return *UnsafeUtility.As<IntPtr, GCHandle>(ref gcHandle);
		}
	}
}
