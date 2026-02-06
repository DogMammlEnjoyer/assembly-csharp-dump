using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Encryption;
using Fusion.Photon.Realtime.Extension;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	internal class FusionRelayClient : LoadBalancingClient, IInRoomCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IConnectionCallbacks
	{
		public void StartFallbackSendAck()
		{
			bool flag = !this._connectionHandler;
			if (flag)
			{
				GameObject gameObject = new GameObject("Fusion_PhotonBackgroundConnectionHandler", new Type[]
				{
					typeof(ConnectionHandler)
				})
				{
					hideFlags = HideFlags.NotEditable
				};
				Object.DontDestroyOnLoad(gameObject);
				this._connectionHandler = gameObject.GetComponent<ConnectionHandler>();
				this._connectionHandler.Client = this;
				this._connectionHandler.KeepAliveInBackground = 60000;
			}
			this._connectionHandler.StartFallbackSendAckThread();
		}

		public void StopFallbackSendAck()
		{
			bool flag = !this._connectionHandler;
			if (!flag)
			{
				this._connectionHandler.StopFallbackSendAckThread();
				Object.Destroy(this._connectionHandler.gameObject);
				this._connectionHandler = null;
			}
		}

		private void OnEventHandler(EventData evt)
		{
			int sender = evt.Sender;
			byte code = evt.Code;
			object customData = evt.CustomData;
			Action<int, int, object> onEventCallback = this.OnEventCallback;
			if (onEventCallback != null)
			{
				onEventCallback(sender, (int)code, customData);
			}
		}

		public unsafe bool SendEvent(int target, byte eventCode, byte* buffer, int bufferLength, bool reliable)
		{
			bool flag = !this.IsReadyAndInRoom;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ByteArraySlice byteArraySlice = base.LoadBalancingPeer.ByteArraySlicePool.Acquire(bufferLength);
				bool flag2 = buffer != null;
				if (flag2)
				{
					byte[] array;
					byte* destination;
					if ((array = byteArraySlice.Buffer) == null || array.Length == 0)
					{
						destination = null;
					}
					else
					{
						destination = &array[0];
					}
					Native.MemCpy((void*)destination, (void*)buffer, bufferLength);
					array = null;
				}
				byteArraySlice.Count = bufferLength;
				this._raiseEventOptions.TargetActors[0] = target;
				bool flag3 = this.OpRaiseEvent(eventCode, byteArraySlice, this._raiseEventOptions, reliable ? this._optionsReliable : this._optionsUnreliable);
				bool flag4 = base.LoadBalancingPeer.SendOutgoingCommands();
				if (flag4)
				{
					base.LoadBalancingPeer.SendOutgoingCommands();
				}
				result = flag3;
			}
			return result;
		}

		public void ExtractData(object dataObj, byte[] buffer, ref int bufferLength)
		{
			ByteArraySlice byteArraySlice = dataObj as ByteArraySlice;
			bool flag = byteArraySlice != null;
			if (flag)
			{
				Assert.Always<int, int>(byteArraySlice.Count <= bufferLength, "Array slice to large for the buffer {0} {1}", bufferLength, byteArraySlice.Count);
				bufferLength = byteArraySlice.Count;
				Array.Copy(byteArraySlice.Buffer, buffer, bufferLength);
				byteArraySlice.Release();
			}
			else
			{
				bufferLength = -1;
			}
		}

		public bool IsReadyAndInRoom
		{
			get
			{
				return base.IsConnectedAndReady && base.InRoom;
			}
		}

		public bool IsEncryptionEnabled
		{
			get
			{
				return this.EncryptionMode > EncryptionMode.PayloadEncryption;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnRoomChanged;

		public bool UseDefaultPorts { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int, int, object> OnEventCallback;

		public int DisconnectTimeout
		{
			get
			{
				return base.LoadBalancingPeer.DisconnectTimeout;
			}
			set
			{
				base.LoadBalancingPeer.DisconnectTimeout = value;
			}
		}

		public FusionRelayClient(FusionAppSettings config) : base(ConnectionProtocol.Udp)
		{
			TraceLogStream logTraceRealtime = InternalLogStreams.LogTraceRealtime;
			if (logTraceRealtime != null)
			{
				logTraceRealtime.Info((config != null) ? config.ToString() : null);
			}
			this.Config = config;
			base.ClientType = ClientAppType.Fusion;
			base.SerializationProtocol = SerializationProtocol.GpBinaryV18;
			base.LoadBalancingPeer.TimePingInterval = 200;
			base.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true;
			base.LoadBalancingPeer.ReuseEventInstance = true;
			base.LoadBalancingPeer.QuickResendAttempts = 8;
			base.LoadBalancingPeer.SentCountAllowance *= 10;
			base.LoadBalancingPeer.DisconnectTimeout = 15000;
			base.LoadBalancingPeer.SendWindowSize /= 3;
			this.EncryptionMode = this.Config.encryptionMode;
			base.LoadBalancingPeer.EncryptorType = (this.IsEncryptionEnabled ? FusionRelayClient.LoadPhotonEncryptorType() : null);
			base.LoadBalancingPeer.OnDisconnectMessage += this.OnDisconnectMessage;
			this.UseDefaultPorts = false;
			base.EventReceived += this.OnEventHandler;
			base.AddCallbackTarget(this);
			this._raiseEventOptions = new RaiseEventOptions
			{
				TargetActors = new int[1]
			};
			this._optionsUnreliable = new SendOptions
			{
				Channel = 0,
				DeliveryMode = DeliveryMode.UnreliableUnsequenced
			};
			this._optionsReliable = new SendOptions
			{
				Channel = 1,
				DeliveryMode = DeliveryMode.Reliable
			};
		}

		private static Type LoadPhotonEncryptorType()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			bool flag = true;
			for (;;)
			{
				foreach (Assembly assembly in assemblies)
				{
					string text = assembly.FullName.ToLower();
					bool flag2 = !flag || text.Contains("assembly-csharp") || text.Contains("fusion") || text.Contains("photon");
					if (flag2)
					{
						foreach (Type type in assembly.GetTypes())
						{
							bool flag3 = !typeof(IPhotonEncryptor).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract;
							if (!flag3)
							{
								try
								{
									IPhotonEncryptor photonEncryptor = (IPhotonEncryptor)Activator.CreateInstance(type);
									photonEncryptor.Init(Array.Empty<byte>(), Array.Empty<byte>(), null, false, 1200);
									IDisposable disposable = photonEncryptor as IDisposable;
									if (disposable != null)
									{
										disposable.Dispose();
									}
								}
								catch
								{
									goto IL_124;
								}
								goto IL_F5;
							}
							IL_124:;
						}
					}
				}
				bool flag4 = flag;
				if (!flag4)
				{
					goto IL_150;
				}
				flag = false;
			}
			IL_F5:
			TraceLogStream logTraceRealtime = InternalLogStreams.LogTraceRealtime;
			Type type;
			if (logTraceRealtime != null)
			{
				logTraceRealtime.Info(string.Format("Encryption IPhotonEncryptor Type: {0}/{1}", type.FullName, type.Assembly));
			}
			return type;
			IL_150:
			throw new InvalidOperationException("No implementation of IPhotonEncryptor found. Make sure to include a Photon Realtime Encryption Library in your project.");
		}

		public void Reset()
		{
			Object.Destroy(this._loggerGO);
		}

		public override bool ConnectUsingSettings(AppSettings appSettings)
		{
			AppSettings copy = appSettings.GetCopy();
			this.ServerPortOverrides = default(PhotonPortDefinition);
			bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
			if (isUNITY_WEBGL)
			{
				copy.Protocol = ConnectionProtocol.WebSocketSecure;
				TraceLogStream logTraceRealtime = InternalLogStreams.LogTraceRealtime;
				if (logTraceRealtime != null)
				{
					logTraceRealtime.Info("Changing Photon Cloud Communication Protocol to WebSocketSecure");
				}
			}
			bool flag = copy.Protocol == ConnectionProtocol.Udp && copy.IsDefaultPort;
			if (flag)
			{
				bool flag2 = !this.UseDefaultPorts;
				if (flag2)
				{
					this.ServerPortOverrides = PhotonPortDefinition.AlternativeUdpPorts;
					TraceLogStream logTraceRealtime2 = InternalLogStreams.LogTraceRealtime;
					if (logTraceRealtime2 != null)
					{
						logTraceRealtime2.Info(string.Concat(new string[]
						{
							"Changing Photon Cloud Communication Ports to [",
							string.Format("{0}={1},", "NameServerPort", this.ServerPortOverrides.NameServerPort),
							string.Format("{0}={1},", "MasterServerPort", this.ServerPortOverrides.MasterServerPort),
							string.Format("{0}={1}", "GameServerPort", this.ServerPortOverrides.GameServerPort),
							"]"
						}));
					}
				}
			}
			bool flag3;
			if (copy.FixedRegion.ToLower().Equals("cn"))
			{
				string server = copy.Server;
				flag3 = string.IsNullOrEmpty((server != null) ? server.Trim() : null);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			if (flag4)
			{
				copy.Server = "ns.photonengine.cn";
			}
			return base.ConnectUsingSettings(copy);
		}

		public bool UpdateRoomProperties(Dictionary<string, SessionProperty> customProperties)
		{
			bool flag = customProperties == null || customProperties.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool isOffline = base.CurrentRoom.IsOffline;
				if (isOffline)
				{
					result = false;
				}
				else
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Merge(base.CurrentRoom.CustomProperties);
					bool flag2 = false;
					foreach (string text in customProperties.Keys)
					{
						bool flag3 = hashtable.ContainsKey(text);
						if (flag3)
						{
							bool flag4 = !hashtable[text].Equals(customProperties[text].PropertyValue);
							if (flag4)
							{
								hashtable[text] = customProperties[text].PropertyValue;
								flag2 = true;
							}
						}
						else
						{
							LogStream logWarn = InternalLogStreams.LogWarn;
							if (logWarn != null)
							{
								logWarn.Log("Invalid custom property key [" + text + "], ignore. Only existing custom properties can be updated.");
							}
						}
					}
					bool flag5 = hashtable.Count > 10;
					if (flag5)
					{
						LogStream logWarn2 = InternalLogStreams.LogWarn;
						if (logWarn2 != null)
						{
							logWarn2.Log("Max number of Custom Session Properties reached, only 10 properties are allowed.");
						}
						result = false;
					}
					else
					{
						int num = hashtable.CalculateTotalSize();
						bool flag6 = num > 500;
						if (flag6)
						{
							LogStream logWarn3 = InternalLogStreams.LogWarn;
							if (logWarn3 != null)
							{
								logWarn3.Log(string.Format("Max size of Custom Session Properties reached, current size of {0} bytes, max 500 bytes are allowed.", num));
							}
							result = false;
						}
						else
						{
							result = (flag2 && base.CurrentRoom.SetCustomProperties(hashtable, null, null));
						}
					}
				}
			}
			return result;
		}

		public bool UpdateRoomIsVisible(bool value)
		{
			bool isOffline = base.CurrentRoom.IsOffline;
			bool result;
			if (isOffline)
			{
				result = false;
			}
			else
			{
				bool flag = base.CurrentRoom.IsVisible == value;
				if (flag)
				{
					result = false;
				}
				else
				{
					base.CurrentRoom.IsVisible = value;
					result = true;
				}
			}
			return result;
		}

		public bool UpdateRoomIsOpen(bool value)
		{
			bool isOffline = base.CurrentRoom.IsOffline;
			bool result;
			if (isOffline)
			{
				result = false;
			}
			else
			{
				bool flag = base.CurrentRoom.IsOpen == value;
				if (flag)
				{
					result = false;
				}
				else
				{
					base.CurrentRoom.IsOpen = value;
					result = true;
				}
			}
			return result;
		}

		public void Update()
		{
			bool flag = this._loggerGO == null && base.LoadBalancingPeer.DebugOut >= DebugLevel.WARNING;
			if (flag)
			{
				SupportLogger supportLogger = Object.FindObjectOfType<SupportLogger>();
				bool flag2 = supportLogger == null;
				if (flag2)
				{
					this._loggerGO = new GameObject
					{
						name = "RealtimeLogger",
						hideFlags = HideFlags.NotEditable
					};
					Object.DontDestroyOnLoad(this._loggerGO);
					supportLogger = this._loggerGO.AddComponent<SupportLogger>();
				}
				supportLogger.Client = this;
				this._loggerGO = supportLogger.gameObject;
			}
			try
			{
				base.Service();
			}
			catch (Exception error)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(error);
				}
			}
		}

		private void OnDisconnectMessage(DisconnectMessage obj)
		{
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(string.Format("DisconnectMessage. Code: {0} Msg: \"{1}\". Debug Info: {2}", obj.Code, obj.DebugMessage, obj.Parameters.ToStringFull()));
			}
		}

		public EnterRoomParams BuildEnterRoomParams(TypedLobby typedLobby, string roomName, int maxPlayers, Dictionary<string, SessionProperty> customProperties = null, bool isOpen = true, bool isVisible = true, bool useDefaultEmptyRoomTtl = true, bool extendedTtl = false)
		{
			Hashtable customRoomProperties;
			string[] customRoomPropertiesForLobby;
			FusionRelayClient.BuildSessionCustomPropertyHolders(customProperties, out customRoomProperties, out customRoomPropertiesForLobby);
			return new EnterRoomParams
			{
				RoomName = roomName,
				Lobby = typedLobby,
				RoomOptions = new RoomOptions
				{
					MaxPlayers = maxPlayers,
					IsOpen = isOpen,
					IsVisible = isVisible,
					DeleteNullProperties = true,
					PlayerTtl = (extendedTtl ? 15000 : 0),
					EmptyRoomTtl = (useDefaultEmptyRoomTtl ? 0 : this.Config.emptyRoomTtl),
					Plugins = new string[]
					{
						"FusionPlugin"
					},
					SuppressRoomEvents = false,
					SuppressPlayerInfo = false,
					PublishUserId = true,
					CustomRoomProperties = customRoomProperties,
					CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
				}
			};
		}

		public OpJoinRandomRoomParams BuildJoinParams(TypedLobby typedLobby, Dictionary<string, SessionProperty> customProperties = null, MatchmakingMode matchmakingMode = MatchmakingMode.FillRoom)
		{
			Hashtable expectedCustomRoomProperties;
			string[] array;
			FusionRelayClient.BuildSessionCustomPropertyHolders(customProperties, out expectedCustomRoomProperties, out array);
			return new OpJoinRandomRoomParams
			{
				MatchingType = matchmakingMode,
				TypedLobby = typedLobby,
				ExpectedCustomRoomProperties = expectedCustomRoomProperties
			};
		}

		private static void BuildSessionCustomPropertyHolders(Dictionary<string, SessionProperty> customProperties, out Hashtable sessionCustomProperties, out string[] publicSessionProperties)
		{
			sessionCustomProperties = null;
			publicSessionProperties = null;
			bool flag = customProperties != null && customProperties.Count > 0;
			if (flag)
			{
				sessionCustomProperties = customProperties.ConvertToHashtable();
				publicSessionProperties = new List<string>(customProperties.Keys).ToArray();
			}
		}

		public void OnJoinedRoom()
		{
			this.StartFallbackSendAck();
		}

		public void OnLeftRoom()
		{
			this.StopFallbackSendAck();
		}

		public void OnCreatedRoom()
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinedLobby()
		{
			this.StartFallbackSendAck();
		}

		public void OnLeftLobby()
		{
			this.StopFallbackSendAck();
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		public void OnMasterClientSwitched(Player newMasterClient)
		{
			Action onRoomChanged = this.OnRoomChanged;
			if (onRoomChanged != null)
			{
				onRoomChanged();
			}
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			Action onRoomChanged = this.OnRoomChanged;
			if (onRoomChanged != null)
			{
				onRoomChanged();
			}
		}

		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			Action onRoomChanged = this.OnRoomChanged;
			if (onRoomChanged != null)
			{
				onRoomChanged();
			}
		}

		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			Action onRoomChanged = this.OnRoomChanged;
			if (onRoomChanged != null)
			{
				onRoomChanged();
			}
		}

		public void OnConnected()
		{
		}

		public void OnConnectedToMaster()
		{
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			this.StopFallbackSendAck();
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

		private ConnectionHandler _connectionHandler;

		private const string FUSION_PLUGIN_NAME = "FusionPlugin";

		private const string SERVER_HOST_CN = "ns.photonengine.cn";

		private const string REGION_CN_ID = "cn";

		private readonly RaiseEventOptions _raiseEventOptions;

		private readonly SendOptions _optionsUnreliable;

		private readonly SendOptions _optionsReliable;

		private GameObject _loggerGO;

		private FusionAppSettings Config;
	}
}
