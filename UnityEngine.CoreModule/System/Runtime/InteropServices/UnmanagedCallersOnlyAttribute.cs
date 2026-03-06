using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	internal sealed class UnmanagedCallersOnlyAttribute : Attribute
	{
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Type[] CallConvs;

		[Nullable(2)]
		public string EntryPoint;
	}
}
