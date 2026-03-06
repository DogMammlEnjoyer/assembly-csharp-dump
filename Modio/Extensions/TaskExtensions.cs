using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Modio.Extensions
{
	public static class TaskExtensions
	{
		public static void ForgetTaskSafely(this Task task)
		{
			TaskExtensions.<ForgetTaskSafely>d__0 <ForgetTaskSafely>d__;
			<ForgetTaskSafely>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ForgetTaskSafely>d__.task = task;
			<ForgetTaskSafely>d__.<>1__state = -1;
			<ForgetTaskSafely>d__.<>t__builder.Start<TaskExtensions.<ForgetTaskSafely>d__0>(ref <ForgetTaskSafely>d__);
		}
	}
}
