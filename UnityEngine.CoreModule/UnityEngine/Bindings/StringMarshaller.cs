using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal static class StringMarshaller
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool TryMarshalEmptyOrNullString(string s, ref ManagedSpanWrapper managedSpanWrapper)
		{
			bool flag = s == null;
			bool result;
			if (flag)
			{
				managedSpanWrapper = default(ManagedSpanWrapper);
				result = true;
			}
			else
			{
				bool flag2 = s.Length == 0;
				if (flag2)
				{
					managedSpanWrapper = new ManagedSpanWrapper((void*)((UIntPtr)1UL), 0);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}
}
