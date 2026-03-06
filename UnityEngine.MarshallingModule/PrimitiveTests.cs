using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("MarshallingScriptingClasses.h")]
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class PrimitiveTests
	{
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterBool(bool param1, bool param2, int param3);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterInt(int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterOutInt(out int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterRefInt(ref int param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int ReturnInt();

		[NativeThrows]
		public unsafe static void ParameterIntDynamicArray(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				PrimitiveTests.ParameterIntDynamicArray_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntNullableDynamicArray(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				PrimitiveTests.ParameterIntNullableDynamicArray_Injected(ref managedSpanWrapper);
			}
		}

		public static int[] ReturnIntDynamicArray()
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnIntDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				result = array;
			}
			return result;
		}

		public static int[] ReturnNullIntDynamicArray()
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnNullIntDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				result = array;
			}
			return result;
		}

		public static bool[] ReturnBoolDynamicArray()
		{
			bool[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnBoolDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				bool[] array;
				blittableArrayWrapper.Unmarshal<bool>(ref array);
				result = array;
			}
			return result;
		}

		public static char[] ReturnCharDynamicArray()
		{
			char[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				PrimitiveTests.ReturnCharDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				char[] array;
				blittableArrayWrapper.Unmarshal<char>(ref array);
				result = array;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntDynamicArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntNullableDynamicArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnIntDynamicArray_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnNullIntDynamicArray_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnBoolDynamicArray_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnCharDynamicArray_Injected(out BlittableArrayWrapper ret);
	}
}
