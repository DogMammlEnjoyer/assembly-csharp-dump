using System;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	internal static class Logger
	{
		public static void Log(string message, LogLevel logLevel)
		{
			switch (logLevel)
			{
			case LogLevel.Verbose:
				Logger.LogVerbose(message);
				return;
			case LogLevel.Info:
				Logger.LogInfo(message);
				return;
			case LogLevel.Warning:
				Logger.LogWarning(message);
				return;
			case LogLevel.Error:
				Logger.LogError(message);
				return;
			case LogLevel.SharedSpatialAnchorsError:
				Logger.LogSharedSpatialAnchorsError(message);
				return;
			default:
				throw new ArgumentOutOfRangeException("logLevel", logLevel, string.Format("colocationLogLevel is unknown: {0}", logLevel));
			}
		}

		private static void LogVerbose(string message)
		{
			if (Logger._isVerboseLogVisible)
			{
				Debug.Log(Logger.GetPrefixMessage(LogLevel.Verbose) + message);
			}
		}

		private static void LogInfo(string message)
		{
			if (Logger._isInfoLogVisible)
			{
				Debug.Log(Logger.GetPrefixMessage(LogLevel.Info) + message);
			}
		}

		private static void LogWarning(string message)
		{
			if (Logger._isWarningLogVisible)
			{
				Debug.LogWarning(Logger.GetPrefixMessage(LogLevel.Warning) + message);
			}
		}

		private static void LogError(string message)
		{
			if (Logger._isErrorLogVisible)
			{
				Debug.LogError(Logger.GetPrefixMessage(LogLevel.Error) + message);
			}
		}

		private static void LogSharedSpatialAnchorsError(string message)
		{
			if (Logger._isSharedSpatialAnchorsErrorVisible)
			{
				Debug.LogError(Logger.GetPrefixMessage(LogLevel.SharedSpatialAnchorsError) + message);
			}
		}

		private static string GetPrefixMessage(LogLevel logLevel)
		{
			return string.Format("[{0}] ", logLevel);
		}

		public static void SetLogLevelVisibility(LogLevel logLevel, bool value)
		{
			switch (logLevel)
			{
			case LogLevel.Verbose:
				Logger._isVerboseLogVisible = value;
				return;
			case LogLevel.Info:
				Logger._isInfoLogVisible = value;
				return;
			case LogLevel.Warning:
				Logger._isWarningLogVisible = value;
				return;
			case LogLevel.Error:
				Logger._isErrorLogVisible = value;
				return;
			case LogLevel.SharedSpatialAnchorsError:
				Logger._isSharedSpatialAnchorsErrorVisible = value;
				return;
			default:
				throw new ArgumentOutOfRangeException("logLevel", logLevel, string.Format("colocationLogLevel is unknown: {0}", logLevel));
			}
		}

		public static void SetAllLogsVisibility(bool value)
		{
			Logger._isVerboseLogVisible = value;
			Logger._isInfoLogVisible = value;
			Logger._isWarningLogVisible = value;
			Logger._isErrorLogVisible = value;
			Logger._isSharedSpatialAnchorsErrorVisible = value;
		}

		private static bool _isVerboseLogVisible;

		private static bool _isInfoLogVisible;

		private static bool _isWarningLogVisible;

		private static bool _isErrorLogVisible;

		private static bool _isSharedSpatialAnchorsErrorVisible;
	}
}
