using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies the event log channel for the event.</summary>
	public enum EventChannel : byte
	{
		/// <summary>No channel specified.</summary>
		None,
		/// <summary>The administrator log channel.</summary>
		Admin = 16,
		/// <summary>The operational channel.</summary>
		Operational,
		/// <summary>The analytic channel.</summary>
		Analytic,
		/// <summary>The debug channel.</summary>
		Debug
	}
}
