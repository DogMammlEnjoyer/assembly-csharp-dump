using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Realtime
{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	public class SupportLogger : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks, IErrorInfoCallback
	{
		public LoadBalancingClient Client
		{
			get
			{
				return this.client;
			}
			set
			{
				if (this.client != value)
				{
					if (this.client != null)
					{
						this.client.RemoveCallbackTarget(this);
					}
					this.client = value;
					if (this.client != null)
					{
						this.client.AddCallbackTarget(this);
					}
				}
			}
		}

		protected void Start()
		{
			this.LogBasics();
			if (this.startStopwatch == null)
			{
				this.startStopwatch = new Stopwatch();
				this.startStopwatch.Start();
			}
		}

		protected void OnDestroy()
		{
			this.Client = null;
		}

		protected void OnApplicationPause(bool pause)
		{
			if (!this.initialOnApplicationPauseSkipped)
			{
				this.initialOnApplicationPauseSkipped = true;
				return;
			}
			Debug.Log(string.Format("{0} SupportLogger OnApplicationPause({1}). Client: {2}.", this.GetFormattedTimestamp(), pause, (this.client == null) ? "null" : this.client.State.ToString()));
		}

		protected void OnApplicationQuit()
		{
			base.CancelInvoke();
		}

		public void StartLogStats()
		{
			base.InvokeRepeating("LogStats", 10f, 10f);
		}

		public void StopLogStats()
		{
			base.CancelInvoke("LogStats");
		}

		private void StartTrackValues()
		{
			base.InvokeRepeating("TrackValues", 0.5f, 0.5f);
		}

		private void StopTrackValues()
		{
			base.CancelInvoke("TrackValues");
		}

		private string GetFormattedTimestamp()
		{
			if (this.startStopwatch == null)
			{
				this.startStopwatch = new Stopwatch();
				this.startStopwatch.Start();
			}
			TimeSpan elapsed = this.startStopwatch.Elapsed;
			if (elapsed.Minutes > 0)
			{
				return string.Format("[{0}:{1}.{1}]", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
			}
			return string.Format("[{0}.{1}]", elapsed.Seconds, elapsed.Milliseconds);
		}

		private void TrackValues()
		{
			if (this.client != null)
			{
				int roundTripTime = this.client.LoadBalancingPeer.RoundTripTime;
				if (roundTripTime > this.pingMax)
				{
					this.pingMax = roundTripTime;
				}
				if (roundTripTime < this.pingMin)
				{
					this.pingMin = roundTripTime;
				}
			}
		}

		public void LogStats()
		{
			if (this.client == null || this.client.State == ClientState.PeerCreated)
			{
				return;
			}
			if (this.LogTrafficStats)
			{
				Debug.Log(string.Format("{0} SupportLogger {1} Ping min/max: {2}/{3}", new object[]
				{
					this.GetFormattedTimestamp(),
					this.client.LoadBalancingPeer.VitalStatsToString(false),
					this.pingMin,
					this.pingMax
				}));
			}
		}

		private void LogBasics()
		{
			if (this.client != null)
			{
				List<string> list = new List<string>(10);
				list.Add(Application.unityVersion);
				list.Add(Application.platform.ToString());
				list.Add("ENABLE_MONO");
				list.Add("NET_STANDARD_2_0");
				list.Add("UNITY_64");
				StringBuilder stringBuilder = new StringBuilder();
				string text = (string.IsNullOrEmpty(this.client.AppId) || this.client.AppId.Length < 8) ? this.client.AppId : (this.client.AppId.Substring(0, 8) + "***");
				stringBuilder.AppendFormat("{0} SupportLogger Info: ", this.GetFormattedTimestamp());
				stringBuilder.AppendFormat("AppID: \"{0}\" AppVersion: \"{1}\" Client: v{2} ({4}) Build: {3} ", new object[]
				{
					text,
					this.client.AppVersion,
					PhotonPeer.Version,
					string.Join(", ", list.ToArray()),
					this.client.LoadBalancingPeer.TargetFramework
				});
				if (this.client != null && this.client.LoadBalancingPeer != null && this.client.LoadBalancingPeer.SocketImplementation != null)
				{
					stringBuilder.AppendFormat("Socket: {0} ", this.client.LoadBalancingPeer.SocketImplementation.Name);
				}
				stringBuilder.AppendFormat("UserId: \"{0}\" AuthType: {1} AuthMode: {2} {3} ", new object[]
				{
					this.client.UserId,
					(this.client.AuthValues != null) ? this.client.AuthValues.AuthType.ToString() : "N/A",
					this.client.AuthMode,
					this.client.EncryptionMode
				});
				stringBuilder.AppendFormat("State: {0} ", this.client.State);
				stringBuilder.AppendFormat("PeerID: {0} ", this.client.LoadBalancingPeer.PeerID);
				stringBuilder.AppendFormat("NameServer: {0} Current Server: {1} IP: {2} Region: {3} ", new object[]
				{
					this.client.NameServerHost,
					this.client.CurrentServerAddress,
					this.client.LoadBalancingPeer.ServerIpAddress,
					this.client.CloudRegion
				});
				Debug.LogWarning(stringBuilder.ToString());
			}
		}

		public void OnConnected()
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnected().");
			this.pingMax = 0;
			this.pingMin = this.client.LoadBalancingPeer.RoundTripTime;
			this.LogBasics();
			if (this.LogTrafficStats)
			{
				this.client.LoadBalancingPeer.TrafficStatsEnabled = false;
				this.client.LoadBalancingPeer.TrafficStatsEnabled = true;
				this.StartLogStats();
			}
			this.StartTrackValues();
		}

		public void OnConnectedToMaster()
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnectedToMaster().");
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnFriendListUpdate(friendList).");
		}

		public void OnJoinedLobby()
		{
			string formattedTimestamp = this.GetFormattedTimestamp();
			string str = " SupportLogger OnJoinedLobby(";
			TypedLobby currentLobby = this.client.CurrentLobby;
			Debug.Log(formattedTimestamp + str + ((currentLobby != null) ? currentLobby.ToString() : null) + ").");
		}

		public void OnLeftLobby()
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftLobby().");
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			Debug.Log(string.Concat(new string[]
			{
				this.GetFormattedTimestamp(),
				" SupportLogger OnCreateRoomFailed(",
				returnCode.ToString(),
				",",
				message,
				")."
			}));
		}

		public void OnJoinedRoom()
		{
			string[] array = new string[7];
			array[0] = this.GetFormattedTimestamp();
			array[1] = " SupportLogger OnJoinedRoom(";
			int num = 2;
			Room currentRoom = this.client.CurrentRoom;
			array[num] = ((currentRoom != null) ? currentRoom.ToString() : null);
			array[3] = "). ";
			int num2 = 4;
			TypedLobby currentLobby = this.client.CurrentLobby;
			array[num2] = ((currentLobby != null) ? currentLobby.ToString() : null);
			array[5] = " GameServer:";
			array[6] = this.client.GameServerAddress;
			Debug.Log(string.Concat(array));
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log(string.Concat(new string[]
			{
				this.GetFormattedTimestamp(),
				" SupportLogger OnJoinRoomFailed(",
				returnCode.ToString(),
				",",
				message,
				")."
			}));
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log(string.Concat(new string[]
			{
				this.GetFormattedTimestamp(),
				" SupportLogger OnJoinRandomFailed(",
				returnCode.ToString(),
				",",
				message,
				")."
			}));
		}

		public void OnCreatedRoom()
		{
			string[] array = new string[7];
			array[0] = this.GetFormattedTimestamp();
			array[1] = " SupportLogger OnCreatedRoom(";
			int num = 2;
			Room currentRoom = this.client.CurrentRoom;
			array[num] = ((currentRoom != null) ? currentRoom.ToString() : null);
			array[3] = "). ";
			int num2 = 4;
			TypedLobby currentLobby = this.client.CurrentLobby;
			array[num2] = ((currentLobby != null) ? currentLobby.ToString() : null);
			array[5] = " GameServer:";
			array[6] = this.client.GameServerAddress;
			Debug.Log(string.Concat(array));
		}

		public void OnLeftRoom()
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftRoom().");
		}

		public void OnPreLeavingRoom()
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPreLeavingRoom()");
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			this.StopLogStats();
			this.StopTrackValues();
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnDisconnected(" + cause.ToString() + ").");
			this.LogBasics();
			this.LogStats();
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRegionListReceived(regionHandler).");
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count.ToString());
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerEnteredRoom(" + ((newPlayer != null) ? newPlayer.ToString() : null) + ").");
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerLeftRoom(" + ((otherPlayer != null) ? otherPlayer.ToString() : null) + ").");
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnMasterClientSwitched(" + ((newMasterClient != null) ? newMasterClient.ToString() : null) + ").");
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationResponse(" + data.ToStringFull() + ").");
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationFailed(" + debugMessage + ").");
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
			Debug.Log(this.GetFormattedTimestamp() + " SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
		}

		public void OnErrorInfo(ErrorInfo errorInfo)
		{
			Debug.LogError(errorInfo.ToString());
		}

		public bool LogTrafficStats = true;

		private bool loggedStillOfflineMessage;

		private LoadBalancingClient client;

		private Stopwatch startStopwatch;

		private bool initialOnApplicationPauseSkipped;

		private int pingMax;

		private int pingMin;
	}
}
