using System;

namespace System.ComponentModel.Composition
{
	/// <summary>Specifies which constructor should be used when creating a part.</summary>
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public class ImportingConstructorAttribute : Attribute
	{
	}
}
