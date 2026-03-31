using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckServiceUnavailableState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			if (LckServiceUnavailableState._enterServiceUnavailableStateCount < 5)
			{
				LckServiceUnavailableState._enterServiceUnavailableStateCount++;
			}
			controller.ShowNotification(NotificationType.ServiceUnavailable);
			this.CheckServiceStatus(controller, controller.CancellationTokenSource.Token);
		}

		private Task CheckServiceStatus(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckServiceUnavailableState.<CheckServiceStatus>d__2 <CheckServiceStatus>d__;
			<CheckServiceStatus>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckServiceStatus>d__.controller = controller;
			<CheckServiceStatus>d__.cancellationToken = cancellationToken;
			<CheckServiceStatus>d__.<>1__state = -1;
			<CheckServiceStatus>d__.<>t__builder.Start<LckServiceUnavailableState.<CheckServiceStatus>d__2>(ref <CheckServiceStatus>d__);
			return <CheckServiceStatus>d__.<>t__builder.Task;
		}

		private static int _enterServiceUnavailableStateCount;
	}
}
