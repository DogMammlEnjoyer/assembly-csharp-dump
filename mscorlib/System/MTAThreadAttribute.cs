using System;

namespace System
{
	/// <summary>Indicates that the COM threading model for an application is multithreaded apartment (MTA).</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MTAThreadAttribute : Attribute
	{
	}
}
