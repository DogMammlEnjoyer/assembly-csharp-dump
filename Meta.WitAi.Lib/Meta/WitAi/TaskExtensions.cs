using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Meta.WitAi
{
	public static class TaskExtensions
	{
		public static void WrapErrors(this Task task)
		{
			task.ContinueWith(delegate(Task t, object state)
			{
				if (t.Exception != null)
				{
					VLog.E(t.Exception, null);
				}
			}, null);
		}

		public static Task<bool> TimeoutAfter(this Task task, int ms)
		{
			TaskExtensions.<TimeoutAfter>d__1 <TimeoutAfter>d__;
			<TimeoutAfter>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<TimeoutAfter>d__.task = task;
			<TimeoutAfter>d__.ms = ms;
			<TimeoutAfter>d__.<>1__state = -1;
			<TimeoutAfter>d__.<>t__builder.Start<TaskExtensions.<TimeoutAfter>d__1>(ref <TimeoutAfter>d__);
			return <TimeoutAfter>d__.<>t__builder.Task;
		}

		public static Task WhenLessThan(this ICollection<Task> tasks, int max)
		{
			return tasks.WhenLessThan(max, CancellationToken.None);
		}

		public static Task WhenLessThan(this ICollection<Task> tasks, int max, CancellationToken cancellationToken)
		{
			if (tasks == null)
			{
				throw new ArgumentNullException("tasks");
			}
			TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
			cancellationToken.Register(delegate()
			{
				completion.TrySetCanceled();
			});
			int running = tasks.Count;
			max = Mathf.Max(0, max);
			Action<Task> <>9__1;
			foreach (Task task in tasks)
			{
				if (task.IsCompleted)
				{
					int running2 = running;
					running = running2 - 1;
				}
				else
				{
					Task task2 = task;
					Action<Task> continuationAction;
					if ((continuationAction = <>9__1) == null)
					{
						continuationAction = (<>9__1 = delegate(Task t)
						{
							if (!completion.Task.IsCompleted && Interlocked.Decrement(ref running) < max)
							{
								completion.SetResult(true);
							}
						});
					}
					task2.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
				}
			}
			if (!completion.Task.IsCompleted && running < max)
			{
				completion.SetResult(true);
			}
			return completion.Task;
		}
	}
}
