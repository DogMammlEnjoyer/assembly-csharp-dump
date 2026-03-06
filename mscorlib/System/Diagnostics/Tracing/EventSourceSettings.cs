using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies configuration options for an event source.</summary>
	[Flags]
	public enum EventSourceSettings
	{
		/// <summary>None of the special configuration options are enabled.</summary>
		Default = 0,
		/// <summary>The event source throws an exception when an error occurs.</summary>
		ThrowOnEventWriteErrors = 1,
		/// <summary>The ETW listener should use a manifest-based format when raising events. Setting this option is a directive to the ETW listener should use manifest-based format when raising events. This is the default option when defining a type derived from <see cref="T:System.Diagnostics.Tracing.EventSource" /> using one of the protected <see cref="T:System.Diagnostics.Tracing.EventSource" /> constructors.</summary>
		EtwManifestEventFormat = 4,
		/// <summary>The ETW listener should use self-describing event format. This is the default option when creating a new instance of the <see cref="T:System.Diagnostics.Tracing.EventSource" /> using one of the public <see cref="T:System.Diagnostics.Tracing.EventSource" /> constructors.</summary>
		EtwSelfDescribingEventFormat = 8
	}
}
