using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.Pun
{
	public class PhotonHandler : ConnectionHandler, IInRoomCallbacks, IMatchmakingCallbacks
	{
		internal static PhotonHandler Instance
		{
			get
			{
				if (PhotonHandler.instance == null)
				{
					PhotonHandler.instance = Object.FindObjectOfType<PhotonHandler>();
					if (PhotonHandler.instance == null)
					{
						PhotonHandler.instance = new GameObject
						{
							name = "PhotonMono"
						}.AddComponent<PhotonHandler>();
					}
				}
				return PhotonHandler.instance;
			}
		}

		protected override void Awake()
		{
			if (PhotonHandler.instance == null || this == PhotonHandler.instance)
			{
				PhotonHandler.instance = this;
				base.Awake();
				return;
			}
			Object.Destroy(this);
		}

		protected virtual void OnEnable()
		{
			if (PhotonHandler.Instance != this)
			{
				Debug.LogError("PhotonHandler is a singleton but there are multiple instances. this != Instance.");
				return;
			}
			base.Client = PhotonNetwork.NetworkingClient;
			if (PhotonNetwork.PhotonServerSettings.EnableSupportLogger)
			{
				SupportLogger supportLogger = base.gameObject.GetComponent<SupportLogger>();
				if (supportLogger == null)
				{
					supportLogger = base.gameObject.AddComponent<SupportLogger>();
				}
				if (this.supportLoggerComponent != null && supportLogger.GetInstanceID() != this.supportLoggerComponent.GetInstanceID())
				{
					Debug.LogWarningFormat("Cached SupportLogger component is different from the one attached to PhotonMono GameObject", Array.Empty<object>());
				}
				this.supportLoggerComponent = supportLogger;
				this.supportLoggerComponent.Client = PhotonNetwork.NetworkingClient;
			}
			this.UpdateInterval = 1000 / PhotonNetwork.SendRate;
			this.UpdateIntervalOnSerialize = 1000 / PhotonNetwork.SerializationRate;
			PhotonNetwork.AddCallbackTarget(this);
			base.StartFallbackSendAckThread();
		}

		protected void Start()
		{
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode loadingMode)
			{
			};
		}

		protected override void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			base.OnDisable();
		}

		protected void FixedUpdate()
		{
			if (Time.timeScale > PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate)
			{
				this.Dispatch();
			}
		}

		protected void LateUpdate()
		{
			if (Time.timeScale <= PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate)
			{
				this.Dispatch();
			}
			int num = (int)(Time.realtimeSinceStartup * 1000f);
			if (PhotonNetwork.IsMessageQueueRunning && num > this.nextSendTickCountOnSerialize)
			{
				PhotonNetwork.RunViewUpdate();
				this.nextSendTickCountOnSerialize = num + this.UpdateIntervalOnSerialize - 8;
				this.nextSendTickCount = 0;
			}
			num = (int)(Time.realtimeSinceStartup * 1000f);
			if (PhotonHandler.SendAsap || num > this.nextSendTickCount)
			{
				PhotonHandler.SendAsap = false;
				bool flag = true;
				int num2 = 0;
				while (PhotonNetwork.IsMessageQueueRunning && flag && num2 < PhotonHandler.MaxDatagrams)
				{
					flag = PhotonNetwork.NetworkingClient.LoadBalancingPeer.SendOutgoingCommands();
					num2++;
				}
				this.nextSendTickCount = num + this.UpdateInterval;
			}
		}

		protected void Dispatch()
		{
			if (PhotonNetwork.NetworkingClient == null)
			{
				Debug.LogError("NetworkPeer broke!");
				return;
			}
			bool flag = true;
			Exception ex = null;
			int num = 0;
			while (PhotonNetwork.IsMessageQueueRunning && flag)
			{
				try
				{
					flag = PhotonNetwork.NetworkingClient.LoadBalancingPeer.DispatchIncomingCommands();
				}
				catch (Exception ex2)
				{
					num++;
					if (ex == null)
					{
						ex = ex2;
					}
				}
			}
			if (ex != null)
			{
				throw new AggregateException("Caught " + num.ToString() + " exception(s) in methods called by DispatchIncomingCommands(). Rethrowing first only (see above).", ex);
			}
		}

		public void OnCreatedRoom()
		{
			PhotonNetwork.SetLevelInPropsIfSynced(SceneManagerHelper.ActiveSceneName);
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			PhotonNetwork.LoadLevelIfSynced();
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
			{
				if (photonView.IsRoomView)
				{
					photonView.OwnerActorNr = newMasterClient.ActorNumber;
					photonView.ControllerActorNr = newMasterClient.ActorNumber;
				}
			}
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		public void OnJoinedRoom()
		{
			if (PhotonNetwork.ViewCount == 0)
			{
				return;
			}
			foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
			{
				photonView.RebuildControllerCache(false);
			}
		}

		public void OnLeftRoom()
		{
			PhotonNetwork.LocalCleanupAnythingInstantiated(true);
		}

		public void OnPreLeavingRoom()
		{
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			NonAllocDictionary<int, PhotonView>.ValueIterator photonViewCollection = PhotonNetwork.PhotonViewCollection;
			int actorNumber = otherPlayer.ActorNumber;
			if (otherPlayer.IsInactive)
			{
				using (NonAllocDictionary<int, PhotonView>.ValueIterator enumerator = photonViewCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PhotonView photonView = enumerator.Current;
						if (photonView.ControllerActorNr == actorNumber)
						{
							photonView.ControllerActorNr = PhotonNetwork.MasterClient.ActorNumber;
						}
					}
					return;
				}
			}
			bool autoCleanUp = PhotonNetwork.CurrentRoom.AutoCleanUp;
			foreach (PhotonView photonView2 in photonViewCollection)
			{
				if ((!autoCleanUp || photonView2.CreatorActorNr != actorNumber) && (photonView2.OwnerActorNr == actorNumber || photonView2.ControllerActorNr == actorNumber))
				{
					photonView2.OwnerActorNr = 0;
					photonView2.ControllerActorNr = PhotonNetwork.MasterClient.ActorNumber;
				}
			}
		}

		private static PhotonHandler instance;

		public static int MaxDatagrams = 3;

		public static bool SendAsap;

		private const int SerializeRateFrameCorrection = 8;

		protected internal int UpdateInterval;

		protected internal int UpdateIntervalOnSerialize;

		private int nextSendTickCount;

		private int nextSendTickCountOnSerialize;

		private SupportLogger supportLoggerComponent;

		protected List<int> reusableIntList = new List<int>();
	}
}
