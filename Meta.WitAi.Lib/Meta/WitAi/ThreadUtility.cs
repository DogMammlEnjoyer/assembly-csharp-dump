using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Meta.Voice.Logging;
using UnityEngine;

namespace Meta.WitAi
{
	public static class ThreadUtility
	{
		public static bool IsMainThread()
		{
			return Thread.CurrentThread == ThreadUtility._mainThread;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (ThreadUtility._mainThreadScheduler != null)
			{
				return;
			}
			ThreadUtility._mainThread = Thread.CurrentThread;
			ThreadUtility._mainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			ThreadUtility.EarlyTask earlyTask;
			while (ThreadUtility._earlyTasks.TryDequeue(out earlyTask))
			{
				earlyTask.Start();
			}
		}

		private static Task EnqueueMainThreadTask(Task task)
		{
			if (ThreadUtility._mainThreadScheduler != null)
			{
				task.Start(ThreadUtility._mainThreadScheduler);
				return task;
			}
			ThreadUtility.EarlyTask item = new ThreadUtility.EarlyTask(task);
			ThreadUtility._earlyTasks.Enqueue(item);
			return task;
		}

		public static Task CallOnMainThread(Action callback)
		{
			return ThreadUtility.CallOnMainThread(null, callback);
		}

		public static Task CallOnMainThread(IVLogger logger, Action callback)
		{
			if (!ThreadUtility.IsMainThread())
			{
				return ThreadUtility.EnqueueMainThreadTask(new Task(delegate()
				{
					ThreadUtility.SafeAction(logger, callback);
				}));
			}
			return Task.FromResult<bool>(ThreadUtility.SafeAction(logger, callback));
		}

		public static Task<T> CallOnMainThread<T>(Func<T> callback)
		{
			return ThreadUtility.CallOnMainThread<T>(null, callback);
		}

		public static Task<T> CallOnMainThread<T>(IVLogger logger, Func<T> callback)
		{
			if (!ThreadUtility.IsMainThread())
			{
				return (Task<T>)ThreadUtility.EnqueueMainThreadTask(new Task<T>(() => ThreadUtility.SafeAction<T>(logger, callback)));
			}
			return Task.FromResult<T>(ThreadUtility.SafeAction<T>(logger, callback));
		}

		private static bool SafeAction(IVLogger logger, Action callback)
		{
			bool result;
			try
			{
				callback();
				result = true;
			}
			catch (Exception ex)
			{
				if (logger == null)
				{
					VLog.E(ex, null);
				}
				else
				{
					logger.Error("{0}\n{1}", new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
				throw;
			}
			return result;
		}

		private static T SafeAction<T>(IVLogger logger, Func<T> callback)
		{
			T result;
			try
			{
				result = callback();
			}
			catch (Exception ex)
			{
				if (logger == null)
				{
					VLog.E(ex, null);
				}
				else
				{
					logger.Error(ex, "", Array.Empty<object>());
				}
				throw;
			}
			return result;
		}

		private static Task SafeTask(IVLogger logger, Func<Task> callback)
		{
			ThreadUtility.<SafeTask>d__13 <SafeTask>d__;
			<SafeTask>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SafeTask>d__.logger = logger;
			<SafeTask>d__.callback = callback;
			<SafeTask>d__.<>1__state = -1;
			<SafeTask>d__.<>t__builder.Start<ThreadUtility.<SafeTask>d__13>(ref <SafeTask>d__);
			return <SafeTask>d__.<>t__builder.Task;
		}

		private static Task<T> SafeTask<T>(IVLogger logger, Func<Task<T>> callback)
		{
			ThreadUtility.<SafeTask>d__14<T> <SafeTask>d__;
			<SafeTask>d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
			<SafeTask>d__.logger = logger;
			<SafeTask>d__.callback = callback;
			<SafeTask>d__.<>1__state = -1;
			<SafeTask>d__.<>t__builder.Start<ThreadUtility.<SafeTask>d__14<T>>(ref <SafeTask>d__);
			return <SafeTask>d__.<>t__builder.Task;
		}

		public static Task BackgroundAsync(IVLogger logger, Func<Task> callback)
		{
			if (ThreadUtility.IsMainThread())
			{
				return Task.Run(() => ThreadUtility.SafeTask(logger, callback));
			}
			return ThreadUtility.SafeTask(logger, callback);
		}

		public static Task<T> BackgroundAsync<T>(IVLogger logger, Func<Task<T>> callback)
		{
			if (ThreadUtility.IsMainThread())
			{
				return Task.Run<T>(() => ThreadUtility.SafeTask<T>(logger, callback));
			}
			return ThreadUtility.SafeTask<T>(logger, callback);
		}

		public static Task Background(IVLogger logger, Action callback)
		{
			if (ThreadUtility.IsMainThread())
			{
				return Task.Run<bool>(() => ThreadUtility.SafeAction(logger, callback));
			}
			return Task.FromResult<bool>(ThreadUtility.SafeAction(logger, callback));
		}

		public static IEnumerator CoroutineAwait(Func<Task> func)
		{
			Task task = func();
			while (!task.IsCompleted)
			{
				yield return null;
			}
			yield break;
		}

		public static IEnumerator CoroutineAwait<T>(Func<Task<T>> func, Action<T> result)
		{
			Task<T> task = func();
			while (!task.IsCompleted)
			{
				yield return null;
			}
			result(task.Result);
			yield break;
		}

		public static IEnumerator CoroutineAwait<T, T1>(Func<T1, Task<T>> func, T1 data, Action<T> result)
		{
			Task<T> task = func(data);
			while (!task.IsCompleted)
			{
				yield return null;
			}
			result(task.Result);
			yield break;
		}

		public static IEnumerator CoroutineAwait<T, T1, T2>(Func<T1, T2, Task<T>> func, T1 data1, T2 data2, Action<T> result)
		{
			Task<T> task = func(data1, data2);
			while (!task.IsCompleted)
			{
				yield return null;
			}
			result(task.Result);
			yield break;
		}

		public static IEnumerator CoroutineAwait<T, T1, T2, T3>(Func<T1, T2, T3, Task<T>> func, T1 data1, T2 data2, T3 data3, Action<T> result)
		{
			Task<T> task = func(data1, data2, data3);
			while (!task.IsCompleted)
			{
				yield return null;
			}
			result(task.Result);
			yield break;
		}

		private static TaskScheduler _mainThreadScheduler;

		private static Thread _mainThread;

		private static readonly ConcurrentQueue<ThreadUtility.EarlyTask> _earlyTasks = new ConcurrentQueue<ThreadUtility.EarlyTask>();

		private class EarlyTask
		{
			public EarlyTask(Task task)
			{
				this._task = task;
			}

			public void Start()
			{
				this._task.Start(ThreadUtility._mainThreadScheduler);
			}

			private Task _task;
		}
	}
}
