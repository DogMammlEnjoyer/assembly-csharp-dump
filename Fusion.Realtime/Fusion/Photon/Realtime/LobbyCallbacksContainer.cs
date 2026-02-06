using System;
using System.Collections.Generic;

namespace Fusion.Photon.Realtime
{
	internal class LobbyCallbacksContainer : List<ILobbyCallbacks>, ILobbyCallbacks
	{
		public LobbyCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnJoinedLobby()
		{
			this.client.UpdateCallbackTargets();
			foreach (ILobbyCallbacks lobbyCallbacks in this)
			{
				lobbyCallbacks.OnJoinedLobby();
			}
		}

		public void OnLeftLobby()
		{
			this.client.UpdateCallbackTargets();
			foreach (ILobbyCallbacks lobbyCallbacks in this)
			{
				lobbyCallbacks.OnLeftLobby();
			}
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			this.client.UpdateCallbackTargets();
			foreach (ILobbyCallbacks lobbyCallbacks in this)
			{
				lobbyCallbacks.OnRoomListUpdate(roomList);
			}
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
			this.client.UpdateCallbackTargets();
			foreach (ILobbyCallbacks lobbyCallbacks in this)
			{
				lobbyCallbacks.OnLobbyStatisticsUpdate(lobbyStatistics);
			}
		}

		private readonly LoadBalancingClient client;
	}
}
