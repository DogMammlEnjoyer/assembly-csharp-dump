using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[NativeClass("StructCoreString", "struct StructCoreString;")]
	[RequiredByNativeCode(GenerateProxy = true, Name = "StructCoreStringManaged", Optional = true)]
	[ExcludeFromDocs]
	internal struct StructCoreString
	{
		public string GetField()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				StructCoreString.GetField_Injected(ref this, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public unsafe void SetField(string value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				StructCoreString.SetField_Injected(ref this, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetField_Injected(ref StructCoreString _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetField_Injected(ref StructCoreString _unity_self, ref ManagedSpanWrapper value);

		public string field;
	}
}
