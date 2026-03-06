using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	/// <summary>When applied to an array parameter in a Windows Runtime component, specifies that the contents of an array that is passed to that parameter are used only for output. The caller does not guarantee that the contents are initialized, and the called method should not read the contents.</summary>
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
	public sealed class WriteOnlyArrayAttribute : Attribute
	{
	}
}
