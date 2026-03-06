using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Meta.WitAi;

namespace Meta.Voice.Logging
{
	internal class VLogger : IVLogger, ICoreLogger
	{
		public CorrelationID CorrelationID
		{
			get
			{
				if (this._correlationID.IsAssigned)
				{
					return this._correlationID;
				}
				if (!VLogger.CorrelationIDThreadLocal.IsValueCreated)
				{
					VLogger.CorrelationIDThreadLocal.Value = Guid.NewGuid().ToString();
				}
				this._correlationID = (CorrelationID)VLogger.CorrelationIDThreadLocal.Value;
				return this._correlationID;
			}
			set
			{
				this._correlationID = value;
				VLogger.CorrelationIDThreadLocal.Value = this._correlationID;
			}
		}

		internal VLogger(string category, ILogSink logSink)
		{
			this._category = category;
			this._logSink = logSink;
		}

		public static void ClearBuffer()
		{
			VLogger.LogBuffer.Clear();
		}

		private void CorrelateIds(CorrelationID correlationId)
		{
			if (!this._correlationID.IsAssigned)
			{
				this.CorrelationID = correlationId;
			}
			if (this.CorrelationID != correlationId)
			{
				if (this._correlations.ContainsKey(correlationId))
				{
					return;
				}
				this.Correlate(correlationId, this.CorrelationID);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Verbose(string message, object p1, object p2 = null, object p3 = null, object p4 = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			LoggingContext context = new LoggingContext(memberName, sourceFilePath, sourceLineNumber);
			this.LogEntry(new LogEntry(this._category, VLoggerVerbosity.Verbose, this.CorrelationID, context, string.Empty, message, new object[]
			{
				p1,
				p2,
				p3,
				p4
			}));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Verbose(string message, params object[] parameters)
		{
			LoggingContext emptyContext = this._emptyContext;
			this.LogEntry(new LogEntry(this._category, VLoggerVerbosity.Verbose, this.CorrelationID, emptyContext, string.Empty, message, parameters));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Verbose(CorrelationID correlationId, string message, params object[] parameters)
		{
			LoggingContext emptyContext = this._emptyContext;
			this.CorrelateIds(correlationId);
			this.LogEntry(new LogEntry(this._category, VLoggerVerbosity.Verbose, correlationId, emptyContext, string.Empty, message, parameters));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Info(string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Info, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Info(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Info, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Info(string message, object p1, object p2 = null, object p3 = null, object p4 = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			LoggingContext context = new LoggingContext(memberName, sourceFilePath, sourceLineNumber);
			this.LogEntry(new LogEntry(this._category, VLoggerVerbosity.Info, this.CorrelationID, context, string.Empty, message, new object[]
			{
				p1,
				p2,
				p3,
				p4
			}));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Debug(string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Debug, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Debug(string message, object p1, object p2 = null, object p3 = null, object p4 = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			LoggingContext context = new LoggingContext(memberName, sourceFilePath, sourceLineNumber);
			this.LogEntry(new LogEntry(this._category, VLoggerVerbosity.Debug, this.CorrelationID, context, string.Empty, message, new object[]
			{
				p1,
				p2,
				p3,
				p4
			}));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Debug(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Debug, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Warning(string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Warning, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(CorrelationID correlationId, ErrorCode errorCode, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Error, errorCode, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(ErrorCode errorCode, string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Error, errorCode, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(CorrelationID correlationId, Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Error, errorCode, new object[]
			{
				exception,
				message,
				parameters
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Error, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Error, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(CorrelationID correlationId, Exception exception, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Error, KnownErrorCode.Unknown, new object[]
			{
				exception,
				message,
				parameters
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Error, errorCode, new object[]
			{
				exception,
				message,
				parameters
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Error(Exception exception, string message = "", params object[] parameters)
		{
			this.Log(this.CorrelationID, VLoggerVerbosity.Error, exception, KnownErrorCode.Unknown, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Warning(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			this.Log(correlationId, VLoggerVerbosity.Warning, message, parameters);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Correlate(CorrelationID newCorrelationId, CorrelationID rootCorrelationId)
		{
			if (!(this._correlations.Add(newCorrelationId, rootCorrelationId, true) | this._downStreamCorrelations.Add(rootCorrelationId, newCorrelationId, true)))
			{
				return;
			}
			this.Log(newCorrelationId, VLoggerVerbosity.Verbose, "Correlated: {0}", new object[]
			{
				newCorrelationId
			});
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			LoggingContext context = new LoggingContext(new StackTrace(true));
			this.LogEntry(new LogEntry(this._category, verbosity, correlationId, context, string.Empty, message, parameters));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			LoggingContext context = new LoggingContext(new StackTrace(true));
			this.LogEntry(new LogEntry(this._category, verbosity, correlationId, errorCode, exception, context, string.Empty, message, parameters));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, ErrorCode errorCode, string message, params object[] parameters)
		{
			LoggingContext context = new LoggingContext(new StackTrace(true));
			this.LogEntry(new LogEntry(this._category, verbosity, correlationId, errorCode, context, string.Empty, message, parameters));
		}

		private void LogEntry(LogEntry logEntry)
		{
			if (this.IsSuppressed(logEntry))
			{
				VLogger.LogBuffer.Add(logEntry.CorrelationID, logEntry, false);
				return;
			}
			if (this.IsFiltered(logEntry))
			{
				VLogger.LogBuffer.Add(logEntry.CorrelationID, logEntry, false);
				return;
			}
			this.Write(logEntry, false);
		}

		private bool IsFiltered(LogEntry logEntry)
		{
			return false;
		}

		private bool IsSuppressed(LogEntry logEntry)
		{
			return VLog.SuppressLogs && logEntry.Verbosity < VLoggerVerbosity.Error;
		}

		public ILogScope Scope(VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			return new LogScope(this, verbosity, this.CorrelationID, message, parameters);
		}

		public ILogScope Scope(CorrelationID correlationId, VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			return new LogScope(this, verbosity, correlationId, message, parameters);
		}

		public int Start(CorrelationID correlationId, VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			this.CorrelateIds(correlationId);
			LoggingContext context = new LoggingContext(new StackTrace(true));
			LogEntry logEntry = new LogEntry(this._category, verbosity, correlationId, context, "Started: ", message, parameters);
			VLogger.LogBuffer.Add(correlationId, logEntry, false);
			this._scopeEntries.Add(this._nextSequenceId, logEntry);
			if (!this.IsFiltered(logEntry))
			{
				this.Write(logEntry, false);
			}
			int nextSequenceId = this._nextSequenceId;
			this._nextSequenceId = nextSequenceId + 1;
			return nextSequenceId;
		}

		public int Start(VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			LoggingContext context = new LoggingContext(new StackTrace(true));
			LogEntry logEntry = new LogEntry(this._category, verbosity, this.CorrelationID, context, "Started: ", message, parameters);
			VLogger.LogBuffer.Add(this.CorrelationID, logEntry, false);
			this._scopeEntries.Add(this._nextSequenceId, logEntry);
			if (!this.IsFiltered(logEntry))
			{
				this.Write(logEntry, false);
			}
			int nextSequenceId = this._nextSequenceId;
			this._nextSequenceId = nextSequenceId + 1;
			return nextSequenceId;
		}

		public void End(int sequenceId)
		{
			if (!this._scopeEntries.ContainsKey(sequenceId))
			{
				this.Error(KnownErrorCode.Logging, "Attempted to end a scope that was not started. Scope ID: {0}", new object[]
				{
					sequenceId
				});
				return;
			}
			LogEntry logEntry = this._scopeEntries[sequenceId];
			if (!this.IsFiltered(logEntry))
			{
				logEntry.Prefix = "Finished: ";
				this.Write(logEntry, false);
			}
			this._scopeEntries.Remove(sequenceId);
		}

		public void Flush(CorrelationID correlationID)
		{
			List<LogEntry> list = this.ExtractRelatedEntries(correlationID);
			list.Sort();
			foreach (LogEntry logEntry in list)
			{
				this.Write(logEntry, true);
			}
		}

		private List<LogEntry> ExtractRelatedEntries(CorrelationID correlationID)
		{
			List<LogEntry> list = new List<LogEntry>();
			CorrelationID key = correlationID;
			list.AddRange(VLogger.LogBuffer.Extract(key));
			while (this._correlations.ContainsKey(key))
			{
				if (this._correlations[key].Count > 1)
				{
					this.Warning(correlationID, new object[]
					{
						KnownErrorCode.Logging,
						"Correlation ID {0} had multiple parent IDs. Found: {1} IDs.",
						correlationID,
						this._correlations[key].Count
					});
				}
				key = this._correlations[key].First<CorrelationID>();
				list.AddRange(VLogger.LogBuffer.Extract(key));
			}
			this.ExtractDownstreamRelatedEntries(correlationID, ref list);
			return list;
		}

		private void ExtractDownstreamRelatedEntries(CorrelationID correlationID, ref List<LogEntry> entries)
		{
			if (!this._downStreamCorrelations.ContainsKey(correlationID))
			{
				return;
			}
			foreach (CorrelationID correlationID2 in this._downStreamCorrelations[correlationID])
			{
				entries.AddRange(VLogger.LogBuffer.Extract(correlationID2));
				this.ExtractDownstreamRelatedEntries(correlationID2, ref entries);
			}
		}

		public void Flush()
		{
			foreach (LogEntry logEntry in VLogger.LogBuffer.ExtractAll())
			{
				this.Write(logEntry, true);
			}
		}

		public IEnumerable<LogEntry> ExtractAllEntries()
		{
			return VLogger.LogBuffer.ExtractAll();
		}

		private void Write(LogEntry logEntry, bool force = false)
		{
			if (logEntry.Verbosity == VLoggerVerbosity.Error)
			{
				this.Flush(logEntry.CorrelationID);
			}
			if (!force & this.IsSuppressed(logEntry))
			{
				return;
			}
			this._logSink.WriteEntry(logEntry);
		}

		internal string GetDependenciesStructure(CorrelationID? correlationID = null, int depth = 0)
		{
			CorrelationID correlationID2 = correlationID ?? this.CorrelationID;
			string text = new string(' ', depth * 2) + correlationID2;
			if (this._downStreamCorrelations.ContainsKey(correlationID2))
			{
				foreach (CorrelationID value in this._downStreamCorrelations[correlationID2])
				{
					text = text + "\n" + this.GetDependenciesStructure(new CorrelationID?(value), depth + 1);
				}
			}
			return text;
		}

		private LoggingContext _emptyContext = new LoggingContext(null);

		private int _nextSequenceId = 1;

		private readonly Dictionary<int, LogEntry> _scopeEntries = new Dictionary<int, LogEntry>();

		private static readonly ThreadLocal<string> CorrelationIDThreadLocal = new ThreadLocal<string>();

		private static readonly RingDictionaryBuffer<CorrelationID, LogEntry> LogBuffer = new RingDictionaryBuffer<CorrelationID, LogEntry>(1000);

		private readonly RingDictionaryBuffer<CorrelationID, CorrelationID> _correlations = new RingDictionaryBuffer<CorrelationID, CorrelationID>(100);

		private readonly RingDictionaryBuffer<CorrelationID, CorrelationID> _downStreamCorrelations = new RingDictionaryBuffer<CorrelationID, CorrelationID>(100);

		private readonly ILogSink _logSink;

		private readonly string _category;

		private CorrelationID _correlationID;
	}
}
