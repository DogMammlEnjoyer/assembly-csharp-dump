using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Provides the ability to collect statistics for very frequent events through the  <see cref="T:System.Diagnostics.Tracing.EventSource" /> class.</summary>
	public class EventCounter : DiagnosticCounter
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Tracing.EventCounter" /> class.</summary>
		/// <param name="name">The event counter name.</param>
		/// <param name="eventSource">The event source.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="eventSource" /> is <see langword="null" />.</exception>
		public EventCounter(string name, EventSource eventSource) : base(name, eventSource)
		{
		}

		/// <summary>Writes the metric if performance counters are on.</summary>
		/// <param name="value">The value to be written.</param>
		public void WriteMetric(float value)
		{
		}

		public void WriteMetric(double value)
		{
		}
	}
}
