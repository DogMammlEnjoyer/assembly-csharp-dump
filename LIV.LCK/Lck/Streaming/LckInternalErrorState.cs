using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckInternalErrorState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			if (LckInternalErrorState._enterInternalErrorStateCount < 5)
			{
				LckInternalErrorState._enterInternalErrorStateCount++;
			}
			controller.ShowNotification(NotificationType.InternalError);
			this.CheckInternalError(controller, controller.CancellationTokenSource.Token);
		}

		private Task CheckInternalError(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckInternalErrorState.<CheckInternalError>d__2 <CheckInternalError>d__;
			<CheckInternalError>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CheckInternalError>d__.controller = controller;
			<CheckInternalError>d__.cancellationToken = cancellationToken;
			<CheckInternalError>d__.<>1__state = -1;
			<CheckInternalError>d__.<>t__builder.Start<LckInternalErrorState.<CheckInternalError>d__2>(ref <CheckInternalError>d__);
			return <CheckInternalError>d__.<>t__builder.Task;
		}

		private static int _enterInternalErrorStateCount;
	}
}
