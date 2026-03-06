using System;
using System.Runtime.CompilerServices;

internal static class OVREnumerable
{
	public unsafe static int CopyTo<[IsUnmanaged] T>(this OVREnumerable<T> enumerable, T* memory) where T : struct, ValueType
	{
		int result = 0;
		foreach (T t in enumerable)
		{
			memory[(IntPtr)(result++) * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = t;
		}
		return result;
	}
}
