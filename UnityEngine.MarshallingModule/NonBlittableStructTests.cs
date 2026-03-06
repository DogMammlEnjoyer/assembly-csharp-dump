using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class NonBlittableStructTests
	{
		[NativeThrows]
		public static void ParameterStructWithStringIntAndFloat(StructWithStringIntAndFloat param)
		{
			NonBlittableStructTests.ParameterStructWithStringIntAndFloat_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RefParameterStructWithStringIntAndFloat(ref StructWithStringIntAndFloat param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void OutParameterStructWithStringIntAndFloat(out StructWithStringIntAndFloat param);

		public static void ParameterStructWithStringIntAndFloat2(StructWithStringIntAndFloat2 param)
		{
			NonBlittableStructTests.ParameterStructWithStringIntAndFloat2_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructWithStringIgnoredIntAndFloat(StructWithStringIgnoredIntAndFloat param)
		{
			NonBlittableStructTests.ParameterStructWithStringIgnoredIntAndFloat_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterStructWithStringIntAndFloatArray(StructWithStringIntAndFloat[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern StructWithStringIntAndFloat[] ReturnStructWithStringIntAndFloatArray();

		[NativeThrows]
		public static void ParameterStructWithNonBlittableArrayField(StructWithNonBlittableArrayField param)
		{
			NonBlittableStructTests.ParameterStructWithNonBlittableArrayField_Injected(ref param);
		}

		public static StructWithNonBlittableArrayField ReturnStructWithNonBlittableArrayField()
		{
			StructWithNonBlittableArrayField result;
			NonBlittableStructTests.ReturnStructWithNonBlittableArrayField_Injected(out result);
			return result;
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CanMarshalManagedObjectToStruct(ClassToStruct param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CanMarshalOutManagedObjectToStruct([Out] ClassToStruct param);

		[NativeThrows]
		public static void CanMarshalStructWithNativeAsStructField(StructWithClassToStruct param)
		{
			NonBlittableStructTests.CanMarshalStructWithNativeAsStructField_Injected(ref param);
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CanMarshalNativeAsStructArray(ClassToStruct[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ClassToStruct CanUnmarshalManagedObjectFromStruct();

		public static StructWithClassToStruct CanUnmarshalStructWithNativeAsStructField()
		{
			StructWithClassToStruct result;
			NonBlittableStructTests.CanUnmarshalStructWithNativeAsStructField_Injected(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ClassToStruct[] CanUnmarshalNativeAsStructArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithStringIntAndFloat_Injected([In] ref StructWithStringIntAndFloat param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithStringIntAndFloat2_Injected([In] ref StructWithStringIntAndFloat2 param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithStringIgnoredIntAndFloat_Injected([In] ref StructWithStringIgnoredIntAndFloat param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructWithNonBlittableArrayField_Injected([In] ref StructWithNonBlittableArrayField param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructWithNonBlittableArrayField_Injected(out StructWithNonBlittableArrayField ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshalStructWithNativeAsStructField_Injected([In] ref StructWithClassToStruct param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshalStructWithNativeAsStructField_Injected(out StructWithClassToStruct ret);
	}
}
