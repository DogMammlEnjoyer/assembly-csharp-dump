using System;

namespace System
{
	/// <summary>Indicates that the COM threading model for an application is single-threaded apartment (STA).</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class STAThreadAttribute : Attribute
	{
	}
}
