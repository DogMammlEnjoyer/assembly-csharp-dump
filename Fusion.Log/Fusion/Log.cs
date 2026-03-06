using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class Log
	{
		public static void Dispose()
		{
			Log.Factory factory = new Log.Factory(default(LogSettings), null);
			Log.InitInternal(factory);
			Log.IsInitialized = false;
		}

		public static bool IsInitialized { get; private set; }

		public static LogSettings Settings { get; private set; }

		public static void Initialize(LogLevel logLevel, Log.CreateLogStreamDelegate streamFactory, TraceChannels traceChannels = (TraceChannels)0)
		{
			Log.Factory factory = new Log.Factory(new LogSettings(logLevel, traceChannels), streamFactory);
			Log.InitInternal(factory);
		}

		public static void Initialize(LogSettings settings, Log.CreateLogStreamDelegate streamFactory)
		{
			Log.Factory factory = new Log.Factory(settings, streamFactory);
			Log.InitInternal(factory);
		}

		public static void InitializeForConsole(LogSettings settings)
		{
			Log.Factory factory = new Log.Factory(settings, (LogLevel type, LogFlags flags, TraceChannels chanel) => new ConsoleLogStream(flags.HasFlag(LogFlags.Debug) ? ConsoleColor.DarkGray : ConsoleColor.Gray, flags.HasFlag(LogFlags.Debug) ? "[DEBUG] " : ""));
			Log.InitInternal(factory);
		}

		private static void DisposeAndNullify<T>(ref T obj) where T : class, IDisposable
		{
			T t = obj;
			if (t != null)
			{
				t.Dispose();
			}
			obj = default(T);
		}

		private static void InitPartial(in Log.Factory factory)
		{
			factory.Init(ref InternalLogStreams.LogTrace, TraceChannels.Global);
			factory.Init(ref InternalLogStreams.LogTraceStun, TraceChannels.Stun);
			factory.Init(ref InternalLogStreams.LogTraceObject, TraceChannels.Object);
			factory.Init(ref InternalLogStreams.LogTraceNetwork, TraceChannels.Network);
			factory.Init(ref InternalLogStreams.LogTracePrefab, TraceChannels.Prefab);
			factory.Init(ref InternalLogStreams.LogTraceSceneInfo, TraceChannels.SceneInfo);
			factory.Init(ref InternalLogStreams.LogTraceSceneManager, TraceChannels.SceneManager);
			factory.Init(ref InternalLogStreams.LogTraceSimulationMessage, TraceChannels.SimulationMessage);
			factory.Init(ref InternalLogStreams.LogTraceHostMigration, TraceChannels.HostMigration);
			factory.Init(ref InternalLogStreams.LogTraceEncryption, TraceChannels.Encryption);
			factory.Init(ref InternalLogStreams.LogTraceDummyTraffic, TraceChannels.DummyTraffic);
			factory.Init(ref InternalLogStreams.LogTraceRealtime, TraceChannels.Realtime);
			factory.Init(ref InternalLogStreams.LogTraceMemoryTrack, TraceChannels.MemoryTrack);
			factory.Init(ref InternalLogStreams.LogTraceSnapshots, TraceChannels.Snapshots);
			factory.Init(ref InternalLogStreams.LogTraceTime, TraceChannels.Time);
		}

		private static void InitInternal(in Log.Factory factory)
		{
			factory.Init(ref InternalLogStreams.LogDebug, TraceChannels.Global);
			factory.Init(ref InternalLogStreams.LogInfo, LogLevel.Info);
			factory.Init(ref InternalLogStreams.LogWarn, LogLevel.Warn);
			factory.Init(ref InternalLogStreams.LogError, LogLevel.Error);
			factory.Init(ref InternalLogStreams.LogException, LogLevel.Error);
			Log.InitPartial(factory);
			Log.Settings = factory.Settings;
			Log.IsInitialized = true;
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Debug(string message)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug == null)
			{
				return;
			}
			logDebug.InfoStream.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DebugWarn(string message)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug == null)
			{
				return;
			}
			logDebug.WarnStream.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DebugError(string message)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug == null)
			{
				return;
			}
			logDebug.ErrorStream.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Info(string message)
		{
			LogStream logInfo = InternalLogStreams.LogInfo;
			if (logInfo == null)
			{
				return;
			}
			logInfo.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Info(ILogSource logSource, string message)
		{
			LogStream logInfo = InternalLogStreams.LogInfo;
			if (logInfo == null)
			{
				return;
			}
			logInfo.Log(logSource, message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Warn(string message)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn == null)
			{
				return;
			}
			logWarn.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Warn(ILogSource logSource, string message)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn == null)
			{
				return;
			}
			logWarn.Log(logSource, message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[Conditional("FUSION_LOGLEVEL_ERROR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Error(string message)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError == null)
			{
				return;
			}
			logError.Log(message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[Conditional("FUSION_LOGLEVEL_ERROR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Error(ILogSource logSource, string message)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError == null)
			{
				return;
			}
			logError.Log(logSource, message);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[Conditional("FUSION_LOGLEVEL_ERROR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Exception(Exception ex)
		{
			LogStream logException = InternalLogStreams.LogException;
			if (logException == null)
			{
				return;
			}
			logException.Log(ex);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[Conditional("FUSION_LOGLEVEL_ERROR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Exception(string message, Exception ex)
		{
			LogStream logException = InternalLogStreams.LogException;
			if (logException == null)
			{
				return;
			}
			logException.Log(message, ex);
		}

		[Conditional("FUSION_LOGLEVEL_TRACE")]
		[Conditional("FUSION_LOGLEVEL_DEBUG")]
		[Conditional("FUSION_LOGLEVEL_INFO")]
		[Conditional("FUSION_LOGLEVEL_WARN")]
		[Conditional("FUSION_LOGLEVEL_ERROR")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Exception(ILogSource source, string message, Exception ex)
		{
			LogStream logException = InternalLogStreams.LogException;
			if (logException == null)
			{
				return;
			}
			logException.Log(source, message, ex);
		}

		[Obsolete("Use IsInitialized instead")]
		public static bool Initialized
		{
			get
			{
				return Log.IsInitialized;
			}
		}

		[Obsolete("Use InitializeForConsole instead")]
		public static void InitForConsole()
		{
			Log.InitForConsole(LogType.Info);
		}

		[Obsolete("Use InitializeForConsole instead")]
		public static void InitForConsole(LogType logType)
		{
			LogLevel logLevel;
			switch (logType)
			{
			case LogType.Error:
				logLevel = LogLevel.Error;
				break;
			case LogType.Warn:
				logLevel = LogLevel.Warn;
				break;
			case LogType.Info:
				logLevel = LogLevel.Info;
				break;
			case LogType.Debug:
				logLevel = LogLevel.Info;
				break;
			case LogType.Trace:
				logLevel = LogLevel.Info;
				break;
			default:
				throw new ArgumentOutOfRangeException("logType", logType, null);
			}
			LogLevel level = logLevel;
			if (logType != LogType.Debug)
			{
				if (logType != LogType.Trace)
				{
				}
			}
			Log.InitializeForConsole(new LogSettings
			{
				Level = level,
				TraceChannels = (TraceChannels)0
			});
		}

		[Obsolete("Use Initialize instead")]
		public static void Init(Action<string> info, Action<string> warn, Action<string> error, Action<Exception> exn)
		{
			LogLevel level = LogLevel.Error;
			if (warn != null)
			{
				level = LogLevel.Warn;
				if (info != null)
				{
					level = LogLevel.Info;
				}
			}
			Log.Initialize(new LogSettings
			{
				Level = level,
				TraceChannels = (TraceChannels)0
			}, delegate(LogLevel type, LogFlags flags, TraceChannels name)
			{
				switch (type)
				{
				case LogLevel.Info:
					return new Log.DelegateMessageStream(info, exn, null);
				case LogLevel.Warn:
					return new Log.DelegateMessageStream(warn, exn, null);
				case LogLevel.Error:
					return new Log.DelegateMessageStream(error, exn, null);
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
			});
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void Trace(object msg)
		{
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void TraceWarn(object msg)
		{
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void TraceError(object msg)
		{
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void Trace<T>(T source, object msg) where T : ILogSource
		{
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void TraceWarn<T>(T source, object msg) where T : ILogSource
		{
		}

		[Conditional("TRACE")]
		[Obsolete("Use string overloads instead")]
		public static void TraceError<T>(T source, object msg) where T : ILogSource
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void Debug(object msg)
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void DebugWarn(object msg)
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void DebugError(object msg)
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void Debug<T>(T source, object msg) where T : ILogSource
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void DebugWarn<T>(T source, object msg) where T : ILogSource
		{
		}

		[Conditional("DEBUG")]
		[Obsolete("Use string overloads instead")]
		public static void DebugError<T>(T source, object msg) where T : ILogSource
		{
		}

		[Obsolete("Use overloads with strings instead")]
		public static void Info(object msg)
		{
		}

		[Obsolete("Use overloads with strings instead")]
		internal static void Info(ILogSource source, object msg)
		{
		}

		[Obsolete("Use overloads with strings instead")]
		public static void Warn(object msg)
		{
		}

		[Obsolete("Use overloads with strings instead")]
		internal static void Warn(ILogSource source, object msg)
		{
		}

		[Obsolete("Use overloads with strings instead")]
		public static void Error(object msg)
		{
		}

		[Obsolete("Use overloads with strings instead")]
		internal static void Error(ILogSource source, object msg)
		{
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Trace(string msg)
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.InfoStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceWarn(string msg)
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.WarnStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceError(string msg)
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.ErrorStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Trace<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.InfoStream.Log(source, msg);
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceWarn<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.WarnStream.Log(source, msg);
		}

		[Conditional("FUSION_TRACE_GLOBAL")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceError<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTrace = InternalLogStreams.LogTrace;
			if (logTrace == null)
			{
				return;
			}
			logTrace.ErrorStream.Log(source, msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManager(string msg)
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.InfoStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManagerWarn(string msg)
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.WarnStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManagerError(string msg)
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.ErrorStream.Log(msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManager<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.InfoStream.Log(source, msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManagerWarn<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.WarnStream.Log(source, msg);
		}

		[Conditional("FUSION_TRACE_SCENEMANAGER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TraceSceneManagerError<T>(T source, string msg) where T : ILogSource
		{
			TraceLogStream logTraceSceneManager = InternalLogStreams.LogTraceSceneManager;
			if (logTraceSceneManager == null)
			{
				return;
			}
			logTraceSceneManager.ErrorStream.Log(source, msg);
		}

		public delegate LogStream CreateLogStreamDelegate(LogLevel level, LogFlags flags, TraceChannels channel);

		private readonly ref struct Factory
		{
			public Factory(LogSettings settings, Log.CreateLogStreamDelegate streamFactory)
			{
				this.StreamFactory = streamFactory;
				this.Settings = settings;
			}

			public void Init(ref DebugLogStream stream, TraceChannels channel)
			{
				Log.DisposeAndNullify<DebugLogStream>(ref stream);
				if (this.StreamFactory == null || LogLevel.Debug < this.Settings.Level)
				{
					stream = null;
					return;
				}
				LogStream logStream = this.StreamFactory(LogLevel.Info, LogFlags.Debug, channel);
				LogStream logStream2 = this.StreamFactory(LogLevel.Warn, LogFlags.Debug, channel);
				LogStream logStream3 = this.StreamFactory(LogLevel.Error, LogFlags.Debug, channel);
				if (logStream == null && logStream2 == null && logStream3 == null)
				{
					stream = null;
					return;
				}
				stream = new DebugLogStream(logStream, logStream2, logStream3);
			}

			public void Init(ref TraceLogStream stream, TraceChannels channel)
			{
				Log.DisposeAndNullify<TraceLogStream>(ref stream);
				if (this.StreamFactory == null || !this.Settings.TraceChannels.HasFlag(channel))
				{
					stream = null;
					return;
				}
				LogStream logStream = this.StreamFactory(LogLevel.Info, LogFlags.Trace, channel);
				LogStream logStream2 = this.StreamFactory(LogLevel.Warn, LogFlags.Trace, channel);
				LogStream logStream3 = this.StreamFactory(LogLevel.Error, LogFlags.Trace, channel);
				if (logStream == null && logStream2 == null && logStream3 == null)
				{
					stream = null;
					return;
				}
				stream = new TraceLogStream(logStream, logStream2, logStream3);
			}

			public void Init(ref LogStream stream, LogLevel logLevel)
			{
				Log.DisposeAndNullify<LogStream>(ref stream);
				if (this.StreamFactory != null && logLevel >= this.Settings.Level)
				{
					stream = this.StreamFactory(logLevel, (LogFlags)0, (TraceChannels)0);
					return;
				}
				stream = null;
			}

			public readonly Log.CreateLogStreamDelegate StreamFactory;

			public readonly LogSettings Settings;
		}

		private class DelegateMessageStream : LogStream
		{
			public DelegateMessageStream(Action<string> action, Action<Exception> exceptionAction, string prefix = null)
			{
				if (action == null)
				{
					throw new ArgumentNullException("action");
				}
				this._logAction = action;
				if (exceptionAction == null)
				{
					throw new ArgumentNullException("exceptionAction");
				}
				this._exceptionAction = exceptionAction;
				this._prefix = prefix;
			}

			public override void Log(ILogSource source, string message)
			{
				this.Log(message);
			}

			public override void Log(string message)
			{
				if (!string.IsNullOrEmpty(this._prefix))
				{
					this._logAction(this._prefix + " " + message);
					return;
				}
				this._logAction(message);
			}

			public override void Log(ILogSource source, string message, Exception error)
			{
				this._exceptionAction(error);
			}

			public override void Log(string message, Exception error)
			{
				this._exceptionAction(error);
			}

			public override void Log(Exception error)
			{
				this._exceptionAction(error);
			}

			private readonly Action<string> _logAction;

			private readonly Action<Exception> _exceptionAction;

			private readonly string _prefix;
		}
	}
}
