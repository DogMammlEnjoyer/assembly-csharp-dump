using System;

namespace Meta.Voice.Logging
{
	public struct LogEntry : IComparable<LogEntry>
	{
		public readonly string Category { get; }

		public readonly DateTime TimeStamp { get; }

		public string Prefix { readonly get; set; }

		public string Message { readonly get; set; }

		public readonly object[] Parameters { get; }

		public readonly CorrelationID CorrelationID { get; }

		public readonly VLoggerVerbosity Verbosity { get; }

		public readonly Exception Exception { get; }

		public readonly ErrorCode? ErrorCode { get; }

		public readonly LoggingContext Context { get; }

		public LogEntry(string category, VLoggerVerbosity verbosity, CorrelationID correlationId, LoggingContext context, string prefix, string message, params object[] parameters)
		{
			this.Category = category;
			this.TimeStamp = DateTime.UtcNow;
			this.Prefix = prefix;
			this.Message = message;
			this.Parameters = parameters;
			this.Verbosity = verbosity;
			this.CorrelationID = correlationId;
			this.Exception = null;
			this.ErrorCode = new ErrorCode?((ErrorCode)null);
			this.Context = context;
		}

		public LogEntry(string category, VLoggerVerbosity verbosity, CorrelationID correlationId, ErrorCode errorCode, Exception exception, LoggingContext context, string prefix, string message, params object[] parameters)
		{
			this.Category = category;
			this.TimeStamp = DateTime.UtcNow;
			this.Prefix = prefix;
			this.Message = message;
			this.Parameters = parameters;
			this.Verbosity = verbosity;
			this.CorrelationID = correlationId;
			this.Exception = exception;
			this.ErrorCode = new ErrorCode?(errorCode);
			this.Context = context;
		}

		public LogEntry(string category, VLoggerVerbosity verbosity, CorrelationID correlationId, ErrorCode errorCode, LoggingContext context, string prefix, string message, params object[] parameters)
		{
			this.Category = category;
			this.TimeStamp = DateTime.UtcNow;
			this.Prefix = prefix;
			this.Message = message;
			this.Parameters = parameters;
			this.Verbosity = verbosity;
			this.CorrelationID = correlationId;
			this.Exception = null;
			this.ErrorCode = new ErrorCode?(errorCode);
			this.Context = context;
		}

		public override string ToString()
		{
			return string.Format(this.Message, this.Parameters) + string.Format(" [{0}]", this.CorrelationID);
		}

		public int CompareTo(LogEntry other)
		{
			return this.TimeStamp.CompareTo(other.TimeStamp);
		}
	}
}
