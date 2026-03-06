using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Specifies the <see cref="T:System.Diagnostics.DebuggerHiddenAttribute" />. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	[Serializable]
	public sealed class DebuggerHiddenAttribute : Attribute
	{
	}
}
