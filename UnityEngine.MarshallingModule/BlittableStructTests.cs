using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class BlittableStructTests
	{
		[NativeThrows]
		public static void ParameterStructInt(StructInt param)
		{
			BlittableStructTests.ParameterStructInt_Injected(ref param);
		}

		public static void ParameterStructInt2(StructInt2 param)
		{
			BlittableStructTests.ParameterStructInt2_Injected(ref param);
		}

		public static StructInt ReturnStructInt()
		{
			StructInt result;
			BlittableStructTests.ReturnStructInt_Injected(out result);
			return result;
		}

		[NativeThrows]
		public static void ParameterNestedBlittableStruct(StructNestedBlittable s)
		{
			BlittableStructTests.ParameterNestedBlittableStruct_Injected(ref s);
		}

		public static StructNestedBlittable ReturnNestedBlittableStruct()
		{
			StructNestedBlittable result;
			BlittableStructTests.ReturnNestedBlittableStruct_Injected(out result);
			return result;
		}

		[NativeThrows]
		public unsafe static void ParameterStructIntDynamicArray(StructInt[] param)
		{
			Span<StructInt> span = new Span<StructInt>(param);
			fixed (StructInt* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BlittableStructTests.ParameterStructIntDynamicArray_Injected(ref managedSpanWrapper);
			}
		}

		public static StructInt[] ReturnStructIntDynamicArray()
		{
			StructInt[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableStructTests.ReturnStructIntDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				StructInt[] array;
				blittableArrayWrapper.Unmarshal<StructInt>(ref array);
				result = array;
			}
			return result;
		}

		[NativeThrows]
		public unsafe static void ParameterStructNestedBlittableDynamicArray(StructNestedBlittable[] param)
		{
			Span<StructNestedBlittable> span = new Span<StructNestedBlittable>(param);
			fixed (StructNestedBlittable* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BlittableStructTests.ParameterStructNestedBlittableDynamicArray_Injected(ref managedSpanWrapper);
			}
		}

		public static StructNestedBlittable[] ReturnStructNestedBlittableDynamicArray()
		{
			StructNestedBlittable[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableStructTests.ReturnStructNestedBlittableDynamicArray_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				StructNestedBlittable[] array;
				blittableArrayWrapper.Unmarshal<StructNestedBlittable>(ref array);
				result = array;
			}
			return result;
		}

		[NativeThrows]
		public static void ParameterStructFixedBuffer(StructFixedBuffer param)
		{
			BlittableStructTests.ParameterStructFixedBuffer_Injected(ref param);
		}

		public static StructFixedBuffer ReturnStructFixedBuffer()
		{
			StructFixedBuffer result;
			BlittableStructTests.ReturnStructFixedBuffer_Injected(out result);
			return result;
		}

		public static StructInt structIntProperty
		{
			get
			{
				StructInt result;
				BlittableStructTests.get_structIntProperty_Injected(out result);
				return result;
			}
			set
			{
				BlittableStructTests.set_structIntProperty_Injected(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructInt_Injected([In] ref StructInt param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructInt2_Injected([In] ref StructInt2 param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructInt_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterNestedBlittableStruct_Injected([In] ref StructNestedBlittable s);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnNestedBlittableStruct_Injected(out StructNestedBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructIntDynamicArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructIntDynamicArray_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructNestedBlittableDynamicArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructNestedBlittableDynamicArray_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructFixedBuffer_Injected([In] ref StructFixedBuffer param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructFixedBuffer_Injected(out StructFixedBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_structIntProperty_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_structIntProperty_Injected([In] ref StructInt value);
	}
}
