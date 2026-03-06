using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fusion.Async
{
	public static class TaskManager
	{
		private static TaskFactory TaskFactory { get; set; } = Task.Factory;

		[Conditional("FUSION_UNITY")]
		public static void Setup()
		{
			bool flag = TaskManager.TaskFactory == null || TaskManager.TaskFactory.Equals(Task.Factory);
			if (flag)
			{
				TaskManager.TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}

		public static Task Service(Action recurringAction, CancellationToken cancellationToken, int interval, string serviceName = null)
		{
			bool flag = recurringAction == null;
			Task result2;
			if (flag)
			{
				result2 = Task.CompletedTask;
			}
			else
			{
				Task<bool> result = Task.FromResult<bool>(true);
				result2 = TaskManager.Service(delegate()
				{
					recurringAction();
					return result;
				}, cancellationToken, interval, serviceName);
			}
			return result2;
		}

		public static Task Service(Func<Task<bool>> recurringAction, CancellationToken cancellationToken, int interval, string serviceName = null)
		{
			TaskManager.<>c__DisplayClass6_0 CS$<>8__locals1 = new TaskManager.<>c__DisplayClass6_0();
			CS$<>8__locals1.serviceName = serviceName;
			CS$<>8__locals1.recurringAction = recurringAction;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.interval = interval;
			Assert.Check(CS$<>8__locals1.recurringAction != null, "Service Action should not be null");
			Assert.Check(CS$<>8__locals1.interval > 0, "Service delay must be greated than 0");
			Assert.Check(CS$<>8__locals1.cancellationToken != default(CancellationToken), "Service CancellationToken can't be the default");
			return TaskManager.TaskFactory.StartNew<Task>(delegate()
			{
				TaskManager.<>c__DisplayClass6_0.<<Service>b__0>d <<Service>b__0>d = new TaskManager.<>c__DisplayClass6_0.<<Service>b__0>d();
				<<Service>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Service>b__0>d.<>4__this = CS$<>8__locals1;
				<<Service>b__0>d.<>1__state = -1;
				<<Service>b__0>d.<>t__builder.Start<TaskManager.<>c__DisplayClass6_0.<<Service>b__0>d>(ref <<Service>b__0>d);
				return <<Service>b__0>d.<>t__builder.Task;
			}, CS$<>8__locals1.cancellationToken, TaskManager.TaskFactory.CreationOptions | TaskCreationOptions.LongRunning, TaskManager.TaskFactory.Scheduler);
		}

		public static Task Run(Func<CancellationToken, Task> action, CancellationToken cancellationToken, TaskCreationOptions options = TaskCreationOptions.None)
		{
			TaskManager.<>c__DisplayClass7_0 CS$<>8__locals1 = new TaskManager.<>c__DisplayClass7_0();
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.action = action;
			Assert.Check(CS$<>8__locals1.action != null);
			Assert.Check(CS$<>8__locals1.cancellationToken != default(CancellationToken));
			return TaskManager.TaskFactory.StartNew<Task>(delegate()
			{
				TaskManager.<>c__DisplayClass7_0.<<Run>b__0>d <<Run>b__0>d = new TaskManager.<>c__DisplayClass7_0.<<Run>b__0>d();
				<<Run>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Run>b__0>d.<>4__this = CS$<>8__locals1;
				<<Run>b__0>d.<>1__state = -1;
				<<Run>b__0>d.<>t__builder.Start<TaskManager.<>c__DisplayClass7_0.<<Run>b__0>d>(ref <<Run>b__0>d);
				return <<Run>b__0>d.<>t__builder.Task;
			}, CS$<>8__locals1.cancellationToken, TaskManager.TaskFactory.CreationOptions | options, TaskManager.TaskFactory.Scheduler);
		}

		public static Task ContinueWhenAll(Task[] precedingTasks, Func<CancellationToken, Task> action, CancellationToken cancellationToken)
		{
			TaskManager.<>c__DisplayClass8_0 CS$<>8__locals1 = new TaskManager.<>c__DisplayClass8_0();
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.action = action;
			Assert.Check(CS$<>8__locals1.action != null);
			Assert.Check(CS$<>8__locals1.cancellationToken != default(CancellationToken));
			Assert.Check(precedingTasks != null);
			Assert.Check(precedingTasks.Length != 0);
			return TaskManager.TaskFactory.ContinueWhenAll<Task>(precedingTasks, delegate(Task[] tasks)
			{
				TaskManager.<>c__DisplayClass8_0.<<ContinueWhenAll>b__0>d <<ContinueWhenAll>b__0>d = new TaskManager.<>c__DisplayClass8_0.<<ContinueWhenAll>b__0>d();
				<<ContinueWhenAll>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ContinueWhenAll>b__0>d.<>4__this = CS$<>8__locals1;
				<<ContinueWhenAll>b__0>d.tasks = tasks;
				<<ContinueWhenAll>b__0>d.<>1__state = -1;
				<<ContinueWhenAll>b__0>d.<>t__builder.Start<TaskManager.<>c__DisplayClass8_0.<<ContinueWhenAll>b__0>d>(ref <<ContinueWhenAll>b__0>d);
				return <<ContinueWhenAll>b__0>d.<>t__builder.Task;
			}, CS$<>8__locals1.cancellationToken, TaskManager.TaskFactory.ContinuationOptions, TaskManager.TaskFactory.Scheduler);
		}

		[DebuggerStepThrough]
		public static Task Delay(int delay, CancellationToken token = default(CancellationToken))
		{
			TaskManager.<Delay>d__9 <Delay>d__ = new TaskManager.<Delay>d__9();
			<Delay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Delay>d__.delay = delay;
			<Delay>d__.token = token;
			<Delay>d__.<>1__state = -1;
			<Delay>d__.<>t__builder.Start<TaskManager.<Delay>d__9>(ref <Delay>d__);
			return <Delay>d__.<>t__builder.Task;
		}
	}
}
