using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Marks a program element as read-only.</summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class IsReadOnlyAttribute : Attribute
	{
	}
}
