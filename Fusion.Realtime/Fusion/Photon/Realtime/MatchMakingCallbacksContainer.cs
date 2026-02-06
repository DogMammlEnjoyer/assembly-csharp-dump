using System;
using System.Collections.Generic;

namespace Fusion.Photon.Realtime
{
	internal class MatchMakingCallbacksContainer : List<IMatchmakingCallbacks>, IMatchmakingCallbacks
	{
		public MatchMakingCallbacksContainer(LoadBalancingClient client)
		{
			this.client = client;
		}

		public void OnCreatedRoom()
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnCreatedRoom();
			}
		}

		public void OnJoinedRoom()
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnJoinedRoom();
			}
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnCreateRoomFailed(returnCode, message);
			}
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnJoinRandomFailed(returnCode, message);
			}
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnJoinRoomFailed(returnCode, message);
			}
		}

		public void OnLeftRoom()
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnLeftRoom();
			}
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
			this.client.UpdateCallbackTargets();
			foreach (IMatchmakingCallbacks matchmakingCallbacks in this)
			{
				matchmakingCallbacks.OnFriendListUpdate(friendList);
			}
		}

		private readonly LoadBalancingClient client;
	}
}
