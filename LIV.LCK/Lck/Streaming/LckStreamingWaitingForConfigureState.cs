using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckStreamingWaitingForConfigureState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			controller.ShowNotification(NotificationType.ConfigureStream);
			this.CheckConfiguredState(controller, controller.CancellationTokenSource.Token);
		}

		private Task CheckConfiguredState(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckStreamingWaitingForConfigureState.<CheckConfiguredState>d__1 <CheckConfiguredState>d__;
			<CheckConfiguredState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckConfiguredState>d__.controller = controller;
			<CheckConfiguredState>d__.cancellationToken = cancellationToken;
			<CheckConfiguredState>d__.<>1__state = -1;
			<CheckConfiguredState>d__.<>t__builder.Start<LckStreamingWaitingForConfigureState.<CheckConfiguredState>d__1>(ref <CheckConfiguredState>d__);
			return <CheckConfiguredState>d__.<>t__builder.Task;
		}
	}
}
