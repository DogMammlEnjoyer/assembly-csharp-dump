using System;
using System.Runtime.CompilerServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	public static class NativeListUnsafeUtility
	{
		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int)
		})]
		public unsafe static T* GetUnsafePtr<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.m_ListData->Ptr;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int)
		})]
		public unsafe static T* GetUnsafeReadOnlyPtr<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
		{
			return list.m_ListData->Ptr;
		}

		[GenerateTestsForBurstCompatibility(GenericTypeArguments = new Type[]
		{
			typeof(int)
		})]
		public unsafe static void* GetInternalListDataPtrUnchecked<[IsUnmanaged] T>(ref NativeList<T> list) where T : struct, ValueType
		{
			return (void*)list.m_ListData;
		}
	}
}
