using System;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.UtilityScripts
{
	public class StatesGui : MonoBehaviour
	{
		private void Awake()
		{
			if (StatesGui.Instance != null)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
			if (this.DontDestroy)
			{
				StatesGui.Instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
			if (this.EventsIn)
			{
				PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
			}
		}

		private void OnDisable()
		{
			if (this.DontDestroy && StatesGui.Instance == this)
			{
				StatesGui.Instance = null;
			}
		}

		private void OnGUI()
		{
			if (PhotonNetwork.NetworkingClient == null || PhotonNetwork.NetworkingClient.LoadBalancingPeer == null || PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsIncoming == null)
			{
				return;
			}
			float x = (float)Screen.width / this.native_width;
			float y = (float)Screen.height / this.native_height;
			GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, new Vector3(x, y, 1f));
			Rect rect = new Rect(this.GuiOffset);
			if (rect.x < 0f)
			{
				rect.x = (float)Screen.width - rect.width;
			}
			this.GuiRect.xMin = rect.x;
			this.GuiRect.yMin = rect.y;
			this.GuiRect.xMax = rect.x + rect.width;
			this.GuiRect.yMax = rect.y + rect.height;
			GUILayout.BeginArea(this.GuiRect);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (this.ServerTimestamp)
			{
				GUILayout.Label(((double)PhotonNetwork.ServerTimestamp / 1000.0).ToString("F3"), Array.Empty<GUILayoutOption>());
			}
			if (this.Server)
			{
				GUILayout.Label(PhotonNetwork.ServerAddress + " " + PhotonNetwork.Server.ToString(), Array.Empty<GUILayoutOption>());
			}
			if (this.DetailedConnection)
			{
				GUILayout.Label(PhotonNetwork.NetworkClientState.ToString(), Array.Empty<GUILayoutOption>());
			}
			if (this.AppVersion)
			{
				GUILayout.Label(PhotonNetwork.NetworkingClient.AppVersion, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (this.UserId)
			{
				GUILayout.Label("UID: " + ((PhotonNetwork.AuthValues != null) ? PhotonNetwork.AuthValues.UserId : "no UserId"), Array.Empty<GUILayoutOption>());
				GUILayout.Label("UserId:" + PhotonNetwork.LocalPlayer.UserId, Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndHorizontal();
			if (this.Room)
			{
				if (PhotonNetwork.InRoom)
				{
					GUILayout.Label(this.RoomProps ? PhotonNetwork.CurrentRoom.ToStringFull() : PhotonNetwork.CurrentRoom.ToString(), Array.Empty<GUILayoutOption>());
				}
				else
				{
					GUILayout.Label("not in room", Array.Empty<GUILayoutOption>());
				}
			}
			if (this.EventsIn)
			{
				int fragmentCommandCount = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsIncoming.FragmentCommandCount;
				GUILayout.Label("Events Received: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsGameLevel.EventCount.ToString() + " Fragments: " + fragmentCommandCount.ToString(), Array.Empty<GUILayoutOption>());
			}
			if (this.LocalPlayer)
			{
				GUILayout.Label(this.PlayerToString(PhotonNetwork.LocalPlayer), Array.Empty<GUILayoutOption>());
			}
			if (this.Others)
			{
				foreach (Player player in PhotonNetwork.PlayerListOthers)
				{
					GUILayout.Label(this.PlayerToString(player), Array.Empty<GUILayoutOption>());
				}
			}
			if (this.ExpectedUsers && PhotonNetwork.InRoom)
			{
				GUILayout.Label("Expected: " + ((PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? PhotonNetwork.CurrentRoom.ExpectedUsers.Length : 0).ToString() + " " + ((PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? string.Join(",", PhotonNetwork.CurrentRoom.ExpectedUsers) : ""), Array.Empty<GUILayoutOption>());
			}
			if (this.Buttons)
			{
				if (!PhotonNetwork.IsConnected && GUILayout.Button("Connect", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.ConnectUsingSettings();
				}
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (PhotonNetwork.IsConnected && GUILayout.Button("Disconnect", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.Disconnect();
				}
				if (PhotonNetwork.IsConnected && GUILayout.Button("Close Socket", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.NetworkingClient.LoadBalancingPeer.StopThread();
				}
				GUILayout.EndHorizontal();
				if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && GUILayout.Button("Leave", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.LeaveRoom(true);
				}
				if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerTtl > 0 && GUILayout.Button("Leave(abandon)", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.LeaveRoom(false);
				}
				if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Join Random", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.JoinRandomRoom();
				}
				if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Create Room", Array.Empty<GUILayoutOption>()))
				{
					PhotonNetwork.CreateRoom(null, null, null, null);
				}
			}
			GUILayout.EndArea();
		}

		private string PlayerToString(Player player)
		{
			if (PhotonNetwork.NetworkingClient == null)
			{
				Debug.LogError("nwp is null");
				return "";
			}
			return string.Format("#{0:00} '{1}'{5} {4}{2} {3} {6}", new object[]
			{
				player.ActorNumber.ToString() + "/userId:<" + player.UserId + ">",
				player.NickName,
				player.IsMasterClient ? "(master)" : "",
				this.PlayerProps ? player.CustomProperties.ToStringFull() : "",
				(PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber) ? "(you)" : "",
				player.UserId,
				player.IsInactive ? " / Is Inactive" : ""
			});
		}

		public Rect GuiOffset = new Rect(250f, 0f, 300f, 300f);

		public bool DontDestroy = true;

		public bool ServerTimestamp;

		public bool DetailedConnection;

		public bool Server;

		public bool AppVersion;

		public bool UserId;

		public bool Room;

		public bool RoomProps;

		public bool EventsIn;

		public bool LocalPlayer;

		public bool PlayerProps;

		public bool Others;

		public bool Buttons;

		public bool ExpectedUsers;

		private Rect GuiRect;

		private static StatesGui Instance;

		private float native_width = 800f;

		private float native_height = 480f;
	}
}
