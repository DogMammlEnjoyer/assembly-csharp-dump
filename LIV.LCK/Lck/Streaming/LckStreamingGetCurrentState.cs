using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Liv.Lck.Streaming
{
	public class LckStreamingGetCurrentState : LckStreamingBaseState
	{
		public override void EnterState(LckStreamingController controller)
		{
			this.GetCurrentState(controller, controller.CancellationTokenSource.Token);
		}

		private Task GetCurrentState(LckStreamingController controller, CancellationToken cancellationToken)
		{
			LckStreamingGetCurrentState.<GetCurrentState>d__1 <GetCurrentState>d__;
			<GetCurrentState>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetCurrentState>d__.controller = controller;
			<GetCurrentState>d__.cancellationToken = cancellationToken;
			<GetCurrentState>d__.<>1__state = -1;
			<GetCurrentState>d__.<>t__builder.Start<LckStreamingGetCurrentState.<GetCurrentState>d__1>(ref <GetCurrentState>d__);
			return <GetCurrentState>d__.<>t__builder.Task;
		}
	}
}
