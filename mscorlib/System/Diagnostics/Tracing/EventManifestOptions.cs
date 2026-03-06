using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies how the ETW manifest for the event source is generated.</summary>
	[Flags]
	public enum EventManifestOptions
	{
		/// <summary>Generates a resources node under the localization folder for every satellite assembly provided.</summary>
		AllCultures = 2,
		/// <summary>Overrides the default behavior that the current <see cref="T:System.Diagnostics.Tracing.EventSource" /> must be the base class of the user-defined type passed to the write method. This enables the validation of .NET event sources.</summary>
		AllowEventSourceOverride = 8,
		/// <summary>No options are specified.</summary>
		None = 0,
		/// <summary>A manifest is generated only the event source must be registered on the host computer.</summary>
		OnlyIfNeededForRegistration = 4,
		/// <summary>Causes an exception to be raised if any inconsistencies occur when writing the manifest file.</summary>
		Strict = 1
	}
}
