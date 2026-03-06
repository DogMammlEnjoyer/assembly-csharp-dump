using System;

namespace System
{
	/// <summary>Indicates that an enumeration can be treated as a bit field; that is, a set of flags.</summary>
	[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
	[Serializable]
	public class FlagsAttribute : Attribute
	{
	}
}
