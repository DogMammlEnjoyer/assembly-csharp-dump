using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies a property should be ignored when writing an event type with the <see cref="M:System.Diagnostics.Tracing.EventSource.Write``1(System.String,System.Diagnostics.Tracing.EventSourceOptions@,``0@)" /> method.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class EventIgnoreAttribute : Attribute
	{
	}
}
