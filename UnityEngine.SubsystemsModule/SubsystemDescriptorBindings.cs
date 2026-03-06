using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal static class SubsystemDescriptorBindings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(IntPtr descriptorPtr);

		public static string GetId(IntPtr descriptorPtr)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				SubsystemDescriptorBindings.GetId_Injected(descriptorPtr, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetId_Injected(IntPtr descriptorPtr, out ManagedSpanWrapper ret);
	}
}
