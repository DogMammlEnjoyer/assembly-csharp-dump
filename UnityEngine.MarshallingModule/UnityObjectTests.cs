using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class UnityObjectTests
	{
		[NativeThrows]
		public static void ParameterUnityObject(MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObject_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterUnityObjectByRef(ref MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObjectByRef_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterUnityObjectPPtr(MarshallingTestObject param)
		{
			UnityObjectTests.ParameterUnityObjectPPtr_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(param));
		}

		[NativeThrows]
		public static void ParameterStructUnityObject(StructUnityObject param)
		{
			UnityObjectTests.ParameterStructUnityObject_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructUnityObjectPPtr(StructUnityObjectPPtr param)
		{
			UnityObjectTests.ParameterStructUnityObjectPPtr_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructUnityObjectDynamicArray(StructUnityObjectDynamicArray param)
		{
			UnityObjectTests.ParameterStructUnityObjectDynamicArray_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterUnityObjectDynamicArray(MarshallingTestObject[] param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterUnityObjectPPtrDynamicArray(MarshallingTestObject[] param);

		public static MarshallingTestObject ReturnUnityObject()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObject_Injected());
		}

		public static MarshallingTestObject ReturnInUnityObject(MarshallingTestObject obj)
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnInUnityObject_Injected(Object.MarshalledUnityObject.Marshal<MarshallingTestObject>(obj)));
		}

		public static MarshallingTestObject ReturnUnityObjectFakeNull()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObjectFakeNull_Injected());
		}

		public static MarshallingTestObject ReturnUnassignedErrorObject()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnassignedErrorObject_Injected());
		}

		public static MarshallingTestObject ReturnUnityObjectPPtr()
		{
			return Unmarshal.UnmarshalUnityObject<MarshallingTestObject>(UnityObjectTests.ReturnUnityObjectPPtr_Injected());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MarshallingTestObject[] ReturnUnityObjectDynamicArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MarshallingTestObject[] ReturnUnityObjectPPtrDynamicArray();

		public static StructUnityObject ReturnStructUnityObject()
		{
			StructUnityObject result;
			UnityObjectTests.ReturnStructUnityObject_Injected(out result);
			return result;
		}

		public static StructUnityObjectPPtr ReturnStructUnityObjectPPtr()
		{
			StructUnityObjectPPtr result;
			UnityObjectTests.ReturnStructUnityObjectPPtr_Injected(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObject[] ReturnStructUnityObjectDynamicArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObjectPPtr[] ReturnStructUnityObjectPPtrDynamicArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructUnityObjectDynamicArray[] ReturnStructUnityObjectDynamicArrayDynamicArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObject_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObjectByRef_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterUnityObjectPPtr_Injected(IntPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObject_Injected([In] ref StructUnityObject param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObjectPPtr_Injected([In] ref StructUnityObjectPPtr param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructUnityObjectDynamicArray_Injected([In] ref StructUnityObjectDynamicArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObject_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnInUnityObject_Injected(IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObjectFakeNull_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnassignedErrorObject_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ReturnUnityObjectPPtr_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructUnityObject_Injected(out StructUnityObject ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructUnityObjectPPtr_Injected(out StructUnityObjectPPtr ret);
	}
}
