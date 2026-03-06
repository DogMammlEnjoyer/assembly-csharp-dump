using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Meta.WitAi;

namespace Meta.Voice.Logging
{
	internal class LogSink : ILogSink
	{
		static LogSink()
		{
			ThreadUtility.CallOnMainThread<Thread>(() => LogSink.mainThread = Thread.CurrentThread).WrapErrors();
		}

		public IErrorMitigator ErrorMitigator
		{
			get
			{
				if (LogSink._errorMitigator == null)
				{
					LogSink._errorMitigator = new ErrorMitigator();
				}
				return LogSink._errorMitigator;
			}
			set
			{
				LogSink._errorMitigator = value;
			}
		}

		public ILogWriter LogWriter { get; set; }

		public LoggerOptions Options { get; set; }

		internal LogSink(ILogWriter logWriter, LoggerOptions options, IErrorMitigator errorMitigator = null)
		{
			this.LogWriter = logWriter;
			if (errorMitigator != null)
			{
				LogSink._errorMitigator = errorMitigator;
			}
			this.Options = options;
		}

		public void WriteEntry(LogEntry logEntry)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Concat(new string[]
			{
				"[",
				logEntry.TimeStamp.ToShortDateString(),
				" ",
				logEntry.TimeStamp.ToShortTimeString(),
				"] "
			}));
			int length = stringBuilder.Length;
			stringBuilder.Append("[VSDK] ");
			if (this.Options.ColorLogs)
			{
				this.WrapWithLogColor(stringBuilder, length, logEntry.Verbosity);
			}
			if (string.IsNullOrEmpty(logEntry.Message) && !string.IsNullOrEmpty(logEntry.Exception.Message))
			{
				logEntry.Message = logEntry.Exception.Message;
			}
			this.Annotate(stringBuilder, logEntry);
			string text;
			try
			{
				text = ((!string.IsNullOrEmpty(logEntry.Message) && logEntry.Parameters != null && logEntry.Parameters.Length != 0) ? string.Format(logEntry.Message, logEntry.Parameters) : logEntry.Message);
			}
			catch
			{
				text = logEntry.Message;
			}
			stringBuilder.Append(text);
			if (this._messagesCache.ContainsKey(text))
			{
				IEnumerable<CorrelationID> source = this._messagesCache.Extract(logEntry.Message);
				this._messagesCache.Add(logEntry.Message, logEntry.CorrelationID, false);
				if (source.First<CorrelationID>() == logEntry.CorrelationID)
				{
					stringBuilder.Append(string.Format(" [{0}]", logEntry.CorrelationID));
				}
				else
				{
					stringBuilder.Append(" [...]");
				}
			}
			else
			{
				stringBuilder.Append(string.Format(" [{0}]", logEntry.CorrelationID));
				this._messagesCache.Add(logEntry.Message, logEntry.CorrelationID, false);
			}
			if (logEntry.ErrorCode != null && logEntry.ErrorCode.Value != null && LogSink._errorMitigator != null)
			{
				stringBuilder.Append("\nMitigation: ");
				stringBuilder.Append(LogSink._errorMitigator.GetMitigation(logEntry.ErrorCode.Value));
			}
			if (logEntry.Verbosity >= this.Options.StackTraceLevel && logEntry.Context != null)
			{
				stringBuilder.Append("\n");
				logEntry.Context.AppendRelevantContext(stringBuilder, this.Options.ColorLogs);
			}
			string message = stringBuilder.ToString();
			Exception exception = logEntry.Exception;
			logEntry.Message = message;
			this.SendEntryToLogWriter(logEntry);
		}

		private void SendEntryToLogWriter(LogEntry logEntry)
		{
			switch (logEntry.Verbosity)
			{
			case VLoggerVerbosity.Debug:
				this.WriteDebug(logEntry.Prefix + logEntry.Message);
				return;
			case VLoggerVerbosity.Info:
				this.WriteInfo(logEntry.Prefix + logEntry.Message);
				return;
			case VLoggerVerbosity.Warning:
				this.WriteWarning(logEntry.Prefix + logEntry.Message);
				return;
			case VLoggerVerbosity.Error:
				this.WriteError(logEntry.Prefix + logEntry.Message);
				return;
			default:
				this.WriteVerbose(logEntry.Prefix + logEntry.Message);
				return;
			}
		}

		private void WrapWithLogColor(StringBuilder builder, int startIndex, VLoggerVerbosity logType)
		{
		}

		private string FormatStackTrace(string stackTrace)
		{
			if (stackTrace == null)
			{
				return string.Empty;
			}
			return new Regex("at (.+) in (.*):(\\d+)").Replace(stackTrace, new MatchEvaluator(this.<FormatStackTrace>g__Evaluator|20_0));
		}

		private void Annotate(StringBuilder sb, LogEntry logEntry)
		{
			if (!this.Options.LinkToCallSite)
			{
				if (!string.IsNullOrEmpty(logEntry.Category))
				{
					sb.Append("[" + logEntry.Category + "] ");
				}
				return;
			}
			ValueTuple<string, int> callSite = logEntry.Context.GetCallSite();
			string item = callSite.Item1;
			int item2 = callSite.Item2;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
			if (!string.IsNullOrEmpty(logEntry.Category) && !string.Equals(fileNameWithoutExtension, logEntry.Category))
			{
				sb.Append("[" + logEntry.Category + "] ");
			}
			if (string.IsNullOrEmpty(fileNameWithoutExtension))
			{
				return;
			}
			sb.Append("[" + fileNameWithoutExtension + ".cs");
			if (item2 > 0)
			{
				sb.Append(string.Format(":{0}", item2));
			}
			sb.Append("]");
			sb.Append(" ");
		}

		public void WriteVerbose(string message)
		{
			if (this.IsSafeToLog())
			{
				this.LogWriter.WriteVerbose(message);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.LogWriter.WriteVerbose(message);
			}).WrapErrors();
		}

		public void WriteDebug(string message)
		{
			if (this.IsSafeToLog())
			{
				this.LogWriter.WriteDebug(message);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.LogWriter.WriteDebug(message);
			}).WrapErrors();
		}

		public void WriteInfo(string message)
		{
			if (this.IsSafeToLog())
			{
				this.LogWriter.WriteInfo(message);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.LogWriter.WriteInfo(message);
			}).WrapErrors();
		}

		public void WriteWarning(string message)
		{
			if (this.IsSafeToLog())
			{
				this.LogWriter.WriteWarning(message);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.LogWriter.WriteWarning(message);
			}).WrapErrors();
		}

		public void WriteError(string message)
		{
			if (this.IsSafeToLog())
			{
				this.LogWriter.WriteError(message);
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.LogWriter.WriteError(message);
			}).WrapErrors();
		}

		[return: TupleElementNames(new string[]
		{
			"fileName",
			"lineNumber"
		})]
		private ValueTuple<string, int> GetCallSite(StackTrace stackTrace)
		{
			for (int i = 1; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (!(method.DeclaringType == null) && !LogSink.IsLoggingClass(method.DeclaringType) && !LogSink.IsSystemClass(method.DeclaringType))
				{
					string fileName = frame.GetFileName();
					string item = (fileName != null) ? fileName.Replace('\\', '/') : null;
					int fileLineNumber = frame.GetFileLineNumber();
					return new ValueTuple<string, int>(item, fileLineNumber);
				}
			}
			this.WriteError("Failed to get call site information.");
			return new ValueTuple<string, int>(string.Empty, 0);
		}

		private static bool IsLoggingClass(Type type)
		{
			return typeof(ICoreLogger).IsAssignableFrom(type) || typeof(ILogWriter).IsAssignableFrom(type) || type == typeof(VLog);
		}

		private static bool IsSystemClass(Type type)
		{
			string @namespace = type.Namespace;
			return @namespace != null && (@namespace.StartsWith("Unity") || @namespace.StartsWith("System") || @namespace.StartsWith("Microsoft"));
		}

		private bool IsSafeToLog()
		{
			return (Thread.CurrentThread.ThreadState & ThreadState.AbortRequested & ThreadState.Aborted) == ThreadState.Running || Thread.CurrentThread == LogSink.mainThread;
		}

		[CompilerGenerated]
		private string <FormatStackTrace>g__Evaluator|20_0(Match match)
		{
			string value = match.Groups[1].Value;
			string text = match.Groups[2].Value.Replace(this._workingDirectory, "");
			string value2 = match.Groups[3].Value;
			if (File.Exists(text))
			{
				string fileName = Path.GetFileName(text);
				return string.Concat(new string[]
				{
					"at ",
					value,
					" in <a href=\"",
					text,
					"\" line=\"",
					value2,
					"\">",
					fileName,
					":<b>",
					value2,
					"</b></a>"
				});
			}
			return match.Value;
		}

		private static Thread mainThread;

		private static IErrorMitigator _errorMitigator;

		private readonly string _workingDirectory = Directory.GetCurrentDirectory();

		private readonly RingDictionaryBuffer<string, CorrelationID> _messagesCache = new RingDictionaryBuffer<string, CorrelationID>(100);
	}
}
