using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Liv.Lck.Core;
using Liv.Lck.Settings;
using UnityEngine;

namespace Liv.Lck
{
	internal static class LckLog
	{
		internal static void OnLckCoreInitialized()
		{
			object lockObject = LckLog._lockObject;
			lock (lockObject)
			{
				LckLog._isInitialized = true;
				while (LckLog._earlyLogs.Count > 0)
				{
					ValueTuple<Liv.Lck.Core.LogType, string, string, string, int> valueTuple = LckLog._earlyLogs.Dequeue();
					Liv.Lck.Core.LogType item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					string item3 = valueTuple.Item3;
					string item4 = valueTuple.Item4;
					int item5 = valueTuple.Item5;
					LckCore.Log(item, item2, item3, item4, item5);
				}
			}
		}

		public static void Log(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			if (LckLog.ShouldPrint(LogLevel.Info))
			{
				Debug.Log(message);
			}
			LckLog.SendToLckCore(Liv.Lck.Core.LogType.Info, message, memberName, LckLog.GetFileName(filePath), lineNumber);
		}

		public static void LogWarning(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			if (LckLog.ShouldPrint(LogLevel.Warning))
			{
				Debug.LogWarning(message);
			}
			LckLog.SendToLckCore(Liv.Lck.Core.LogType.Warning, message, memberName, LckLog.GetFileName(filePath), lineNumber);
		}

		public static void LogError(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			if (LckLog.ShouldPrint(LogLevel.Error))
			{
				Debug.LogError(message);
			}
			LckLog.SendToLckCore(Liv.Lck.Core.LogType.Error, message, memberName, LckLog.GetFileName(filePath), lineNumber);
		}

		[Conditional("LCK_TRACE")]
		public static void LogTrace(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
		{
			Debug.Log(message);
			LckLog.SendToLckCore(Liv.Lck.Core.LogType.Trace, message, memberName, LckLog.GetFileName(filePath), lineNumber);
		}

		private static void SendToLckCore(Liv.Lck.Core.LogType type, string message, string memberName, string filePath, int lineNumber)
		{
			object lockObject = LckLog._lockObject;
			lock (lockObject)
			{
				if (LckLog._isInitialized)
				{
					LckCore.Log(type, message, memberName, filePath, lineNumber);
				}
				else
				{
					LckLog._earlyLogs.Enqueue(new ValueTuple<Liv.Lck.Core.LogType, string, string, string, int>(type, message, memberName, filePath, lineNumber));
				}
			}
		}

		private static bool ShouldPrint(LogLevel level)
		{
			return LckSettings.Instance.BaseLogLevel >= level;
		}

		private static string GetFileName(string filePath)
		{
			int num = filePath.LastIndexOfAny(new char[]
			{
				'/',
				'\\'
			});
			if (num >= 0 && num < filePath.Length - 1)
			{
				return filePath.Substring(num + 1);
			}
			return filePath;
		}

		[TupleElementNames(new string[]
		{
			"type",
			"message",
			"memberName",
			"filePath",
			"lineNumber"
		})]
		private static readonly Queue<ValueTuple<Liv.Lck.Core.LogType, string, string, string, int>> _earlyLogs = new Queue<ValueTuple<Liv.Lck.Core.LogType, string, string, string, int>>();

		private static bool _isInitialized = false;

		private static readonly object _lockObject = new object();
	}
}
