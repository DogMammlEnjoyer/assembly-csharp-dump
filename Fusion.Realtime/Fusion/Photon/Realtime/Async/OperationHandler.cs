using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fusion.Photon.Realtime.Async
{
	internal class OperationHandler : IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		public Task<short> Task
		{
			get
			{
				return this._result.Task;
			}
		}

		public TaskCompletionSource<short> CompletionSource
		{
			get
			{
				return this._result;
			}
		}

		public CancellationToken Token
		{
			get
			{
				return this._cancellation.Token;
			}
		}

		public bool IsCancellationRequested
		{
			get
			{
				return this._cancellation.IsCancellationRequested;
			}
		}

		public OperationHandler(bool throwOnErrors = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			this._result = new TaskCompletionSource<short>();
			this._cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(30.0));
			this._cancellation.Token.Register(new Action(this.Expire));
			this._throwOnErrors = throwOnErrors;
			bool flag = externalCancellationToken != default(CancellationToken);
			if (flag)
			{
				externalCancellationToken.Register(new Action(this.Cancel));
			}
		}

		public void SetResult(short result)
		{
			bool flag = this._result.TrySetResult(result);
			if (flag)
			{
				bool flag2 = !this._cancellation.IsCancellationRequested;
				if (flag2)
				{
					this._cancellation.Cancel();
				}
				this._cancellation.Dispose();
			}
		}

		public void SetException(Exception e)
		{
			bool flag = this._result.TrySetException(e);
			if (flag)
			{
				bool flag2 = !this._cancellation.IsCancellationRequested;
				if (flag2)
				{
					this._cancellation.Cancel();
				}
				this._cancellation.Dispose();
			}
		}

		private void Expire()
		{
			this.SetException(new OperationTimeoutException("Operation timed out"));
		}

		private void Cancel()
		{
			this.SetException(new OperationCanceledException("Operation cancelled."));
		}

		public void OnConnected()
		{
			Action connectedToNameServer = this.ConnectionCallbacks.ConnectedToNameServer;
			if (connectedToNameServer != null)
			{
				connectedToNameServer();
			}
		}

		public void OnConnectedToMaster()
		{
			bool flag = this.ConnectionCallbacks.ConnectedToMaster != null;
			if (flag)
			{
				this.ConnectionCallbacks.ConnectedToMaster();
			}
			else
			{
				this.SetResult(0);
			}
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			bool flag = this.ConnectionCallbacks.CustomAuthenticationFailed != null;
			if (flag)
			{
				this.ConnectionCallbacks.CustomAuthenticationFailed(debugMessage);
			}
			else
			{
				this.SetException(new AuthenticationFailedException(debugMessage));
			}
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			Action<Dictionary<string, object>> customAuthenticationResponse = this.ConnectionCallbacks.CustomAuthenticationResponse;
			if (customAuthenticationResponse != null)
			{
				customAuthenticationResponse(data);
			}
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			bool flag = this.ConnectionCallbacks.Disconnected != null;
			if (flag)
			{
				this.ConnectionCallbacks.Disconnected(cause);
			}
			else
			{
				this.SetException(new DisconnectException(cause));
			}
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Action<RegionHandler> regionListReceived = this.ConnectionCallbacks.RegionListReceived;
			if (regionListReceived != null)
			{
				regionListReceived(regionHandler);
			}
		}

		public void OnCreatedRoom()
		{
			Action createdRoom = this.MatchmakingCallbacks.CreatedRoom;
			if (createdRoom != null)
			{
				createdRoom();
			}
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			bool flag = this.MatchmakingCallbacks.CreateRoomFailed != null;
			if (flag)
			{
				this.MatchmakingCallbacks.CreateRoomFailed(returnCode, message);
			}
			else
			{
				bool throwOnErrors = this._throwOnErrors;
				if (throwOnErrors)
				{
					this.SetException(new OperationException(returnCode, message));
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(message);
					}
					this.SetResult(returnCode);
				}
			}
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
			Action<List<FriendInfo>> friendListUpdate = this.MatchmakingCallbacks.FriendListUpdate;
			if (friendListUpdate != null)
			{
				friendListUpdate(friendList);
			}
		}

		public void OnJoinedRoom()
		{
			bool flag = this.MatchmakingCallbacks.JoinedRoom != null;
			if (flag)
			{
				this.MatchmakingCallbacks.JoinedRoom();
			}
			else
			{
				this.SetResult(0);
			}
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			bool flag = this.MatchmakingCallbacks.JoinRoomRandomFailed != null;
			if (flag)
			{
				this.MatchmakingCallbacks.JoinRoomRandomFailed(returnCode, message);
			}
			else
			{
				bool throwOnErrors = this._throwOnErrors;
				if (throwOnErrors)
				{
					this.SetException(new OperationException(returnCode, message));
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(message);
					}
					this.SetResult(returnCode);
				}
			}
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			bool flag = this.MatchmakingCallbacks.JoinRoomFailed != null;
			if (flag)
			{
				this.MatchmakingCallbacks.JoinRoomFailed(returnCode, message);
			}
			else
			{
				bool throwOnErrors = this._throwOnErrors;
				if (throwOnErrors)
				{
					this.SetException(new OperationException(returnCode, message));
				}
				else
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(message);
					}
					this.SetResult(returnCode);
				}
			}
		}

		public void OnLeftRoom()
		{
			bool flag = this.MatchmakingCallbacks.LeftRoom != null;
			if (flag)
			{
				this.MatchmakingCallbacks.LeftRoom();
			}
			else
			{
				this.SetResult(0);
			}
		}

		public void OnJoinedLobby()
		{
			bool flag = this.LobbyCallbacks.JoinedLobby != null;
			if (flag)
			{
				this.LobbyCallbacks.JoinedLobby();
			}
			else
			{
				this.SetResult(0);
			}
		}

		public void OnLeftLobby()
		{
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		public PhotonConnectionCallbacks ConnectionCallbacks = new PhotonConnectionCallbacks();

		public PhotonMatchmakingCallbacks MatchmakingCallbacks = new PhotonMatchmakingCallbacks();

		public PhotonLobbyCallbacks LobbyCallbacks = new PhotonLobbyCallbacks();

		private bool _throwOnErrors;

		private TaskCompletionSource<short> _result;

		private CancellationTokenSource _cancellation;

		private const float OPERATION_TIMEOUT_SEC = 30f;
	}
}
