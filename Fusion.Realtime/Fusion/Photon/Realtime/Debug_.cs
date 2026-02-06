using System;
using System.Diagnostics;

namespace Fusion.Photon.Realtime
{
	internal static class Debug_
	{
		[Conditional("DEBUG")]
		public static void Log(string msg)
		{
			TraceLogStream logTraceRealtime = InternalLogStreams.LogTraceRealtime;
			if (logTraceRealtime != null)
			{
				logTraceRealtime.Log(msg);
			}
		}

		[Conditional("DEBUG")]
		public static void LogWarning(string msg)
		{
			LogStream logWarn = InternalLogStreams.LogWarn;
			if (logWarn != null)
			{
				logWarn.Log(msg);
			}
		}

		[Conditional("DEBUG")]
		public static void LogError(string msg)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(msg);
			}
		}

		[Conditional("DEBUG")]
		public static void LogException(Exception ex)
		{
			LogStream logException = InternalLogStreams.LogException;
			if (logException != null)
			{
				logException.Log(ex);
			}
		}
	}
}
