using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/PropertyName.h")]
	internal class PropertyNameUtils
	{
		[FreeFunction("PropertyNameFromStringICall", IsThreadSafe = true)]
		public unsafe static PropertyName PropertyNameFromString(string name)
		{
			PropertyName result;
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
				PropertyName propertyName;
				PropertyNameUtils.PropertyNameFromString_Injected(ref managedSpanWrapper, out propertyName);
			}
			finally
			{
				char* ptr = null;
				PropertyName propertyName;
				result = propertyName;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PropertyNameFromString_Injected(ref ManagedSpanWrapper name, out PropertyName ret);
	}
}
