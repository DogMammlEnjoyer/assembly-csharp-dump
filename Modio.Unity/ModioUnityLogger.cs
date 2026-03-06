using System;
using UnityEngine;

namespace Modio.Unity
{
	public class ModioUnityLogger : IModioLogHandler
	{
		public ModioUnityLogger() : this("[mod.io] ")
		{
		}

		public ModioUnityLogger(string prefix)
		{
			this._prefix = prefix;
		}

		public void LogHandler(LogLevel logLevel, object message)
		{
			string text;
			if (logLevel != LogLevel.Error)
			{
				if (logLevel != LogLevel.Warning)
				{
					text = string.Empty;
				}
				else
				{
					text = "[WARNING] ";
				}
			}
			else
			{
				text = "[ERROR] ";
			}
			string arg = text;
			ILogger unityLogger = Debug.unityLogger;
			LogType logType;
			switch (logLevel)
			{
			case LogLevel.None:
				logType = LogType.Log;
				break;
			case LogLevel.Error:
				logType = LogType.Error;
				break;
			case LogLevel.Warning:
				logType = LogType.Warning;
				break;
			case LogLevel.Message:
				logType = LogType.Log;
				break;
			case LogLevel.Verbose:
				logType = LogType.Log;
				break;
			default:
				logType = LogType.Log;
				break;
			}
			unityLogger.Log(logType, string.Format("{0}{1}{2}", this._prefix, arg, message));
		}

		private readonly string _prefix;
	}
}
