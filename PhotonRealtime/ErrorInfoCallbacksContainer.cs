using System;
using System.Collections.Generic;

namespace Photon.Realtime
{
	internal class ErrorInfoCallbacksContainer : List<IErrorInfoCallback>, IErrorInfoCallback
	{
		public ErrorInfoCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnErrorInfo(ErrorInfo errorInfo)
		{
			this.client.UpdateCallbackTargets();
			foreach (IErrorInfoCallback errorInfoCallback in this)
			{
				errorInfoCallback.OnErrorInfo(errorInfo);
			}
		}

		private LoadBalancingClient client;
	}
}
