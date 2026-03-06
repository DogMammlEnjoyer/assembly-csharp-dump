using System;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace System.Diagnostics
{
	/// <summary>Provides trace event data specific to a thread and a process.</summary>
	public class TraceEventCache
	{
		internal Guid ActivityId
		{
			get
			{
				return Trace.CorrelationManager.ActivityId;
			}
		}

		/// <summary>Gets the call stack for the current thread.</summary>
		/// <returns>A string containing stack trace information. This value can be an empty string ("").</returns>
		public string Callstack
		{
			get
			{
				if (this.stackTrace == null)
				{
					this.stackTrace = Environment.StackTrace;
				}
				return this.stackTrace;
			}
		}

		/// <summary>Gets the correlation data, contained in a stack.</summary>
		/// <returns>A <see cref="T:System.Collections.Stack" /> containing correlation data.</returns>
		public Stack LogicalOperationStack
		{
			get
			{
				return Trace.CorrelationManager.LogicalOperationStack;
			}
		}

		/// <summary>Gets the date and time at which the event trace occurred.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> structure whose value is a date and time expressed in Coordinated Universal Time (UTC).</returns>
		public DateTime DateTime
		{
			get
			{
				if (this.dateTime == DateTime.MinValue)
				{
					this.dateTime = DateTime.UtcNow;
				}
				return this.dateTime;
			}
		}

		/// <summary>Gets the unique identifier of the current process.</summary>
		/// <returns>The system-generated unique identifier of the current process.</returns>
		public int ProcessId
		{
			get
			{
				return TraceEventCache.GetProcessId();
			}
		}

		/// <summary>Gets a unique identifier for the current managed thread.</summary>
		/// <returns>A string that represents a unique integer identifier for this managed thread.</returns>
		public string ThreadId
		{
			get
			{
				return TraceEventCache.GetThreadId().ToString(CultureInfo.InvariantCulture);
			}
		}

		/// <summary>Gets the current number of ticks in the timer mechanism.</summary>
		/// <returns>The tick counter value of the underlying timer mechanism.</returns>
		public long Timestamp
		{
			get
			{
				if (this.timeStamp == -1L)
				{
					this.timeStamp = Stopwatch.GetTimestamp();
				}
				return this.timeStamp;
			}
		}

		private static void InitProcessInfo()
		{
			if (TraceEventCache.processName == null)
			{
				Process currentProcess = Process.GetCurrentProcess();
				try
				{
					TraceEventCache.processId = currentProcess.Id;
					TraceEventCache.processName = currentProcess.ProcessName;
				}
				finally
				{
					currentProcess.Dispose();
				}
			}
		}

		internal static int GetProcessId()
		{
			TraceEventCache.InitProcessInfo();
			return TraceEventCache.processId;
		}

		internal static string GetProcessName()
		{
			TraceEventCache.InitProcessInfo();
			return TraceEventCache.processName;
		}

		internal static int GetThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		private static volatile int processId;

		private static volatile string processName;

		private long timeStamp = -1L;

		private DateTime dateTime = DateTime.MinValue;

		private string stackTrace;
	}
}
