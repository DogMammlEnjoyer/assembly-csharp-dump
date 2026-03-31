using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckStreamingShowCodeState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			controller.SetNotificationStreamCode("Loading...");
			controller.ShowNotification(NotificationType.EnterStreamCode);
			this.GetCodeFromCore(controller, controller.CancellationTokenSource.Token);
		}

		private Task GetCodeFromCore(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckStreamingShowCodeState.<GetCodeFromCore>d__1 <GetCodeFromCore>d__;
			<GetCodeFromCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetCodeFromCore>d__.<>4__this = this;
			<GetCodeFromCore>d__.controller = controller;
			<GetCodeFromCore>d__.cancellationToken = cancellationToken;
			<GetCodeFromCore>d__.<>1__state = -1;
			<GetCodeFromCore>d__.<>t__builder.Start<LckStreamingShowCodeState.<GetCodeFromCore>d__1>(ref <GetCodeFromCore>d__);
			return <GetCodeFromCore>d__.<>t__builder.Task;
		}

		private Task WaitForUserToPairTablet(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckStreamingShowCodeState.<WaitForUserToPairTablet>d__2 <WaitForUserToPairTablet>d__;
			<WaitForUserToPairTablet>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForUserToPairTablet>d__.<>4__this = this;
			<WaitForUserToPairTablet>d__.controller = controller;
			<WaitForUserToPairTablet>d__.cancellationToken = cancellationToken;
			<WaitForUserToPairTablet>d__.<>1__state = -1;
			<WaitForUserToPairTablet>d__.<>t__builder.Start<LckStreamingShowCodeState.<WaitForUserToPairTablet>d__2>(ref <WaitForUserToPairTablet>d__);
			return <WaitForUserToPairTablet>d__.<>t__builder.Task;
		}

		private void LoginAttemptExpired(LckStreamingController controller)
		{
			controller.Log("Login request timed out after 15 mins, switching to camera mode");
			controller.ToggleCameraPage();
		}
	}
}
