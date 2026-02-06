using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	[RequireComponent(typeof(VoiceConnection))]
	public class ConnectAndJoin : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks
	{
		public bool IsConnected
		{
			get
			{
				return this.voiceConnection.Client.IsConnected;
			}
		}

		private void Awake()
		{
			this.voiceConnection = base.GetComponent<VoiceConnection>();
		}

		private void OnEnable()
		{
			this.voiceConnection.Client.AddCallbackTarget(this);
			if (this.autoConnect)
			{
				this.ConnectNow();
			}
		}

		private void OnDisable()
		{
			this.voiceConnection.Client.RemoveCallbackTarget(this);
		}

		public void ConnectNow()
		{
			this.voiceConnection.ConnectUsingSettings(null);
		}

		public void OnCreatedRoom()
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			Debug.LogErrorFormat("OnCreateRoomFailed errorCode={0} errorMessage={1}", new object[]
			{
				returnCode,
				message
			});
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public void OnJoinedRoom()
		{
			if (this.voiceConnection.PrimaryRecorder == null)
			{
				this.voiceConnection.PrimaryRecorder = base.gameObject.AddComponent<Recorder>();
			}
			if (this.autoTransmit)
			{
				this.voiceConnection.PrimaryRecorder.TransmitEnabled = this.autoTransmit;
			}
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.LogErrorFormat("OnJoinRandomFailed errorCode={0} errorMessage={1}", new object[]
			{
				returnCode,
				message
			});
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.LogErrorFormat("OnJoinRoomFailed roomName={0} errorCode={1} errorMessage={2}", new object[]
			{
				this.RoomName,
				returnCode,
				message
			});
		}

		public void OnLeftRoom()
		{
		}

		public void OnPreLeavingRoom()
		{
		}

		public void OnConnected()
		{
		}

		public void OnConnectedToMaster()
		{
			this.enterRoomParams.RoomOptions.PublishUserId = this.publishUserId;
			if (this.RandomRoom)
			{
				this.enterRoomParams.RoomName = null;
				this.voiceConnection.Client.OpJoinRandomOrCreateRoom(new OpJoinRandomRoomParams(), this.enterRoomParams);
				return;
			}
			this.enterRoomParams.RoomName = this.RoomName;
			this.voiceConnection.Client.OpJoinOrCreateRoom(this.enterRoomParams);
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			if (cause == DisconnectCause.None || cause == DisconnectCause.DisconnectByClientLogic)
			{
				return;
			}
			Debug.LogErrorFormat("OnDisconnected cause={0}", new object[]
			{
				cause
			});
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		private VoiceConnection voiceConnection;

		public bool RandomRoom = true;

		[SerializeField]
		private bool autoConnect = true;

		[SerializeField]
		private bool autoTransmit = true;

		[SerializeField]
		private bool publishUserId;

		public string RoomName;

		private readonly EnterRoomParams enterRoomParams = new EnterRoomParams
		{
			RoomOptions = new RoomOptions()
		};
	}
}
