using System;

namespace System.Security
{
	/// <summary>Specifies that an assembly cannot cause an elevation of privilege.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class SecurityTransparentAttribute : Attribute
	{
	}
}
