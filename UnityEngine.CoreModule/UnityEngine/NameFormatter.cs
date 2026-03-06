using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/NameFormatter/NameFormatter.h")]
	[VisibleToOtherModules]
	internal sealed class NameFormatter
	{
		[FreeFunction]
		public unsafe static string FormatVariableName(string name)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpan;
				NameFormatter.FormatVariableName_Injected(ref managedSpanWrapper, out managedSpan);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FormatVariableName_Injected(ref ManagedSpanWrapper name, out ManagedSpanWrapper ret);
	}
}
