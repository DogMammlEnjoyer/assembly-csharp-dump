using System;
using System.Linq;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class XRLoggingUtils
	{
		public static void Log(string message, Object context = null)
		{
			if (!XRLoggingUtils.k_DontLogAnything)
			{
				Debug.Log(message, context);
			}
		}

		public static void LogWarning(string message, Object context = null)
		{
			if (!XRLoggingUtils.k_DontLogAnything)
			{
				Debug.LogWarning(message, context);
			}
		}

		public static void LogError(string message, Object context = null)
		{
			if (!XRLoggingUtils.k_DontLogAnything)
			{
				Debug.LogError(message, context);
			}
		}

		public static void LogException(Exception exception, Object context = null)
		{
			if (!XRLoggingUtils.k_DontLogAnything)
			{
				Debug.LogException(exception, context);
			}
		}

		private static readonly bool k_DontLogAnything = Environment.GetCommandLineArgs().Contains("-runTests");
	}
}
