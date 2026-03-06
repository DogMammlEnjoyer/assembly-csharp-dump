using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class ValueTypeArrayTests
	{
		[NativeThrows]
		public unsafe static void ParameterIntArrayReadOnly(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayWritable(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayWritable_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayEmpty(int[] param, int[] param2)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(param2);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					ValueTypeArrayTests.ParameterIntArrayEmpty_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public unsafe static void ParameterIntArrayNullExceptions([NotNull] int[] param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayNullExceptions_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntMultidimensionalArray(int[,] param)
		{
			int length;
			void* begin;
			if (param == null || (length = param.Length) == 0)
			{
				length = 0;
				begin = null;
			}
			else
			{
				begin = (void*)(&param[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, length);
			ValueTypeArrayTests.ParameterIntMultidimensionalArray_Injected(ref managedSpanWrapper);
		}

		public unsafe static void ParameterIntMultidimensionalArrayNullExceptions([NotNull] int[,] param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			int length;
			void* begin;
			if (param == null || (length = param.Length) == 0)
			{
				length = 0;
				begin = null;
			}
			else
			{
				begin = (void*)(&param[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, length);
			ValueTypeArrayTests.ParameterIntMultidimensionalArrayNullExceptions_Injected(ref managedSpanWrapper);
		}

		[NativeThrows]
		public unsafe static void ParameterCharArrayReadOnly(char[] param)
		{
			Span<char> span = new Span<char>(param);
			fixed (char* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterCharArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBlittableCornerCaseStructArrayReadOnly(BlittableCornerCases[] param)
		{
			Span<BlittableCornerCases> span = new Span<BlittableCornerCases>(param);
			fixed (BlittableCornerCases* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayOutAttr([Out] int[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (int[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterIntArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				int[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<int>(ref array);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCharArrayOutAttr([Out] char[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (char[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterCharArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				char[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<char>(ref array);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBlittableCornerCaseStructArrayOutAttr([Out] BlittableCornerCases[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (BlittableCornerCases[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableCornerCases[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<BlittableCornerCases>(ref array);
			}
		}

		public static int[] ParameterIntArrayReturn()
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturn_Injected(out blittableArrayWrapper);
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

		public static int[] ParameterIntArrayReturnEmpty()
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturnEmpty_Injected(out blittableArrayWrapper);
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

		public static int[] ParameterIntArrayReturnNull()
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturnNull_Injected(out blittableArrayWrapper);
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

		public static char[] ParameterCharArrayReturn()
		{
			char[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterCharArrayReturn_Injected(out blittableArrayWrapper);
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

		public static BlittableCornerCases[] ParameterBlittableCornerCaseStructArrayReturn()
		{
			BlittableCornerCases[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayReturn_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableCornerCases[] array;
				blittableArrayWrapper.Unmarshal<BlittableCornerCases>(ref array);
				result = array;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayWritable_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayEmpty_Injected(ref ManagedSpanWrapper param, ref ManagedSpanWrapper param2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayNullExceptions_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntMultidimensionalArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntMultidimensionalArrayNullExceptions_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturn_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturnEmpty_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturnNull_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayReturn_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayReturn_Injected(out BlittableArrayWrapper ret);
	}
}
