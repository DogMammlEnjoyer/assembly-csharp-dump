using System;

namespace System.ComponentModel.Composition
{
	/// <summary>Specifies that a custom attribute's properties provide metadata for exports applied to the same type, property, field, or method.</summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class MetadataAttributeAttribute : Attribute
	{
	}
}
