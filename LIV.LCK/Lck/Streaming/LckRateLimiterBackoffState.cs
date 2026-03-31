using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Liv.Lck.Tablet;

namespace Liv.Lck.Streaming
{
	public class LckRateLimiterBackoffState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			controller.ShowNotification(NotificationType.RateLimiterBackoff);
			this.WaitForRateLimiter(controller, controller.CancellationTokenSource.Token);
		}

		private Task WaitForRateLimiter(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckRateLimiterBackoffState.<WaitForRateLimiter>d__1 <WaitForRateLimiter>d__;
			<WaitForRateLimiter>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForRateLimiter>d__.controller = controller;
			<WaitForRateLimiter>d__.cancellationToken = cancellationToken;
			<WaitForRateLimiter>d__.<>1__state = -1;
			<WaitForRateLimiter>d__.<>t__builder.Start<LckRateLimiterBackoffState.<WaitForRateLimiter>d__1>(ref <WaitForRateLimiter>d__);
			return <WaitForRateLimiter>d__.<>t__builder.Task;
		}
	}
}
