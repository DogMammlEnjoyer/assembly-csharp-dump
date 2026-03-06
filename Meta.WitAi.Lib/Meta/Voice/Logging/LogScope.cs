using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Meta.Voice.TelemetryUtilities.PerformanceTracing;

namespace Meta.Voice.Logging
{
	public class LogScope : ILogScope, IDisposable, ICoreLogger
	{
		public LogScope(ICoreLogger logger, VLoggerVerbosity verbosity, CorrelationID correlationID, string message, object[] parameters)
		{
			this.CorrelationID = correlationID;
			this._logger = logger;
			this._sequenceId = this._logger.Start(correlationID, verbosity, message, parameters);
		}

		public CorrelationID CorrelationID { get; set; }

		public void Verbose(string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Verbose, message, parameters);
		}

		public void Verbose(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Verbose, message, parameters);
		}

		public void Verbose(string message, object p1 = null, object p2 = null, object p3 = null, object p4 = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this._logger.Verbose(message, p1, p2, p3, p4, memberName, sourceFilePath, sourceLineNumber);
		}

		public void Info(string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Info, message, parameters);
		}

		public void Info(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Info, message, parameters);
		}

		public void Info(string message, object p1 = null, object p2 = null, object p3 = null, object p4 = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			this._logger.Info(message, p1, p2, p3, p4, memberName, sourceFilePath, sourceLineNumber);
		}

		public void Debug(string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Debug, message, parameters);
		}

		public void Debug(string message, object p1 = null, object p2 = null, object p3 = null, object p4 = null, string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
		{
			this._logger.Debug(message, p1, p2, p3, p4, memberName, sourceFilePath, sourceLineNumber);
		}

		public void Debug(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Debug, message, parameters);
		}

		public void Warning(string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Warning, message, parameters);
		}

		public void Warning(CorrelationID correlationId, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Warning, message, parameters);
		}

		public void Error(ErrorCode errorCode, string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Error, message, new object[]
			{
				errorCode,
				parameters
			});
		}

		public void Error(CorrelationID correlationId, ErrorCode errorCode, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Error, message, new object[]
			{
				errorCode,
				parameters
			});
		}

		public void Error(CorrelationID correlationId, Exception exception, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Error, exception, KnownErrorCode.Unknown, message, parameters);
		}

		public void Error(Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Verbose, exception, errorCode, message, parameters);
		}

		public void Error(Exception exception, string message = "", params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Error, exception, KnownErrorCode.Unknown, "", Array.Empty<object>());
		}

		public void Error(CorrelationID correlationId, Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			this._logger.Log(correlationId, VLoggerVerbosity.Verbose, exception, errorCode, message, parameters);
		}

		public void Error(CorrelationID correlationId, string message, params object[] parameters)
		{
			this._logger.Log(correlationId, VLoggerVerbosity.Error, message, parameters);
		}

		public void Error(string message, params object[] parameters)
		{
			this._logger.Log(this.CorrelationID, VLoggerVerbosity.Error, message, parameters);
		}

		public int Start(CorrelationID correlationId, VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			this.Correlate(correlationId, this.CorrelationID);
			int num = this._logger.Start(correlationId, verbosity, message, parameters);
			this.StartProfiling(num, message);
			return num;
		}

		public int Start(VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			VsdkProfiler.BeginSample(message);
			int num = this._logger.Start(verbosity, message, parameters);
			this.StartProfiling(num, message);
			return num;
		}

		private void StartProfiling(int sequenceId, string message)
		{
			if (VsdkProfiler.profilingEnabled)
			{
				VsdkProfiler.BeginSample(message);
				this._activeSamples[sequenceId] = message;
			}
		}

		public void End(int sequenceId)
		{
			string sampleName;
			if (VsdkProfiler.profilingEnabled && this._activeSamples.TryRemove(sequenceId, out sampleName))
			{
				VsdkProfiler.EndSample(sampleName);
			}
			this._logger.End(sequenceId);
		}

		public void Correlate(CorrelationID newCorrelationId, CorrelationID rootCorrelationId)
		{
			this._logger.Correlate(newCorrelationId, rootCorrelationId);
		}

		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, string message, params object[] parameters)
		{
			this._logger.Log(correlationId, verbosity, message, parameters);
		}

		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, Exception exception, ErrorCode errorCode, string message, params object[] parameters)
		{
			this._logger.Log(correlationId, verbosity, exception, errorCode, message, parameters);
		}

		public void Log(CorrelationID correlationId, VLoggerVerbosity verbosity, ErrorCode errorCode, string message, params object[] parameters)
		{
			this._logger.Log(correlationId, verbosity, errorCode, message, parameters);
		}

		public void Dispose()
		{
			this._logger.End(this._sequenceId);
		}

		private readonly ICoreLogger _logger;

		private readonly int _sequenceId;

		private ConcurrentDictionary<int, string> _activeSamples = new ConcurrentDictionary<int, string>();
	}
}
