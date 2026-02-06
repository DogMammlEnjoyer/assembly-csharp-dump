using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	[DisallowMultipleComponent]
	[AddComponentMenu("")]
	internal class SupportLogger : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks, IErrorInfoCallback
	{
		public LoadBalancingClient Client
		{
			get
			{
				return this.client;
			}
			set
			{
				bool flag = this.client != value;
				if (flag)
				{
					bool flag2 = this.client != null;
					if (flag2)
					{
						this.client.RemoveCallbackTarget(this);
					}
					this.client = value;
					bool flag3 = this.client != null;
					if (flag3)
					{
						this.client.AddCallbackTarget(this);
					}
				}
			}
		}

		protected void Start()
		{
			this.LogBasics();
			bool flag = this.startStopwatch == null;
			if (flag)
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
			bool flag = !this.initialOnApplicationPauseSkipped;
			if (flag)
			{
				this.initialOnApplicationPauseSkipped = true;
			}
			else
			{
				Debug_.Log(string.Format("{0} SupportLogger OnApplicationPause({1}). Client: {2}.", this.GetFormattedTimestamp(), pause, (this.client == null) ? "null" : this.client.State.ToString()));
			}
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
			bool flag = this.startStopwatch == null;
			if (flag)
			{
				this.startStopwatch = new Stopwatch();
				this.startStopwatch.Start();
			}
			TimeSpan elapsed = this.startStopwatch.Elapsed;
			bool flag2 = elapsed.Minutes > 0;
			string result;
			if (flag2)
			{
				result = string.Format("[{0}:{1}.{1}]", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
			}
			else
			{
				result = string.Format("[{0}.{1}]", elapsed.Seconds, elapsed.Milliseconds);
			}
			return result;
		}

		private void TrackValues()
		{
			bool flag = this.client != null;
			if (flag)
			{
				int roundTripTime = this.client.LoadBalancingPeer.RoundTripTime;
				bool flag2 = roundTripTime > this.pingMax;
				if (flag2)
				{
					this.pingMax = roundTripTime;
				}
				bool flag3 = roundTripTime < this.pingMin;
				if (flag3)
				{
					this.pingMin = roundTripTime;
				}
			}
		}

		public void LogStats()
		{
			bool flag = this.client == null || this.client.State == ClientState.PeerCreated;
			if (!flag)
			{
				bool logTrafficStats = this.LogTrafficStats;
				if (logTrafficStats)
				{
					Debug_.Log(string.Format("{0} SupportLogger {1} Ping min/max: {2}/{3}", new object[]
					{
						this.GetFormattedTimestamp(),
						this.client.LoadBalancingPeer.VitalStatsToString(false),
						this.pingMin,
						this.pingMax
					}));
				}
			}
		}

		private void LogBasics()
		{
			bool flag = this.client != null;
			if (flag)
			{
				List<string> list = new List<string>(10);
				list.Add(Application.unityVersion);
				list.Add(Application.platform.ToString());
				bool isENABLE_IL2CPP = RuntimeUnityFlagsSetup.IsENABLE_IL2CPP;
				if (isENABLE_IL2CPP)
				{
					list.Add("ENABLE_IL2CPP");
				}
				bool isENABLE_MONO = RuntimeUnityFlagsSetup.IsENABLE_MONO;
				if (isENABLE_MONO)
				{
					list.Add("ENABLE_MONO");
				}
				list.Add("DEBUG");
				bool isNET_4_ = RuntimeUnityFlagsSetup.IsNET_4_6;
				if (isNET_4_)
				{
					list.Add("NET_4_6");
				}
				bool isNET_STANDARD_2_ = RuntimeUnityFlagsSetup.IsNET_STANDARD_2_0;
				if (isNET_STANDARD_2_)
				{
					list.Add("NET_STANDARD_2_0");
				}
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
				bool flag2 = this.client != null && this.client.LoadBalancingPeer != null && this.client.LoadBalancingPeer.SocketImplementation != null;
				if (flag2)
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
				stringBuilder.AppendFormat("{0} UTC", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
				Debug_.LogWarning(stringBuilder.ToString());
			}
		}

		public void OnConnected()
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnected().");
			this.pingMax = 0;
			this.pingMin = this.client.LoadBalancingPeer.RoundTripTime;
			this.LogBasics();
			bool logTrafficStats = this.LogTrafficStats;
			if (logTrafficStats)
			{
				this.client.LoadBalancingPeer.TrafficStatsEnabled = false;
				this.client.LoadBalancingPeer.TrafficStatsEnabled = true;
				this.StartLogStats();
			}
			this.StartTrackValues();
		}

		public void OnConnectedToMaster()
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnConnectedToMaster().");
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnFriendListUpdate(friendList).");
		}

		public void OnJoinedLobby()
		{
			string formattedTimestamp = this.GetFormattedTimestamp();
			string str = " SupportLogger OnJoinedLobby(";
			TypedLobby currentLobby = this.client.CurrentLobby;
			Debug_.Log(formattedTimestamp + str + ((currentLobby != null) ? currentLobby.ToString() : null) + ").");
		}

		public void OnLeftLobby()
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftLobby().");
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			Debug_.Log(string.Concat(new string[]
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
			Debug_.Log(string.Concat(array));
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug_.Log(string.Concat(new string[]
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
			Debug_.Log(string.Concat(new string[]
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
			Debug_.Log(string.Concat(array));
		}

		public void OnLeftRoom()
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnLeftRoom().");
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			this.StopLogStats();
			this.StopTrackValues();
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnDisconnected(" + cause.ToString() + ").");
			this.LogBasics();
			this.LogStats();
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnRegionListReceived(regionHandler).");
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomListUpdate(roomList). roomList.Count: " + roomList.Count.ToString());
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerEnteredRoom(" + ((newPlayer != null) ? newPlayer.ToString() : null) + ").");
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerLeftRoom(" + ((otherPlayer != null) ? otherPlayer.ToString() : null) + ").");
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnRoomPropertiesUpdate(propertiesThatChanged).");
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnPlayerPropertiesUpdate(targetPlayer,changedProps).");
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnMasterClientSwitched(" + ((newMasterClient != null) ? newMasterClient.ToString() : null) + ").");
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationResponse(" + data.ToStringFull() + ").");
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnCustomAuthenticationFailed(" + debugMessage + ").");
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
			Debug_.Log(this.GetFormattedTimestamp() + " SupportLogger OnLobbyStatisticsUpdate(lobbyStatistics).");
		}

		public void OnErrorInfo(ErrorInfo errorInfo)
		{
			Debug_.LogError(errorInfo.ToString());
		}

		public bool LogTrafficStats = true;

		private LoadBalancingClient client;

		private Stopwatch startStopwatch;

		private bool initialOnApplicationPauseSkipped = false;

		private int pingMax;

		private int pingMin;
	}
}
