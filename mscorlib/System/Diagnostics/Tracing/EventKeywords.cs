using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Defines the standard keywords that apply to events.</summary>
	[Flags]
	public enum EventKeywords : long
	{
		/// <summary>No filtering on keywords is performed when the event is published.</summary>
		None = 0L,
		/// <summary>All the bits are set to 1, representing every possible group of events.</summary>
		All = -1L,
		/// <summary>Attached to all Microsoft telemetry events.</summary>
		MicrosoftTelemetry = 562949953421312L,
		/// <summary>Attached to all Windows Diagnostics Infrastructure (WDI) context events.</summary>
		WdiContext = 562949953421312L,
		/// <summary>Attached to all Windows Diagnostics Infrastructure (WDI) diagnostic events.</summary>
		WdiDiagnostic = 1125899906842624L,
		/// <summary>Attached to all Service Quality Mechanism (SQM) events.</summary>
		Sqm = 2251799813685248L,
		/// <summary>Attached to all failed security audit events. Use this keyword only  for events in the security log.</summary>
		AuditFailure = 4503599627370496L,
		/// <summary>Attached to all successful security audit events. Use this keyword only for events in the security log.</summary>
		AuditSuccess = 9007199254740992L,
		/// <summary>Attached to transfer events where the related activity ID (correlation ID) is a computed value and is not guaranteed to be unique (that is, it is not a real GUID).</summary>
		CorrelationHint = 4503599627370496L,
		/// <summary>Attached to events that are raised by using the <see langword="RaiseEvent" /> function.</summary>
		EventLogClassic = 36028797018963968L
	}
}
