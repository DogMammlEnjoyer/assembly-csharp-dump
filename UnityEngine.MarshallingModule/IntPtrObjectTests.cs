using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class IntPtrObjectTests
	{
		[NativeThrows]
		public static void ParameterIntPtrObject(MyIntPtrObject param)
		{
			IntPtrObjectTests.ParameterIntPtrObject_Injected((param == null) ? ((IntPtr)0) : MyIntPtrObject.BindingsMarshaller.ConvertToNative(param));
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterIntPtrObjectDynamicArray(MyIntPtrObject[] param);

		[NativeThrows]
		public static void ParameterStructIntPtrObject(StructIntPtrObject param)
		{
			IntPtrObjectTests.ParameterStructIntPtrObject_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MyIntPtrObject[] ReturnIntPtrObjectDynamicArray();

		[NativeThrows]
		public static void ParameterStructIntPtrObjectDynamicArray(StructIntPtrObjectDynamicArray param)
		{
			IntPtrObjectTests.ParameterStructIntPtrObjectDynamicArray_Injected(ref param);
		}

		public static MyIntPtrObject ReturnIntPtrObject(int value)
		{
			IntPtr intPtr = IntPtrObjectTests.ReturnIntPtrObject_Injected(value);
			return (intPtr == 0) ? null : MyIntPtrObject.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntPtrObject_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntPtrObject_Injected([In] ref StructIntPtrObject param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntPtrObjectDynamicArray_Injected([In] ref StructIntPtrObjectDynamicArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnIntPtrObject_Injected(int value);
	}
}
