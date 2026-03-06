using System;

namespace System
{
	/// <summary>Indicates that the value of a static field is unique for each thread.</summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[Serializable]
	public class ThreadStaticAttribute : Attribute
	{
	}
}
