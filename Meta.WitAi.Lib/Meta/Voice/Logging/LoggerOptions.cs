using System;

namespace Meta.Voice.Logging
{
	public class LoggerOptions
	{
		public LoggerOptions(VLoggerVerbosity minimumVerbosity, VLoggerVerbosity suppressionLevel, VLoggerVerbosity stackTraceLevel, bool colorLogs = false, bool linkToCallSite = false)
		{
			this.MinimumVerbosity = minimumVerbosity;
			this.ColorLogs = colorLogs;
			this.LinkToCallSite = linkToCallSite;
			this.SuppressionLevel = suppressionLevel;
			this.StackTraceLevel = stackTraceLevel;
		}

		public void CopyFrom(LoggerOptions other)
		{
			this.MinimumVerbosity = other.MinimumVerbosity;
			this.ColorLogs = other.ColorLogs;
			this.LinkToCallSite = other.LinkToCallSite;
			this.SuppressionLevel = other.SuppressionLevel;
			this.StackTraceLevel = other.StackTraceLevel;
		}

		public VLoggerVerbosity MinimumVerbosity;

		public VLoggerVerbosity SuppressionLevel;

		public VLoggerVerbosity StackTraceLevel;

		public bool ColorLogs;

		public bool LinkToCallSite;
	}
}
