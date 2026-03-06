using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEngine;

namespace Fusion
{
	internal sealed class UnityLogStream : LogStream
	{
		public UnityLogStream(FusionUnityLoggerBase logger, LogLevel logLevel, TraceChannels channel, LogFlags flags)
		{
			this._prefix = ((channel == (TraceChannels)0 || channel == TraceChannels.Global) ? "" : channel.ToString());
			this._logLevel = logLevel;
			this._logger = logger;
			this._flags = flags;
		}

		public override void Log(ILogSource source, string message)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext(message, this._prefix, source, this._flags);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			LogLevel logLevel = this._logLevel;
			if (logLevel == LogLevel.Warn)
			{
				Debug.LogWarning(item, item2);
				return;
			}
			if (logLevel == LogLevel.Error)
			{
				Debug.LogError(item, item2);
				return;
			}
			Debug.Log(item, item2);
		}

		public override void Log(string message)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext(message, this._prefix, null, this._flags);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			LogLevel logLevel = this._logLevel;
			if (logLevel == LogLevel.Warn)
			{
				Debug.LogWarning(item, item2);
				return;
			}
			if (logLevel == LogLevel.Error)
			{
				Debug.LogError(item, item2);
				return;
			}
			Debug.Log(item, item2);
		}

		public override void Log(ILogSource source, string message, Exception error)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext((message ?? error.GetType().FullName) + " <i>See next error log entry for details.</i>", null, source, (LogFlags)0);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			Debug.LogWarning(item, item2);
			if (Application.isEditor)
			{
				ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(error);
				Thread thread = new Thread(delegate()
				{
					edi.Throw();
				});
				thread.Start();
				thread.Join();
				return;
			}
			if (item2)
			{
				Debug.LogException(error, item2);
				return;
			}
			Debug.LogException(error);
		}

		public override void Log(string message, Exception error)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext((message ?? error.GetType().FullName) + " <i>See next error log entry for details.</i>", null, null, (LogFlags)0);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			Debug.LogWarning(item, item2);
			if (Application.isEditor)
			{
				ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(error);
				Thread thread = new Thread(delegate()
				{
					edi.Throw();
				});
				thread.Start();
				thread.Join();
				return;
			}
			if (item2)
			{
				Debug.LogException(error, item2);
				return;
			}
			Debug.LogException(error);
		}

		public override void Log(ILogSource source, Exception error)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext(error.GetType().FullName + " <i>See next error log entry for details.</i>", null, source, (LogFlags)0);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			Debug.LogWarning(item, item2);
			if (Application.isEditor)
			{
				ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(error);
				Thread thread = new Thread(delegate()
				{
					edi.Throw();
				});
				thread.Start();
				thread.Join();
				return;
			}
			if (item2)
			{
				Debug.LogException(error, item2);
				return;
			}
			Debug.LogException(error);
		}

		public override void Log(Exception error)
		{
			FusionUnityLoggerBase logger = this._logger;
			FusionUnityLoggerBase.LogContext logContext = new FusionUnityLoggerBase.LogContext(error.GetType().FullName + " <i>See next error log entry for details.</i>", null, null, (LogFlags)0);
			ValueTuple<string, Object> valueTuple = logger.CreateMessage(logContext);
			string item = valueTuple.Item1;
			Object item2 = valueTuple.Item2;
			if (item == null)
			{
				return;
			}
			Debug.LogWarning(item, item2);
			if (Application.isEditor)
			{
				ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(error);
				Thread thread = new Thread(delegate()
				{
					edi.Throw();
				});
				thread.Start();
				thread.Join();
				return;
			}
			if (item2)
			{
				Debug.LogException(error, item2);
				return;
			}
			Debug.LogException(error);
		}

		private readonly FusionUnityLoggerBase _logger;

		private readonly LogLevel _logLevel;

		private readonly string _prefix;

		private readonly LogFlags _flags;
	}
}
