using System;

namespace System.Diagnostics.Eventing.Reader
{
	/// <summary>Defines the standard keywords that are attached to events by the event provider. For more information about keywords, see <see cref="T:System.Diagnostics.Eventing.Reader.EventKeyword" />.</summary>
	[Flags]
	public enum StandardEventKeywords : long
	{
		/// <summary>Attached to all failed security audit events. This keyword should only be used for events in the Security log.</summary>
		AuditFailure = 4503599627370496L,
		/// <summary>Attached to all successful security audit events. This keyword should only be used for events in the Security log.</summary>
		AuditSuccess = 9007199254740992L,
		/// <summary>Attached to transfer events where the related Activity ID (Correlation ID) is a computed value and is not guaranteed to be unique (not a real GUID).</summary>
		[Obsolete("Incorrect value: use CorrelationHint2 instead", false)]
		CorrelationHint = 4503599627370496L,
		/// <summary>Attached to transfer events where the related Activity ID (Correlation ID) is a computed value and is not guaranteed to be unique (not a real GUID).</summary>
		CorrelationHint2 = 18014398509481984L,
		/// <summary>Attached to events which are raised using the RaiseEvent function.</summary>
		EventLogClassic = 36028797018963968L,
		/// <summary>This value indicates that no filtering on keyword is performed when the event is published.</summary>
		None = 0L,
		/// <summary>Attached to all response time events. </summary>
		ResponseTime = 281474976710656L,
		/// <summary>Attached to all Service Quality Mechanism (SQM) events.</summary>
		Sqm = 2251799813685248L,
		/// <summary>Attached to all Windows Diagnostic Infrastructure (WDI) context events.</summary>
		WdiContext = 562949953421312L,
		/// <summary>Attached to all Windows Diagnostic Infrastructure (WDI) diagnostic events.</summary>
		WdiDiagnostic = 1125899906842624L
	}
}
