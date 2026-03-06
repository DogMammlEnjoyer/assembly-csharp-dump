using System;
using System.Runtime.CompilerServices;

namespace Modio
{
	[NullableContext(2)]
	[Nullable(0)]
	public class ModioLog
	{
		public static ModioLog Error { get; private set; }

		public static ModioLog Warning { get; private set; }

		public static ModioLog Message { get; private set; }

		public static ModioLog Verbose { get; private set; }

		static ModioLog()
		{
			ModioLog.ApplyLogLevel(LogLevel.Verbose);
			ModioServices.Bind<IModioLogHandler>().FromNew<ModioConsoleLog>(ModioServicePriority.Default, null);
			ModioServices.AddBindingChangedListener<IModioLogHandler>(new Action<IModioLogHandler>(ModioLog.UpdateLogHandler), true);
			string text;
			if (!ModioCommandLine.TryGet("loglevel", out text))
			{
				ModioServices.AddBindingChangedListener<ModioSettings>(new Action<ModioSettings>(ModioLog.GetLogLevelFromSettings), true);
				return;
			}
			LogLevel logLevel;
			if (Enum.TryParse<LogLevel>(text, true, out logLevel))
			{
				ModioLog.ApplyLogLevel(logLevel);
				return;
			}
			ModioLog error = ModioLog.Error;
			if (error == null)
			{
				return;
			}
			error.Log("Unrecognized log level: " + text);
		}

		[NullableContext(0)]
		private static void UpdateLogHandler(IModioLogHandler logHandler)
		{
			ModioLog._logHandler = logHandler;
		}

		[NullableContext(0)]
		private static void GetLogLevelFromSettings(ModioSettings settings)
		{
			ModioLog.ApplyLogLevel(settings.LogLevel);
		}

		private static void ApplyLogLevel(LogLevel logLevel)
		{
			ModioLog.Error = ((logLevel < LogLevel.Error) ? null : (ModioLog.Error ?? new ModioLog(LogLevel.Error)));
			ModioLog.Warning = ((logLevel < LogLevel.Warning) ? null : (ModioLog.Warning ?? new ModioLog(LogLevel.Warning)));
			ModioLog.Message = ((logLevel < LogLevel.Message) ? null : (ModioLog.Message ?? new ModioLog(LogLevel.Message)));
			ModioLog.Verbose = ((logLevel < LogLevel.Verbose) ? null : (ModioLog.Verbose ?? new ModioLog(LogLevel.Verbose)));
		}

		private ModioLog(LogLevel logLevel)
		{
			this._logLevel = logLevel;
		}

		[NullableContext(0)]
		public void Log(object message)
		{
			if (ModioLog._logHandler != null)
			{
				ModioLog._logHandler.LogHandler(this._logLevel, message);
				return;
			}
			Console.WriteLine(string.Format("{0}{1}: {2}", "[mod.io] ", this._logLevel, message));
		}

		public static ModioLog GetLogLevel(LogLevel logLevel)
		{
			ModioLog result;
			switch (logLevel)
			{
			case LogLevel.Error:
				result = ModioLog.Error;
				break;
			case LogLevel.Warning:
				result = ModioLog.Warning;
				break;
			case LogLevel.Message:
				result = ModioLog.Message;
				break;
			case LogLevel.Verbose:
				result = ModioLog.Verbose;
				break;
			default:
				result = ModioLog.Error;
				break;
			}
			return result;
		}

		[Nullable(0)]
		public const string LOG_PREFIX_DEFAULT = "[mod.io] ";

		[Nullable(0)]
		private static IModioLogHandler _logHandler;

		private readonly LogLevel _logLevel;

		[NullableContext(0)]
		public delegate void LogHandler(LogLevel logLevel, object message);
	}
}
