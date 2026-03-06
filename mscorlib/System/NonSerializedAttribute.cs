using System;

namespace System
{
	/// <summary>Indicates that a field of a serializable class should not be serialized. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class NonSerializedAttribute : Attribute
	{
	}
}
