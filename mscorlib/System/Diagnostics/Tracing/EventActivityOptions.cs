using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies the tracking of activity start and stop events.</summary>
	[Flags]
	public enum EventActivityOptions
	{
		/// <summary>Use the default behavior for start and stop tracking.</summary>
		None = 0,
		/// <summary>Turn off start and stop tracking.</summary>
		Disable = 2,
		/// <summary>Allow recursive activity starts. By default, an activity cannot be recursive. That is, a sequence of Start A, Start A, Stop A, Stop A is not allowed. Unintentional recursive activities can occur if the app executes and for some the stop is not reached before another start is called.</summary>
		Recursive = 4,
		/// <summary>Allow overlapping activities. By default, activity starts and stops must be property nested. That is, a sequence of Start A, Start B, Stop A, Stop B is not allowed will result in B stopping at the same time as A.</summary>
		Detachable = 8
	}
}
