using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	/// <summary>When applied to an array parameter in a Windows Runtime component, specifies that the contents of the array that is passed to that parameter are used only for input. The caller expects the array to be unchanged by the call.</summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
	public sealed class ReadOnlyArrayAttribute : Attribute
	{
	}
}
