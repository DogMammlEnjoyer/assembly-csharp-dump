using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/SystemReflectionFieldInfoMarshallingTests.h")]
	[ExcludeFromDocs]
	internal static class SystemReflectionFieldInfoMarshallingTests
	{
		public static string CanMarshallFieldInfoArgumentToScriptingFieldPtr(FieldInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				SystemReflectionFieldInfoMarshallingTests.CanMarshallFieldInfoArgumentToScriptingFieldPtr_Injected(param, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string CanMarshallSystemReflectionFieldInfoStructField(StructSystemReflectionFieldInfo param)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				SystemReflectionFieldInfoMarshallingTests.CanMarshallSystemReflectionFieldInfoStructField_Injected(ref param, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string[] CanMarshallSystemReflectionFieldInfoArrayStructField(StructSystemReflectionFieldInfoArray param)
		{
			return SystemReflectionFieldInfoMarshallingTests.CanMarshallSystemReflectionFieldInfoArrayStructField_Injected(ref param);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfFieldInfoArgumentToDynamicArrayOfScriptingFieldInfoObjectPtr(FieldInfo[] param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] CanMarshallArrayOfFieldInfoArgumentToDynamicArrayOfScriptingFieldPtr(FieldInfo[] param);

		public static StructSystemReflectionFieldInfo CanUnmarshallSystemReflectionFieldInfoStructField()
		{
			StructSystemReflectionFieldInfo result;
			SystemReflectionFieldInfoMarshallingTests.CanUnmarshallSystemReflectionFieldInfoStructField_Injected(out result);
			return result;
		}

		public static StructSystemReflectionFieldInfoArray CanUnmarshallSystemReflectionFieldInfoArrayStructField()
		{
			StructSystemReflectionFieldInfoArray result;
			SystemReflectionFieldInfoMarshallingTests.CanUnmarshallSystemReflectionFieldInfoArrayStructField_Injected(out result);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo CanUnmarshallScriptingFieldInfoObjectPtrToFieldInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo CanUnmarshallScriptingFieldPtrToFieldInfo();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo[] CanUnmarshallScriptingArrayPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo[] CanUnmarshallArrayOfScriptingFieldInfoObjectPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FieldInfo[] CanUnmarshallArrayOfScriptingFieldPtrToFieldInfoArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallFieldInfoArgumentToScriptingFieldPtr_Injected(FieldInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanMarshallSystemReflectionFieldInfoStructField_Injected([In] ref StructSystemReflectionFieldInfo param, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] CanMarshallSystemReflectionFieldInfoArrayStructField_Injected([In] ref StructSystemReflectionFieldInfoArray param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionFieldInfoStructField_Injected(out StructSystemReflectionFieldInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CanUnmarshallSystemReflectionFieldInfoArrayStructField_Injected(out StructSystemReflectionFieldInfoArray ret);
	}
}
