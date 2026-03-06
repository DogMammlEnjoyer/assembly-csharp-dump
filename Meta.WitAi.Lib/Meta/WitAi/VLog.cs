using System;
using System.Diagnostics;
using Meta.Voice.Logging;
using UnityEngine;

namespace Meta.WitAi
{
	public static class VLog
	{
		public static bool SuppressLogs { get; set; } = !Application.isEditor && !Debug.isDebugBuild;

		public static void I(object log)
		{
			VLog.Log(VLoggerVerbosity.Info, null, log, null);
		}

		[Obsolete("Use VLogger.Info() instead")]
		public static void I(string logCategory, object log)
		{
			VLog.Log(VLoggerVerbosity.Info, logCategory, log, null);
		}

		public static void D(object log)
		{
			VLog.Log(VLoggerVerbosity.Debug, null, log, null);
		}

		[Obsolete("Use VLogger.Debug() instead")]
		public static void D(string logCategory, object log)
		{
			VLog.Log(VLoggerVerbosity.Debug, logCategory, log, null);
		}

		public static void W(object log, Exception e = null)
		{
			VLog.Log(VLoggerVerbosity.Warning, null, log, e);
		}

		public static void W(string logCategory, object log, Exception e = null)
		{
			VLog.Log(VLoggerVerbosity.Warning, logCategory, log, e);
		}

		public static void E(object log, Exception e = null)
		{
			VLog.Log(VLoggerVerbosity.Error, null, log, e);
		}

		public static void E(string logCategory, object log, Exception e = null)
		{
			VLog.Log(VLoggerVerbosity.Error, logCategory, log, e);
		}

		private static void Log(VLoggerVerbosity logType, string logCategory, object log, Exception exception = null)
		{
			string text = logCategory;
			if (string.IsNullOrEmpty(text))
			{
				text = VLog.GetCallingCategory();
			}
			IVLogger logger = VLog.LoggerRegistry.GetLogger(text, null);
			if (logType == VLoggerVerbosity.Warning)
			{
				logger.Warning(log.ToString(), Array.Empty<object>());
				return;
			}
			if (logType == VLoggerVerbosity.Error)
			{
				logger.Error(KnownErrorCode.Unknown, ((log != null) ? log.ToString() : null) + ((exception == null) ? "" : string.Format("\n{0}", exception)), Array.Empty<object>());
				return;
			}
			logger.Debug(log.ToString(), null, null, null, null, "Log", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Lib\\Wit\\Runtime\\Utilities\\Logging\\VLog.cs", 197);
		}

		private static string GetCallingCategory()
		{
			StackTrace stackTrace = new StackTrace();
			string text;
			if (stackTrace == null)
			{
				text = null;
			}
			else
			{
				StackFrame frame = stackTrace.GetFrame(3);
				text = ((frame != null) ? frame.GetMethod().DeclaringType.Name : null);
			}
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				return "NoStacktrace";
			}
			return text2;
		}

		private static readonly ILoggerRegistry LoggerRegistry = Meta.Voice.Logging.LoggerRegistry.Instance;
	}
}
