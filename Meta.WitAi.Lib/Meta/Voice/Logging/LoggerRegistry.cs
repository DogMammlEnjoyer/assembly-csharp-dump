using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Meta.Voice.Logging
{
	public sealed class LoggerRegistry : ILoggerRegistry
	{
		public ILogSink LogSink { get; set; }

		public IVLoggerFactory VLoggerFactory { get; set; } = new VLoggerFactory();

		public LoggerOptions Options { get; }

		public bool PoolLoggers { get; set; } = true;

		public VLoggerVerbosity LogStackTraceLevel
		{
			get
			{
				return this.Options.StackTraceLevel;
			}
			set
			{
				if (this.Options.StackTraceLevel == value)
				{
					return;
				}
				this.Options.StackTraceLevel = value;
			}
		}

		public VLoggerVerbosity LogSuppressionLevel
		{
			get
			{
				return this.Options.SuppressionLevel;
			}
			set
			{
				if (this.Options.SuppressionLevel == value)
				{
					return;
				}
				this.Options.SuppressionLevel = value;
			}
		}

		public VLoggerVerbosity EditorLogFilteringLevel
		{
			get
			{
				return this.Options.MinimumVerbosity;
			}
			set
			{
				if (this.Options.MinimumVerbosity == value)
				{
					return;
				}
				this.Options.MinimumVerbosity = value;
			}
		}

		public static ILoggerRegistry Instance { get; } = new LoggerRegistry();

		public IEnumerable<IVLogger> AllLoggers
		{
			get
			{
				return this._loggers.Values;
			}
		}

		internal LoggerRegistry()
		{
			this.Options = new LoggerOptions(VLoggerVerbosity.Warning, VLoggerVerbosity.Verbose, VLoggerVerbosity.Error, false, false);
			ILogWriter logWriter = new UnityLogWriter();
			this.LogSink = new LogSink(logWriter, this.Options, null);
		}

		public IVLogger GetLogger(LogCategory logCategory, ILogSink logSink = null)
		{
			return new LazyLogger(() => this.GetCoreLogger(logCategory, logSink));
		}

		public IVLogger GetLogger(string category, ILogSink logSink)
		{
			return new LazyLogger(() => this.GetCoreLogger(category, logSink));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IVLogger GetCoreLogger(LogCategory category, ILogSink logSink)
		{
			return this.GetCoreLogger(category.ToString(), logSink);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IVLogger GetCoreLogger(string category, ILogSink logSink)
		{
			if (logSink == null)
			{
				logSink = this.LogSink;
			}
			logSink.Options = this.Options;
			if (this.PoolLoggers)
			{
				if (!this._loggers.ContainsKey(category))
				{
					this._loggers.Add(category, this.VLoggerFactory.GetLogger(category, logSink));
				}
				return this._loggers[category];
			}
			return this.VLoggerFactory.GetLogger(category, logSink);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private IVLogger GetCoreLogger(ILogSink logSink = null, int frameDepth = 1)
		{
			if (logSink == null)
			{
				logSink = this.LogSink;
			}
			StackTrace stackTrace = new StackTrace();
			string category = LogCategory.Global.ToString();
			StackFrame[] frames = stackTrace.GetFrames();
			StackFrame stackFrame = (frames != null) ? frames.Skip(frameDepth).FirstOrDefault(new Func<StackFrame, bool>(this.IsNonLoggingFrame)) : null;
			Type type;
			if (stackFrame == null)
			{
				type = null;
			}
			else
			{
				MethodBase method = stackFrame.GetMethod();
				type = ((method != null) ? method.DeclaringType : null);
			}
			Type type2 = type;
			if (type2 == null)
			{
				return this.GetCoreLogger(category, logSink);
			}
			LogCategoryAttribute customAttribute = type2.GetCustomAttribute<LogCategoryAttribute>();
			if (customAttribute == null)
			{
				return this.GetCoreLogger(category, logSink);
			}
			category = customAttribute.CategoryName;
			return this.GetCoreLogger(category, logSink);
		}

		private bool IsNonLoggingFrame(StackFrame frame)
		{
			MethodBase methodBase = (frame != null) ? frame.GetMethod() : null;
			return !(methodBase == null) && !(methodBase.DeclaringType == null) && !typeof(LoggerRegistry).IsAssignableFrom(methodBase.DeclaringType) && !typeof(IVLogger).IsAssignableFrom(methodBase.DeclaringType) && (methodBase.DeclaringType.Namespace == null || !methodBase.DeclaringType.Namespace.StartsWith("System"));
		}

		private const string EDITOR_LOG_LEVEL_KEY = "VSDK_EDITOR_LOG_LEVEL";

		private const string EDITOR_LOG_SUPPRESSION_LEVEL_KEY = "VSDK_EDITOR_LOG_SUPPRESSION_LEVEL";

		private const string EDITOR_LOG_STACKTRACE_LEVEL_KEY = "VSDK_EDITOR_LOG_STACKTRACE_LEVEL";

		private readonly Dictionary<string, IVLogger> _loggers = new Dictionary<string, IVLogger>();
	}
}
