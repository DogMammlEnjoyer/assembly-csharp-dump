using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class WebRpcCallbacksContainer : List<IWebRpcCallback>, IWebRpcCallback
	{
		public WebRpcCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnWebRpcResponse(OperationResponse response)
		{
			this.client.UpdateCallbackTargets();
			foreach (IWebRpcCallback webRpcCallback in this)
			{
				webRpcCallback.OnWebRpcResponse(response);
			}
		}

		private LoadBalancingClient client;
	}
}
