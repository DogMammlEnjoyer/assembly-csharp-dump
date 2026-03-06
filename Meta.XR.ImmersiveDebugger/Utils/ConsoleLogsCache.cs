using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	internal static class ConsoleLogsCache
	{
		private static void OnApplicationQuitting()
		{
			Application.logMessageReceivedThreaded -= ConsoleLogsCache.EnqueueLogEntry;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void OnLoad()
		{
			List<ValueTuple<string, string, LogType>> startupLogs = ConsoleLogsCache.StartupLogs;
			if (startupLogs != null)
			{
				startupLogs.Clear();
			}
			ConsoleLogsCache._mainThreadContext = null;
			Application.quitting -= ConsoleLogsCache.OnApplicationQuitting;
			Application.logMessageReceivedThreaded -= ConsoleLogsCache.EnqueueLogEntry;
			ConsoleLogsCache.StartCachingLogs();
		}

		private static void StartCachingLogs()
		{
			ConsoleLogsCache._mainThreadContext = SynchronizationContext.Current;
			if (RuntimeSettings.Instance.ImmersiveDebuggerEnabled)
			{
				Application.logMessageReceivedThreaded += ConsoleLogsCache.EnqueueLogEntry;
				Application.quitting += ConsoleLogsCache.OnApplicationQuitting;
			}
		}

		internal static void ConsumeStartupLogs(Action<string, string, LogType> logProcessor)
		{
			foreach (ValueTuple<string, string, LogType> valueTuple in ConsoleLogsCache.StartupLogs)
			{
				logProcessor(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3);
			}
			ConsoleLogsCache.StartupLogs.Clear();
		}

		private static void EnqueueLogEntry(string logString, string stackTrace, LogType type)
		{
			ConsoleLogsCache._mainThreadContext.Post(delegate(object _)
			{
				if (ConsoleLogsCache.OnLogReceived == null)
				{
					ConsoleLogsCache.StartupLogs.Add(new ValueTuple<string, string, LogType>(logString, stackTrace, type));
					return;
				}
				ConsoleLogsCache.OnLogReceived(logString, stackTrace, type);
			}, null);
		}

		internal static Action<string, string, LogType> OnLogReceived;

		private static readonly List<ValueTuple<string, string, LogType>> StartupLogs = new List<ValueTuple<string, string, LogType>>();

		private static SynchronizationContext _mainThreadContext;
	}
}
