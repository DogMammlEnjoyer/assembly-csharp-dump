using System;

namespace System.Runtime.CompilerServices
{
	/// <summary>Indicates that a structure is byref-like.</summary>
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class IsByRefLikeAttribute : Attribute
	{
	}
}
