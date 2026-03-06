using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckInvalidArgumentState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			controller.ShowNotification(NotificationType.InvalidArgument);
			this.SwitchStateAfterDelay(controller, controller.CancellationTokenSource.Token);
		}

		private Task SwitchStateAfterDelay(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckInvalidArgumentState.<SwitchStateAfterDelay>d__1 <SwitchStateAfterDelay>d__;
			<SwitchStateAfterDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SwitchStateAfterDelay>d__.controller = controller;
			<SwitchStateAfterDelay>d__.cancellationToken = cancellationToken;
			<SwitchStateAfterDelay>d__.<>1__state = -1;
			<SwitchStateAfterDelay>d__.<>t__builder.Start<LckInvalidArgumentState.<SwitchStateAfterDelay>d__1>(ref <SwitchStateAfterDelay>d__);
			return <SwitchStateAfterDelay>d__.<>t__builder.Task;
		}
	}
}
