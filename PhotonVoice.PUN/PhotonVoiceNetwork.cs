using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

namespace Photon.Voice.PUN
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Photon Voice/Photon Voice Network")]
	[HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
	public class PhotonVoiceNetwork : VoiceConnection
	{
		public static PhotonVoiceNetwork Instance
		{
			get
			{
				object obj = PhotonVoiceNetwork.instanceLock;
				PhotonVoiceNetwork result;
				lock (obj)
				{
					if (ConnectionHandler.AppQuits)
					{
						if (PhotonVoiceNetwork.instance.Logger.IsWarningEnabled)
						{
							PhotonVoiceNetwork.instance.Logger.LogWarning("PhotonVoiceNetwork Instance already destroyed on application quit. Won't create again - returning null.", Array.Empty<object>());
						}
						result = null;
					}
					else
					{
						if (!PhotonVoiceNetwork.instantiated)
						{
							PhotonVoiceNetwork[] array = Object.FindObjectsOfType<PhotonVoiceNetwork>();
							if (array == null || array.Length < 1)
							{
								PhotonVoiceNetwork.instance = new GameObject
								{
									name = "PhotonVoiceNetwork singleton"
								}.AddComponent<PhotonVoiceNetwork>();
								if (PhotonVoiceNetwork.instance.Logger.IsInfoEnabled)
								{
									PhotonVoiceNetwork.instance.Logger.LogInfo("An instance of PhotonVoiceNetwork was automatically created in the scene.", Array.Empty<object>());
								}
							}
							else if (array.Length >= 1)
							{
								PhotonVoiceNetwork.instance = array[0];
								if (array.Length > 1)
								{
									if (PhotonVoiceNetwork.instance.Logger.IsErrorEnabled)
									{
										PhotonVoiceNetwork.instance.Logger.LogError("{0} PhotonVoiceNetwork instances found. Using first one only and destroying all the other extra instances.", new object[]
										{
											array.Length
										});
									}
									for (int i = 1; i < array.Length; i++)
									{
										Object.Destroy(array[i]);
									}
								}
							}
							PhotonVoiceNetwork.instantiated = true;
							if (PhotonVoiceNetwork.instance.Logger.IsDebugEnabled)
							{
								PhotonVoiceNetwork.instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is now set.", Array.Empty<object>());
							}
						}
						result = PhotonVoiceNetwork.instance;
					}
				}
				return result;
			}
			set
			{
				object obj = PhotonVoiceNetwork.instanceLock;
				lock (obj)
				{
					if (value == null || !value)
					{
						if (PhotonVoiceNetwork.instantiated)
						{
							if (PhotonVoiceNetwork.instance.Logger.IsErrorEnabled)
							{
								PhotonVoiceNetwork.instance.Logger.LogError("Cannot set PhotonVoiceNetwork.Instance to null or destroyed.", Array.Empty<object>());
							}
						}
						else
						{
							Debug.LogError("Cannot set PhotonVoiceNetwork.Instance to null or destroyed.");
						}
					}
					else if (PhotonVoiceNetwork.instantiated)
					{
						if (PhotonVoiceNetwork.instance.GetInstanceID() != value.GetInstanceID())
						{
							if (PhotonVoiceNetwork.instance.Logger.IsErrorEnabled)
							{
								PhotonVoiceNetwork.instance.Logger.LogError("An instance of PhotonVoiceNetwork is already set. Destroying extra instance.", Array.Empty<object>());
							}
							Object.Destroy(value);
						}
					}
					else
					{
						PhotonVoiceNetwork.instance = value;
						PhotonVoiceNetwork.instantiated = true;
						if (PhotonVoiceNetwork.instance.Logger.IsDebugEnabled)
						{
							PhotonVoiceNetwork.instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is now set.", Array.Empty<object>());
						}
					}
				}
			}
		}

		public bool UsePunAuthValues
		{
			get
			{
				return this.usePunAuthValues;
			}
			set
			{
				this.usePunAuthValues = value;
			}
		}

		public bool ConnectAndJoinRoom()
		{
			if (!PhotonNetwork.InRoom)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Cannot connect and join if PUN is not joined.", Array.Empty<object>());
				}
				return false;
			}
			if (this.Connect())
			{
				this.clientCalledConnectAndJoin = true;
				this.clientCalledDisconnect = false;
				return true;
			}
			if (base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Connecting to server failed.", Array.Empty<object>());
			}
			return false;
		}

		public void Disconnect()
		{
			if (!base.Client.IsConnected)
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Cannot Disconnect if not connected.", Array.Empty<object>());
				}
				return;
			}
			this.clientCalledDisconnect = true;
			this.clientCalledConnectAndJoin = false;
			this.clientCalledConnectOnly = false;
			base.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
		}

		protected override void Awake()
		{
			PhotonVoiceNetwork.Instance = this;
			object obj = PhotonVoiceNetwork.instanceLock;
			lock (obj)
			{
				if (PhotonVoiceNetwork.instantiated && PhotonVoiceNetwork.instance.GetInstanceID() == base.GetInstanceID())
				{
					base.Awake();
				}
			}
		}

		private void OnEnable()
		{
			PhotonNetwork.NetworkingClient.StateChanged += this.OnPunStateChanged;
			this.FollowPun();
			this.clientCalledConnectAndJoin = false;
			this.clientCalledConnectOnly = false;
			this.clientCalledDisconnect = false;
			this.internalDisconnect = false;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			PhotonNetwork.NetworkingClient.StateChanged -= this.OnPunStateChanged;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			object obj = PhotonVoiceNetwork.instanceLock;
			lock (obj)
			{
				if (PhotonVoiceNetwork.instantiated && PhotonVoiceNetwork.instance.GetInstanceID() == base.GetInstanceID())
				{
					PhotonVoiceNetwork.instantiated = false;
					if (PhotonVoiceNetwork.instance.Logger.IsDebugEnabled)
					{
						PhotonVoiceNetwork.instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is being reset because destroyed.", Array.Empty<object>());
					}
					PhotonVoiceNetwork.instance = null;
				}
			}
		}

		private void OnPunStateChanged(ClientState fromState, ClientState toState)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnPunStateChanged from {0} to {1}", new object[]
				{
					fromState,
					toState
				});
			}
			this.FollowPun(toState);
		}

		protected override void OnVoiceStateChanged(ClientState fromState, ClientState toState)
		{
			base.OnVoiceStateChanged(fromState, toState);
			if (toState == ClientState.Disconnected)
			{
				if (this.internalDisconnect)
				{
					this.internalDisconnect = false;
				}
				else if (!this.clientCalledDisconnect)
				{
					this.clientCalledDisconnect = (base.Client.DisconnectedCause == DisconnectCause.DisconnectByClientLogic);
				}
				if (base.PrimaryRecorder != null && base.PrimaryRecorder)
				{
					base.PrimaryRecorder.UserData = -1;
				}
			}
			else if (toState == ClientState.ConnectedToMasterServer)
			{
				if (this.internalConnect)
				{
					this.internalConnect = false;
				}
				else if (!this.clientCalledConnectOnly && !this.clientCalledConnectAndJoin)
				{
					this.clientCalledConnectOnly = true;
					this.clientCalledDisconnect = false;
				}
			}
			this.FollowPun(toState);
		}

		private void FollowPun(ClientState toState)
		{
			if (toState == ClientState.Joined || toState - ClientState.Disconnected <= 1)
			{
				this.FollowPun();
			}
		}

		protected override Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
		{
			if (!(userData is int))
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("UserData ({0}) does not contain PhotonViewId. Remote voice {1}/{2} not linked. Do you have a Recorder not used with a PhotonVoiceView? is this expected?", new object[]
					{
						(userData == null) ? "null" : userData.ToString(),
						playerId,
						voiceId
					});
				}
				return null;
			}
			PhotonView photonView = PhotonView.Find((int)userData);
			if (photonView == null || !photonView)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("No PhotonView with ID {0} found. Remote voice {1}/{2} not linked.", new object[]
					{
						userData,
						playerId,
						voiceId
					});
				}
				return null;
			}
			PhotonVoiceView component = photonView.GetComponent<PhotonVoiceView>();
			if (component == null || !component)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("No PhotonVoiceView attached to the PhotonView with ID {0}. Remote voice {1}/{2} not linked.", new object[]
					{
						userData,
						playerId,
						voiceId
					});
				}
				return null;
			}
			if (!component.IgnoreGlobalLogLevel)
			{
				component.LogLevel = base.LogLevel;
			}
			if (!component.IsSpeaker)
			{
				component.SetupSpeakerInUse();
			}
			return component.SpeakerInUse;
		}

		internal static string GetVoiceRoomName()
		{
			if (PhotonNetwork.InRoom)
			{
				return string.Format("{0}{1}", PhotonNetwork.CurrentRoom.Name, "_voice_");
			}
			return null;
		}

		private void ConnectOrJoin()
		{
			ClientState clientState = base.ClientState;
			if (clientState != ClientState.PeerCreated && clientState != ClientState.Disconnected)
			{
				if (clientState != ClientState.ConnectedToMasterServer)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("PUN joined room, Voice client is busy ({0}). Is this expected?", new object[]
						{
							base.ClientState
						});
					}
				}
				else
				{
					if (base.Logger.IsInfoEnabled)
					{
						base.Logger.LogInfo("PUN joined room, now joining Voice room", Array.Empty<object>());
					}
					if (!this.JoinRoom(PhotonVoiceNetwork.GetVoiceRoomName()) && base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Joining a voice room failed.", Array.Empty<object>());
						return;
					}
				}
			}
			else
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("PUN joined room, now connecting Voice client", Array.Empty<object>());
				}
				if (this.Connect())
				{
					this.internalConnect = (this.AutoConnectAndJoin && !this.clientCalledConnectOnly && !this.clientCalledConnectAndJoin);
					return;
				}
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Connecting to server failed.", Array.Empty<object>());
					return;
				}
			}
		}

		private bool Connect()
		{
			AppSettings appSettings = null;
			if (this.usePunAppSettings)
			{
				appSettings = new AppSettings();
				appSettings = PhotonNetwork.PhotonServerSettings.AppSettings.CopyTo(appSettings);
				if (!string.IsNullOrEmpty(PhotonNetwork.CloudRegion))
				{
					appSettings.FixedRegion = PhotonNetwork.CloudRegion;
				}
				base.Client.SerializationProtocol = PhotonNetwork.NetworkingClient.SerializationProtocol;
			}
			if (this.UsePunAuthValues)
			{
				if (PhotonNetwork.AuthValues != null)
				{
					if (base.Client.AuthValues == null)
					{
						base.Client.AuthValues = new AuthenticationValues();
					}
					base.Client.AuthValues = PhotonNetwork.AuthValues.CopyTo(base.Client.AuthValues);
				}
				base.Client.AuthMode = PhotonNetwork.NetworkingClient.AuthMode;
				base.Client.EncryptionMode = PhotonNetwork.NetworkingClient.EncryptionMode;
			}
			return base.ConnectUsingSettings(appSettings);
		}

		private bool JoinRoom(string voiceRoomName)
		{
			if (string.IsNullOrEmpty(voiceRoomName))
			{
				if (base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Voice room name is null or empty.", Array.Empty<object>());
				}
				return false;
			}
			this.voiceRoomParams.RoomName = voiceRoomName;
			return base.Client.OpJoinOrCreateRoom(this.voiceRoomParams);
		}

		private void FollowPun()
		{
			if (ConnectionHandler.AppQuits)
			{
				return;
			}
			if (PhotonNetwork.OfflineMode && !this.WorkInOfflineMode)
			{
				return;
			}
			if (PhotonNetwork.NetworkClientState == base.ClientState)
			{
				if (PhotonNetwork.InRoom && this.AutoConnectAndJoin)
				{
					string voiceRoomName = PhotonVoiceNetwork.GetVoiceRoomName();
					string name = base.Client.CurrentRoom.Name;
					if (!name.Equals(voiceRoomName))
					{
						if (base.Logger.IsWarningEnabled)
						{
							base.Logger.LogWarning("Voice room mismatch: Expected:\"{0}\" Current:\"{1}\", leaving the second to join the first.", new object[]
							{
								voiceRoomName,
								name
							});
						}
						if (!base.Client.OpLeaveRoom(false, false) && base.Logger.IsErrorEnabled)
						{
							base.Logger.LogError("Leaving the current voice room failed.", Array.Empty<object>());
							return;
						}
					}
				}
				else if (base.ClientState == ClientState.ConnectedToMasterServer && this.AutoLeaveAndDisconnect && !this.clientCalledConnectAndJoin && !this.clientCalledConnectOnly)
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: PUN and Voice clients have the same client state: ConnectedToMasterServer, Disconnecting Voice client.", Array.Empty<object>());
					}
					this.internalDisconnect = true;
					base.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
				}
				return;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.clientCalledConnectAndJoin || (this.AutoConnectAndJoin && !this.clientCalledDisconnect))
				{
					this.ConnectOrJoin();
					return;
				}
			}
			else if (base.Client.InRoom && this.AutoLeaveAndDisconnect && !this.clientCalledConnectAndJoin && !this.clientCalledConnectOnly)
			{
				if (base.Logger.IsInfoEnabled)
				{
					base.Logger.LogInfo("PUN left room, disconnecting Voice", Array.Empty<object>());
				}
				this.internalDisconnect = true;
				base.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
			}
		}

		internal void CheckLateLinking(Speaker speaker, int viewId)
		{
			if (speaker == null || !speaker)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot check late linking for null Speaker", Array.Empty<object>());
				}
				return;
			}
			if (viewId <= 0)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot check late linking for ViewID = {0} (<= 0)", new object[]
					{
						viewId
					});
				}
				return;
			}
			if (!base.Client.InRoom)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Cannot check late linking while not joined to a voice room, client state: {0}", new object[]
					{
						Enum.GetName(typeof(ClientState), base.ClientState)
					});
				}
				return;
			}
			for (int i = 0; i < this.cachedRemoteVoices.Count; i++)
			{
				RemoteVoiceLink remoteVoiceLink = this.cachedRemoteVoices[i];
				if (remoteVoiceLink.Info.UserData is int)
				{
					int num = (int)remoteVoiceLink.Info.UserData;
					if (viewId == num)
					{
						if (base.Logger.IsInfoEnabled)
						{
							base.Logger.LogInfo("Speaker 'late-linking' for the PhotonView with ID {0} with remote voice {1}/{2}.", new object[]
							{
								viewId,
								remoteVoiceLink.PlayerId,
								remoteVoiceLink.VoiceId
							});
						}
						base.LinkSpeaker(speaker, remoteVoiceLink);
						return;
					}
				}
				else if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("VoiceInfo.UserData should be int/ViewId, received: {0}, do you have a Recorder not used with a PhotonVoiceView? is this expected?", new object[]
					{
						(remoteVoiceLink.Info.UserData == null) ? "null" : string.Format("{0} ({1})", remoteVoiceLink.Info.UserData, remoteVoiceLink.Info.UserData.GetType())
					});
					if (remoteVoiceLink.PlayerId == viewId / PhotonNetwork.MAX_VIEW_IDS)
					{
						base.Logger.LogWarning("Player with ActorNumber {0} has started recording (voice # {1}) too early without setting a ViewId maybe? (before PhotonVoiceView setup)", new object[]
						{
							remoteVoiceLink.PlayerId,
							remoteVoiceLink.VoiceId
						});
					}
				}
			}
		}

		public const string VoiceRoomNameSuffix = "_voice_";

		public bool AutoConnectAndJoin = true;

		public bool AutoLeaveAndDisconnect = true;

		public bool WorkInOfflineMode = true;

		private EnterRoomParams voiceRoomParams = new EnterRoomParams
		{
			RoomOptions = new RoomOptions
			{
				IsVisible = false
			}
		};

		private bool clientCalledConnectAndJoin;

		private bool clientCalledDisconnect;

		private bool clientCalledConnectOnly;

		private bool internalDisconnect;

		private bool internalConnect;

		private static object instanceLock = new object();

		private static PhotonVoiceNetwork instance;

		private static bool instantiated;

		[SerializeField]
		private bool usePunAppSettings = true;

		[SerializeField]
		private bool usePunAuthValues = true;
	}
}
