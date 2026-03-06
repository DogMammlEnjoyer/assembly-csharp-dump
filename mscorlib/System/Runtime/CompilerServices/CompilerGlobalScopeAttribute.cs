using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Indicates that a class should be treated as if it has global scope.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	[Serializable]
	public class CompilerGlobalScopeAttribute : Attribute
	{
	}
}
