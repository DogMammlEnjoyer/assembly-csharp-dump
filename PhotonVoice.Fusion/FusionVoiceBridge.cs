using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

namespace Photon.Voice.Fusion
{
	[RequireComponent(typeof(NetworkRunner))]
	[RequireComponent(typeof(VoiceConnection))]
	public class FusionVoiceBridge : VoiceComponent, INetworkRunnerCallbacks, IPublicFacingInterface
	{
		public bool UseFusionAppSettings { get; set; } = true;

		public bool UseFusionAuthValues { get; set; } = true;

		protected override void Awake()
		{
			base.Awake();
			FusionVoiceBridge.VoiceRegisterCustomTypes();
			this.networkRunner = base.GetComponent<NetworkRunner>();
			this.voiceConnection = base.GetComponent<VoiceConnection>();
			this.voiceConnection.SpeakerFactory = new Func<int, byte, object, Speaker>(this.FusionSpeakerFactory);
		}

		private void OnEnable()
		{
			this.voiceConnection.Client.StateChanged += this.OnVoiceClientStateChanged;
			if (this.networkRunner.IsPlayer && this.networkRunner.IsConnectedToServer)
			{
				this.VoiceConnectOrJoinRoom();
			}
		}

		private void OnDisable()
		{
			this.voiceConnection.Client.StateChanged -= this.OnVoiceClientStateChanged;
		}

		private void OnVoiceClientStateChanged(ClientState previous, ClientState current)
		{
			this.VoiceConnectOrJoinRoom(current);
		}

