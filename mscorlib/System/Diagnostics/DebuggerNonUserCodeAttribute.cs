using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Identifies a type or member that is not part of the user code for an application.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	[Serializable]
	public sealed class DebuggerNonUserCodeAttribute : Attribute
	{
	}
}
