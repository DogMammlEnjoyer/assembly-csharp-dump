using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Meta.WitAi
{
	public static class TaskUtility
	{
		public static Task FromAsyncResult(IAsyncResult asyncResult)
		{
			if (asyncResult.IsCompleted)
			{
				return Task.FromResult<bool>(true);
			}
			return Task.Factory.FromAsync(asyncResult, new Action<IAsyncResult>(TaskUtility.StubForTaskFactory));
		}

		private static void StubForTaskFactory(IAsyncResult result)
		{
		}

		public static Task FromAsyncOp(AsyncOperation asyncOperation)
		{
			if (asyncOperation.isDone)
			{
				return Task.FromResult<bool>(true);
			}
			TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
			asyncOperation.completed += delegate(AsyncOperation operation)
			{
				completion.SetResult(true);
			};
			return completion.Task;
		}

		public static Task WaitForTimeout(int timeoutMs, Func<DateTime> getLastUpdate = null, Task completionTask = null)
		{
			TaskUtility.<WaitForTimeout>d__3 <WaitForTimeout>d__;
			<WaitForTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTimeout>d__.timeoutMs = timeoutMs;
			<WaitForTimeout>d__.getLastUpdate = getLastUpdate;
			<WaitForTimeout>d__.completionTask = completionTask;
			<WaitForTimeout>d__.<>1__state = -1;
			<WaitForTimeout>d__.<>t__builder.Start<TaskUtility.<WaitForTimeout>d__3>(ref <WaitForTimeout>d__);
			return <WaitForTimeout>d__.<>t__builder.Task;
		}
	}
}
