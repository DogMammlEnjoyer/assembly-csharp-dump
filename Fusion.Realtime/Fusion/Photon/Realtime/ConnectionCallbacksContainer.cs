using System;
using System.Collections.Generic;

namespace Fusion.Photon.Realtime
{
	internal class ConnectionCallbacksContainer : List<IConnectionCallbacks>, IConnectionCallbacks
	{
		public ConnectionCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnConnected()
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnConnected();
			}
		}

		public void OnConnectedToMaster()
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnConnectedToMaster();
			}
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnRegionListReceived(regionHandler);
			}
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnDisconnected(cause);
			}
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnCustomAuthenticationResponse(data);
			}
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			this.client.UpdateCallbackTargets();
			foreach (IConnectionCallbacks connectionCallbacks in this)
			{
				connectionCallbacks.OnCustomAuthenticationFailed(debugMessage);
			}
		}

		private readonly LoadBalancingClient client;
	}
}
