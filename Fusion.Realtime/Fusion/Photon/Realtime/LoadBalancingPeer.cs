using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime
{
	internal class LoadBalancingPeer : PhotonPeer
	{
		[Obsolete("Use RegionHandler.PingImplementation directly.")]
		protected internal static Type PingImplementation
		{
			get
			{
				return RegionHandler.PingImplementation;
			}
			set
			{
				RegionHandler.PingImplementation = value;
			}
		}

		public LoadBalancingPeer(ConnectionProtocol protocolType) : base(protocolType)
		{
			this.ConfigUnitySockets();
		}

		public LoadBalancingPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : this(protocolType)
		{
			base.Listener = listener;
		}

		[Conditional("SUPPORTED_UNITY")]
		private void ConfigUnitySockets()
		{
			bool flag = (RuntimeUnityFlagsSetup.IsUNITY_XBOXONE || RuntimeUnityFlagsSetup.IsUNITY_GAMECORE) && !RuntimeUnityFlagsSetup.IsUNITY_EDITOR;
			Type type;
			if (flag)
			{
				type = Type.GetType("ExitGames.Client.Photon.SocketNativeSource, Assembly-CSharp", false);
				bool flag2 = type == null;
				if (flag2)
				{
					type = Type.GetType("ExitGames.Client.Photon.SocketNativeSource, Assembly-CSharp-firstpass", false);
				}
				bool flag3 = type == null;
				if (flag3)
				{
					type = Type.GetType("ExitGames.Client.Photon.SocketNativeSource, PhotonRealtime", false);
				}
				bool flag4 = type != null;
				if (flag4)
				{
					this.SocketImplementationConfig[ConnectionProtocol.Udp] = type;
				}
			}
			else
			{
				type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, PhotonWebSocket", false);
				bool flag5 = type == null;
				if (flag5)
				{
					type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", false);
				}
				bool flag6 = type == null;
				if (flag6)
				{
					type = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", false);
				}
				bool isUNITY_WEBGL = RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
				if (isUNITY_WEBGL)
				{
					bool flag7 = type == null && this.DebugOut >= DebugLevel.WARNING;
					if (flag7)
					{
						base.Listener.DebugReturn(DebugLevel.WARNING, "SocketWebTcp type not found in the usual Assemblies. This is required as wrapper for the browser WebSocket API. Make sure to make the PhotonLibs\\WebSocket code available.");
					}
				}
			}
			bool flag8 = type != null;
			if (flag8)
			{
				this.SocketImplementationConfig[ConnectionProtocol.WebSocket] = type;
				this.SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = type;
			}
		}

		public virtual bool OpGetRegions(string appId)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>(1);
			dictionary[224] = appId;
			return this.SendOperation(220, dictionary, new SendOptions
			{
				Reliability = true,
				Encrypt = true
			});
		}

		public virtual bool OpJoinLobby(TypedLobby lobby = null)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinLobby()");
			}
			Dictionary<byte, object> dictionary = null;
			bool flag2 = lobby != null && !lobby.IsDefault;
			if (flag2)
			{
				dictionary = new Dictionary<byte, object>();
				dictionary[213] = lobby.Name;
				dictionary[212] = (byte)lobby.Type;
			}
			return this.SendOperation(229, dictionary, SendOptions.SendReliable);
		}

		public virtual bool OpLeaveLobby()
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpLeaveLobby()");
			}
			return this.SendOperation(228, null, SendOptions.SendReliable);
		}

		private void RoomOptionsToOpParameters(Dictionary<byte, object> op, RoomOptions roomOptions, bool usePropertiesKey = false)
		{
			bool flag = roomOptions == null;
			if (flag)
			{
				roomOptions = new RoomOptions();
			}
			Hashtable hashtable = new Hashtable();
			hashtable[253] = roomOptions.IsOpen;
			hashtable[254] = roomOptions.IsVisible;
			hashtable[250] = ((roomOptions.CustomRoomPropertiesForLobby == null) ? new string[0] : roomOptions.CustomRoomPropertiesForLobby);
			hashtable.MergeStringKeys(roomOptions.CustomRoomProperties);
			bool flag2 = roomOptions.MaxPlayers > 0;
			if (flag2)
			{
				byte b = (roomOptions.MaxPlayers <= 255) ? ((byte)roomOptions.MaxPlayers) : 0;
				hashtable[byte.MaxValue] = b;
				hashtable[243] = roomOptions.MaxPlayers;
			}
			bool flag3 = !usePropertiesKey;
			if (flag3)
			{
				op[248] = hashtable;
			}
			else
			{
				op[251] = hashtable;
			}
			int num = 0;
			bool cleanupCacheOnLeave = roomOptions.CleanupCacheOnLeave;
			if (cleanupCacheOnLeave)
			{
				op[241] = true;
				num |= 2;
			}
			else
			{
				op[241] = false;
				hashtable[249] = false;
			}
			num |= 1;
			op[232] = true;
			bool flag4 = roomOptions.PlayerTtl > 0 || roomOptions.PlayerTtl == -1;
			if (flag4)
			{
				op[235] = roomOptions.PlayerTtl;
			}
			bool flag5 = roomOptions.EmptyRoomTtl > 0;
			if (flag5)
			{
				op[236] = roomOptions.EmptyRoomTtl;
			}
			bool suppressRoomEvents = roomOptions.SuppressRoomEvents;
			if (suppressRoomEvents)
			{
				num |= 4;
				op[237] = true;
			}
			bool suppressPlayerInfo = roomOptions.SuppressPlayerInfo;
			if (suppressPlayerInfo)
			{
				num |= 64;
			}
			bool flag6 = roomOptions.Plugins != null;
			if (flag6)
			{
				op[204] = roomOptions.Plugins;
			}
			bool publishUserId = roomOptions.PublishUserId;
			if (publishUserId)
			{
				num |= 8;
				op[239] = true;
			}
			bool deleteNullProperties = roomOptions.DeleteNullProperties;
			if (deleteNullProperties)
			{
				num |= 16;
			}
			bool broadcastPropsChangeToAll = roomOptions.BroadcastPropsChangeToAll;
			if (broadcastPropsChangeToAll)
			{
				num |= 32;
			}
			op[191] = num;
		}

		public virtual bool OpCreateRoom(EnterRoomParams opParams)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpCreateRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			bool flag2 = !string.IsNullOrEmpty(opParams.RoomName);
			if (flag2)
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			bool flag3 = opParams.Lobby != null && !opParams.Lobby.IsDefault;
			if (flag3)
			{
				dictionary[213] = opParams.Lobby.Name;
				dictionary[212] = (byte)opParams.Lobby.Type;
			}
			bool flag4 = opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length != 0;
			if (flag4)
			{
				dictionary[238] = opParams.ExpectedUsers;
				sendOptions.Encrypt = true;
			}
			bool flag5 = opParams.Ticket != null;
			if (flag5)
			{
				dictionary[190] = opParams.Ticket;
			}
			bool onGameServer = opParams.OnGameServer;
			if (onGameServer)
			{
				bool flag6 = opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0;
				if (flag6)
				{
					dictionary[249] = opParams.PlayerProperties;
				}
				dictionary[250] = true;
				this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions, false);
			}
			return this.SendOperation(227, dictionary, sendOptions);
		}

		public virtual bool OpJoinRoom(EnterRoomParams opParams)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRoom()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			bool flag2 = !string.IsNullOrEmpty(opParams.RoomName);
			if (flag2)
			{
				dictionary[byte.MaxValue] = opParams.RoomName;
			}
			bool flag3 = opParams.JoinMode == JoinMode.CreateIfNotExists;
			if (flag3)
			{
				dictionary[215] = 1;
				bool flag4 = opParams.Lobby != null && !opParams.Lobby.IsDefault;
				if (flag4)
				{
					dictionary[213] = opParams.Lobby.Name;
					dictionary[212] = (byte)opParams.Lobby.Type;
				}
			}
			else
			{
				bool flag5 = opParams.JoinMode == JoinMode.RejoinOnly;
				if (flag5)
				{
					dictionary[215] = 3;
				}
			}
			bool flag6 = opParams.ExpectedUsers != null && opParams.ExpectedUsers.Length != 0;
			if (flag6)
			{
				dictionary[238] = opParams.ExpectedUsers;
				sendOptions.Encrypt = true;
			}
			bool flag7 = opParams.Ticket != null;
			if (flag7)
			{
				dictionary[190] = opParams.Ticket;
			}
			bool onGameServer = opParams.OnGameServer;
			if (onGameServer)
			{
				bool flag8 = opParams.PlayerProperties != null && opParams.PlayerProperties.Count > 0;
				if (flag8)
				{
					dictionary[249] = opParams.PlayerProperties;
				}
				dictionary[250] = true;
				this.RoomOptionsToOpParameters(dictionary, opParams.RoomOptions, false);
			}
			return this.SendOperation(226, dictionary, sendOptions);
		}

		public virtual bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomRoom()");
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
			bool flag2 = opJoinRandomRoomParams.ExpectedMaxPlayers > 0;
			if (flag2)
			{
				byte b = (opJoinRandomRoomParams.ExpectedMaxPlayers <= 255) ? ((byte)opJoinRandomRoomParams.ExpectedMaxPlayers) : 0;
				hashtable[byte.MaxValue] = b;
				bool flag3 = opJoinRandomRoomParams.ExpectedMaxPlayers > 255;
				if (flag3)
				{
					hashtable[243] = opJoinRandomRoomParams.ExpectedMaxPlayers;
				}
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			bool flag4 = hashtable.Count > 0;
			if (flag4)
			{
				dictionary[248] = hashtable;
			}
			bool flag5 = opJoinRandomRoomParams.MatchingType > MatchmakingMode.FillRoom;
			if (flag5)
			{
				dictionary[223] = (byte)opJoinRandomRoomParams.MatchingType;
			}
			bool flag6 = opJoinRandomRoomParams.TypedLobby != null && !opJoinRandomRoomParams.TypedLobby.IsDefault;
			if (flag6)
			{
				dictionary[213] = opJoinRandomRoomParams.TypedLobby.Name;
				dictionary[212] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
			}
			bool flag7 = !string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter);
			if (flag7)
			{
				dictionary[245] = opJoinRandomRoomParams.SqlLobbyFilter;
			}
			bool flag8 = opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length != 0;
			if (flag8)
			{
				dictionary[238] = opJoinRandomRoomParams.ExpectedUsers;
				sendOptions.Encrypt = true;
			}
			bool flag9 = opJoinRandomRoomParams.Ticket != null;
			if (flag9)
			{
				dictionary[190] = opJoinRandomRoomParams.Ticket;
			}
			dictionary[188] = true;
			return this.SendOperation(225, dictionary, sendOptions);
		}

		public virtual bool OpJoinRandomOrCreateRoom(OpJoinRandomRoomParams opJoinRandomRoomParams, EnterRoomParams createRoomParams)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpJoinRandomOrCreateRoom()");
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(opJoinRandomRoomParams.ExpectedCustomRoomProperties);
			bool flag2 = opJoinRandomRoomParams.ExpectedMaxPlayers > 0;
			if (flag2)
			{
				byte b = (opJoinRandomRoomParams.ExpectedMaxPlayers <= 255) ? ((byte)opJoinRandomRoomParams.ExpectedMaxPlayers) : 0;
				hashtable[byte.MaxValue] = b;
				bool flag3 = opJoinRandomRoomParams.ExpectedMaxPlayers > 255;
				if (flag3)
				{
					hashtable[243] = opJoinRandomRoomParams.ExpectedMaxPlayers;
				}
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			bool flag4 = hashtable.Count > 0;
			if (flag4)
			{
				dictionary[248] = hashtable;
			}
			bool flag5 = opJoinRandomRoomParams.MatchingType > MatchmakingMode.FillRoom;
			if (flag5)
			{
				dictionary[223] = (byte)opJoinRandomRoomParams.MatchingType;
			}
			bool flag6 = opJoinRandomRoomParams.TypedLobby != null && !opJoinRandomRoomParams.TypedLobby.IsDefault;
			if (flag6)
			{
				dictionary[213] = opJoinRandomRoomParams.TypedLobby.Name;
				dictionary[212] = (byte)opJoinRandomRoomParams.TypedLobby.Type;
			}
			bool flag7 = !string.IsNullOrEmpty(opJoinRandomRoomParams.SqlLobbyFilter);
			if (flag7)
			{
				dictionary[245] = opJoinRandomRoomParams.SqlLobbyFilter;
			}
			bool flag8 = opJoinRandomRoomParams.ExpectedUsers != null && opJoinRandomRoomParams.ExpectedUsers.Length != 0;
			if (flag8)
			{
				dictionary[238] = opJoinRandomRoomParams.ExpectedUsers;
				sendOptions.Encrypt = true;
			}
			bool flag9 = opJoinRandomRoomParams.Ticket != null;
			if (flag9)
			{
				dictionary[190] = opJoinRandomRoomParams.Ticket;
			}
			dictionary[215] = 1;
			dictionary[188] = true;
			bool flag10 = createRoomParams != null;
			if (flag10)
			{
				bool flag11 = !string.IsNullOrEmpty(createRoomParams.RoomName);
				if (flag11)
				{
					dictionary[byte.MaxValue] = createRoomParams.RoomName;
				}
			}
			return this.SendOperation(225, dictionary, sendOptions);
		}

		public virtual bool OpLeaveRoom(bool becomeInactive, bool sendAuthCookie = false)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (becomeInactive)
			{
				dictionary[233] = true;
			}
			if (sendAuthCookie)
			{
				dictionary[234] = 2;
			}
			return this.SendOperation(254, dictionary, SendOptions.SendReliable);
		}

		public virtual bool OpGetGameList(TypedLobby lobby, string queryData)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList()");
			}
			bool flag2 = lobby == null;
			bool result;
			if (flag2)
			{
				bool flag3 = this.DebugOut >= DebugLevel.INFO;
				if (flag3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. Lobby cannot be null.");
				}
				result = false;
			}
			else
			{
				bool flag4 = lobby.Type != LobbyType.SqlLobby;
				if (flag4)
				{
					bool flag5 = this.DebugOut >= DebugLevel.INFO;
					if (flag5)
					{
						base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. LobbyType must be SqlLobby.");
					}
					result = false;
				}
				else
				{
					bool isDefault = lobby.IsDefault;
					if (isDefault)
					{
						bool flag6 = this.DebugOut >= DebugLevel.INFO;
						if (flag6)
						{
							base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. LobbyName must be not null and not empty.");
						}
						result = false;
					}
					else
					{
						bool flag7 = string.IsNullOrEmpty(queryData);
						if (flag7)
						{
							bool flag8 = this.DebugOut >= DebugLevel.INFO;
							if (flag8)
							{
								base.Listener.DebugReturn(DebugLevel.INFO, "OpGetGameList not sent. queryData must be not null and not empty.");
							}
							result = false;
						}
						else
						{
							Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
							dictionary[213] = lobby.Name;
							dictionary[212] = (byte)lobby.Type;
							dictionary[245] = queryData;
							result = this.SendOperation(217, dictionary, SendOptions.SendReliable);
						}
					}
				}
			}
			return result;
		}

		public virtual bool OpFindFriends(string[] friendsToFind, FindFriendsOptions options = null)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			bool flag = friendsToFind != null && friendsToFind.Length != 0;
			if (flag)
			{
				dictionary[1] = friendsToFind;
			}
			bool flag2 = options != null;
			if (flag2)
			{
				dictionary[2] = options.ToIntFlags();
			}
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true,
				Encrypt = true
			};
			return this.SendOperation(222, dictionary, sendOptions);
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable actorProperties)
		{
			return this.OpSetPropertiesOfActor(actorNr, actorProperties.StripToStringKeys(), null, null);
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webflags = null)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor()");
			}
			bool flag2 = actorNr <= 0 || actorProperties == null || actorProperties.Count == 0;
			bool result;
			if (flag2)
			{
				bool flag3 = this.DebugOut >= DebugLevel.INFO;
				if (flag3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfActor not sent. ActorNr must be > 0 and actorProperties must be not null nor empty.");
				}
				result = false;
			}
			else
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(251, actorProperties);
				dictionary.Add(254, actorNr);
				dictionary.Add(250, true);
				bool flag4 = expectedProperties != null && expectedProperties.Count != 0;
				if (flag4)
				{
					dictionary.Add(231, expectedProperties);
				}
				bool flag5 = webflags != null && webflags.HttpForward;
				if (flag5)
				{
					dictionary[234] = webflags.WebhookFlags;
				}
				result = this.SendOperation(252, dictionary, SendOptions.SendReliable);
			}
			return result;
		}

		protected bool OpSetPropertyOfRoom(byte propCode, object value)
		{
			Hashtable hashtable = new Hashtable();
			hashtable[propCode] = value;
			return this.OpSetPropertiesOfRoom(hashtable, null, null);
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable gameProperties)
		{
			return this.OpSetPropertiesOfRoom(gameProperties.StripToStringKeys(), null, null);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webflags = null)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom()");
			}
			bool flag2 = gameProperties == null || gameProperties.Count == 0;
			bool result;
			if (flag2)
			{
				bool flag3 = this.DebugOut >= DebugLevel.INFO;
				if (flag3)
				{
					base.Listener.DebugReturn(DebugLevel.INFO, "OpSetPropertiesOfRoom not sent. gameProperties must be not null nor empty.");
				}
				result = false;
			}
			else
			{
				Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
				dictionary.Add(251, gameProperties);
				dictionary.Add(250, true);
				bool flag4 = expectedProperties != null && expectedProperties.Count != 0;
				if (flag4)
				{
					dictionary.Add(231, expectedProperties);
				}
				bool flag5 = webflags != null && webflags.HttpForward;
				if (flag5)
				{
					dictionary[234] = webflags.WebhookFlags;
				}
				result = this.SendOperation(252, dictionary, SendOptions.SendReliable);
			}
			return result;
		}

		public virtual bool OpAuthenticate(string appId, string appVersion, AuthenticationValues authValues, string regionCode, bool getLobbyStatistics)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, "OpAuthenticate()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (getLobbyStatistics)
			{
				dictionary[211] = true;
			}
			bool flag2 = authValues != null && authValues.Token != null;
			bool result;
			if (flag2)
			{
				dictionary[221] = authValues.Token;
				result = this.SendOperation(230, dictionary, SendOptions.SendReliable);
			}
			else
			{
				dictionary[220] = appVersion;
				dictionary[224] = appId;
				bool flag3 = !string.IsNullOrEmpty(regionCode);
				if (flag3)
				{
					dictionary[210] = regionCode;
				}
				bool flag4 = authValues != null;
				if (flag4)
				{
					bool flag5 = !string.IsNullOrEmpty(authValues.UserId);
					if (flag5)
					{
						dictionary[225] = authValues.UserId;
					}
					bool flag6 = authValues.AuthType != CustomAuthenticationType.None;
					if (flag6)
					{
						dictionary[217] = (byte)authValues.AuthType;
						bool flag7 = !string.IsNullOrEmpty(authValues.AuthGetParameters);
						if (flag7)
						{
							dictionary[216] = authValues.AuthGetParameters;
						}
						bool flag8 = authValues.AuthPostData != null;
						if (flag8)
						{
							dictionary[214] = authValues.AuthPostData;
						}
					}
				}
				result = this.SendOperation(230, dictionary, new SendOptions
				{
					Reliability = true,
					Encrypt = true
				});
			}
			return result;
		}

		public virtual bool OpAuthenticateOnce(string appId, string appVersion, AuthenticationValues authValues, string regionCode, EncryptionMode encryptionMode, ConnectionProtocol expectedProtocol)
		{
			bool flag = this.DebugOut >= DebugLevel.INFO;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.INFO, string.Concat(new string[]
				{
					"OpAuthenticateOnce(): authValues = ",
					(authValues != null) ? authValues.ToString() : null,
					", region = ",
					regionCode,
					", encryption = ",
					encryptionMode.ToString()
				}));
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			bool flag2 = authValues != null && authValues.Token != null;
			bool result;
			if (flag2)
			{
				dictionary[221] = authValues.Token;
				result = this.SendOperation(231, dictionary, SendOptions.SendReliable);
			}
			else
			{
				bool flag3 = encryptionMode == EncryptionMode.DatagramEncryptionGCM && expectedProtocol > ConnectionProtocol.Udp;
				if (flag3)
				{
					throw new NotSupportedException("Expected protocol set to UDP, due to encryption mode DatagramEncryptionGCM.");
				}
				dictionary[195] = (byte)expectedProtocol;
				dictionary[193] = (byte)encryptionMode;
				dictionary[220] = appVersion;
				dictionary[224] = appId;
				bool flag4 = !string.IsNullOrEmpty(regionCode);
				if (flag4)
				{
					dictionary[210] = regionCode;
				}
				bool flag5 = authValues != null;
				if (flag5)
				{
					bool flag6 = !string.IsNullOrEmpty(authValues.UserId);
					if (flag6)
					{
						dictionary[225] = authValues.UserId;
					}
					bool flag7 = authValues.AuthType != CustomAuthenticationType.None;
					if (flag7)
					{
						dictionary[217] = (byte)authValues.AuthType;
						bool flag8 = authValues.Token != null;
						if (flag8)
						{
							dictionary[221] = authValues.Token;
						}
						else
						{
							bool flag9 = !string.IsNullOrEmpty(authValues.AuthGetParameters);
							if (flag9)
							{
								dictionary[216] = authValues.AuthGetParameters;
							}
							bool flag10 = authValues.AuthPostData != null;
							if (flag10)
							{
								dictionary[214] = authValues.AuthPostData;
							}
						}
					}
				}
				result = this.SendOperation(231, dictionary, new SendOptions
				{
					Reliability = true,
					Encrypt = true
				});
			}
			return result;
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			bool flag = this.DebugOut >= DebugLevel.ALL;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "OpChangeGroups()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			bool flag2 = groupsToRemove != null;
			if (flag2)
			{
				dictionary[239] = groupsToRemove;
			}
			bool flag3 = groupsToAdd != null;
			if (flag3)
			{
				dictionary[238] = groupsToAdd;
			}
			return this.SendOperation(248, dictionary, SendOptions.SendReliable);
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			ParameterDictionary parameterDictionary = this.paramDictionaryPool.Acquire();
			bool result;
			try
			{
				bool flag = raiseEventOptions != null;
				if (flag)
				{
					bool flag2 = raiseEventOptions.CachingOption > EventCaching.DoNotCache;
					if (flag2)
					{
						parameterDictionary.Add(247, (byte)raiseEventOptions.CachingOption);
					}
					switch (raiseEventOptions.CachingOption)
					{
					case EventCaching.RemoveFromRoomCache:
					{
						bool flag3 = raiseEventOptions.TargetActors != null;
						if (flag3)
						{
							parameterDictionary.Add(252, raiseEventOptions.TargetActors);
						}
						goto IL_15B;
					}
					case EventCaching.RemoveFromRoomCacheForActorsLeft:
					case EventCaching.SliceIncreaseIndex:
						return this.SendOperation(253, parameterDictionary, sendOptions);
					case EventCaching.SliceSetIndex:
					case EventCaching.SlicePurgeIndex:
					case EventCaching.SlicePurgeUpToIndex:
						return this.SendOperation(253, parameterDictionary, sendOptions);
					}
					bool flag4 = raiseEventOptions.TargetActors != null;
					if (flag4)
					{
						parameterDictionary.Add(252, raiseEventOptions.TargetActors);
					}
					else
					{
						bool flag5 = raiseEventOptions.InterestGroup > 0;
						if (flag5)
						{
							parameterDictionary.Add(240, raiseEventOptions.InterestGroup);
						}
						else
						{
							bool flag6 = raiseEventOptions.Receivers > ReceiverGroup.Others;
							if (flag6)
							{
								parameterDictionary.Add(246, (byte)raiseEventOptions.Receivers);
							}
						}
					}
					bool httpForward = raiseEventOptions.Flags.HttpForward;
					if (httpForward)
					{
						parameterDictionary.Add(234, raiseEventOptions.Flags.WebhookFlags);
					}
					IL_15B:;
				}
				parameterDictionary.Add(244, eventCode);
				bool flag7 = customEventContent != null;
				if (flag7)
				{
					parameterDictionary.Add(245, customEventContent);
				}
				result = this.SendOperation(253, parameterDictionary, sendOptions);
			}
			finally
			{
				this.paramDictionaryPool.Release(parameterDictionary);
			}
			return result;
		}

		public virtual bool OpSettings(bool receiveLobbyStats)
		{
			bool flag = this.DebugOut >= DebugLevel.ALL;
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "OpSettings()");
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (receiveLobbyStats)
			{
				dictionary[0] = receiveLobbyStats;
			}
			bool flag2 = dictionary.Count == 0;
			return flag2 || this.SendOperation(218, dictionary, SendOptions.SendReliable);
		}

		private readonly Pool<ParameterDictionary> paramDictionaryPool = new Pool<ParameterDictionary>(() => new ParameterDictionary(), delegate(ParameterDictionary x)
		{
			x.Clear();
		}, 1);
	}
}