		private Speaker FusionSpeakerFactory(int playerId, byte voiceId, object userData)
		{
			if (!(userData is NetworkId))
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("UserData ({0}) is not of type NetworkId. Remote voice {1}/{2} not linked. Do you have a Recorder not used with a VoiceNetworkObject? is this expected?", new object[]
					{
						(userData == null) ? "null" : userData.ToString(),
						playerId,
						voiceId
					});
				}
				return null;
			}
			NetworkId networkId = (NetworkId)userData;
			if (!networkId.IsValid)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("NetworkId is not valid ({0}). Remote voice {1}/{2} not linked.", new object[]
					{
						networkId,
						playerId,
						voiceId
					});
				}
				return null;
			}
			VoiceNetworkObject voiceNetworkObject = this.networkRunner.TryGetNetworkedBehaviourFromNetworkedObjectRef<VoiceNetworkObject>(networkId);
			if (voiceNetworkObject == null || !voiceNetworkObject)
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("No voiceNetworkObject found with ID {0}. Remote voice {1}/{2} not linked.", new object[]
					{
						networkId,
						playerId,
						voiceId
					});
				}
				return null;
			}
			if (!voiceNetworkObject.IgnoreGlobalLogLevel)
			{
				voiceNetworkObject.LogLevel = base.LogLevel;
			}
			if (!voiceNetworkObject.IsSpeaker)
			{
				voiceNetworkObject.SetupSpeakerInUse();
			}
			return voiceNetworkObject.SpeakerInUse;
		}

		private string VoiceGetMirroringRoomName()
		{
			return string.Format("{0}_voice", this.networkRunner.SessionInfo.Name);
		}

		private void VoiceConnectOrJoinRoom()
		{
			this.VoiceConnectOrJoinRoom(this.voiceConnection.ClientState);
		}

		private void VoiceConnectOrJoinRoom(ClientState state)
		{
			if (ConnectionHandler.AppQuits)
			{
				return;
			}
			if (state <= ClientState.Joined)
			{
				if (state != ClientState.PeerCreated)
				{
					if (state != ClientState.Joined)
					{
						return;
					}
					string text = this.VoiceGetMirroringRoomName();
					string name = this.voiceConnection.Client.CurrentRoom.Name;
					if (name.Equals(text))
					{
						return;
					}
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Voice room mismatch: Expected:\"{0}\" Current:\"{1}\", leaving the second to join the first.", new object[]
						{
							text,
							name
						});
					}
					if (!this.voiceConnection.Client.OpLeaveRoom(false, false) && base.Logger.IsErrorEnabled)
					{
						base.Logger.LogError("Leaving the current voice room failed.", Array.Empty<object>());
						return;
					}
					return;
				}
			}
			else if (state != ClientState.Disconnected)
			{
				if (state != ClientState.ConnectedToMasterServer)
				{
					return;
				}
				if (!this.VoiceJoinMirroringRoom() && base.Logger.IsErrorEnabled)
				{
					base.Logger.LogError("Joining a voice room failed.", Array.Empty<object>());
					return;
				}
				return;
			}
			if (!this.VoiceConnectAndFollowFusion() && base.Logger.IsErrorEnabled)
			{
				base.Logger.LogError("Connecting to server failed.", Array.Empty<object>());
				return;
			}
		}

		private bool VoiceConnectAndFollowFusion()
		{
			Photon.Realtime.AppSettings appSettings = new Photon.Realtime.AppSettings();
			if (this.UseFusionAppSettings)
			{
				appSettings.AppIdVoice = PhotonAppSettings.Global.AppSettings.AppIdVoice;
				appSettings.AppVersion = PhotonAppSettings.Global.AppSettings.AppVersion;
				appSettings.FixedRegion = PhotonAppSettings.Global.AppSettings.FixedRegion;
				appSettings.UseNameServer = PhotonAppSettings.Global.AppSettings.UseNameServer;
				appSettings.Server = PhotonAppSettings.Global.AppSettings.Server;
				appSettings.Port = PhotonAppSettings.Global.AppSettings.Port;
				appSettings.ProxyServer = PhotonAppSettings.Global.AppSettings.ProxyServer;
				appSettings.BestRegionSummaryFromStorage = PhotonAppSettings.Global.AppSettings.BestRegionSummaryFromStorage;
				appSettings.EnableLobbyStatistics = false;
				appSettings.EnableProtocolFallback = PhotonAppSettings.Global.AppSettings.EnableProtocolFallback;
				appSettings.Protocol = PhotonAppSettings.Global.AppSettings.Protocol;
				appSettings.AuthMode = (Photon.Realtime.AuthModeOption)PhotonAppSettings.Global.AppSettings.AuthMode;
				appSettings.NetworkLogging = PhotonAppSettings.Global.AppSettings.NetworkLogging;
			}
			else
			{
				this.voiceConnection.Settings.CopyTo(appSettings);
			}
			string region = this.networkRunner.SessionInfo.Region;
			if (string.IsNullOrEmpty(region))
			{
				if (base.Logger.IsWarningEnabled)
				{
					base.Logger.LogWarning("Unexpected: fusion region is empty.", Array.Empty<object>());
				}
				if (!string.IsNullOrEmpty(appSettings.FixedRegion))
				{
					if (base.Logger.IsWarningEnabled)
					{
						base.Logger.LogWarning("Unexpected: fusion region is empty while voice region is set to \"{0}\". Setting it to null now.", new object[]
						{
							appSettings.FixedRegion
						});
					}
					appSettings.FixedRegion = null;
				}
			}
			else if (!string.Equals(appSettings.FixedRegion, region, StringComparison.OrdinalIgnoreCase))
			{
				if (base.Logger.IsInfoEnabled)
				{
					if (string.IsNullOrEmpty(appSettings.FixedRegion))
					{
						base.Logger.LogInfo("Setting voice region to \"{0}\" to match fusion region.", new object[]
						{
							region
						});
					}
					else
					{
						base.Logger.LogInfo("Switching voice region to \"{0}\" from \"{1}\" to match fusion region.", new object[]
						{
							region,
							appSettings.FixedRegion
						});
					}
				}
				appSettings.FixedRegion = region;
			}
			if (this.UseFusionAuthValues && this.networkRunner.AuthenticationValues != null)
			{
				this.voiceConnection.Client.AuthValues = new Photon.Realtime.AuthenticationValues(this.networkRunner.AuthenticationValues.UserId)
				{
					AuthGetParameters = this.networkRunner.AuthenticationValues.AuthGetParameters,
					AuthType = (Photon.Realtime.CustomAuthenticationType)this.networkRunner.AuthenticationValues.AuthType
				};
				if (this.networkRunner.AuthenticationValues.AuthPostData != null)
				{
					byte[] array = this.networkRunner.AuthenticationValues.AuthPostData as byte[];
					if (array != null)
					{
						this.voiceConnection.Client.AuthValues.SetAuthPostData(array);
					}
					else
					{
						string text = this.networkRunner.AuthenticationValues.AuthPostData as string;
						if (text != null)
						{
							this.voiceConnection.Client.AuthValues.SetAuthPostData(text);
						}
						else
						{
							Dictionary<string, object> dictionary = this.networkRunner.AuthenticationValues.AuthPostData as Dictionary<string, object>;
							if (dictionary != null)
							{
								this.voiceConnection.Client.AuthValues.SetAuthPostData(dictionary);
							}
						}
					}
				}
			}
			return this.voiceConnection.ConnectUsingSettings(appSettings);
		}

		private void VoiceDisconnect()
		{
			this.voiceConnection.Client.Disconnect(DisconnectCause.DisconnectByClientLogic);
		}

		private bool VoiceJoinRoom(string voiceRoomName)
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
			return this.voiceConnection.Client.OpJoinOrCreateRoom(this.voiceRoomParams);
		}

		private bool VoiceJoinMirroringRoom()
		{
			return this.VoiceJoinRoom(this.VoiceGetMirroringRoomName());
		}

		private static void VoiceRegisterCustomTypes()
		{
			PhotonPeer.RegisterType(typeof(NetworkId), 0, new SerializeStreamMethod(FusionVoiceBridge.SerializeFusionNetworkId), new DeserializeStreamMethod(FusionVoiceBridge.DeserializeFusionNetworkId));
		}

		private static object DeserializeFusionNetworkId(StreamBuffer instream, short length)
		{
			NetworkId networkId = default(NetworkId);
			byte[] obj = FusionVoiceBridge.memCompressedUInt64;
			lock (obj)
			{
				ulong num = FusionVoiceBridge.ReadCompressedUInt64(instream);
				networkId.Raw = (uint)num;
			}
			return networkId;
		}

		private static ulong ReadCompressedUInt64(StreamBuffer stream)
		{
			ulong num = 0UL;
			int num2 = 0;
			byte[] buffer = stream.GetBuffer();
			int num3 = stream.Position;
			while (num2 != 70)
			{
				if (num3 >= buffer.Length)
				{
					throw new EndOfStreamException("Failed to read full ulong.");
				}
				byte b = buffer[num3];
				num3++;
				num |= (ulong)((ulong)((long)(b & 127)) << num2);
				num2 += 7;
				if ((b & 128) == 0)
				{
					break;
				}
			}
			stream.Position = num3;
			return num;
		}

		private static int WriteCompressedUInt64(StreamBuffer stream, ulong value)
		{
			int num = 0;
			byte[] obj = FusionVoiceBridge.memCompressedUInt64;
			lock (obj)
			{
				FusionVoiceBridge.memCompressedUInt64[num] = (byte)(value & 127UL);
				for (value >>= 7; value > 0UL; value >>= 7)
				{
					byte[] array = FusionVoiceBridge.memCompressedUInt64;
					int num2 = num;
					array[num2] |= 128;
					FusionVoiceBridge.memCompressedUInt64[++num] = (byte)(value & 127UL);
				}
				num++;
				stream.Write(FusionVoiceBridge.memCompressedUInt64, 0, num);
			}
			return num;
		}

		private static short SerializeFusionNetworkId(StreamBuffer outstream, object customobject)
		{
			NetworkId networkId = (NetworkId)customobject;
			return (short)FusionVoiceBridge.WriteCompressedUInt64(outstream, (ulong)networkId.Raw);
		}

		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnPlayerJoined {0}", new object[]
				{
					player
				});
			}
			if (runner.LocalPlayer == player)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Local player joined, calling VoiceConnectOrJoinRoom", Array.Empty<object>());
				}
				this.VoiceConnectOrJoinRoom();
			}
		}

		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnPlayerLeft {0}", new object[]
				{
					player
				});
			}
			if (runner.LocalPlayer == player)
			{
				if (base.Logger.IsDebugEnabled)
				{
					base.Logger.LogDebug("Local player left, calling VoiceDisconnect", Array.Empty<object>());
				}
				this.VoiceDisconnect();
			}
		}

		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
		{
		}

		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{
		}

		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{
		}

		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
		{
			this.VoiceConnectOrJoinRoom();
		}

		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{
		}

		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
		}

		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{
		}

		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
		}

		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{
		}

		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{
		}

		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
		{
		}

		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
		{
		}

		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
			if (base.Logger.IsDebugEnabled)
			{
				base.Logger.LogDebug("OnDisconnectedFromServer, calling VoiceDisconnect", Array.Empty<object>());
			}
			this.VoiceDisconnect();
		}

		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{
		}

		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
		}

		private NetworkRunner networkRunner;

		private VoiceConnection voiceConnection;

		private EnterRoomParams voiceRoomParams = new EnterRoomParams
		{
			RoomOptions = new RoomOptions
			{
				IsVisible = false
			}
		};

		private const byte FusionNetworkIdTypeCode = 0;

		private static byte[] memCompressedUInt64 = new byte[10];
	}
}
