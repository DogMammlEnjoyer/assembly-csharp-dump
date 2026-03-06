using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/SystemTypeMarshallingTests.h")]
	internal static class SystemTypeMarshallingTests
	{
		public static string CanMarshallSystemTypeArgumentToScriptingClassPtr(Type param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				SystemTypeMarshallingTests.CanMarshallSystemTypeArgumentToScriptingClassPtr_Injected(param, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string CanMarshallSystemTypeStructField(StructSystemType param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				SystemTypeMarshallingTests.CanMarshallSystemTypeStructField_Injected(ref param, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string[] CanMarshallSystemTypeArrayStructField(StructSystemTypeArray param)
		{
			return SystemTypeMarshallingTests.CanMarshallSystemTypeArrayStructField_Injected(ref param);
		}

		public static StructSystemType CanUnmarshallSystemTypeStructField()
		{
			StructSystemType result;
			SystemTypeMarshallingTests.CanUnmarshallSystemTypeStructField_Injected(out result);
			return result;
		}

		public static StructSystemTypeArray CanUnmarshallSystemTypeArrayStructField()
		{
			StructSystemTypeArray result;
			SystemTypeMarshallingTests.CanUnmarshallSystemTypeArrayStructField_Injected(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToDynamicArrayOfScriptingSystemTypeObjectPtr(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToDynamicArrayOfUnityType(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanUnmarshallArrayOfSystemTypeArgumentToDynamicArrayOfScriptingClassPtr(Type[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallScriptingSystemTypeObjectPtrToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallUnityTypeToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type CanUnmarshallScriptingClassPtrToSystemType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallScriptingArrayPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfScriptingSystemTypeObjectPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfUnityTypeToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Type[] CanUnmarshallArrayOfScriptingClassPtrToSystemTypeArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemTypeArgumentToScriptingClassPtr_Injected(Type param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemTypeStructField_Injected([In] ref StructSystemType param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] CanMarshallSystemTypeArrayStructField_Injected([In] ref StructSystemTypeArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemTypeStructField_Injected(out StructSystemType ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemTypeArrayStructField_Injected(out StructSystemTypeArray ret);
	}
}
