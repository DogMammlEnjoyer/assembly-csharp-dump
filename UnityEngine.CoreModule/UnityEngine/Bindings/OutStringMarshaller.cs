using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal ref struct OutStringMarshaller
	{
		public unsafe static string GetStringAndDispose(ManagedSpanWrapper managedSpan)
		{
			bool flag = managedSpan.length == 0;
			string result;
			if (flag)
			{
				result = ((managedSpan.begin == null) ? null : string.Empty);
			}
			else
			{
				string text = new string((char*)managedSpan.begin, 0, managedSpan.length);
				BindingsAllocator.Free(managedSpan.begin);
				result = text;
			}
			return result;
		}

		public static void UpdateStringAndDispose(ManagedSpanWrapper inSpanWrapper, ManagedSpanWrapper outSpanWrapper, ref string outString)
		{
			bool flag = inSpanWrapper.begin != outSpanWrapper.begin;
			if (flag)
			{
				outString = OutStringMarshaller.GetStringAndDispose(outSpanWrapper);
			}
		}
	}
}
