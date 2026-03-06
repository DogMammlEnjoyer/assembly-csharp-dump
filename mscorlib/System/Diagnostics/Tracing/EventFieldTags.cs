using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies the user-defined tag that is placed on fields of user-defined types that are passed as <see cref="T:System.Diagnostics.Tracing.EventSource" /> payloads through the <see cref="T:System.Diagnostics.Tracing.EventFieldAttribute" />.</summary>
	[Flags]
	public enum EventFieldTags
	{
		/// <summary>Specifies no tag and is equal to zero.</summary>
		None = 0
	}
}
