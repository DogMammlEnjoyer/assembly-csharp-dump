using System;
using System.Diagnostics;
using Liv.Lck.Settings;
using UnityEngine;

namespace Liv.Lck
{
	internal static class LckLog
	{
		public static void Log(string message)
		{
			if (LckLog.ShouldPrint(LogLevel.Info))
			{
				Debug.Log(message);
			}
		}

		public static void LogWarning(string message)
		{
			if (LckLog.ShouldPrint(LogLevel.Warning))
			{
				Debug.LogWarning(message);
			}
		}

		public static void LogError(string message)
		{
			if (LckLog.ShouldPrint(LogLevel.Error))
			{
				Debug.LogError(message);
			}
		}

		[Conditional("LCK_TRACE")]
		public static void LogTrace(string message)
		{
			Debug.Log(message);
		}

		private static bool ShouldPrint(LogLevel level)
		{
			return LckSettings.Instance.BaseLogLevel >= level;
		}
	}
}
