using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class InRoomCallbacksContainer : List<IInRoomCallbacks>, IInRoomCallbacks
	{
		public InRoomCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			this.client.UpdateCallbackTargets();
			foreach (IInRoomCallbacks inRoomCallbacks in this)
			{
				inRoomCallbacks.OnPlayerEnteredRoom(newPlayer);
			}
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			this.client.UpdateCallbackTargets();
			foreach (IInRoomCallbacks inRoomCallbacks in this)
			{
				inRoomCallbacks.OnPlayerLeftRoom(otherPlayer);
			}
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			this.client.UpdateCallbackTargets();
			foreach (IInRoomCallbacks inRoomCallbacks in this)
			{
				inRoomCallbacks.OnRoomPropertiesUpdate(propertiesThatChanged);
			}
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProp)
		{
			this.client.UpdateCallbackTargets();
			foreach (IInRoomCallbacks inRoomCallbacks in this)
			{
				inRoomCallbacks.OnPlayerPropertiesUpdate(targetPlayer, changedProp);
			}
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			this.client.UpdateCallbackTargets();
			foreach (IInRoomCallbacks inRoomCallbacks in this)
			{
				inRoomCallbacks.OnMasterClientSwitched(newMasterClient);
			}
		}

		private readonly LoadBalancingClient client;
	}
}
