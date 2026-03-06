using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	internal class StringTests
	{
		public unsafe static void SetTestOutString(string testString)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(testString, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = testString.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.SetTestOutString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterICallString(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterICallString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterICallNullString(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterICallNullString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCoreString(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterCoreString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterConstCharPtr(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterConstCharPtr_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterConstCharPtrNull(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterConstCharPtrNull_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterConstCharPtrEmptyString(string param)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterConstCharPtrEmptyString_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterCoreStringVector(string[] param);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ParameterCoreStringDynamicArray(string[] param);

		[NativeThrows]
		public static void ParameterStructCoreString(StructCoreString param)
		{
			StringTests.ParameterStructCoreString_Injected(ref param);
		}

		[NativeThrows]
		public static void ParameterStructCoreStringVector(StructCoreStringVector param)
		{
			StringTests.ParameterStructCoreStringVector_Injected(ref param);
		}

		[NativeThrows]
		public static StructCoreString TestCoreStringViaProxy(StructCoreString param)
		{
			StructCoreString result;
			StringTests.TestCoreStringViaProxy_Injected(ref param, out result);
			return result;
		}

		public static string ReturnCoreString()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ReturnCoreString_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string ReturnCoreStringRef()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ReturnCoreStringRef_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string ReturnConstCharPtr()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ReturnConstCharPtr_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] ReturnCoreStringVector();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] ReturnCoreStringDynamicArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] ReturnNullStringDynamicArray();

		public static StructCoreString ReturnStructCoreString()
		{
			StructCoreString result;
			StringTests.ReturnStructCoreString_Injected(out result);
			return result;
		}

		[NativeConditional("FOO")]
		public static string FalseConditional()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.FalseConditional_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static StructCoreStringVector ReturnStructCoreStringVector()
		{
			StructCoreStringVector result;
			StringTests.ReturnStructCoreStringVector_Injected(out result);
			return result;
		}

		[NativeThrows]
		public static void ParameterOutString(out string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ParameterOutString_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
		}

		[NativeThrows]
		public static void ParameterOutStringInNull(out string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ParameterOutStringInNull_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
		}

		[NativeThrows]
		public static void ParameterOutStringNotSet(out string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				StringTests.ParameterOutStringNotSet_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterRefString(ref string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpan))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpan = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterRefString_Injected(ref managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterRefStringInNull(ref string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpan))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpan = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterRefStringInNull_Injected(ref managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
				char* ptr = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterRefStringNotSet(ref string param)
		{
			try
			{
				ManagedSpanWrapper managedSpan;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(param, ref managedSpan))
				{
					ReadOnlySpan<char> readOnlySpan = param.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpan = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StringTests.ParameterRefStringNotSet_Injected(ref managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				param = OutStringMarshaller.GetStringAndDispose(managedSpan);
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTestOutString_Injected(ref ManagedSpanWrapper testString);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterICallString_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterICallNullString_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCoreString_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterConstCharPtr_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterConstCharPtrNull_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterConstCharPtrEmptyString_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructCoreString_Injected([In] ref StructCoreString param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterStructCoreStringVector_Injected([In] ref StructCoreStringVector param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TestCoreStringViaProxy_Injected([In] ref StructCoreString param, out StructCoreString ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnCoreString_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnCoreStringRef_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnConstCharPtr_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructCoreString_Injected(out StructCoreString ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FalseConditional_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReturnStructCoreStringVector_Injected(out StructCoreStringVector ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterOutString_Injected(out ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterOutStringInNull_Injected(out ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterOutStringNotSet_Injected(out ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterRefString_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterRefStringInNull_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterRefStringNotSet_Injected(ref ManagedSpanWrapper param);
	}
}
