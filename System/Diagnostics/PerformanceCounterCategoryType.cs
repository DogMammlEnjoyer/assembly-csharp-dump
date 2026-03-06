using System;

namespace System.Diagnostics
{
	/// <summary>Indicates whether the performance counter category can have multiple instances.</summary>
	public enum PerformanceCounterCategoryType
	{
		/// <summary>The performance counter category can have only a single instance.</summary>
		SingleInstance,
		/// <summary>The performance counter category can have multiple instances.</summary>
		MultiInstance,
		/// <summary>The instance functionality for the performance counter category is unknown.</summary>
		Unknown = -1
	}
}
