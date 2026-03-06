using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal static class AsyncHelper
	{
		public static bool IsSuccess(this Task task)
		{
			return task.IsCompleted && task.Exception == null;
		}

		public static Task CallVoidFuncWhenFinish(this Task task, Action func)
		{
			if (task.IsSuccess())
			{
				func();
				return AsyncHelper.DoneTask;
			}
			return task._CallVoidFuncWhenFinish(func);
		}

		private static Task _CallVoidFuncWhenFinish(this Task task, Action func)
		{
			AsyncHelper.<_CallVoidFuncWhenFinish>d__6 <_CallVoidFuncWhenFinish>d__;
			<_CallVoidFuncWhenFinish>d__.task = task;
			<_CallVoidFuncWhenFinish>d__.func = func;
			<_CallVoidFuncWhenFinish>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<_CallVoidFuncWhenFinish>d__.<>1__state = -1;
			<_CallVoidFuncWhenFinish>d__.<>t__builder.Start<AsyncHelper.<_CallVoidFuncWhenFinish>d__6>(ref <_CallVoidFuncWhenFinish>d__);
			return <_CallVoidFuncWhenFinish>d__.<>t__builder.Task;
		}

		public static Task<bool> ReturnTaskBoolWhenFinish(this Task task, bool ret)
		{
			if (!task.IsSuccess())
			{
				return task._ReturnTaskBoolWhenFinish(ret);
			}
			if (ret)
			{
				return AsyncHelper.DoneTaskTrue;
			}
			return AsyncHelper.DoneTaskFalse;
		}

		public static Task<bool> _ReturnTaskBoolWhenFinish(this Task task, bool ret)
		{
			AsyncHelper.<_ReturnTaskBoolWhenFinish>d__8 <_ReturnTaskBoolWhenFinish>d__;
			<_ReturnTaskBoolWhenFinish>d__.task = task;
			<_ReturnTaskBoolWhenFinish>d__.ret = ret;
			<_ReturnTaskBoolWhenFinish>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ReturnTaskBoolWhenFinish>d__.<>1__state = -1;
			<_ReturnTaskBoolWhenFinish>d__.<>t__builder.Start<AsyncHelper.<_ReturnTaskBoolWhenFinish>d__8>(ref <_ReturnTaskBoolWhenFinish>d__);
			return <_ReturnTaskBoolWhenFinish>d__.<>t__builder.Task;
		}

		public static Task CallTaskFuncWhenFinish(this Task task, Func<Task> func)
		{
			if (task.IsSuccess())
			{
				return func();
			}
			return AsyncHelper._CallTaskFuncWhenFinish(task, func);
		}

		private static Task _CallTaskFuncWhenFinish(Task task, Func<Task> func)
		{
			AsyncHelper.<_CallTaskFuncWhenFinish>d__10 <_CallTaskFuncWhenFinish>d__;
			<_CallTaskFuncWhenFinish>d__.task = task;
			<_CallTaskFuncWhenFinish>d__.func = func;
			<_CallTaskFuncWhenFinish>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<_CallTaskFuncWhenFinish>d__.<>1__state = -1;
			<_CallTaskFuncWhenFinish>d__.<>t__builder.Start<AsyncHelper.<_CallTaskFuncWhenFinish>d__10>(ref <_CallTaskFuncWhenFinish>d__);
			return <_CallTaskFuncWhenFinish>d__.<>t__builder.Task;
		}

		public static Task<bool> CallBoolTaskFuncWhenFinish(this Task task, Func<Task<bool>> func)
		{
			if (task.IsSuccess())
			{
				return func();
			}
			return task._CallBoolTaskFuncWhenFinish(func);
		}

		private static Task<bool> _CallBoolTaskFuncWhenFinish(this Task task, Func<Task<bool>> func)
		{
			AsyncHelper.<_CallBoolTaskFuncWhenFinish>d__12 <_CallBoolTaskFuncWhenFinish>d__;
			<_CallBoolTaskFuncWhenFinish>d__.task = task;
			<_CallBoolTaskFuncWhenFinish>d__.func = func;
			<_CallBoolTaskFuncWhenFinish>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_CallBoolTaskFuncWhenFinish>d__.<>1__state = -1;
			<_CallBoolTaskFuncWhenFinish>d__.<>t__builder.Start<AsyncHelper.<_CallBoolTaskFuncWhenFinish>d__12>(ref <_CallBoolTaskFuncWhenFinish>d__);
			return <_CallBoolTaskFuncWhenFinish>d__.<>t__builder.Task;
		}

		public static Task<bool> ContinueBoolTaskFuncWhenFalse(this Task<bool> task, Func<Task<bool>> func)
		{
			if (!task.IsSuccess())
			{
				return AsyncHelper._ContinueBoolTaskFuncWhenFalse(task, func);
			}
			if (task.Result)
			{
				return AsyncHelper.DoneTaskTrue;
			}
			return func();
		}

		private static Task<bool> _ContinueBoolTaskFuncWhenFalse(Task<bool> task, Func<Task<bool>> func)
		{
			AsyncHelper.<_ContinueBoolTaskFuncWhenFalse>d__14 <_ContinueBoolTaskFuncWhenFalse>d__;
			<_ContinueBoolTaskFuncWhenFalse>d__.task = task;
			<_ContinueBoolTaskFuncWhenFalse>d__.func = func;
			<_ContinueBoolTaskFuncWhenFalse>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ContinueBoolTaskFuncWhenFalse>d__.<>1__state = -1;
			<_ContinueBoolTaskFuncWhenFalse>d__.<>t__builder.Start<AsyncHelper.<_ContinueBoolTaskFuncWhenFalse>d__14>(ref <_ContinueBoolTaskFuncWhenFalse>d__);
			return <_ContinueBoolTaskFuncWhenFalse>d__.<>t__builder.Task;
		}

		public static readonly Task DoneTask = Task.FromResult<bool>(true);

		public static readonly Task<bool> DoneTaskTrue = Task.FromResult<bool>(true);

		public static readonly Task<bool> DoneTaskFalse = Task.FromResult<bool>(false);

		public static readonly Task<int> DoneTaskZero = Task.FromResult<int>(0);
	}
}
