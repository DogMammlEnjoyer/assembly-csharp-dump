using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UnityEngine.ProBuilder
{
	internal static class Log
	{
		public static void PushLogLevel(LogLevel level)
		{
			Log.s_logStack.Push(Log.s_LogLevel);
			Log.s_LogLevel = level;
		}

		public static void PopLogLevel()
		{
			Log.s_LogLevel = Log.s_logStack.Pop();
		}

		public static void SetLogLevel(LogLevel level)
		{
			Log.s_LogLevel = level;
		}

		public static void SetOutput(LogOutput output)
		{
			Log.s_Output = output;
		}

		public static void SetLogFile(string path)
		{
			Log.s_LogFilePath = path;
		}

		[Conditional("DEBUG")]
		public static void Debug<T>(T value)
		{
		}

		[Conditional("DEBUG")]
		public static void Debug(string message)
		{
			Log.DoPrint(message, LogType.Log);
		}

		[Conditional("DEBUG")]
		public static void Debug(string format, params object[] values)
		{
		}

		public static void Info(string format, params object[] values)
		{
			Log.Info(string.Format(format, values));
		}

		public static void Info(string message)
		{
			if ((Log.s_LogLevel & LogLevel.Info) > LogLevel.None)
			{
				Log.DoPrint(message, LogType.Log);
			}
		}

		public static void Warning(string format, params object[] values)
		{
			Log.Warning(string.Format(format, values));
		}

		public static void Warning(string message)
		{
			if ((Log.s_LogLevel & LogLevel.Warning) > LogLevel.None)
			{
				Log.DoPrint(message, LogType.Warning);
			}
		}

		public static void Error(string format, params object[] values)
		{
			Log.Error(string.Format(format, values));
		}

		public static void Error(string message)
		{
			if ((Log.s_LogLevel & LogLevel.Error) > LogLevel.None)
			{
				Log.DoPrint(message, LogType.Error);
			}
		}

		[Conditional("CONSOLE_PRO_ENABLED")]
		internal static void Watch<T, K>(T key, K value)
		{
			UnityEngine.Debug.Log(string.Format("{0} : {1}\nCPAPI:{{\"cmd\":\"Watch\" \"name\":\"{0}\"}}", key.ToString(), value.ToString()));
		}

		private static void DoPrint(string message, LogType type)
		{
			if ((Log.s_Output & LogOutput.Console) > LogOutput.None)
			{
				Log.PrintToConsole(message, type);
			}
			if ((Log.s_Output & LogOutput.File) > LogOutput.None)
			{
				Log.PrintToFile(message, Log.s_LogFilePath);
			}
		}

		private static void PrintToFile(string message, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			string fullPath = Path.GetFullPath(path);
			if (string.IsNullOrEmpty(fullPath))
			{
				Log.PrintToConsole("m_LogFilePath bad: " + fullPath, LogType.Log);
				return;
			}
			if (!File.Exists(fullPath))
			{
				string directoryName = Path.GetDirectoryName(fullPath);
				if (string.IsNullOrEmpty(directoryName))
				{
					Log.PrintToConsole("m_LogFilePath bad: " + fullPath, LogType.Log);
					return;
				}
				Directory.CreateDirectory(directoryName);
				using (StreamWriter streamWriter = File.CreateText(fullPath))
				{
					streamWriter.WriteLine(message);
					return;
				}
			}
			using (StreamWriter streamWriter2 = File.AppendText(fullPath))
			{
				streamWriter2.WriteLine(message);
			}
		}

		public static void ClearLogFile()
		{
			if (File.Exists(Log.s_LogFilePath))
			{
				File.Delete(Log.s_LogFilePath);
			}
		}

		private static void PrintToConsole(string message, LogType type = LogType.Log)
		{
			if (type == LogType.Log)
			{
				UnityEngine.Debug.Log(message);
				return;
			}
			if (type == LogType.Warning)
			{
				UnityEngine.Debug.LogWarning(message);
				return;
			}
			if (type == LogType.Error)
			{
				UnityEngine.Debug.LogError(message);
				return;
			}
			if (type != LogType.Assert)
			{
				UnityEngine.Debug.Log(message);
			}
		}

		internal static void NotNull<T>(T obj, string message)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(message);
			}
		}

		public const string k_ProBuilderLogFileName = "ProBuilderLog.txt";

		private static Stack<LogLevel> s_logStack = new Stack<LogLevel>();

		private static LogLevel s_LogLevel = LogLevel.All;

		private static LogOutput s_Output = LogOutput.Console;

		private static string s_LogFilePath = "ProBuilderLog.txt";
	}
}
