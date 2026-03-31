using System;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks
{
	public static class UniTaskScheduler
	{
		public static event Action<Exception> UnobservedTaskException;

		private static void InvokeUnobservedTaskException(object state)
		{
			UniTaskScheduler.UnobservedTaskException((Exception)state);
		}

		internal static void PublishUnobservedTaskException(Exception ex)
		{
			if (ex != null)
			{
				if (!UniTaskScheduler.PropagateOperationCanceledException && ex is OperationCanceledException)
				{
					return;
				}
				if (UniTaskScheduler.UnobservedTaskException != null)
				{
					if (!UniTaskScheduler.DispatchUnityMainThread || Thread.CurrentThread.ManagedThreadId == PlayerLoopHelper.MainThreadId)
					{
						UniTaskScheduler.UnobservedTaskException(ex);
						return;
					}
					PlayerLoopHelper.UnitySynchronizationContext.Post(UniTaskScheduler.handleExceptionInvoke, ex);
					return;
				}
				else
				{
					string message = null;
					if (UniTaskScheduler.UnobservedExceptionWriteLogType != LogType.Exception)
					{
						message = "UnobservedTaskException: " + ex.ToString();
					}
					switch (UniTaskScheduler.UnobservedExceptionWriteLogType)
					{
					case LogType.Error:
						Debug.LogError(message);
						return;
					case LogType.Assert:
						break;
					case LogType.Warning:
						Debug.LogWarning(message);
						return;
					case LogType.Log:
						Debug.Log(message);
						return;
					case LogType.Exception:
						Debug.LogException(ex);
						break;
					default:
						return;
					}
				}
			}
		}

		public static bool PropagateOperationCanceledException = false;

		public static LogType UnobservedExceptionWriteLogType = LogType.Exception;

		public static bool DispatchUnityMainThread = true;

		private static readonly SendOrPostCallback handleExceptionInvoke = new SendOrPostCallback(UniTaskScheduler.InvokeUnobservedTaskException);
	}
}
