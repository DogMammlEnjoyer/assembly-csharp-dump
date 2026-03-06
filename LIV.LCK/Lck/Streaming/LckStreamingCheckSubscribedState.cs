using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckStreamingCheckSubscribedState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			controller.ShowNotification(NotificationType.CheckSubscribed);
			this.CheckSubscribedState(controller, controller.CancellationTokenSource.Token);
		}

		private Task CheckSubscribedState(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckStreamingCheckSubscribedState.<CheckSubscribedState>d__1 <CheckSubscribedState>d__;
			<CheckSubscribedState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckSubscribedState>d__.controller = controller;
			<CheckSubscribedState>d__.cancellationToken = cancellationToken;
			<CheckSubscribedState>d__.<>1__state = -1;
			<CheckSubscribedState>d__.<>t__builder.Start<LckStreamingCheckSubscribedState.<CheckSubscribedState>d__1>(ref <CheckSubscribedState>d__);
			return <CheckSubscribedState>d__.<>t__builder.Task;
		}
	}
}
