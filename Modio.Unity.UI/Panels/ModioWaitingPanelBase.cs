using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Modio.Unity.UI.Panels
{
	public abstract class ModioWaitingPanelBase : ModioPanelBase
	{
		public void OpenAndWaitFor<T>(Task<T> task, Action<T> action)
		{
			ModioWaitingPanelBase.<OpenAndWaitFor>d__0<T> <OpenAndWaitFor>d__;
			<OpenAndWaitFor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OpenAndWaitFor>d__.<>4__this = this;
			<OpenAndWaitFor>d__.task = task;
			<OpenAndWaitFor>d__.action = action;
			<OpenAndWaitFor>d__.<>1__state = -1;
			<OpenAndWaitFor>d__.<>t__builder.Start<ModioWaitingPanelBase.<OpenAndWaitFor>d__0<T>>(ref <OpenAndWaitFor>d__);
		}

		public Task<T> OpenAndWaitForAsync<T>(Task<T> task)
		{
			ModioWaitingPanelBase.<OpenAndWaitForAsync>d__1<T> <OpenAndWaitForAsync>d__;
			<OpenAndWaitForAsync>d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
			<OpenAndWaitForAsync>d__.<>4__this = this;
			<OpenAndWaitForAsync>d__.task = task;
			<OpenAndWaitForAsync>d__.<>1__state = -1;
			<OpenAndWaitForAsync>d__.<>t__builder.Start<ModioWaitingPanelBase.<OpenAndWaitForAsync>d__1<T>>(ref <OpenAndWaitForAsync>d__);
			return <OpenAndWaitForAsync>d__.<>t__builder.Task;
		}

		public Task OpenAndWaitFor(Task task, Action action = null)
		{
			ModioWaitingPanelBase.<OpenAndWaitFor>d__2 <OpenAndWaitFor>d__;
			<OpenAndWaitFor>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OpenAndWaitFor>d__.<>4__this = this;
			<OpenAndWaitFor>d__.task = task;
			<OpenAndWaitFor>d__.action = action;
			<OpenAndWaitFor>d__.<>1__state = -1;
			<OpenAndWaitFor>d__.<>t__builder.Start<ModioWaitingPanelBase.<OpenAndWaitFor>d__2>(ref <OpenAndWaitFor>d__);
			return <OpenAndWaitFor>d__.<>t__builder.Task;
		}

		public override void DoDefaultSelection()
		{
			this.SetSelectedGameObject(null);
		}

		protected override void CancelPressed()
		{
		}
	}
}
